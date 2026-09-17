using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

internal sealed record ConstructorNamingSummary(int Found, int Named, int ThisBound);

/// <summary>Finds and names constructors from the primary vtables they store into <c>this</c>.</summary>
internal static unsafe partial class ConstructorNaming
{
    // IDA x86 register numbers.
    private const ushort Rcx = 1, Rsp = 4, Rbp = 5, Rdi = 7;
    private static readonly ushort[] MsvcVolatile = [0, 1, 2, 8, 9, 10, 11];
    private static readonly ushort[] SystemVVolatile = [0, 1, 2, 6, 7, 8, 9, 10, 11];

    // A register or stack slot value: this + Offset, or a data address (a vtable) + Offset.
    private readonly record struct Value(bool IsThis, ulong Base, long Offset);

    internal static ConstructorNamingSummary Apply(SchemaSelection selection, SchemaTargetPlatform platform,
        SchemaImport.LimitedDiagnostics diagnostics)
    {
        var (primary, symbols, slotTargets) = platform == SchemaTargetPlatform.WindowsMsvc
            ? ReadMsvcTables() : ReadItaniumTables();
        ushort thisRegister = platform == SchemaTargetPlatform.WindowsMsvc ? Rcx : Rdi;
        ushort[] clobbered = platform == SchemaTargetPlatform.WindowsMsvc ? MsvcVolatile : SystemVVolatile;

        // MSVC loads a vtable by its symbol; Itanium code may load the symbol and add the address point offset.
        var functions = primary.Keys.Concat(symbols).Distinct().SelectMany(Xrefs.DataTo).Select(FunctionStart)
            .Where(x => x != ulong.MaxValue).ToHashSet();
        var writers = new List<VptrWriter>();
        foreach (ulong function in functions.Order())
            if (Scan(function, thisRegister, clobbered, primary, slotTargets) is VptrWriter writer) writers.Add(writer);

        var constructors = ConstructorAnalysis.SelectConstructors(writers);
        int named = 0, thisBound = 0;
        foreach ((string className, ulong address) in constructors.OrderBy(x => x.Value))
        {
            bool typed = selection.Classes.ContainsKey(className) &&
                SdkFunctionBinding.CanUpdateType(address) &&
                SchemaImport.TryBindThisParameter(address, className, diagnostics);
            thisBound += typed ? 1 : 0;
            if (SdkFunctionBinding.TryNameFunction(address, ConstructorAnalysis.ConstructorName(className), typed,
                    "constructor", diagnostics.Write))
                named++;
        }
        return new(constructors.Count, named, thisBound);
    }

    private static (Dictionary<ulong, string> Primary, List<ulong> Symbols, HashSet<ulong> SlotTargets) ReadMsvcTables()
    {
        var primary = new Dictionary<ulong, string>();
        var symbols = new List<ulong>();
        var slotTargets = new HashSet<ulong>();
        foreach ((ulong address, string rawName) in Names())
        {
            if (!rawName.StartsWith("??_7", StringComparison.Ordinal)) continue;
            symbols.Add(address);
            for (ulong slot = address; IsFunctionStart(IdaNative.get_qword(slot)); slot += 8)
                slotTargets.Add(IdaNative.get_qword(slot));
            // IDA suffixes repeated symbols (_0, _1, ...); the undecorated symbol names the class.
            string symbol = DuplicateSuffix().Replace(rawName, "");
            if (VTableAnalysis.TryDescribe(address, symbol, Demangle(symbol), out VTableDescriptor descriptor) &&
                descriptor.SecondaryBaseName == null)
                primary[address] = descriptor.ClassName;
        }
        return (primary, symbols, slotTargets);
    }

    private static (Dictionary<ulong, string> Primary, List<ulong> Symbols, HashSet<ulong> SlotTargets) ReadItaniumTables()
    {
        var names = Names();
        var boundaries = new SortedSet<ulong>(names.Select(x => x.Address));
        var primary = new Dictionary<ulong, string>();
        var symbols = new List<ulong>();
        var slotTargets = new HashSet<ulong>();
        var memory = new Memory();
        foreach ((ulong address, string rawName) in names)
        {
            if (!rawName.StartsWith("_ZTV", StringComparison.Ordinal) ||
                !VTableAnalysis.TryDescribe(address, rawName, Demangle(rawName), out VTableDescriptor descriptor))
                continue;
            symbols.Add(address);
            ulong end = boundaries.GetViewBetween(address + 1, ulong.MaxValue).FirstOrDefault();
            foreach (VTableSlice slice in VTableEntryScanner.ScanItaniumTables(memory, address,
                         endExclusive: end == 0 ? null : end))
            {
                if (slice.OffsetToTop == 0) primary.TryAdd(slice.AddressPoint, descriptor.ClassName);
                slotTargets.UnionWith(slice.Functions);
            }
        }
        return (primary, symbols, slotTargets);
    }

