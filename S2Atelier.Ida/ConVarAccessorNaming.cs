using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

/// <summary>
/// Names the tier1/convar.h functions compiled into the module, from how the convar globals use them.
/// Registration and the Init call before it are known from the registrations themselves; the getters and
/// setters are recognised by their shape around the one function that looks a slot's value up; the
/// destructor is the one run at exit for each convar. A function every call of which passes convars of one
/// value type is that CConVar&lt;T&gt; instantiation; one called for several types is a folded instantiation and
/// is named after the template.
/// </summary>
internal static unsafe class ConVarAccessorNaming
{
    private const ulong BadAddr = ulong.MaxValue;
    private const int MaxCallInsns = 12;
    // A getter is Value(slot) and a fallback to the default value: a few instructions around one call.
    private const ulong MaxGetterSize = 0x60;
    private const ulong MaxLeafSize = 0x100;
    // Windows runs a convar's destructor from a small stub passed to atexit.
    private const ulong MaxStubSize = 0x30;
    // EConVarType: bool through double are passed and returned by value.
    private const int LastPrimitiveType = 8;
    private const int PtSilent = 0x0001;
    private const int PtHigh = 0x0080;
    private const uint TinfoDefinite = 0x0001;

    private sealed class Callee
    {
        internal readonly HashSet<int> Types = [];
        internal int Calls;
    }

    internal static int Run(IReadOnlyList<ConVarInfo> all, string[] typeNames, int[] regs)
    {
        var variables = all.Where(x => !x.IsCommand).ToList();
        var names = new Dictionary<ulong, Role>();

        // Registration.
        var registrars = Group(variables.Select(x => (x.Ctor, x.ValueType)));
        var inits = Group(variables.Where(x => x.InitAt != BadAddr).Select(x => (CallTarget(x.InitAt), x.ValueType)));
        foreach ((ulong init, Callee _) in inits)
        {
            names.TryAdd(init, new("ConVarRefAbstract::Init", "ConVarRefAbstract",
                $"void __fastcall f(ConVarRefAbstract *this, int ref, {TypeOr("EConVarType", "short")} type);"));
        }

        var referenceStyle = variables.GroupBy(x => x.Ctor).ToDictionary(x => x.Key, x => x.All(y => y.ReferenceStyle));
        foreach ((ulong registrar, Callee use) in registrars)
        {
            List<ulong> calls = Calls(registrar);
            bool wraps = calls.Any(x => x != registrar && registrars.ContainsKey(x));
            if (wraps && calls.Any(inits.ContainsKey))
            {
                // CConVar(name, flags, help, default): Init, the default into the value info, then Register. Its
                // third argument, the default, can look like a type, so this is decided first.
                names.TryAdd(registrar, Member("CConVar", "CConVar", use.Types, typeNames,
                    "void __fastcall f({0} *this, const char *name, {1} default_value, unsigned __int64 flags, " +
                    "const char *help, ConVarValueInfo_t *value_info);",
                    primitiveOnly: true));
            }
            else if (wraps || referenceStyle.GetValueOrDefault(registrar))
            {
                // CConVarRef(name): the object, the name and the type, registered with FCVAR_REFERENCE.
                names.TryAdd(registrar, Member("CConVarRef", "CConVarRef", use.Types, typeNames,
                    $"void __fastcall f({{0}} *this, const char *name, {TypeOr("EConVarType", "short")} type);"));
            }
            else
            {
                // Register(name, flags, help, value info), called once Init has run.
                names.TryAdd(registrar, Member("CConVarRef", "Register", use.Types, typeNames,
                    "void __fastcall f({0} *this, const char *name, unsigned __int64 flags, const char *help, " +
                    "ConVarValueInfo_t *value_info);"));
            }
        }

        // Value, and the getters and setters around it.
        var callees = CalleesOf(variables, regs[0]);
        var shapes = callees.Keys.ToDictionary(x => x, Calls);
        var values = new HashSet<ulong>();
        foreach ((ulong function, List<ulong> calls) in shapes)
        {
            if (calls.Count > 0 && shapes.TryGetValue(calls[0], out List<ulong>? first) && first.Count == 0 &&
                Size(calls[0]) <= MaxLeafSize &&
                shapes.Count(x => x.Value.Count > 0 && x.Value[0] == calls[0]) >= 2)
            {
                values.Add(calls[0]);
            }
        }

        foreach (ulong value in values)
        {
            names.TryAdd(value, new("ConVarRefAbstract::Value", "ConVarRefAbstract",
                "CVValue_t *__fastcall f(ConVarRefAbstract *this, int slot);"));
        }

        foreach ((ulong function, List<ulong> calls) in shapes)
        {
            if (values.Contains(function) || calls.Count == 0 || !values.Contains(calls[0]))
            {
                continue;
            }

            // Get is *ValueOrDefault(slot): the lookup and a load, returning the value itself for a primitive.
            // Set looks the slot up, then converts and notifies through further calls; the compiled Set
            // takes the slot before the value.
            bool getter = calls.Count == 1 && Size(function) <= MaxGetterSize;
            names.TryAdd(function, Member("CConVarRef", getter ? "Get" : "Set", callees[function].Types, typeNames,
                getter ? "{1} __fastcall f({0} *this, int slot);" : "void __fastcall f({0} *this, int slot, {1} value);",
                primitiveOnly: true));
        }

        // The destructor run at exit.
        var destructors = new Dictionary<ulong, Callee>();
        foreach (ConVarInfo cv in variables)
        {
            if (Destructor(cv, regs) is ulong destructor)
            {
                Add(destructors, destructor, cv.ValueType);
            }
        }

        foreach ((ulong destructor, Callee use) in destructors)
        {
            // IDA names cannot hold '~'; destructor slots are named dtr_ here as elsewhere.
            names.TryAdd(destructor, Member("CConVar", "dtr_CConVar", use.Types, typeNames,
                "void __fastcall f({0} *this);"));
        }

        var diagnostics = new SchemaImport.LimitedDiagnostics(16);
        int named = 0;
        foreach ((ulong function, Role role) in names)
        {
            bool typed = role.Prototype != null && SdkFunctionBinding.CanUpdateType(function)
                ? ApplyPrototype(function, role.Prototype, diagnostics)
                : TypeExists(role.ThisType) && SchemaImport.TryBindThisParameter(function, role.ThisType, diagnostics);
            if (SdkFunctionBinding.TryNameFunction(function, role.Name, typed, "convar", diagnostics.Write))
            {
                named++;
            }
        }

        diagnostics.Finish();
        return named;
    }

