using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

internal sealed record EntityClassNamingSummary(
    int ClassesFound, int Named, int Typed, int InfosNamed,
    int SchemaBindingsNamed, int DataMapsNamed, int Skipped,
    int AbstractInfos, int RoundTripFailures, int DanglingBases, string Layout)
{
    internal int DataMapInitsNamed { get; init; }
    internal int DataMapHoldersNamed { get; init; }

    internal static EntityClassNamingSummary Empty(string layout) => new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, layout);
}

/// <summary>
/// Names and types every entity class's <c>CEntityClass</c> static, its <c>CEntityClassInfo</c>, the
/// accessor that builds them (<c>GetEntityClassInternal</c>), the cached pointer it returns, its guards and
/// the callbacks, schema binding and data map the pair points at. Both ABIs are read the same way: every
/// store of a constant address, by code or as initialized (relocated) data, goes into one map, and a class
/// is a pair whose <c>m_pClassInfo</c> and <c>m_pClass</c> point at each other. MSVC builds the info in the
/// accessor; Clang emits it as data and only patches <c>m_pClass</c> and the base at run time.
/// The member offsets come from hl2sdk's entityclass.h, checked against the build before anything is applied.
/// </summary>
internal static unsafe partial class EntityClassNaming
{
    private const string InfoType = "CEntityClassInfo";
    private const string ClassType = "CEntityClass";
    private const string FunctionSource = "entity class";

    private const int PtSilent = 0x0001, PtVariable = 0x0008, PtHigh = 0x0080;
    private const uint TinfoDefinite = 0x0001;
    private const uint UserTypeFlag = 0x02000000;
    private const int SnNoCheck = 0x01, SnNonAuto = 0x40;
    private const int MaxStringLength = 127;
    private const ulong GuardReach = 0x10;
    // How far past a static MSVC's guard may lie: the largest static the pass expects, with room to spare.
    private const ulong MaxStaticSize = 0x400;
    // The SDK's layout is taken when it pairs this share of what the known layout pairs.
    private const int AgreementPercent = 95;
    // Fewer classes than this means the scan no longer fits the build.
    private const int MinClasses = 50;

    // IDA x86 register numbers of the registers a call preserves.
    private static readonly ushort[] MsvcPreserved = [3, 5, 6, 7, 12, 13, 14, 15];
    private static readonly ushort[] SystemVPreserved = [3, 5, 12, 13, 14, 15];

    /// <summary>
    /// The member offsets the pass reads. Callbacks are CEntityClass's function pointer members; which of
    /// the members are is read from the build (the ones holding code), since the SDK types them loosely.
    /// </summary>
    internal sealed record Layout(
        ulong InfoSize, ulong Designer, ulong Cpp, ulong Class, ulong Base, ulong Binding, ulong DataMap,
        ulong ClassSize, ulong ClassInfo, IReadOnlyList<(ulong Offset, string Member)> Members);

    // hl2sdk-cs2 entityclass.h as of the 2026-09 builds; used when the SDK's own declaration is unavailable.
    internal static readonly Layout Default = new(0x38, 0x00, 0x08, 0x18, 0x20, 0x28, 0x30, 0x148, 0x58,
    [
        (0x30, "m_pfnRegisterPulseBindings"), (0x38, "m_NameToThinkFunc"),
        (0x40, "m_ThinkFuncToName"), (0x48, "m_pfnEnumerateComponents"),
    ]);

    internal sealed record EntityClass(string Cpp, string? Designer, ulong Info)
    {
        internal ulong? Object { get; init; }
        internal ulong? Accessor { get; init; }
        internal ulong? Cached { get; init; }
        internal ulong? ObjectGuard { get; init; }
        internal ulong? InfoGuard { get; init; }
        internal ulong? Base { get; set; }
        internal ulong? Binding { get; set; }
        internal ulong? DataMap { get; set; }
    }

