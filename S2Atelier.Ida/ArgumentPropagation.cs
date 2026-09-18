using System.Runtime.InteropServices;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

internal sealed record ArgumentPropagationSummary(int Candidates, int Typed, int NoCommonBase);

/// <summary>
/// Types the first argument of functions only ever called with a typed function's own <c>this</c>: a helper
/// that CCSPlayer_MovementServices::vfn_39 alone calls with its object takes a CCSPlayer_MovementServices *.
/// With several callers it takes their closest common primary base, whose pointer is the same value. Newly
/// typed functions pass their argument on in turn, so this repeats until nothing changes.
/// </summary>
internal static unsafe class ArgumentPropagation
{
    private const ulong BadAddr = ulong.MaxValue;
    private const int MaxRounds = 8;
    private const int MaxBlockInsns = 256;
    // tinfo_t properties (GTA_*).
    private const int GtaFinalOrdinal = 8;
    private const int GtaPointedObject = 9;
    private const int GtaUdtMemberCount = 16;
    private const int GtaFunctionArgumentCount = 23;
    private const int GtaFunctionArgument = 25;
    // udm_t::flags UDM_BASECLASS.
    private const uint BaseClassMember = 0x20;
    // func_t::flags FUNC_LIB and FUNC_THUNK.
    private const ulong LibraryOrThunk = 0x04 | 0x80;

    internal static ArgumentPropagationSummary Run(int[] argumentRegisters, SchemaImport.LimitedDiagnostics diagnostics)
    {
        ushort thisRegister = (ushort)argumentRegisters[0];
        var classOf = new Dictionary<ulong, uint>();
        for (nuint i = 0, count = IdaNative.get_func_qty(); i < count; i++)
        {
            void* function = IdaNative.getn_func(i);
            if (function != null && FirstArgumentClass(*(ulong*)function) is uint ordinal)
            {
                classOf[*(ulong*)function] = ordinal;
            }
        }

        var chains = new Dictionary<uint, List<uint>>();
        var tried = new HashSet<ulong>();
        int candidates = 0, typed = 0, noCommonBase = 0;
        IEnumerable<ulong> callers = classOf.Keys.ToList();
        for (int round = 0; round < MaxRounds; round++)
        {
            var fresh = new List<ulong>();
            foreach (ulong callee in callers.SelectMany(x => PassesThis(x, thisRegister)).Distinct())
            {
                if (classOf.ContainsKey(callee) || !tried.Add(callee) || !Propagatable(callee))
                {
                    continue;
                }

                // Every reference is a call that passes some typed function's own object. Unwind and EH tables
                // hold 32-bit RVAs; a full pointer (a vtable, a callback) means callers that cannot be seen.
                var classes = new List<uint>();
                if (Xrefs.DataTo(callee).Any(source => IdaNative.get_qword(source) == callee))
                {
                    continue;
                }

                foreach (ulong site in Xrefs.CodeTo(callee))
                {
                    ulong caller = FunctionStart(site);
                    if (caller == BadAddr || !classOf.TryGetValue(caller, out uint ordinal) ||
                        !IsCallTo(site, callee) || !ArgumentIsThis(site, caller, thisRegister))
                    {
                        classes.Clear();
                        break;
                    }

                    classes.Add(ordinal);
                }

                if (classes.Count == 0)
                {
                    continue;
                }

                candidates++;
                if (CommonBase(classes, chains) is not uint common)
                {
                    noCommonBase++;
                    continue;
                }

                string? owner = TypeName(common);
                if (owner != null && SchemaImport.TryBindThisParameter(callee, owner, diagnostics, argumentName: null))
                {
                    SdkFunctionBinding.RecordType(callee, "argument", diagnostics.Write);
                    classOf[callee] = common;
                    fresh.Add(callee);
                    typed++;
                }
            }

            if (fresh.Count == 0)
            {
                break;
            }

            callers = fresh;
        }

        return new ArgumentPropagationSummary(candidates, typed, noCommonBase);
    }

