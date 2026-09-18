using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

/// <summary>
/// Evidence that IDA's guessed prototype for a function (guess_tinfo) is incomplete: a floating-point
/// return, or callers that set up more argument registers than it has parameters. Applying such a guess,
/// even as a guess, makes Hex-Rays drop the return value and the missing arguments at every call, so a
/// prototype built on it is only worth applying when neither shows.
/// </summary>
internal static unsafe class GuessedPrototype
{
    private const ulong BadAddr = ulong.MaxValue;
    private const int MaxBlockInsns = 16;
    private const int MaxReturnInsns = 6;

    internal static bool LooksIncomplete(ulong function, int parameters, bool floatReturn)
        => (!floatReturn && ReturnsFloat(function)) || ArgumentsAtCalls(function) > parameters;

    // xmm0 written, and rax not, just before a return. Comparisons only read their first operand.
    private static bool ReturnsFloat(ulong function)
    {
        ushort xmm0 = Register("xmm0"), al = Register("al");
        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong end = FunctionEnd(function);
        for (ulong ea = function; ea < end && ea != BadAddr; ea = IdaNative.next_head(ea, end))
        {
            if (!Mnemonic(ea).StartsWith("ret", StringComparison.Ordinal))
            {
                continue;
            }

            ulong back = ea;
            for (int i = 0; i < MaxReturnInsns; i++)
            {
                back = IdaNative.prev_head(back, function);
                if (back == BadAddr || back < function || !Insn.TryDecode(back, buf) || Insn.IsCall(buf))
                {
                    break;
                }

                string mnemonic = Mnemonic(back);
                if (Insn.OpType(buf, 0) != Insn.OpReg || mnemonic.Contains("comis", StringComparison.Ordinal) ||
                    mnemonic is "cmp" or "test")
                {
                    continue;
                }

                ushort written = Insn.OpRegister(buf, 0);
                if (written == 0 || written == al)
                {
                    break;
                }

                if (written == xmm0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// The most parameters any direct caller passes in registers: the highest argument register written in
    /// the call's block before it. Microsoft x64 assigns a position to either the integer or the xmm
    /// register; System V counts the two sequences separately.
    /// </summary>
    private static int ArgumentsAtCalls(ulong function)
    {
        int[] integers = ConVarNaming.ArgumentRegisters();
        bool positional = integers.Length == 4;
        ushort xmm0 = Register("xmm0");
        int floats = positional ? 4 : 8;
        byte* buf = stackalloc byte[Insn.BufferSize];
        int most = 0;
        foreach (ulong call in Xrefs.CodeTo(function))
        {
            if (!Insn.TryDecode(call, buf) || !Insn.IsCall(buf) || Insn.OpType(buf, 0) != Insn.OpNear ||
                Insn.OpAddr(buf, 0) != function)
            {
                continue;
            }

            ulong caller = FunctionStart(call);
            int integerCount = 0, floatCount = 0;
            ulong ea = call;
            for (int i = 0; i < MaxBlockInsns && caller != BadAddr; i++)
            {
                ea = IdaNative.prev_head(ea, caller);
                if (ea == BadAddr || ea < caller || !Insn.TryDecode(ea, buf) || Insn.IsCall(buf) || IsLabel(ea))
                {
                    break;
                }

                if (Insn.OpType(buf, 0) != Insn.OpReg || Mnemonic(ea) is "cmp" or "test" or "push")
                {
                    continue;
                }

                ushort written = Insn.OpRegister(buf, 0);
                int index = Array.IndexOf(integers, (int)written);
                if (index >= 0)
                {
                    integerCount = Math.Max(integerCount, index + 1);
                }
                else if (written >= xmm0 && written < xmm0 + floats)
                {
                    floatCount = Math.Max(floatCount, written - xmm0 + 1);
                }
            }

            most = Math.Max(most, positional ? Math.Max(integerCount, floatCount) : integerCount + floatCount);
        }

        return most;
    }

    private static bool IsLabel(ulong ea)
    {
        foreach (ulong from in Xrefs.CodeTo(ea))
        {
            if (IdaNative.next_head(from, ea + 1) != ea)
            {
                return true;
            }
        }

        return false;
    }

    private static ushort Register(string name)
    {
        byte* native = Utf8.Allocate(name);
        try { return (ushort)IdaNative.str2reg(native); }
        finally { Utf8.Free(native); }
    }

    private static string Mnemonic(ulong ea)
    {
        var text = new QString();
        try { return IdaNative.print_insn_mnem(&text, ea) > 0 ? text.Read() : string.Empty; }
        finally { text.Dispose(); }
    }

    private static ulong FunctionStart(ulong ea)
    {
        void* function = IdaNative.get_func(ea);
        return function == null ? BadAddr : *(ulong*)function;
    }

    private static ulong FunctionEnd(ulong ea)
    {
        void* function = IdaNative.get_func(ea);
        return function == null ? BadAddr : *((ulong*)function + 1);
    }
}
