using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

/// <summary>
/// Recovers a convar's EConVarType (tier1/convar.h) from its registration. The global itself is 16 zero
/// bytes in the file - a handle and a ConVarData pointer the engine fills in at runtime - so the value
/// type only exists statically as the constant the registration passes.
/// </summary>
internal static unsafe class ConVarTypeRecovery
{
    private const ulong BadAddr = ulong.MaxValue;
    private const int MaxSetupInsns = 48;
    private const byte DtWord = 1;
    private const string ValueInfoType = "ConVarValueInfo_t";
    private const string TypeField = "m_eVarType";
    // tinfo_t property: member count of a UDT.
    private const int MemberCount = 16;
    private const int ShadowSpace = 0x20;
    // How far past the shadow space a stack argument is looked for.
    private const int StackArguments = 4;

    private static readonly Dictionary<ulong, FunctionFrame> Frames = [];

    // Per call, the stack addresses it was given as arguments, plus every 16-bit constant store in the
    // function, all as frame offsets against the stack pointer on entry so operands through rsp, rbp or a
    // copy such as MSVC's "mov r11, rsp" become comparable.
    private sealed record FunctionFrame(
        Dictionary<ulong, List<long>> ArgumentsByCall,
        List<(ulong Ea, long Offset, ulong Value)> WordStores);

    internal static void Reset() => Frames.Clear();

    /// <summary>
    /// The type as an argument of the registration itself, or of the ConVarRefAbstract::Init call the
    /// CConVar constructor makes on the same object beforehand. Both are exact; -1 when neither applies.
    /// initCall is that Init call when there is one; ownType says the type came from the registration itself.
    /// </summary>
    internal static int FromCall(ulong registeredAt, ulong pfnStart, ulong obj, int flagSlot, int[] regs, int typeCount,
        out ulong initCall, out bool ownType)
    {
        initCall = BadAddr;
        ownType = false;
        if (regs.Length < 3 || pfnStart == BadAddr)
        {
            return -1;
        }

        // A reference-style registration takes (object, name, type) in one call. Its third argument is
        // only the type when the argument profile did not already claim it for the flags.
        if (flagSlot != 2 && ArgSetup(registeredAt, pfnStart, regs[2], out ulong own, out bool ownIsAddress) &&
            !ownIsAddress && own < (ulong)typeCount)
        {
            ownType = true;
            return (int)own;
        }

        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong ea = registeredAt;
        for (int i = 0; i < MaxSetupInsns; i++)
        {
            ea = IdaNative.prev_head(ea, pfnStart);
            if (ea == BadAddr || ea < pfnStart || !Insn.TryDecode(ea, buf))
            {
                return -1;
            }

            if (!Insn.IsCall(buf))
            {
                continue;
            }

            // find_reg_value gives up on constants set this far ahead of the call, so read the
            // instructions that load the argument registers instead.
            if (!ArgSetup(ea, pfnStart, regs[0], out ulong self, out bool isAddress) || !isAddress || self != obj ||
                !ArgSetup(ea, pfnStart, regs[2], out ulong type, out bool typeIsAddress) || typeIsAddress ||
                type >= (ulong)typeCount)
            {
                return -1;
            }

            initCall = ea;
            return (int)type;
        }

        return -1;
    }

