using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

/// <summary>
/// Every constant address the module stores to a fixed location: <c>lea reg, obj; mov [target], reg</c>
/// under MSVC, the same through a register holding the target (<c>lea rax, obj; mov [rax+58h], rdx</c>) or
/// as two quadwords of an SSE register under Clang, and what initialized data holds. Register values follow
/// the control flow within a function: a jump target knows what every jump there agrees on, and a call
/// keeps only the registers it preserves, plus its result when the callee always returns one address
/// (<c>lea rax, info; ret</c>, a static's accessor).
/// </summary>
internal sealed unsafe class StoreMap
{
    private const byte QwordType = 7;
    private const int OpsOffset = 40, OpSize = 40, SpecFlag1 = 32, SpecFlag2 = 33, InsnPrefix = 33;
    private const byte RexX = 0x02;
    private const int NoIndex = 4;
    private const int MaxPasses = 8;
    private const int MaxCallDepth = 4;
    private const ushort Rax = 0;

    private readonly Dictionary<ulong, List<(ulong Value, ulong Function)>> _entries = new();
    private readonly Dictionary<ulong, List<ulong>> _targets = new();
    private readonly Dictionary<ulong, List<ulong>> _arguments = new();
    // Scanned functions and the address each always returns, if any.
    private readonly Dictionary<ulong, ulong?> _returns = new();
    private readonly Dictionary<ushort, string> _mnemonics = new();
    private ushort[] _preserved = [];
    private ushort _firstArgument;

    internal IReadOnlyDictionary<ulong, List<(ulong Value, ulong Function)>> Entries => _entries;

    /// <summary>Addresses stored at a location by code, then its initialized contents if they are an address.</summary>
    internal IEnumerable<ulong> Pointers(ulong location)
    {
        if (_entries.TryGetValue(location, out var stored))
        {
            foreach (var entry in stored)
            {
                yield return entry.Value;
            }
        }

        // Uninitialized data (MSVC's .data tail) has no contents to read.
        ulong initial = IdaNative.is_loaded(location) != 0 ? IdaNative.get_qword(location) : 0;
        if (initial != 0 && IdaNative.is_mapped(initial) != 0)
        {
            yield return initial;
        }
    }

    /// <summary>The locations code stores this address to.</summary>
    internal IReadOnlyList<ulong> TargetsOf(ulong value) => _targets.TryGetValue(value, out var targets) ? targets : [];

    /// <summary>The functions that always return this address.</summary>
    internal IReadOnlyList<ulong> ReturnersOf(ulong value)
    {
        _returners ??= _returns.Where(x => x.Value != null).GroupBy(x => x.Value!.Value)
            .ToDictionary(g => g.Key, g => g.Select(x => x.Key).Order().ToList());
        return _returners.TryGetValue(value, out var functions) ? functions : [];
    }

    private Dictionary<ulong, List<ulong>>? _returners;

    /// <summary>Constant addresses a function passes as the first argument of its calls.</summary>
    internal IReadOnlyCollection<ulong> CallArguments(ulong function)
        => _arguments.TryGetValue(function, out var arguments) ? arguments : [];

    internal static StoreMap Build(ushort[] preserved, ushort firstArgument)
    {
        var map = new StoreMap { _preserved = preserved, _firstArgument = firstArgument };
        for (nuint i = 0, count = IdaNative.get_func_qty(); i < count; i++)
        {
            void* function = IdaNative.getn_func(i);
            if (function != null)
            {
                map.Returned(*(ulong*)function, 0);
            }
        }

        return map;
    }

    // Scans a function once; the address it always returns, if any.
    private ulong? Returned(ulong function, int depth)
    {
        if (_returns.TryGetValue(function, out ulong? known))
        {
            return known;
        }

        void* pfn = IdaNative.get_func(function);
        if (pfn == null || *(ulong*)pfn != function || depth > MaxCallDepth)
        {
            return null;
        }

        // A recursive call sees no result until the function is done.
        _returns[function] = null;
        return _returns[function] = Scan(function, *((ulong*)pfn + 1), depth);
    }

    // A register: a 64-bit value, and for an SSE register its high quadword too.
    private readonly record struct Lanes(ulong? Low, ulong? High);

