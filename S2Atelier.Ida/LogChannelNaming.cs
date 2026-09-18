using System.Runtime.InteropServices;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

public sealed record LogChannelNamingResult(bool Applicable, int Found, int Renamed);

/// <summary>
/// Names the globals DEFINE_LOGGING_CHANNEL (tier0/logging.h) creates: each one is written from the
/// LoggingChannelID_t that LoggingSystem_RegisterLoggingChannel returns for the channel name passed as
/// its first argument, and is named LOG_ plus that name, as source2utils' "Map Logging Channels" does.
/// </summary>
public static unsafe class LogChannelNaming
{
    private const ulong BadAddr = ulong.MaxValue;
    private const string Registrar = "LoggingSystem_RegisterLoggingChannel";
    private const string ChannelType = "LoggingChannelID_t";
    // tier0/logging.h.
    private const string ChannelTypeDeclaration = "typedef int LoggingChannelID_t;";
    private const int HtiDcl = 0x400;
    private const string Prefix = "LOG_";
    // The result is stored before the next call, but GCC interleaves the next registration's setup first.
    private const int MaxStoreInsns = 64;
    private const int MaxNameLength = 128;
    private const ulong MaxStubSize = 16;
    // GCC can schedule a static initializer's unrelated stores between loading the name and the call. The scan
    // stops at the first write to the register or at any call, so the window only bounds the cost.
    private const int MaxSetupInsns = 512;
    private const int SnNoCheck = 0x01;
    private const int SnForce = 0x800;
    private const int PtSilent = 0x0001;
    private const int PtVariable = 0x0008;
    private const int PtHigh = 0x0080;
    private const byte TinfoDefinite = 0x0001;
    private const uint UserTypeFlag = 0x02000000;
    private const ushort Rax = 0;

    private static readonly string[] AutoNamePrefixes = ["dword_", "unk_", "byte_", "word_", "qword_"];

    public static LogChannelNamingResult Run()
    {
        List<ulong> registrars = FindRegistrars();
        if (registrars.Count == 0)
        {
            return new LogChannelNamingResult(false, 0, 0);
        }

        int[] regs = ConVarNaming.ArgumentRegisters();
        var channels = new Dictionary<ulong, string>();
        var unresolved = new List<string>();
        foreach (ulong registrar in registrars)
        {
            foreach (ulong call in Xrefs.CodeTo(registrar))
            {
                ulong function = FunctionStart(call);
                if (function == BadAddr || registrars.Contains(function))
                {
                    continue;
                }

                string? name = NameArgument(call, function, (ushort)regs[0]) is ulong namePtr ? ReadString(namePtr) : null;
                ulong? global = StoredResult(call);
                if (string.IsNullOrEmpty(name) || global == null)
                {
                    unresolved.Add($"{call:X} ({(string.IsNullOrEmpty(name) ? "no name" : name)}, " +
                                   $"{(global == null ? "no stored result" : "stored")})");
                    continue;
                }

                channels.TryAdd(global.Value, name);
            }
        }

        if (unresolved.Count > 0)
        {
            Console.Error.WriteLine($"[log-channels] {unresolved.Count} registration(s) not resolved: " +
                                    string.Join(", ", unresolved.Take(8)));
        }

        EnsureChannelType();
        TypeImportedChannels();
        int renamed = 0;
        foreach ((ulong global, string channel) in channels)
        {
            if (Apply(global, Prefix + Normalize(channel)))
            {
                renamed++;
            }
        }

        return new LogChannelNamingResult(true, channels.Count, renamed);
    }

