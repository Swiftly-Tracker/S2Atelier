using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace S2Atelier.Ida.Schema;

public enum SchemaTargetPlatform
{
    WindowsMsvc,
    LinuxItanium,
}

public sealed record SchemaHeaderResult(
    string Text,
    IReadOnlyList<string> ImportedTypeNames,
    IReadOnlyList<string> ExplicitTemplateInstantiations);

public static partial class SchemaHeaderGenerator
{
    // Source2SchemaDumper's list, with the three missing commas in common.py restored.
    public static readonly IReadOnlySet<string> Hl2SdkCommonTypes = new HashSet<string>(
    [
        "int8", "uint8", "int16", "uint16", "int32", "uint32", "int64", "uint64", "float32", "float64",
        "CUtlVector", "VectorAligned", "Vector", "Vector2D", "Vector4D", "matrix3x4_t", "matrix3x4a_t",
        "QAngle", "Quaternion", "Color", "CUtlString", "CUtlStringToken", "CUtlMap", "CUtlOrderedMap",
        "RadianEuler", "ThreeState_t", "CBufferString", "CBufferStringN", "CUtlHashtable", "CUtlLinkedList",
        "KeyValues", "KeyValues3", "SolidType_t", "RenderMode_t", "RenderFx_t", "MoveType_t", "MoveCollide_t",
        "LifeState_t", "CEntityIdentity", "CEntityInstance", "WorldGroupId_t", "CEntityIndex", "CPlayerSlot",
        "HitGroup_t", "EntComponentInfo_t", "CVariantDefaultAllocator", "CVariant", "CVariantBase", "CUtlSymbolLarge",
        "CTransform", "HSCRIPT", "fieldtype_t", "CScriptComponent", "CEntityComponent",
        "CPhysSurfacePropertiesSoundNames", "CPhysSurfacePropertiesPhysics", "CPhysSurfacePropertiesAudio",
        "CPhysSurfaceProperties", "CHitBox", "CEntityHandle", "ChangeAccessorFieldPathIndex_t",
        "CEntityComponentHelper", "soundlevel_t", "SoundFlags_t", "RenderMultisampleType_t", "EngineLoopState_t",
        "GameTime_t", "CUtlSymbol", "EntityIOTargetType_t", "EntityDormancyType_t", "CSplitScreenSlot", "CHandle",
        "CSmartPtr", "VectorWS", "CUtlLeanVectorFixedGrowable", "CUtlLeanVector", "CUtlVectorFixedGrowable",
        "EventClientOutput_t", "CUtlDict", "BASEPTR", "ENTITYFUNCPTR", "USEPTR", "ENetworkDisconnectionReason",
        "CBitVec", "CTypedBitVec", "Flags_t", "CPhysSurfacePropertiesVehicle", "EntityEffects_t", "CEntityKeyValues",
        "ItemFlagTypes_t", "DamageTypes_t", "ObserverMode_t", "EntityDissolveType_t", "Class_T", "InputBitMask_t",
    ], StringComparer.Ordinal);

    // HL2SDK names these template specializations in interface method signatures, but their argument
    // is engine-private and never defined. IDAClang instantiates such specializations eagerly, so
    // declare them explicitly without a definition. Shared with the Valve interface import headers.
    internal static readonly string[] OpaqueSdkSpecializations =
    [
        // ISource2Server::GetEntity2Networkables; an instantiated map node holds the incomplete element.
        "struct Entity2Networkable_t;",
        "template <typename T> class CDefLess;",
        "template <typename K, typename T, typename LF, typename I> class CUtlOrderedMap;",
        "template <> class CUtlOrderedMap<int, Entity2Networkable_t, CDefLess<int>, unsigned short>;",
    ];

    // The value types a convar can hold, indexed by EConVarType (tier1/convar.h).
    internal static readonly string[] ConVarValueTypes =
    [
        "bool", "short", "unsigned short", "int", "unsigned int", "long long", "unsigned long long",
        "float", "double", "CUtlString", "Color", "Vector2D", "Vector", "Vector4D", "QAngle", "VectorWS",
    ];

    // CConVar<T> is only instantiated in modules that declare convars, so the SDK translation unit has
    // none of them. An extern of each value type instantiates the class - not its member bodies - which
    // is all the convar pass needs to type a registered convar's global.
    internal static IEnumerable<string> ConVarInstantiations()
        => ConVarValueTypes.Select((type, index) => $"extern CConVar<{type}> __s2atelier_convar_{index:D2};");

