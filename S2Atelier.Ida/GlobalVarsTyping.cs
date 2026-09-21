using System.Text;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

internal sealed record GlobalVarsTypingSummary(
    string Module, string Era, string Types, ulong? GpGlobals, ulong? FallbackGlobals, ulong? WarningFunc,
    ulong? ClientOffset, ulong? ServerOffset, int Helpers, int Skipped, List<string> Warnings);

/// <summary>
/// Finds the CGlobalVars a module uses and types it for the layout of its build. A game DLL keeps a pointer,
/// gpGlobals, which its SetGlobals stores together with the usage-warning callback (the store is followed by
/// <c>mov [reg+warn], callback</c>); engine2 embeds one instance in its network game client and one in its
/// server, and passes them to the time-scope helpers. The layout changed four times; the era is read from
/// the code (the warning offset, which flags and fields the module touches), never from a date. hl2sdk's
/// declaration is used when it matches that era, the era's own table otherwise.
/// </summary>
internal static unsafe class GlobalVarsTyping
{
    private const string BaseType = "CGlobalVarsBase";
    private const string GlobalsType = "CGlobalVars";
    private const string ScopeType = "CGlobalVarsTimeScope";
    private const string FunctionSource = "global vars";
    private const int HtiDcl = 0x400;
    private const int AccessWindow = 14;
    private const ulong MaxField = 0x80;
    private const int MinFallbackLoads = 300;
    private const int MaxHelperSize = 0x200;
    private const long StackArguments = 0x28;
    private const byte DtByte = 0, DtDword = 2, DtFloat = 3, DtQword = 7;

    internal sealed record Field(ulong Offset, string Type, string Name);

    internal sealed record Era(string Name, ulong Size, ulong Warn, ulong Curtime, ulong Frametime, ulong Tickcount,
        ulong InSimulation, ulong? Subtick, ulong? ThreadId, IReadOnlyList<Field> Fields);

    private static readonly Field[] Head =
    [
        new(0x00, "float", "realtime"), new(0x04, "int", "framecount"), new(0x08, "float", "absoluteframetime"),
        new(0x0C, "float", "absoluteframetime_unbounded"), new(0x10, "int", "maxClients"),
        new(0x14, "int", "m_nCurrentTickThisFrame"), new(0x18, "int", "m_nTotalTicksThisFrame"),
    ];

    private static readonly Field[] CurrentMiddle =
    [
        new(0x1C, "float", "m_flUsercmdTickInterval"), new(0x20, "double", "m_flUsercmdTickStartTime"),
        new(0x28, "FnGlobalVarsWarningFunc", "m_pfnWarningFunc"), new(0x30, "float", "curtime"),
        new(0x34, "float", "frametime"), new(0x38, "float", "interpolation_amount"),
        new(0x3C, "float", "m_flUsercmdTickFraction"), new(0x40, "bool", "m_bInSimulation"),
        new(0x41, "bool", "m_bEnableAssertions"), new(0x44, "int", "tickcount"), new(0x48, "int", "m_nClientTickCount"),
        new(0x4C, "float", "m_flClientTime"), new(0x50, "float", "m_flSubtickFraction"),
    ];

    /// <summary>The four layouts, oldest first. Names that were never verified for an era are m_unkXX.</summary>
    internal static readonly Era[] Eras =
    [
        new("A", 0x48, 0x20, 0x2C, 0x28, 0x40, 0x3C, 0x44, null,
        [
            new(0x00, "float", "realtime"), new(0x04, "int", "framecount"), new(0x08, "float", "absoluteframetime"),
            new(0x0C, "float", "m_unk0C"), new(0x10, "int", "maxClients"), new(0x14, "int", "m_unk14"),
            new(0x18, "int", "m_unk18"), new(0x20, "FnGlobalVarsWarningFunc", "m_pfnWarningFunc"),
            new(0x28, "float", "frametime"), new(0x2C, "float", "curtime"), new(0x30, "float", "rendertime"),
            new(0x34, "float", "m_unk34"), new(0x38, "float", "m_unk38"), new(0x3C, "bool", "m_bInSimulation"),
            new(0x3D, "bool", "m_bEnableAssertions"), new(0x40, "int", "tickcount"), new(0x44, "float", "m_flSubtickFraction"),
        ]),
        new("B", 0x58, 0x28, 0x34, 0x30, 0x48, 0x44, 0x54, null,
        [
            .. Head,
            new(0x1C, "float", "m_flUsercmdTickInterval"), new(0x20, "double", "m_flUsercmdTickStartTime"),
            new(0x28, "FnGlobalVarsWarningFunc", "m_pfnWarningFunc"), new(0x30, "float", "frametime"),
            new(0x34, "float", "curtime"), new(0x38, "float", "rendertime"), new(0x3C, "float", "m_unk3C"),
            new(0x40, "float", "m_unk40"), new(0x44, "bool", "m_bInSimulation"), new(0x45, "bool", "m_bEnableAssertions"),
            new(0x48, "int", "tickcount"), new(0x4C, "int", "m_nClientTickCount"), new(0x50, "float", "m_flClientTime"),
            new(0x54, "float", "m_flSubtickFraction"),
        ]),
        new("C", 0x58, 0x28, 0x30, 0x34, 0x44, 0x40, 0x50, null, [.. Head, .. CurrentMiddle]),
        new("D", 0x60, 0x28, 0x30, 0x34, 0x44, 0x40, 0x50, 0x58,
        [
            .. Head, .. CurrentMiddle,
            new(0x54, "bool", "m_bIsOncePerFrameAsyncWorkPhase"), new(0x58, "unsigned int", "m_nThreadId"),
        ]),
    ];

