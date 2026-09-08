using System.Runtime.InteropServices;

namespace S2Atelier.Ida;

internal static class IdaPlatform
{
    // IDAClang writes through the native CRT. Console.Out.Flush() only flushes
    // managed output; native diagnostics must precede the JSON protocol delimiter.
    internal static void FlushNativeOutput()
    {
        if (OperatingSystem.IsWindows())
            FlushWindows(0);
        else if (OperatingSystem.IsMacOS())
            FlushMac(0);
        else
            FlushUnix(0);
    }

    [DllImport("ucrtbase", EntryPoint = "fflush", CallingConvention = CallingConvention.Cdecl)]
    private static extern int FlushWindows(nint stream);

    [DllImport("libc", EntryPoint = "fflush", CallingConvention = CallingConvention.Cdecl)]
    private static extern int FlushUnix(nint stream);

    [DllImport("/usr/lib/libSystem.B.dylib", EntryPoint = "fflush", CallingConvention = CallingConvention.Cdecl)]
    private static extern int FlushMac(nint stream);

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
