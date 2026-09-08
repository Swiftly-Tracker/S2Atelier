namespace S2Atelier.Ida;

public interface IValveInterfaceMemory
{
    ulong ReadPointer(ulong address);
    bool IsMapped(ulong address);
    bool IsWritable(ulong address);
    IEnumerable<ulong> DataReferencesTo(ulong address);
}

public sealed record ValveInterfaceTableRow(
    ulong Address,
    string Version,
    ulong GlobalAddress);

public sealed record ValveInterfaceTable(
    ulong Address,
    IReadOnlyList<ValveInterfaceTableRow> Rows);

public sealed record ValveInterfaceBinding(
    ValveInterfaceTableRow Row,
    ValveInterfaceDefinition? Definition);

public static class ValveInterfaceTableDetector
{
    private const ulong EntrySize = 16;
    private const int MaximumEntries = 4096;

    public static IReadOnlyList<ValveInterfaceTable> Detect(
        IReadOnlyDictionary<ulong, string> versionStrings,
        IValveInterfaceMemory memory,
        int minimumEntries = 4)
    {
        ArgumentNullException.ThrowIfNull(versionStrings);
        ArgumentNullException.ThrowIfNull(memory);
        if (minimumEntries < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumEntries));
        }

        var seeds = new HashSet<ulong>();
        foreach (ulong stringAddress in versionStrings.Keys)
        {
            foreach (ulong reference in memory.DataReferencesTo(stringAddress))
            {
                if (TryReadRow(reference, versionStrings, memory, out _))
                {
                    seeds.Add(reference);
                }
            }
        }

        var tables = new Dictionary<ulong, ValveInterfaceTable>();
        foreach (ulong seed in seeds.Order())
        {
            ulong start = seed;
            while (start >= EntrySize &&
                   TryReadRow(start - EntrySize, versionStrings, memory, out _))
            {
                start -= EntrySize;
            }
            if (tables.ContainsKey(start))
            {
                continue;
            }

            var rows = new List<ValveInterfaceTableRow>();
            ulong cursor = start;
            for (int i = 0; i < MaximumEntries; i++)
            {
                if (!TryReadRow(cursor, versionStrings, memory, out ValveInterfaceTableRow? row))
                {
                    break;
                }
                rows.Add(row);
                if (cursor > ulong.MaxValue - EntrySize)
                {
                    break;
                }
                cursor += EntrySize;
            }
            if (rows.Count >= minimumEntries)
            {
                tables.Add(start, new ValveInterfaceTable(start, rows));
            }
        }

        return tables.Values.OrderByDescending(x => x.Rows.Count).ThenBy(x => x.Address).ToArray();
    }

    private static bool TryReadRow(
        ulong address,
        IReadOnlyDictionary<ulong, string> versionStrings,
        IValveInterfaceMemory memory,
        out ValveInterfaceTableRow row)
    {
        row = null!;
        if (address > ulong.MaxValue - (EntrySize - 1) ||
            !memory.IsMapped(address) || !memory.IsMapped(address + EntrySize - 1))
        {
            return false;
        }
        ulong stringAddress = memory.ReadPointer(address);
        if (!versionStrings.TryGetValue(stringAddress, out string? version))
        {
            return false;
        }
        ulong globalAddress = memory.ReadPointer(address + 8);
        if (globalAddress == 0 || !memory.IsMapped(globalAddress) || !memory.IsWritable(globalAddress))
        {
            return false;
        }
        row = new ValveInterfaceTableRow(address, version, globalAddress);
        return true;
    }
}

public static class ValveInterfaceTableResolver
{
    public static IReadOnlyList<ValveInterfaceBinding> Resolve(
        ValveInterfaceTable table,
        ValveInterfaceCatalog catalog)
    {
        var slots = catalog.TableSlots
            .GroupBy(x => x.Version, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => new Queue<ValveInterfaceTableSlot>(x), StringComparer.Ordinal);
        var rowCounts = table.Rows.GroupBy(x => x.Version, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.Count(), StringComparer.Ordinal);
        var mismatchedVersions = slots.Where(x =>
                rowCounts.TryGetValue(x.Key, out int count) && count != x.Value.Count)
            .Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
        var unique = catalog.Entries.GroupBy(x => x.Version, StringComparer.Ordinal)
            .Where(x => x.Count() == 1)
            .ToDictionary(x => x.Key, x => x.Single(), StringComparer.Ordinal);

        var result = new List<ValveInterfaceBinding>(table.Rows.Count);
        foreach (ValveInterfaceTableRow row in table.Rows)
        {
            ValveInterfaceDefinition? definition = null;
            // Verified server binaries have a single Cvar slot even when the SDK
            // lists both cvar and g_pCVar. Bind it to the known canonical global.
            if (row.Version == ValveInterfaceCatalog.CvarVersion && rowCounts[row.Version] == 1)
            {
                result.Add(new ValveInterfaceBinding(row, unique[row.Version]));
                continue;
            }
            // Alias order is evidence only when the binary has the same number of slots.
            if (mismatchedVersions.Contains(row.Version))
            {
                result.Add(new ValveInterfaceBinding(row, null));
                continue;
            }
            if (slots.TryGetValue(row.Version, out Queue<ValveInterfaceTableSlot>? queue) && queue.Count > 0)
            {
                definition = queue.Dequeue().Definition;
            }
            else if (rowCounts[row.Version] == 1)
            {
                unique.TryGetValue(row.Version, out definition);
            }
            result.Add(new ValveInterfaceBinding(row, definition));
        }
        return result;
    }
}

public static class ValveInterfaceNaming
{
    private static readonly string[] AutoNamePrefixes =
    [
        "sub_", "nullsub_", "loc_", "off_", "unk_", "byte_", "word_", "dword_",
        "qword_", "asc_", "algn_", "stru_", "xmmword_", "ymmword_", "flt_", "dbl_",
    ];

    public static bool CanReplace(string current)
        => string.IsNullOrEmpty(current) || AutoNamePrefixes.Any(prefix =>
            current.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
}

internal static class ValveInterfaceComments
{
    internal static string Merge(string existing, IEnumerable<string> versions)
    {
        var lines = existing.Replace("\r\n", "\n", StringComparison.Ordinal).Split('\n')
            .Select(x => x.Trim()).ToHashSet(StringComparer.Ordinal);
        string result = existing;
        foreach (string version in versions.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal))
        {
            string comment = $"Valve interface \"{version}\"";
            if (!lines.Add(comment)) continue;
            if (result.Length != 0 && !result.EndsWith('\n')) result += "\n";
            result += comment;
        }
        return result;
    }
}