    private static readonly string[] Includes =
    [
        "tier0/platform.h", "eiface.h", "iserver.h", "inetchannel.h", "iloopmode.h", "interfaces/interfaces.h",
        "const.h", "shareddefs.h", "mathlib/vector.h", "mathlib/vector2d.h", "mathlib/vector4d.h", "Color.h", "variant.h",
        "tier1/bufferstring.h", "tier1/KeyValues.h", "tier1/keyvalues3.h", "mathlib/transform.h", "igameevents.h",
        "tier1/smartptr.h", "gametrace.h", "playerslot.h", "datamap.h", "soundflags.h", "tier1/convar.h",
        "ehandle.h", "entity2/entityidentity.h", "entity2/entitysystem.h", "entity2/entityinstance.h",
        "entity2/entitykeyvalues.h", "entity2/entityclass.h", "entityhandle.h", "schemasystem/schemasystem.h",
        "schemasystem/schematypes.h", "networksystem/inetworkserializer.h", "networksystem/inetworkmessages.h",
        "networksystem/netmessage.h", "networksystem/iflattenedserializers.h", "networksystem/inetworksystem.h",
        "networksystem/iprotobufbinding.h", "tier1/memstack.h", "tier1/utlsymbol.h", "tier1/utlsymbollarge.h",
        "tier1/utlscratchmemory.h", "tier1/utlleanvector.h", "tier1/utldict.h", "tier1/bitbuf.h",
        "tier1/circularbuffer.h", "tier1/utlhash.h", "tier1/utlhashdict.h", "tier1/utlhashtable.h",
        "tier1/utlmap.h", "tier1/utlstring.h", "tier1/utllinkedlist.h", "tier1/utlvector.h", "tier1/utltshash.h",
    ];

    public static SchemaHeaderResult Generate(
        SchemaSelection selection,
        SchemaTargetPlatform platform,
        IReadOnlySet<string>? vtableClasses = null)
    {
        vtableClasses ??= new HashSet<string>(StringComparer.Ordinal);
        var polymorphic = ExpandPolymorphic(selection, vtableClasses);
        var alignments = ComputeClassAlignments(selection, polymorphic);
        var opaque = CollectOpaqueTypes(selection);
        var templates = CollectExplicitTemplateInstantiations(selection);
        var text = new StringBuilder(1024 * 1024);

        EmitPreamble(text, platform);
        EmitOpaqueTypes(text, opaque);

        var generated = selection.Classes.Values
            .Where(x => !x.Synthetic && !IsHl2Override(x.Name))
            .ToDictionary(x => x.Name, StringComparer.Ordinal);
        var synthetic = selection.Classes.Values
            .Where(x => x.Synthetic)
            .ToDictionary(x => x.Name, StringComparer.Ordinal);

        foreach (SchemaClass item in generated.Values.Where(x => !x.Name.Contains("::", StringComparison.Ordinal)))
        {
            text.Append("class ").Append(item.Name).AppendLine(";");
        }
        foreach (SchemaEnum item in selection.Enums.Values.Where(x => !IsHl2Override(x.Name) &&
                                                                       !x.Name.Contains("::", StringComparison.Ordinal)))
        {
            text.Append("enum class ").Append(item.Name).Append(" : ").Append(EnumUnderlying(item)).AppendLine(";");
        }
        text.AppendLine();

        var groups = generated.Keys.Concat(synthetic.Keys)
            .Concat(selection.Enums.Keys.Where(x => !IsHl2Override(x)))
            .Select(TopLevelName).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
        var emitted = new HashSet<string>(StringComparer.Ordinal);
        var visiting = new HashSet<string>(StringComparer.Ordinal);
        var emittedTemplates = new HashSet<string>(StringComparer.Ordinal);
        var assertions = new List<string>();

        void EmitTopLevel(string group)
        {
            if (emitted.Contains(group))
            {
                return;
            }
            if (!visiting.Add(group))
            {
                throw new SchemaFormatException($"By-value schema dependency cycle involving '{group}'.");
            }

            foreach (string dependency in GroupDependencies(group, selection))
            {
                string dependencyGroup = TopLevelName(dependency);
                if (!dependencyGroup.Equals(group, StringComparison.Ordinal) && groups.Contains(dependencyGroup, StringComparer.Ordinal))
                {
                    EmitTopLevel(dependencyGroup);
                }
            }

            visiting.Remove(group);
            foreach (string template in GroupTemplateInstantiations(group, selection)
                         .Where(x => CanInstantiateBeforeGroup(x, group, selection)))
            {
                if (emittedTemplates.Add(template))
                {
                    text.Append("template class ").Append(template).AppendLine(";");
                }
            }
            EmitScopedType(text, group, 0, selection, generated, synthetic, polymorphic, alignments, opaque, assertions);
            int forceIndex = 0;
            string nestedPrefix = group + "::";
            foreach (string nested in generated.Keys.Concat(selection.Enums.Keys.Where(x => !IsHl2Override(x)))
                         .Where(x => x.StartsWith(nestedPrefix, StringComparison.Ordinal)).Order(StringComparer.Ordinal))
            {
                // IDAClang otherwise tends to retain only the forward declaration of nested types.
                text.Append("static ").Append(nested).Append(" __s2_force_nested_").Append(group)
                    .Append('_').Append(forceIndex++).AppendLine(";");
            }
            text.AppendLine();
            emitted.Add(group);
        }

        foreach (string group in groups)
        {
            EmitTopLevel(group);
        }

        foreach (string template in templates.Where(emittedTemplates.Add))
        {
            text.Append("template class ").Append(template).AppendLine(";");
        }
        if (templates.Count > 0)
        {
            text.AppendLine();
        }

        text.AppendLine("#ifndef S2ATELIER_SCHEMA_SKIP_LAYOUT_ASSERTS");
        foreach (string assertion in assertions)
        {
            text.AppendLine(assertion);
        }
        EmitOverrideAssertions(text, selection);
        text.AppendLine("#endif");
        text.AppendLine("#pragma pack(pop)");

        var names = selection.Classes.Values.Where(x => !x.Synthetic).Select(x => x.Name)
            .Concat(selection.Enums.Keys).Order(StringComparer.Ordinal).ToArray();
        return new SchemaHeaderResult(text.ToString(), names, templates);
    }

