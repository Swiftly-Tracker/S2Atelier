using System.Text;
using System.Text.RegularExpressions;

namespace S2Atelier.Ida;

public sealed record ValveInterfaceDefinition(
    string VersionMacro,
    string Version,
    string ClassName,
    string GlobalName,
    string? DefinitionHeader);

public sealed record ValveInterfaceTableSlot(
    string Version,
    ValveInterfaceDefinition? Definition);

public sealed partial class ValveInterfaceCatalog
{
    internal const string CvarVersion = "VEngineCvar007";

    private ValveInterfaceCatalog(
        IReadOnlyList<ValveInterfaceDefinition> entries,
        IReadOnlyList<ValveInterfaceTableSlot> tableSlots)
    {
        Entries = entries;
        TableSlots = tableSlots;
        Versions = entries.Select(x => x.Version)
            .Concat(tableSlots.Select(x => x.Version))
            .ToHashSet(StringComparer.Ordinal);
    }

    public IReadOnlyList<ValveInterfaceDefinition> Entries { get; }

    public IReadOnlyList<ValveInterfaceTableSlot> TableSlots { get; }

    public IReadOnlySet<string> Versions { get; }

    public static ValveInterfaceCatalog Load(string hl2SdkPath)
    {
        string root = Path.GetFullPath(hl2SdkPath);
        string interfaceHeader = Path.Combine(root, "public", "interfaces", "interfaces.h");
        if (!File.Exists(interfaceHeader))
        {
            throw new ValveInterfaceImportException(
                $"HL2SDK interface catalog does not exist: '{interfaceHeader}'.");
        }

        var headers = ReadHeaders(root);

        string relativeInterfaceHeader = NormalizePath(Path.GetRelativePath(root, interfaceHeader));
        string headerText = headers.TryGetValue(relativeInterfaceHeader, out string? loaded)
            ? loaded
            : File.ReadAllText(interfaceHeader);
        string interfaceSource = Path.Combine(root, "interfaces", "interfaces.cpp");
        return Parse(headerText, File.Exists(interfaceSource) ? File.ReadAllText(interfaceSource) : null, headers);
    }

    internal static Dictionary<string, string> ReadHeaders(string root)
    {
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string subtree in new[] { "public", "game" })
        {
            string directory = Path.Combine(root, subtree);
            if (!Directory.Exists(directory))
            {
                continue;
            }
            foreach (string path in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
                         .Where(IsHeader))
            {
                string relative = NormalizePath(Path.GetRelativePath(root, path));
                headers[relative] = File.ReadAllText(path);
            }
        }

