using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

internal sealed record SdkInterfaceSummary(int Tables, VTableBindingSummary Binding);

/// <summary>
/// Binds the vtables of classes sdk.json does not describe that implement a class hl2sdk declares. Two routes
/// find them: a class whose primary base chain reaches a declared class (CSource2Server implementing
/// ISource2Server), and every interface the module exports under an interfaces.h version string, whose
/// registered object may well be a secondary base (CCSGameConfiguration + 8 is its ISource2ServerConfig). The
/// declared class's methods are the table's first slots in declaration order; they are named after the
/// implementing class (CSource2Server::GameInit) and typed with the declared class as this.
/// </summary>
internal static unsafe partial class SdkInterfaceBinding
{
    private const int MaxSetupInsns = 8;
    private const ulong MaxCreateSize = 0x40;

    /// <param name="schemaClasses">Classes the schema pass binds itself, left alone here.</param>
    internal static SdkInterfaceSummary Run(string hl2SdkPath, SchemaTargetPlatform platform,
        IReadOnlySet<string> schemaClasses, VTableDrift? drift, Action<string> diagnostic)
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

        var image = new Image();
        var tables = new Dictionary<ulong, SchemaVTable>();
        foreach ((ulong addressPoint, IReadOnlyList<ulong> functions, VTableAbi abi) in image.PrimaryTables(platform))
        {
            IReadOnlyList<string> chain = RttiChain.Read(addressPoint, abi);
            if (chain.Count == 0 || schemaClasses.Contains(chain[0]))
            {
                continue;
            }

            // The most derived declared class: everything above it in the chain shares its first slots.
            if (chain.FirstOrDefault(Declared) is string sdkClass)
            {
                tables.TryAdd(addressPoint, new SchemaVTable(chain[0], addressPoint, 0, sdkClass, [], functions));
            }
        }

        int registered = 0;
        var abiOfModule = platform == SchemaTargetPlatform.WindowsMsvc ? VTableAbi.Msvc : VTableAbi.Itanium;
        foreach ((string sdkClass, ulong addressPoint) in image.ExportedInterfaces(ValveInterfaceCatalog.Load(hl2SdkPath), Declared))
        {
            if (tables.ContainsKey(addressPoint) || RttiChain.Owner(addressPoint, abiOfModule) is not (string owner, ulong offset) ||
                schemaClasses.Contains(owner))
            {
                continue;
            }

            var functions = image.Slots(addressPoint);
            if (functions.Count > 0)
            {
                tables[addressPoint] = new SchemaVTable(owner, addressPoint, offset, sdkClass, [], functions);
                registered++;
            }
        }

        Stage($"discovery ({registered} exported interface table(s) beyond the base chains)");
        if (tables.Count == 0)
        {
            return new(0, new(0, 0, 0));
        }