    private static void EmitPreamble(StringBuilder text, SchemaTargetPlatform platform)
    {
        text.AppendLine("// Generated by S2Atelier. This file is intentionally temporary.");
        text.AppendLine("#include <cstddef>");
        text.AppendLine("#include <cstdint>");
        text.AppendLine("#include <utility>");
        // eiface.h/igameevents.h pass CNetMessagePB<T> (see netmessage.h) for a couple of
        // protobuf message types; T must be complete before those headers are reached. When the
        // SDK's protoc compiled real headers for them (see ConfigureClang), pull those in now;
        // otherwise these are absent and T stays an incomplete forward declaration, same as before.
        foreach (string include in (string[])["netmessages.pb.h", "gameevents.pb.h"])
        {
            text.Append("#if __has_include(\"").Append(include).AppendLine("\")");
            text.Append("#include \"").Append(include).AppendLine("\"");
            text.AppendLine("#endif");
        }
        foreach (string declaration in OpaqueSdkSpecializations)
        {
            text.AppendLine(declaration);
        }
        text.AppendLine("#define UTLDELEGATE_H 1");
        text.AppendLine("template <typename T> class CUtlDelegate;");
        for (int count = 0; count <= 5; count++)
        {
            string args = count == 0 ? string.Empty : ", " + string.Join(", ", Enumerable.Range(1, count).Select(i => $"typename A{i}"));
            string signature = count == 0 ? string.Empty : string.Join(", ", Enumerable.Range(1, count).Select(i => $"A{i}"));
            text.Append("template <typename R").Append(args).Append("> class CUtlDelegate<R(").Append(signature)
                .AppendLine(")> { unsigned char __s2_delegate_abi[16]; };");
        }
        text.AppendLine("class CUtlAbstractDelegate { unsigned char __s2_delegate_abi[16]; };");
        foreach (string include in Includes.Take(6))
        {
            text.Append("#if __has_include(\"").Append(include).AppendLine("\")");
            text.Append("#include \"").Append(include).AppendLine("\"");
            text.AppendLine("#endif");
        }
        text.AppendLine("#define private public");
        text.AppendLine("#define protected public");
        foreach (string include in Includes.Skip(6))
        {
            // __has_include also makes older/sparser HL2SDK branches usable without editing them.
            text.Append("#if __has_include(\"").Append(include).AppendLine("\")");
            text.Append("#include \"").Append(include).AppendLine("\"");
            text.AppendLine("#endif");
        }
        text.AppendLine("#undef protected");
        text.AppendLine("#undef private");
        // Valve only declares the primary hash functor in the public SDK. Games add
        // specializations for their own key types in private headers, so provide the
        // same ABI-neutral fallback while parsing layout-only declarations.
        text.AppendLine("template <typename T> struct DefaultHashFunctor : Mix32HashFunctor { };");
        text.AppendLine("using CUtlStringTokenNoRegistration = CUtlStringToken;");
        text.AppendLine("using int8 = std::int8_t; using uint8 = std::uint8_t;");
        text.AppendLine("using int16 = std::int16_t; using uint16 = std::uint16_t;");
        text.AppendLine("using int32 = std::int32_t; using uint32 = std::uint32_t;");
        text.AppendLine("using int64 = std::int64_t; using uint64 = std::uint64_t;");
        text.AppendLine("using float32 = float; using float64 = double;");
        text.Append("static_assert(sizeof(void*) == 8, \"")
            .Append(platform == SchemaTargetPlatform.WindowsMsvc ? "PE x64" : "ELF x64")
            .AppendLine(" schema import requires 64-bit pointers\");");
        foreach (string declaration in ConVarInstantiations())
        {
            text.AppendLine(declaration);
        }
        text.AppendLine("#pragma pack(push, 1)");
        text.AppendLine();
    }

    private static void EmitOpaqueTypes(StringBuilder text, IReadOnlyDictionary<OpaqueKey, string> opaque)
    {
        foreach ((OpaqueKey key, string name) in opaque.OrderBy(x => x.Value, StringComparer.Ordinal))
        {
            string align = ValidAlignment(key.Alignment) ? $" alignas({key.Alignment})" : string.Empty;
            text.Append("struct").Append(align).Append(' ').Append(name).Append(" { unsigned char __data[")
                .Append(Math.Max(1, key.Size)).AppendLine("]; };");
            text.Append("static_assert(sizeof(").Append(name).Append(") == ").Append(key.Size).AppendLine(");");
            if (ValidAlignment(key.Alignment))
            {
                text.Append("static_assert(alignof(").Append(name).Append(") == ").Append(key.Alignment).AppendLine(");");
            }
        }
        if (opaque.Count > 0)
        {
            text.AppendLine();
        }
    }