    /// <summary>
    /// The type read out of the ConVarValueInfo_t the registration was given, which the caller builds on
    /// its stack: m_eVarType is a 16-bit store at the field's own offset, taken from the imported
    /// tier1/convar.h. It is the field itself, so it also corrects a type read off the registration
    /// arguments, where the third argument is the flags on some overloads. Returns how many were filled in.
    /// </summary>
    internal static int FromStack(IReadOnlyList<ConVarInfo> all, int typeCount, bool spilledValueInfo)
    {
        if (FieldOffset() is not long field)
        {
            return 0;
        }

        Dictionary<ulong, ulong> previous = Preceding(all);
        // The types each registration function was seen to register, where the field itself was readable.
        var byRegistrar = new Dictionary<ulong, HashSet<int>>();
        var fromField = new HashSet<ConVarInfo>();
        int filled = 0;
        foreach (ConVarInfo cv in all)
        {
            if (cv.IsCommand ||
                Candidate(cv, field, spilledValueInfo, previous.GetValueOrDefault(cv.RegisteredAt)) is not int value ||
                value >= typeCount)
            {
                continue;
            }

            if (cv.ValueType < 0)
            {
                filled++;
            }

            cv.ValueType = value;
            fromField.Add(cv);
            ulong registrar = IdaNative.get_first_fcref_from(cv.RegisteredAt);
            if (registrar != BadAddr)
            {
                byRegistrar.TryAdd(registrar, []);
                byRegistrar[registrar].Add(value);
            }
        }

        // What is left came from the registration arguments, where the third one is the flags on the
        // overloads that take a ConVarValueInfo_t. Drop it when the same registration function contradicts
        // it for a convar whose field was readable.
        foreach (ConVarInfo cv in all)
        {
            ulong registrar = IdaNative.get_first_fcref_from(cv.RegisteredAt);
            if (cv.ValueType < 0 || fromField.Contains(cv) || registrar == BadAddr ||
                !byRegistrar.TryGetValue(registrar, out HashSet<int>? seen) || seen.Contains(cv.ValueType))
            {
                continue;
            }

            cv.ValueType = -1;
        }

        return filled;
    }

    /// <summary>offsetof(ConVarValueInfo_t, m_eVarType), or null when the header is not imported.</summary>
    private static long? FieldOffset()
    {
        if (!ValveImplementationTypes.Load(ValueInfoType, out TypeInfo type))
        {
            return null;
        }

        try
        {
            nuint members = IdaNative.get_tinfo_property(type.Typid, MemberCount);
            for (nuint i = 0; i < members; i++)
            {
                if (!ValveImplementationTypes.ReadMember(type.Typid, i, out IdaUdtMember member))
                {
                    continue;
                }

                try
                {
                    if (member.Name.Read() == TypeField)
                    {
                        // udm_t::offset is in bits.
                        return (long)(member.Offset / 8);
                    }
                }
                finally { member.Dispose(); }
            }
        }
        finally { type.Dispose(); }

        return null;
    }

    /// <summary>
    /// Each convar registered in the same function as another, mapped to the registration before it. A
    /// function that registers several rebuilds one ConVarValueInfo_t per convar, so only the stores
    /// between the two calls describe the second one.
    /// </summary>
    private static Dictionary<ulong, ulong> Preceding(IReadOnlyList<ConVarInfo> all)
    {
        var byFunction = new Dictionary<ulong, List<ulong>>();
        foreach (ConVarInfo cv in all)
        {
            ulong start = FunctionStart(cv.RegisteredAt);
            if (start != BadAddr)
            {
                byFunction.TryAdd(start, []);
                byFunction[start].Add(cv.RegisteredAt);
            }
        }

        var preceding = new Dictionary<ulong, ulong>();
        foreach (List<ulong> registrations in byFunction.Values)
        {
            registrations.Sort();
            for (int i = 1; i < registrations.Count; i++)
            {
                preceding.TryAdd(registrations[i], registrations[i - 1]);
            }
        }

        return preceding;
    }

    /// <summary>
    /// The value last written to m_eVarType before the registration, in each stack struct the call was
    /// given. Only one of those arguments is the ConVarValueInfo_t, so two arguments that disagree mean
    /// neither can be trusted and nothing is returned. Stores outside this convar's own window belong to
    /// another convar built in the same frame.
    /// </summary>
    private static int? Candidate(ConVarInfo cv, long field, bool spilledValueInfo, ulong after)
    {
        if (Layout(cv.RegisteredAt, spilledValueInfo) is not (FunctionFrame frame, List<long> arguments))
        {
            return null;
        }

        var latest = new Dictionary<long, ulong>();
        foreach ((ulong ea, long offset, ulong value) in frame.WordStores)
        {
            if (ea > after && ea <= cv.RegisteredAt)
            {
                latest[offset] = value;
            }
        }

        int? found = null;
        foreach (long argument in arguments)
        {
            if (!latest.TryGetValue(argument + field, out ulong value))
            {
                continue;
            }

            if (found is int other && other != (int)value)
            {
                return null;
            }

            found = (int)value;
        }

        return found;
    }

