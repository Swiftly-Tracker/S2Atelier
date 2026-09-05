using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

public static unsafe class Insn
{
    public const int BufferSize = 512;

    private const int EaOffset = 16;
    private const int ItypeOffset = 24;
    private const int OpsOffset = 40;
    private const int OpSize = 40;

    private const int OpTypeOffset = 1;
    private const int OpRegOffset = 6;
    private const int OpValueOffset = 8;
    private const int OpAddrOffset = 16;

    public const byte OpVoid = 0;
    public const byte OpReg = 1;
    public const byte OpMem = 2;
    public const byte OpPhrase = 3;
    public const byte OpDispl = 4;
    public const byte OpImm = 5;
    public const byte OpFar = 6;
    public const byte OpNear = 7;

    public static bool TryDecode(ulong ea, byte* buffer)
    {
        new Span<byte>(buffer, BufferSize).Clear();
        return IdaNative.decode_insn(buffer, ea) > 0;
    }

    public static bool IsCall(byte* buffer) => IdaNative.is_call_insn(buffer) != 0;

    public static ushort Itype(byte* buffer) => *(ushort*)(buffer + ItypeOffset);

    public static ulong Ea(byte* buffer) => *(ulong*)(buffer + EaOffset);

    public static byte OpType(byte* buffer, int index) => buffer[OpsOffset + index * OpSize + OpTypeOffset];

    public static ushort OpRegister(byte* buffer, int index) => *(ushort*)(buffer + OpsOffset + index * OpSize + OpRegOffset);

    public static ulong OpValue(byte* buffer, int index) => *(ulong*)(buffer + OpsOffset + index * OpSize + OpValueOffset);

    public static ulong OpAddr(byte* buffer, int index) => *(ulong*)(buffer + OpsOffset + index * OpSize + OpAddrOffset);
}
