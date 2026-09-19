using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

internal sealed record SdkInterfaceSummary(int Tables, VTableBindingSummary Binding);

/// <summary>
/// Binds the vtables of classes sdk.json does not describe but whose primary base chain reaches a class hl2sdk
/// declares, such as CSource2Server implementing ISource2Server. The declared class's methods are the primary
/// table's first slots in declaration order; they are named after the implementing class
/// (CSource2Server::GameInit) and typed with the declared class as this.
/// </summary>
internal static unsafe partial class SdkInterfaceBinding
{
    internal static SdkInterfaceSummary Run(string hl2SdkPath, SchemaTargetPlatform platform, SchemaSelection selection,
        IReadOnlySet<ulong> pureCalls, VTableDrift drift, Action<string> diagnostic)
    {
        var clock = System.Diagnostics.Stopwatch.StartNew();
        void Stage(string name)
        {
            Console.Error.WriteLine($"[schema-timing] interface {name}: {clock.Elapsed.TotalSeconds:F3}s");
            clock.Restart();
        }

        var headers = ValveInterfaceCatalog.ReadHeaders(hl2SdkPath);
        // Most class names never occur in the SDK; only those that do are looked up as declarations.
        var identifiers = headers.Values.SelectMany(x => Identifier().Matches(x)).Select(x => x.Value)
            .ToHashSet(StringComparer.Ordinal);
        var declared = new Dictionary<string, bool>(StringComparer.Ordinal);
        bool Declared(string name)
        {
            int separator = name.LastIndexOf("::", StringComparison.Ordinal);
            string leaf = separator < 0 ? name : name[(separator + 2)..];
            if (name.Contains('<') || !identifiers.Contains(leaf))
            {
                return false;
            }

            return declared.TryGetValue(name, out bool known)
                ? known
                : declared[name] = ValveInterfaceCatalog.FindDefinitionHeader(name, headers) != null;
        }

        var tables = new List<SchemaVTable>();
        var seen = new HashSet<ulong>();
        foreach ((ulong addressPoint, IReadOnlyList<ulong> functions, VTableAbi abi) in PrimaryTables(platform))
        {
            IReadOnlyList<string> chain = RttiChain.Read(addressPoint, abi);
            if (chain.Count == 0 || selection.Classes.ContainsKey(chain[0]) || !seen.Add(addressPoint))
            {
                continue;
            }

            // The most derived declared class: everything above it in the chain shares its first slots.
            if (chain.FirstOrDefault(Declared) is string sdkClass)
            {
                tables.Add(new SchemaVTable(chain[0], addressPoint, 0, sdkClass, [], functions));
            }
        }

        Stage("discovery");
        if (tables.Count == 0)
        {
            return new(0, new(0, 0, 0));
        }

        using var sdk = Hl2SdkVTables.LoadClasses(hl2SdkPath, platform, tables.Select(x => x.ThisType!), diagnostic, headers);
        Stage("SDK header load");
        var slots = drift.Filter(tables, sdk.ResolveDeclared(tables));
        Stage("SDK prototype resolution");
        var bound = SdkFunctionBinding.Bind(tables, slots, pureCalls, new HashSet<ulong>(), diagnostic, nameByClass: true);
        Stage("binding");
        return new(tables.Count, bound);
    }

    /// <summary>
    /// Every class's table at offset 0. MSVC tables are found through their RTTI locators, since IDA does not
    /// name every vftable; Itanium ones through their symbols, the first group being the primary table.
    /// </summary>
    private static List<(ulong AddressPoint, IReadOnlyList<ulong> Functions, VTableAbi Abi)> PrimaryTables(
        SchemaTargetPlatform platform)
    {
        var tables = new List<(ulong, IReadOnlyList<ulong>, VTableAbi)>();
        var names = new List<(ulong Address, string Name)>();
        for (nuint i = 0, count = IdaNative.get_nlist_size(); i < count; i++)
        {
            byte* raw = IdaNative.get_nlist_name(i);
            if (raw != null)
            {
                names.Add((IdaNative.get_nlist_ea(i), Marshal.PtrToStringUTF8((nint)raw) ?? string.Empty));
            }
        }

        var boundaries = new SortedSet<ulong>(names.Select(x => x.Address));
        var memory = new Memory();
        ulong? End(ulong address)
        {
            ulong next = boundaries.GetViewBetween(address + 1, ulong.MaxValue).FirstOrDefault();
            return next == 0 ? null : next;
        }

        if (platform == SchemaTargetPlatform.WindowsMsvc)
        {
            foreach ((ulong locator, string name) in names)
            {
                // RTTICompleteObjectLocator; its offset field is 0 for a class's primary table.
                if (!name.StartsWith("??_R4", StringComparison.Ordinal) || IdaNative.get_dword(locator + 4) != 0)
                {
                    continue;
                }

                foreach (ulong slot in Xrefs.DataTo(locator))
                {
                    if (IdaNative.get_qword(slot) != locator)
                    {
                        continue;
                    }

                    var functions = VTableEntryScanner.ScanMsvc(memory, slot + 8, endExclusive: End(slot + 8));
                    if (functions.Count > 0)
                    {
                        tables.Add((slot + 8, functions, VTableAbi.Msvc));
                    }
                }
            }

            return tables;
        }

        foreach ((ulong address, string name) in names)
        {
            if (name.StartsWith("_ZTV", StringComparison.Ordinal) &&
                VTableEntryScanner.ScanItaniumTables(memory, address, endExclusive: End(address))
                    .FirstOrDefault(x => x.OffsetToTop == 0 && x.Functions.Count > 0) is VTableSlice primary)
            {
                tables.Add((primary.AddressPoint, primary.Functions, VTableAbi.Itanium));
            }
        }

        return tables;
    }

    [GeneratedRegex(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.CultureInvariant)]
    private static partial Regex Identifier();

    private sealed class Memory : IVTableMemory
    {
        public ulong ReadPointer(ulong address) => IdaNative.get_qword(address);
        public bool IsMapped(ulong address) => IdaNative.is_mapped(address) != 0;

        public bool IsFunctionStart(ulong address)
        {
            if (address == 0 || IdaNative.is_mapped(address) == 0)
            {
                return false;
            }

            void* function = IdaNative.get_func(address);
            return function != null && *(ulong*)function == address;
        }
    }
}