    private static (FunctionFrame Frame, List<long> Arguments)? Layout(ulong registeredAt, bool spilledValueInfo)
    {
        ulong pfnStart = FunctionStart(registeredAt);
        if (pfnStart == BadAddr)
        {
            return null;
        }

        if (!Frames.TryGetValue(pfnStart, out FunctionFrame? frame))
        {
            Frames[pfnStart] = frame = Analyze(pfnStart, spilledValueInfo);
        }

        return frame.ArgumentsByCall.TryGetValue(registeredAt, out List<long>? arguments)
            ? (frame, arguments)
            : null;
    }

    private static FunctionFrame Analyze(ulong pfnStart, bool spilledValueInfo)
    {
        var argumentsByCall = new Dictionary<ulong, List<long>>();
        var stores = new List<(ulong, long, ulong)>();
        // Registers holding a stack address, as an offset against the stack pointer on entry.
        var pointers = new Dictionary<ushort, long> { [Register("rsp")] = 0 };
        ushort rsp = Register("rsp");
        int[] regs = ArgumentRegisters();
        // Stack slots a tracked pointer was stored into, so a reload recovers it.
        var slots = new Dictionary<long, long>();

        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong end = FunctionEnd(pfnStart);
        for (ulong ea = pfnStart; ea < end && ea != BadAddr; ea = IdaNative.next_head(ea, end))
        {
            if (!Insn.TryDecode(ea, buf))
            {
                continue;
            }

            string mnemonic = Mnemonic(ea);
            if (!pointers.TryGetValue(rsp, out long stack))
            {
                // An opaque write to rsp loses the frame; nothing after it is comparable.
                break;
            }

            if (Insn.IsCall(buf))
            {
                // Which argument holds the ConVarValueInfo_t differs between the registration overloads,
                // so every argument that is a stack address is a candidate.
                var passed = new List<long>();
                // MSVC passes the fifth argument onwards in the shadow space instead of a register.
                for (int i = 0; spilledValueInfo && i < StackArguments; i++)
                {
                    if (slots.TryGetValue(stack + ShadowSpace + i * 8, out long onStack))
                    {
                        passed.Add(onStack);
                    }
                }

                foreach (int register in regs)
                {
                    if (pointers.TryGetValue((ushort)register, out long held))
                    {
                        passed.Add(held);
                    }
                }

                if (passed.Count > 0)
                {
                    argumentsByCall[ea] = passed;
                }

                foreach (ushort clobbered in Volatile(regs))
                {
                    pointers.Remove(clobbered);
                }
                continue;
            }

            if (mnemonic == "push")
            {
                pointers[rsp] = stack - 8;
                continue;
            }
            if (mnemonic == "pop")
            {
                pointers[rsp] = stack + 8;
                continue;
            }
            if (mnemonic is "sub" or "add" && Insn.OpType(buf, 0) == Insn.OpReg && Insn.OpRegister(buf, 0) == rsp &&
                Insn.OpType(buf, 1) == Insn.OpImm)
            {
                pointers[rsp] = stack + (mnemonic == "add" ? 1 : -1) * unchecked((long)Insn.OpValue(buf, 1));
                continue;
            }

            if (Insn.OpType(buf, 0) == Insn.OpReg)
            {
                // Including rsp itself: a function with several exits restores it from the frame pointer
                // mid-body, and that is recoverable, while any other write to it is not.
                ushort target = Insn.OpRegister(buf, 0);
                if (mnemonic == "mov" && Insn.OpType(buf, 1) == Insn.OpReg &&
                    pointers.TryGetValue(Insn.OpRegister(buf, 1), out long copied))
                {
                    pointers[target] = copied;
                }
                else if (mnemonic == "lea" && Insn.OpType(buf, 1) is Insn.OpDispl or Insn.OpPhrase &&
                         pointers.TryGetValue(Insn.OpRegister(buf, 1), out long based))
                {
                    pointers[target] = based + Displacement(buf, 1);
                }
                else if (mnemonic == "mov" && Insn.OpType(buf, 1) is Insn.OpDispl or Insn.OpPhrase &&
                         pointers.TryGetValue(Insn.OpRegister(buf, 1), out long from) &&
                         slots.TryGetValue(from + Displacement(buf, 1), out long reloaded))
                {
                    pointers[target] = reloaded;
                }
                else
                {
                    pointers.Remove(target);
                }
                continue;
            }

            if (mnemonic != "mov" || Insn.OpType(buf, 0) is not (Insn.OpDispl or Insn.OpPhrase) ||
                !pointers.TryGetValue(Insn.OpRegister(buf, 0), out long baseOffset))
            {
                continue;
            }

            long offset = baseOffset + Displacement(buf, 0);
            if (Insn.OpType(buf, 1) == Insn.OpReg && pointers.TryGetValue(Insn.OpRegister(buf, 1), out long saved))
            {
                slots[offset] = saved;
                continue;
            }

            slots.Remove(offset);
            if (Insn.OpWidth(buf, 0) != DtWord)
            {
                continue;
            }

            if (Insn.OpType(buf, 1) == Insn.OpImm)
            {
                stores.Add((ea, offset, Insn.OpValue(buf, 1)));
            }
            else if (Insn.OpType(buf, 1) == Insn.OpReg &&
                     ArgSetup(ea, pfnStart, Insn.OpRegister(buf, 1), out ulong value, out bool isAddress) && !isAddress)
            {
                stores.Add((ea, offset, value));
            }
        }

        return new FunctionFrame(argumentsByCall, stores);
    }