    internal static EntityClassNamingSummary Apply(string? hl2SdkPath, SchemaTargetPlatform platform, string? graphPath,
        Action<string> diagnostic)
    {
        bool msvc = platform == SchemaTargetPlatform.WindowsMsvc;
        bool imported = hl2SdkPath != null && ImportTypes(hl2SdkPath, platform, diagnostic);
        Layout? sdk = imported ? ReadLayout() : null;
        var stores = StoreMap.Build(msvc ? MsvcPreserved : SystemVPreserved, msvc ? (ushort)1 : (ushort)7);

        // The SDK's layout is checked against the one the pass knows: a stale declaration puts m_pClassInfo
        // or m_pClass elsewhere, and the pairs stop pointing at each other. Its types are then not applied.
        var known = Discover(stores, Default);
        (Dictionary<ulong, EntityClass> Classes, int Failures)? declared = sdk == null ? null : Discover(stores, sdk);
        bool typesAgree = declared != null && declared.Value.Classes.Count * 100 >= known.Classes.Count * AgreementPercent;
        var (classes, failures) = typesAgree ? declared!.Value : known;
        Layout layout = typesAgree ? sdk! : Default;
        string layoutState = sdk == null ? "default" : $"sdk (info 0x{sdk.InfoSize:X}, class 0x{sdk.ClassSize:X})";
        if (declared != null && !typesAgree)
        {
            layoutState = $"drift: hl2sdk {InfoType}/{ClassType} pairs {declared.Value.Classes.Count} class(es), " +
                          $"the build's layout {known.Classes.Count}";
            diagnostic($"[entity-classes] {layoutState}; SDK types not applied.");
        }

        if (classes.Count < MinClasses)
        {
            diagnostic($"[entity-classes] only {classes.Count} class(es) found; the scan does not fit this build, " +
                       "nothing applied.");
            return EntityClassNamingSummary.Empty(layoutState) with { ClassesFound = classes.Count };
        }

        var infos = Bases(stores, layout, classes, out int dangling);
        foreach (EntityClass info in infos.Values.ToList())
        {
            info.Binding = SinglePointer(stores, info.Info + layout.Binding);
            info.DataMap = SinglePointer(stores, info.Info + layout.DataMap);
            if (info.Object != null)
            {
                infos[info.Info] = Locate(stores, layout, info, msvc);
            }
        }

        // A class whose info is built outside its accessor passes no guard for it; the nearest one then
        // belongs to another static. A guard is its static's only if no other static lies in between.
        var starts = infos.Values.SelectMany(x => new[] { x.Info, x.Object }).OfType<ulong>().Distinct().Order().ToArray();
        foreach (EntityClass entry in infos.Values.Where(x => x.Object != null).ToList())
        {
            infos[entry.Info] = entry with
            {
                ObjectGuard = Between(starts, entry.Object!.Value, entry.ObjectGuard) ? null : entry.ObjectGuard,
                InfoGuard = Between(starts, entry.Info, entry.InfoGuard) ? null : entry.InfoGuard,
            };
        }

        // The offsets the pass reads can agree while the rest does not: a stale CEntityClassInfo, with
        // m_pszDescription where the build keeps m_nFlags and m_pPredDescMap after the data map, still pairs
        // every class. Typed as the SDK has it, each info would cover the guard MSVC placed right after it,
        // and read its flags as a pointer. No info or object may cover another static, and what the SDK
        // declares a pointer must hold one.
        if (typesAgree && Overlaps(infos.Values, sdk!) is (int infoOverlaps, int objectOverlaps) &&
            NotPointers(stores, infos.Values, TypeLayout.PointerMembers(InfoType)) is var notPointers &&
            infoOverlaps + objectOverlaps + notPointers.Count > 0)
        {
            typesAgree = false;
            var reasons = new List<string>();
            if (infoOverlaps + objectOverlaps > 0)
                reasons.Add($"at 0x{sdk!.InfoSize:X} / 0x{sdk.ClassSize:X} bytes they cover the next static after " +
                            $"{infoOverlaps} info(s) and {objectOverlaps} object(s)");
            if (notPointers.Count > 0)
                reasons.Add($"{InfoType} declares a pointer at {string.Join(", ", notPointers.Select(x => $"+0x{x.Offset:X}"))} " +
                            $"where {notPointers.Sum(x => x.Infos)} info(s) hold none");
            layoutState = $"drift: hl2sdk's {InfoType}/{ClassType} do not fit the build: {string.Join("; ", reasons)}";
            diagnostic($"[entity-classes] {layoutState}; SDK types not applied.");
        }

        var summary = Name(stores, layout, infos, typesAgree && imported, msvc, diagnostic) with
        {
            RoundTripFailures = failures, DanglingBases = dangling, Layout = layoutState,
        };
        if (graphPath != null)
        {
            WriteGraph(graphPath, infos.Values, layoutState);
        }

        return summary;
    }

