using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

// SDK 9.2/9.3 x64 udt_type_data_t. create_tinfo consumes this vector.
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct IdaUdtData : IDisposable
{
    internal IdaUdtMember* Members;
    internal nuint Count, Capacity, TotalSize, UnpaddedSize;
    internal uint Alignment, Flags;
    internal byte Version, DeclaredAlignment, Pack, IsUnion;

    internal static IdaUdtData Allocate(int count)
    {
        var result = new IdaUdtData { Version = 1, Count = (nuint)count, Capacity = (nuint)count };
        if (count != 0)
        {
            result.Members = (IdaUdtMember*)IdaNative.qalloc(checked((nuint)count * (nuint)sizeof(IdaUdtMember)));
            if (result.Members == null) throw new OutOfMemoryException();
            new Span<IdaUdtMember>(result.Members, count).Clear();
        }
        return result;
    }

    public void Dispose()
    {
        for (nuint i = 0; i < Count; i++) Members[i].Dispose();
        if (Members != null) IdaNative.qfree(Members);
        this = default;
    }
}

[StructLayout(LayoutKind.Sequential)]
internal struct IdaPointerData
{
    internal TypeInfo Object, Closure, Parent;
    internal int Delta;
    internal byte Size, Flags;
}

internal sealed record SchemaVTableMetadata(string ClassName, ulong? ObjectOffset, string? ThisType,
    IReadOnlyList<string> BasePath);

