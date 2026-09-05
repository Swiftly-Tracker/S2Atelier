using System.Text.RegularExpressions;

namespace S2Atelier.Ida.Codegen;

internal static partial class ConditionalFilter
{
    [GeneratedRegex(@"^\s*#\s*if(?:def|ndef)?\b", RegexOptions.Compiled)]
    private static partial Regex IfPattern();

    [GeneratedRegex(@"^\s*#\s*else\b", RegexOptions.Compiled)]
    private static partial Regex ElsePattern();

    [GeneratedRegex(@"^\s*#\s*endif\b", RegexOptions.Compiled)]
    private static partial Regex EndifPattern();

    [GeneratedRegex(@"^\s*#\s*if(?:def|ndef)?\s*\(?\s*(!?)\s*defined?\s*\(?\s*__EA64__\s*\)?\)?\s*$",
        RegexOptions.Compiled)]
    private static partial Regex Ea64IfPattern();

    private readonly record struct Frame(bool IsEa64, bool Include);

    internal static string Strip64BitOnly(string text)
    {
        var lines = text.Split('\n');
        var output = new List<string>(lines.Length);
        var stack = new Stack<Frame>();

        foreach (string line in lines)
        {
            if (Ea64IfPattern().IsMatch(line))
            {
                bool negated = Ea64IfPattern().Match(line).Groups[1].Value == "!";
                bool isIfndef = line.Contains("ifndef", StringComparison.Ordinal);
                bool inTrueBranchIsEa64 = isIfndef == negated;
                stack.Push(new Frame(IsEa64: true, Include: inTrueBranchIsEa64));
                continue;
            }

            if (IfPattern().IsMatch(line))
            {
                stack.Push(new Frame(IsEa64: false, Include: true));
                continue;
            }

            if (ElsePattern().IsMatch(line))
            {
                if (stack.Count > 0)
                {
                    var top = stack.Pop();
                    stack.Push(top with { Include = top.IsEa64 ? !top.Include : true });
                }
                continue;
            }

            if (EndifPattern().IsMatch(line))
            {
                if (stack.Count > 0)
                {
                    stack.Pop();
                }
                continue;
            }

            if (stack.All(f => f.Include))
            {
                output.Add(line);
            }
        }

        return string.Join('\n', output);
    }
}
