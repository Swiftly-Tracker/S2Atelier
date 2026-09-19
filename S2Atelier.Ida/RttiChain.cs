using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

/// <summary>
/// The primary base chain RTTI records for a class: the class, its first base, that base's first base and
/// so on, as long as each shares the object's start (a non-virtual base at offset 0). Those classes all use
/// the class's primary vtable, so their declared methods are its first slots. Names come out the way the
/// vtable symbols name the classes.
/// </summary>
internal static unsafe class RttiChain
{
    private const int MaxDepth = 64;

    /// <summary>The chain for the primary vtable whose address point is given; empty when RTTI is unreadable.</summary>
    internal static IReadOnlyList<string> Read(ulong addressPoint, VTableAbi abi)
        => abi == VTableAbi.Msvc ? ReadMsvc(addressPoint) : ReadItanium(addressPoint);

    // RTTICompleteObjectLocator (before the address point) -> RTTIClassHierarchyDescriptor -> base class array.
    // The array lists bases in pre-order, so while an entry has bases, the next entry is its first base.
    private static List<string> ReadMsvc(ulong addressPoint)
    {
        var chain = new List<string>();
        if (addressPoint < 8 || !Mapped(addressPoint - 8, 8))
        {
            return chain;
        }

        ulong locator = IdaNative.get_qword(addressPoint - 8);
        if (!Mapped(locator, 24) || IdaNative.get_dword(locator) != 1 || IdaNative.get_dword(locator + 4) != 0)
        {
            return chain;
        }

        uint self = IdaNative.get_dword(locator + 20);
        if (self > locator)
        {
            return chain;
        }

        ulong image = locator - self;
        ulong hierarchy = image + IdaNative.get_dword(locator + 16);
        if (!Mapped(hierarchy, 16))
        {
            return chain;
        }

        uint count = IdaNative.get_dword(hierarchy + 8);
        ulong array = image + IdaNative.get_dword(hierarchy + 12);
        for (uint i = 0; i < count && i < MaxDepth && Mapped(array + i * 4, 4); i++)
        {
            ulong descriptor = image + IdaNative.get_dword(array + i * 4);
            if (!Mapped(descriptor, 24))
            {
                break;
            }

            // PMD: mdisp, pdisp (-1 for a non-virtual base), vdisp.
            if (i > 0 && (IdaNative.get_dword(descriptor + 8) != 0 || IdaNative.get_dword(descriptor + 12) != uint.MaxValue))
            {
                break;
            }

            if (MsvcName(image + IdaNative.get_dword(descriptor)) is not string name)
            {
                break;
            }

            chain.Add(name);
            if (IdaNative.get_dword(descriptor + 4) == 0)
            {
                break;
            }
        }

        return chain;
    }

    // TypeDescriptor: vftable pointer, spare, then the decorated name ".?AVName@@".
    private static string? MsvcName(ulong typeDescriptor)
    {
        string decorated = ReadString(typeDescriptor + 16);
        if (decorated.Length < 5 || !decorated.StartsWith(".?A", StringComparison.Ordinal))
        {
            return null;
        }

        string raw = "??_7" + decorated[4..] + "6B@";
        return VTableAnalysis.TryDescribe(0, raw, Demangle(raw), out VTableDescriptor described) ? described.ClassName : null;
    }

    // The typeinfo pointer precedes the address point. Its own vtable tells the kind: __class_type_info has no
    // base, __si_class_type_info one at offset 0, __vmi_class_type_info a list with offsets and flags.
    private static List<string> ReadItanium(ulong addressPoint)
    {
        var chain = new List<string>();
        if (addressPoint < 8 || !Mapped(addressPoint - 8, 8))
        {
            return chain;
        }

        ulong typeInfo = IdaNative.get_qword(addressPoint - 8);
        for (int depth = 0; depth < MaxDepth && typeInfo != 0 && Mapped(typeInfo, 16); depth++)
        {
            if (ItaniumName(IdaNative.get_qword(typeInfo + 8)) is not string name)
            {
                break;
            }

            chain.Add(name);
            string kind = TypeInfoKind(IdaNative.get_qword(typeInfo));
            if (kind.Contains("__si_class_type_info", StringComparison.Ordinal) && Mapped(typeInfo + 16, 8))
            {
                typeInfo = IdaNative.get_qword(typeInfo + 16);
            }
            else if (kind.Contains("__vmi_class_type_info", StringComparison.Ordinal) && Mapped(typeInfo + 16, 24) &&
                     IdaNative.get_dword(typeInfo + 20) > 0)
            {
                // base_info[0]: the base's typeinfo, then offset << 8 | flags (1 virtual, 2 public).
                ulong offsetFlags = IdaNative.get_qword(typeInfo + 32);
                if ((offsetFlags & 1) != 0 || offsetFlags >> 8 != 0)
                {
                    break;
                }

                typeInfo = IdaNative.get_qword(typeInfo + 24);
            }
            else
            {
                break;
            }
        }

        return chain;
    }

    // The mangled name the typeinfo points at, "14CSource2Server", read as the vtable symbol would spell it.
    private static string? ItaniumName(ulong mangled)
    {
        // libstdc++ prefixes the names of internal-linkage classes with '*'.
        string text = Mapped(mangled, 1) ? ReadString(mangled).TrimStart('*') : string.Empty;
        if (text.Length == 0)
        {
            return null;
        }

        string raw = "_ZTV" + text;
        return VTableAnalysis.TryDescribe(0, raw, Demangle(raw), out VTableDescriptor described) ? described.ClassName : null;
    }

    // The vtable a typeinfo object points into: its address point is 16 bytes past the vtable symbol,
    // whether the symbol is defined here or imported.
    private static string TypeInfoKind(ulong vtablePoint)
    {
        string name = vtablePoint >= 16 ? Name(vtablePoint - 16) : string.Empty;
        return name.Length > 0 ? name : Name(vtablePoint);
    }

    private static bool Mapped(ulong ea, ulong size) => ea != 0 && IdaNative.is_mapped(ea) != 0 && IdaNative.is_mapped(ea + size - 1) != 0;

    private static string ReadString(ulong ea)
    {
        var text = new QString();
        try { return IdaNative.get_strlit_contents(&text, ea, nuint.MaxValue, 0, null, 0) > 0 ? text.Read() : ReadAscii(ea); }
        finally { text.Dispose(); }
    }

    // RTTI names are not always string literals to IDA; read the bytes directly.
    private static string ReadAscii(ulong ea)
    {
        var chars = new List<char>();
        for (ulong i = 0; i < 1024 && IdaNative.is_mapped(ea + i) != 0; i++)
        {
            byte value = IdaNative.get_byte(ea + i);
            if (value == 0)
            {
                break;
            }

            chars.Add((char)value);
        }

        return new string([.. chars]);
    }

    private static string Name(ulong ea)
    {
        var text = new QString();
        try { return IdaNative.get_ea_name(&text, ea, 0, null) > 0 ? text.Read() : string.Empty; }
        finally { text.Dispose(); }
    }

    private static string? Demangle(string raw)
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