    private static void EmitScopedType(
        StringBuilder text,
        string fullName,
        int indent,
        SchemaSelection selection,
        IReadOnlyDictionary<string, SchemaClass> generated,
        IReadOnlyDictionary<string, SchemaClass> synthetic,
        IReadOnlySet<string> polymorphic,
        IReadOnlyDictionary<string, int> alignments,
        IReadOnlyDictionary<OpaqueKey, string> opaque,
        ICollection<string> assertions)
    {
        string pad = new(' ', indent * 4);
        string leaf = LeafName(fullName);
        if (selection.Enums.TryGetValue(fullName, out SchemaEnum? schemaEnum))
        {
            if (IsHl2Override(fullName))
            {
                return;
            }
            text.Append(pad).Append("enum class ").Append(leaf).Append(" : ").Append(EnumUnderlying(schemaEnum)).AppendLine(" {");
            foreach (SchemaEnumValue value in schemaEnum.Fields)
            {
                text.Append(pad).Append("    ").Append(value.Name).Append(" = ")
                    .Append(EnumLiteral(value))
                    .AppendLine(",");
            }
            text.Append(pad).AppendLine("};");
            assertions.Add($"static_assert(sizeof({fullName}) == {schemaEnum.Size});");
            if (ValidAlignment(schemaEnum.Alignment))
            {
                assertions.Add($"static_assert(alignof({fullName}) == {schemaEnum.Alignment});");
            }
            AddEnumValueAssertions(assertions, schemaEnum);
            return;
        }

        bool isSynthetic = synthetic.TryGetValue(fullName, out SchemaClass? type);
        if (!isSynthetic && !generated.TryGetValue(fullName, out type))
        {
            return;
        }

        // Classes without a schema alignment still need their real alignment: clang would otherwise
        // derive it under pack(1) and misplace this class wherever it is a non-primary base.
        int alignment = isSynthetic ? 1 : alignments.GetValueOrDefault(fullName, 1);
        string align = !isSynthetic && (ValidAlignment(type!.Alignment) || alignment > 1) ? $" alignas({alignment})" : string.Empty;
        text.Append(pad).Append("class").Append(align).Append(' ').Append(leaf);
        if (!isSynthetic && type!.BaseClasses.Count > 0)
        {
            text.Append(" : ").AppendJoin(", ", type.BaseClasses.Select(x => "public " + x));
        }
        text.AppendLine(" {");
        text.Append(pad).AppendLine("public:");

        string prefix = fullName + "::";
        var children = generated.Keys.Concat(synthetic.Keys).Concat(selection.Enums.Keys.Where(x => !IsHl2Override(x)))
            .Where(x => x.StartsWith(prefix, StringComparison.Ordinal) &&
                        !x[prefix.Length..].Contains("::", StringComparison.Ordinal))
            .Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
        foreach (string child in children)
        {
            EmitScopedType(text, child, indent + 1, selection, generated, synthetic, polymorphic, alignments, opaque, assertions);
        }

        if (!isSynthetic)
        {
            EmitFields(text, type!, fullName, indent + 1, selection, polymorphic, alignments, opaque, assertions);
        }
        text.Append(pad).AppendLine("};");
    }

