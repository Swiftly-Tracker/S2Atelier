using System.Runtime.InteropServices;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

// Serialized with named references so neither the snapshot nor its dependencies retain a temporary TIL.
internal sealed record SdkPortableType(byte[] Type, byte[] Fields, byte[] Comments)
{
    internal static unsafe SdkPortableType? Capture(ref TypeInfo type, void* til)
    {
        TypeInfo copy = default;
        QString bytes = default, fields = default, comments = default;
        try
        {
            fixed (TypeInfo* source = &type) IdaNative.copy_tinfo_t(&copy, source);
            if (IdaNative.detach_tinfo_t(&copy) == 0 || IdaNative.replace_ordinal_typerefs(til, &copy) < 0 ||
                IdaNative.serialize_tinfo(&bytes, &fields, &comments, &copy, 0x10 | 0x200) == 0) return null;
            return new(Copy(bytes), Copy(fields), Copy(comments));
        }
        finally { copy.Dispose(); bytes.Dispose(); fields.Dispose(); comments.Dispose(); }
        static byte[] Copy(QString value) => value.Array == null ? [0] :
            new ReadOnlySpan<byte>(value.Array, checked((int)value.Length)).ToArray();
    }

    internal unsafe bool Restore(out TypeInfo result, void* til = null)
    {
        result = default;
        bool success;
        fixed (byte* t = Type, f = Fields, c = Comments)
        fixed (TypeInfo* output = &result)
        {
            byte* tp = t, fp = f, cp = c;
            success = IdaNative.deserialize_tinfo(output, til == null ? IdaNative.get_idati() : til,
                &tp, &fp, &cp, null) != 0;
        }
        if (!success) result.Dispose();
        return success;
    }
}

internal sealed record SdkSlotDefinition(string Name, SdkPortableType Prototype);
internal sealed record SdkVTableDefinition(string ClassName, ulong Offset, string Header,
    IReadOnlyList<SdkSlotDefinition> Slots);
internal sealed record SdkResolvedSlot(string Name, string SourceClass, string Header, SdkPortableType Prototype)
{
    internal unsafe bool TryGetFunction(out TypeInfo function) => Prototype.Restore(out function);
}

internal static class SdkVTableMatching
{
    internal static IEnumerable<(string Class, ulong Offset)> Candidates(SchemaVTable table,
        IReadOnlyDictionary<string, SchemaClass> classes)
    {
        if (table.ObjectOffset == null || table.ThisType == null) yield break;
        yield return (table.ClassName, table.ObjectOffset.Value);
        string current = table.ObjectOffset == 0 ? table.ClassName : table.ThisType;
        var visited = new HashSet<string>(StringComparer.Ordinal);
        while (visited.Add(current))
        {
            if (current != table.ClassName) yield return (current, 0);
            // Only the first nonvirtual base shares the primary address point. Other bases
            // require a resolved secondary subobject, never concatenated primary slot counts.
            if (!classes.TryGetValue(current, out var schema) || schema.BaseClasses.Count == 0) break;
            current = schema.BaseClasses[0];
        }
    }

    internal static IReadOnlyDictionary<int, (SdkVTableDefinition Definition, SdkSlotDefinition Slot)> Match(
        SchemaVTable table, IReadOnlyDictionary<string, SchemaClass> classes,
        IReadOnlyDictionary<(string Class, ulong Offset), SdkVTableDefinition> definitions, Action<string> diagnostic,
        Func<SdkVTableDefinition, bool>? compatibleLayout = null)
    {
        var slots = new Dictionary<int, (SdkVTableDefinition, SdkSlotDefinition)>();
        foreach (var key in Candidates(table, classes))
        {
            if (!definitions.TryGetValue(key, out var definition)) continue;
            if (compatibleLayout != null && !compatibleLayout(definition))
            {
                diagnostic($"[schema-sdk] {table.ClassName}: {definition.ClassName} subobject layout mismatch; definition skipped.");
                continue;
            }
            if (definition.Slots.Count > table.Functions.Count)
            {
                diagnostic($"[schema-sdk] {table.ClassName}: {key.Class} SDK table has {definition.Slots.Count} slots, " +
                    $"binary has {table.Functions.Count}; definition skipped.");
                continue;
            }
            for (int i = 0; i < definition.Slots.Count; i++) slots.TryAdd(i, (definition, definition.Slots[i]));
        }
        return slots;
    }