    // ---- types -------------------------------------------------------------------------------------------

    private static bool ImportTypes(string hl2SdkPath, SchemaTargetPlatform platform, Action<string> diagnostic)
    {
        string directory = Path.Combine(Path.GetTempPath(), $"s2atelier-entity-{Guid.NewGuid():N}");
        string header = Path.Combine(directory, "entityclass.hpp");
        bool keep = false;
        try
        {
            Directory.CreateDirectory(directory);
            File.WriteAllText(header, "#pragma once\n#include \"entity2/entityclass.h\"\n");
            SchemaImport.ConfigureClang(hl2SdkPath, platform, skipLayoutAssertions: true, directory);
            if (SchemaImport.ParseHeader(header, testOnly: true, printDiagnostics: true) is int preflight and not 0)
            {
                keep = true;
                diagnostic($"[entity-classes] IDAClang rejected entity2/entityclass.h with {preflight} error(s); " +
                           $"using the known layout without types. Header retained: {header}");
                return false;
            }

            SchemaImport.DeleteReplacedTypes([InfoType, ClassType]);
            int errors = SchemaImport.ParseHeader(header, testOnly: false, printDiagnostics: true);
            keep = errors != 0;
            return errors == 0;
        }
        finally
        {
            SchemaImport.ResetParser();
            if (!keep)
            {
                try { Directory.Delete(directory, recursive: true); }
                catch (IOException) { }
            }
        }
    }

    private static Layout? ReadLayout()
    {
        var info = TypeLayout.Members(InfoType, out ulong infoSize);
        var cls = TypeLayout.Members(ClassType, out ulong classSize);
        if (info == null || cls == null ||
            !info.TryGetValue("m_pszClassname", out ulong designer) || !info.TryGetValue("m_pszCPPClassname", out ulong cpp) ||
            !info.TryGetValue("m_pClass", out ulong owner) || !info.TryGetValue("m_pBaseClassInfo", out ulong baseInfo) ||
            !info.TryGetValue("m_pSchemaBinding", out ulong binding) || !info.TryGetValue("m_pDataDescMap", out ulong dataMap) ||
            !cls.TryGetValue("m_pClassInfo", out ulong classInfo))
        {
            return null;
        }

        return new Layout(infoSize, designer, cpp, owner, baseInfo, binding, dataMap, classSize, classInfo,
            [.. cls.Where(x => x.Value % 8 == 0).Select(x => (x.Value, x.Key)).OrderBy(x => x.Value)]);
    }

    // ---- discovery ---------------------------------------------------------------------------------------

