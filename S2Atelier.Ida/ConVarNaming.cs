using System.Text.Json;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

public sealed class ConVarInfo
{
    public string Name = "";
    public string Description = "";
    public ulong Object = ulong.MaxValue;
    public ulong RegisteredAt = ulong.MaxValue;
    public ulong Ctor = ulong.MaxValue;
    public ulong Callback = ulong.MaxValue;
    public ulong Flags;
    public bool HasFlags;
    // EConVarType (tier1/convar.h), or -1 when the registration does not reveal it.
    public int ValueType = -1;
    public bool IsCommand;
    public readonly List<ulong> ReadSites = [];
    public readonly List<ulong> Accessors = [];
}

public sealed record ConVarNamingResult(
    bool Applicable, int Found, int RenamedObjects, int RenamedHandlers, int Variables, int Commands,
    int TypedObjects = 0);

internal enum ArgRole { Unknown, Object, Name, Flags, Description, Callback }

internal sealed class CtorProfile
{
    public ulong Addr;
    public int Sites;
    public readonly ArgRole[] Roles = new ArgRole[6];
}

internal sealed class ConVarStats
{
    public int StringsScanned;
    public int CtorsFound;
    public int Extracted;
}

public static unsafe class ConVarNaming
{
    private const ulong BadAddr = ulong.MaxValue;

    private const int MinCtorSites = 8;
    private const int MaxForwardInsns = 14;
    private const int MaxCtors = 128;
    private const int MaxRounds = 4;
    private const int CtorSample = 200;
    private const int AccessorMinUses = 20;
    private const int MinNameLen = 3;
    private const int MaxNameLen = 64;
    private const ulong StackArg5 = 0x20;
    private const int MaxBackInsns = 14;
    private const int ArgSlots = 6;
    private const int ArgRoles = 6;
    private const int MaxCallbackInsns = 16;

    private const int PtSilent = 0x0001;
    private const int PtVariable = 0x0008;
    private const int PtHigh = 0x0080;
    private const uint TinfoDefinite = 0x0001;
    private const uint UserTypeFlag = 0x02000000;

    private const int SnNoCheck = 0x01;
    private const int SnForce = 0x800;
    private const int SegPermWrite = 2;

    private static readonly string[] EngineMarkers =
        ["FCVAR_", "VEngineCvar", "SchemaSystem_", "libtier0", "tier0.dll"];

    private static readonly string[] SeedPrefixes =
    [
        "sv_", "mp_", "bot_", "host_", "tv_", "ds_", "mm_", "net_", "nav_",
        "cash_", "ammo_", "game_", "round_",
        "cl_", "r_", "mat_", "hud_", "view_", "viewmodel_", "voice_", "snd_",
        "fog_", "joy_", "in_", "demo_", "vgui_", "ui_", "con_", "engine_",
        "spec_", "crosshair_", "safezone_", "lobby_", "fps_", "gl_", "key_",
    ];

    private static readonly string[] AutoNamePrefixes =
    [
        "sub_", "nullsub_", "loc_", "off_", "unk_", "byte_", "word_", "dword_",
        "qword_", "asc_", "algn_", "stru_", "xmmword_", "ymmword_", "flt_", "dbl_",
    ];

    public static ConVarNamingResult Run(IReadOnlyDictionary<string, int>? dumpedTypes = null)
    {
        if (!LooksLikeSourceEngine())
        {
            return new ConVarNamingResult(false, 0, 0, 0, 0, 0);
        }

        var stats = new ConVarStats();
        var all = FindAll(stats);

        var ctors = new HashSet<ulong>();
        foreach (var cv in all)
        {
            ctors.Add(cv.Ctor);
        }

        Collect(all, ctors);
        Classify(all, ctors);

        ConVarTypeRecovery.FromStack(all, ConVarValueTypeNames.Length, spilledValueInfo: ArgRegs().Length == 4);
        if (dumpedTypes != null)
        {
            ApplyDumpedTypes(all, dumpedTypes);
        }

        var owners = HandlerOwners(all);

        int renamedObjects = 0;
        int renamedHandlers = 0;
        int typedObjects = 0;
        int variables = 0;
        int commands = 0;

        foreach (var cv in all)
        {
            if (cv.IsCommand)
            {
                commands++;
            }
            else
            {
                variables++;
            }

            if (Apply(cv))
            {
                renamedObjects++;
            }

            if (ApplyType(cv))
            {
                typedObjects++;
            }

            if (NameHandler(cv, owners))
            {
                renamedHandlers++;
            }
        }

        return new ConVarNamingResult(true, all.Count, renamedObjects, renamedHandlers, variables, commands,
            typedObjects);
    }

