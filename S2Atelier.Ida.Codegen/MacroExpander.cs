using System.Text.RegularExpressions;

namespace S2Atelier.Ida.Codegen;

internal static partial class MacroExpander
{
    [GeneratedRegex(
        @"^[ \t]*#\s*define\s+(?<name>\w+)\s*\(\s*(?<param>\w+)\s*\)(?<body>(?:.*\\\r?\n)*.*)$",
        RegexOptions.Multiline | RegexOptions.Compiled)]
    private static partial Regex MacroDefinition();

    internal static string ExpandExportMacros(string text)
    {
        var additions = new List<string>();

        foreach (Match m in MacroDefinition().Matches(text))
        {
            string body = m.Groups["body"].Value.Replace("\\\r\n", "\n").Replace("\\\n", "\n");

            if (!body.Contains("ida_export", StringComparison.Ordinal))
            {
                continue;
            }

            string param = m.Groups["param"].Value;
            string name = m.Groups["name"].Value;

            string afterDefinition = text[(m.Index + m.Length)..];
            if (!Regex.IsMatch(afterDefinition, $@"\b{Regex.Escape(name)}\s*\("))
            {
                continue;
            }

            string expanded = Regex.Replace(body, $@"\b{Regex.Escape(param)}\b", "idaman");
            additions.Add(expanded);
        }

        return additions.Count == 0 ? text : text + "\n" + string.Join('\n', additions);
    }
}
