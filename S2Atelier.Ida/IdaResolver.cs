using System.Reflection;
using System.Runtime.InteropServices;

namespace S2Atelier.Ida;

internal static class IdaResolver
{
    private static readonly Lock Gate = new();
    private static string? _root;
    private static bool _registered;

    internal static void SetRoot(string root)
    {
        lock (Gate)
        {
            _root = root;

            if (_registered)
            {
                return;
            }

            NativeLibrary.SetDllImportResolver(typeof(IdaResolver).Assembly, Resolve);
            _registered = true;
        }
    }

    internal static nint Load(string module)
    {
        nint handle = TryResolve(module);
        if (handle != nint.Zero)
        {
            return handle;
        }

        throw new DllNotFoundException(
            $"Could not load '{IdaPlatform.LibraryFileName(module)}'" +
            (string.IsNullOrEmpty(_root) ? "." : $" from '{_root}'.") +
            " Point --ida-path at an IDA installation.");
    }

    private static nint Resolve(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        => libraryName is "ida" or "idalib" ? TryResolve(libraryName) : nint.Zero;

    private static nint TryResolve(string module)
    {
        string fileName = IdaPlatform.LibraryFileName(module);
        string? root = _root;

        if (!string.IsNullOrEmpty(root) && NativeLibrary.TryLoad(Path.Combine(root, fileName), out nint handle))
        {
            return handle;
        }

        return NativeLibrary.TryLoad(fileName, out handle) ? handle : nint.Zero;
    }
}
