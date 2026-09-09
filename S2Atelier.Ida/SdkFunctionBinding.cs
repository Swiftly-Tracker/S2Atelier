using System.Security.Cryptography;
using System.Text.Json;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

internal sealed record SdkFunctionOwnership(string? Name, string? Prototype, string Source);

internal static unsafe class SdkFunctionBinding
{
    internal const string Marker = "S2Atelier HL2SDK vfunc v1 ";

    internal static VTableBindingSummary Bind(IReadOnlyList<SchemaVTable> tables,
        IReadOnlyDictionary<(ulong Table, int Index), SdkResolvedSlot> slots,
        IReadOnlySet<ulong> pureCalls, IReadOnlySet<ulong> unresolved, Action<string> diagnostic)
    {
        var candidates = tables.SelectMany(table => table.Functions.Select((address, index) =>
            (Address: address, Table: table, Slot: slots.GetValueOrDefault((table.AddressPoint, index)))))
            .GroupBy(x => x.Address).Where(g => g.Any(x => x.Slot != null));
        int bound = 0, skipped = 0, conflicts = 0;
        foreach (var group in candidates.OrderBy(x => x.Key))
        {
            ulong address = group.Key;
            void* nativeFunction = IdaNative.get_func(address);
            if (address == 0 || pureCalls.Contains(address) || unresolved.Contains(address) ||
                nativeFunction == null || *(ulong*)nativeFunction != address)
            { skipped++; continue; }
            var chosen = group.First(x => x.Slot != null);
            string method = chosen.Slot!.Name;
            string owner = chosen.Table.ThisType!;
            TypeInfo expected = default;
            bool compatible = chosen.Slot.TryGetFunction(out expected);
            try
            {
                foreach (var candidate in group)
                {
                    if (candidate.Table.ThisType != owner) { compatible = false; break; }
                    if (candidate.Slot == null) continue;
                    if (candidate.Slot.Name != method || !candidate.Slot.TryGetFunction(out var other))
                    { compatible = false; break; }
                    try { compatible &= IdaNative.compare_tinfo(expected.Typid, other.Typid, 0) != 0; }
                    finally { other.Dispose(); }
                }
                if (!compatible)
                {
                    conflicts++;
                    diagnostic($"[schema-sdk] 0x{address:X}: shared function has incompatible SDK names/prototypes/this; preserved.");
                    continue;
                }
                string oldComment = ReadComment(address);
                var metadata = ReadOwnership(oldComment);
                string currentName = SchemaVTableTypes.NameAt(address);
                string? appliedName = metadata?.Name, appliedType = metadata?.Prototype;
                bool changed = false;
                // IDA marks both explicit user types and apply_tinfo(TINFO_DEFINITE) as USERTI.
                // A stored fingerprint distinguishes our previous output from subsequent edits.
                if (CanUpdateType(address, metadata))
                {
                    if (IdaNative.apply_tinfo(address, &expected, 1) != 0)
                    {
                        appliedType = FingerprintAt(address);
                        changed = true;
                    }
                    else diagnostic($"[schema-sdk] 0x{address:X}: IDA rejected SDK prototype.");
                }
                else diagnostic($"[schema-sdk] 0x{address:X}: explicit/edited function type preserved.");

                if (CanUpdateName(currentName, metadata))
                {
                    string wanted = owner + "::" + method;
                    // Overloads and distinct table instances must not steal an existing address name.
                    if (!NameAvailable(wanted, address)) wanted += $"_ea_{address:X}";
                    byte* name = Utf8.Allocate(wanted);
                    try
                    {
                        if (NameAvailable(wanted, address) && IdaNative.set_name(address, name, 0x01 | 0x40) != 0)
                        { appliedName = SchemaVTableTypes.NameAt(address); changed = true; }
                        else diagnostic($"[schema-sdk] 0x{address:X}: SDK function name unavailable/rejected.");
                    }
                    finally { Utf8.Free(name); }
                }
                if (changed)
                {
                    string source = $"{chosen.Slot.Header}: {chosen.Slot.SourceClass}::{method}";
                    string comment = MergeOwnership(oldComment, new(appliedName, appliedType, source));
                    byte* text = Utf8.Allocate(comment);
                    try
                    {
                        if (IdaNative.set_cmt(address, text, 1) == 0)
                            diagnostic($"[schema-sdk] 0x{address:X}: ownership comment could not be saved.");
                    }
                    finally { Utf8.Free(text); }
                    bound++;
                }
                else skipped++;
            }
            finally { expected.Dispose(); }
        }
        return new(bound, skipped, conflicts);
    }

    internal static bool CanUpdateName(string name, SdkFunctionOwnership? ownership)
        => ownership?.Name is string applied ? applied == name : ValveInterfaceNaming.CanReplace(name);

    internal static bool CanUpdateType(ulong address, SdkFunctionOwnership? ownership = null)
    {
        ownership ??= ReadOwnership(ReadComment(address));
        return ownership?.Prototype is string expected
            ? FingerprintAt(address) == expected
            : (IdaNative.get_aflags(address) & 0x02000000) == 0;
    }

    internal static string? FingerprintAt(ulong address)
    {
        TypeInfo type = default;
        try
        {
            if (IdaNative.get_tinfo(&type, address) == 0) return null;
            var portable = SdkPortableType.Capture(ref type, IdaNative.get_idati());
            return portable == null ? null : Convert.ToHexString(SHA256.HashData(
                portable.Type.Concat(portable.Fields).ToArray()));
        }
        finally { type.Dispose(); }
    }

    private static bool NameAvailable(string name, ulong address)
    {
        byte* text = Utf8.Allocate(name);
        try { ulong existing = IdaNative.get_name_ea(ulong.MaxValue, text); return existing == ulong.MaxValue || existing == address; }
        finally { Utf8.Free(text); }
    }

    private static string ReadComment(ulong address)
    {
        QString comment = default;
        try { IdaNative.get_cmt(&comment, address, 1); return comment.Read(); }
        finally { comment.Dispose(); }
    }

    internal static SdkFunctionOwnership? ReadOwnership(string comment)
    {
        string[] lines = comment.Split('\n').Where(x => x.StartsWith(Marker, StringComparison.Ordinal)).ToArray();
        if (lines.Length != 1) return null;
        try { return JsonSerializer.Deserialize<SdkFunctionOwnership>(lines[0][Marker.Length..]); }
        catch (JsonException) { return null; }
    }

    internal static string MergeOwnership(string comment, SdkFunctionOwnership ownership)
    {
        string preserved = string.Join("\n", comment.Split('\n')
            .Where(x => !x.StartsWith(Marker, StringComparison.Ordinal))).TrimEnd('\r', '\n');
        return (preserved.Length == 0 ? "" : preserved + "\n") + Marker + JsonSerializer.Serialize(ownership);
    }
}