    // The last instruction to write the register before a call, when it is a plain immediate, a loaded
    // address, or a zeroing xor.
    internal static bool ArgSetup(ulong callEa, ulong pfnStart, int reg, out ulong value, out bool address)
    {
        value = 0;
        address = false;
        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong ea = callEa;
        for (int i = 0; i < MaxSetupInsns; i++)
        {
            ea = IdaNative.prev_head(ea, pfnStart);
            if (ea == BadAddr || ea < pfnStart || !Insn.TryDecode(ea, buf) || Insn.IsCall(buf))
            {
                return false;
            }

            if (Insn.OpType(buf, 0) != Insn.OpReg || Insn.OpRegister(buf, 0) != reg)
            {
                continue;
            }

            string mnemonic = Mnemonic(ea);
            if (mnemonic == "mov" && Insn.OpType(buf, 1) == Insn.OpImm)
            {
                value = Insn.OpValue(buf, 1);
                return true;
            }
            if (mnemonic == "lea" && Insn.OpType(buf, 1) == Insn.OpMem)
            {
                value = Insn.OpAddr(buf, 1);
                address = true;
                return true;
            }
            if (mnemonic == "xor" && Insn.OpType(buf, 1) == Insn.OpReg && Insn.OpRegister(buf, 1) == reg)
            {
                return true;
            }
            return false;
        }

        return false;
    }

    private static long Displacement(byte* insn, int operand)
        => Insn.OpType(insn, operand) == Insn.OpDispl ? unchecked((long)Insn.OpAddr(insn, operand)) : 0;

    private static IEnumerable<ushort> Volatile(int[] regs)
        => regs.Select(x => (ushort)x).Concat(new[] { "rax", "r10", "r11" }.Select(Register));

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

    private static int[]? _argumentRegisters;

    internal static int[] ArgumentRegisters() => _argumentRegisters ??= ConVarNaming.ArgumentRegisters();
}