    // Library and thunk functions (memcpy and the like) take whatever object they are given.
    private static bool Propagatable(ulong function)
    {
        void* pfn = IdaNative.get_func(function);
        return pfn != null && *(ulong*)pfn == function && (*((ulong*)pfn + 2) & LibraryOrThunk) == 0 &&
               SdkFunctionBinding.CanUpdateType(function);
    }

    /// <summary>The ordinal of the class the function's applied first argument points to.</summary>
    private static uint? FirstArgumentClass(ulong function)
    {
        TypeInfo type = default;
        try
        {
            if (IdaNative.get_tinfo(&type, function) == 0)
            {
                return null;
            }

            nuint arguments = IdaNative.get_tinfo_property(type.Typid, GtaFunctionArgumentCount);
            if (arguments == 0 || arguments > 255)
            {
                return null;
            }

            ulong argument = IdaNative.get_tinfo_property(type.Typid, GtaFunctionArgument);
            ulong pointed = argument == 0 ? 0 : IdaNative.get_tinfo_property(argument, GtaPointedObject);
            uint ordinal = pointed == 0 ? 0 : (uint)IdaNative.get_tinfo_property(pointed, GtaFinalOrdinal);
            return ordinal != 0 && IdaNative.get_tinfo_property(pointed, GtaUdtMemberCount) is > 0 and < 100000
                ? ordinal
                : null;
        }
        finally { type.Dispose(); }
    }

    /// <summary>Direct call targets in the function whose first argument is the function's own this.</summary>
    private static IEnumerable<ulong> PassesThis(ulong function, ushort thisRegister)
    {
        var targets = new List<ulong>();
        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong end = FunctionEnd(function);
        for (ulong ea = function; ea < end && ea != BadAddr; ea = IdaNative.next_head(ea, end))
        {
            if (Insn.TryDecode(ea, buf) && (Insn.IsCall(buf) || IsJump(ea)) && Insn.OpType(buf, 0) == Insn.OpNear &&
                FunctionStart(Insn.OpAddr(buf, 0)) is var target && target != function && target != BadAddr &&
                ArgumentIsThis(ea, function, thisRegister))
            {
                targets.Add(target);
            }
        }

        return targets;
    }

    private static bool IsCallTo(ulong site, ulong callee)
    {
        byte* buf = stackalloc byte[Insn.BufferSize];
        return Insn.TryDecode(site, buf) && (Insn.IsCall(buf) || IsJump(site)) && Insn.OpType(buf, 0) == Insn.OpNear &&
               Insn.OpAddr(buf, 0) == callee;
    }

    /// <summary>
    /// Whether the first argument register holds the function's own object at the call. Its last write in
    /// the call's basic block must copy a register that holds this for the whole function, or there is no
    /// write at all between the function's entry and the call. A label on the way means another path can
    /// reach the call with another value, so the answer is no.
    /// </summary>
    private static bool ArgumentIsThis(ulong call, ulong function, ushort thisRegister)
    {
        byte* buf = stackalloc byte[Insn.BufferSize];
        ushort register = thisRegister;
        ulong ea = call;
        for (int i = 0; i < MaxBlockInsns; i++)
        {
            if (ea == function)
            {
                return register == thisRegister;
            }

            if (IsLabel(ea))
            {
                return false;
            }

            ea = IdaNative.prev_head(ea, function);
            if (ea == BadAddr || ea < function || !Insn.TryDecode(ea, buf) || Insn.IsCall(buf))
            {
                return false;
            }

            if (Insn.OpType(buf, 0) != Insn.OpReg || Insn.OpRegister(buf, 0) != register ||
                Mnemonic(ea) is "cmp" or "test" or "push")
            {
                continue;
            }

            return Mnemonic(ea) == "mov" && Insn.OpType(buf, 1) == Insn.OpReg &&
                   HoldsThis(function, Insn.OpRegister(buf, 1), thisRegister);
        }

        return false;
    }