    internal static string UniqueName(string wanted, int index, ISet<string> used)
    {
        if (used.Add(wanted)) return wanted;
        string stem = $"{wanted}_slot_{index}", name = stem;
        for (int suffix = 2; !used.Add(name); suffix++) name = $"{stem}_{suffix}";
        return name;
    }
}

internal sealed unsafe class Hl2SdkVTables(Action<string> diagnostic) : IDisposable
{
    private readonly Dictionary<(string Class, ulong Offset), SdkVTableDefinition> _definitions = [];
    private readonly List<nint> _libraries = [];
    private readonly Dictionary<(string Class, ulong Offset), nint> _sources = [];
    internal IReadOnlyDictionary<(string Class, ulong Offset), SdkVTableDefinition> Definitions => _definitions;

    internal static Hl2SdkVTables Load(string root, SchemaTargetPlatform platform, SchemaSelection selection,
        IReadOnlyList<SchemaVTable> tables, Action<string> diagnostic)
    {
        var result = new Hl2SdkVTables(diagnostic);
        string directory = Path.Combine(Path.GetTempPath(), $"s2atelier-vtables-{Guid.NewGuid():N}");
        bool keep = false;
        try
        {
            var headers = ValveInterfaceCatalog.ReadHeaders(root);
            var requested = tables.SelectMany(t => SdkVTableMatching.Candidates(t with
            {
                ObjectOffset = t.ObjectOffset ?? 0, ThisType = t.ThisType ?? t.ClassName,
            }, selection.Classes)).Select(x => x.Class).Distinct(StringComparer.Ordinal).ToArray();
            var groups = requested.Select(name => (Name: name, Header: ValveInterfaceCatalog.FindDefinitionHeader(name, headers)))
                .Where(x => x.Header != null).GroupBy(x => x.Header!, StringComparer.OrdinalIgnoreCase).ToArray();
            if (groups.Length == 0) return result;
            Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, "network_connection.pb.h"), "#pragma once\n");
            SchemaImport.ConfigureClang(root, platform, skipLayoutAssertions: true);
            int index = 0;
            foreach (var group in groups)
            {
                string path = Path.Combine(directory, $"sdk-{index++:D4}.hpp");
                var definitions = group.Select(x => new ValveInterfaceDefinition("", "", x.Name, "", x.Header)).ToArray();
                ValveInterfaceImport.WriteHeader(path, ["public/tier0/platform.h", group.Key], definitions);
                // The parser must never resolve a requested SDK class from a previous schema import.
                void* til = CreateLibrary();
                if (til == null) { diagnostic("[schema-sdk] cannot allocate temporary TIL."); continue; }
                int errors;
                try { errors = SchemaImport.ParseHeader(path, false, true, til); }
                catch { IdaNative.free_til(til); throw; }
                if (errors != 0)
                {
                    IdaNative.free_til(til);
                    keep = true;
                    diagnostic($"[schema-sdk] {group.Key}: {errors} parse error(s); group skipped.");
                    continue;
                }
                result._libraries.Add((nint)til);
                foreach (var item in group) result.ReadClass(til, item.Name, group.Key);
            }
            return result;
        }
        catch { result.Dispose(); throw; }
        finally
        {
            if (keep) diagnostic($"[schema-sdk] diagnostic headers retained: {directory}");
            else if (Directory.Exists(directory))
                try { Directory.Delete(directory, true); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
        }
    }

    internal static void* CreateLibrary()
    {
        byte* name = Utf8.Allocate("s2atelier_sdk");
        try { return IdaNative.new_til(name, null); }
        finally { Utf8.Free(name); }
    }

    internal static bool LoadType(void* til, string name, out TypeInfo type)
    {
        type = default;
        byte* native = Utf8.Allocate(name);
        try
        {
            byte* bytes = null, fields = null, comment = null, comments = null;
            if (IdaNative.get_named_type(til, native, 1, &bytes, &fields, &comment, &comments, null, null) == 0) return false;
            fixed (TypeInfo* output = &type)
                return IdaNative.deserialize_tinfo(output, til, &bytes, &fields, &comments, comment) != 0;
        }
        finally { Utf8.Free(native); }
    }

    private void ReadClass(void* til, string name, string header)
    {
        ReadTable(til, name, 0, header);
        if (!LoadType(til, name, out var type)) return;
        try { ReadSecondary(type.Typid, 0, new HashSet<ulong>(), 0); }
        finally { type.Dispose(); }
        void ReadSecondary(ulong id, ulong offset, HashSet<ulong> visiting, int depth)
        {
            if (depth > 32 || !visiting.Add(id)) return;
            try
            {
                nuint count = IdaNative.get_tinfo_property(id, 16);
                for (ulong i = 0; i < Math.Min((ulong)count, 4096); i++)
                {
                    if (!ValveImplementationTypes.ReadMember(id, i, out var member)) continue;
                    try
                    {
                        if ((member.Flags & (0x20 | 0x80)) != 0x20 || member.Offset % 8 != 0) continue;
                        ulong childOffset = offset + member.Offset / 8;
                        if (childOffset != 0) ReadTable(til, name, childOffset, header);
                        ReadSecondary(member.Type.Typid, childOffset, visiting, depth + 1);
                    }
                    finally { member.Dispose(); }
                }
            }
            finally { visiting.Remove(id); }
        }
    }

    private void ReadTable(void* til, string name, ulong offset, string header)
    {
        string tableName = offset == 0 ? name + "_vtbl" : $"{name}_{offset:X4}_vtbl";
        if (!LoadType(til, tableName, out var type)) return;
        try
        {
            nuint count = IdaNative.get_tinfo_property(type.Typid, 16);
            if ((IdaNative.get_tinfo_property(type.Typid, 306) & 0x100) == 0 || count is 0 or > 1024) return;
            var slots = new List<SdkSlotDefinition>();
            for (ulong i = 0; i < count; i++)
            {
                if (!ValveImplementationTypes.ReadMember(type.Typid, i, out var member)) return;
                try
                {
                    if (member.Offset != i * 64 || member.Size != 64 ||
                        IdaNative.get_tinfo_property(member.Type.Typid, 6) == 0) return;
                    TypeInfo function = new() { Typid = (ulong)IdaNative.get_tinfo_property(member.Type.Typid, 9) };
                    try
                    {
                        var prototype = SdkPortableType.Capture(ref function, til);
                        if (prototype == null || member.Name.Read().Length == 0) return;
                        slots.Add(new(member.Name.Read(), prototype));
                    }
                    finally { function.Dispose(); }
                }
                finally { member.Dispose(); }
            }
            _definitions[(name, offset)] = new(name, offset, header, slots);
            _sources[(name, offset)] = (nint)til;
        }
        finally { type.Dispose(); }
    }

    internal IReadOnlyDictionary<(ulong Table, int Index), SdkResolvedSlot> Resolve(
        IReadOnlyList<SchemaVTable> tables, SchemaSelection selection)
    {
        var result = new Dictionary<(ulong, int), SdkResolvedSlot>();
        int inherited = 0;
        foreach (var table in tables)
        {
            foreach (var (index, match) in SdkVTableMatching.Match(table, selection.Classes, _definitions, diagnostic,
                definition => CompatibleLayout(table, definition, (void*)_sources[(definition.ClassName, definition.Offset)])))
            {
                var definition = match.Definition;
                void* source = (void*)_sources[(definition.ClassName, definition.Offset)];
                if (!match.Slot.Prototype.Restore(out var function, source))
                {
                    function.Dispose();
                    diagnostic($"[schema-sdk] {table.ClassName} slot {index}: cannot restore SDK prototype; fallback.");
                    continue;
                }
                try
                {
                    if (!ImportDependencies(function.Typid, source, new HashSet<string>(), new HashSet<ulong>()))
                    {
                        diagnostic($"[schema-sdk] {table.ClassName} slot {index}: unresolved prototype dependencies/this; fallback.");
                        continue;
                    }
                    var portable = SdkPortableType.Capture(ref function, source);
                    if (portable == null)
                    {
                        diagnostic($"[schema-sdk] {table.ClassName} slot {index}: cannot export prototype; fallback.");
                        continue;
                    }
                    if (!portable.Restore(out var check))
                    {
                        check.Dispose();
                        diagnostic($"[schema-sdk] {table.ClassName} slot {index}: cannot relocate prototype into database; fallback.");
                        continue;
                    }
                    try
                    {
                        if (!AdjustThis(ref check, table.ThisType!))
                        {
                            diagnostic($"[schema-sdk] {table.ClassName} slot {index}: cannot resolve this type; fallback.");
                            continue;
                        }
                        var local = SdkPortableType.Capture(ref check, IdaNative.get_idati());
                        if (local == null) continue;
                        result[(table.AddressPoint, index)] = new(match.Slot.Name, definition.ClassName, definition.Header, local);
                        if (definition.ClassName != table.ClassName) inherited++;
                    }
                    finally { check.Dispose(); }
                }
                finally { function.Dispose(); }
            }
        }
        diagnostic($"[schema-sdk] sdk-slots={result.Count}, inherited-slots={inherited}, " +
            $"fallback-slots={tables.Sum(t => t.Functions.Count) - result.Count}.");
        return result;
    }

    private static bool CompatibleLayout(SchemaVTable table, SdkVTableDefinition definition, void* source)
    {
        if (definition.ClassName != table.ClassName)
        {
            if (!ValveImplementationTypes.Load(table.ClassName, out var actual)) return false;
            try { return SchemaVTableTypes.HasBaseAt(actual.Typid, definition.ClassName, table.ObjectOffset!.Value); }
            finally { actual.Dispose(); }
        }
        if (definition.Offset == 0) return true;
        if (!LoadType(source, definition.ClassName, out var sdkClass)) return false;
        try { return SchemaVTableTypes.HasBaseAt(sdkClass.Typid, table.ThisType!, definition.Offset); }
        finally { sdkClass.Dispose(); }
    }

    private bool ImportDependencies(ulong id, void* source, HashSet<string> importing, HashSet<ulong> visiting)
    {
        if (!visiting.Add(id)) return true;
        if ((id & 0x100) != 0)
        {
            QString text = default;
            string name;
            try { IdaNative.get_tinfo_pdata(&text, id, 0); name = text.Read(); }
            finally { text.Dispose(); }
            if (name.Length != 0)
            {
                // Built-in/base-TIL aliases need not have an IDB ordinal (e.g. uint).
                if (LoadType(IdaNative.get_idati(), name, out var existing)) { existing.Dispose(); return true; }
                existing.Dispose();
                if (!importing.Add(name)) return true;
                if (IdaNative.get_tinfo_property(id, 5) != 0)
                    return ImportForward(name, (byte)IdaNative.get_tinfo_property(id, 290));
                if (!LoadType(source, name, out var dependency))
                {
                    diagnostic($"[schema-sdk] dependency {name}: SDK definition unavailable.");
                    return false;
                }
                try
                {
                    if (!ImportChildren(dependency.Typid, source, importing, visiting)) return false;
                    var portable = SdkPortableType.Capture(ref dependency, source);
                    if (portable == null)
                    {
                        diagnostic($"[schema-sdk] dependency {name}: cannot export definition.");
                        return false;
                    }
                    if (!portable.Restore(out var local))
                    {
                        diagnostic($"[schema-sdk] dependency {name}: cannot relocate definition.");
                        return false;
                    }
                    try
                    {
                        // No REPLACE: schema and user definitions always win dependency name collisions.
                        byte* native = Utf8.Allocate(name);
                        try
                        {
                            int status = IdaNative.save_tinfo(&local, IdaNative.get_idati(), 0, native, 1);
                            if (status != 0) diagnostic($"[schema-sdk] dependency {name}: save failed ({status}).");
                            return status == 0;
                        }
                        finally { Utf8.Free(native); }
                    }
                    finally { local.Dispose(); }
                }
                finally { dependency.Dispose(); }
            }
        }
        return ImportChildren(id, source, importing, visiting);
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TypedefData
    {
        internal void* Til;
        internal byte* Name;
        internal byte IsOrdinal, Resolve;
    }

    private static bool ImportForward(string name, byte tag)
    {
        if ((tag & 0x30) == 0x30) return false;
        byte* empty = Utf8.Allocate(""), native = Utf8.Allocate(name);
        TypeInfo forward = default;
        try
        {
            var data = new TypedefData { Til = IdaNative.get_idati(), Name = empty };
            // tinfo_t::create_forward_decl: an unresolved, empty-name typedef with the SDK tag.
            return IdaNative.create_tinfo(&forward, (byte)(0x0d | (tag & 0x30)), 0xbd, &data) != 0 &&
                IdaNative.save_tinfo(&forward, IdaNative.get_idati(), 0, native, 1) == 0;
        }
        finally { forward.Dispose(); Utf8.Free(empty); Utf8.Free(native); }
    }

    private bool ImportChildren(ulong id, void* source, HashSet<string> importing, HashSet<ulong> visiting)
    {
        int kind = (int)IdaNative.get_tinfo_property(id, 1) & 0x0f;
        if (kind is 10 or 11) return Child(kind == 10 ? 9 : 11);
        if (kind == 12)
        {
            if (!Child(24)) return false;
            nuint count = IdaNative.get_tinfo_property(id, 23);
            if (count > 255) return false;
            for (int i = 0; i < (int)count; i++) if (!Child(25 + i)) return false;
        }
        if (kind == 13 && (IdaNative.get_tinfo_property(id, 1) & 0x30) != 0x20)
        {
            nuint count = IdaNative.get_tinfo_property(id, 16);
            if (count > 16384) return false;
            for (ulong i = 0; i < count; i++)
            {
                if (!ValveImplementationTypes.ReadMember(id, i, out var member)) return false;
                try { if (!ImportDependencies(member.Type.Typid, source, importing, visiting)) return false; }
                finally { member.Dispose(); }
            }
        }
        return true;
        bool Child(int property)
        {
            TypeInfo child = new() { Typid = (ulong)IdaNative.get_tinfo_property(id, property) };
            try { return child.Typid != 0 && ImportDependencies(child.Typid, source, importing, visiting); }
            finally { child.Dispose(); }
        }
    }

    internal static bool AdjustThis(ref TypeInfo function, string owner)
    {
        if (IdaNative.get_tinfo_property(function.Typid, 23) is 0 or > 255) return false;
        TypeInfo oldThis = new() { Typid = (ulong)IdaNative.get_tinfo_property(function.Typid, 25) };
        TypeInfo objectType = default;
        string qualifiers;
        try
        {
            if ((IdaNative.get_tinfo_property(oldThis.Typid, 1) & 0x0f) != 10) return false;
            objectType.Typid = (ulong)IdaNative.get_tinfo_property(oldThis.Typid, 9);
            nuint kind = IdaNative.get_tinfo_property(objectType.Typid, 1);
            qualifiers = ((kind & 0x40) != 0 ? "const " : "") + ((kind & 0x80) != 0 ? "volatile " : "");
        }
        finally { objectType.Dispose(); oldThis.Dispose(); }
        byte* declaration = Utf8.Allocate($"{qualifiers}{owner} *__s2_this;");
        TypeInfo pointer = default;
        try
        {
            if (IdaNative.parse_decl(&pointer, null, IdaNative.get_idati(), declaration, 1 | 8 | 128) == 0) return false;
            fixed (TypeInfo* target = &function)
                return IdaNative.set_tinfo_property4(target, 31, 0, (nuint)(void*)&pointer, 0, 0) == 0;
        }
        finally { pointer.Dispose(); Utf8.Free(declaration); }
    }

    public void Dispose()
    {
        foreach (nint til in _libraries) IdaNative.free_til((void*)til);
        _libraries.Clear();
    }
}