    private static readonly ushort[] MsvcArguments = [1, 2, 8, 9];
    private static readonly ushort[] SystemVArguments = [7, 6, 2, 1, 8, 9];
    private const ushort Rax = 0, Rsp = 4, Rbp = 5;

    internal static GlobalVarsTypingSummary Apply(string module, string? hl2SdkPath, SchemaTargetPlatform platform,
        Action<string> diagnostic)
    {
        bool msvc = platform == SchemaTargetPlatform.WindowsMsvc;
        ushort[] arguments = msvc ? MsvcArguments : SystemVArguments;
        var warnings = new List<string>();
        int skipped = 0;

        var anchor = FindSetGlobals(diagnostic);
        ulong? gpGlobals = anchor?.Global;
        if (anchor == null && FallbackGlobal() is ulong statistic)
        {
            gpGlobals = statistic;
            diagnostic($"[globals] no SetGlobals store; gpGlobals taken from its accesses: 0x{statistic:X}.");
        }

        // Without a gpGlobals the module may be engine2, which embeds the instances instead.
        var helpers = gpGlobals == null ? FindTimeScopeHelpers(arguments) : [];
        ulong? clientOffset = null, serverOffset = null;
        if (gpGlobals == null)
        {
            (clientOffset, serverOffset) = EmbeddedOffsets(helpers, arguments, platform, warnings);
        }

        if (gpGlobals == null && helpers.Count == 0 && clientOffset == null && serverOffset == null)
        {
            return new(module, "-", "-", null, null, null, null, null, 0, 0, warnings);
        }

        // The era: from the game DLL's accesses, or from the fields engine2's time scopes save.
        Era era;
        string signals;
        if (gpGlobals is ulong global)
        {
            var accesses = Accesses(global);
            (era, signals) = DetectEra(anchor?.Warn, accesses);
        }
        else if (helpers.Count > 0)
        {
            (era, signals) = EraFromHelpers(helpers);
        }
        else
        {
            // Clang inlines the time scopes; nothing else in engine2 tells the era.
            era = Eras[^1];
            signals = "no time-scope helper to read it from; the latest assumed";
            warnings.Add($"era {era.Name} assumed: engine2 has no time-scope helper to read the era from");
        }

        diagnostic($"[globals] {module}: era {era.Name} ({signals}).");

        // Types: hl2sdk's when they match the era, the era's table otherwise.
        string types = PrepareTypes(era, hl2SdkPath, platform, warnings, diagnostic);

        if (gpGlobals is ulong pointer)
        {
            EntityClassNaming.NameData(pointer, "gpGlobals", ref skipped);
            EntityClassNaming.ApplyType(pointer, $"{GlobalsType} *__s2_globals;");
        }

        if (anchor?.Fallback is ulong fallback)
        {
            EntityClassNaming.NameData(fallback, "g_DefaultGlobalVars", ref skipped);
            EntityClassNaming.ApplyType(fallback, $"{GlobalsType} __s2_default_globals;");
        }

        if (anchor?.Callback is ulong callback)
        {
            SdkFunctionBinding.TryNameFunction(callback, "GlobalVarsWarningFunc", false, FunctionSource, diagnostic);
            if (SdkFunctionBinding.CanUpdateType(callback) &&
                EntityClassNaming.ApplyType(callback, "void __fastcall f(GlobalVarsUsageWarning_t warning);"))
            {
                SdkFunctionBinding.RecordType(callback, FunctionSource, diagnostic);
            }
        }

        if (helpers.Count > 0)
        {
            DeclareScope();
            foreach (var helper in helpers)
            {
                // The variants that take a frametime also take whether the scope keeps the globals:
                // BeginTick(tick), BeginTickFrametime(tick, frametime, keep), Begin(curtime, frametime, keep)
                // and BeginCurtime(curtime).
                bool frametime = helper.Kind != "End" && TakesFrametime(helper.Address, msvc);
                string name = (helper.Kind, frametime) switch
                {
                    ("BeginTick", true) => "BeginTickFrametime",
                    ("Begin", false) => "BeginCurtime",
                    _ => helper.Kind,
                };
                SdkFunctionBinding.TryNameFunction(helper.Address, $"{ScopeType}__{name}", false, FunctionSource, diagnostic);
                string head = $"{ScopeType} *__fastcall f({ScopeType} *scope, const char *location, {GlobalsType} *globals, " +
                              (helper.Kind == "BeginTick" ? "int tick" : "float curtime");
                string prototype = helper.Kind == "End"
                    ? $"void __fastcall f({ScopeType} *scope);"
                    : frametime ? head + ", float frametime, bool keep);" : head + ");";
                if (SdkFunctionBinding.CanUpdateType(helper.Address) && EntityClassNaming.ApplyType(helper.Address, prototype))
                {
                    SdkFunctionBinding.RecordType(helper.Address, FunctionSource, diagnostic);
                }
            }

        }

        if (clientOffset is ulong client) DeclareHolder("CNetworkGameClient", client, warnings);
        if (serverOffset is ulong server) DeclareHolder("CNetworkGameServerBase", server, warnings);

        return new(module, era.Name, types, gpGlobals, anchor?.Fallback, anchor?.Callback, clientOffset, serverOffset,
            helpers.Count, skipped, warnings);
    }

