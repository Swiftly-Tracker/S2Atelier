using System.Text.RegularExpressions;

namespace S2Atelier.Ida.Codegen;

internal readonly record struct NativeDecl(string Name, string ReturnType, IReadOnlyList<string> ParamTypes);

internal readonly record struct ScanResult(string Name, NativeDecl? Decl, string? ExcludeReason)
{
    internal bool IsExcluded => ExcludeReason != null;
}

internal static partial class HeaderScanner
{
    [GeneratedRegex(
        @"idaman\s+(?<ret>[^;(]+?)\s*ida_export\s+(?<name>\w+)\s*\((?<params>[^)]*)\)\s*;",
        RegexOptions.Compiled)]
    private static partial Regex DeclPattern();

    [GeneratedRegex(@"[A-Za-z_]\w*\s*$", RegexOptions.Compiled)]
    private static partial Regex TrailingIdentifier();

    [GeneratedRegex(@"\(\s*\*", RegexOptions.Compiled)]
    private static partial Regex FunctionPointerParam();

    internal static IEnumerable<ScanResult> ScanAll(string text)
    {
        foreach (Match m in DeclPattern().Matches(text))
        {
            string name = m.Groups["name"].Value;
            string returnType = CleanReturnType(m.Groups["ret"].Value);
            string paramsText = m.Groups["params"].Value.Trim();

            if (paramsText.Contains("...", StringComparison.Ordinal))
            {
                yield return new ScanResult(name, null, "variadic parameter list");
                continue;
            }

            var paramTypes = new List<string>();
            bool hasFunctionPointerParam = false;

            if (paramsText.Length > 0 && paramsText != "void")
            {
                foreach (string raw in SplitTopLevel(paramsText))
                {
                    if (FunctionPointerParam().IsMatch(raw))
                    {
                        hasFunctionPointerParam = true;
                        break;
                    }

                    paramTypes.Add(ExtractParamType(raw));
                }
            }

            if (hasFunctionPointerParam)
            {
                yield return new ScanResult(name, null, "function-pointer parameter");
                continue;
            }

            yield return new ScanResult(name, new NativeDecl(name, returnType, paramTypes), null);
        }
    }

    private static IEnumerable<string> SplitTopLevel(string paramsText)
    {
        int depth = 0;
        int start = 0;

        for (int i = 0; i < paramsText.Length; i++)
        {
            char c = paramsText[i];
            if (c is '<' or '(') depth++;
            else if (c is '>' or ')') depth--;
            else if (c == ',' && depth == 0)
            {
                yield return paramsText[start..i];
                start = i + 1;
            }
        }

        yield return paramsText[start..];
    }

    private static readonly string[] ReturnTypeNoise = ["THREAD_SAFE", "DEPRECATED", "NORETURN"];

    private static string CleanReturnType(string raw)
    {
        string s = raw;
        foreach (string noise in ReturnTypeNoise)
        {
            s = s.Replace(noise, "", StringComparison.Ordinal);
        }
        return s.Trim();
    }

    private static string ExtractParamType(string raw)
    {
        string s = raw.Trim();

        int eq = s.IndexOf('=');
        if (eq >= 0)
        {
            s = s[..eq].Trim();
        }

        var identifier = TrailingIdentifier().Match(s);
        if (identifier is { Success: true, Index: > 0 })
        {
            s = s[..identifier.Index].TrimEnd();
        }

        return s;
    }
}