    /// <summary>
    /// Classes under a layout: an info (named by a C++ class name string) that a CEntityClass's
    /// m_pClassInfo points at and whose m_pClass points back. Failures are cross links: an object whose
    /// m_pClassInfo names an info whose m_pClass is another object, or an info or object claimed twice.
    /// </summary>
    internal static (Dictionary<ulong, EntityClass> Classes, int Failures) Discover(StoreMap stores, Layout layout)
    {
        var byInfo = new Dictionary<ulong, EntityClass>();
        var claimed = new HashSet<ulong>();
        var unpaired = new List<(ulong Entity, List<ulong> Owners)>();
        int failures = 0;
        foreach ((ulong target, var values) in stores.Entries)
        {
            if (target < layout.ClassInfo)
            {
                continue;
            }

            ulong entity = target - layout.ClassInfo;
            foreach (ulong info in values.Select(x => x.Value).Distinct())
            {
                if (ClassName(stores, layout, info) is not string cpp)
                {
                    continue;
                }

                var owners = stores.Pointers(info + layout.Class).ToList();
                if (!owners.Contains(entity))
                {
                    unpaired.Add((entity, owners));
                    continue;
                }

                if (byInfo.ContainsKey(info) || !claimed.Add(entity))
                {
                    failures++;
                    continue;
                }

                byInfo[info] = new EntityClass(cpp, Designer(stores, layout, info), info)
                {
                    Object = entity,
                    Accessor = values.First(x => x.Value == info).Function,
                };
            }
        }

        // Objects and infos hold their base's info (m_pBaseClassInfo in both), which reads as a pairing seen
        // from inside them; only a pointer to an info from anywhere else is a cross link.
        var objects = claimed.Order().ToArray();
        var infos = byInfo.Keys.Order().ToArray();
        failures += unpaired.Count(x => x.Owners.Any(claimed.Contains) &&
                                        !Within(objects, x.Entity + layout.ClassInfo, layout.ClassSize) &&
                                        !Within(infos, x.Entity + layout.ClassInfo, layout.InfoSize));
        return (byInfo, failures);
    }

    private static bool Within(ulong[] starts, ulong address, ulong size)
    {
        int index = Array.BinarySearch(starts, address);
        index = index >= 0 ? index : ~index - 1;
        return index >= 0 && address < starts[index] + size;
    }

    // Adds the base infos the classes point at, abstract ones included; counts bases that are no info.
    private static Dictionary<ulong, EntityClass> Bases(StoreMap stores, Layout layout,
        Dictionary<ulong, EntityClass> classes, out int dangling)
    {
        var all = new Dictionary<ulong, EntityClass>(classes);
        var pending = new Queue<EntityClass>(classes.Values);
        var missing = new HashSet<ulong>();
        while (pending.Count > 0)
        {
            EntityClass entry = pending.Dequeue();
            if (SinglePointer(stores, entry.Info + layout.Base) is not ulong baseInfo)
            {
                continue;
            }

            entry.Base = baseInfo;
            if (all.ContainsKey(baseInfo))
            {
                continue;
            }

            if (ClassName(stores, layout, baseInfo) is not string cpp)
            {
                missing.Add(baseInfo);
                continue;
            }

            var added = new EntityClass(cpp, Designer(stores, layout, baseInfo), baseInfo);
            all[baseInfo] = added;
            pending.Enqueue(added);
        }

        dangling = missing.Count;
        return all;
    }

    private static string? ClassName(StoreMap stores, Layout layout, ulong info)
        => stores.Pointers(info + layout.Cpp).Select(ReadString).FirstOrDefault(x => x != null && Identifier().IsMatch(x));

    private static string? Designer(StoreMap stores, Layout layout, ulong info)
        => stores.Pointers(info + layout.Designer).Select(ReadString).FirstOrDefault(x => x != null);

    // The one address stored at a location, if it only ever holds one.
    private static ulong? SinglePointer(StoreMap stores, ulong location)
    {
        var values = stores.Pointers(location).Distinct().ToList();
        return values.Count == 1 ? values[0] : null;
    }

    private static string? ReadString(ulong address)
    {
        var bytes = new List<byte>();
        for (ulong i = 0; i <= MaxStringLength && IdaNative.is_mapped(address + i) != 0; i++)
        {
            byte value = IdaNative.get_byte(address + i);
            if (value == 0)
            {
                return bytes.Count >= 2 ? Encoding.ASCII.GetString([.. bytes]) : null;
            }

            if (value is < 0x20 or > 0x7E)
            {
                return null;
            }

            bytes.Add(value);
        }

        return null;
    }

    // ---- naming and types --------------------------------------------------------------------------------

