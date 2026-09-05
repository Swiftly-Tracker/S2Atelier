using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

public sealed record FnPtrNamingResult(bool Applicable, int Found, int Renamed);

public static unsafe class FnPtrNaming
{
    private const ulong BadAddr = ulong.MaxValue;
    private const int SegPermExec = 1;
    private const int MaxNameLen = 128;

    private static readonly string[] AutoNamePrefixes =
    [
        "sub_", "nullsub_", "loc_", "off_", "unk_", "byte_", "word_", "dword_",
        "qword_", "asc_", "algn_", "stru_", "xmmword_", "ymmword_", "flt_", "dbl_",
    ];

    private enum State { Idle, SawLea, SawCmp }

    private const int MatchScanBudget = 12;

    public static FnPtrNamingResult Run()
    {
        var found = new Dictionary<ulong, string>();
        var segments = ExecutableSegments();

        foreach (var (segStart, segEnd) in segments)
        {
            ScanRange(segStart, segEnd, found);
        }

        foreach (var (segStart, segEnd) in segments)
        {
            ScanRangeStrcmp(segStart, segEnd, found);
        }

        int renamed = 0;
        foreach (var (target, name) in found)
        {
            if (Apply(target, name))
            {
                renamed++;
            }
        }

        return new FnPtrNamingResult(true, found.Count, renamed);
    }

    private static void ScanRange(ulong segStart, ulong segEnd, Dictionary<ulong, string> found)
    {
        byte* buf = stackalloc byte[Insn.BufferSize];
        byte* matchBuf = stackalloc byte[Insn.BufferSize];

        var state = State.Idle;
        ulong leaReg = 0;
        ulong leaTarget = 0;

        ulong ea = segStart;
        while (ea != BadAddr && ea < segEnd)
        {
            if (!Insn.TryDecode(ea, buf))
            {
                ea = IdaNative.next_head(ea, segEnd);
                continue;
            }

            bool isLeaToFunc = TryLeaToFunctionStart(buf, out ulong candReg, out ulong candTarget);

            switch (state)
            {
                case State.Idle:
                    if (isLeaToFunc)
                    {
                        state = State.SawLea;
                        leaReg = candReg;
                        leaTarget = candTarget;
                    }
                    break;

                case State.SawLea:
                    if (RegisterReferenced(buf, leaReg) && IsMnemonic(ea, "cmp", "test"))
                    {
                        state = State.SawCmp;
                    }
                    else if (isLeaToFunc)
                    {
                        state = State.SawLea;
                        leaReg = candReg;
                        leaTarget = candTarget;
                    }
                    else
                    {
                        state = State.Idle;
                    }
                    break;

                case State.SawCmp:
                    if (TryClassifyJcc(ea, out bool isEqualBranch, out ulong jccTarget))
                    {
                        ulong nextEa = IdaNative.next_head(ea, segEnd);
                        ulong matchEa = isEqualBranch ? jccTarget : nextEa;

                        if (TryResolveReturnedString(matchEa, matchBuf, out string text))
                        {
                            found.TryAdd(leaTarget, text);
                        }

                        state = State.Idle;
                    }
                    else if (isLeaToFunc)
                    {
                        state = State.SawLea;
                        leaReg = candReg;
                        leaTarget = candTarget;
                    }
                    else
                    {
                        state = State.Idle;
                    }
                    break;
            }

            ea = IdaNative.next_head(ea, segEnd);
        }
    }

    private static void ScanRangeStrcmp(ulong segStart, ulong segEnd, Dictionary<ulong, string> found)
    {
        byte* buf = stackalloc byte[Insn.BufferSize];
        byte* matchBuf = stackalloc byte[Insn.BufferSize];

        ulong ea = segStart;
        while (ea != BadAddr && ea < segEnd)
        {
            if (!Insn.TryDecode(ea, buf))
            {
                ea = IdaNative.next_head(ea, segEnd);
                continue;
            }

            if (TryLeaAnyToString(buf, out string text))
            {
                ulong afterLea = IdaNative.next_head(ea, segEnd);

                if (TryFindStrcmpCall(afterLea, segEnd, out ulong callEa)
                    && TryFindJcc(IdaNative.next_head(callEa, segEnd), segEnd,
                        out ulong jccEa, out bool isEqualBranch, out ulong jccTarget))
                {
                    ulong nextEa = IdaNative.next_head(jccEa, segEnd);
                    ulong matchEa = isEqualBranch ? jccTarget : nextEa;

                    if (TryResolveMatchAddress(matchEa, matchBuf, out ulong targetAddr))
                    {
                        found.TryAdd(targetAddr, text);
                    }
                }
            }

            ea = IdaNative.next_head(ea, segEnd);
        }
    }