    // ---- game DLLs ---------------------------------------------------------------------------------------

    internal sealed record SetGlobalsStore(ulong Global, ulong Warn, ulong? Callback, ulong? Fallback);

    /// <summary>
    /// SetGlobals: <c>mov [rip+gpGlobals], reg</c>, then at once <c>mov [reg+warn], callback</c>. The register
    /// was rcx in 2023 and is rdx or rsi now, so it is not fixed; the callback is a function whose address a
    /// register holds, and the fallback instance the data address a <c>cmovz</c> puts into the register.
    /// </summary>
    private static SetGlobalsStore? FindSetGlobals(Action<string> diagnostic)
    {
        var found = new List<SetGlobalsStore>();
        byte* insn = stackalloc byte[Insn.BufferSize];
        byte* next = stackalloc byte[Insn.BufferSize];
        for (nuint i = 0, count = IdaNative.get_func_qty(); i < count; i++)
        {
            void* function = IdaNative.getn_func(i);
            if (function == null)
            {
                continue;
            }

            ulong start = *(ulong*)function, end = *((ulong*)function + 1);
            var addresses = new Dictionary<ushort, ulong>();
            var fallbacks = new Dictionary<ushort, ulong>();
            for (ulong ea = start; ea < end && ea != ulong.MaxValue; ea = IdaNative.next_head(ea, end))
            {
                if (!Insn.TryDecode(ea, insn))
                {
                    continue;
                }

                string mnemonic = Mnemonic(ea);
                byte first = Insn.OpType(insn, 0), second = Insn.OpType(insn, 1);
                if (first == Insn.OpReg)
                {
                    ushort target = Insn.OpRegister(insn, 0);
                    if (mnemonic == "lea" && second == Insn.OpMem)
                    {
                        addresses[target] = Insn.OpAddr(insn, 1);
                    }
                    else if (mnemonic.StartsWith("cmov", StringComparison.Ordinal) && second == Insn.OpReg &&
                             addresses.TryGetValue(Insn.OpRegister(insn, 1), out ulong alternative))
                    {
                        fallbacks[target] = alternative;
                    }
                    else if (mnemonic is not ("test" or "cmp"))
                    {
                        addresses.Remove(target);
                    }

                    continue;
                }

                if (mnemonic != "mov" || first != Insn.OpMem || second != Insn.OpReg || Insn.OpWidth(insn, 0) != DtQword)
                {
                    continue;
                }

                ushort stored = Insn.OpRegister(insn, 1);
                ulong following = IdaNative.next_head(ea, end);
                if (following == ulong.MaxValue || !Insn.TryDecode(following, next) || Mnemonic(following) != "mov" ||
                    Insn.OpType(next, 0) != Insn.OpDispl || Insn.OpRegister(next, 0) != stored ||
                    Insn.OpType(next, 1) != Insn.OpReg ||
                    !addresses.TryGetValue(Insn.OpRegister(next, 1), out ulong callback) || !IsFunctionStart(callback))
                {
                    continue;
                }

                ulong warn = Insn.OpAddr(next, 0);
                if (warn is not (0x20 or 0x28))
                {
                    diagnostic($"[globals] 0x{ea:X}: a SetGlobals-like store with warn offset 0x{warn:X}; skipped.");
                    continue;
                }

                found.Add(new(Insn.OpAddr(insn, 0), warn, callback,
                    fallbacks.TryGetValue(stored, out ulong fallback) ? fallback : null));
            }
        }

        var distinct = found.DistinctBy(x => x.Global).ToList();
        if (distinct.Count > 1)
        {
            diagnostic($"[globals] {distinct.Count} SetGlobals-like stores: " +
                       string.Join(", ", distinct.Select(x => $"0x{x.Global:X}")) + "; taking the most used global.");
        }

        return distinct.OrderByDescending(x => Xrefs.DataTo(x.Global).Count()).FirstOrDefault();
    }

