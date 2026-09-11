using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace S2Atelier.Ida.Schema;

public sealed partial class SchemaDatabase
{
    private readonly Dictionary<(string Name, string Project), SchemaClass> _classes;
    private readonly Dictionary<(string Name, string Project), SchemaEnum> _enums;
    private readonly Dictionary<string, List<SchemaClass>> _classesByName;
    private readonly Dictionary<string, List<SchemaEnum>> _enumsByName;

    private SchemaDatabase(
        Dictionary<(string Name, string Project), SchemaClass> classes,
        Dictionary<(string Name, string Project), SchemaEnum> enums)
    {
        _classes = classes;
        _enums = enums;
        _classesByName = classes.Values.GroupBy(x => x.Name, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Project, StringComparer.Ordinal).ToList(),
                StringComparer.Ordinal);
        _enumsByName = enums.Values.GroupBy(x => x.Name, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.OrderBy(y => y.Project, StringComparer.Ordinal).ToList(),
                StringComparer.Ordinal);
    }

    public IReadOnlySet<string> Projects => _classes.Values.Select(x => x.Project)
        .Concat(_enums.Values.Select(x => x.Project)).ToHashSet(StringComparer.OrdinalIgnoreCase);

    public static SchemaDatabase Load(string path)
    {
        using var stream = File.OpenRead(path);
        using var json = JsonDocument.Parse(stream, new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        });

        if (json.RootElement.ValueKind != JsonValueKind.Object)
        {
            throw new SchemaFormatException("sdk.json root must be an object.");
        }

        var classes = new Dictionary<(string, string), SchemaClass>();
        var classSignatures = new Dictionary<(string, string), string>();
        foreach (JsonElement item in RequiredArray(json.RootElement, "classes"))
        {
            SchemaClass value = ParseClass(item);
            var key = (value.Name, value.Project);
            string signature = ClassSignature(value);
            if (classSignatures.TryGetValue(key, out string? existing))
            {
                if (!StringComparer.Ordinal.Equals(existing, signature))
                {
                    throw new SchemaFormatException(
                        $"Conflicting class layout for '{value.Name}' in project '{value.Project}'.");
                }

                continue;
            }

            classes.Add(key, value);
            classSignatures.Add(key, signature);
        }

        var enums = new Dictionary<(string, string), SchemaEnum>();
        var enumSignatures = new Dictionary<(string, string), string>();
        foreach (JsonElement item in RequiredArray(json.RootElement, "enums"))
        {
            SchemaEnum value = ParseEnum(item);
            var key = (value.Name, value.Project);
            string signature = EnumSignature(value);
            if (enumSignatures.TryGetValue(key, out string? existing))
            {
                if (!StringComparer.Ordinal.Equals(existing, signature))
                {
                    throw new SchemaFormatException(
                        $"Conflicting enum definition for '{value.Name}' in project '{value.Project}'.");
                }

                continue;
            }

            enums.Add(key, value);
            enumSignatures.Add(key, signature);
        }

        return new SchemaDatabase(classes, enums);
    }

    public static string InferProject(string binaryPath)
    {
        string name = Path.GetFileNameWithoutExtension(binaryPath.Replace('\\', '/'));
        if (name.StartsWith("lib", StringComparison.OrdinalIgnoreCase) && name.Length > 3)
        {
            name = name[3..];
        }

        return name.ToLowerInvariant();
    }

    public bool HasProject(string project)
        => Projects.Contains(project);

    public SchemaSelection? Select(string requestedProject, string binaryPath)
    {
        bool automatic = requestedProject.Equals("auto", StringComparison.OrdinalIgnoreCase);
        string project = automatic ? InferProject(binaryPath) : requestedProject.Trim();

        if (!HasProject(project))
        {
            if (automatic)
            {
                return null;
            }

            throw new SchemaFormatException($"Schema project '{project}' does not exist in sdk.json.");
        }

        string canonicalProject = Projects.First(x => x.Equals(project, StringComparison.OrdinalIgnoreCase));
        var rootClasses = _classes.Values.Where(x => x.Project == canonicalProject)
            .Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
        var rootEnums = _enums.Values.Where(x => x.Project == canonicalProject)
            .Select(x => x.Name).ToHashSet(StringComparer.Ordinal);

        if (rootClasses.Count == 0 && rootEnums.Count == 0)
        {
            return automatic ? null : throw new SchemaFormatException(
                $"Schema project '{canonicalProject}' has no root types.");
        }

        var selectedClasses = new Dictionary<string, SchemaClass>(StringComparer.Ordinal);
        var selectedEnums = new Dictionary<string, SchemaEnum>(StringComparer.Ordinal);
        var synthetic = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>();

        foreach (string name in rootClasses.Concat(rootEnums).Order(StringComparer.Ordinal))
        {
            queue.Enqueue(name);
        }

        while (queue.TryDequeue(out string? requestedName))
        {
            string name = NormalizeTypeReference(requestedName);
            if (name.Length == 0 || IsBuiltin(name) || selectedClasses.ContainsKey(name) || selectedEnums.ContainsKey(name))
            {
                continue;
            }

            if (TryChooseClass(name, canonicalProject, out SchemaClass? schemaClass))
            {
                selectedClasses.Add(name, schemaClass);
                AddScopeDependencies(name, queue);
                foreach (string child in ChildTypeNames(name))
                {
                    queue.Enqueue(child);
                }
                foreach (string baseName in schemaClass.BaseClasses)
                {
                    queue.Enqueue(baseName);
                }
                foreach (SchemaField field in schemaClass.Fields)
                {
                    foreach (string dependency in FieldDependencies(field))
                    {
                        queue.Enqueue(dependency);
                    }
                }

                continue;
            }

            if (TryChooseEnum(name, canonicalProject, out SchemaEnum? schemaEnum))
            {
                selectedEnums.Add(name, schemaEnum);
                AddScopeDependencies(name, queue);
                foreach (string child in ChildTypeNames(name))
                {
                    queue.Enqueue(child);
                }
                continue;
            }

        }

        // A selected outer scope owns all of its nested schema declarations. Repeat until stable.
        bool changed;
        do
        {
            changed = false;
            foreach (string selected in selectedClasses.Keys.Concat(selectedEnums.Keys).ToArray())
            {
                foreach (string child in ChildTypeNames(selected))
                {
                    if (!selectedClasses.ContainsKey(child) && !selectedEnums.ContainsKey(child))
                    {
                        queue.Enqueue(child);
                        changed = true;
                    }
                }
            }

            while (queue.TryDequeue(out string? name))
            {
                name = NormalizeTypeReference(name);
                if (selectedClasses.ContainsKey(name) || selectedEnums.ContainsKey(name))
                {
                    continue;
                }
                if (TryChooseClass(name, canonicalProject, out SchemaClass? c))
                {
                    selectedClasses.Add(name, c);
                    foreach (string child in ChildTypeNames(name))
                    {
                        queue.Enqueue(child);
                    }
                    foreach (string dep in c.BaseClasses.Concat(c.Fields.SelectMany(FieldDependencies)))
                    {
                        queue.Enqueue(dep);
                    }
                    AddScopeDependencies(name, queue);
                }
                else if (TryChooseEnum(name, canonicalProject, out SchemaEnum? e))
                {
                    selectedEnums.Add(name, e);
                    foreach (string child in ChildTypeNames(name))
                    {
                        queue.Enqueue(child);
                    }
                    AddScopeDependencies(name, queue);
                }
            }
        } while (changed);

        foreach (string nestedName in selectedClasses.Keys.Concat(selectedEnums.Keys).ToArray())
        {
            int separator = nestedName.LastIndexOf("::", StringComparison.Ordinal);
            while (separator > 0)
            {
                string scope = nestedName[..separator];
                if (!selectedClasses.ContainsKey(scope) && !selectedEnums.ContainsKey(scope))
                {
                    selectedClasses.Add(scope,
                        new SchemaClass(scope, 0, "<synthetic>", 0, 1, false, false, [], [], true));
                    synthetic.Add(scope);
                }
                separator = scope.LastIndexOf("::", StringComparison.Ordinal);
            }
        }

        return new SchemaSelection(canonicalProject, selectedClasses, selectedEnums, rootClasses, rootEnums, synthetic);
    }

    private bool TryChooseClass(string name, string currentProject, out SchemaClass value)
    {
        value = null!;
        if (!_classesByName.TryGetValue(name, out List<SchemaClass>? candidates))
        {
            return false;
        }

        value = Choose(candidates, currentProject, x => x.Project);
        return true;
    }

    private bool TryChooseEnum(string name, string currentProject, out SchemaEnum value)
    {
        value = null!;
        if (!_enumsByName.TryGetValue(name, out List<SchemaEnum>? candidates))
        {
            return false;
        }

        value = Choose(candidates, currentProject, x => x.Project);
        return true;
    }

    private static T Choose<T>(IReadOnlyList<T> candidates, string currentProject, Func<T, string> getProject)
    {
        T? exact = candidates.FirstOrDefault(x => getProject(x) == currentProject);
        if (exact != null)
        {
            return exact;
        }

        T? server = candidates.FirstOrDefault(x => getProject(x) == "server");
        if (server != null)
        {
            return server;
        }

        return candidates.OrderBy(getProject, StringComparer.Ordinal).First();
    }

    private IEnumerable<string> ChildTypeNames(string owner)
    {
        string prefix = owner + "::";
        return _classesByName.Keys.Concat(_enumsByName.Keys)
            .Where(x => x.StartsWith(prefix, StringComparison.Ordinal) &&
                        !x[prefix.Length..].Contains("::", StringComparison.Ordinal))
            .Order(StringComparer.Ordinal);
    }

    private static void AddScopeDependencies(string name, Queue<string> queue)
    {
        int index = name.LastIndexOf("::", StringComparison.Ordinal);
        while (index > 0)
        {
            queue.Enqueue(name[..index]);
            index = name.LastIndexOf("::", index - 1, StringComparison.Ordinal);
        }
    }

    public static IEnumerable<string> FieldDependencies(SchemaField field)
    {
        if (field.Kind is SchemaFieldKind.Reference or SchemaFieldKind.Pointer or SchemaFieldKind.FixedArray)
        {
            yield return StripArray(field.TypeName);
        }

        if (field.TemplateArguments != null)
        {
            foreach (SchemaTemplateArgument argument in field.TemplateArguments)
            {
                if (!argument.IsLiteral)
                {
                    foreach (Match match in IdentifierRegex().Matches(argument.Text))
                    {
                        yield return match.Value;
                    }
                }
            }
        }
    }

    public static string NormalizeTypeReference(string text)
    {
        string value = text.Trim();
        value = PrefixRegex().Replace(value, string.Empty);
        value = value.TrimEnd('*', '&', ' ');
        return StripArray(value);
    }

    public static string StripArray(string text)
    {
        int bracket = text.IndexOf('[');
        return (bracket >= 0 ? text[..bracket] : text).Trim();
    }

    public static bool IsBuiltin(string name)
        => name is "void" or "bool" or "char" or "signed char" or "unsigned char" or
            "short" or "unsigned short" or "int" or "unsigned int" or "long" or "unsigned long" or
            "long long" or "unsigned long long" or "float" or "double" or
            "int8" or "uint8" or "int16" or "uint16" or "int32" or "uint32" or "int64" or "uint64" or
            "float32" or "float64" or "size_t" or "uintptr_t" or "intptr_t";

    private static SchemaClass ParseClass(JsonElement item)
    {
        string name = RequiredString(item, "name");
        string project = RequiredString(item, "project");
        int size = RequiredInt(item, "size");
        int alignment = RequiredInt(item, "alignment");
        ValidateType(name, project, size, alignment);

        var bases = OptionalArray(item, "base_classes")
            .Select(x => x.GetString() ?? throw new SchemaFormatException($"Null base class on '{name}'."))
            .ToArray();
        var fields = OptionalArray(item, "fields").Select(x => ParseField(x, name)).ToArray();
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].Size >= 0)
            {
                continue;
            }
            int nextOffset = fields.Skip(i + 1).Where(x => x.Kind != SchemaFieldKind.Bitfield && x.Offset >= fields[i].Offset)
                .Select(x => x.Offset).DefaultIfEmpty(size).Min();
            int inferredSize = nextOffset - fields[i].Offset;
            if (inferredSize <= 0)
            {
                throw new SchemaFormatException($"Cannot infer unknown field size for '{name}::{fields[i].Name}'.");
            }
            fields[i] = fields[i] with
            {
                Size = inferredSize,
                Alignment = fields[i].Alignment == 255 ? alignment : fields[i].Alignment,
            };
        }

        int declaredFields = OptionalInt(item, "fields_count", fields.Length);
        int declaredBases = OptionalInt(item, "base_classes_count", bases.Length);
        if (declaredFields != fields.Length || declaredBases != bases.Length)
        {
            throw new SchemaFormatException($"Count metadata is inconsistent for class '{name}' in '{project}'.");
        }

        return new SchemaClass(name, RequiredUInt64(item, "name_hash"), project, size, alignment,
            OptionalBool(item, "is_struct"), OptionalBool(item, "has_chainer"), bases, fields);
    }

    private static SchemaField ParseField(JsonElement item, string owner)
    {
        string rawKind = RequiredString(item, "kind");
        SchemaFieldKind kind = rawKind switch
        {
            "ref" => SchemaFieldKind.Reference,
            "atomic" => SchemaFieldKind.Atomic,
            "fixed_array" => SchemaFieldKind.FixedArray,
            "ptr" => SchemaFieldKind.Pointer,
            "bitfield" => SchemaFieldKind.Bitfield,
            _ => throw new SchemaFormatException($"Unknown field kind '{rawKind}' on '{owner}'."),
        };

        string name = RequiredString(item, "name");
        int size = RequiredInt(item, "size");
        int alignment = RequiredInt(item, "alignment");
        int offset = RequiredInt(item, "offset");
        if (size < -1 || offset < 0 || alignment <= 0)
        {
            throw new SchemaFormatException($"Invalid layout for field '{owner}::{name}'.");
        }

        var templateArguments = new List<SchemaTemplateArgument>();
        foreach (JsonElement argument in OptionalArray(item, "template"))
        {
            if (argument.ValueKind == JsonValueKind.String)
            {
                templateArguments.Add(new SchemaTemplateArgument(argument.GetString()!, false));
            }
            else if (argument.ValueKind == JsonValueKind.Object &&
                     OptionalString(argument, "type") == "literal")
            {
                JsonElement literal = RequiredProperty(argument, "value");
                templateArguments.Add(new SchemaTemplateArgument(literal.GetRawText(), true));
            }
            else
            {
                throw new SchemaFormatException($"Unsupported template argument on '{owner}::{name}'.");
            }
        }

        return new SchemaField(
            name,
            RequiredUInt64(item, "name_hash"),
            kind,
            OptionalString(item, "type") ?? string.Empty,
            offset,
            size,
            alignment,
            OptionalBool(item, "networked"),
            OptionalString(item, "templated"),
            templateArguments,
            OptionalInt(item, "element_count", 0),
            OptionalInt(item, "element_size", 0),
            OptionalInt(item, "element_alignment", 0),
            OptionalInt(item, "count", 0));
    }

    private static SchemaEnum ParseEnum(JsonElement item)
    {
        string name = RequiredString(item, "name");
        string project = RequiredString(item, "project");
        int size = RequiredInt(item, "size");
        int alignment = RequiredInt(item, "alignment");
        ValidateType(name, project, size, alignment);
        if (size is not (1 or 2 or 4 or 8))
        {
            throw new SchemaFormatException($"Enum '{name}' has unsupported width {size}.");
        }

        var fields = OptionalArray(item, "fields").Select(x => ParseEnumValue(x, name)).ToArray();
        if (OptionalInt(item, "fields_count", fields.Length) != fields.Length)
        {
            throw new SchemaFormatException($"Count metadata is inconsistent for enum '{name}'.");
        }

        return new SchemaEnum(name, OptionalUInt64(item, "name_hash", 0), project, size, alignment, fields);
    }

    private static SchemaEnumValue ParseEnumValue(JsonElement item, string owner)
    {
        string name = RequiredString(item, "name");
        JsonElement value = RequiredProperty(item, "value");
        if (value.ValueKind != JsonValueKind.Number)
        {
            throw new SchemaFormatException($"Enum value '{owner}::{name}' is not an integer.");
        }
        if (value.TryGetInt64(out long signed))
        {
            return new SchemaEnumValue(name, signed, unchecked((ulong)signed), false);
        }
        if (value.TryGetUInt64(out ulong unsigned))
        {
            return new SchemaEnumValue(name, unchecked((long)unsigned), unsigned, true);
        }
        throw new SchemaFormatException($"Enum value '{owner}::{name}' is outside 64-bit range.");
    }

    private static void ValidateType(string name, string project, int size, int alignment)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(project) || size < 0 || alignment <= 0)
        {
            throw new SchemaFormatException($"Invalid schema type '{name}' in project '{project}'.");
        }
    }

    private static string ClassSignature(SchemaClass value)
    {
        var text = new StringBuilder();
        text.Append(value.Size).Append('|').Append(value.Alignment).Append('|').Append(value.IsStruct)
            .Append('|').Append(value.HasChainer).Append('|').AppendJoin(',', value.BaseClasses);
        foreach (SchemaField field in value.Fields)
        {
            text.Append(';').Append(field.Name).Append('|').Append(field.NameHash.ToString(CultureInfo.InvariantCulture))
                .Append('|').Append(field.Kind).Append('|').Append(field.TypeName).Append('|').Append(field.Offset)
                .Append('|').Append(field.Size).Append('|').Append(field.Alignment).Append('|').Append(field.Networked)
                .Append('|').Append(field.TemplatedType).Append('|').Append(field.ElementCount).Append('|')
                .Append(field.ElementSize).Append('|').Append(field.ElementAlignment).Append('|').Append(field.BitCount);
            if (field.TemplateArguments != null)
            {
                foreach (SchemaTemplateArgument argument in field.TemplateArguments)
                {
                    text.Append('|').Append(argument.IsLiteral ? 'L' : 'T').Append(':').Append(argument.Text);
                }
            }
        }
        return text.ToString();
    }

    private static string EnumSignature(SchemaEnum value)
        => $"{value.Size}|{value.Alignment}|" + string.Join(';', value.Fields.Select(x =>
            $"{x.Name}|{x.IsUnsigned}|{x.UnsignedValue.ToString(CultureInfo.InvariantCulture)}"));

    private static JsonElement RequiredProperty(JsonElement item, string name)
        => item.TryGetProperty(name, out JsonElement value)
            ? value
            : throw new SchemaFormatException($"Missing required property '{name}'.");

    private static IEnumerable<JsonElement> RequiredArray(JsonElement item, string name)
    {
        JsonElement value = RequiredProperty(item, name);
        if (value.ValueKind != JsonValueKind.Array)
        {
            throw new SchemaFormatException($"Property '{name}' must be an array.");
        }
        return value.EnumerateArray();
    }

    private static IEnumerable<JsonElement> OptionalArray(JsonElement item, string name)
    {
        if (!item.TryGetProperty(name, out JsonElement value) || value.ValueKind == JsonValueKind.Null)
        {
            return [];
        }
        if (value.ValueKind != JsonValueKind.Array)
        {
            throw new SchemaFormatException($"Property '{name}' must be an array.");
        }
        return value.EnumerateArray().ToArray();
    }

    private static string RequiredString(JsonElement item, string name)
        => OptionalString(item, name) ?? throw new SchemaFormatException($"Missing string property '{name}'.");

    private static string? OptionalString(JsonElement item, string name)
        => item.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static int RequiredInt(JsonElement item, string name)
        => item.TryGetProperty(name, out JsonElement value) && value.TryGetInt32(out int result)
            ? result
            : throw new SchemaFormatException($"Missing integer property '{name}'.");

    private static int OptionalInt(JsonElement item, string name, int fallback)
        => item.TryGetProperty(name, out JsonElement value) && value.TryGetInt32(out int result) ? result : fallback;

    private static ulong RequiredUInt64(JsonElement item, string name)
        => item.TryGetProperty(name, out JsonElement value) && value.TryGetUInt64(out ulong result)
            ? result
            : throw new SchemaFormatException($"Missing unsigned 64-bit property '{name}'.");

    private static ulong OptionalUInt64(JsonElement item, string name, ulong fallback)
        => item.TryGetProperty(name, out JsonElement value) && value.TryGetUInt64(out ulong result) ? result : fallback;

    private static bool OptionalBool(JsonElement item, string name)
        => item.TryGetProperty(name, out JsonElement value) && value.ValueKind is JsonValueKind.True or JsonValueKind.False &&
           value.GetBoolean();

    [GeneratedRegex(@"^(?:const\s+|class\s+|struct\s+|enum\s+)+", RegexOptions.CultureInvariant)]
    private static partial Regex PrefixRegex();

    [GeneratedRegex(@"[A-Za-z_][A-Za-z0-9_]*(?:::[A-Za-z_][A-Za-z0-9_]*)*", RegexOptions.CultureInvariant)]
    private static partial Regex IdentifierRegex();
}