    // Prototype, when known exactly, declares a function f; ThisType is the class it belongs to.
    private sealed record Role(string Name, string ThisType, string? Prototype);

    /// <summary>
    /// Class::Method for the instantiation of the given value type, or of the template itself when the
    /// function serves several types. The this type is the IDA alias of the instantiation, since the legacy
    /// parser reads no template arguments. The prototype receives the this type as {0} and the value type as
    /// {1}; with primitiveOnly it only applies to the arithmetic types, which are passed and returned by value.
    /// </summary>
    private static Role Member(string template, string method, HashSet<int> types, string[] typeNames,
        string? prototype, bool primitiveOnly = false)
    {
        var known = types.Where(x => x >= 0 && x < typeNames.Length).ToList();
        if (known.Count != 1 || types.Contains(-1))
        {
            return new($"{template}::{method}", "ConVarRefAbstract",
                prototype == null || primitiveOnly ? null : string.Format(prototype, "ConVarRefAbstract", ""));
        }

        string alias = TemplateAliases.Alias($"{template}<{typeNames[known[0]]}>");
        bool primitive = known[0] <= LastPrimitiveType;
        return new($"{alias}::{method}", alias,
            prototype == null || (primitiveOnly && !primitive) ? null : string.Format(prototype, alias, typeNames[known[0]]));
    }

    private static string TypeOr(string name, string fallback) => TypeExists(name) ? name : fallback;

    private static bool ApplyPrototype(ulong function, string declaration, SchemaImport.LimitedDiagnostics diagnostics)
    {
        TypeInfo type = default;
        var name = new QString();
        byte* native = Utf8.Allocate(declaration);
        try
        {
            if (IdaNative.parse_decl(&type, &name, IdaNative.get_idati(), native, PtSilent | PtHigh) == 0)
            {
                diagnostics.Write($"[convars] function 0x{function:X}: could not parse {declaration}");
                return false;
            }

            if (IdaNative.apply_tinfo(function, &type, TinfoDefinite) == 0)
            {
                diagnostics.Write($"[convars] function 0x{function:X}: IDA rejected {declaration}");
                return false;
            }

            return true;
        }
        finally
        {
            Utf8.Free(native);
            name.Dispose();
            type.Dispose();
        }
    }

    private static Dictionary<ulong, Callee> Group(IEnumerable<(ulong Function, int Type)> uses)
    {
        var grouped = new Dictionary<ulong, Callee>();
        foreach ((ulong function, int type) in uses)
        {
            if (function != BadAddr && FunctionStart(function) == function)
            {
                Add(grouped, function, type);
            }
        }

        return grouped;
    }

    private static void Add(Dictionary<ulong, Callee> grouped, ulong function, int type)
    {
        if (!grouped.TryGetValue(function, out Callee? callee))
        {
            grouped[function] = callee = new Callee();
        }

        callee.Types.Add(type);
        callee.Calls++;
    }

