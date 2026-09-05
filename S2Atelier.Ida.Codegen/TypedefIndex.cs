using System.Text.RegularExpressions;

namespace S2Atelier.Ida.Codegen;

internal static partial class TypedefIndex
{
    [GeneratedRegex(@"^\s*typedef\s+([A-Za-z_][\w\s\*]*?)\s+([A-Za-z_]\w*)\s*;", RegexOptions.Compiled)]
    private static partial Regex TypedefPattern();

    [GeneratedRegex(@"^\s*using\s+([A-Za-z_]\w*)\s*=\s*([A-Za-z_][\w\s\*]*?)\s*;", RegexOptions.Compiled)]
    private static partial Regex UsingPattern();

    [GeneratedRegex(@"^\s*enum\s+(?:class\s+)?([A-Za-z_]\w*)\b", RegexOptions.Compiled)]
    private static partial Regex EnumPattern();

    internal static (Dictionary<string, string> Aliases, HashSet<string> Enums) Build(IEnumerable<string> filteredHeaderTexts)
    {
        var aliases = new Dictionary<string, string>();
        var enums = new HashSet<string>(StringComparer.Ordinal);

        foreach (string text in filteredHeaderTexts)
        {
            foreach (string rawLine in text.Split('\n'))
            {
                string line = rawLine.TrimEnd('\r');

                var td = TypedefPattern().Match(line);
                if (td.Success)
                {
                    string baseType = td.Groups[1].Value.Trim();
                    string name = td.Groups[2].Value;

                    if (!string.Equals(name, baseType, StringComparison.Ordinal))
                    {
                        aliases.TryAdd(name, baseType);
                    }
                    continue;
                }

                var us = UsingPattern().Match(line);
                if (us.Success)
                {
                    string name = us.Groups[1].Value;
                    string baseType = us.Groups[2].Value.Trim();
                    if (!string.Equals(name, baseType, StringComparison.Ordinal))
                    {
                        aliases.TryAdd(name, baseType);
                    }
                    continue;
                }

                var en = EnumPattern().Match(line);
                if (en.Success)
                {
                    enums.Add(en.Groups[1].Value);
                }
            }
        }

        return (aliases, enums);
    }
}