internal sealed unsafe class SchemaVTableTypes(Action<string> diagnostic,
    IReadOnlyDictionary<(ulong Table, int Index), SdkResolvedSlot>? sdkSlots = null) : IVTableTypeEditor
{
    private const byte StructType = 0x0D, PointerType = 0x0A;
    private const uint Vft = 0x100, CppObject = 0x80, Fixed = 0x400;
    private const string Marker = "S2Atelier schema vtable v1 ";

    internal static void PrepareClassVptrs(SchemaSelection selection, IReadOnlySet<string> detected,
        Action<string> diagnostic)
    {
        var names = new HashSet<string>(detected, StringComparer.Ordinal);
        var queue = new Queue<string>(names);
        while (queue.TryDequeue(out string? name))
            if (selection.Classes.TryGetValue(name, out SchemaClass? schema) && schema.BaseClasses.Count > 0 &&
                names.Add(schema.BaseClasses[0])) queue.Enqueue(schema.BaseClasses[0]);
        foreach (string name in names.Order(StringComparer.Ordinal))
        {
            if (!ValveImplementationTypes.Load(name, out TypeInfo original)) continue;
            IdaUdtData data = default;
            TypeInfo modified = default;
            try
            {
                if (IdaNative.get_tinfo_details(original.Typid, StructType, &data) == 0) continue;
                bool changed = false;
                for (nuint i = 0; i < data.Count; i++)
                    if (data.Members[i].Name.Read() == "__vftable" && data.Members[i].Size == 64 &&
                        (data.Members[i].Flags & Vft) == 0)
                    {
                        data.Members[i].Flags |= Vft;
                        changed = true;
                    }
                if (!changed) continue;
                data.Flags |= CppObject;
                if (IdaNative.create_tinfo(&modified, StructType, StructType, &data) == 0 ||
                    !SameLayout(original.Typid, modified.Typid) || !Save(ref modified, name))
                    diagnostic($"[schema] {name}: could not mark the imported __vftable member.");
            }
            finally { modified.Dispose(); data.Dispose(); original.Dispose(); }
        }
    }

    internal static SchemaVTable ResolveLayout(SchemaVTable table)
    {
        if (!ValveImplementationTypes.Load(table.ClassName, out TypeInfo type))
            return table with { ObjectOffset = null, ThisType = null, BasePath = [] };
        try
        {
            if (table.ObjectOffset == 0 && IdaNative.get_tinfo_size(null, type.Typid, 0) >= 8)
                return table with { ThisType = table.ClassName, BasePath = [] };
            var paths = new List<(string Name, ulong Offset, string[] Path)>();
            ReadBases(type.Typid, 0, [], paths, new HashSet<ulong>());
            var matches = paths.Where(x => (!table.ObjectOffset.HasValue || x.Offset == table.ObjectOffset) &&
                (table.ThisType == null || x.Name == table.ThisType)).ToArray();
            if (matches.Length == 1)
                return table with { ObjectOffset = matches[0].Offset, ThisType = matches[0].Name, BasePath = matches[0].Path };
            // Several nested bases can share an offset: choose the outer subobject only if unique.
            if (table.ThisType == null && matches.Length > 1)
            {
                int depth = matches.Min(x => x.Path.Length);
                matches = matches.Where(x => x.Path.Length == depth).ToArray();
                if (matches.Length == 1)
                    return table with { ObjectOffset = matches[0].Offset, ThisType = matches[0].Name, BasePath = matches[0].Path };
            }
            return table with { ObjectOffset = null, ThisType = null, BasePath = [] };
        }
        finally { type.Dispose(); }
    }

    // A base path revisits a class only through a cycle, which a type id cannot show: every member read gives
    // its type a fresh id. Paths are bounded too, since a wide hierarchy multiplies them.
    private const int MaxBasePaths = 4096;

    private static void ReadBases(ulong type, ulong offset, string[] path,
        List<(string Name, ulong Offset, string[] Path)> output, HashSet<ulong> visiting)
    {
        if (path.Length >= 32 || output.Count >= MaxBasePaths || !visiting.Add(type)) return;
        try
        {
            // A forward declaration answers the member count with an error value, not a count.
            nuint count = IdaNative.get_tinfo_property(type, 16);
            if (count > 16384) return;
            for (ulong i = 0; i < count; i++)
            {
                if (!ValveImplementationTypes.ReadMember(type, i, out IdaUdtMember member)) continue;
                try
                {
                    if ((member.Flags & (0x20 | 0x80)) != 0x20 || member.Offset % 8 != 0) continue;
                    var name = new QString();
                    try
                    {
                        IdaNative.get_tinfo_pdata(&name, member.Type.Typid, 0);
                        string spelling = name.Read();
                        if (spelling.Length == 0 || path.Contains(spelling, StringComparer.Ordinal)) continue;
                        ulong absolute = checked(offset + member.Offset / 8);
                        string[] next = [.. path, spelling];
                        output.Add((spelling, absolute, next));
                        ReadBases(member.Type.Typid, absolute, next, output, visiting);
                    }
                    finally { name.Dispose(); }
                }
                finally { member.Dispose(); }
            }
        }
        finally { visiting.Remove(type); }
    }

    internal static bool HasBaseAt(ulong type, string name, ulong offset)
    {
        var paths = new List<(string Name, ulong Offset, string[] Path)>();
        ReadBases(type, 0, [], paths, new HashSet<ulong>());
        var matches = paths.Where(x => x.Name == name).ToArray();
        return matches.Length == 1 && matches[0].Offset == offset;
    }

    internal static string NameAt(ulong address)
    {
        var name = new QString();
        try { IdaNative.get_ea_name(&name, address, 0, null); return name.Read(); }
        finally { name.Dispose(); }
    }

    internal static uint Ordinal(string name)
    {
        byte* native = Utf8.Allocate(name);
        try { int ordinal = IdaNative.get_type_ordinal(IdaNative.get_idati(), native); return ordinal > 0 ? (uint)ordinal : 0; }
        finally { Utf8.Free(native); }
    }

    private static ulong BoundAddress(uint ordinal)
    {
        ulong address = IdaNative.get_vftable_ea(ordinal);
        // IDA 9.3 returns zero for an absent association, including after del_vftable_ea.
        return address == 0 && IdaNative.get_vftable_ordinal(0) != ordinal ? ulong.MaxValue : address;
    }

    internal static SchemaVTableMetadata? Metadata(ulong type)
    {
        var comment = new QString();
        try
        {
            IdaNative.get_tinfo_pdata(&comment, type, 6); // GTP_RPTCMT
            string value = comment.Read();
            return value.StartsWith(Marker, StringComparison.Ordinal)
                ? JsonSerializer.Deserialize<SchemaVTableMetadata>(value[Marker.Length..]) : null;
        }
        catch (JsonException) { return null; }
        finally { comment.Dispose(); }
    }

    internal static IEnumerable<(string Name, ulong Address, SchemaVTableMetadata Metadata, int Slots)> Existing(
        SchemaSelection selection)
    {
        // Iterator cannot contain pointer operations. Read the snapshot eagerly on the IDA owner thread.
        return ReadExisting(selection);
    }

    private static List<(string, ulong, SchemaVTableMetadata, int)> ReadExisting(SchemaSelection selection)
    {
        var result = new List<(string, ulong, SchemaVTableMetadata, int)>();
        void* til = IdaNative.get_idati();
        uint limit = IdaNative.get_ordinal_limit(til);
        for (uint ordinal = 1; ordinal < limit; ordinal++)
        {
            ulong address = BoundAddress(ordinal);
            if (address == ulong.MaxValue || IdaNative.is_mapped(address) == 0) continue;
            string? name = Marshal.PtrToStringUTF8((nint)IdaNative.get_numbered_type_name(til, ordinal));
            if (name == null || !ValveImplementationTypes.Load(name, out TypeInfo type)) continue;
            try
            {
                if ((IdaNative.get_tinfo_property(type.Typid, 306) & Vft) == 0) continue;
                SchemaVTableMetadata? metadata = Metadata(type.Typid);
                // Existing primary Class_vtbl types may predate our ownership metadata.
                if (metadata == null && name.EndsWith("_vtbl", StringComparison.Ordinal) &&
                    selection.Classes.ContainsKey(name[..^5]))
                    metadata = new(name[..^5], 0, name[..^5], []);
                if (metadata == null && name.EndsWith("_vtbl", StringComparison.Ordinal))
                {
                    string stem = name[..^5];
                    int separator = stem.LastIndexOf('_');
                    if (separator > 0 && selection.Classes.ContainsKey(stem[..separator]) &&
                        ulong.TryParse(stem[(separator + 1)..], System.Globalization.NumberStyles.HexNumber,
                            System.Globalization.CultureInfo.InvariantCulture, out ulong offset))
                        metadata = new(stem[..separator], offset, null, []);
                }
                if (metadata == null || !selection.Classes.ContainsKey(metadata.ClassName)) continue;
                nuint size = IdaNative.get_tinfo_size(null, type.Typid, 0);
                if (size is > 0 and <= 8192 && size % 8 == 0)
                    result.Add((name, address, metadata, (int)(size / 8)));
            }
            finally { type.Dispose(); }
        }
        return result;
    }

    public VTableTypeEditResult Complete(SchemaVTable table, string typeName)
    {
        int unknown = 0;
        TypeInfo existing = default, built = default, saved = default;
        IdaUdtData data = default;
        try
        {
            uint ordinal = Ordinal(typeName);
            uint occupied = IdaNative.get_vftable_ordinal(table.AddressPoint);
            if (occupied != 0 && occupied != ordinal)
                return Conflict($"address is bound to ordinal {occupied}");
            if (ValveImplementationTypes.Load(typeName, out existing))
            {
                SchemaVTableMetadata? metadata = Metadata(existing.Typid);
                ulong existingAddress = BoundAddress(ordinal);
                bool compatible = (table.ExistingTypeName == typeName || typeName == CanonicalName(table)) &&
                    (existingAddress == ulong.MaxValue || existingAddress == table.AddressPoint) &&
                    (IdaNative.get_tinfo_property(existing.Typid, 306) & Vft) != 0;
                if (metadata != null ? metadata.ClassName != table.ClassName : !compatible)
                    return Conflict("existing type is not owned by the schema importer");
            }

            ulong previous = ordinal == 0 ? ulong.MaxValue : BoundAddress(ordinal);
            data = IdaUdtData.Allocate(table.Functions.Count);
            data.TotalSize = data.UnpaddedSize = (nuint)table.Functions.Count * 8;
            data.Alignment = 8;
            data.Flags = Vft | Fixed;
            var memberNames = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < table.Functions.Count; i++)
            {
                ulong target = table.Functions[i];
                ref IdaUdtMember member = ref data.Members[i];
                member.Offset = (ulong)i * 64;
                member.Size = 64;
                SdkResolvedSlot? sdk = sdkSlots?.GetValueOrDefault((table.AddressPoint, i));
                string wanted = sdk?.Name ?? VTableTypeBinder.SlotName(NameAt(target), i);
                member.Name = String(SdkVTableMatching.UniqueName(wanted, i, memberNames));
                TypeInfo function = default;
                bool known = false;
                try
                {
                    if (sdk != null && sdk.TryGetFunction(out function))
                        known = Pointer(ref function, out member.Type);
                    if (!known && target != 0 && IdaNative.is_mapped(target) != 0 &&
                        ReadFunction(target, ref function))
                        known = Pointer(ref function, out member.Type);
                    if (!known)
                    {
                        member.Type.Dispose();
                        TypeInfo voidType = new() { Typid = 1 }; // BT_VOID
                        if (!Pointer(ref voidType, out member.Type)) throw new InvalidOperationException("cannot create slot pointer");
                        unknown++;
                    }
                }
                finally { function.Dispose(); }
                member.Comment = String($"slot {i}; target 0x{target:X}; " +
                    (sdk != null ? $"HL2SDK {sdk.SourceClass}: {sdk.Header}" : known ? "IDA prototype" : "unknown prototype"));
            }
            if (IdaNative.create_tinfo(&built, StructType, StructType, &data) == 0)
                throw new InvalidOperationException("cannot create VFT UDT");
            // Add ownership before publishing. Type updates replace complete definitions, never append slots.
            string metadataJson = JsonSerializer.Serialize(new SchemaVTableMetadata(table.ClassName,
                table.ObjectOffset, table.ThisType, table.BasePath));
            byte* comment = Utf8.Allocate(Marker + metadataJson);
            try
            {
                if (IdaNative.set_tinfo_property4(&built, 5, (nuint)comment, 0, 0, 0) != 0)
                    throw new InvalidOperationException("cannot mark VFT ownership");
            }
            finally { Utf8.Free(comment); }
            if (!Save(ref built, typeName) || !ValveImplementationTypes.Load(typeName, out saved))
                throw new InvalidOperationException("cannot save VFT type");
            ordinal = Ordinal(typeName);
            if (ordinal == 0) throw new InvalidOperationException("VFT type has no ordinal");
            if (IdaNative.apply_tinfo(table.AddressPoint, &saved, 1) == 0)
            {
                diagnostic($"[schema] {typeName}: cannot apply type at 0x{table.AddressPoint:X}.");
                return new(true, false, unknown);
            }
            byte setResult = IdaNative.set_vftable_ea(ordinal, table.AddressPoint);
            ulong actualAddress = IdaNative.get_vftable_ea(ordinal);
            uint actualOrdinal = IdaNative.get_vftable_ordinal(table.AddressPoint);
            bool bound = actualAddress == table.AddressPoint && actualOrdinal == ordinal;
            if (!bound)
            {
                IdaNative.set_vftable_ea(ordinal, previous);
                diagnostic($"[schema] {typeName}: VFT address binding failed verification " +
                    $"(set={setResult}, ordinal={ordinal}, address=0x{actualAddress:X}, reverse={actualOrdinal}).");
            }
            if (table.ObjectOffset == null)
                diagnostic($"[schema] {typeName}: unknown subobject offset; class vptr unchanged.");
            else if (typeName != CanonicalName(table))
                diagnostic($"[schema] {typeName}: alternate table instance; class vptr uses {CanonicalName(table)}.");
            else if (!AssociateClass(table, ref saved, out string reason))
                diagnostic($"[schema] {typeName}: class vptr association skipped: {reason}.");
            return new(true, bound, unknown);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or OverflowException)
        {
            diagnostic($"[schema] {typeName} at 0x{table.AddressPoint:X}: {ex.Message}.");
            return new(false, false, unknown);
        }
        finally { data.Dispose(); saved.Dispose(); built.Dispose(); existing.Dispose(); }

        VTableTypeEditResult Conflict(string reason)
        {
            diagnostic($"[schema] {typeName} at 0x{table.AddressPoint:X}: conflict: {reason}; preserved.");
            return new(false, false, 0, true);
        }
    }

    private static bool ReadFunction(ulong address, ref TypeInfo type)
    {
        fixed (TypeInfo* native = &type)
        {
            if (IdaNative.get_tinfo(native, address) != 0 &&
                (IdaNative.get_tinfo_property(type.Typid, 2) & 0x0F) == 0x0C) return true;
            type.Dispose();
            return IdaNative.guess_tinfo(native, address) != 0 &&
                (IdaNative.get_tinfo_property(type.Typid, 2) & 0x0F) == 0x0C;
        }
    }

    private static string CanonicalName(SchemaVTable table) => table.ObjectOffset == 0
        ? table.ClassName + "_vtbl" : $"{table.ClassName}_{table.ObjectOffset:X4}_vtbl";

    internal static QString String(string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value + '\0');
        byte* buffer = (byte*)IdaNative.qalloc((nuint)bytes.Length);
        if (buffer == null) throw new OutOfMemoryException();
        bytes.CopyTo(new Span<byte>(buffer, bytes.Length));
        return new QString { Array = buffer, Length = (nuint)bytes.Length, Capacity = (nuint)bytes.Length };
    }

    internal static bool Pointer(ref TypeInfo target, out TypeInfo pointer)
    {
        pointer = default;
        var data = new IdaPointerData();
        fixed (TypeInfo* source = &target) IdaNative.copy_tinfo_t(&data.Object, source);
        try
        {
            fixed (TypeInfo* output = &pointer) return IdaNative.create_tinfo(output, PointerType, PointerType, &data) != 0;
        }
        finally { data.Object.Dispose(); data.Closure.Dispose(); data.Parent.Dispose(); }
    }

    internal static bool Save(ref TypeInfo type, string name)
    {
        byte* native = Utf8.Allocate(name);
        try { fixed (TypeInfo* pointer = &type) return IdaNative.save_tinfo(pointer, IdaNative.get_idati(), 0, native, 1 | 4) == 0; }
        finally { Utf8.Free(native); }
    }

    private static bool AssociateClass(SchemaVTable table, ref TypeInfo vtable, out string reason)
    {
        reason = "class type unavailable";
        if (!ValveImplementationTypes.Load(table.ClassName, out TypeInfo original)) return false;
        IdaUdtData source = default, replacement = default;
        TypeInfo modified = default;
        try
        {
            if (IdaNative.get_tinfo_details(original.Typid, StructType, &source) == 0) return false;
            ulong offset = checked(table.ObjectOffset!.Value * 8);
            if (offset / 8 + 8 > source.TotalSize) { reason = "vptr lies outside class"; return false; }
            int replace = -1;
            bool coveredByBase = false;
            for (int i = 0; i < (int)source.Count; i++)
            {
                ref IdaUdtMember member = ref source.Members[i];
                if ((member.Flags & 0x20) != 0 && member.Offset <= offset && offset + 64 <= member.Offset + member.Size)
                    coveredByBase = true;
                else if (member.Offset == offset && member.Size == 64 &&
                    ((member.Flags & Vft) != 0 || member.Name.Read() == "__vftable")) replace = i;
                else if ((member.Flags & 0x20) == 0 && member.Offset < offset + 64 && offset < member.Offset + member.Size)
                { reason = "vptr overlaps a schema field"; return false; }
            }
            if (replace < 0 && !coveredByBase) { reason = "no vptr or base subobject at offset"; return false; }
            if (replace < 0 && coveredByBase)
            {
                // IDA synthesizes inherited vptrs by looking up Class[_%04X]_vtbl.
                // An overlapping physical member is invalid and must never be inserted.
                return VerifyVptr(original.Typid, offset, vtable.Typid, out reason);
            }
            replacement = IdaUdtData.Allocate((int)source.Count);
            replacement.TotalSize = source.TotalSize;
            replacement.UnpaddedSize = source.UnpaddedSize;
            replacement.Alignment = source.Alignment;
            replacement.DeclaredAlignment = source.DeclaredAlignment;
            replacement.Pack = source.Pack;
            replacement.Flags = source.Flags | CppObject | Fixed;
            for (int i = 0; i < (int)source.Count; i++)
            {
                replacement.Members[i] = source.Members[i];
                source.Members[i] = default; // transfer ownership
            }
            ref IdaUdtMember vptr = ref replacement.Members[replace];
            vptr.Dispose();
            vptr = default;
            vptr.Offset = offset;
            vptr.Size = 64;
            vptr.Flags = Vft;
            vptr.Name = String(offset == 0 ? "__vftable" : $"__vftable_{table.ObjectOffset:X}");
            if (!Pointer(ref vtable, out vptr.Type)) { reason = "cannot create vptr"; return false; }
            if (IdaNative.create_tinfo(&modified, StructType, StructType, &replacement) == 0)
            { reason = "IDA rejected vptr type"; return false; }
            if (!SameLayout(original.Typid, modified.Typid)) { reason = "layout validation failed"; return false; }
            if (!Save(ref modified, table.ClassName)) { reason = "cannot save class"; return false; }
            return VerifyVptr(modified.Typid, offset, vtable.Typid, out reason);
        }
        finally { modified.Dispose(); replacement.Dispose(); source.Dispose(); original.Dispose(); }
    }

    private static bool VerifyVptr(ulong type, ulong offset, ulong tableType, out string reason)
    {
        reason = "IDA did not resolve the class-specific vptr";
        if (!ValveImplementationTypes.ReadMember(type, offset, out IdaUdtMember member, true)) return false;
        try
        {
            ulong pointed = (ulong)IdaNative.get_tinfo_property(member.Type.Typid, 9);
            if (member.Offset != offset || member.Size != 64 || (member.Flags & Vft) == 0 ||
                IdaNative.compare_tinfo(pointed, tableType, 0) == 0) return false;
            reason = "";
            return true;
        }
        finally { member.Dispose(); }
    }

    internal static bool SameLayout(ulong original, ulong modified)
    {
        if (IdaNative.get_tinfo_size(null, original, 0) != IdaNative.get_tinfo_size(null, modified, 0)) return false;
        nuint count = IdaNative.get_tinfo_property(original, 16);
        nuint newCount = IdaNative.get_tinfo_property(modified, 16);
        if (count > 16384 || newCount > 16384) return false;
        for (ulong i = 0; i < count; i++)
        {
            if (!ValveImplementationTypes.ReadMember(original, i, out IdaUdtMember before)) return false;
            try
            {
                if ((before.Flags & Vft) != 0 || before.Name.Read() == "__vftable") continue;
                bool found = false;
                for (ulong j = 0; j < newCount; j++)
                {
                    if (!ValveImplementationTypes.ReadMember(modified, j, out IdaUdtMember after)) return false;
                    try
                    {
                        if (before.Offset == after.Offset && before.Size == after.Size && before.Flags == after.Flags &&
                            before.Name.Read() == after.Name.Read() && IdaNative.compare_tinfo(before.Type.Typid, after.Type.Typid, 0) != 0)
                        { found = true; break; }
                    }
                    finally { after.Dispose(); }
                }
                if (!found) return false;
            }
            finally { before.Dispose(); }
        }
        return true;
    }
}
