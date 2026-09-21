using System.Runtime.InteropServices;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

/// <summary>Reads the layout of a type in the database: its size, members and enum values.</summary>
internal static unsafe class TypeLayout
{
    private const int MemberCount = 16;
    private const int PointedObject = 9;
    private const int MaxMembers = 16384;
    private const int ReportedPerType = 3;
    // BT_COMPLEX | BTMT_ENUM, and enum_type_data_t / edm_t as IDA 9 lays them out.
    private const byte EnumTypeCode = 0x2D;
    private const int EnumDetailsSize = 512;
    private const int EdmSize = 56, EdmComment = 24, EdmValue = 48;

    /// <summary>Member name to byte offset for a complete type; null for a missing or forward-declared one.</summary>
    internal static Dictionary<string, ulong>? Members(string typeName, out ulong size)
    {
        size = 0;
        if (!ValveImplementationTypes.Load(typeName, out TypeInfo type))
        {
            return null;
        }

        try
        {
            size = IdaNative.get_tinfo_size(null, type.Typid, 0);
            // A forward declaration answers the member count with an error value, not a count.
            nuint count = IdaNative.get_tinfo_property(type.Typid, MemberCount);
            if (size == 0 || size == ulong.MaxValue || count == 0 || count > MaxMembers)
            {
                return null;
            }

            var members = new Dictionary<string, ulong>(StringComparer.Ordinal);
            for (nuint i = 0; i < count; i++)
            {
                if (!ValveImplementationTypes.ReadMember(type.Typid, i, out IdaUdtMember member))
                {
                    continue;
                }

                // udm_t::offset is in bits.
                try { members.TryAdd(member.Name.Read(), member.Offset / 8); }
                finally { member.Dispose(); }
            }

            return members;
        }
        finally { type.Dispose(); }
    }

    /// <summary>The byte offsets of a complete type's pointer members.</summary>
    internal static List<ulong> PointerMembers(string typeName)
    {
        var offsets = new List<ulong>();
        if (!ValveImplementationTypes.Load(typeName, out TypeInfo type))
        {
            return offsets;
        }

        try
        {
            nuint count = IdaNative.get_tinfo_property(type.Typid, MemberCount);
            for (nuint i = 0; i < count && count <= MaxMembers; i++)
            {
                if (!ValveImplementationTypes.ReadMember(type.Typid, i, out IdaUdtMember member))
                {
                    continue;
                }

                try
                {
                    ulong pointed = (ulong)IdaNative.get_tinfo_property(member.Type.Typid, PointedObject);
                    if (pointed != 0 && pointed != ulong.MaxValue)
                    {
                        offsets.Add(member.Offset / 8);
                    }
                }
                finally { member.Dispose(); }
            }
        }
        finally { type.Dispose(); }

        return offsets;
    }

    /// <summary>An enum's members and their values; null when the type is no enum.</summary>
    internal static Dictionary<string, ulong>? EnumValues(string enumType)
    {
        if (!ValveImplementationTypes.Load(enumType, out TypeInfo type))
        {
            return null;
        }

        // enum_type_data_t starts with its qvector<edm_t>; an edm_t is its name, comment and value.
        byte* details = (byte*)NativeMemory.AllocZeroed(EnumDetailsSize);
        try
        {
            if (IdaNative.get_tinfo_details(type.Typid, EnumTypeCode, details) == 0)
            {
                return null;
            }

            byte* members = *(byte**)details;
            ulong count = *(ulong*)(details + 8);
            var values = new Dictionary<string, ulong>(StringComparer.Ordinal);
            for (ulong i = 0; i < count && i < MaxMembers; i++)
            {
                byte* member = members + i * EdmSize;
                values.TryAdd(((QString*)member)->Read(), *(ulong*)(member + EdmValue));
                ((QString*)member)->Dispose();
                ((QString*)(member + EdmComment))->Dispose();
            }

            if (members != null)
            {
                IdaNative.qfree(members);
            }

            return values;
        }
        finally
        {
            NativeMemory.Free(details);
            type.Dispose();
        }
    }

    /// <summary>
    /// Where the SDK's declaration of a type sdk.json also describes disagrees with the build: its size, a
    /// member's offset, or an enum value. One line per type, the first few differences.
    /// </summary>
    internal static List<string> CompareWithSchema(IEnumerable<SchemaClass> classes, IEnumerable<SchemaEnum> enums)
    {
        var report = new List<string>();
        foreach (SchemaClass schema in classes.OrderBy(x => x.Name, StringComparer.Ordinal))
        {
            if (Members(schema.Name, out ulong size) is not { } members)
            {
                continue;
            }

            var differences = new List<string>();
            if (size != (ulong)schema.Size)
            {
                differences.Add($"size SDK 0x{size:X}, build 0x{schema.Size:X}");
            }

            foreach (SchemaField field in schema.Fields.Where(x => x.Kind != SchemaFieldKind.Bitfield && x.Size >= 0))
            {
                if (members.TryGetValue(field.Name, out ulong offset) && offset != (ulong)field.Offset)
                {
                    differences.Add($"{field.Name} SDK 0x{offset:X}, build 0x{field.Offset:X}");
                }
            }

            Add(report, schema.Name, differences);
        }

        foreach (SchemaEnum schema in enums.OrderBy(x => x.Name, StringComparer.Ordinal))
        {
            if (EnumValues(schema.Name) is not { } members)
            {
                continue;
            }

            // Names the SDK lacks are left alone: it may well spell a value differently. A name both have with
            // different values is a stale declaration.
            // IDA keeps a value in the enum's width; sdk.json sign-extends a negative one.
            ulong mask = schema.Size is > 0 and < 8 ? (1UL << (schema.Size * 8)) - 1 : ulong.MaxValue;
            var differences = new List<string>();
            foreach (SchemaEnumValue value in schema.Fields)
            {
                if (members.TryGetValue(value.Name, out ulong declared) && (declared & mask) != (value.UnsignedValue & mask))
                {
                    differences.Add($"{value.Name} SDK {(long)declared}, build " +
                                    (value.IsUnsigned ? value.UnsignedValue.ToString() : value.SignedValue.ToString()));
                }
            }

            Add(report, schema.Name, differences);
        }

        return report;
    }

    private static void Add(List<string> report, string type, List<string> differences)
    {
        if (differences.Count == 0)
        {
            return;
        }

        report.Add($"{type}: {string.Join("; ", differences.Take(ReportedPerType))}" +
                   (differences.Count > ReportedPerType ? $"; {differences.Count - ReportedPerType} more" : "") + ".");
    }
}
