using System.Text.RegularExpressions;

namespace S2Atelier.Ida.Codegen;

internal readonly record struct TypeResolution(string? CSharpType, string? ExcludeReason)
{
    internal bool IsExcluded => ExcludeReason != null;
    internal static TypeResolution Ok(string cs) => new(cs, null);
    internal static TypeResolution Exclude(string reason) => new(null, reason);
}

internal sealed partial class TypeResolver(Dictionary<string, string> aliases, HashSet<string> enums)
{
    internal static readonly Dictionary<string, string> HandMirrored = new()
    {
        ["auto_display_t"] = "AutoDisplay",
        ["tinfo_t"] = "TypeInfo",
        ["qstring"] = "QString",
    };

    private static readonly Dictionary<string, string> Primitives = new()
    {
        ["void"] = "void",
        ["bool"] = "byte",
        ["char"] = "byte",
        ["signed char"] = "sbyte",
        ["unsigned char"] = "byte",
        ["uchar"] = "byte",
        ["short"] = "short",
        ["short int"] = "short",
        ["unsigned short"] = "ushort",
        ["unsigned short int"] = "ushort",
        ["int"] = "int",
        ["signed int"] = "int",
        ["signed"] = "int",
        ["unsigned int"] = "uint",
        ["unsigned"] = "uint",
        ["long long"] = "long",
        ["signed long long"] = "long",
        ["unsigned long long"] = "ulong",
        ["float"] = "float",
        ["double"] = "double",
        ["int8"] = "sbyte",
        ["uint8"] = "byte",
        ["int16"] = "short",
        ["uint16"] = "ushort",
        ["int32"] = "int",
        ["uint32"] = "uint",
        ["int64"] = "long",
        ["uint64"] = "ulong",
        ["size_t"] = "nuint",
        ["ssize_t"] = "nint",
        ["ptrdiff_t"] = "nint",
    };

    [GeneratedRegex(@"\bconst\b|\bvolatile\b|\bstruct\b|\bclass\b|\btypename\b|\benum\b|\bIEEE_\b", RegexOptions.Compiled)]
    private static partial Regex NoiseKeyword();

    internal TypeResolution Resolve(string rawType)
    {
        string text = NoiseKeyword().Replace(rawType, " ");
        int pointerDepth = 0;
        var visited = new HashSet<string>(StringComparer.Ordinal);

        while (true)
        {
            text = Normalize(text);

            while (text.EndsWith('*') || text.EndsWith('&'))
            {
                pointerDepth++;
                text = Normalize(text[..^1]);
            }

            if (text.Length == 0)
            {
                return TypeResolution.Exclude("empty base type after stripping qualifiers");
            }

            if (Primitives.TryGetValue(text, out string? prim))
            {
                return Emit(prim, pointerDepth);
            }

            if (enums.Contains(text))
            {
                return Emit("int", pointerDepth);
            }

            if (HandMirrored.TryGetValue(text, out string? mirror))
            {
                return pointerDepth switch
                {
                    0 => TypeResolution.Exclude($"'{text}' used by value has no safe by-value mapping"),
                    1 => TypeResolution.Ok(mirror + "*"),
                    _ => TypeResolution.Ok("void*"),
                };
            }

            if (!visited.Add(text))
            {
                return pointerDepth > 0
                    ? TypeResolution.Ok("void*")
                    : TypeResolution.Exclude($"cyclic typedef chasing '{text}'");
            }

            if (aliases.TryGetValue(text, out string? target))
            {
                text = target;
                continue;
            }

            return pointerDepth > 0
                ? TypeResolution.Ok("void*")
                : TypeResolution.Exclude($"'{text}' is not a known primitive or typedef, used by value");
        }
    }

    private static TypeResolution Emit(string primitive, int pointerDepth)
        => TypeResolution.Ok(primitive == "void" && pointerDepth == 0
            ? "void"
            : primitive + new string('*', pointerDepth));

    private static string Normalize(string text)
        => string.Join(' ', text.Split([' ', '\t', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries));
}
