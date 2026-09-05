using System.Runtime.InteropServices;

namespace S2Atelier.Ida;

[StructLayout(LayoutKind.Sequential)]
public struct AutoDisplay
{
    public int Type;
    public ulong Ea;
    public int State;
}