    private static bool TryFindStrcmpCall(ulong startEa, ulong outerBound, out ulong callEa)
    {
        callEa = 0;
        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong bound = Math.Min(startEa + 64, outerBound);

        ulong ea = startEa;
        for (int i = 0; i < 6 && ea != BadAddr && ea < bound; i++)
        {
            if (!Insn.TryDecode(ea, buf))
            {
                return false;
            }

            if (IsCallToStrcmp(buf))
            {
                callEa = ea;
                return true;
            }

            if (IsMnemonic(ea, "ret", "retn", "leave"))
            {
                return false;
            }

            ea = IdaNative.next_head(ea, bound);
        }

        return false;
    }

    private static bool TryFindJcc(ulong startEa, ulong outerBound, out ulong jccEa, out bool isEqualBranch, out ulong target)
    {
        jccEa = 0;
        isEqualBranch = false;
        target = 0;
        ulong bound = Math.Min(startEa + 64, outerBound);

        ulong ea = startEa;
        for (int i = 0; i < 6 && ea != BadAddr && ea < bound; i++)
        {
            if (TryClassifyJcc(ea, out isEqualBranch, out target))
            {
                jccEa = ea;
                return true;
            }

            ea = IdaNative.next_head(ea, bound);
        }

        return false;
    }

    private static bool TryResolveMatchAddress(ulong startEa, byte* buf, out ulong targetAddr)
    {
        targetAddr = 0;
        ulong bound = startEa + 96;

        ulong ea = startEa;
        for (int i = 0; i < 8 && ea != BadAddr && ea < bound; i++)
        {
            if (!Insn.TryDecode(ea, buf))
            {
                return false;
            }

            if (TryLeaToFunctionStart(buf, out _, out ulong target))
            {
                targetAddr = target;
                return true;
            }

            if (IsMnemonic(ea, "ret", "retn"))
            {
                return false;
            }

            ea = IdaNative.next_head(ea, bound);
        }

        return false;
    }

    private static bool IsCallToStrcmp(byte* buf)
    {
        if (!Insn.IsCall(buf))
        {
            return false;
        }

        if (Insn.OpType(buf, 0) is not (Insn.OpNear or Insn.OpFar))
        {
            return false;
        }

        ulong target = Insn.OpAddr(buf, 0);
        return GetName(target).Contains("strcmp", StringComparison.Ordinal);
    }

    private static bool TryLeaAnyToString(byte* buf, out string text)
    {
        text = string.Empty;

        if (Insn.OpType(buf, 0) != Insn.OpReg || Insn.OpType(buf, 1) != Insn.OpMem)
        {
            return false;
        }

        ulong addr = Insn.OpAddr(buf, 1);
        if (IdaNative.is_mapped(addr) == 0)
        {
            return false;
        }

        ulong ea = Insn.Ea(buf);
        if (!IsMnemonic(ea, "lea"))
        {
            return false;
        }

        string s = ReadDisplayString(addr);
        if (s.Length == 0)
        {
            return false;
        }

        text = s;
        return true;
    }

    private static bool TryClassifyJcc(ulong ea, out bool isEqualBranch, out ulong target)
    {
        isEqualBranch = false;
        target = 0;

        var qs = new QString();
        bool ok = IdaNative.print_insn_mnem(&qs, ea) > 0;
        string mnem = ok ? qs.Read() : string.Empty;
        qs.Dispose();

        if (mnem is not ("je" or "jz" or "jne" or "jnz"))
        {
            return false;
        }

        byte* buf = stackalloc byte[Insn.BufferSize];
        if (!Insn.TryDecode(ea, buf))
        {
            return false;
        }

        isEqualBranch = mnem is "je" or "jz";
        target = Insn.OpAddr(buf, 0);
        return target != 0;
    }

    private static bool TryResolveReturnedString(ulong startEa, byte* buf, out string text)
    {
        text = string.Empty;
        ulong strAddr = 0;
        ulong bound = startEa + 256;

        ulong ea = startEa;
        for (int i = 0; i < MatchScanBudget && ea != BadAddr && ea < bound; i++)
        {
            if (!Insn.TryDecode(ea, buf))
            {
                return false;
            }

            if (strAddr == 0 && TryLeaString(buf, out ulong candStrAddr))
            {
                strAddr = candStrAddr;
            }
            else if (IsMnemonic(ea, "retn", "ret"))
            {
                if (strAddr == 0)
                {
                    return false;
                }

                text = ReadDisplayString(strAddr);
                return text.Length > 0;
            }

            ea = IdaNative.next_head(ea, bound);
        }

        return false;
    }