    /// <summary>
    /// A callee-saved register every write of which in the function copies this, either from the incoming
    /// argument register before anything overwrote it or from another such register.
    /// </summary>
    private static bool HoldsThis(ulong function, ushort register, ushort thisRegister)
    {
        if (register == thisRegister)
        {
            return false;
        }

        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong end = FunctionEnd(function);
        bool written = false;
        for (ulong ea = function; ea < end && ea != BadAddr; ea = IdaNative.next_head(ea, end))
        {
            if (!Insn.TryDecode(ea, buf))
            {
                continue;
            }

            if (Insn.IsCall(buf))
            {
                // A call preserves callee-saved registers; a register it clobbers never holds this across it.
                if (!CalleeSaved(register))
                {
                    return false;
                }

                continue;
            }

            if (Insn.OpType(buf, 0) != Insn.OpReg || Insn.OpRegister(buf, 0) != register)
            {
                continue;
            }

            string mnemonic = Mnemonic(ea);
            if (mnemonic is "cmp" or "test")
            {
                continue;
            }

            // push/pop in the prologue and epilogue save and restore the caller's value.
            if (mnemonic is "push" or "pop")
            {
                continue;
            }

            if (mnemonic != "mov" || Insn.OpType(buf, 1) != Insn.OpReg ||
                !(Insn.OpRegister(buf, 1) == thisRegister && ArgumentIsThis(ea, function, thisRegister)))
            {
                return false;
            }

            written = true;
        }

        return written;
    }

    // rbx, rbp, rdi, rsi, r12-r15 on Microsoft x64; the System V set leaves rdi and rsi out.
    private static bool CalleeSaved(ushort register)
        => register is 3 or 5 or 12 or 13 or 14 or 15 ||
           (register is 6 or 7 && ConVarNaming.ArgumentRegisters().Length == 4);

    // A jump target: some code reference other than falling through from the previous instruction.
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

    private static bool IsJump(ulong ea) => Mnemonic(ea) == "jmp";

    /// <summary>The closest class every one of the classes derives from at offset 0, including itself.</summary>
    private static uint? CommonBase(List<uint> classes, Dictionary<uint, List<uint>> chains)
    {
        List<uint> first = Chain(classes[0], chains);
        foreach (uint candidate in first)
        {
            if (classes.All(x => Chain(x, chains).Contains(candidate)))
            {
                return candidate;
            }
        }

        return null;
    }

    private static List<uint> Chain(uint ordinal, Dictionary<uint, List<uint>> chains)
    {
        if (chains.TryGetValue(ordinal, out List<uint>? chain))
        {
            return chain;
        }

        chain = [];
        for (uint current = ordinal; current != 0 && !chain.Contains(current); current = PrimaryBase(current))
        {
            chain.Add(current);
        }

        return chains[ordinal] = chain;
    }

    private static uint PrimaryBase(uint ordinal)
    {
        if (TypeName(ordinal) is not string name || !ValveImplementationTypes.Load(name, out TypeInfo type))
        {
            return 0;
        }

        try
        {
            nuint members = IdaNative.get_tinfo_property(type.Typid, GtaUdtMemberCount);
            for (nuint i = 0; i < members && i < 8; i++)
            {
                if (!ValveImplementationTypes.ReadMember(type.Typid, i, out IdaUdtMember member))
                {
                    continue;
                }

                try
                {
                    if ((member.Flags & BaseClassMember) != 0 && member.Offset == 0)
                    {
                        return (uint)IdaNative.get_tinfo_property(member.Type.Typid, GtaFinalOrdinal);
                    }
                }
                finally { member.Dispose(); }
            }
        }
        finally { type.Dispose(); }

        return 0;
    }

    private static string? TypeName(uint ordinal)
    {
        byte* name = IdaNative.get_numbered_type_name(IdaNative.get_idati(), ordinal);
        return name == null ? null : Marshal.PtrToStringUTF8((nint)name);
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