    // The import itself, called through directly on PE, plus the stubs that jump to it, which is how ELF
    // calls it once the PLT is resolved (--patch-plt on mold-linked binaries).
    private static List<ulong> FindRegistrars()
    {
        byte* native = Utf8.Allocate(Registrar);
        ulong import;
        try { import = IdaNative.get_name_ea(BadAddr, native); }
        finally { Utf8.Free(native); }

        if (import == BadAddr)
        {
            return [];
        }

        var found = new List<ulong> { import };
        byte* buf = stackalloc byte[Insn.BufferSize];
        foreach (ulong from in Xrefs.AllTo(import))
        {
            ulong stub = FunctionStart(from);
            if (stub != BadAddr && !found.Contains(stub) && FunctionEnd(stub) - stub <= MaxStubSize &&
                Insn.TryDecode(from, buf) && !Insn.IsCall(buf))
            {
                found.Add(stub);
            }
        }

        return found;
    }

    // The address loaded into the register for the call, through register copies: GCC often keeps the name
    // in a callee-saved register and moves it into rdi.
    private static ulong? NameArgument(ulong call, ulong function, ushort register)
    {
        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong ea = call;
        for (int i = 0; i < MaxSetupInsns; i++)
        {
            ea = IdaNative.prev_head(ea, function);
            if (ea == BadAddr || ea < function || !Insn.TryDecode(ea, buf) || Insn.IsCall(buf))
            {
                return null;
            }

            if (Insn.OpType(buf, 0) != Insn.OpReg || Insn.OpRegister(buf, 0) != register)
            {
                continue;
            }

            if (Insn.OpType(buf, 1) == Insn.OpMem && Mnemonic(ea) == "lea")
            {
                return Insn.OpAddr(buf, 1);
            }

            if (Insn.OpType(buf, 1) != Insn.OpReg || Mnemonic(ea) != "mov")
            {
                return null;
            }

            register = Insn.OpRegister(buf, 1);
        }

        return null;
    }

    private static string Mnemonic(ulong ea)
    {
        var text = new QString();
        try { return IdaNative.print_insn_mnem(&text, ea) > 0 ? text.Read() : string.Empty; }
        finally { text.Dispose(); }
    }

    // The global the returned channel id is written to, before another call or a write to rax loses it.
    private static ulong? StoredResult(ulong call)
    {
        byte* buf = stackalloc byte[Insn.BufferSize];
        ulong end = FunctionEnd(call);
        ulong ea = call;
        for (int i = 0; i < MaxStoreInsns; i++)
        {
            ea = IdaNative.next_head(ea, end);
            if (ea == BadAddr || !Insn.TryDecode(ea, buf) || Insn.IsCall(buf))
            {
                return null;
            }

            if (Insn.OpType(buf, 0) == Insn.OpMem && Insn.OpType(buf, 1) == Insn.OpReg &&
                Insn.OpRegister(buf, 1) == Rax)
            {
                return Insn.OpAddr(buf, 0);
            }

            if (Insn.OpType(buf, 0) == Insn.OpReg && Insn.OpRegister(buf, 0) == Rax && Mnemonic(ea) != "cmp" &&
                Mnemonic(ea) != "test")
            {
                return null;
            }
        }

        return null;
    }

    private static string Normalize(string channel)
    {
        var name = new System.Text.StringBuilder(Math.Min(channel.Length, MaxNameLength));
        foreach (char c in channel)
        {
            if (name.Length == MaxNameLength)
            {
                break;
            }

            name.Append(char.IsAsciiLetterOrDigit(c) ? char.ToUpperInvariant(c) : '_');
        }

        return name.ToString();
    }

    private static bool Apply(ulong global, string wanted)
    {
        string current = GetName(global);
        // SN_FORCE appends _N when another global already holds the name.
        bool ours = current == wanted || (current.StartsWith(wanted + "_", StringComparison.Ordinal) &&
                                          current[(wanted.Length + 1)..].All(char.IsAsciiDigit));
        if (!ours && current.Length > 0 && !AutoNamePrefixes.Any(x => current.StartsWith(x, StringComparison.Ordinal)))
        {
            return false;
        }

        ApplyType(global);
        if (ours)
        {
            return false;
        }

        byte* native = Utf8.Allocate(wanted);
        try { return IdaNative.set_name(global, native, SnNoCheck | SnForce) != 0; }
        finally { Utf8.Free(native); }
    }