    // The qword global whose loads are most often followed by a float access at a small offset.
    private static ulong? FallbackGlobal()
    {
        var scores = new Dictionary<ulong, int>();
        byte* insn = stackalloc byte[Insn.BufferSize];
        for (nuint i = 0, count = IdaNative.get_func_qty(); i < count; i++)
        {
            void* function = IdaNative.getn_func(i);
            if (function == null)
            {
                continue;
            }

            ulong start = *(ulong*)function, end = *((ulong*)function + 1);
            for (ulong ea = start; ea < end && ea != ulong.MaxValue; ea = IdaNative.next_head(ea, end))
            {
                if (Insn.TryDecode(ea, insn) && Insn.OpType(insn, 0) == Insn.OpReg && Insn.OpType(insn, 1) == Insn.OpMem &&
                    Insn.OpWidth(insn, 1) == DtQword && Mnemonic(ea) == "mov" &&
                    Follow(ea, end, Insn.OpRegister(insn, 0)).Any(x => x.Width == DtFloat))
                {
                    ulong global = Insn.OpAddr(insn, 1);
                    scores[global] = scores.GetValueOrDefault(global) + 1;
                }
            }
        }

        var best = scores.OrderByDescending(x => x.Value).FirstOrDefault();
        return best.Value >= MinFallbackLoads ? best.Key : null;
    }

    /// <summary>How the module touches the global's fields: (offset, width) to count.</summary>
    internal static Dictionary<(ulong Offset, byte Width), int> Accesses(ulong global)
    {
        var counts = new Dictionary<(ulong, byte), int>();
        byte* insn = stackalloc byte[Insn.BufferSize];
        foreach (ulong load in Xrefs.DataTo(global))
        {
            void* function = IdaNative.get_func(load);
            if (function == null || !Insn.TryDecode(load, insn) || Mnemonic(load) is not ("mov" or "lea") ||
                Insn.OpType(insn, 0) != Insn.OpReg || Insn.OpType(insn, 1) != Insn.OpMem || Insn.OpAddr(insn, 1) != global)
            {
                continue;
            }

            foreach (var access in Follow(load, *((ulong*)function + 1), Insn.OpRegister(insn, 0), Mnemonic(load) == "lea"))
            {
                counts[access] = counts.GetValueOrDefault(access) + 1;
            }
        }

        return counts;
    }

    // The fields read or written through a register loaded at ea, in the straight-line code after it. With
    // address, the register holds the global's address and a load through it gives the pointer.
    private static List<(ulong Offset, byte Width)> Follow(ulong ea, ulong end, ushort register, bool address = false)
    {
        var found = new List<(ulong, byte)>();
        var tracked = new HashSet<ushort>();
        var addresses = new HashSet<ushort>();
        (address ? addresses : tracked).Add(register);
        byte* insn = stackalloc byte[Insn.BufferSize];
        for (int i = 0; i < AccessWindow && tracked.Count + addresses.Count > 0; i++)
        {
            ea = IdaNative.next_head(ea, end);
            if (ea == ulong.MaxValue || !Insn.TryDecode(ea, insn) || Insn.IsCall(insn))
            {
                break;
            }

            string mnemonic = Mnemonic(ea);
            if (mnemonic is "jmp" or "retn" or "ret")
            {
                break;
            }

            for (int operand = 0; operand < 2; operand++)
            {
                byte type = Insn.OpType(insn, operand);
                if (type is Insn.OpDispl or Insn.OpPhrase && tracked.Contains(Insn.OpRegister(insn, operand)) &&
                    !addresses.Contains(Insn.OpRegister(insn, operand)))
                {
                    ulong offset = type == Insn.OpDispl ? Insn.OpAddr(insn, operand) : 0;
                    if (offset < MaxField)
                    {
                        found.Add((offset, IsScalarFloat(mnemonic) ? DtFloat : Insn.OpWidth(insn, operand)));
                    }
                }
            }

            if (Insn.OpType(insn, 0) == Insn.OpReg)
            {
                ushort target = Insn.OpRegister(insn, 0);
                byte source = Insn.OpType(insn, 1);
                if (mnemonic == "mov" && source == Insn.OpPhrase && addresses.Contains(Insn.OpRegister(insn, 1)))
                {
                    addresses.Remove(target);
                    tracked.Add(target);
                }
                else if (mnemonic == "mov" && source == Insn.OpReg && tracked.Contains(Insn.OpRegister(insn, 1)))
                {
                    tracked.Add(target);
                }
                else if (mnemonic is not ("cmp" or "test"))
                {
                    tracked.Remove(target);
                    addresses.Remove(target);
                }
            }
        }

        return found;
    }

    /// <summary>
    /// The era from the warning offset and the fields the code touches: era A keeps the callback at 0x20;
    /// B reads it at 0x28 through checked accessors, which C compiled out; D adds the byte at 0x54 and the
    /// thread id at 0x58, where C's derived part begins with a pointer.
    /// </summary>
    internal static (Era Era, string Signals) DetectEra(ulong? warn, IReadOnlyDictionary<(ulong Offset, byte Width), int> accesses)
    {
        int Count(ulong offset, byte width) => accesses.GetValueOrDefault((offset, width));
        int warnLoads = Count(0x28, DtQword);
        int asyncPhase = Count(0x54, DtByte);
        int threadId = Count(0x58, DtDword);
        string name = warn == 0x20 ? "A"
            : warnLoads >= 20 ? "B"
            : asyncPhase + threadId > 0 ? "D"
            : "C";
        Era era = Eras.Single(x => x.Name == name);
        string signals = $"warn 0x{warn ?? era.Warn:X}, [g+0x28] qword loads {warnLoads}, [g+0x54] byte accesses {asyncPhase}, " +
                         $"[g+0x58] dword accesses {threadId}, " +
                         $"tickcount 0x{era.Tickcount:X} int accesses {Count(era.Tickcount, DtDword)}, " +
                         $"curtime 0x{era.Curtime:X} float accesses {Count(era.Curtime, DtFloat)}";
        return (era, signals);
    }

