using System.Text.RegularExpressions;

namespace S2Atelier.Ida.Schema;

public enum VTableAbi
{
    Msvc,
    Itanium,
}

public sealed record VTableDescriptor(
    ulong Address,
    string SymbolName,
    string ClassName,
    string? SecondaryBaseName,
    VTableAbi Abi)
{
    public string EffectiveThisType => SecondaryBaseName ?? ClassName;
}

public static partial class VTableAnalysis
{
    public static bool TryDescribe(ulong address, string rawName, string? demangledName, out VTableDescriptor descriptor)
    {
        descriptor = null!;
        VTableAbi? abi = rawName.StartsWith("??_7", StringComparison.Ordinal) ? VTableAbi.Msvc
            : rawName.StartsWith("_ZTV", StringComparison.Ordinal) ? VTableAbi.Itanium
            : null;
        if (abi == null)
        {
            return false;
        }

        string? className = null;
        string? secondary = null;
        string demangled = demangledName ?? string.Empty;
        if (abi == VTableAbi.Itanium)
        {
            Match match = ItaniumDemangledRegex().Match(demangled);
            if (match.Success)
            {
                className = match.Groups[1].Value.Trim();
            }
            className ??= DecodeItanium(rawName);
        }
        else
        {
            Match secondaryMatch = MsvcSecondaryRegex().Match(demangled);
            if (secondaryMatch.Success)
            {
                className = secondaryMatch.Groups[1].Value.Trim();
                secondary = secondaryMatch.Groups[2].Value.Trim();
            }
            else
            {
                Match primaryMatch = MsvcDemangledRegex().Match(demangled);
                if (primaryMatch.Success)
                {
                    className = primaryMatch.Groups[1].Value.Trim();
                }
            }

            className ??= DecodeMsvc(rawName, out secondary);
        }

        className = NormalizeClassName(className);
        secondary = NormalizeClassName(secondary);
        if (string.IsNullOrEmpty(className))
        {
            return false;
        }

        descriptor = new VTableDescriptor(address, rawName, className, secondary, abi.Value);
        return true;
    }

    public static string? ResolveSharedFunctionOwner(
        IReadOnlyCollection<string> owners,
        IReadOnlyDictionary<string, SchemaClass> classes)
    {
        string[] candidates = owners.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
        if (candidates.Length == 0)
        {
            return null;
        }
        if (candidates.Length == 1)
        {
            return candidates[0];
        }

        var common = AncestorsAndSelf(candidates[0], classes);
        foreach (string candidate in candidates.Skip(1))
        {
            common.IntersectWith(AncestorsAndSelf(candidate, classes));
        }
        return common.OrderByDescending(x => InheritanceDepth(x, classes)).ThenBy(x => x, StringComparer.Ordinal)
            .FirstOrDefault();
    }

    public static bool IsAncestorOrSame(
        string possibleAncestor,
        string descendant,
        IReadOnlyDictionary<string, SchemaClass> classes)
    {
        if (possibleAncestor == descendant)
        {
            return true;
        }

        var visited = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>();
        queue.Enqueue(descendant);
        while (queue.TryDequeue(out string? current) && visited.Add(current))
        {
            if (!classes.TryGetValue(current, out SchemaClass? type))
            {
                continue;
            }
            foreach (string baseName in type.BaseClasses)
            {
                if (baseName == possibleAncestor)
                {
                    return true;
                }
                queue.Enqueue(baseName);
            }
        }
        return false;
    }

