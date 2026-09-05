using System.Text;

namespace S2Atelier.Ida.Codegen;

internal readonly record struct EmittedSymbol(string Name, string Module, string ReturnType, IReadOnlyList<string> ParamTypes)
{
    internal string PointerType => ParamTypes.Count == 0
        ? $"delegate* unmanaged[Cdecl]<{ReturnType}>"
        : $"delegate* unmanaged[Cdecl]<{string.Join(", ", ParamTypes)}, {ReturnType}>";
}

internal static class Emitter
{
    internal static void EmitSurface(string outDir, IReadOnlyList<EmittedSymbol> symbols)
    {
        var sb = new StringBuilder();
        sb.Append("#nullable enable\n\nusing S2Atelier.Ida;\n\nnamespace S2Atelier.Ida.Generated;\n\n");
        sb.Append("public static unsafe partial class IdaNative\n{\n");

        foreach (var s in symbols)
        {
            sb.Append($"    internal static {s.PointerType} _{s.Name};\n");
        }

        sb.Append('\n');

        foreach (var s in symbols)
        {
            string paramList = string.Join(", ", s.ParamTypes.Select((t, i) => $"{t} p{i}"));
            string argList = string.Join(", ", s.ParamTypes.Select((_, i) => $"p{i}"));

            sb.Append($"    public static {s.ReturnType} @{s.Name}({paramList})\n    {{\n");
            sb.Append($"        if ((nint)_{s.Name} == 0) throw new System.EntryPointNotFoundException(\"'{s.Name}' is not available for the loaded IDA SDK version.\");\n");
            sb.Append(s.ReturnType == "void" ? $"        _{s.Name}({argList});\n" : $"        return _{s.Name}({argList});\n");
            sb.Append("    }\n\n");
        }

        sb.Append("}\n");

        File.WriteAllText(Path.Combine(outDir, "IdaNative.Surface.g.cs"), sb.ToString());
    }

    internal static void EmitBinder(string outDir, string versionMember, IReadOnlyList<EmittedSymbol> symbolsInThisVersion)
    {
        var sb = new StringBuilder();
        sb.Append("using System.Runtime.InteropServices;\n\nnamespace S2Atelier.Ida.Generated;\n\n");
        sb.Append($"internal static unsafe class IdaNativeBinder_{versionMember}\n{{\n");
        sb.Append("    internal static void BindAll(nint idaHandle, nint idalibHandle)\n    {\n");

        foreach (var s in symbolsInThisVersion)
        {
            string handle = s.Module == "idalib" ? "idalibHandle" : "idaHandle";
            sb.Append($"        if (NativeLibrary.TryGetExport({handle}, \"{s.Name}\", out nint p{s.Name})) IdaNative._{s.Name} = ({s.PointerType})p{s.Name};\n");
        }

        sb.Append("    }\n");
        sb.Append("}\n");

        File.WriteAllText(Path.Combine(outDir, $"IdaNative.{versionMember}.Bind.g.cs"), sb.ToString());
    }

    internal static void EmitSdkVersion(string outDir, IReadOnlyList<string> versionMembers)
    {
        var sb = new StringBuilder();
        sb.Append("namespace S2Atelier.Ida.Generated;\n\n");
        sb.Append("public enum IdaSdkVersion\n{\n    Auto = 0,\n\n");

        foreach (string member in versionMembers)
        {
            sb.Append($"    {member},\n");
        }

        sb.Append("}\n");

        File.WriteAllText(Path.Combine(outDir, "IdaSdkVersion.g.cs"), sb.ToString());
    }

    internal static void EmitDispatch(string outDir, IReadOnlyList<string> versionMembers)
    {
        var sb = new StringBuilder();
        sb.Append("namespace S2Atelier.Ida.Generated;\n\n");
        sb.Append("public static unsafe partial class IdaNative\n{\n");
        sb.Append($"    public static readonly IdaSdkVersion[] GeneratedVersions = [{string.Join(", ", versionMembers.Select(v => $"IdaSdkVersion.{v}"))}];\n\n");
        sb.Append("    public static void BindAll(nint idaHandle, nint idalibHandle, IdaSdkVersion version)\n    {\n");
        sb.Append("        switch (version)\n        {\n");

        foreach (string member in versionMembers)
        {
            sb.Append($"            case IdaSdkVersion.{member}: IdaNativeBinder_{member}.BindAll(idaHandle, idalibHandle); break;\n");
        }

        sb.Append("            default:\n");
        sb.Append("                throw new System.NotSupportedException($\"No generated bindings for IDA SDK {version}.\");\n");
        sb.Append("        }\n    }\n}\n");

        File.WriteAllText(Path.Combine(outDir, "IdaNative.Dispatch.g.cs"), sb.ToString());
    }
}