    // ---- engine2 -----------------------------------------------------------------------------------------

    internal sealed record Helper(ulong Address, string Kind, IReadOnlySet<ulong> Fields);

    /// <summary>
    /// The time-scope helpers: a Begin stores its location and globals arguments at scope+0 and scope+8 and
    /// reads the fields it saves; End loads the globals from scope+8 and swaps them back, calling nothing. A
    /// Begin that calls a local function before setting curtime converts a tick to a time: BeginTick.
    /// </summary>
    private static List<Helper> FindTimeScopeHelpers(ushort[] arguments)
    {
        var found = new List<Helper>();
        byte* insn = stackalloc byte[Insn.BufferSize];
        for (nuint i = 0, count = IdaNative.get_func_qty(); i < count; i++)
        {
            void* function = IdaNative.getn_func(i);
            if (function == null)
            {
                continue;
            }

            ulong start = *(ulong*)function, end = *((ulong*)function + 1);
            if (end - start > MaxHelperSize)
            {
                continue;
            }

            bool location = false, globals = false, loadsGlobals = false;
            int localCalls = 0, callsBeforeCurtime = -1;
            var read = new HashSet<ulong>();
            var written = new HashSet<ulong>();
            var scope = new HashSet<ushort> { arguments[0] };
            var fields = new HashSet<ushort> { arguments[2] };
            for (ulong ea = start; ea < end && ea != ulong.MaxValue; ea = IdaNative.next_head(ea, end))
            {
                if (!Insn.TryDecode(ea, insn))
                {
                    continue;
                }

                if (Insn.IsCall(insn))
                {
                    if (Insn.OpType(insn, 0) == Insn.OpNear && IsFunctionStart(Insn.OpAddr(insn, 0)))
                    {
                        localCalls++;
                    }

                    continue;
                }

                string mnemonic = Mnemonic(ea);
                byte first = Insn.OpType(insn, 0), second = Insn.OpType(insn, 1);
                ulong Offset(int operand) => Insn.OpType(insn, operand) == Insn.OpDispl ? Insn.OpAddr(insn, operand) : 0;
                if (mnemonic == "mov" && first is Insn.OpDispl or Insn.OpPhrase && scope.Contains(Insn.OpRegister(insn, 0)) &&
                    second == Insn.OpReg)
                {
                    location |= Offset(0) == 0 && Insn.OpRegister(insn, 1) == arguments[1];
                    globals |= Offset(0) == 8 && fields.Contains(Insn.OpRegister(insn, 1));
                }

                if (first == Insn.OpReg && mnemonic == "mov" && second is Insn.OpDispl && scope.Contains(Insn.OpRegister(insn, 1)) &&
                    Offset(1) == 8)
                {
                    fields.Add(Insn.OpRegister(insn, 0));
                    loadsGlobals = true;
                    continue;
                }

                if (first is Insn.OpDispl or Insn.OpPhrase && fields.Contains(Insn.OpRegister(insn, 0)))
                {
                    written.Add(Offset(0));
                    if (callsBeforeCurtime < 0 && IsScalarFloat(mnemonic))
                    {
                        callsBeforeCurtime = localCalls;
                    }
                }

                if (second is Insn.OpDispl or Insn.OpPhrase && fields.Contains(Insn.OpRegister(insn, 1)))
                {
                    read.Add(Offset(1));
                }

                if (first == Insn.OpReg)
                {
                    ushort target = Insn.OpRegister(insn, 0);
                    if (mnemonic == "mov" && second == Insn.OpReg)
                    {
                        ushort source = Insn.OpRegister(insn, 1);
                        if (scope.Contains(source)) scope.Add(target); else scope.Remove(target);
                        if (fields.Contains(source)) fields.Add(target); else fields.Remove(target);
                    }
                    else if (mnemonic is not ("cmp" or "test"))
                    {
                        scope.Remove(target);
                        fields.Remove(target);
                    }
                }
            }

            // Every era saves curtime, frametime and tickcount; a helper touches at least three fields.
            if (location && globals && read.Count >= 3)
            {
                found.Add(new(start, callsBeforeCurtime > 0 ? "BeginTick" : "Begin", read));
            }
            else if (!location && loadsGlobals && written.Count >= 3 && localCalls == 0)
            {
                found.Add(new(start, "End", written));
            }
        }

        // An End is only one if a Begin saved the same fields.
        var saved = found.Where(x => x.Kind != "End").SelectMany(x => x.Fields).ToHashSet();
        return saved.Count == 0 ? [] : [.. found.Where(x => x.Kind != "End" || x.Fields.All(saved.Contains))];
    }