    private static HashSet<string> AncestorsAndSelf(
        string name, IReadOnlyDictionary<string, SchemaClass> classes)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        var queue = new Queue<string>();
        queue.Enqueue(name);
        while (queue.TryDequeue(out string? current) && result.Add(current))
        {
            if (classes.TryGetValue(current, out SchemaClass? type))
            {
                foreach (string baseName in type.BaseClasses)
                {
                    queue.Enqueue(baseName);
                }
            }
        }
        return result;
    }

    private static int InheritanceDepth(string name, IReadOnlyDictionary<string, SchemaClass> classes)
    {
        if (!classes.TryGetValue(name, out SchemaClass? type) || type.BaseClasses.Count == 0)
        {
            return 0;
        }
        return 1 + type.BaseClasses.Max(x => InheritanceDepth(x, classes));
    }

    public static string RewriteFirstParameter(string declaration, string marker, string thisType, int argumentCount)
    {
        int markerOffset = declaration.IndexOf(marker, StringComparison.Ordinal);
        if (markerOffset < 0)
        {
            throw new SchemaFormatException("IDA did not print the requested function marker.");
        }
        int open = declaration.IndexOf('(', markerOffset + marker.Length);
        if (open < 0)
        {
            throw new SchemaFormatException("IDA function declaration has no parameter list.");
        }

        int depth = 1;
        int firstComma = -1;
        int close = -1;
        for (int i = open + 1; i < declaration.Length; i++)
        {
            switch (declaration[i])
            {
                case '(':
                    depth++;
                    break;
                case ')':
                    depth--;
                    if (depth == 0)
                    {
                        close = i;
                    }
                    break;
                case ',' when depth == 1 && firstComma < 0:
                    firstComma = i;
                    break;
            }
            if (close >= 0)
            {
                break;
            }
        }
        if (close < 0)
        {
            throw new SchemaFormatException("IDA function declaration has an unbalanced parameter list.");
        }

        string replacement = $"{thisType} *__s2_this";
        if (argumentCount == 0)
        {
            string inside = declaration[(open + 1)..close].Trim();
            if (inside == "...")
            {
                replacement += ", ...";
            }
            return declaration[..(open + 1)] + replacement + declaration[close..];
        }

        int end = firstComma >= 0 ? firstComma : close;
        return declaration[..(open + 1)] + replacement + declaration[end..];
    }

    private static string? DecodeMsvc(string raw, out string? secondary)
    {
        secondary = null;
        int start = "??_7".Length;
        int end = raw.IndexOf("@@", start, StringComparison.Ordinal);
        if (end <= start)
        {
            return null;
        }
        string[] components = raw[start..end].Split('@', StringSplitOptions.RemoveEmptyEntries);
        Array.Reverse(components);
        string primary = string.Join("::", components);

        int secondaryStart = raw.IndexOf("6B", end + 2, StringComparison.Ordinal);
        if (secondaryStart >= 0)
        {
            secondaryStart += 2;
            int secondaryEnd = raw.IndexOf("@@", secondaryStart, StringComparison.Ordinal);
            if (secondaryEnd > secondaryStart)
            {
                string[] secondaryParts = raw[secondaryStart..secondaryEnd]
                    .Split('@', StringSplitOptions.RemoveEmptyEntries);
                Array.Reverse(secondaryParts);
                secondary = string.Join("::", secondaryParts);
            }
        }
        return primary;
    }

    private static string? DecodeItanium(string raw)
    {
        string encoded = raw["_ZTV".Length..];
        bool nested = encoded.StartsWith('N');
        if (nested)
        {
            encoded = encoded[1..];
        }

        var components = new List<string>();
        int offset = 0;
        while (offset < encoded.Length && encoded[offset] != 'E')
        {
            int digitsStart = offset;
            while (offset < encoded.Length && char.IsDigit(encoded[offset]))
            {
                offset++;
            }
            if (digitsStart == offset || !int.TryParse(encoded[digitsStart..offset], out int length) ||
                length <= 0 || offset + length > encoded.Length)
            {
                return null;
            }
            components.Add(encoded.Substring(offset, length));
            offset += length;
            if (!nested)
            {
                break;
            }
        }
        return components.Count == 0 ? null : string.Join("::", components);
    }

    private static string? NormalizeClassName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }
        string value = PrefixRegex().Replace(name.Trim(), string.Empty);
        return value.Trim(' ', '`', '\'', '"');
    }

    [GeneratedRegex(@"(?:vtable for\s+)(.+)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex ItaniumDemangledRegex();

    [GeneratedRegex(@"^(?:const\s+)?(.+?)::[`']vftable['`](?:\s|$)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex MsvcDemangledRegex();

    [GeneratedRegex(@"^(?:const\s+)?(.+?)::[`']vftable['`]\{for [`'](.+?)['`]\}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex MsvcSecondaryRegex();

    [GeneratedRegex(@"^(?:class|struct|const)\s+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex PrefixRegex();
}

public interface IVTableMemory
{
    ulong ReadPointer(ulong address);
    bool IsMapped(ulong address);
    bool IsFunctionStart(ulong address);
}

public sealed record VTableSlice(long OffsetToTop, IReadOnlyList<ulong> Functions);

public interface IVirtualFunctionTypeEditor
{
    bool TryBindThis(ulong address, string owner);
}

public sealed record VTableBindingSummary(int Bound, int Skipped, int Conflicts);

public static class VTableFunctionBinder
{
    public static VTableBindingSummary Bind<TCollection>(
        IReadOnlyDictionary<ulong, TCollection> functionOwners,
        IReadOnlySet<ulong> pureCalls,
        IReadOnlySet<ulong> unresolvedThis,
        IReadOnlyDictionary<string, SchemaClass> classes,
        IVirtualFunctionTypeEditor editor,
        Action<ulong, IReadOnlyCollection<string>>? onConflict = null)
        where TCollection : IReadOnlyCollection<string>
    {
        int bound = 0;
        int skipped = unresolvedThis.Count(x => !functionOwners.ContainsKey(x));
        int conflicts = 0;
        foreach ((ulong address, TCollection owners) in functionOwners.OrderBy(x => x.Key))
        {
            if (unresolvedThis.Contains(address) || pureCalls.Contains(address))
            {
                skipped++;
                continue;
            }
            string? owner = VTableAnalysis.ResolveSharedFunctionOwner(owners, classes);
            if (owner == null)
            {
                conflicts++;
                onConflict?.Invoke(address, owners);
                continue;
            }
            if (editor.TryBindThis(address, owner))
            {
                bound++;
            }
            else
            {
                skipped++;
            }
        }
        return new VTableBindingSummary(bound, skipped, conflicts);
    }
}

public static class VTableEntryScanner
{
    public static IReadOnlyList<ulong> ScanMsvc(IVTableMemory memory, ulong start, int maximumEntries = 1024)
    {
        var result = new List<ulong>();
        for (int i = 0; i < maximumEntries; i++)
        {
            ulong target = memory.ReadPointer(start + (ulong)(i * 8));
            if (!memory.IsFunctionStart(target))
            {
                break;
            }
            result.Add(target);
        }
        return result;
    }

    public static IReadOnlyList<ulong> ScanItanium(IVTableMemory memory, ulong symbol, int maximumEntries = 1024)
        => ScanItaniumTables(memory, symbol, maximumEntries).SelectMany(x => x.Functions).ToArray();

    public static IReadOnlyList<VTableSlice> ScanItaniumTables(
        IVTableMemory memory, ulong symbol, int maximumEntries = 1024)
    {
        var result = new List<VTableSlice>();
        bool pointsAtFunction = memory.IsFunctionStart(memory.ReadPointer(symbol));
        long offsetToTop = pointsAtFunction ? 0 : unchecked((long)memory.ReadPointer(symbol));
        ulong cursor = pointsAtFunction ? symbol : symbol + 16;
        var current = new List<ulong>();
        for (int i = 0; i < maximumEntries; i++)
        {
            ulong value = memory.ReadPointer(cursor);
            if (memory.IsFunctionStart(value))
            {
                current.Add(value);
                cursor += 8;
                continue;
            }

            long nextOffsetToTop = unchecked((long)value);
            bool plausibleOffset = nextOffsetToTop <= 0 && nextOffsetToTop >= -0x100000;
            ulong possibleFunction = memory.ReadPointer(cursor + 16);
            if (plausibleOffset && memory.IsMapped(memory.ReadPointer(cursor + 8)) &&
                memory.IsFunctionStart(possibleFunction))
            {
                if (current.Count > 0)
                {
                    result.Add(new VTableSlice(offsetToTop, current.ToArray()));
                }
                current.Clear();
                offsetToTop = nextOffsetToTop;
                cursor += 16;
                continue;
            }
            break;
        }
        if (current.Count > 0)
        {
            result.Add(new VTableSlice(offsetToTop, current.ToArray()));
        }
        return result;
    }
}