    private static void EmitFields(
        StringBuilder text,
        SchemaClass type,
        string fullName,
        int indent,
        SchemaSelection selection,
        IReadOnlySet<string> polymorphic,
        IReadOnlyDictionary<string, int> alignments,
        IReadOnlyDictionary<OpaqueKey, string> opaque,
        ICollection<string> assertions)
    {
        string pad = new(' ', indent * 4);
        int current = 0;
        foreach (string baseName in type.BaseClasses)
        {
            int size = FindClassSize(selection, baseName);
            // Non-primary bases start at their own alignment boundary (e.g. a CTransform-holding base).
            if (size > 0 && current > 0) current = AlignUp(current, alignments.GetValueOrDefault(baseName, 1));
            current += size;
        }
        bool primaryBasePolymorphic = type.BaseClasses.Count > 0 && polymorphic.Contains(type.BaseClasses[0]);
        bool ownVptr = polymorphic.Contains(fullName) && !primaryBasePolymorphic && current == 0;
        if (ownVptr)
        {
            text.Append(pad).AppendLine("void *__vftable;");
            assertions.Add($"static_assert(offsetof({fullName}, __vftable) == 0);");
            current = 8;
        }

        int paddingIndex = 0;
        for (int i = 0; i < type.Fields.Count; i++)
        {
            SchemaField field = type.Fields[i];
            if (field.Kind == SchemaFieldKind.Bitfield)
            {
                int end = i;
                while (end + 1 < type.Fields.Count && type.Fields[end + 1].Kind == SchemaFieldKind.Bitfield)
                {
                    end++;
                }
                int nextOffset = end + 1 < type.Fields.Count ? type.Fields[end + 1].Offset : type.Size;
                int available = Math.Max(0, nextOffset - current);
                EmitBitfieldGroup(text, type.Fields.Skip(i).Take(end - i + 1).ToArray(), available, pad);
                int used = BitfieldStorageBytes(type.Fields.Skip(i).Take(end - i + 1).ToArray(), available);
                current += used;
                i = end;
                continue;
            }

            if (field.Offset < current)
            {
                throw new SchemaFormatException(
                    $"Field '{fullName}::{field.Name}' at 0x{field.Offset:X} overlaps inherited/generated data ending at 0x{current:X}.");
            }
            if (field.Offset > current)
            {
                EmitPadding(text, pad, ref paddingIndex, field.Offset - current);
            }

            string declaration = FieldDeclaration(field, selection, opaque);
            text.Append(pad).Append(declaration).AppendLine(";");
            assertions.Add($"static_assert(offsetof({fullName}, {field.Name}) == {field.Offset});");
            current = field.Offset + field.Size;
        }

        if (current < type.Size)
        {
            EmitPadding(text, pad, ref paddingIndex, type.Size - current);
            current = type.Size;
        }
        if (current != type.Size)
        {
            throw new SchemaFormatException($"Generated layout for '{fullName}' is {current} bytes, expected {type.Size}.");
        }

        assertions.Add($"static_assert(sizeof({fullName}) == {type.Size});");
        if (ValidAlignment(type.Alignment))
        {
            assertions.Add($"static_assert(alignof({fullName}) == {type.Alignment});");
        }
    }

    private static void EmitBitfieldGroup(StringBuilder text, IReadOnlyList<SchemaField> fields, int available, string pad)
    {
        foreach (BitfieldUnit unit in PartitionBitfields(fields, available))
        {
            string storage = unit.Capacity switch
            {
                8 => "std::uint8_t",
                16 => "std::uint16_t",
                32 => "std::uint32_t",
                _ => "std::uint64_t",
            };
            foreach (SchemaField field in unit.Fields)
            {
                text.Append(pad).Append(storage).Append(' ').Append(field.Name).Append(" : ")
                    .Append(field.BitCount).AppendLine(";");
            }
        }
    }

    private static int BitfieldStorageBytes(IReadOnlyList<SchemaField> fields, int available)
        => PartitionBitfields(fields, available).Sum(x => x.Capacity / 8);

    private static IReadOnlyList<BitfieldUnit> PartitionBitfields(IReadOnlyList<SchemaField> fields, int available)
    {
        var memo = new Dictionary<int, IReadOnlyList<BitfieldUnit>?>();
        IReadOnlyList<BitfieldUnit>? Solve(int index)
        {
            if (index == fields.Count)
            {
                return [];
            }
            if (memo.TryGetValue(index, out IReadOnlyList<BitfieldUnit>? cached))
            {
                return cached;
            }
            IReadOnlyList<BitfieldUnit>? best = null;
            int bits = 0;
            for (int end = index; end < fields.Count; end++)
            {
                if (fields[end].BitCount is <= 0 or > 64)
                {
                    throw new SchemaFormatException($"Invalid bitfield width {fields[end].BitCount} for '{fields[end].Name}'.");
                }
                bits += fields[end].BitCount;
                foreach (int capacity in new[] { 8, 16, 32, 64 })
                {
                    if (bits > capacity || fields.Skip(index).Take(end - index + 1).Any(x => x.BitCount > capacity))
                    {
                        continue;
                    }
                    var tail = Solve(end + 1);
                    if (tail == null)
                    {
                        continue;
                    }
                    var candidate = new[] { new BitfieldUnit(capacity, fields.Skip(index).Take(end - index + 1).ToArray()) }
                        .Concat(tail).ToArray();
                    int bytes = candidate.Sum(x => x.Capacity / 8);
                    if (bytes <= available && (best == null || bytes < best.Sum(x => x.Capacity / 8) ||
                                               bytes == best.Sum(x => x.Capacity / 8) && candidate.Length < best.Count))
                    {
                        best = candidate;
                    }
                }
            }
            memo[index] = best;
            return best;
        }

        var result = Solve(0);
        if (result == null)
        {
            throw new SchemaFormatException($"Bitfield group does not fit its {available}-byte schema slot.");
        }
        return result;
    }

    private static void EmitPadding(StringBuilder text, string pad, ref int index, int size)
    {
        text.Append(pad).Append("unsigned char __pad_").Append(index++).Append('[').Append(size).AppendLine("];");
    }