    private static (Era, string) EraFromHelpers(List<Helper> helpers)
    {
        var fields = helpers.Where(x => x.Kind != "End").SelectMany(x => x.Fields).ToHashSet();
        Era era = Eras.Reverse().FirstOrDefault(x => fields.Contains(x.Tickcount) && fields.Contains(x.Curtime) &&
                                                     (x.ThreadId == null || fields.Contains(x.ThreadId.Value)))
                  ?? Eras[^1];
        return (era, "time scopes save " + string.Join(", ", fields.Order().Select(x => $"0x{x:X}")));
    }

    /// <summary>
    /// The offsets of the two embedded instances. The server's is what CNetworkGameServerBase's getter returns
    /// (lea rax, [this+offset]; ret), confirmed by what the time scopes' callers pass as the globals (lea reg,
    /// [this+offset]) when there are any; the client's is the instance AdvanceTime fills, or else the other
    /// offset the time scopes are passed.
    /// </summary>
    private static (ulong? Client, ulong? Server) EmbeddedOffsets(List<Helper> helpers, ushort[] arguments,
        SchemaTargetPlatform platform, List<string> warnings)
    {
        var passed = new Dictionary<ulong, int>();
        byte* insn = stackalloc byte[Insn.BufferSize];
        foreach (var helper in helpers.Where(x => x.Kind != "End"))
        {
            foreach (ulong call in Xrefs.CodeTo(helper.Address))
            {
                void* function = IdaNative.get_func(call);
                if (function == null)
                {
                    continue;
                }

                ulong ea = call;
                for (int i = 0; i < AccessWindow; i++)
                {
                    ea = IdaNative.prev_head(ea, *(ulong*)function);
                    if (ea == ulong.MaxValue || !Insn.TryDecode(ea, insn) || Insn.IsCall(insn))
                    {
                        break;
                    }

                    if (Insn.OpType(insn, 0) != Insn.OpReg || Insn.OpRegister(insn, 0) != arguments[2])
                    {
                        continue;
                    }

                    if (Mnemonic(ea) == "lea" && Insn.OpType(insn, 1) == Insn.OpDispl &&
                        Insn.OpRegister(insn, 1) is not (Rsp or Rbp))
                    {
                        ulong offset = Insn.OpAddr(insn, 1);
                        passed[offset] = passed.GetValueOrDefault(offset) + 1;
                    }

                    break;
                }
            }
        }

        var abi = platform == SchemaTargetPlatform.WindowsMsvc ? VTableAbi.Msvc : VTableAbi.Itanium;
        // Offset returned to the vtable byte offsets of the slots that return it.
        var getters = new Dictionary<ulong, HashSet<ulong>>();
        foreach ((ulong addressPoint, IReadOnlyList<ulong> functions, VTableAbi _) in new SdkInterfaceBinding.Image().PrimaryTables(platform))
        {
            if (RttiChain.Read(addressPoint, abi) is { Count: > 0 } chain && chain[0] == "CNetworkGameServerBase")
            {
                for (int slot = 0; slot < functions.Count; slot++)
                {
                    if (Getter(functions[slot], arguments[0]) is ulong offset)
                    {
                        if (!getters.TryGetValue(offset, out var slots)) getters[offset] = slots = [];
                        slots.Add((ulong)slot * 8);
                    }
                }
            }
        }

        // The client instance is the one AdvanceTime writes realtime, framecount and the two frame times into;
        // it reaches the server's through the getter, a virtual call, so that call picks the getter.
        var (advanced, virtualCalls) = AdvanceTimeOffsets();
        var ranked = passed.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
        var called = getters.Where(x => x.Value.Overlaps(virtualCalls)).Select(x => x.Key).ToList();
        ulong? server = ranked.Where(getters.ContainsKey).Select(x => (ulong?)x).FirstOrDefault()
                        ?? (getters.Count == 1 ? getters.Keys.Single() : called.Count == 1 ? called[0] : null);
        ulong? client = advanced.Where(x => x != server).Select(x => (ulong?)x).FirstOrDefault()
                        ?? ranked.Where(x => x != server).Select(x => (ulong?)x).FirstOrDefault();
        if (server == null && getters.Count > 0)
        {
            warnings.Add("CNetworkGameServerBase getters return " + string.Join(", ", getters.Keys.Order().Select(x => $"0x{x:X}")) +
                         "; none is confirmed as its globals");
        }

        if (client != null && ranked.Count > 0 && !ranked.Contains(client.Value))
        {
            warnings.Add($"AdvanceTime writes a client instance at 0x{client:X} the time scopes are never passed");
        }

        return (client, server);
    }

