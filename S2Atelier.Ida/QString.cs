using System.Runtime.InteropServices;
using System.Text;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct QString : IDisposable
{
    public byte* Array;
    public nuint Length;
    public nuint Capacity;

    public readonly string Read()
    {
        if (Array == null || Length == 0)
        {
            return string.Empty;
        }

        int len = (int)Length;
        if (Array[len - 1] == 0)
        {
            len--;
        }

        return len <= 0 ? string.Empty : Encoding.UTF8.GetString(Array, len);
    }

    public void Dispose()
    {
        if (Array != null)
        {
            IdaNative.qfree(Array);
            Array = null;
            Length = 0;
            Capacity = 0;
        }
    }
}
