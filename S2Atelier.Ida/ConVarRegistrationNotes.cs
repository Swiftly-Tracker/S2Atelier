using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

/// <summary>
/// What a convar's registration shows beyond its name and type: the FCVAR_ flags it passes, written on the
/// line before the call, and for a command the ConCommand type of its object and the prototype its
/// callback is dispatched with.
/// </summary>
internal static unsafe class ConVarRegistrationNotes
{
    private const int AnteriorLine = 1000; // E_PREV
    private const string FlagPrefix = "// FCVAR_";
    private const uint UserTypeFlag = 0x02000000;
    private const uint TinfoDefinite = 0x0001;
    private const int PtSilent = 0x0001, PtVariable = 0x0008, PtHigh = 0x0080;
    private const string CallbackSource = "concommand callback";

    // tier1/convar.h, and two bits it does not name, spelled after it from what tier0 does with them:
    // bit 30 keeps a cvar out of TakeConVarSnapshot/ResetConVarsToSnapshot (volumes, gamma, password);
    // bit 34 makes registration skip the gameinfo.gi default/min/max override (cheat render toggles).
    private static readonly (int Bit, string Name)[] FlagNames =
    [
        (0, "FCVAR_LINKED_CONCOMMAND"), (1, "FCVAR_DEVELOPMENTONLY"), (2, "FCVAR_GAMEDLL"), (3, "FCVAR_CLIENTDLL"),
        (4, "FCVAR_HIDDEN"), (5, "FCVAR_PROTECTED"), (6, "FCVAR_SPONLY"), (7, "FCVAR_ARCHIVE"), (8, "FCVAR_NOTIFY"),
        (9, "FCVAR_USERINFO"), (10, "FCVAR_REFERENCE"), (11, "FCVAR_UNLOGGED"), (12, "FCVAR_INITIAL_SETVALUE"),
        (13, "FCVAR_REPLICATED"), (14, "FCVAR_CHEAT"), (15, "FCVAR_PER_USER"), (16, "FCVAR_DEMO"),
        (17, "FCVAR_DONTRECORD"), (18, "FCVAR_PERFORMING_CALLBACKS"), (19, "FCVAR_RELEASE"),
        (20, "FCVAR_MENUBAR_ITEM"), (21, "FCVAR_COMMANDLINE_ENFORCED"), (22, "FCVAR_NOT_CONNECTED"),
        (23, "FCVAR_VCONSOLE_FUZZY_MATCHING"), (24, "FCVAR_SERVER_CAN_EXECUTE"), (25, "FCVAR_CLIENT_CAN_EXECUTE"),
        (26, "FCVAR_SERVER_CANNOT_QUERY"), (27, "FCVAR_VCONSOLE_SET_FOCUS"), (28, "FCVAR_CLIENTCMD_CAN_EXECUTE"),
        (29, "FCVAR_EXECUTE_PER_TICK"), (30, "FCVAR_SNAPSHOT_IGNORED"), (32, "FCVAR_DEFENSIVE"),
        (34, "FCVAR_GAMEINFO_CANNOT_OVERRIDE"),
    ];

    /// <summary>The flags as the SDK spells them: FCVAR_A | FCVAR_B, unknown bits in hex.</summary>
    internal static string Describe(ulong flags)
    {
        if (flags == 0)
        {
            return "FCVAR_NONE";
        }

        var parts = new List<string>();
        ulong known = 0;
        foreach ((int bit, string name) in FlagNames)
        {
            if ((flags & (1ul << bit)) != 0)
            {
                parts.Add(name);
                known |= 1ul << bit;
            }
        }

        if ((flags & ~known) != 0)
        {
            parts.Add($"0x{flags & ~known:X}");
        }

        return string.Join(" | ", parts);
    }

    /// <summary>Writes the flags on the line before the registration call; a line someone else wrote stays.</summary>
    internal static bool CommentFlags(ConVarInfo cv)
    {
        if (!cv.HasFlags || cv.RegisteredAt == ulong.MaxValue)
        {
            return false;
        }

        string wanted = "// " + Describe(cv.Flags);
        var existing = new QString();
        try
        {
            if (IdaNative.get_extra_cmt(&existing, cv.RegisteredAt, AnteriorLine) > 0)
            {
                string current = existing.Read();
                if (current == wanted || !current.StartsWith(FlagPrefix, StringComparison.Ordinal))
                {
                    return false;
                }
            }
        }
        finally { existing.Dispose(); }

        byte* text = Utf8.Allocate(wanted);
        try { return IdaNative.update_extra_cmt(cv.RegisteredAt, AnteriorLine, text) != 0; }
        finally { Utf8.Free(text); }
    }

    /// <summary>
    /// Types a command's object ConCommand and its callback by how the registration dispatches it:
    /// (context, command), (command) or (). An interface callback is an object, not a function, and stays.
    /// </summary>
    internal static (bool Object, bool Callback) TypeCommand(ConVarInfo cv, Func<ConVarInfo, int?> kindOf)
    {
        bool typedObject = cv.Object != ulong.MaxValue && (IdaNative.get_aflags(cv.Object) & UserTypeFlag) == 0 &&
                           Apply(cv.Object, "ConCommand __s2_command;");

        if (cv.Callback == ulong.MaxValue || !IsFunctionStart(cv.Callback) || kindOf(cv) is not int kind ||
            !SdkFunctionBinding.CanUpdateType(cv.Callback))
        {
            return (typedObject, false);
        }

        string? prototype = kind switch
        {
            0 => "void __fastcall f(const CCommandContext *context, const CCommand *args);",
            2 => "void __fastcall f();",
            4 => "void __fastcall f(const CCommand *args);",
            _ => null,
        };
        if (prototype == null || !Apply(cv.Callback, prototype, variable: false))
        {
            return (typedObject, false);
        }

        SdkFunctionBinding.RecordType(cv.Callback, CallbackSource, _ => { });
        return (typedObject, true);
    }

    private static bool Apply(ulong address, string declaration, bool variable = true)
    {
        TypeInfo type = default;
        var name = new QString();
        byte* text = Utf8.Allocate(declaration);
        try
        {
            return IdaNative.parse_decl(&type, &name, IdaNative.get_idati(), text,
                       PtSilent | PtHigh | (variable ? PtVariable : 0)) != 0 &&
                   IdaNative.apply_tinfo(address, &type, TinfoDefinite) != 0;
        }
        finally
        {
            Utf8.Free(text);
            name.Dispose();
            type.Dispose();
        }
    }

    private static bool IsFunctionStart(ulong address)
    {
        void* function = IdaNative.get_func(address);
        return function != null && *(ulong*)function == address;
    }
}