    /// <summary>
    /// Offsets o where the function with AdvanceTime's log string stores a float at o, an int at o+4 and floats
    /// at o+8 and o+0xC: realtime, framecount and the two frame times of an embedded instance. The server's is
    /// written through its getter, at offset 0, and is left out.
    /// </summary>
    private static (List<ulong> Offsets, HashSet<ulong> VirtualCalls) AdvanceTimeOffsets()
    {
        var floats = new HashSet<ulong>();
        var ints = new HashSet<ulong>();
        var calls = new HashSet<ulong>();
        byte* insn = stackalloc byte[Insn.BufferSize];
        foreach (ulong text in StringsContaining("AdvanceTime ticks this frame"))
        {
            foreach (ulong use in Xrefs.DataTo(text))
            {
                void* function = IdaNative.get_func(use);
                if (function == null)
                {
                    continue;
                }

                ulong end = *((ulong*)function + 1);
                for (ulong ea = *(ulong*)function; ea < end && ea != ulong.MaxValue; ea = IdaNative.next_head(ea, end))
                {
                    if (!Insn.TryDecode(ea, insn))
                    {
                        continue;
                    }

                    if (Insn.IsCall(insn) && Insn.OpType(insn, 0) == Insn.OpDispl)
                    {
                        calls.Add(Insn.OpAddr(insn, 0));
                        continue;
                    }

                    if (Insn.OpType(insn, 0) != Insn.OpDispl || Insn.OpRegister(insn, 0) is Rsp or Rbp ||
                        Insn.OpType(insn, 1) != Insn.OpReg)
                    {
                        continue;
                    }

                    string mnemonic = Mnemonic(ea);
                    if (mnemonic == "movss") floats.Add(Insn.OpAddr(insn, 0));
                    else if (mnemonic == "mov" && Insn.OpWidth(insn, 0) == DtDword) ints.Add(Insn.OpAddr(insn, 0));
                }
            }
        }

        return ([.. floats.Where(o => o != 0 && ints.Contains(o + 4) && floats.Contains(o + 8) && floats.Contains(o + 0xC)).Order()],
            calls);
    }