    /// <summary>Functions called with a convar loaded straight into the first argument register.</summary>
    private static Dictionary<ulong, Callee> CalleesOf(List<ConVarInfo> variables, int firstArgument)
    {
        var callees = new Dictionary<ulong, Callee>();
        byte* buf = stackalloc byte[Insn.BufferSize];
        foreach (ConVarInfo cv in variables)
        {
            foreach (ulong from in Xrefs.DataTo(cv.Object))
            {
                if (from == cv.RegisteredAt || !Insn.TryDecode(from, buf) || Mnemonic(from) != "lea" ||
                    Insn.OpType(buf, 0) != Insn.OpReg || Insn.OpRegister(buf, 0) != firstArgument ||
                    Insn.OpType(buf, 1) != Insn.OpMem || Insn.OpAddr(buf, 1) != cv.Object)
                {
                    continue;
                }

                ulong end = FunctionEnd(from);
                ulong ea = from;
                for (int i = 0; i < MaxCallInsns; i++)
                {
                    ea = IdaNative.next_head(ea, end);
                    if (ea == BadAddr || !Insn.TryDecode(ea, buf))
                    {
                        break;
                    }

                    if (Insn.IsCall(buf))
                    {
                        if (Insn.OpType(buf, 0) == Insn.OpNear && FunctionStart(Insn.OpAddr(buf, 0)) == Insn.OpAddr(buf, 0))
                        {
                            Add(callees, Insn.OpAddr(buf, 0), cv.ValueType);
                        }

                        break;
                    }

                    if (Insn.OpType(buf, 0) == Insn.OpReg && Insn.OpRegister(buf, 0) == firstArgument)
                    {
                        break;
                    }
                }
            }
        }

        return callees;
    }

    /// <summary>
    /// The function atexit is given after the registration: the destructor itself on ELF (__cxa_atexit
    /// passes it the object), or on PE a stub that loads the convar and calls it.
    /// </summary>
    private static ulong? Destructor(ConVarInfo cv, int[] regs)
    {
        ulong function = FunctionStart(cv.RegisteredAt);
        if (function == BadAddr)
        {
            return null;
        }

        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong end = FunctionEnd(function);
        ulong ea = cv.RegisteredAt;
        for (int i = 0; i < 64; i++)
        {
            ea = IdaNative.next_head(ea, end);
            if (ea == BadAddr || !Insn.TryDecode(ea, buf))
            {
                return null;
            }

            bool transfer = Insn.IsCall(buf) || Mnemonic(ea) == "jmp";
            if (!transfer || Insn.OpType(buf, 0) != Insn.OpNear)
            {
                continue;
            }

            if (!Name(Insn.OpAddr(buf, 0)).Contains("atexit", StringComparison.Ordinal))
            {
                // Another registration or call first: the object has no destructor registered here.
                return null;
            }

            if (!ConVarTypeRecovery.ArgSetup(ea, function, regs[0], out ulong handler, out bool isAddress) ||
                !isAddress || FunctionStart(handler) != handler)
            {
                return null;
            }

            return Size(handler) <= MaxStubSize && StubTarget(handler, cv.Object, regs[0]) is ulong target
                ? target
                : regs.Length == 4 ? null : handler;
        }

        return null;
    }

    // The function a PE atexit stub calls with the convar.
    private static ulong? StubTarget(ulong stub, ulong obj, int firstArgument)
    {
        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong end = FunctionEnd(stub);
        bool loaded = false;
        for (ulong ea = stub; ea < end && ea != BadAddr; ea = IdaNative.next_head(ea, end))
        {
            if (!Insn.TryDecode(ea, buf))
            {
                continue;
            }

            if (Insn.OpType(buf, 0) == Insn.OpReg && Insn.OpRegister(buf, 0) == firstArgument)
            {
                loaded = Insn.OpType(buf, 1) == Insn.OpMem && Insn.OpAddr(buf, 1) == obj;
                continue;
            }

            if (Insn.IsCall(buf) && Insn.OpType(buf, 0) == Insn.OpNear)
            {
                return loaded ? Insn.OpAddr(buf, 0) : null;
            }
        }

        return null;
    }

    // Direct call targets in instruction order; a tail jump to another function counts as a call.
    private static List<ulong> Calls(ulong function)
    {
        var calls = new List<ulong>();
        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong end = FunctionEnd(function);
        for (ulong ea = function; ea < end && ea != BadAddr; ea = IdaNative.next_head(ea, end))
        {
            if (!Insn.TryDecode(ea, buf) || Insn.OpType(buf, 0) != Insn.OpNear)
            {
                continue;
            }

            ulong target = Insn.OpAddr(buf, 0);
            if (Insn.IsCall(buf) || (Mnemonic(ea) == "jmp" && FunctionStart(target) == target && target != function))
            {
                calls.Add(target);
            }
        }

        return calls;
    }

    private static bool TypeExists(string name)
    {
        byte* native = Utf8.Allocate(name);
        try { return IdaNative.get_named_type_tid(native) != BadAddr; }
        finally { Utf8.Free(native); }
    }

    private static ulong CallTarget(ulong call)
    {
        byte* buf = stackalloc byte[Insn.BufferSize];
        return Insn.TryDecode(call, buf) && Insn.OpType(buf, 0) == Insn.OpNear ? Insn.OpAddr(buf, 0) : BadAddr;
    }

    private static ulong Size(ulong function) => FunctionEnd(function) - function;

    private static string Name(ulong ea)
    {
        var text = new QString();
        try { return IdaNative.get_ea_name(&text, ea, 0, null) > 0 ? text.Read() : string.Empty; }
        finally { text.Dispose(); }
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
        return function == null ? BadAddr : *(ulong*)function;
    }

    private static ulong FunctionEnd(ulong ea)
    {
        void* function = IdaNative.get_func(ea);
        return function == null ? BadAddr : *((ulong*)function + 1);
    }
}
