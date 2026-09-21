using System.Text.RegularExpressions;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

internal sealed record LibraryMisnamesResult(int Checked, int Dropped);

/// <summary>
/// FLIRT names a function by its bytes, and a short one matches library code it is not: an accessor
/// (mov [rcx+28h], rdx; retn) reads as std::swfun, an AddRef as Concurrency's _RefCounter::_Reference. Where
/// the linker folded identical code the library function does share the address, but the module uses it as the
/// method its vtables hold. A library name held by the vtables of classes outside its own scope is therefore
/// dropped before the vtable passes run, so they name and type the function as the method; the comment keeps
/// what FLIRT called it.
/// </summary>
internal static unsafe class LibraryMisnames
{
    private const ulong FunctionLibrary = 0x04; // FUNC_LIB
    private const string CommentPrefix = "FLIRT matched ";

    internal static LibraryMisnamesResult Run(SchemaTargetPlatform platform, Action<string> diagnostic)
    {
        var abi = platform == SchemaTargetPlatform.WindowsMsvc ? VTableAbi.Msvc : VTableAbi.Itanium;
        var holders = new Dictionary<ulong, HashSet<string>>();
        var chains = new Dictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
        foreach ((ulong addressPoint, IReadOnlyList<ulong> functions) in new SdkInterfaceBinding.Image().AllTables(platform))
        {
            if (RttiChain.Owner(addressPoint, abi) is not (string owner, ulong offset))
            {
                continue;
            }

            if (offset == 0 && !chains.ContainsKey(owner) && RttiChain.Read(addressPoint, abi) is { Count: > 0 } chain)
            {
                chains[owner] = chain;
            }

            foreach (ulong function in functions)
            {
                if (IsLibrary(function))
                {
                    if (!holders.TryGetValue(function, out var classes))
                    {
                        holders.Add(function, classes = new HashSet<string>(StringComparer.Ordinal));
                    }

                    classes.Add(owner);
                }
            }
        }

        int dropped = 0;
        foreach ((ulong function, var classes) in holders.OrderBy(x => x.Key))
        {
            string name = SchemaVTableTypes.NameAt(function);
            if (!IsMangled(name) || Belongs(Demangle(name), classes, chains))
            {
                continue;
            }

            if (Drop(function, name))
            {
                dropped++;
            }
            else
            {
                diagnostic($"[library-misnames] 0x{function:X}: could not drop {name}.");
            }
        }

        return new(holders.Count, dropped);
    }

    /// <summary>A symbol's mangled name, whose demangled form carries the function's real prototype.</summary>
    internal static bool IsMangled(string name)
        => name.StartsWith('?') || name.StartsWith("_Z", StringComparison.Ordinal);

    /// <summary>A function IDA named by its bytes (FLIRT) rather than by a symbol.</summary>
    internal static bool IsLibrary(ulong address)
    {
        void* function = IdaNative.get_func(address);
        return function != null && *(ulong*)function == address && (*((ulong*)function + 2) & FunctionLibrary) != 0;
    }

    /// <summary>
    /// Whether the library name fits the classes whose vtables hold the function: a library class's own table,
    /// or a method of one of the holders or their bases. A free function in a vtable never fits.
    /// </summary>
    internal static bool Belongs(string? demangled, IEnumerable<string> holders,
        IReadOnlyDictionary<string, IReadOnlyList<string>> chains)
    {
        if (demangled == null)
        {
            return true;
        }

        foreach (string holder in holders)
        {
            if (holder.StartsWith("std::", StringComparison.Ordinal) ||
                holder.StartsWith("Concurrency::", StringComparison.Ordinal))
            {
                return true;
            }

            foreach (string scope in chains.GetValueOrDefault(holder) ?? [holder])
            {
                if (Regex.IsMatch(demangled, $@"(?<![A-Za-z0-9_:]){Regex.Escape(scope)}::"))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool Drop(ulong address, string name)
    {
        // An empty name gives the function back its automatic one, which the vtable passes replace.
        byte* empty = Utf8.Allocate("");
        try
        {
            if (IdaNative.set_name(address, empty, 0) == 0)
            {
                return false;
            }
        }
        finally { Utf8.Free(empty); }

        void* function = IdaNative.get_func(address);
        *((ulong*)function + 2) &= ~FunctionLibrary;
        IdaNative.update_func(function);

        var existing = new QString();
        string comment;
        try
        {
            IdaNative.get_cmt(&existing, address, 1);
            comment = existing.Read();
        }
        finally { existing.Dispose(); }

        string line = CommentPrefix + name;
        if (!comment.Contains(line, StringComparison.Ordinal))
        {
            comment = comment.Length == 0 ? line : comment + "\n" + line;
            byte* text = Utf8.Allocate(comment);
            try { IdaNative.set_cmt(address, text, 1); }
            finally { Utf8.Free(text); }
        }

        return true;
    }

    // IDA suffixes a repeated name (_0, _1, ...), which the demangler does not take.
    private static string? Demangle(string name)
        => DemangleRaw(name) ?? (Regex.Match(name, @"^(.*)_\d+$") is { Success: true } m ? DemangleRaw(m.Groups[1].Value) : null);

    private static string? DemangleRaw(string raw)
    {
        byte* native = Utf8.Allocate(raw);
        var output = new QString();
        try { return IdaNative.demangle_name(&output, native, 0, 2) > 0 ? output.Read() : null; }
        finally
        {
            output.Dispose();
            Utf8.Free(native);
        }
    }
}