    private ulong? Scan(ulong function, ulong end, int depth)
    {
        byte* insn = stackalloc byte[Insn.BufferSize];
        var mnemonics = _mnemonics;
        ushort[] preserved = _preserved;
        ushort firstArgument = _firstArgument;
        ulong? returned = null;
        bool returns = false, varies = false;
        // What is known on entry to each jump target. Code no known path has reached yet assumes nothing
        // and passes nothing on; a jump from further down reaches it on the next pass, so a function with
        // one is read again until the targets settle.
        var entries = new Dictionary<ulong, Dictionary<ushort, Lanes>>();
        var stores = new List<(ulong Target, ulong Value)>();
        var arguments = new List<ulong>();
        for (int pass = 0; pass < MaxPasses; pass++)
        {
            stores.Clear();
            arguments.Clear();
            (returned, returns, varies) = (null, false, false);
            bool changed = false, backwards = false, falls = true;
            // Null: not reached by any path seen so far.
            Dictionary<ushort, Lanes>? registers = new();
            for (ulong ea = function; ea < end && ea != ulong.MaxValue; ea = IdaNative.next_head(ea, end))
            {
                if (entries.TryGetValue(ea, out var incoming))
                {
                    registers = falls && registers != null ? Merge(registers, incoming) : new(incoming);
                }
                else if (!falls)
                {
                    registers = null;
                }

                falls = true;
                if (!Insn.TryDecode(ea, insn))
                {
                    registers?.Clear();
                    continue;
                }

                if (registers == null)
                {
                    falls = Insn.IsCall(insn) || !EndsFlow(Mnemonic(insn, ea, mnemonics));
                    continue;
                }

                if (Insn.IsCall(insn))
                {
                    if (registers.TryGetValue(firstArgument, out Lanes argument) && argument.Low is ulong passed &&
                        IdaNative.is_mapped(passed) != 0)
                    {
                        arguments.Add(passed);
                    }

                    foreach (ushort register in registers.Keys.Where(x => !preserved.Contains(x)).ToList())
                    {
                        registers.Remove(register);
                    }

                    if (Insn.OpType(insn, 0) == Insn.OpNear && Returned(Insn.OpAddr(insn, 0), depth + 1) is ulong result)
                    {
                        registers[Rax] = new Lanes(result, null);
                    }

                    continue;
                }

                string mnemonic = Mnemonic(insn, ea, mnemonics);
                ulong next = IdaNative.next_head(ea, end);
                for (ulong target = IdaNative.get_first_cref_from(ea); target != ulong.MaxValue;
                     target = IdaNative.get_next_cref_from(ea, target))
                {
                    if (target != next && target >= function && target < end)
                    {
                        changed |= Propagate(entries, target, registers);
                        backwards |= target <= ea;
                    }
                }

                if (EndsFlow(mnemonic))
                {
                    if (mnemonic is "retn" or "ret")
                    {
                        ulong? value = registers.GetValueOrDefault(Rax).Low;
                        varies |= value == null || returns && value != returned;
                        (returned, returns) = (value, true);
                    }

                    falls = false;
                    continue;
                }

                Step(insn, mnemonic, registers, stores);
            }

            if (!changed || !backwards)
            {
                break;
            }
        }

        foreach ((ulong target, ulong value) in stores)
        {
            Store(target, value, function);
        }

        foreach (ulong argument in arguments)
        {
            Add(_arguments, function, argument);
        }

        return returns && !varies && returned is ulong address && IdaNative.is_mapped(address) != 0 ? address : null;
    }

    private static bool EndsFlow(string mnemonic) => mnemonic is "jmp" or "retn" or "ret" or "int3" or "ud2" or "hlt";

    // One instruction's effect on the registers, and the store it makes.
    private static void Step(byte* insn, string mnemonic, Dictionary<ushort, Lanes> registers,
        List<(ulong Target, ulong Value)> stores)
    {
        byte first = Insn.OpType(insn, 0), second = Insn.OpType(insn, 1);
        if (first == Insn.OpReg)
        {
            ushort target = Insn.OpRegister(insn, 0);
            Lanes? next = mnemonic switch
            {
                "lea" when Location(insn, 1, registers) is ulong address => new Lanes(address, null),
                "mov" or "movq" when second == Insn.OpReg && registers.TryGetValue(Insn.OpRegister(insn, 1), out Lanes copied)
                    => new Lanes(copied.Low, mnemonic == "movq" ? 0 : copied.High),
                "mov" when second == Insn.OpImm => new Lanes(Insn.OpValue(insn, 1), null),
                // movq xmm, [constant]: a pointer from a literal pool.
                "movq" when Location(insn, 1, registers) is ulong pool => new Lanes(Loaded(pool), 0),
                "xor" or "xorps" or "pxor" or "xorpd" when second == Insn.OpReg && Insn.OpRegister(insn, 1) == target
                    => new Lanes(0, 0),
                "pinsrq" when Insn.OpType(insn, 2) == Insn.OpImm && Insn.OpValue(insn, 2) == 1 &&
                              second == Insn.OpReg && registers.TryGetValue(target, out Lanes low) &&
                              registers.TryGetValue(Insn.OpRegister(insn, 1), out Lanes inserted)
                    => new Lanes(low.Low, inserted.Low),
                "punpcklqdq" or "movlhps" when second == Insn.OpReg && registers.TryGetValue(target, out Lanes low) &&
                                               registers.TryGetValue(Insn.OpRegister(insn, 1), out Lanes high)
                    => new Lanes(low.Low, high.Low),
                "cmp" or "test" or "push" or "ucomiss" or "comiss" => registers.GetValueOrDefault(target),
                _ => null,
            };
            if (next is Lanes value && (value.Low != null || value.High != null))
            {
                registers[target] = value;
            }
            else
            {
                registers.Remove(target);
            }

            return;
        }

        if (Location(insn, 0, registers) is not ulong destination)
        {
            return;
        }

        if (second == Insn.OpImm)
        {
            if (mnemonic == "mov" && Insn.OpWidth(insn, 0) == QwordType)
            {
                stores.Add((destination, Insn.OpValue(insn, 1)));
            }

            return;
        }

        if (second != Insn.OpReg || !registers.TryGetValue(Insn.OpRegister(insn, 1), out Lanes source))
        {
            return;
        }

        if (mnemonic is "mov" or "movq" && Insn.OpWidth(insn, 0) == QwordType && source.Low is ulong stored)
        {
            stores.Add((destination, stored));
        }
        else if (mnemonic is "movups" or "movdqu" or "movaps" or "movdqa" or "movupd" or "movapd")
        {
            if (source.Low is ulong low) stores.Add((destination, low));
            if (source.High is ulong high) stores.Add((destination + 8, high));
        }
    }