    // Imported with tier0/logging.h when the SDK headers were; declared here otherwise.
    private static void EnsureChannelType()
    {
        byte* name = Utf8.Allocate(ChannelType);
        byte* declaration = Utf8.Allocate(ChannelTypeDeclaration);
        try
        {
            if (IdaNative.get_named_type_tid(name) == BadAddr)
            {
                IdaNative.parse_decls(IdaNative.get_idati(), declaration, null, HtiDcl);
            }
        }
        finally
        {
            Utf8.Free(name);
            Utf8.Free(declaration);
        }
    }

    // tier0 exports its own channels (DECLARE_LOGGING_CHANNEL: LOG_GENERAL, LOG_CONSOLE, ...). A PE import
    // names the IAT slot holding the variable's address; an ELF import names the variable, and its GOT
    // slot is the name with _ptr appended.
    private static void TypeImportedChannels()
    {
        var imports = new List<(ulong Ea, string Name)>();
        GCHandle handle = GCHandle.Alloc(imports);
        try
        {
            for (int module = 0, count = (int)IdaNative.get_import_module_qty(); module < count; module++)
            {
                IdaNative.enum_import_names(module, (delegate* unmanaged<ulong, byte*, ulong, void*, int>)&CollectImport,
                    (void*)GCHandle.ToIntPtr(handle));
            }
        }
        finally { handle.Free(); }

        // Only the Microsoft x64 convention passes four arguments in registers.
        bool pe = ConVarNaming.ArgumentRegisters().Length == 4;
        foreach ((ulong ea, string name) in imports)
        {
            if (!name.StartsWith(Prefix, StringComparison.Ordinal) || IdaNative.get_func(ea) != null)
            {
                continue;
            }

            ApplyType(ea, pointer: pe);
            if (!pe)
            {
                byte* slot = Utf8.Allocate(name + "_ptr");
                try
                {
                    ulong got = IdaNative.get_name_ea(BadAddr, slot);
                    if (got != BadAddr)
                    {
                        ApplyType(got, pointer: true);
                    }
                }
                finally { Utf8.Free(slot); }
            }
        }
    }

    [UnmanagedCallersOnly]
    private static int CollectImport(ulong ea, byte* name, ulong ordinal, void* state)
    {
        if (name != null && GCHandle.FromIntPtr((nint)state).Target is List<(ulong, string)> imports)
        {
            imports.Add((ea, Marshal.PtrToStringUTF8((nint)name) ?? string.Empty));
        }

        return 1;
    }

    // An explicit type, whether the user's or a previous run's, is kept.
    private static void ApplyType(ulong global, bool pointer = false)
    {
        if ((IdaNative.get_aflags(global) & UserTypeFlag) != 0)
        {
            return;
        }

        TypeInfo type = default;
        var name = new QString();
        byte* declaration = Utf8.Allocate($"{ChannelType} {(pointer ? "*" : "")}__s2_channel;");
        try
        {
            if (IdaNative.parse_decl(&type, &name, IdaNative.get_idati(), declaration, PtSilent | PtVariable | PtHigh) != 0)
            {
                IdaNative.apply_tinfo(global, &type, TinfoDefinite);
            }
        }
        finally
        {
            Utf8.Free(declaration);
            name.Dispose();
            type.Dispose();
        }
    }

    private static string ReadString(ulong ea)
    {
        if (IdaNative.is_mapped(ea) == 0)
        {
            return string.Empty;
        }

        var text = new QString();
        try { return IdaNative.get_strlit_contents(&text, ea, nuint.MaxValue, 0, null, 0) > 0 ? text.Read() : string.Empty; }
        finally { text.Dispose(); }
    }

    private static string GetName(ulong ea)
    {
        var text = new QString();
        try { return IdaNative.get_ea_name(&text, ea, 0, null) > 0 ? text.Read() : string.Empty; }
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