    private static EntityClassNamingSummary Name(StoreMap stores, Layout layout, Dictionary<ulong, EntityClass> infos,
        bool applyTypes, bool msvc, Action<string> diagnostic)
    {
        // MSVC guards a magic static with an int; the Itanium ABI with a 64-bit guard object.
        string guardType = msvc ? "int" : "__int64";
        int named = 0, typed = 0, infosNamed = 0, bindings = 0, dataMaps = 0, skipped = 0;
        var objects = infos.Values.Where(x => x.Object != null).ToList();

        // Callback members: those holding code in most classes. A callback several classes share is a stub.
        var callbacks = layout.Members
            .Where(m => m.Offset != layout.ClassInfo && objects.Count(c => stores.Pointers(c.Object!.Value + m.Offset)
                .Any(IsFunction)) * 2 > objects.Count)
            .ToList();
        var users = new Dictionary<ulong, int>();
        foreach (EntityClass entry in objects)
            foreach (var member in callbacks)
                foreach (ulong target in stores.Pointers(entry.Object!.Value + member.Offset).Where(IsFunction).Distinct())
                    users[target] = users.GetValueOrDefault(target) + 1;

        // A class without a binding or data map of its own points at its base's, so bases come first and name
        // a shared one.
        int Depth(EntityClass entry)
        {
            int depth = 0;
            for (var seen = new HashSet<ulong>(); entry.Base is ulong b && seen.Add(b) && infos.TryGetValue(b, out var parent);
                 entry = parent)
            {
                depth++;
            }

            return depth;
        }

        var initializers = DataMapInits(stores, infos.Values.Select(x => x.DataMap).OfType<ulong>().ToHashSet());
        int inits = 0, holders = 0;
        var sharedData = new Dictionary<ulong, string>();
        foreach (EntityClass entry in infos.Values.OrderBy(Depth).ThenBy(x => x.Cpp, StringComparer.Ordinal))
        {
            string cpp = entry.Cpp;
            if (NameData(entry.Info, $"g_{cpp}ClassInfo", ref skipped)) infosNamed++;
            if (applyTypes && ApplyType(entry.Info, $"{InfoType} __s2_info;")) typed++;
            if (entry.Binding is ulong binding && sharedData.TryAdd(binding, cpp) &&
                NameData(binding, $"g_{cpp}SchemaBinding", ref skipped))
            {
                bindings++;
                if (applyTypes) ApplyType(binding, "CSchemaClassInfo __s2_binding;");
            }

            if (entry.DataMap is ulong dataMap && sharedData.TryAdd(dataMap, cpp))
            {
                if (NameData(dataMap, $"g_{cpp}DataDescMap", ref skipped))
                {
                    dataMaps++;
                    if (applyTypes) ApplyType(dataMap, "datamap_t __s2_datamap;");
                }

                if (initializers.TryGetValue(dataMap, out ulong init) && Identifier().IsMatch(cpp))
                {
                    bool typedInit = applyTypes && (ApplyType(init, $"datamap_t *__fastcall f({cpp} *);") ||
                                                    ApplyType(init, "datamap_t *__fastcall f(void *);"));
                    if (SdkFunctionBinding.TryNameFunction(init, DataMapInitName(cpp, msvc), typedInit, FunctionSource,
                            diagnostic))
                    {
                        inits++;
                    }

                    if (DataMapHolder(stores, dataMap, init, infos.Values, layout) is ulong holder &&
                        NameData(holder, $"{cpp}_DataDescInit::g_DataMapHolder", ref skipped))
                    {
                        holders++;
                        if (applyTypes) ApplyType(holder, "datamap_t *__s2_holder;");
                    }
                }
            }

            if (entry.Object is not ulong entity)
            {
                continue;
            }

            var located = entry;
            if (located.Accessor is ulong accessor &&
                SdkFunctionBinding.TryNameFunction(accessor, $"{cpp}::GetEntityClassInternal", false, FunctionSource, diagnostic))
            {
                named++;
            }

            if (NameData(entity, $"{cpp}::GetEntityClassInternal::g_Entity{cpp}Class", ref skipped)) named++;
            if (applyTypes && ApplyType(entity, $"{ClassType} __s2_class;")) typed++;
            if (located.Cached is ulong cached)
            {
                NameData(cached, $"g_pEntity{cpp}Class", ref skipped);
                if (applyTypes) ApplyType(cached, $"{ClassType} *__s2_cached;");
            }

            if (located.ObjectGuard is ulong objectGuard)
            {
                NameData(objectGuard, $"{cpp}_EntityClass_guard", ref skipped);
                ApplyType(objectGuard, $"{guardType} __s2_guard;");
            }

            if (located.InfoGuard is ulong infoGuard)
            {
                NameData(infoGuard, $"{cpp}_ClassInfo_guard", ref skipped);
                ApplyType(infoGuard, $"{guardType} __s2_guard;");
            }

            foreach (var member in callbacks)
            {
                foreach (ulong target in stores.Pointers(entity + member.Offset).Where(IsFunction).Distinct())
                {
                    if (users[target] == 1)
                    {
                        SdkFunctionBinding.TryNameFunction(target, $"{cpp}::{CallbackName(member.Member)}", false,
                            FunctionSource, diagnostic);
                    }
                }
            }
        }

        return new(objects.Count, named, typed, infosNamed, bindings, dataMaps, skipped, infos.Count - objects.Count, 0, 0, "")
        {
            DataMapInitsNamed = inits,
            DataMapHoldersNamed = holders,
        };
    }