    private static string FieldDeclaration(
        SchemaField field, SchemaSelection selection, IReadOnlyDictionary<OpaqueKey, string> opaque)
    {
        if (field.Kind == SchemaFieldKind.Pointer)
        {
            return $"{NormalizeCppType(field.TypeName)} *{field.Name}";
        }

        if (field.Kind == SchemaFieldKind.FixedArray)
        {
            string type = NormalizeCppType(SchemaDatabase.StripArray(field.TypeName));
            string existing = field.TypeName[SchemaDatabase.StripArray(field.TypeName).Length..].Replace(" ", string.Empty, StringComparison.Ordinal);
            return $"{type} {field.Name}[{field.ElementCount}]{existing}";
        }

        if (field.Kind == SchemaFieldKind.Atomic)
        {
            string spelling = field.TemplatedType ?? field.TypeName;
            if ((IsHl2Override(field.TypeName) || IsStdOverride(field.TypeName)) &&
                !RequiresOpaqueAtomic(field))
            {
                return $"{spelling} {field.Name}";
            }
            return $"{opaque[new OpaqueKey(spelling, field.Size, field.Alignment)]} {field.Name}";
        }

        string rawReference = SchemaDatabase.NormalizeTypeReference(field.TypeName);
        string reference = NormalizeCppType(rawReference);
        if (!SchemaDatabase.IsBuiltin(rawReference) && !selection.Classes.ContainsKey(rawReference) &&
            !selection.Enums.ContainsKey(rawReference) && !IsHl2Override(rawReference))
        {
            reference = opaque[new OpaqueKey(reference, field.Size, field.Alignment)];
        }
        return $"{reference} {field.Name}";
    }

    private static IReadOnlyDictionary<OpaqueKey, string> CollectOpaqueTypes(SchemaSelection selection)
    {
        var keys = new HashSet<OpaqueKey>();
        foreach (SchemaField field in selection.Classes.Values.SelectMany(x => x.Fields))
        {
            if (field.Kind == SchemaFieldKind.Atomic &&
                (RequiresOpaqueAtomic(field) ||
                 (!IsHl2Override(field.TypeName) && !IsStdOverride(field.TypeName))))
            {
                keys.Add(new OpaqueKey(field.TemplatedType ?? field.TypeName, field.Size, field.Alignment));
            }
            else if (field.Kind == SchemaFieldKind.Reference)
            {
                string name = SchemaDatabase.NormalizeTypeReference(field.TypeName);
                if (!SchemaDatabase.IsBuiltin(name) && !selection.Classes.ContainsKey(name) &&
                    !selection.Enums.ContainsKey(name) && !IsHl2Override(name))
                {
                    keys.Add(new OpaqueKey(name, field.Size, field.Alignment));
                }
            }
        }

        var result = new Dictionary<OpaqueKey, string>();
        var used = new HashSet<string>(StringComparer.Ordinal);
        var spellingCounts = keys.GroupBy(x => x.Spelling, StringComparer.Ordinal)
            .ToDictionary(x => x.Key, x => x.Count(), StringComparer.Ordinal);
        foreach (OpaqueKey key in keys.OrderBy(x => x.Spelling, StringComparer.Ordinal).ThenBy(x => x.Size).ThenBy(x => x.Alignment))
        {
            if (spellingCounts[key.Spelling] == 1 && CppIdentifierRegex().IsMatch(key.Spelling))
            {
                used.Add(key.Spelling);
                result.Add(key, key.Spelling);
                continue;
            }

            string stem = InvalidIdentifierChars().Replace(key.Spelling, "_").Trim('_');
            if (stem.Length > 48)
            {
                stem = stem[..48];
            }
            string name = $"__s2_opaque_{stem}_{key.Size}_{key.Alignment}";
            int suffix = 2;
            while (!used.Add(name))
            {
                name = $"__s2_opaque_{stem}_{key.Size}_{key.Alignment}_{suffix++}";
            }
            result.Add(key, name);
        }
        return result;
    }

    private static IReadOnlyList<string> CollectExplicitTemplateInstantiations(SchemaSelection selection)
        => selection.Classes.Values.SelectMany(x => x.Fields)
            .Where(x => x.Kind == SchemaFieldKind.Atomic && x.TemplatedType != null &&
                        IsHl2Override(x.TypeName) && x.TemplatedType.Contains("::", StringComparison.Ordinal))
            .Select(x => x.TemplatedType!).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();