    private static bool TryLeaToFunctionStart(byte* buf, out ulong reg, out ulong target)
    {
        reg = 0;
        target = 0;

        if (Insn.OpType(buf, 0) != Insn.OpReg || Insn.OpType(buf, 1) != Insn.OpMem)
        {
            return false;
        }

        ulong addr = Insn.OpAddr(buf, 1);
        if (!IsFunctionStart(addr))
        {
            return false;
        }

        ulong ea = Insn.Ea(buf);
        if (!IsMnemonic(ea, "lea"))
        {
            return false;
        }

        reg = Insn.OpRegister(buf, 0);
        target = addr;
        return true;
    }

    private static bool TryLeaString(byte* buf, out ulong strAddr)
    {
        strAddr = 0;

        if (Insn.OpType(buf, 0) != Insn.OpReg || Insn.OpRegister(buf, 0) != RaxRegister()
            || Insn.OpType(buf, 1) != Insn.OpMem)
        {
            return false;
        }

        ulong addr = Insn.OpAddr(buf, 1);
        if (IdaNative.is_mapped(addr) == 0)
        {
            return false;
        }

        ulong ea = Insn.Ea(buf);
        if (!IsMnemonic(ea, "lea"))
        {
            return false;
        }

        string text = ReadDisplayString(addr);
        if (text.Length == 0)
        {
            return false;
        }

        strAddr = addr;
        return true;
    }

    private static bool RegisterReferenced(byte* buf, ulong reg)
    {
        return (Insn.OpType(buf, 0) == Insn.OpReg && Insn.OpRegister(buf, 0) == reg)
            || (Insn.OpType(buf, 1) == Insn.OpReg && Insn.OpRegister(buf, 1) == reg);
    }

    private static ulong? _raxRegister;

    private static ulong RaxRegister()
    {
        if (_raxRegister is { } cached)
        {
            return cached;
        }

        byte* namePtr = Utf8.Allocate("rax");
        int reg = IdaNative.str2reg(namePtr);
        Utf8.Free(namePtr);
        return (_raxRegister = (ulong)reg).Value;
    }

    private static bool IsMnemonic(ulong ea, params string[] candidates)
    {
        var qs = new QString();
        bool ok = IdaNative.print_insn_mnem(&qs, ea) > 0;
        string mnem = ok ? qs.Read() : string.Empty;
        qs.Dispose();

        foreach (string c in candidates)
        {
            if (mnem == c)
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsFunctionStart(ulong ea)
    {
        void* pfn = IdaNative.get_func(ea);
        return pfn != null && *(ulong*)pfn == ea;
    }

    private static string ReadDisplayString(ulong ea)
    {
        if (ea == BadAddr || IdaNative.is_mapped(ea) == 0)
        {
            return string.Empty;
        }

        var qs = new QString();
        nint written = IdaNative.get_strlit_contents(&qs, ea, nuint.MaxValue, 0, null, 0);
        if (written <= 0)
        {
            qs.Dispose();
            return string.Empty;
        }

        string text = qs.Read();
        qs.Dispose();
        return text;
    }

    private static List<(ulong Start, ulong End)> ExecutableSegments()
    {
        var result = new List<(ulong Start, ulong End)>();

        void* seg = IdaNative.get_first_seg();
        while (seg != null)
        {
            ulong start = *(ulong*)seg;
            ulong end = *((ulong*)seg + 1);
            byte perm = ((byte*)seg)[42];

            if ((perm & SegPermExec) != 0)
            {
                result.Add((start, end));
            }

            seg = IdaNative.get_next_seg(start);
        }

        return result;
    }

    private static string SanitizeName(string raw)
    {
        var chars = new char[Math.Min(raw.Length, MaxNameLen)];
        int n = 0;

        foreach (char c in raw)
        {
            if (n >= chars.Length)
            {
                break;
            }

            chars[n++] = char.IsAsciiLetterOrDigit(c) || c == '_' ? c : '_';
        }

        string s = new string(chars, 0, n).Trim('_');
        if (s.Length == 0)
        {
            return string.Empty;
        }

        return char.IsAsciiLetter(s[0]) || s[0] == '_' ? s : "_" + s;
    }

    private static bool IsAutoName(string name)
    {
        foreach (string prefix in AutoNamePrefixes)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static string GetName(ulong ea)
    {
        var qs = new QString();
        nint len = IdaNative.get_ea_name(&qs, ea, 0, null);
        string text = len > 0 ? qs.Read() : string.Empty;
        qs.Dispose();
        return text;
    }

    private static bool Apply(ulong ea, string rawName)
    {
        string wanted = SanitizeName(rawName);
        if (wanted.Length == 0)
        {
            return false;
        }

        string current = GetName(ea);
        if (current == wanted)
        {
            return false;
        }

        if (current.Length > 0 && !IsAutoName(current))
        {
            return false;
        }

        byte* namePtr = Utf8.Allocate(wanted);
        bool ok = IdaNative.set_name(ea, namePtr, 0x800 | 0x01) != 0;
        Utf8.Free(namePtr);
        return ok;
    }
}