    private static VptrWriter? Scan(ulong function, ushort thisRegister, ushort[] clobbered,
        IReadOnlyDictionary<ulong, string> primary, HashSet<ulong> slotTargets)
    {
        byte* insn = stackalloc byte[Insn.BufferSize];
        var registers = new Dictionary<ushort, Value> { [thisRegister] = new(true, 0, 0) };
        // Unoptimized paths keep this in a frame slot across calls.
        var frame = new Dictionary<(ushort Register, long Offset), Value>();
        var written = new List<string>();
        bool callBefore = false;
        ulong? firstCall = null;
        ulong end = FunctionEnd(function);
        for (ulong ea = function; ea < end; ea = IdaNative.next_head(ea, end))
        {
            if (!Insn.TryDecode(ea, insn)) continue;
            byte first = Insn.OpType(insn, 0), second = Insn.OpType(insn, 1);
            if (Insn.IsCall(insn))
            {
                if (written.Count == 0)
                {
                    if (!callBefore && first == Insn.OpNear) firstCall = Insn.OpAddr(insn, 0);
                    callBefore = true;
                }
                foreach (ushort register in clobbered) registers.Remove(register);
                continue;
            }
            string mnemonic = Mnemonic(ea);
            if (first == Insn.OpReg)
            {
                ushort target = Insn.OpRegister(insn, 0);
                Value? next = null;
                if (mnemonic == "lea" && second == Insn.OpMem)
                    next = new(false, Insn.OpAddr(insn, 1), 0);
                else if (mnemonic == "lea" && second is Insn.OpDispl or Insn.OpPhrase &&
                         registers.TryGetValue(Insn.OpRegister(insn, 1), out Value source))
                    next = source with { Offset = source.Offset + Displacement(insn, 1) };
                else if (mnemonic == "mov" && second == Insn.OpReg && registers.TryGetValue(Insn.OpRegister(insn, 1), out Value copied))
                    next = copied;
                else if (mnemonic == "mov" && second == Insn.OpDispl && IsFrame(Insn.OpRegister(insn, 1), registers) &&
                         frame.TryGetValue((Insn.OpRegister(insn, 1), Displacement(insn, 1)), out Value saved))
                    next = saved;
                else if (mnemonic is "add" or "sub" && second == Insn.OpImm && registers.TryGetValue(target, out Value current))
                    next = current with { Offset = current.Offset + (mnemonic == "add" ? 1 : -1) * (long)Insn.OpValue(insn, 1) };
                else if (mnemonic is "cmp" or "test" or "push")
                    continue;
                if (next is Value value) registers[target] = value;
                else registers.Remove(target);
                continue;
            }
            if (mnemonic != "mov" || first is not (Insn.OpDispl or Insn.OpPhrase)) continue;
            if (first == Insn.OpDispl && IsFrame(Insn.OpRegister(insn, 0), registers))
            {
                var slot = (Insn.OpRegister(insn, 0), Displacement(insn, 0));
                if (second == Insn.OpReg && registers.TryGetValue(Insn.OpRegister(insn, 1), out Value kept)) frame[slot] = kept;
                else frame.Remove(slot);
                continue;
            }
            if (second == Insn.OpReg &&
                registers.TryGetValue(Insn.OpRegister(insn, 0), out Value destination) && destination.IsThis &&
                destination.Offset + Displacement(insn, 0) == 0 &&
                registers.TryGetValue(Insn.OpRegister(insn, 1), out Value stored) && !stored.IsThis &&
                primary.TryGetValue(unchecked(stored.Base + (ulong)stored.Offset), out string? className))
                written.Add(className);
        }
        if (written.Count == 0) return null;

        bool referenced = slotTargets.Contains(function) ||
            Xrefs.CodeTo(function).Any(caller => slotTargets.Contains(FunctionStart(caller))) ||
            // Unwind and EH tables hold 32-bit RVAs; a full pointer comes from a table such as an unnamed vtable.
            Xrefs.DataTo(function).Any(source => IdaNative.get_qword(source) == function);
        return new VptrWriter(function, written, callBefore, firstCall, referenced);
    }

    // rbp is a frame pointer unless it holds a tracked value (MSVC often keeps this in rbp).
    private static bool IsFrame(ushort register, Dictionary<ushort, Value> registers)
        => register == Rsp || register == Rbp && !registers.ContainsKey(Rbp);

    private static long Displacement(byte* insn, int operand)
        => Insn.OpType(insn, operand) == Insn.OpDispl ? unchecked((long)Insn.OpAddr(insn, operand)) : 0;

    private static List<(ulong Address, string Name)> Names()
    {
        var names = new List<(ulong, string)>();
        for (nuint i = 0, count = IdaNative.get_nlist_size(); i < count; i++)
        {
            byte* rawPointer = IdaNative.get_nlist_name(i);
            if (rawPointer != null)
                names.Add((IdaNative.get_nlist_ea(i), Marshal.PtrToStringUTF8((nint)rawPointer) ?? string.Empty));
        }
        return names;
    }

    private static string Mnemonic(ulong ea)
    {
        var text = new QString();
        try { return IdaNative.print_insn_mnem(&text, ea) > 0 ? text.Read() : string.Empty; }
        finally { text.Dispose(); }
    }

    private static bool IsFunctionStart(ulong address)
        => address != 0 && IdaNative.is_mapped(address) != 0 && FunctionStart(address) == address;

    private static ulong FunctionStart(ulong address)
    {
        void* function = IdaNative.get_func(address);
        return function == null ? ulong.MaxValue : *(ulong*)function;
    }

    private static ulong FunctionEnd(ulong address)
    {
        void* function = IdaNative.get_func(address);
        return function == null ? address : *(ulong*)((byte*)function + 8);
    }

    private static string? Demangle(string rawName)
    {
        byte* native = Utf8.Allocate(rawName);
        var output = new QString();
        try { return IdaNative.demangle_name(&output, native, 0, 2) > 0 ? output.Read() : null; }
        finally
        {
            output.Dispose();
            Utf8.Free(native);
        }
    }

    private sealed class Memory : IVTableMemory
    {
        public ulong ReadPointer(ulong address) => IdaNative.get_qword(address);
        public bool IsMapped(ulong address) => IdaNative.is_mapped(address) != 0;
        public bool IsFunctionStart(ulong address) => ConstructorNaming.IsFunctionStart(address);
    }

    [GeneratedRegex("(?<=@)_[0-9]+$", RegexOptions.CultureInvariant)]
    private static partial Regex DuplicateSuffix();
}