    private static IEnumerable<string> GroupTemplateInstantiations(string group, SchemaSelection selection)
    {
        string prefix = group + "::";
        return selection.Classes.Values.Where(x => x.Name == group || x.Name.StartsWith(prefix, StringComparison.Ordinal))
            .SelectMany(x => x.Fields)
            .Where(x => x.Kind == SchemaFieldKind.Atomic && x.TemplatedType != null &&
                        IsHl2Override(x.TypeName) && x.TemplatedType.Contains("::", StringComparison.Ordinal))
            .Select(x => x.TemplatedType!).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal);
    }

    private static bool CanInstantiateBeforeGroup(string template, string group, SchemaSelection selection)
    {
        foreach (Match match in TemplateIdentifierRegex().Matches(template))
        {
            string dependency = match.Value;
            if (selection.Classes.ContainsKey(dependency) || selection.Enums.ContainsKey(dependency))
            {
                if (TopLevelName(dependency) == group)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private static IReadOnlySet<string> ExpandPolymorphic(SchemaSelection selection, IReadOnlySet<string> detected)
    {
        var result = new HashSet<string>(detected.Where(selection.Classes.ContainsKey), StringComparer.Ordinal);
        var queue = new Queue<string>(result);
        while (queue.TryDequeue(out string? name))
        {
            if (!selection.Classes.TryGetValue(name, out SchemaClass? type) || type.BaseClasses.Count == 0)
            {
                continue;
            }
            string primary = type.BaseClasses[0];
            if (selection.Classes.ContainsKey(primary) && result.Add(primary))
            {
                queue.Enqueue(primary);
            }
        }
        return result;
    }

    private static IEnumerable<string> GroupDependencies(string topLevel, SchemaSelection selection)
    {
        string prefix = topLevel + "::";
        foreach (SchemaClass type in selection.Classes.Values.Where(x => x.Name == topLevel || x.Name.StartsWith(prefix, StringComparison.Ordinal)))
        {
            foreach (string baseName in type.BaseClasses)
            {
                yield return baseName;
            }
            foreach (SchemaField field in type.Fields)
            {
                if (field.Kind is SchemaFieldKind.Reference or SchemaFieldKind.FixedArray)
                {
                    yield return SchemaDatabase.NormalizeTypeReference(field.TypeName);
                }
                else if (field.Kind == SchemaFieldKind.Atomic &&
                         SchemaDatabase.NormalizeTypeReference(field.TypeName).Contains("Hashtable", StringComparison.Ordinal))
                {
                    foreach (SchemaTemplateArgument argument in field.TemplateArguments ?? [])
                    {
                        if (argument.IsLiteral)
                        {
                            continue;
                        }

                        // Only a direct template argument needs to be complete here. Treating the
                        // pointee of CHandle<T> as a by-value dependency creates artificial cycles.
                        string normalized = SchemaDatabase.NormalizeTypeReference(argument.Text);
                        if (selection.Classes.ContainsKey(normalized) || selection.Enums.ContainsKey(normalized))
                        {
                            yield return normalized;
                        }
                    }
                }
            }
        }
    }

    private static void EmitOverrideAssertions(StringBuilder text, SchemaSelection selection)
    {
        foreach (SchemaClass type in selection.Classes.Values.Where(x => !x.Synthetic && IsHl2Override(x.Name)).OrderBy(x => x.Name, StringComparer.Ordinal))
        {
            text.Append("static_assert(sizeof(").Append(type.Name).Append(") == ").Append(type.Size).AppendLine(");");
            if (ValidAlignment(type.Alignment))
            {
                text.Append("static_assert(alignof(").Append(type.Name).Append(") == ").Append(type.Alignment).AppendLine(");");
            }
            foreach (SchemaField field in type.Fields.Where(x => x.Kind != SchemaFieldKind.Bitfield && x.Size >= 0))
            {
                text.Append("static_assert(offsetof(").Append(type.Name).Append(", ").Append(field.Name)
                    .Append(") == ").Append(field.Offset).AppendLine(");");
            }
        }
        foreach (SchemaEnum type in selection.Enums.Values.Where(x => IsHl2Override(x.Name)).OrderBy(x => x.Name, StringComparer.Ordinal))
        {
            text.Append("static_assert(sizeof(").Append(type.Name).Append(") == ").Append(type.Size).AppendLine(");");
            if (ValidAlignment(type.Alignment))
            {
                text.Append("static_assert(alignof(").Append(type.Name).Append(") == ").Append(type.Alignment).AppendLine(");");
            }
            foreach (SchemaEnumValue field in type.Fields)
            {
                text.AppendLine(EnumValueAssertion(type.Name, field));
            }
        }
    }

    private static void AddEnumValueAssertions(ICollection<string> assertions, SchemaEnum type)
    {
        foreach (SchemaEnumValue field in type.Fields)
        {
            assertions.Add(EnumValueAssertion(type.Name, field));
        }
    }

    private static string EnumValueAssertion(string typeName, SchemaEnumValue field)
        => field.IsUnsigned
            ? $"static_assert(static_cast<unsigned long long>({typeName}::{field.Name}) == {field.UnsignedValue.ToString(CultureInfo.InvariantCulture)}ULL);"
            : $"static_assert(static_cast<long long>({typeName}::{field.Name}) == {EnumLiteral(field)});";

    private static string EnumLiteral(SchemaEnumValue field)
        => field.IsUnsigned
            ? field.UnsignedValue.ToString(CultureInfo.InvariantCulture) + "ULL"
            : field.SignedValue == long.MinValue
                ? "(-9223372036854775807LL - 1LL)"
                : field.SignedValue.ToString(CultureInfo.InvariantCulture) + "LL";

    private static int AlignUp(int value, int alignment) => (value + alignment - 1) / alignment * alignment;

    private static IReadOnlyDictionary<string, int> ComputeClassAlignments(
        SchemaSelection selection, IReadOnlySet<string> polymorphic)
    {
        var result = new Dictionary<string, int>(StringComparer.Ordinal);
        var visiting = new HashSet<string>(StringComparer.Ordinal);

        int ClassAlignment(string name)
        {
            if (result.TryGetValue(name, out int cached)) return cached;
            if (!selection.Classes.TryGetValue(name, out SchemaClass? type) || type.Synthetic || !visiting.Add(name))
                return 1;
            int alignment = type.Alignment;
            if (!ValidAlignment(alignment))
            {
                alignment = DeriveAlignment(type);
                // A derived alignment that the schema size contradicts is not trustworthy.
                if (type.Size > 0 && type.Size % alignment != 0) alignment = 1;
            }
            visiting.Remove(name);
            result[name] = alignment;
            return alignment;
        }

        int DeriveAlignment(SchemaClass type)
        {
            int alignment = polymorphic.Contains(type.Name) ? 8 : 1;
            foreach (string baseName in type.BaseClasses) alignment = Math.Max(alignment, ClassAlignment(baseName));
            foreach (SchemaField field in type.Fields)
            {
                alignment = Math.Max(alignment, field switch
                {
                    { Kind: SchemaFieldKind.Bitfield } => 1,
                    _ when ValidAlignment(field.Alignment) => field.Alignment,
                    { Kind: SchemaFieldKind.Pointer } => 8,
                    { Kind: SchemaFieldKind.Reference or SchemaFieldKind.FixedArray } =>
                        ReferenceAlignment(SchemaDatabase.NormalizeTypeReference(field.TypeName)),
                    _ => 1,
                });
            }
            return alignment;
        }

        int ReferenceAlignment(string name)
            => selection.Enums.TryGetValue(name, out SchemaEnum? schemaEnum)
                ? ValidAlignment(schemaEnum.Alignment) ? schemaEnum.Alignment : 1
                : ClassAlignment(name);

        foreach (string name in selection.Classes.Keys) ClassAlignment(name);
        return result;
    }

    private static int FindClassSize(SchemaSelection selection, string name)
        => selection.Classes.TryGetValue(name, out SchemaClass? type) ? type.Size : 0;

    private static bool IsHl2Override(string name)
        => Hl2SdkCommonTypes.Contains(SchemaDatabase.NormalizeTypeReference(name));

    private static bool IsStdOverride(string name)
        => name is "std::pair" or "std::function" or "std::shared_mutex";

    // Attribute_t is an entity2-private map value that is intentionally absent from
    // the public HL2SDK. Keep the containing map's exact schema layout as an opaque
    // field instead of inventing a bogus public definition for the private value.
    private static bool RequiresOpaqueAtomic(SchemaField field)
        => field.TemplateArguments?.Any(x => !x.IsLiteral && x.Text == "Attribute_t") == true;

    private static string NormalizeCppType(string name)
        => SchemaDatabase.NormalizeTypeReference(name) switch
        {
            "int8" => "std::int8_t",
            "uint8" => "std::uint8_t",
            "int16" => "std::int16_t",
            "uint16" => "std::uint16_t",
            "int32" => "std::int32_t",
            "uint32" => "std::uint32_t",
            "int64" => "std::int64_t",
            "uint64" => "std::uint64_t",
            "float32" => "float",
            "float64" => "double",
            var value => value,
        };

    private static string EnumUnderlying(SchemaEnum value)
    {
        bool signed = value.Fields.Any(x => !x.IsUnsigned && x.SignedValue < 0);
        return (value.Size, signed) switch
        {
            (1, true) => "std::int8_t",
            (1, false) => "std::uint8_t",
            (2, true) => "std::int16_t",
            (2, false) => "std::uint16_t",
            (4, true) => "std::int32_t",
            (4, false) => "std::uint32_t",
            (8, true) => "std::int64_t",
            _ => "std::uint64_t",
        };
    }

    private static string TopLevelName(string name)
    {
        int separator = name.IndexOf("::", StringComparison.Ordinal);
        return separator < 0 ? name : name[..separator];
    }

    private static string LeafName(string name)
    {
        int separator = name.LastIndexOf("::", StringComparison.Ordinal);
        return separator < 0 ? name : name[(separator + 2)..];
    }

    private static bool ValidAlignment(int value) => value is 1 or 2 or 4 or 8 or 16;

    private sealed record OpaqueKey(string Spelling, int Size, int Alignment);
    private sealed record BitfieldUnit(int Capacity, IReadOnlyList<SchemaField> Fields);

    [GeneratedRegex(@"[^A-Za-z0-9_]+", RegexOptions.CultureInvariant)]
    private static partial Regex InvalidIdentifierChars();

    [GeneratedRegex(@"^[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.CultureInvariant)]
    private static partial Regex CppIdentifierRegex();

    [GeneratedRegex(@"[A-Za-z_][A-Za-z0-9_]*(?:::[A-Za-z_][A-Za-z0-9_]*)*", RegexOptions.CultureInvariant)]
    private static partial Regex TemplateIdentifierRegex();
}