    // What both states agree on, lane by lane.
    private static Dictionary<ushort, Lanes> Merge(Dictionary<ushort, Lanes> left, Dictionary<ushort, Lanes> right)
    {
        var merged = new Dictionary<ushort, Lanes>();
        foreach ((ushort register, Lanes value) in left)
        {
            if (right.TryGetValue(register, out Lanes other))
            {
                var lanes = new Lanes(value.Low == other.Low ? value.Low : null, value.High == other.High ? value.High : null);
                if (lanes.Low != null || lanes.High != null)
                {
                    merged[register] = lanes;
                }
            }
        }

        return merged;
    }

    // Joins a jump's state into its target; true when the target's state changed.
    private static bool Propagate(Dictionary<ulong, Dictionary<ushort, Lanes>> entries, ulong target,
        Dictionary<ushort, Lanes> state)
    {
        if (!entries.TryGetValue(target, out var known))
        {
            entries[target] = new(state);
            return true;
        }

        var merged = Merge(known, state);
        if (merged.Count == known.Count && merged.All(x => known[x.Key] == x.Value))
        {
            return false;
        }

        entries[target] = merged;
        return true;
    }

    private void Store(ulong target, ulong value, ulong function)
    {
        if (value == 0 || IdaNative.is_mapped(value) == 0)
        {
            return;
        }

        if (!_entries.TryGetValue(target, out var entries))
        {
            _entries.Add(target, entries = []);
        }

        entries.Add((value, function));
        Add(_targets, value, target);
    }

    private static void Add(Dictionary<ulong, List<ulong>> map, ulong key, ulong value)
    {
        if (!map.TryGetValue(key, out var list))
        {
            map.Add(key, list = []);
        }

        list.Add(value);
    }

    // The fixed location a memory operand names: [rip+disp], or [reg+disp] from a register holding a constant.
    private static ulong? Location(byte* insn, int operand, Dictionary<ushort, Lanes> registers)
    {
        byte type = Insn.OpType(insn, operand);
        if (type == Insn.OpMem)
        {
            return Insn.OpAddr(insn, operand);
        }

        if (type is not (Insn.OpDispl or Insn.OpPhrase) || Indexed(insn, operand) ||
            !registers.TryGetValue(Insn.OpRegister(insn, operand), out Lanes baseValue) || baseValue.Low is not ulong address)
        {
            return null;
        }

        return type == Insn.OpDispl ? unchecked(address + Insn.OpAddr(insn, operand)) : address;
    }

    // An operand with a SIB index register adds a value the scan does not know.
    private static bool Indexed(byte* insn, int operand)
    {
        byte* op = insn + OpsOffset + operand * OpSize;
        if (op[SpecFlag1] == 0)
        {
            return false;
        }

        int index = (op[SpecFlag2] >> 3) & 7;
        return index != NoIndex || (insn[InsnPrefix] & RexX) != 0;
    }

    private static ulong? Loaded(ulong location)
    {
        if (IdaNative.is_mapped(location) == 0)
        {
            return null;
        }

        ulong value = IdaNative.get_qword(location);
        return value != 0 && IdaNative.is_mapped(value) != 0 ? value : null;
    }

    private static string Mnemonic(byte* insn, ulong ea, Dictionary<ushort, string> cache)
    {
        ushort itype = Insn.Itype(insn);
        if (cache.TryGetValue(itype, out string? known))
        {
            return known;
        }

        var text = new QString();
        try
        {
            string mnemonic = IdaNative.print_insn_mnem(&text, ea) > 0 ? text.Read() : string.Empty;
            cache[itype] = mnemonic;
            return mnemonic;
        }
        finally { text.Dispose(); }
    }
}