    /// <summary>
    /// <c>g_DataMapHolder</c>: the one location that only ever receives the map, from an initializer that calls
    /// its <c>DataMapInit</c> or has it inlined. An info's <c>m_pDataDescMap</c> receives the map too.
    /// </summary>
    private static ulong? DataMapHolder(StoreMap stores, ulong map, ulong init, IEnumerable<EntityClass> infos,
        Layout layout)
    {
        var calls = new HashSet<ulong>();
        foreach (ulong site in Xrefs.CodeTo(init))
        {
            void* caller = IdaNative.get_func(site);
            if (caller != null) calls.Add(*(ulong*)caller);
        }

        var infoSlots = infos.Select(x => x.Info + layout.DataMap).ToHashSet();
        var found = stores.TargetsOf(map).Distinct()
            .Where(t => !infoSlots.Contains(t) && stores.Entries[t].All(x => x.Value == map) &&
                        stores.Entries[t].All(x => calls.Contains(x.Function) ||
                                                   stores.ReturnersOf(map).Contains(x.Function)))
            .ToList();
        return found.Count == 1 ? found[0] : null;
    }

    /// <summary>
    /// Each data map's <c>DataMapInit&lt;T&gt;</c>: it fills the map's fields, including the base's map, and
    /// returns the map, so it is a function that references the map and always returns it. The map's
    /// <c>GetDataDescMap</c> and a derived class's <c>GetBaseMap</c> return it too, in two instructions. The
    /// initializer of <c>g_DataMapHolder</c> returns it after storing it there, and may have the whole body
    /// inlined; the function itself never stores the map.
    /// </summary>
    private static Dictionary<ulong, ulong> DataMapInits(StoreMap stores, HashSet<ulong> maps)
    {
        var referenced = new Dictionary<ulong, HashSet<ulong>>();
        foreach (ulong map in maps)
        {
            foreach (ulong from in DataMapFields.SelectMany(offset => Xrefs.DataTo(map + offset)))
            {
                void* function = IdaNative.get_func(from);
                if (function != null)
                {
                    ulong start = *(ulong*)function;
                    if (!referenced.TryGetValue(start, out var set)) referenced[start] = set = [];
                    set.Add(map);
                }
            }
        }

        var result = new Dictionary<ulong, ulong>();
        foreach (ulong map in maps)
        {
            var found = stores.ReturnersOf(map)
                .Where(f => referenced.TryGetValue(f, out var set) && set.Contains(map) &&
                            Instructions(f, MinInitInstructions) >= MinInitInstructions &&
                            !stores.TargetsOf(map).Any(t => stores.Entries[t].Any(x => x.Function == f)))
                .ToList();
            if (found.Count == 1)
            {
                result[map] = found[0];
            }
        }

        return result;
    }

    // datamap_t's dataDesc, dataNumFields, dataClassName and baseMap.
    private static readonly ulong[] DataMapFields = [0x00, 0x08, 0x10, 0x18];
    private const int MinInitInstructions = 4;

