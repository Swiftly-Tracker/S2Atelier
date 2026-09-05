using System.Text;
using System.Text.RegularExpressions;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

public sealed record ProtoImportResult(bool Applicable, int TypesFound, int TypesDefined, int Errors);

public static unsafe class ProtoImport
{
    private sealed class ParsedMessage
    {
        public required string Name;
        public required bool IsZeroFieldsBase;
        public required List<string> Fields;
    }

    private static readonly Dictionary<string, string> ScalarTypes = new()
    {
        ["int32_t"] = "int",
        ["int64_t"] = "__int64",
        ["uint32_t"] = "unsigned int",
        ["uint64_t"] = "unsigned __int64",
        ["int16_t"] = "short",
        ["uint16_t"] = "unsigned short",
        ["int8_t"] = "char",
        ["uint8_t"] = "unsigned char",
        ["float"] = "float",
        ["double"] = "double",
        ["bool"] = "bool",
        ["int"] = "int",
    };

    private static readonly Regex ClassRegex = new(
        @"^class (\w+) :\r?\n\s*public ::PROTOBUF_NAMESPACE_ID::(Message|internal::ZeroFieldsBase)\b",
        RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex AnyClassRegex = new(@"^class \w+", RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex ImplRegex = new(
        @"struct Impl_ \{(.*?)\n  \};", RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex EnumRegex = new(
        @"^enum (\w+) : int \{(.*?)\n\};", RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

    private static readonly Regex HasBitsRegex = new(@"HasBits<(\d+)>", RegexOptions.Compiled);

    private static readonly Dictionary<string, (int TypesDefined, string Text)> Cache = [];

    public static ProtoImportResult Run(string headerDir)
    {
        if (!Cache.TryGetValue(headerDir, out var built))
        {
            var files = Directory.Exists(headerDir)
                ? Directory.EnumerateFiles(headerDir, "*.pb.h", SearchOption.AllDirectories).ToList()
                : [];

            var sb = new StringBuilder();
            var containerNames = new SortedSet<string>(StringComparer.Ordinal);
            var messages = new List<ParsedMessage>();
            int typesDefined = 0;

            foreach (string file in files)
            {
                string text = File.ReadAllText(file);
                typesDefined += ExtractEnums(text, sb);
                ExtractMessages(text, messages, containerNames);
            }

            if (containerNames.Count > 0 || messages.Count > 0)
            {
                EmitContainers(sb, containerNames);
            }

            foreach (var msg in messages)
            {
                EmitMessage(sb, msg);
                typesDefined++;
            }

            built = (typesDefined, sb.ToString());
            Cache[headerDir] = built;
        }

        if (built.TypesDefined == 0)
        {
            return new ProtoImportResult(true, 0, 0, 0);
        }

        void* til = IdaNative.get_idati();
        byte* input = Utf8.Allocate(built.Text);
        int errors;
        try
        {
            errors = IdaNative.parse_decls(til, input, null, 0x00000400 /* HTI_DCL */);
        }
        finally
        {
            Utf8.Free(input);
        }

        return new ProtoImportResult(true, built.TypesDefined, built.TypesDefined, errors);
    }

    // --- Enums -------------------------------------------------------------

    private static int ExtractEnums(string text, StringBuilder sb)
    {
        int count = 0;

        foreach (Match m in EnumRegex.Matches(text))
        {
            string name = m.Groups[1].Value;
            string body = m.Groups[2].Value;

            var seen = new HashSet<string>(StringComparer.Ordinal);
            sb.Append("enum ").Append(name).Append(" {\n");
            bool first = true;

            foreach (string rawMember in body.Split(','))
            {
                string member = rawMember.Trim();
                int eq = member.IndexOf('=');
                if (eq < 0)
                {
                    continue;
                }

                string memberName = member[..eq].Trim();
                string valueText = member[(eq + 1)..].Trim();
                int comment = valueText.IndexOf("//", StringComparison.Ordinal);
                if (comment >= 0)
                {
                    valueText = valueText[..comment].Trim();
                }

                if (memberName.Length == 0 || valueText.Length == 0 || !seen.Add(memberName))
                {
                    continue;
                }

                if (!first)
                {
                    sb.Append(",\n");
                }

                first = false;
                sb.Append("  ").Append(memberName).Append(" = ").Append(valueText);
            }

            sb.Append("\n};\n\n");
            count++;
        }

        return count;
    }

    // --- Messages ------------------------------------------------------------

    private static void ExtractMessages(string text, List<ParsedMessage> outMessages, SortedSet<string> containerNames)
    {
        var classMatches = ClassRegex.Matches(text);

        foreach (Match cm in classMatches)
        {
            string className = cm.Groups[1].Value;
            bool isZeroFieldsBase = cm.Groups[2].Value == "internal::ZeroFieldsBase";
            int searchStart = cm.Index + cm.Length;

            var nextAny = AnyClassRegex.Match(text, searchStart);
            int searchEnd = nextAny.Success ? nextAny.Index : text.Length;

            var implMatch = ImplRegex.Match(text, searchStart, searchEnd - searchStart);
            if (!implMatch.Success)
            {
                continue;
            }

            var fields = new List<string>();
            foreach (string rawMember in implMatch.Groups[1].Value.Split(';'))
            {
                string? decl = ClassifyMember(rawMember, containerNames);
                if (decl != null)
                {
                    fields.Add(decl);
                }
            }

            outMessages.Add(new ParsedMessage { Name = className, IsZeroFieldsBase = isZeroFieldsBase, Fields = fields });
        }
    }

    private static string? ClassifyMember(string rawLine, SortedSet<string> containerNames)
    {
        string line = rawLine.Trim();
        if (line.Length == 0)
        {
            return null;
        }

        if (line.StartsWith("mutable ", StringComparison.Ordinal))
        {
            line = line["mutable ".Length..].TrimStart();
        }

        int lastSpace = line.LastIndexOf(' ');
        if (lastSpace < 0)
        {
            return null;
        }

        string typePart = line[..lastSpace].Trim();
        string name = line[(lastSpace + 1)..].Trim();
        if (!IsIdentifier(name))
        {
            return null;
        }

        if (typePart.Contains("HasBits<", StringComparison.Ordinal))
        {
            var m = HasBitsRegex.Match(typePart);
            int n = m.Success ? int.Parse(m.Groups[1].Value) : 1;
            return $"unsigned int {name}[{n}]";
        }

        if (typePart.Contains("CachedSize", StringComparison.Ordinal) || typePart == "std::atomic<int>")
        {
            return $"int {name}";
        }

        if (typePart.Contains("ArenaStringPtr", StringComparison.Ordinal))
        {
            return $"void *{name}";
        }

        if (typePart.Contains("ExtensionSet", StringComparison.Ordinal))
        {
            return $"struct ExtensionSet_ {name}";
        }

        if (typePart.Contains("RepeatedPtrField<", StringComparison.Ordinal))
        {
            string container = $"RepeatedPtrField_{SanitizeTypeName(ExtractTemplateArg(typePart))}";
            containerNames.Add(container);
            return $"struct {container} {name}";
        }

        if (typePart.Contains("RepeatedField<", StringComparison.Ordinal))
        {
            string container = $"RepeatedField_{SanitizeTypeName(ExtractTemplateArg(typePart))}";
            containerNames.Add(container);
            return $"struct {container} {name}";
        }

        if (typePart.Contains("MapField<", StringComparison.Ordinal))
        {
            // Real MapFieldBase is polymorphic (vtable) plus a Map<K,V> - not hand-verified
            // here (unused by this SDK's proto set). Opaque pointer-sized placeholder.
            return $"void *{name}";
        }

        if (typePart.EndsWith('*'))
        {
            string inner = typePart[..^1].Trim().TrimStart(':');
            return IsIdentifier(inner) ? $"struct {inner} *{name}" : null;
        }

        return ScalarTypes.TryGetValue(typePart, out string? scalar) ? $"{scalar} {name}" : null;
    }

    private static string ExtractTemplateArg(string typePart)
    {
        int lt = typePart.IndexOf('<');
        int gt = typePart.LastIndexOf('>');
        if (lt < 0 || gt < 0 || gt <= lt)
        {
            return "unknown";
        }

        string inner = typePart[(lt + 1)..gt].Trim();
        if (inner == "std::string")
        {
            return "string";
        }

        inner = inner.TrimStart(':');
        return inner.Length == 0 ? "unknown" : inner;
    }

    private static bool IsIdentifier(string s)
    {
        if (s.Length == 0 || (!char.IsAsciiLetter(s[0]) && s[0] != '_'))
        {
            return false;
        }

        foreach (char c in s)
        {
            if (!char.IsAsciiLetterOrDigit(c) && c != '_')
            {
                return false;
            }
        }

        return true;
    }

    private static string SanitizeTypeName(string name)
    {
        var chars = new char[name.Length];
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            chars[i] = char.IsAsciiLetterOrDigit(c) || c == '_' ? c : '_';
        }

        return new string(chars);
    }

    private static void EmitMessage(StringBuilder sb, ParsedMessage msg)
    {
        sb.Append("struct ").Append(msg.Name).Append(" {\n");
        sb.Append("  void *__vftable;\n");
        sb.Append("  void *_internal_metadata_;\n");

        if (msg.IsZeroFieldsBase)
        {
            sb.Append("  int _cached_size_;\n");
        }

        foreach (string decl in msg.Fields)
        {
            sb.Append("  ").Append(decl).Append(";\n");
        }

        sb.Append("};\n\n");
    }

    // Hand-verified against protobuf-3.21.8: has_bits.h (HasBits<N> = N x uint32_t),
    // generated_message_util.h (CachedSize = std::atomic<int>, 4 bytes),
    // arenastring.h (ArenaStringPtr = TaggedStringPtr{ void* }, 8 bytes),
    // repeated_field.h (RepeatedField = {int,int,void*}, 16 bytes),
    // repeated_ptr_field.h (RepeatedPtrFieldBase = {void*,int,int,void*}, 24 bytes),
    // extension_set.h (ExtensionSet = {void*,u16,u16,void*}, 24 bytes with padding).
    private static void EmitContainers(StringBuilder sb, SortedSet<string> containerNames)
    {
        sb.Append("struct ExtensionSet_ {\n  void *arena;\n  unsigned short flat_capacity;\n" +
                  "  unsigned short flat_size;\n  void *map;\n};\n\n");

        foreach (string name in containerNames)
        {
            if (name.StartsWith("RepeatedPtrField_", StringComparison.Ordinal))
            {
                sb.Append("struct ").Append(name).Append(
                    " {\n  void *arena;\n  int current_size;\n  int total_size;\n  void *rep;\n};\n\n");
            }
            else
            {
                sb.Append("struct ").Append(name).Append(
                    " {\n  int current_size;\n  int total_size;\n  void *arena_or_elements;\n};\n\n");
            }
        }
    }
}
