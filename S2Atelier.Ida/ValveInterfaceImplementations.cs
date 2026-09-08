using System.Runtime.InteropServices;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

internal sealed record ValveInterfaceImplementation(
    string Version, string InterfaceClass, string ClassName, string Header);

internal static class ValveInterfaceImplementations
{
    internal static readonly IReadOnlyList<ValveInterfaceImplementation> Known =
    [
        new("VEngineCvar007", "ICvar", "CCvar", "public/icvar.h"),
        new("SchemaSystem_001", "ISchemaSystem", "CSchemaSystem", "public/schemasystem/schemasystem.h"),
        new("HostStateMgr001", "IHostStateMgr", "CHostStateMgr", "public/engine/hoststate.h"),
    ];

    internal static ValveInterfaceImplementation? Find(ValveInterfaceDefinition definition)
        => Known.SingleOrDefault(x => x.Version == definition.Version &&
            x.InterfaceClass == definition.ClassName &&
            x.Header.Equals(definition.DefinitionHeader, StringComparison.OrdinalIgnoreCase));

    internal static bool SlotNameMatches(ValveInterfaceImplementation mapping, string expected, string actual)
        => expected == actual || (expected == "dtr_" + mapping.InterfaceClass && actual == "dtr_" + mapping.ClassName);
}

// IDA SDK 9.2/9.3 x64 udm_t. value_repr_t has no owned allocations.
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct IdaUdtMember : IDisposable
{
    internal ulong Offset;
    internal ulong Size;
    internal QString Name;
    internal QString Comment;
    internal TypeInfo Type;
    private fixed byte _representation[56];
    private int _effectiveAlignment;
    internal uint Flags;
    private byte _fieldAlignment;

    public void Dispose()
    {
        Name.Dispose();
        Comment.Dispose();
        Type.Dispose();
    }
}

internal static unsafe class ValveImplementationTypes
{
    private const int MemberByIndex = 1;
    private const int MemberVftable = 0x10000000;
    private const uint BaseClass = 0x20;
    private const uint VirtualBase = 0x80;
    private const uint VftableMember = 0x100;
    private const int MemberCount = 16;
    private const int ForwardType = 5;
    private const int PointedObject = 9;
    private const int UdtBits = 306;

    internal static bool Load(string name, out TypeInfo type)
    {
        type = default;
        byte* native = Utf8.Allocate(name);
        try
        {
            ulong tid = IdaNative.get_named_type_tid(native);
            fixed (TypeInfo* target = &type)
                return tid != ulong.MaxValue && IdaNative.get_type_by_tid(target, tid) != 0;
        }
        finally { Utf8.Free(native); }
    }

    internal static bool ReadMember(ulong type, ulong index, out IdaUdtMember member, bool vftable = false)
    {
        member = default;
        member.Offset = index;
        fixed (IdaUdtMember* target = &member)
            return IdaNative.find_tinfo_udt_member(target, type, vftable ? MemberVftable : MemberByIndex) >= 0;
    }

