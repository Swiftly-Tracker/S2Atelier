using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

internal sealed record VTableSlotNamingResult(int Tables, int Named, int Ambiguous);

/// <summary>
/// Names what the earlier passes leave behind: every slot of every vtable RTTI describes, whether or not
/// sdk.json or hl2sdk says anything about the class. A slot function keeps a name of its own
/// (CSource2Server::vfn_42), so its class and index read off the disassembly. A function several classes hold
/// is named after the least derived of them, the class whose table it first appears in; one held by unrelated
/// classes, which identical code folding does, is left alone.
/// </summary>
internal static unsafe class VTableSlotNaming
{
    private const string Source = "rtti vtable slot";

    internal static VTableSlotNamingResult Run(SchemaTargetPlatform platform, Action<string> diagnostic)
    {
        var abi = platform == SchemaTargetPlatform.WindowsMsvc ? VTableAbi.Msvc : VTableAbi.Itanium;
        var slots = new Dictionary<ulong, List<(string Class, ulong Offset, int Index)>>();
        var chains = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
        int tables = 0;
        foreach ((ulong addressPoint, IReadOnlyList<ulong> functions) in new SdkInterfaceBinding.Image().AllTables(platform))
        {
            if (RttiChain.Owner(addressPoint, abi) is not (string owner, ulong offset))
            {
                continue;
            }

            tables++;
            // The primary table carries the class's base chain, which decides who owns an inherited slot.
            if (offset == 0 && !chains.ContainsKey(owner) && RttiChain.Read(addressPoint, abi) is { Count: > 0 } chain)
            {
                chains[owner] = chain;
            }

            for (int index = 0; index < functions.Count; index++)
            {
                if (!slots.TryGetValue(functions[index], out var entries))
                {
                    slots.Add(functions[index], entries = []);
                }

                entries.Add((owner, offset, index));
            }
        }

        int named = 0, ambiguous = 0;
        foreach ((ulong address, var entries) in slots.OrderBy(x => x.Key))
        {
            // One function at two indices, or at two subobject offsets, has no one slot to be named after.
            if (entries.Select(x => x.Index).Distinct().Count() != 1 ||
                entries.Select(x => x.Offset).Distinct().Count() != 1 ||
                Owner(entries.Select(x => x.Class), chains) is not string owner)
            {
                ambiguous++;
                continue;
            }

            (_, ulong subobject, int slot) = entries[0];
            string name = subobject == 0
                ? $"{owner}::vfn_{slot}"
                // The class's tables for its bases repeat the indices, so the offset tells them apart.
                : $"{owner}::vfn_{subobject:X4}_{slot}";
            if (Replaceable(address) && SdkFunctionBinding.TryNameFunction(address, name, false, Source, diagnostic))
            {
                named++;
            }
        }

        return new(tables, named, ambiguous);
    }

    // An automatic name, or a slot name of ours a previous run applied; anything else is the user's or another
    // pass's, which knows the method's real name.
    private static bool Replaceable(ulong address)
    {
        string current = SchemaVTableTypes.NameAt(address);
        var ownership = SdkFunctionBinding.OwnershipAt(address);
        return ownership?.Name is string applied
            ? applied == current && ownership.Source == Source
            : ValveInterfaceNaming.CanReplace(current);
    }

    /// <summary>
    /// The least derived of the classes holding the function: the one every other one derives from. Classes
    /// without that relation share the function by folding, not by inheritance.
    /// </summary>
    internal static string? Owner(IEnumerable<string> classes, IReadOnlyDictionary<string, IReadOnlyList<string>> chains)
    {
        var owners = classes.Distinct(StringComparer.Ordinal).ToArray();
        if (owners.Length == 1)
        {
            return owners[0];
        }

        return owners.SingleOrDefault(candidate => owners.All(other => other == candidate ||
            chains.GetValueOrDefault(other)?.Contains(candidate, StringComparer.Ordinal) == true));
    }
}