        return headers;
    }

    public static ValveInterfaceCatalog Parse(
        string interfaceHeader,
        string? interfaceSource,
        IReadOnlyDictionary<string, string> headers)
    {
        ArgumentNullException.ThrowIfNull(interfaceHeader);
        ArgumentNullException.ThrowIfNull(headers);

        var macroValues = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        foreach ((string name, string value) in ParseLiteralMacros(interfaceHeader))
        {
            if (!macroValues.TryGetValue(name, out HashSet<string>? values))
            {
                values = new HashSet<string>(StringComparer.Ordinal);
                macroValues.Add(name, values);
            }
            values.Add(value);
        }
        Dictionary<string, string> macros = macroValues
            .Where(x => x.Value.Count == 1)
            .ToDictionary(x => x.Key, x => x.Value.Single(), StringComparer.Ordinal);

        var entries = ParseDeclarations(interfaceHeader)
            .Select(x => new ValveInterfaceDefinition(
                x.Macro, x.Version, x.ClassName, x.GlobalName,
                FindDefinitionHeader(x.ClassName, headers)))
            .ToArray();
        if (entries.Length == 0)
        {
            throw new ValveInterfaceImportException(
                "HL2SDK public/interfaces/interfaces.h contains no versioned DECLARE_TIER*_INTERFACE declarations.");
        }

        // This known interface must not depend on the reverse-engineered SDK
        // interfaces.cpp table, which can contain an extra cvar alias slot.
        var cvarHeaders = headers.Where(x =>
                NormalizePath(x.Key).Equals("public/icvar.h", StringComparison.OrdinalIgnoreCase))
            .ToDictionary(x => NormalizePath(x.Key), x => x.Value, StringComparer.OrdinalIgnoreCase);
        var cvarDefinition = new ValveInterfaceDefinition(
            "CVAR_INTERFACE_VERSION", CvarVersion, "ICvar", "g_pCVar",
            FindDefinitionHeader("ICvar", cvarHeaders));
        entries = entries.Where(x => x.Version != CvarVersion).Append(cvarDefinition).ToArray();

        var tableSlots = new List<ValveInterfaceTableSlot>();
        if (!string.IsNullOrWhiteSpace(interfaceSource))
        {
            string source = StripCommentsAndLiterals(interfaceSource!);
            Dictionary<string, string> sourceGlobals = ParseSourceGlobals(source);
            string table = ExtractInterfaceTable(source);
            foreach (Match row in TableRowRegex().Matches(table))
            {
                string macro = row.Groups["macro"].Value;
                string global = row.Groups["global"].Value;
                if (!macros.TryGetValue(macro, out string? version))
                {
                    continue;
                }
                string mappingMacro = version == CvarVersion ? cvarDefinition.VersionMacro : macro;
                ValveInterfaceDefinition? definition = entries.FirstOrDefault(x =>
                    x.VersionMacro.Equals(mappingMacro, StringComparison.Ordinal) &&
                    x.GlobalName.Equals(global, StringComparison.Ordinal));
                if (definition == null && sourceGlobals.TryGetValue(global, out string? className))
                {
                    // interfaces.cpp also defines SDK aliases (e.g. ICvar *cvar, *g_pCVar).
                    // Require a matching explicit pointer declaration, not just the table row.
                    ValveInterfaceDefinition[] candidates = entries.Where(x =>
                        x.VersionMacro == mappingMacro && x.ClassName == className).ToArray();
                    if (candidates.Length == 1)
                    {
                        definition = candidates[0] with { GlobalName = global };
                    }
                }
                tableSlots.Add(new ValveInterfaceTableSlot(version, definition));
            }
        }

        return new ValveInterfaceCatalog(entries, tableSlots);
    }

    private static Dictionary<string, string> ParseSourceGlobals(string source)
    {
        var declarations = new List<KeyValuePair<string, string>>();
        int offset = 0;
        int depth = 0;
        foreach (Match declaration in SourceGlobalRegex().Matches(source))
        {
            for (; offset < declaration.Index; offset++)
            {
                if (source[offset] == '{') depth++;
                else if (source[offset] == '}') depth--;
            }
            if (depth != 0) continue; // Do not mistake members or local variables for globals.
            string className = declaration.Groups["class"].Value;
            foreach (string declarator in declaration.Groups["pointers"].Value.Split(','))
            {
                declarations.Add(KeyValuePair.Create(declarator.Trim().TrimStart('*').Trim(), className));
            }
        }
        return declarations.GroupBy(x => x.Key, StringComparer.Ordinal)
            .Where(x => x.Select(d => d.Value).Distinct(StringComparer.Ordinal).Count() == 1)
            .ToDictionary(x => x.Key, x => x.First().Value, StringComparer.Ordinal);
    }

    private static IReadOnlyList<KeyValuePair<string, string>> ParseLiteralMacros(string text)
    {
        var result = new List<KeyValuePair<string, string>>();
        foreach (Match match in LiteralMacroRegex().Matches(text))
        {
            result.Add(KeyValuePair.Create(match.Groups["macro"].Value, match.Groups["value"].Value));
        }
        return result;
    }

    private static IReadOnlyList<RawDeclaration> ParseDeclarations(string text)
    {
        var result = new List<RawDeclaration>();
        string? pendingMacro = null;
        string? pendingVersion = null;

        foreach (string rawLine in text.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n'))
        {
            Match literal = LiteralMacroLineRegex().Match(rawLine);
            if (literal.Success)
            {
                pendingMacro = literal.Groups["macro"].Value;
                pendingVersion = literal.Groups["value"].Value;
                continue;
            }

            Match declaration = DeclareRegex().Match(rawLine);
            if (declaration.Success && !declaration.Groups["class"].Value.StartsWith('_') &&
                pendingMacro != null && pendingVersion != null)
            {
                result.Add(new RawDeclaration(
                    pendingMacro,
                    pendingVersion,
                    declaration.Groups["class"].Value,
                    declaration.Groups["global"].Value));
                pendingMacro = null;
                pendingVersion = null;
                continue;
            }

            string trimmed = rawLine.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith("//", StringComparison.Ordinal))
            {
                continue;
            }
            pendingMacro = null;
            pendingVersion = null;
        }

        return result;
    }

    internal static string? FindDefinitionHeader(
        string className,
        IReadOnlyDictionary<string, string> headers)
    {
        int separator = className.LastIndexOf("::", StringComparison.Ordinal);
        string leaf = separator < 0 ? className : className[(separator + 2)..];
        string pattern = $@"(?<![A-Za-z0-9_])(?:abstract_class|class|struct)(?:[ \t]+[A-Za-z_][A-Za-z0-9_]*)*[ \t]+{Regex.Escape(leaf)}\b[^;{{]*{{";
        // A scoped class (GCSDK::CJob) is only declared by a header that opens its namespace or outer class;
        // another class of the same name (tier1's CJob) is not it.
        string? scope = separator < 0 ? null : className[..separator];
        if (scope?.LastIndexOf("::", StringComparison.Ordinal) is int inner and >= 0) scope = scope[(inner + 2)..];
        string? scopePattern = scope == null
            ? null
            : $@"(?<![A-Za-z0-9_])(?:namespace|class|struct)[ \t\r\n]+(?:[A-Za-z_][A-Za-z0-9_]*[ \t]+)*{Regex.Escape(scope)}\b";
        var matches = new List<string>();
        foreach ((string path, string source) in headers)
        {
            if (!source.Contains(leaf, StringComparison.Ordinal)) continue;
            string sanitized = StripCommentsAndLiterals(source);
            if (Regex.IsMatch(sanitized, pattern, RegexOptions.CultureInvariant) &&
                (scopePattern == null || Regex.IsMatch(sanitized, scopePattern, RegexOptions.CultureInvariant)))
            {
                matches.Add(NormalizePath(path));
            }
        }
        return matches.Distinct(StringComparer.OrdinalIgnoreCase).Take(2).Count() == 1
            ? matches[0]
            : null;
    }

    private static string StripCommentsAndLiterals(string source)
    {
        var output = new StringBuilder(source.Length);
        bool lineComment = false;
        bool blockComment = false;
        bool str = false;
        bool chr = false;
        bool escaped = false;

        for (int i = 0; i < source.Length; i++)
        {
            char value = source[i];
            char next = i + 1 < source.Length ? source[i + 1] : '\0';
            if (lineComment)
            {
                if (value is '\r' or '\n')
                {
                    lineComment = false;
                    output.Append(value);
                }
                else
                {
                    output.Append(' ');
                }
                continue;
            }
            if (blockComment)
            {
                if (value == '*' && next == '/')
                {
                    output.Append("  ");
                    i++;
                    blockComment = false;
                }
                else
                {
                    output.Append(value is '\r' or '\n' ? value : ' ');
                }
                continue;
            }
            if (escaped)
            {
                output.Append(' ');
                escaped = false;
                continue;
            }
            if ((str || chr) && value == '\\')
            {
                output.Append(' ');
                escaped = true;
                continue;
            }
            if (str)
            {
                output.Append(value is '\r' or '\n' ? value : ' ');
                if (value == '"') str = false;
                continue;
            }
            if (chr)
            {
                output.Append(value is '\r' or '\n' ? value : ' ');
                if (value == '\'') chr = false;
                continue;
            }
            if (value == '/' && next == '/')
            {
                output.Append("  ");
                i++;
                lineComment = true;
                continue;
            }
            if (value == '/' && next == '*')
            {
                output.Append("  ");
                i++;
                blockComment = true;
                continue;
            }
            if (value == '"')
            {
                output.Append(' ');
                str = true;
                continue;
            }
            if (value == '\'')
            {
                output.Append(' ');
                chr = true;
                continue;
            }
            output.Append(value);
        }
        return output.ToString();
    }

    private static string ExtractInterfaceTable(string source)
    {
        int marker = source.IndexOf("g_pInterfaceGlobals[]", StringComparison.Ordinal);
        if (marker < 0)
        {
            return string.Empty;
        }
        int open = source.IndexOf('{', marker);
        int close = open < 0 ? -1 : source.IndexOf("};", open, StringComparison.Ordinal);
        return open >= 0 && close > open ? source[open..close] : string.Empty;
    }

    private static string NormalizePath(string path) => path.Replace('\\', '/');

    private static bool IsHeader(string path)
        => Path.GetExtension(path).ToLowerInvariant() is ".h" or ".hh" or ".hpp" or ".hxx";

    private sealed record RawDeclaration(string Macro, string Version, string ClassName, string GlobalName);

    [GeneratedRegex(@"(?m)^[ \t]*#[ \t]*define[ \t]+(?<macro>[A-Za-z_][A-Za-z0-9_]*)[ \t]+""(?<value>[^""\r\n]+)""")]
    private static partial Regex LiteralMacroRegex();

    [GeneratedRegex(@"^[ \t]*#[ \t]*define[ \t]+(?<macro>[A-Za-z_][A-Za-z0-9_]*)[ \t]+""(?<value>[^""\r\n]+)""", RegexOptions.CultureInvariant)]
    private static partial Regex LiteralMacroLineRegex();

    [GeneratedRegex(@"DECLARE_TIER[1-4]_INTERFACE\s*\(\s*(?<class>[A-Za-z_][A-Za-z0-9_]*(?:::[A-Za-z_][A-Za-z0-9_]*)*)\s*,\s*(?<global>[A-Za-z_][A-Za-z0-9_]*)\s*\)", RegexOptions.CultureInvariant)]
    private static partial Regex DeclareRegex();

    [GeneratedRegex(@"(?m)^[ \t]*(?:(?:extern|static)\s+)?(?<class>[A-Za-z_][A-Za-z0-9_]*(?:::[A-Za-z_][A-Za-z0-9_]*)*)\s+(?<pointers>\*\s*[A-Za-z_][A-Za-z0-9_]*(?:\s*,\s*\*\s*[A-Za-z_][A-Za-z0-9_]*)*)\s*;", RegexOptions.CultureInvariant)]
    private static partial Regex SourceGlobalRegex();

    [GeneratedRegex(@"\{\s*(?<macro>[A-Za-z_][A-Za-z0-9_]*)\s*,\s*&(?<global>[A-Za-z_][A-Za-z0-9_]*)\s*\}", RegexOptions.CultureInvariant)]
    private static partial Regex TableRowRegex();
}