    /// <summary>
    /// The EConVarType of each convar from a runtime dump (CS2-Dumps' convars.json), keyed by name. Names are
    /// unique across modules, and a convar shared by several binaries is listed once under one of them, so
    /// the module is not part of the key.
    /// </summary>
    public static IReadOnlyDictionary<string, int> LoadDumpedTypes(string path)
    {
        using FileStream stream = File.OpenRead(path);
        using JsonDocument document = JsonDocument.Parse(stream);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            throw new JsonException("expected an array of convars");
        }

        var types = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (JsonElement convar in document.RootElement.EnumerateArray())
        {
            if (convar.TryGetProperty("name", out JsonElement name) && name.ValueKind == JsonValueKind.String &&
                convar.TryGetProperty("datatype_raw", out JsonElement type) && type.TryGetInt32(out int value))
            {
                types[name.GetString()!] = value;
            }
        }

        if (types.Count == 0)
        {
            throw new JsonException("no convar carries both 'name' and 'datatype_raw'");
        }

        return types;
    }

    // A runtime dump reports the type the engine registered, so it wins; the binary still covers convars
    // the dump does not list. Disagreements point at a dump from another build.
    private static void ApplyDumpedTypes(IReadOnlyList<ConVarInfo> all, IReadOnlyDictionary<string, int> dumpedTypes)
    {
        int applied = 0;
        var disagreements = new List<string>();
        foreach (ConVarInfo cv in all)
        {
            if (cv.IsCommand || !dumpedTypes.TryGetValue(cv.Name, out int type) ||
                type < 0 || type >= ConVarValueTypeNames.Length)
            {
                continue;
            }

            if (cv.ValueType >= 0 && cv.ValueType != type)
            {
                disagreements.Add($"{cv.Name} ({ConVarValueTypeNames[cv.ValueType]} in the binary, " +
                                  $"{ConVarValueTypeNames[type]} in the dump)");
            }

            cv.ValueType = type;
            applied++;
        }

        Console.Error.WriteLine($"[convars] {applied} types from the dump, {disagreements.Count} disagreeing " +
                                "with the binary.");
        foreach (string disagreement in disagreements.Take(10))
        {
            Console.Error.WriteLine($"[convars] type mismatch: {disagreement}");
        }
    }

    private static bool LooksLikeSourceEngine()
    {
        nuint total = IdaNative.get_strlist_qty();
        byte* si = stackalloc byte[32];

        for (nuint i = 0; i < total; i++)
        {
            if (IdaNative.get_strlist_item(si, i) == 0)
            {
                continue;
            }

            ulong ea = *(ulong*)si;
            int length = *(int*)(si + 8);
            int type = *(int*)(si + 12);

            string text = ReadStrlitContents(ea, length, type);
            foreach (string marker in EngineMarkers)
            {
                if (text.Contains(marker, StringComparison.Ordinal))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static readonly HashSet<string> LivePrefixes = new(SeedPrefixes, StringComparer.Ordinal);

    private static bool HasConVarPrefix(string s)
    {
        int sep = s.IndexOf('_');
        if (sep <= 0)
        {
            return false;
        }

        return LivePrefixes.Contains(s[..(sep + 1)]);
    }

    private static int LearnPrefixes(List<ConVarInfo> found)
    {
        int added = 0;
        foreach (var cv in found)
        {
            int sep = cv.Name.IndexOf('_');
            if (sep <= 0 || sep > 12)
            {
                continue;
            }

            if (LivePrefixes.Add(cv.Name[..(sep + 1)]))
            {
                added++;
            }
        }

        return added;
    }

    private static bool LooksLikeConVarName(string s)
    {
        if (s.Length < MinNameLen || s.Length > MaxNameLen)
        {
            return false;
        }

        if (!char.IsAsciiLetter(s[0]) && s[0] != '_')
        {
            return false;
        }

        bool hasLower = false;
        foreach (char c in s)
        {
            if (c is >= 'a' and <= 'z')
            {
                hasLower = true;
                continue;
            }

            if (char.IsAsciiLetterOrDigit(c) || c == '_')
            {
                continue;
            }

            return false;
        }

        return hasLower;
    }

    private static string ReadStringAt(ulong ea)
    {
        if (ea == BadAddr || IdaNative.is_mapped(ea) == 0)
        {
            return string.Empty;
        }

        return ReadStrlitContents(ea, -1, 0);
    }

    private static string ReadStrlitContents(ulong ea, int length, int type)
    {
        var qs = new QString();
        nint written = IdaNative.get_strlit_contents(
            &qs, ea, length < 0 ? nuint.MaxValue : (nuint)length, type, null, 0);

        if (written <= 0)
        {
            qs.Dispose();
            return string.Empty;
        }

        string text = qs.Read();
        qs.Dispose();
        return text;
    }

    private static bool TrackedValue(ulong ea, int reg, out ulong value)
    {
        ulong v = 0;
        bool ok = IdaNative.find_reg_value((ulong*)&v, ea, reg) == 1;
        value = v;
        return ok;
    }

    private static int[]? _argRegs;

    internal static int[] ArgumentRegisters() => ArgRegs();

    private static int[] ArgRegs()
    {
        if (_argRegs != null)
        {
            return _argRegs;
        }

        byte* buf = stackalloc byte[64];
        nuint len = IdaNative.get_file_type_name(buf, 64);
        string typeName = len == 0 ? "" : System.Text.Encoding.UTF8.GetString(buf, (int)len);
        // IDA spells this "Portable executable for AMD64 (PE)".
        bool ms = typeName.Contains("portable executable", StringComparison.OrdinalIgnoreCase);

        string[] names = ms
            ? ["rcx", "rdx", "r8", "r9"]
            : ["rdi", "rsi", "rdx", "rcx", "r8", "r9"];

        var regs = new int[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            byte* namePtr = Utf8.Allocate(names[i]);
            regs[i] = IdaNative.str2reg(namePtr);
            Utf8.Free(namePtr);
        }

        return _argRegs = regs;
    }

    private static ulong CallTarget(ulong callEa)
    {
        ulong direct = IdaNative.get_first_fcref_from(callEa);
        if (direct != BadAddr)
        {
            return direct;
        }

        return IdaNative.get_first_dref_from(callEa);
    }

    private static ulong CallAfter(ulong useEa, ulong pfnEnd)
    {
        ulong ea = useEa;
        byte* buf = stackalloc byte[Insn.BufferSize];

        for (int i = 0; i < MaxForwardInsns; i++)
        {
            ea = IdaNative.next_head(ea, pfnEnd);
            if (ea == BadAddr)
            {
                return BadAddr;
            }

            if (!Insn.TryDecode(ea, buf))
            {
                return BadAddr;
            }

            if (!Insn.IsCall(buf))
            {
                continue;
            }

            return CallTarget(ea) != BadAddr ? ea : BadAddr;
        }

        return BadAddr;
    }

    private static bool SpilledValue(ulong callEa, ulong pfnStart, ulong offset, out ulong value)
    {
        value = 0;
        int rsp = ArgRegRsp();

        ulong ea = callEa;
        byte* buf = stackalloc byte[Insn.BufferSize];

        for (int i = 0; i < MaxBackInsns; i++)
        {
            ea = IdaNative.prev_head(ea, pfnStart);
            if (ea == BadAddr)
            {
                return false;
            }

            if (!Insn.TryDecode(ea, buf))
            {
                return false;
            }

            if (Insn.OpType(buf, 0) != Insn.OpDispl || Insn.OpRegister(buf, 0) != rsp || Insn.OpAddr(buf, 0) != offset)
            {
                continue;
            }

            var mnem = new QString();
            bool ok = IdaNative.print_insn_mnem(&mnem, ea) > 0;
            string text = ok ? mnem.Read() : string.Empty;
            mnem.Dispose();
            if (text != "mov")
            {
                continue;
            }

            byte srcType = Insn.OpType(buf, 1);
            if (srcType == Insn.OpImm)
            {
                value = Insn.OpValue(buf, 1);
                return true;
            }

            if (srcType == Insn.OpReg)
            {
                return TrackedValue(ea, Insn.OpRegister(buf, 1), out value);
            }

            return false;
        }

        return false;
    }

    private static int? _rsp;

    private static int ArgRegRsp()
    {
        if (_rsp is { } cached)
        {
            return cached;
        }

        byte* namePtr = Utf8.Allocate("rsp");
        int reg = IdaNative.str2reg(namePtr);
        Utf8.Free(namePtr);
        return (_rsp = reg).Value;
    }

    private static bool ArgValue(ulong callEa, ulong pfnStart, int slot, out ulong value)
    {
        int[] regs = ArgRegs();

        if (slot < regs.Length)
        {
            return TrackedValue(callEa, regs[slot], out value);
        }

        if (pfnStart == BadAddr)
        {
            value = 0;
            return false;
        }

        return SpilledValue(callEa, pfnStart, StackArg5 + 8ul * (ulong)(slot - regs.Length), out value);
    }

    private static ulong CallbackNear(ulong callEa, ulong pfnStart)
    {
        ulong found = BadAddr;
        ulong ea = callEa;
        byte* buf = stackalloc byte[Insn.BufferSize];

        for (int i = 0; i < MaxCallbackInsns; i++)
        {
            ea = IdaNative.prev_head(ea, pfnStart);
            if (ea == BadAddr)
            {
                break;
            }

            if (!Insn.TryDecode(ea, buf))
            {
                break;
            }

            if (Insn.OpType(buf, 1) != Insn.OpMem)
            {
                continue;
            }

            ulong addr = Insn.OpAddr(buf, 1);
            void* target = IdaNative.get_func(addr);
            if (target == null || *(ulong*)target != addr)
            {
                continue;
            }

            if (found != BadAddr && found != addr)
            {
                return BadAddr;
            }

            found = addr;
        }

        return found;
    }

    private static bool InWritableData(ulong ea)
    {
        void* seg = IdaNative.getseg(ea);
        if (seg == null)
        {
            return false;
        }

        byte perm = ((byte*)seg)[42];
        return (perm & SegPermWrite) != 0;
    }

    private static bool IsFunctionStart(ulong ea)
    {
        void* pfn = IdaNative.get_func(ea);
        return pfn != null && *(ulong*)pfn == ea;
    }

    // A function defined in this module: not an import slot, and not a PLT or import thunk.
    private static bool IsLocalFunction(ulong ea)
    {
        void* pfn = IdaNative.get_func(ea);
        return pfn != null && *(ulong*)pfn == ea && (*((ulong*)pfn + 2) & FuncThunk) == 0;
    }

    private static ulong FuncStart(ulong ea)
    {
        void* pfn = IdaNative.get_func(ea);
        return pfn == null ? BadAddr : *(ulong*)pfn;
    }

    private static ulong FuncEnd(ulong ea)
    {
        void* pfn = IdaNative.get_func(ea);
        return pfn == null ? BadAddr : *((ulong*)pfn + 1);
    }

    private static List<ulong> FindCtors(ConVarStats st)
    {
        var tally = new Dictionary<ulong, int>();

        nuint total = IdaNative.get_strlist_qty();
        byte* si = stackalloc byte[32];

        for (nuint i = 0; i < total; i++)
        {
            if (IdaNative.get_strlist_item(si, i) == 0)
            {
                continue;
            }

            ulong ea = *(ulong*)si;
            int length = *(int*)(si + 8);
            int type = *(int*)(si + 12);

            string text = ReadStrlitContents(ea, length, type);
            if (!LooksLikeConVarName(text) || !HasConVarPrefix(text))
            {
                continue;
            }

            st.StringsScanned++;

            foreach (ulong from in Xrefs.DataTo(ea))
            {
                ulong pfnStart = FuncStart(from);
                if (pfnStart == BadAddr)
                {
                    continue;
                }

                ulong callEa = CallAfter(from, FuncEnd(from));
                if (callEa == BadAddr)
                {
                    continue;
                }

                // Registration is tier1 code linked into every module. An import that takes a global
                // and a name, such as FindOrCreateQuantizedFloatEncoder for networked fields, is not.
                ulong target = CallTarget(callEa);
                if (!IsLocalFunction(target))
                {
                    continue;
                }

                tally[target] = tally.GetValueOrDefault(target) + 1;
            }
        }

        var ranked = new List<(int Count, ulong Ea)>();
        foreach (var (ea, count) in tally)
        {
            if (count >= MinCtorSites)
            {
                ranked.Add((count, ea));
            }
        }

        ranked.Sort((a, b) => b.Count.CompareTo(a.Count));
        if (ranked.Count > MaxCtors)
        {
            ranked.RemoveRange(MaxCtors, ranked.Count - MaxCtors);
        }

        st.CtorsFound = ranked.Count;
        return ranked.ConvertAll(r => r.Ea);
    }

    private static CtorProfile ProfileCtor(ulong ctor)
    {
        var prof = new CtorProfile { Addr = ctor };
        var votes = new int[ArgSlots, ArgRoles];

        foreach (ulong from in Xrefs.AllTo(ctor))
        {
            ulong pfnStart = FuncStart(from);
            if (pfnStart == BadAddr)
            {
                continue;
            }

            prof.Sites++;

            for (int i = 0; i < ArgSlots; i++)
            {
                if (!ArgValue(from, pfnStart, i, out ulong v) || v == 0)
                {
                    continue;
                }

                string s = ReadStringAt(v);
                bool pointsAtFuncStart = IsFunctionStart(v);

                if (s.Length > 0 && s.Contains(' '))
                {
                    votes[i, (int)ArgRole.Description]++;
                }
                else if (LooksLikeConVarName(s))
                {
                    votes[i, (int)ArgRole.Name]++;
                }
                else if (pointsAtFuncStart)
                {
                    votes[i, (int)ArgRole.Callback]++;
                }
                else if (InWritableData(v))
                {
                    votes[i, (int)ArgRole.Object]++;
                }
                else if (v < 0x100000000ul)
                {
                    votes[i, (int)ArgRole.Flags]++;
                }
            }
        }

        var taken = new bool[ArgSlots];
        for (int r = 1; r < ArgRoles; r++)
        {
            int best = -1, top = 0;
            for (int i = 0; i < ArgSlots; i++)
            {
                if (taken[i] || votes[i, r] <= top)
                {
                    continue;
                }

                top = votes[i, r];
                best = i;
            }

            if (best < 0 || top * 3 < prof.Sites)
            {
                continue;
            }

            prof.Roles[best] = (ArgRole)r;
            taken[best] = true;
        }

        return prof;
    }

    private static bool IsRegistration(CtorProfile prof, int objSlot, int nameSlot)
    {
        if (objSlot < 0 || nameSlot < 0)
        {
            return false;
        }

        var owner = new Dictionary<ulong, string>();
        int checkedCount = 0, writable = 0, named = 0, conflicts = 0;

        foreach (ulong from in Xrefs.AllTo(prof.Addr))
        {
            ulong pfnStart = FuncStart(from);
            if (pfnStart == BadAddr)
            {
                continue;
            }

            if (++checkedCount > CtorSample)
            {
                break;
            }

            bool hasObj = ArgValue(from, pfnStart, objSlot, out ulong obj) && InWritableData(obj);
            if (hasObj)
            {
                writable++;
            }

            if (!ArgValue(from, pfnStart, nameSlot, out ulong namePtr))
            {
                continue;
            }

            string s = ReadStringAt(namePtr);
            if (s.Length == 0 || !LooksLikeConVarName(s))
            {
                continue;
            }

            named++;

            if (!hasObj)
            {
                continue;
            }

            if (owner.TryGetValue(obj, out string? existing))
            {
                if (existing != s)
                {
                    conflicts++;
                }
            }
            else
            {
                owner[obj] = s;
            }
        }

        if (checkedCount == 0)
        {
            return false;
        }

        bool mostlyWritable = writable * 10 >= checkedCount * 6;
        bool oneNameEach = conflicts * 20 <= writable;
        bool mostlyNamed = named * 10 >= checkedCount * 6;

        return mostlyWritable && oneNameEach && mostlyNamed;
    }

    private static List<ConVarInfo> Extract(CtorProfile prof, ConVarStats st)
    {
        var found = new List<ConVarInfo>();

        int nameSlot = -1, objSlot = -1, flagSlot = -1, descSlot = -1, cbSlot = -1;
        for (int i = 0; i < ArgSlots; i++)
        {
            switch (prof.Roles[i])
            {
                case ArgRole.Name: nameSlot = i; break;
                case ArgRole.Object: objSlot = i; break;
                case ArgRole.Flags: flagSlot = i; break;
                case ArgRole.Description: descSlot = i; break;
                case ArgRole.Callback: cbSlot = i; break;
            }
        }

        if (descSlot < 0)
        {
            flagSlot = -1;
        }

        if (!IsRegistration(prof, objSlot, nameSlot))
        {
            return found;
        }

        foreach (ulong from in Xrefs.AllTo(prof.Addr))
        {
            ulong pfnStart = FuncStart(from);
            if (pfnStart == BadAddr)
            {
                continue;
            }

            if (!ArgValue(from, pfnStart, nameSlot, out ulong namePtr))
            {
                continue;
            }

            if (!ArgValue(from, pfnStart, objSlot, out ulong obj) || IdaNative.is_mapped(obj) == 0)
            {
                continue;
            }

            string name = ReadStringAt(namePtr);
            if (name.Length == 0 || !LooksLikeConVarName(name))
            {
                continue;
            }

            var cv = new ConVarInfo
            {
                Name = name,
                Object = obj,
                RegisteredAt = from,
                Ctor = prof.Addr,
            };

            if (flagSlot >= 0 && ArgValue(from, pfnStart, flagSlot, out ulong rawFlags) && rawFlags < 0x100000000ul)
            {
                cv.Flags = rawFlags;
                cv.HasFlags = true;
            }

            if (descSlot >= 0 && ArgValue(from, pfnStart, descSlot, out ulong rawDesc))
            {
                cv.Description = ReadStringAt(rawDesc);
            }

            cv.Callback = cbSlot >= 0 && ArgValue(from, pfnStart, cbSlot, out ulong cb)
                ? cb
                : CallbackNear(from, pfnStart);
            cv.ValueType = ConVarTypeRecovery.FromCall(from, pfnStart, obj, flagSlot, ArgRegs(),
                ConVarValueTypeNames.Length);

            st.Extracted++;
            found.Add(cv);
        }

        return found;
    }

    private static void CollectAccessors(List<ConVarInfo> sure, HashSet<ulong> accessors)
    {
        var tally = new Dictionary<ulong, int>();

        foreach (var cv in sure)
        {
            foreach (ulong from in Xrefs.DataTo(cv.Object))
            {
                ulong pfnStart = FuncStart(from);
                if (pfnStart == BadAddr || from == cv.RegisteredAt)
                {
                    continue;
                }

                ulong target = CallAfter(from, FuncEnd(from));
                if (target != BadAddr)
                {
                    ulong resolved = CallTarget(target);
                    if (resolved != BadAddr)
                    {
                        tally[resolved] = tally.GetValueOrDefault(resolved) + 1;
                    }
                }
            }
        }

        foreach (var (ea, count) in tally)
        {
            if (count >= AccessorMinUses)
            {
                accessors.Add(ea);
            }
        }
    }

    private static bool UsesKnownAccessor(List<ConVarInfo> batch, HashSet<ulong> accessors)
    {
        int checkedCount = 0, matched = 0;

        foreach (var cv in batch)
        {
            if (++checkedCount > CtorSample)
            {
                break;
            }

            foreach (ulong from in Xrefs.DataTo(cv.Object))
            {
                ulong pfnStart = FuncStart(from);
                if (pfnStart == BadAddr || from == cv.RegisteredAt)
                {
                    continue;
                }

                ulong callEa = CallAfter(from, FuncEnd(from));
                if (callEa != BadAddr && accessors.Contains(CallTarget(callEa)))
                {
                    matched++;
                }

                break;
            }
        }

        return checkedCount > 0 && matched * 2 >= checkedCount;
    }

    private static bool CarriesMetadata(CtorProfile prof)
    {
        foreach (var role in prof.Roles)
        {
            if (role is ArgRole.Flags or ArgRole.Description)
            {
                return true;
            }
        }

        return false;
    }

    private static List<ConVarInfo> FindAll(ConVarStats st)
    {
        var byObject = new Dictionary<ulong, ConVarInfo>();
        var done = new HashSet<ulong>();
        var accessors = new HashSet<ulong>();
        var deferred = new List<CtorProfile>();

        for (int round = 0; round < MaxRounds; round++)
        {
            var fresh = new List<ConVarInfo>();

            foreach (ulong ctor in FindCtors(st))
            {
                if (!done.Add(ctor))
                {
                    continue;
                }

                var prof = ProfileCtor(ctor);
                var batch = Extract(prof, st);
                if (batch.Count == 0)
                {
                    continue;
                }

                if (!CarriesMetadata(prof))
                {
                    if (accessors.Count == 0)
                    {
                        deferred.Add(prof);
                        continue;
                    }

                    if (!UsesKnownAccessor(batch, accessors))
                    {
                        continue;
                    }
                }
                else
                {
                    CollectAccessors(batch, accessors);
                }

                foreach (var cv in batch)
                {
                    if (!byObject.TryGetValue(cv.Object, out var existing)
                        || cv.Description.Length > existing.Description.Length)
                    {
                        byObject[cv.Object] = cv;
                    }

                    fresh.Add(cv);
                }
            }

            if (fresh.Count == 0)
            {
                break;
            }

            if (LearnPrefixes(fresh) == 0)
            {
                break;
            }
        }

        foreach (var prof in deferred)
        {
            var batch = Extract(prof, st);
            if (batch.Count == 0 || !UsesKnownAccessor(batch, accessors))
            {
                continue;
            }

            foreach (var cv in batch)
            {
                byObject.TryAdd(cv.Object, cv);
            }
        }

        var all = new List<ConVarInfo>(byObject.Values);
        all.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));
        return all;
    }

    private const int MaxUseForwardInsns = 10;

    private static void Classify(List<ConVarInfo> all, HashSet<ulong> ctors)
    {
        var perCtor = new Dictionary<ulong, (int Used, int Total)>();

        foreach (var cv in all)
        {
            var score = perCtor.GetValueOrDefault(cv.Ctor);
            score.Total++;

            foreach (ulong acc in cv.Accessors)
            {
                if (acc != BadAddr && !ctors.Contains(acc))
                {
                    score.Used++;
                    break;
                }
            }

            perCtor[cv.Ctor] = score;
        }

        foreach (var cv in all)
        {
            var score = perCtor[cv.Ctor];
            cv.IsCommand = score.Used * 4 < score.Total;
        }
    }

    private static void Collect(List<ConVarInfo> all, HashSet<ulong> ctors)
    {
        byte* buf = stackalloc byte[Insn.BufferSize];

        foreach (var cv in all)
        {
            foreach (ulong from in Xrefs.DataTo(cv.Object))
            {
                ulong pfnStart = FuncStart(from);
                if (pfnStart == BadAddr)
                {
                    continue;
                }

                if (pfnStart == FuncStart(cv.RegisteredAt))
                {
                    continue;
                }

                ulong ea = from;
                ulong consumer = BadAddr;

                for (int i = 0; i < MaxUseForwardInsns; i++)
                {
                    ea = IdaNative.next_head(ea, FuncEnd(from));
                    if (ea == BadAddr)
                    {
                        break;
                    }

                    if (!Insn.TryDecode(ea, buf))
                    {
                        break;
                    }

                    if (!Insn.IsCall(buf))
                    {
                        continue;
                    }

                    consumer = CallTarget(ea);
                    break;
                }

                if (consumer != BadAddr && ctors.Contains(consumer))
                {
                    continue;
                }

                cv.ReadSites.Add(from);
                cv.Accessors.Add(consumer);
            }
        }
    }

    private const string VarPrefix = "cvar_";
    private const string CmdPrefix = "cmd_";
    private const string HandlerSuffix = "_callback";

    private static string WantedName(ConVarInfo cv) => (cv.IsCommand ? CmdPrefix : VarPrefix) + cv.Name;

    private static string WantedHandlerName(ConVarInfo cv) => WantedName(cv) + HandlerSuffix;

    private static bool LooksLikeOurHandlerName(string name)
        => name.StartsWith(CmdPrefix, StringComparison.Ordinal) && name.EndsWith(HandlerSuffix, StringComparison.Ordinal);

    private static bool IsAutoName(string name)
    {
        foreach (string prefix in AutoNamePrefixes)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsOurs(string current, string wanted) => current.StartsWith(wanted, StringComparison.Ordinal);

    private static string GetName(ulong ea)
    {
        var qs = new QString();
        nint len = IdaNative.get_ea_name(&qs, ea, 0, null);
        string text = len > 0 ? qs.Read() : string.Empty;
        qs.Dispose();
        return text;
    }

    private static bool SetName(ulong ea, string name, int flags)
    {
        byte* namePtr = Utf8.Allocate(name);
        bool ok = IdaNative.set_name(ea, namePtr, flags) != 0;
        Utf8.Free(namePtr);
        return ok;
    }

    private static Dictionary<ulong, ConVarInfo> HandlerOwners(List<ConVarInfo> all)
    {
        var owners = new Dictionary<ulong, ConVarInfo>();

        foreach (var cv in all)
        {
            if (!cv.IsCommand || cv.Callback == BadAddr)
            {
                continue;
            }

            if (!owners.TryGetValue(cv.Callback, out var existing) || string.CompareOrdinal(cv.Name, existing.Name) < 0)
            {
                owners[cv.Callback] = cv;
            }
        }

        return owners;
    }

    private static bool OwnsHandler(ConVarInfo cv, Dictionary<ulong, ConVarInfo> owners)
        => cv.IsCommand && cv.Callback != BadAddr && owners.TryGetValue(cv.Callback, out var owner) && owner == cv;

    private static bool NameHandler(ConVarInfo cv, Dictionary<ulong, ConVarInfo> owners)
    {
        if (!OwnsHandler(cv, owners))
        {
            return false;
        }

        string wanted = WantedHandlerName(cv);
        string current = GetName(cv.Callback);

        if (current.Length > 0)
        {
            if (current == wanted)
            {
                return false;
            }

            if (!IsAutoName(current) && !LooksLikeOurHandlerName(current))
            {
                return false;
            }
        }

        return SetName(cv.Callback, wanted, SnNoCheck | SnForce);
    }

    // func_t::flags FUNC_THUNK.
    private const ulong FuncThunk = 0x80;

    private static readonly string[] ConVarValueTypeNames = Schema.SchemaHeaderGenerator.ConVarValueTypes;

    private static bool ApplyType(ConVarInfo cv)
    {
        if (cv.IsCommand || cv.ValueType < 0 || cv.ValueType >= ConVarValueTypeNames.Length ||
            // An explicit type, whether the user's or a previous run's, is never overwritten.
            (IdaNative.get_aflags(cv.Object) & UserTypeFlag) != 0)
        {
            return false;
        }

        string value = ConVarValueTypeNames[cv.ValueType];
        // CConVar<T> exists once the SDK headers were imported; CConVarRef<T> is its base and has the
        // same layout, and some SDK translation units only instantiate that one.
        foreach (string template in (string[])["CConVar", "CConVarRef"])
        {
            TypeInfo type = default;
            var name = new QString();
            byte* declaration = Utf8.Allocate($"{template}<{value}> __s2_convar;");
            try
            {
                if (IdaNative.parse_decl(&type, &name, IdaNative.get_idati(), declaration, PtSilent | PtVariable | PtHigh) != 0 &&
                    IdaNative.apply_tinfo(cv.Object, &type, TinfoDefinite) != 0)
                {
                    return true;
                }
            }
            finally
            {
                Utf8.Free(declaration);
                name.Dispose();
                type.Dispose();
            }
        }

        return false;
    }

    private static bool Apply(ConVarInfo cv)
    {
        string current = GetName(cv.Object);
        string wanted = WantedName(cv);
        bool named = current.Length > 0;

        if (named && IsOurs(current, wanted))
        {
            return false;
        }

        if (named && !IsAutoName(current))
        {
            return false;
        }

        if (!SetName(cv.Object, wanted, SnNoCheck | SnForce))
        {
            return false;
        }

        if (cv.Description.Length > 0)
        {
            byte* cmt = Utf8.Allocate(cv.Description);
            IdaNative.set_cmt(cv.Object, cmt, 1);
            Utf8.Free(cmt);
        }

        return true;
    }
}