    private static int Instructions(ulong function, int limit)
    {
        void* pfn = IdaNative.get_func(function);
        if (pfn == null) return 0;
        ulong end = *((ulong*)pfn + 1);
        int count = 0;
        for (ulong ea = function; ea < end && ea != ulong.MaxValue && count < limit; ea = IdaNative.next_head(ea, end))
        {
            count++;
        }

        return count;
    }

    /// <summary>
    /// The mangled name of <c>datamap_t *DataMapInit&lt;T&gt;(T *)</c>: IDA names cannot hold the angle brackets,
    /// but it shows a mangled name demangled.
    /// </summary>
    internal static string DataMapInitName(string cpp, bool msvc)
        => msvc
            ? $"??$DataMapInit@V{cpp}@@@@YAPEAUdatamap_t@@PEAV{cpp}@@@Z"
            : $"_Z11DataMapInitI{cpp.Length}{cpp}EP9datamap_tPT_";

    // The accessor stored m_pClassInfo; it returns a cached pointer it stores the object to, and passes the
    // guards of both statics, next to them, to the thread-safe-static helpers.
    private static EntityClass Locate(StoreMap stores, Layout layout, EntityClass entry, bool msvc)
    {
        ulong entity = entry.Object!.Value;
        ulong? accessor = entry.Accessor;
        ulong? cached = null;
        ulong classSize = Math.Max(layout.ClassSize, Default.ClassSize), infoSize = Math.Max(layout.InfoSize, Default.InfoSize);
        if (accessor is ulong function)
        {
            var candidates = stores.TargetsOf(entity).Distinct()
                .Where(t => !Inside(t, entity, classSize) && !Inside(t, entry.Info, infoSize) &&
                            stores.Entries[t].All(x => x.Value == entity) && stores.Entries[t].Any(x => x.Function == function))
                .ToList();
            cached = candidates.Count == 1 ? candidates[0] : null;
        }

        var arguments = accessor is ulong owner ? stores.CallArguments(owner) : [];
        return entry with
        {
            Cached = cached,
            ObjectGuard = Guard(arguments, entity, msvc),
            InfoGuard = Guard(arguments, entry.Info, msvc),
        };
    }

    // The guard the accessor passes to the thread-safe-static helpers for a static: MSVC places it right
    // after the static, Clang right before it. Found by position, not size, so the size can be checked.
    private static ulong? Guard(IReadOnlyCollection<ulong> arguments, ulong start, bool msvc)
    {
        var found = arguments.Where(x => msvc ? x > start && x - start <= MaxStaticSize : x < start && start - x <= GuardReach)
            .Distinct().ToList();
        return found.Count == 0 ? null : msvc ? found.Min() : found.Max();
    }

    /// <summary>
    /// Pointer members of the SDK's info that some info fills with something else: a value initialized data
    /// holds (Clang), neither null nor an address. MSVC's infos are uninitialized data its code fills in.
    /// </summary>
    internal static List<(ulong Offset, int Infos)> NotPointers(StoreMap stores, IEnumerable<EntityClass> entries,
        IReadOnlyList<ulong> pointerOffsets)
    {
        var list = entries.ToList();
        return [.. pointerOffsets
            .Select(offset => (Offset: offset, Infos: list.Count(x => IdaNative.is_loaded(x.Info + offset) != 0 &&
                                                                     IdaNative.get_qword(x.Info + offset) is ulong value &&
                                                                     value != 0 && IdaNative.is_mapped(value) == 0)))
            .Where(x => x.Infos > 0)];
    }

    // Whether another static starts between a static and its guard (after it under MSVC, before it under Clang).
    private static bool Between(ulong[] starts, ulong start, ulong? guard)
    {
        if (guard is not ulong address)
        {
            return false;
        }

        ulong low = Math.Min(start, address), high = Math.Max(start, address);
        return starts.Any(x => x != start && x > low && x <= high);
    }