        var list = tables.Values.ToList();
        using var sdk = Hl2SdkVTables.LoadClasses(hl2SdkPath, platform, list.Select(x => x.ThisType!), diagnostic, headers);
        Stage("SDK header load");
        var resolved = sdk.ResolveDeclared(list);
        var slots = drift == null ? resolved : drift.Filter(list, resolved);
        Stage("SDK prototype resolution");
        // A pure-call slot points at a function that never returns (it aborts); it is not the method.
        var pureCalls = list.SelectMany(x => x.Functions)
            .Where(x => IdaNative.get_func(x) != null && IdaNative.func_does_return(x) == 0).ToHashSet();
        var bound = SdkFunctionBinding.Bind(list, slots, pureCalls, new HashSet<ulong>(), diagnostic, nameByClass: true);
        Stage("binding");
        return new(list.Count, bound);
    }

    [GeneratedRegex(@"[A-Za-z_][A-Za-z0-9_]*", RegexOptions.CultureInvariant)]
    private static partial Regex Identifier();

    internal sealed class Image
    {
        private readonly List<(ulong Address, string Name)> _names = [];
        private readonly SortedSet<ulong> _boundaries;
        private readonly Memory _memory = new();

        internal Image()
        {
            for (nuint i = 0, count = IdaNative.get_nlist_size(); i < count; i++)
            {
                byte* raw = IdaNative.get_nlist_name(i);
                if (raw != null)
                {
                    _names.Add((IdaNative.get_nlist_ea(i), Marshal.PtrToStringUTF8((nint)raw) ?? string.Empty));
                }
            }

            _boundaries = new SortedSet<ulong>(_names.Select(x => x.Address));
        }

        private ulong? End(ulong address)
        {
            ulong next = _boundaries.GetViewBetween(address + 1, ulong.MaxValue).FirstOrDefault();
            return next == 0 ? null : next;
        }

        // The function pointers from an address point up to the next named address or non-function.
        internal IReadOnlyList<ulong> Slots(ulong addressPoint)
            => VTableEntryScanner.ScanMsvc(_memory, addressPoint, endExclusive: End(addressPoint));

        /// <summary>
        /// Every vtable RTTI describes, primary and secondary, with its address point. The caller reads the
        /// class and the subobject offset back from the RTTI at the address point.
        /// </summary>
        internal List<(ulong AddressPoint, IReadOnlyList<ulong> Functions)> AllTables(SchemaTargetPlatform platform)
        {
            var tables = new List<(ulong, IReadOnlyList<ulong>)>();
            if (platform == SchemaTargetPlatform.WindowsMsvc)
            {
                foreach ((ulong locator, string name) in _names)
                {
                    if (!name.StartsWith("??_R4", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    foreach (ulong slot in Xrefs.DataTo(locator))
                    {
                        if (IdaNative.get_qword(slot) == locator && Slots(slot + 8) is { Count: > 0 } functions)
                        {
                            tables.Add((slot + 8, functions));
                        }
                    }
                }

                return tables;
            }

            foreach ((ulong address, string name) in _names)
            {
                if (!name.StartsWith("_ZTV", StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (VTableSlice slice in VTableEntryScanner.ScanItaniumTables(_memory, address, endExclusive: End(address)))
                {
                    if (slice.Functions.Count > 0)
                    {
                        tables.Add((slice.AddressPoint, slice.Functions));
                    }
                }
            }

            return tables;
        }

        /// <summary>
        /// Every class's table at offset 0. MSVC tables are found through their RTTI locators, since IDA does
        /// not name every vftable; Itanium ones through their symbols, the first group being the primary table.
        /// </summary>
        internal List<(ulong AddressPoint, IReadOnlyList<ulong> Functions, VTableAbi Abi)> PrimaryTables(
            SchemaTargetPlatform platform)
        {
            var tables = new List<(ulong, IReadOnlyList<ulong>, VTableAbi)>();
            if (platform == SchemaTargetPlatform.WindowsMsvc)
            {
                foreach ((ulong locator, string name) in _names)
                {
                    // RTTICompleteObjectLocator; its offset field is 0 for a class's primary table.
                    if (!name.StartsWith("??_R4", StringComparison.Ordinal) || IdaNative.get_dword(locator + 4) != 0)
                    {
                        continue;
                    }

                    foreach (ulong slot in Xrefs.DataTo(locator))
                    {
                        if (IdaNative.get_qword(slot) == locator && Slots(slot + 8) is { Count: > 0 } functions)
                        {
                            tables.Add((slot + 8, functions, VTableAbi.Msvc));
                        }
                    }
                }

                return tables;
            }

            foreach ((ulong address, string name) in _names)
            {
                if (name.StartsWith("_ZTV", StringComparison.Ordinal) &&
                    VTableEntryScanner.ScanItaniumTables(_memory, address, endExclusive: End(address))
                        .FirstOrDefault(x => x.OffsetToTop == 0 && x.Functions.Count > 0) is VTableSlice primary)
                {
                    tables.Add((primary.AddressPoint, primary.Functions, VTableAbi.Itanium));
                }
            }

            return tables;
        }

        /// <summary>
        /// The interfaces the module exports: an EXPOSE_INTERFACE registration passes the version string and a
        /// create function, which returns the exported (sub)object; a static initializer stores that object's
        /// vtable. Yields the declared interface class with the table's address point.
        /// </summary>
        internal List<(string SdkClass, ulong AddressPoint)> ExportedInterfaces(ValveInterfaceCatalog catalog,
            Func<string, bool> declared)
        {
            var found = new List<(string, ulong)>();
            var versions = catalog.Entries.Where(x => declared(x.ClassName))
                .GroupBy(x => x.Version, StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.First().ClassName, StringComparer.Ordinal);
            int[] regs = ConVarNaming.ArgumentRegisters();
            byte* buf = stackalloc byte[Insn.BufferSize];
            foreach ((ulong text, string version) in Strings(versions.Keys))
            {
                foreach (ulong use in Xrefs.DataTo(text))
                {
                    ulong function = FunctionStart(use);
                    if (function == ulong.MaxValue || NextTransfer(use, buf) is not ulong call ||
                        !ConVarTypeRecovery.ArgSetup(call, function, regs[1], out ulong create, out bool isAddress) ||
                        !isAddress || ReturnedAddress(create, buf) is not ulong exported ||
                        StoredTable(exported, buf) is not ulong table)
                    {
                        continue;
                    }

                    found.Add((versions[version], table));
                }
            }

            return found;
        }

        // The call or tail jump the registration arguments are set up for.
        private static ulong? NextTransfer(ulong ea, byte* buf)
        {
            ulong end = FunctionEnd(ea);
            for (int i = 0; i < MaxSetupInsns; i++)
            {
                ea = IdaNative.next_head(ea, end);
                if (ea == ulong.MaxValue || !Insn.TryDecode(ea, buf))
                {
                    return null;
                }

                if (Insn.IsCall(buf) || Mnemonic(ea) == "jmp")
                {
                    return ea;
                }
            }

            return null;
        }

        // A create function of EXPOSE_SINGLE_INTERFACE: return &g_Object (or the interface's subobject of it).
        private static ulong? ReturnedAddress(ulong function, byte* buf)
        {
            void* pfn = IdaNative.get_func(function);
            if (pfn == null || *(ulong*)pfn != function || *((ulong*)pfn + 1) - function > MaxCreateSize)
            {
                return null;
            }

            ulong end = *((ulong*)pfn + 1);
            ulong? returned = null;
            for (ulong ea = function; ea < end && ea != ulong.MaxValue; ea = IdaNative.next_head(ea, end))
            {
                if (Insn.TryDecode(ea, buf) && Mnemonic(ea) == "lea" && Insn.OpType(buf, 0) == Insn.OpReg &&
                    Insn.OpRegister(buf, 0) == 0 && Insn.OpType(buf, 1) == Insn.OpMem)
                {
                    returned = Insn.OpAddr(buf, 1);
                }
            }

            return returned;
        }

        // The vtable a static initializer stores at the object: mov [object], reg, the register loaded with it.
        private static ulong? StoredTable(ulong exported, byte* buf)
        {
            foreach (ulong use in Xrefs.DataTo(exported))
            {
                ulong function = FunctionStart(use);
                if (function == ulong.MaxValue || !Insn.TryDecode(use, buf) || Mnemonic(use) != "mov" ||
                    Insn.OpType(buf, 0) != Insn.OpMem || Insn.OpAddr(buf, 0) != exported || Insn.OpType(buf, 1) != Insn.OpReg)
                {
                    continue;
                }

                if (ConVarTypeRecovery.ArgSetup(use, function, Insn.OpRegister(buf, 1), out ulong table, out bool isAddress) &&
                    isAddress)
                {
                    return table;
                }
            }

            return null;
        }

        // String literals among the wanted texts, with their addresses.
        private static List<(ulong Address, string Text)> Strings(IEnumerable<string> wanted)
        {
            var set = wanted.ToHashSet(StringComparer.Ordinal);
            var found = new List<(ulong, string)>();
            byte* item = stackalloc byte[32];
            for (nuint i = 0, count = IdaNative.get_strlist_qty(); i < count; i++)
            {
                if (IdaNative.get_strlist_item(item, i) == 0)
                {
                    continue;
                }

                ulong ea = *(ulong*)item;
                var text = new QString();
                try
                {
                    if (IdaNative.get_strlit_contents(&text, ea, (nuint)(*(int*)(item + 8)), *(int*)(item + 12), null, 0) > 0 &&
                        text.Read() is string value && set.Contains(value))
                    {
                        found.Add((ea, value));
                    }
                }
                finally { text.Dispose(); }
            }

            return found;
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
            return function == null ? ulong.MaxValue : *(ulong*)function;
        }

        private static ulong FunctionEnd(ulong ea)
        {
            void* function = IdaNative.get_func(ea);
            return function == null ? ulong.MaxValue : *((ulong*)function + 1);
        }
    }

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