    private static List<ulong> StringsContaining(string wanted)
    {
        var found = new List<ulong>();
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
                    text.Read().Contains(wanted, StringComparison.Ordinal))
                {
                    found.Add(ea);
                }
            }
            finally { text.Dispose(); }
        }

        return found;
    }

    /// <summary>
    /// Whether a Begin variant takes a frametime and keep flag as well: its fifth and sixth arguments. Under
    /// MSVC they are on the stack past the four home slots, which the function reads; System V passes them in
    /// registers, which its callers set.
    /// </summary>
    private static bool TakesFrametime(ulong function, bool msvc)
    {
        if (!msvc)
        {
            return GuessedPrototype.ArgumentsAtCalls(function).Count >= 6;
        }

        void* pfn = IdaNative.get_func(function);
        byte* insn = stackalloc byte[Insn.BufferSize];
        ulong end = *((ulong*)pfn + 1);
        for (ulong ea = function; ea < end && ea != ulong.MaxValue; ea = IdaNative.next_head(ea, end))
        {
            // Relative to the stack pointer at entry, the fifth argument is past the return address and the
            // four home slots.
            if (Insn.TryDecode(ea, insn) && Insn.OpType(insn, 1) == Insn.OpDispl && Insn.OpRegister(insn, 1) == Rsp &&
                (long)Insn.OpAddr(insn, 1) + IdaNative.get_spd(pfn, ea) >= StackArguments)
            {
                return true;
            }
        }

        return false;
    }

    // lea rax, [this+offset]; ret
    private static ulong? Getter(ulong function, ushort self)
    {
        byte* insn = stackalloc byte[Insn.BufferSize];
        if (!Insn.TryDecode(function, insn) || Mnemonic(function) != "lea" || Insn.OpType(insn, 0) != Insn.OpReg ||
            Insn.OpRegister(insn, 0) != Rax || Insn.OpType(insn, 1) != Insn.OpDispl || Insn.OpRegister(insn, 1) != self)
        {
            return null;
        }

        ulong offset = Insn.OpAddr(insn, 1);
        ulong next = IdaNative.next_head(function, ulong.MaxValue);
        return Mnemonic(next) is "retn" or "ret" ? offset : null;
    }

    // ---- types -------------------------------------------------------------------------------------------

    /// <summary>
    /// hl2sdk's CGlobalVarsBase when it has the era's layout; otherwise the era's own, replacing the
    /// existing definition under the same name so what is typed with it already picks the members up.
    /// </summary>
    private static string PrepareTypes(Era era, string? hl2SdkPath, SchemaTargetPlatform platform, List<string> warnings,
        Action<string> diagnostic)
    {
        if (TypeLayout.Members(BaseType, out _) == null && hl2SdkPath != null)
        {
            ImportSdk(hl2SdkPath, platform, diagnostic);
        }

        if (TypeLayout.Members(BaseType, out ulong size) is { } members)
        {
            var differences = Differences(era, members, size);
            if (differences.Count == 0)
            {
                return "sdk";
            }

            warnings.Add($"hl2sdk's {BaseType} is not era {era.Name}'s: {string.Join("; ", differences)}; the era's layout applied");
        }

        int errors = Declare(EraDeclarations(era));
        if (errors != 0)
        {
            warnings.Add($"the era {era.Name} declarations had {errors} error(s)");
        }

        return $"era {era.Name}";
    }

    internal static List<string> Differences(Era era, IReadOnlyDictionary<string, ulong> members, ulong size)
    {
        var differences = new List<string>();
        if (size != era.Size)
        {
            differences.Add($"size 0x{size:X}, build 0x{era.Size:X}");
        }

        foreach (Field field in era.Fields.Where(x => !x.Name.StartsWith("m_unk", StringComparison.Ordinal)))
        {
            if (!members.TryGetValue(field.Name, out ulong offset))
            {
                differences.Add($"no {field.Name}");
            }
            else if (offset != field.Offset)
            {
                differences.Add($"{field.Name} at 0x{offset:X}, build 0x{field.Offset:X}");
            }
        }

        return differences;
    }

    internal static string EraDeclarations(Era era)
    {
        var text = new StringBuilder();
        text.AppendLine("enum GlobalVarsUsageWarning_t { GV_RENDERTIME_CALLED_DURING_SIMULATION = 0, GV_CURTIME_CALLED_DURING_RENDERING = 1 };");
        text.AppendLine("typedef void (__fastcall *FnGlobalVarsWarningFunc)(GlobalVarsUsageWarning_t);");
        text.AppendLine($"struct {BaseType}").AppendLine("{");
        ulong at = 0;
        foreach (Field field in era.Fields.OrderBy(x => x.Offset))
        {
            if (field.Offset > at)
            {
                text.AppendLine($"  char __pad{at:X2}[0x{field.Offset - at:X}];");
            }

            text.AppendLine($"  {field.Type} {field.Name};");
            at = field.Offset + FieldSize(field.Type);
        }

        if (era.Size > at)
        {
            text.AppendLine($"  char __pad{at:X2}[0x{era.Size - at:X}];");
        }

        text.AppendLine("};");
        // The derived part is not verified for CS2.
        text.AppendLine($"struct {GlobalsType} : {BaseType} {{ char m_Derived[0x20]; }};");
        return text.ToString();
    }

    private static ulong FieldSize(string type) => type switch
    {
        "bool" => 1,
        "double" or "FnGlobalVarsWarningFunc" => 8,
        _ => 4,
    };

    private static void DeclareScope()
    {
        if (TypeLayout.Members(ScopeType, out _) != null)
        {
            return;
        }

        Declare($$"""
            struct {{ScopeType}}
            {
              const char *m_pszLocation;
              {{GlobalsType}} *m_pGlobals;
              float m_flSavedCurtime;
              float m_flSavedCurtime2;
              float m_flSavedFrametime;
              int m_nSavedTickcount;
              float m_flSavedSubtickFraction;
              unsigned int m_nSavedThreadId;
            };
            """);
    }

    // A minimal class for the instance's owner, unless the database already has one.
    private static void DeclareHolder(string owner, ulong offset, List<string> warnings)
    {
        if (TypeLayout.Members(owner, out _) is { } existing)
        {
            if (!existing.ContainsKey("m_Globals"))
            {
                warnings.Add($"{owner} is already declared; m_Globals at 0x{offset:X} not added");
            }

            return;
        }

        Declare($"struct {owner} {{ char __pad0[0x{offset:X}]; {GlobalsType} m_Globals; }};");
    }

    private static int Declare(string declarations)
    {
        byte* text = Utf8.Allocate(declarations);
        try { return IdaNative.parse_decls(IdaNative.get_idati(), text, null, HtiDcl); }
        finally { Utf8.Free(text); }
    }

    private static void ImportSdk(string hl2SdkPath, SchemaTargetPlatform platform, Action<string> diagnostic)
    {
        string directory = Path.Combine(Path.GetTempPath(), $"s2atelier-globals-{Guid.NewGuid():N}");
        string header = Path.Combine(directory, "globalvars.hpp");
        try
        {
            Directory.CreateDirectory(directory);
            File.WriteAllText(header, "#pragma once\n#include \"globalvars.h\"\n");
            SchemaImport.ConfigureClang(hl2SdkPath, platform, skipLayoutAssertions: true, directory);
            if (SchemaImport.ParseHeader(header, testOnly: false, printDiagnostics: true) is int errors and not 0)
            {
                diagnostic($"[globals] IDAClang reported {errors} error(s) in globalvars.h.");
            }
        }
        finally
        {
            SchemaImport.ResetParser();
            try { Directory.Delete(directory, recursive: true); }
            catch (IOException) { }
        }
    }

    // IDA types a scalar SSE instruction's memory operand as a dword; the mnemonic tells the float.
    private static bool IsScalarFloat(string mnemonic)
        => mnemonic.EndsWith("ss", StringComparison.Ordinal) && mnemonic != "cmpxchg" ||
           mnemonic.StartsWith("cvtss", StringComparison.Ordinal);

    private static bool IsFunctionStart(ulong address)
    {
        void* function = IdaNative.get_func(address);
        return function != null && *(ulong*)function == address;
    }

    private static string Mnemonic(ulong ea)
    {
        var text = new QString();
        try { return IdaNative.print_insn_mnem(&text, ea) > 0 ? text.Read() : string.Empty; }
        finally { text.Dispose(); }
    }
}
