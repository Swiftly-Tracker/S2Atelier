using System.Runtime.InteropServices;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

internal static unsafe class IdaBootstrap
{
    internal static bool TryInitLibrary(nint idalib, out int status)
    {
        status = 0;

        if (!NativeLibrary.TryGetExport(idalib, "init_library", out nint address))
        {
            return false;
        }

        status = ((delegate* unmanaged[Cdecl]<int, byte**, int>)address)(0, null);
        return true;
    }

    internal static bool TryProbeVersion(nint idalib, out (int Major, int Minor, int Build) version)
    {
        version = default;

        if (!NativeLibrary.TryGetExport(idalib, "get_library_version", out nint address))
        {
            return false;
        }

        var get = (delegate* unmanaged[Cdecl]<int*, int*, int*, byte>)address;
        int major = 0, minor = 0, build = 0;

        if (get(&major, &minor, &build) == 0)
        {
            return false;
        }

        version = (major, minor, build);
        return true;
    }

    internal static IdaSdkVersion ToSdkVersion((int Major, int Minor, int Build) version)
        => Enum.TryParse($"V{version.Major}{version.Minor}", out IdaSdkVersion sdk) ? sdk : IdaSdkVersion.Auto;
}
