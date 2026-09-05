using System.Runtime.InteropServices;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct TypeInfo : IDisposable
{
    public ulong Typid;

    public void Dispose()
    {
        fixed (TypeInfo* self = &this)
        {
            IdaNative.clear_tinfo_t(self);
        }

        Typid = 0;
    }
}
