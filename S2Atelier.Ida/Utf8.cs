using System.Runtime.InteropServices;
using System.Text;

namespace S2Atelier.Ida;

internal static unsafe class Utf8
{
    internal static byte* Allocate(string text)
    {
        int byteCount = Encoding.UTF8.GetByteCount(text);
        byte* buffer = (byte*)NativeMemory.Alloc((nuint)(byteCount + 1));
        int written = Encoding.UTF8.GetBytes(text, new Span<byte>(buffer, byteCount));
        buffer[written] = 0;
        return buffer;
    }

    internal static void Free(byte* buffer) => NativeMemory.Free(buffer);
}
