using System.Runtime.InteropServices;

namespace S2Atelier.Ida;

internal static class IdaPlatform
{
    internal static string LibraryFileName(string module)
    {
        if (OperatingSystem.IsWindows())
        {
            return module + ".dll";
        }

        return OperatingSystem.IsMacOS() ? $"lib{module}.dylib" : $"lib{module}.so";
    }

    internal static void PreloadSiblingLibraries(string root)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        foreach (string file in Directory.EnumerateFiles(root, "*.dll"))
        {
            string stem = Path.GetFileNameWithoutExtension(file);

            if (IsKernelModule(stem))
            {
                continue;
            }

            NativeLibrary.TryLoad(file, out _);
        }
    }

    private static bool IsKernelModule(string stem)
        => stem.Equals("ida", StringComparison.OrdinalIgnoreCase)
        || stem.Equals("ida32", StringComparison.OrdinalIgnoreCase)
        || stem.Equals("idalib", StringComparison.OrdinalIgnoreCase)
        || stem.Equals("idalib32", StringComparison.OrdinalIgnoreCase);
}
