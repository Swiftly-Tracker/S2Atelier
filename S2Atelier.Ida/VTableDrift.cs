using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

/// <summary>
/// Holds back an SDK class's slot names once it drifted from the build since the previous one. Each run records,
/// per SDK class, one table's slots it names: a fingerprint of each function and the method the SDK gives it.
/// The next build's slots are matched to those by fingerprint, so a method inserted or removed in the middle
/// shows as the same functions one slot further or closer. Where the SDK would give a matched function another
/// method than before, the declaration is out of date: none of its names are applied, in any table, until the
/// SDK agrees again. The representative table is commented and the class reported.
/// </summary>
internal sealed unsafe class VTableDrift
{
    private const int MinInstructions = 4;
    private const int MaxInstructions = 4096;
    private const ulong MaxConstant = 0x10000;
    private const int FingerprintLength = 16;
    private const int ReportedSlots = 3;

    // Slots[i] = "<fingerprint or -> <method>" for the SDK class's slot i, in the table that represents it.
    private sealed record Declaration(string Table, List<string> Slots);

    private sealed record Snapshot(string Module, SortedDictionary<string, Declaration> Classes);

    private readonly SortedDictionary<string, Declaration>? _baseline;
    private readonly SortedDictionary<string, Declaration> _current = new(StringComparer.Ordinal);
    private readonly List<string> _held = [];
    private readonly HashSet<string> _drifted = new(StringComparer.Ordinal);
    private readonly string _module;

    internal VTableDrift(string module, string? baselinePath)
    {
        _module = module;
        if (baselinePath != null && File.Exists(baselinePath))
        {
            _baseline = JsonSerializer.Deserialize<Snapshot>(File.ReadAllText(baselinePath))?.Classes;
        }
    }

    internal static string SnapshotName(string module) => module + ".vtables.json";

    internal IReadOnlyList<string> Held => _held;

    /// <summary>The SDK slots to apply: every slot whose SDK class has not drifted.</summary>
    internal IReadOnlyDictionary<(ulong Table, int Index), SdkResolvedSlot> Filter(IReadOnlyList<SchemaVTable> tables,
        IReadOnlyDictionary<(ulong Table, int Index), SdkResolvedSlot> slots)
    {
        var byAddress = tables.GroupBy(x => x.AddressPoint).ToDictionary(x => x.Key, x => x.First());
        foreach (var declaration in slots.GroupBy(x => x.Value.SourceClass, StringComparer.Ordinal))
        {
            string sdkClass = declaration.Key;
            if (_current.ContainsKey(sdkClass))
            {
                continue;
            }

            // The class's own table when it has one, else the one whose slots say the most.
            var candidates = declaration.GroupBy(x => x.Key.Table)
                .Select(x => (Table: byAddress[x.Key], Slots: x.OrderBy(y => y.Key.Index).ToList()))
                .ToList();
            var chosen = candidates.FirstOrDefault(x => x.Table.ClassName == sdkClass && x.Table.ObjectOffset == 0);
            if (chosen.Table == null)
            {
                chosen = candidates.OrderByDescending(x => x.Slots.Count(y => Fingerprint(x.Table.Functions[y.Key.Index]) != null))
                    .ThenBy(x => x.Table.AddressPoint).First();
            }

            int count = chosen.Slots.Max(x => x.Key.Index) + 1;
            var names = new string?[count];
            var fingerprints = new string?[count];
            foreach (var slot in chosen.Slots)
            {
                names[slot.Key.Index] = slot.Value.Name;
                fingerprints[slot.Key.Index] = Fingerprint(chosen.Table.Functions[slot.Key.Index]);
            }

            var previous = _baseline?.GetValueOrDefault(sdkClass);
            var pairs = previous == null ? [] : Match(previous.Slots.Select(Parse).ToList(), fingerprints);
            var conflicts = previous == null ? [] : Conflicts(previous.Slots, pairs, names);
            if (conflicts.Count == 0)
            {
                _current[sdkClass] = new(Key(chosen.Table), [.. names.Select((name, i) => Format(fingerprints[i], name))]);
                continue;
            }

            // Keep the evidence for the next build: matched functions keep the methods they had.
            var carried = new string?[count];
            foreach ((int old, int now) in pairs)
            {
                carried[now] = Parse(previous!.Slots[old]).Name;
            }

            _current[sdkClass] = new(Key(chosen.Table), [.. carried.Select((name, i) => Format(fingerprints[i], name))]);
            _drifted.Add(sdkClass);
            string detail = string.Join("; ", conflicts.Take(ReportedSlots).Select(p =>
                $"slot {p.New} holds {Parse(previous!.Slots[p.Old]).Name} (slot {p.Old} before), the SDK says {names[p.New]}"));
            string message = $"{sdkClass}: SDK names held, the declaration no longer matches the build " +
                             $"(checked on {chosen.Table.ClassName}): {detail}" +
                             (conflicts.Count > ReportedSlots ? $"; {conflicts.Count - ReportedSlots} more" : "") + ".";
            _held.Add(message);
            Comment(chosen.Table.AddressPoint, "S2Atelier: " + message);
        }

        return slots.Where(x => !_drifted.Contains(x.Value.SourceClass)).ToDictionary(x => x.Key, x => x.Value);
    }