    internal static bool Validate(ValveInterfaceImplementation mapping, out string reason)
    {
        TypeInfo implementation = default, iface = default, vtable = default, interfaceVtable = default;
        try
        {
            if (!Load(mapping.ClassName, out implementation) || !Load(mapping.InterfaceClass, out iface) ||
                !Load(mapping.ClassName + "_vtbl", out vtable) ||
                !Load(mapping.InterfaceClass + "_vtbl", out interfaceVtable))
            {
                reason = "implementation or vtable definition is missing";
                return false;
            }
            if (!Complete(implementation.Typid) || !Complete(iface.Typid) ||
                !ValidVtable(vtable.Typid) || !ValidVtable(interfaceVtable.Typid))
            {
                reason = "incomplete class or non-VFT vtable";
                return false;
            }
            if (!HasPrimaryBase(implementation.Typid, iface.Typid, 0))
            {
                reason = "interface is not a non-virtual base at offset zero";
                return false;
            }
            ReadMember(implementation.Typid, 0, out IdaUdtMember pointer, vftable: true);
            try
            {
                ulong pointed = (ulong)IdaNative.get_tinfo_property(pointer.Type.Typid, PointedObject);
                if (pointer.Offset != 0 || pointer.Size != 64 || (pointer.Flags & VftableMember) == 0 ||
                    pointed == 0 || IdaNative.compare_tinfo(pointed, vtable.Typid, 0) == 0)
                {
                    reason = "primary vftable pointer does not reference the implementation vtable";
                    return false;
                }
            }
            finally { pointer.Dispose(); }

            nuint slots = IdaNative.get_tinfo_property(interfaceVtable.Typid, MemberCount);
            if (IdaNative.get_tinfo_property(vtable.Typid, MemberCount) < slots)
            {
                reason = "implementation vtable is missing interface slots";
                return false;
            }
            for (ulong i = 0; i < slots; i++)
            {
                ReadMember(interfaceVtable.Typid, i, out IdaUdtMember expected);
                ReadMember(vtable.Typid, i, out IdaUdtMember actual);
                try
                {
                    if (actual.Offset != expected.Offset || actual.Size != 64 || expected.Size != 64 ||
                        !ValveInterfaceImplementations.SlotNameMatches(mapping, expected.Name.Read(), actual.Name.Read()) ||
                        !SameSignature(expected.Type.Typid, actual.Type.Typid))
                    {
                        reason = $"implementation vtable slot {i} does not match '{expected.Name.Read()}'";
                        return false;
                    }
                }
                finally { actual.Dispose(); expected.Dispose(); }
            }
            reason = string.Empty;
            return true;
        }
        finally
        {
            interfaceVtable.Dispose(); vtable.Dispose(); iface.Dispose(); implementation.Dispose();
        }
    }

    private static bool Complete(ulong type)
        => IdaNative.get_tinfo_property(type, ForwardType) == 0 &&
           IdaNative.get_tinfo_property(type, MemberCount) != 0;

    private static bool ValidVtable(ulong type)
        => Complete(type) && (IdaNative.get_tinfo_property(type, UdtBits) & 0x100) != 0;

    private static bool HasPrimaryBase(ulong type, ulong iface, int depth)
    {
        if (IdaNative.compare_tinfo(type, iface, 0) != 0) return true;
        if (depth >= 16) return false;
        nuint count = IdaNative.get_tinfo_property(type, MemberCount);
        for (ulong i = 0; i < count; i++)
        {
            ReadMember(type, i, out IdaUdtMember member);
            try
            {
                if (member.Offset == 0 && (member.Flags & (BaseClass | VirtualBase)) == BaseClass &&
                    HasPrimaryBase(member.Type.Typid, iface, depth + 1)) return true;
            }
            finally { member.Dispose(); }
        }
        return false;
    }

    private static bool SameSignature(ulong expectedPointer, ulong actualPointer)
    {
        if (IdaNative.get_tinfo_property(expectedPointer, 6) == 0 ||
            IdaNative.get_tinfo_property(actualPointer, 6) == 0) return false; // GTA_IS_FUNCPTR
        ulong expected = (ulong)IdaNative.get_tinfo_property(expectedPointer, PointedObject);
        ulong actual = (ulong)IdaNative.get_tinfo_property(actualPointer, PointedObject);
        nuint arguments = IdaNative.get_tinfo_property(expected, 23); // GTA_FUNC_NARGS
        if (arguments == 0 || arguments != IdaNative.get_tinfo_property(actual, 23) ||
            IdaNative.get_tinfo_property(expected, 20) != IdaNative.get_tinfo_property(actual, 20) ||
            IdaNative.compare_tinfo((ulong)IdaNative.get_tinfo_property(expected, 24),
                (ulong)IdaNative.get_tinfo_property(actual, 24), 0) == 0) return false;
        // A derived override may specialize only the hidden this argument.
        for (int i = 1; (nuint)i < arguments; i++)
            if (IdaNative.compare_tinfo((ulong)IdaNative.get_tinfo_property(expected, 25 + i),
                (ulong)IdaNative.get_tinfo_property(actual, 25 + i), 0) == 0) return false;
        return true;
    }
}