    /// <summary>Infos and objects that, at the layout's sizes, cover another of the pass's statics.</summary>
    internal static (int Infos, int Objects) Overlaps(IEnumerable<EntityClass> entries, Layout layout)
    {
        var list = entries.ToList();
        var statics = list.SelectMany(x => new[] { x.Info, x.Object, x.Cached, x.ObjectGuard, x.InfoGuard, x.Binding, x.DataMap })
            .OfType<ulong>().Distinct().Order().ToArray();
        bool Covers(ulong start, ulong size)
        {
            int index = Array.BinarySearch(statics, start + 1);
            index = index >= 0 ? index : ~index;
            return index < statics.Length && statics[index] < start + size;
        }

        return (list.Count(x => Covers(x.Info, layout.InfoSize)),
                list.Count(x => x.Object is ulong entity && Covers(entity, layout.ClassSize)));
    }

    private static bool Inside(ulong address, ulong start, ulong size) => address >= start && address < start + size;

    internal static string CallbackName(string member)
        => member.StartsWith("m_pfn", StringComparison.Ordinal) ? member[5..]
            : member.StartsWith("m_", StringComparison.Ordinal) ? member[2..] : member;

    private static bool IsFunction(ulong address)
    {
        void* function = IdaNative.get_func(address);
        return function != null && *(ulong*)function == address;
    }

    // Replaces only automatic names; a name already taken elsewhere gets the address appended.
    internal static bool NameData(ulong address, string name, ref int skipped)
    {
        string current = SchemaVTableTypes.NameAt(address);
        if (current == name || current.StartsWith(name + "_", StringComparison.Ordinal))
        {
            return true;
        }

        if (!ValveInterfaceNaming.CanReplace(current))
        {
            skipped++;
            return false;
        }

        foreach (string candidate in (string[])[name, $"{name}_{address:X}"])
        {
            byte* native = Utf8.Allocate(candidate);
            try
            {
                ulong existing = IdaNative.get_name_ea(ulong.MaxValue, native);
                if ((existing == ulong.MaxValue || existing == address) &&
                    IdaNative.set_name(address, native, SnNoCheck | SnNonAuto) != 0)
                {
                    return true;
                }
            }
            finally { Utf8.Free(native); }
        }

        skipped++;
        return false;
    }

    // An explicit type, the user's or a previous run's, stays.
    internal static bool ApplyType(ulong address, string declaration)
    {
        if ((IdaNative.get_aflags(address) & UserTypeFlag) != 0)
        {
            return false;
        }

        TypeInfo type = default;
        var name = new QString();
        byte* text = Utf8.Allocate(declaration);
        try
        {
            if (IdaNative.parse_decl(&type, &name, IdaNative.get_idati(), text, PtSilent | PtVariable | PtHigh) == 0)
            {
                return false;
            }

            if (IdaNative.apply_tinfo(address, &type, TinfoDefinite) != 0)
            {
                return true;
            }

            // Auto-analysis left data items (xmmword_...) inside the object.
            ulong size = IdaNative.get_tinfo_size(null, type.Typid, 0);
            return size is > 0 and < 0x10000 && IdaNative.del_items(address, 0, size, null) != 0 &&
                   IdaNative.apply_tinfo(address, &type, TinfoDefinite) != 0;
        }
        finally
        {
            Utf8.Free(text);
            name.Dispose();
            type.Dispose();
        }
    }

    private static void WriteGraph(string path, IEnumerable<EntityClass> entries, string layout)
    {
        var list = entries.OrderBy(x => x.Cpp, StringComparer.Ordinal).ToList();
        var byInfo = list.ToDictionary(x => x.Info);
        static string? Hex(ulong? value) => value is ulong v ? $"0x{v:X}" : null;
        var graph = new
        {
            layout,
            classes = list.Select(x => new
            {
                cpp = x.Cpp,
                designer = x.Designer,
                @base = x.Base is ulong b && byInfo.TryGetValue(b, out var baseEntry) ? baseEntry.Cpp : null,
                abstractInfo = x.Object == null,
                classInfo = Hex(x.Info),
                entityClass = Hex(x.Object),
                accessor = Hex(x.Accessor),
                schemaBinding = Hex(x.Binding),
                dataMap = Hex(x.DataMap),
            }),
        };
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        File.WriteAllText(path, JsonSerializer.Serialize(graph, new JsonSerializerOptions { WriteIndented = true }));
    }

    internal static string GraphName(string module) => module + ".entities.json";

    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)]
    private static partial Regex Identifier();
}