    internal void Write(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        File.WriteAllText(path, JsonSerializer.Serialize(new Snapshot(_module, _current),
            new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>
    /// The matched slots whose function the SDK now calls something else than the recorded method: the
    /// declaration moved away from the build there. previous holds the recorded "fingerprint method" slots.
    /// </summary>
    internal static List<(int Old, int New)> Conflicts(IReadOnlyList<string> previous, string?[] fingerprints,
        string?[] names)
        => Conflicts(previous, Match(previous.Select(Parse).ToList(), fingerprints), names);

    private static List<(int Old, int New)> Conflicts(IReadOnlyList<string> previous, List<(int Old, int New)> pairs,
        string?[] names)
        => [.. pairs.Where(p => Parse(previous[p.Old]).Name is string was && names[p.New] is string now && was != now)];

    private static string Key(SchemaVTable table) => $"{table.ClassName}@{table.ObjectOffset ?? 0}";

    private static string Format(string? fingerprint, string? name) => $"{fingerprint ?? "-"} {name ?? "-"}";

    private static (string? Fingerprint, string? Name) Parse(string slot)
    {
        int space = slot.IndexOf(' ');
        string fingerprint = space < 0 ? slot : slot[..space];
        string name = space < 0 ? "-" : slot[(space + 1)..];
        return (fingerprint == "-" ? null : fingerprint, name == "-" ? null : name);
    }

    /// <summary>
    /// Old and new slots holding the same function: fingerprints that occur once in each table. A trivial or
    /// repeated body (a stub returning a constant) says nothing about which slot it came from.
    /// </summary>
    private static List<(int Old, int New)> Match(List<(string? Fingerprint, string? Name)> previous, string?[] current)
    {
        var oldIndex = Unique(previous.Select(x => x.Fingerprint));
        var newIndex = Unique(current);
        return [.. newIndex.Where(x => oldIndex.ContainsKey(x.Key)).Select(x => (oldIndex[x.Key], x.Value)).OrderBy(x => x.Item2)];

        static Dictionary<string, int> Unique(IEnumerable<string?> fingerprints)
            => fingerprints.Select((fingerprint, index) => (fingerprint, index))
                .Where(x => x.fingerprint != null)
                .GroupBy(x => x.fingerprint!, StringComparer.Ordinal)
                .Where(g => g.Count() == 1)
                .ToDictionary(g => g.Key, g => g.Single().index, StringComparer.Ordinal);
    }

    /// <summary>
    /// What survives a recompile: the instruction mnemonics, small constants and the imports called. Registers,
    /// addresses and field displacements are left out, since they move between builds.
    /// </summary>
    private static string? Fingerprint(ulong function)
    {
        void* pfn = IdaNative.get_func(function);
        if (pfn == null || *(ulong*)pfn != function)
        {
            return null;
        }

        ulong end = *((ulong*)pfn + 1);
        var text = new StringBuilder();
        byte* buf = stackalloc byte[Insn.BufferSize];
        int count = 0;
        for (ulong ea = function; ea < end && ea != ulong.MaxValue && count < MaxInstructions;
             ea = IdaNative.next_head(ea, end))
        {
            if (!Insn.TryDecode(ea, buf))
            {
                continue;
            }

            count++;
            text.Append(Mnemonic(ea));
            if (Insn.OpType(buf, 1) == Insn.OpImm && Insn.OpValue(buf, 1) < MaxConstant)
            {
                text.Append(' ').Append(Insn.OpValue(buf, 1));
            }

            if (Insn.IsCall(buf) && Insn.OpType(buf, 0) is Insn.OpNear or Insn.OpMem &&
                ImportName(Insn.OpAddr(buf, 0)) is string import)
            {
                text.Append(' ').Append(import);
            }

            text.Append('\n');
        }

        if (count < MinInstructions)
        {
            return null;
        }

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString())))[..FingerprintLength];
    }

    // A call into another module keeps its name from build to build; a local function's name is ours.
    private static string? ImportName(ulong target)
    {
        void* pfn = IdaNative.get_func(target);
        if (pfn != null && (*((ulong*)pfn + 2) & 0x80) == 0)
        {
            return null;
        }

        var text = new QString();
        try { return IdaNative.get_ea_name(&text, target, 0, null) > 0 ? text.Read() : null; }
        finally { text.Dispose(); }
    }

    private static void Comment(ulong ea, string text)
    {
        byte* native = Utf8.Allocate(text);
        try { IdaNative.set_cmt(ea, native, 0); }
        finally { Utf8.Free(native); }
    }

    private static string Mnemonic(ulong ea)
    {
        var text = new QString();
        try { return IdaNative.print_insn_mnem(&text, ea) > 0 ? text.Read() : string.Empty; }
        finally { text.Dispose(); }
    }
}
