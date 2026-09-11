using System.Text;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

public sealed record ValveInterfaceImportResult(
    bool Applicable,
    int GlobalsFound = 0,
    int GlobalsRenamed = 0,
    int TypesApplied = 0,
    int VTablesImported = 0,
    int Skipped = 0,
    int ClangErrors = 0);

public sealed class ValveInterfaceImportException(string message, int clangErrors = 0) : Exception(message)
{
    public int ClangErrors { get; } = clangErrors;
}

public static unsafe class ValveInterfaceImport
{
    private const ulong BadAddr = ulong.MaxValue;
    private const int PtSilent = 0x00000001;
    private const int PtVariable = 0x00000008;
    private const int PtHigh = 0x00000080;
    private const int PrTypeMulti = 0x00001;
    private const int PrTypeDef = 0x00020;
    private const uint TinfoDefinite = 0x0001;
    private const int SnNoCheck = 0x01;
    private const int SnNonAuto = 0x40;
    private const int SegPermWrite = 2;
    private const int GtaIsForward = 5;
    private const int GtaUdtMemberCount = 16;
    private const int GtaUdtBits = 306;
    private const nuint TaudtVftable = 0x0100;
    private static readonly IReadOnlyDictionary<string, string[]> RequiredVTableMembers =
        new Dictionary<string, string[]>(StringComparer.Ordinal)
        {
            ["IVEngineServer2"] = ["IsPaused", "GetSteamUniverse", "ChangeLevel"],
        };

    public static ValveInterfaceImportResult Run(string binaryPath, string hl2SdkPath)
    {
        SchemaTargetPlatform platform;
        try
        {
            platform = SchemaImport.DetectTargetPlatform();
        }
        catch (SchemaImportException exception)
        {
            Console.Error.WriteLine($"[interfaces] {Path.GetFileName(binaryPath)}: {exception.Message} Skipped.");
            return new ValveInterfaceImportResult(false);
        }

        ValveInterfaceCatalog catalog = ValveInterfaceCatalog.Load(hl2SdkPath);
        IReadOnlyDictionary<ulong, string> strings = FindVersionStrings(catalog.Versions);
        if (strings.Count == 0)
        {
            Console.Error.WriteLine(
                $"[interfaces] {Path.GetFileName(binaryPath)}: no HL2SDK interface version strings; skipped.");
            return new ValveInterfaceImportResult(false);
        }

        IReadOnlyList<ValveInterfaceTable> tables = ValveInterfaceTableDetector.Detect(strings, new IdaMemory());
        if (tables.Count == 0)
        {
            Console.Error.WriteLine(
                $"[interfaces] {Path.GetFileName(binaryPath)}: no structurally valid InterfaceGlobals_t table; skipped.");
            return new ValveInterfaceImportResult(false);
        }

        ValveInterfaceBinding[] allBindings = tables
            .SelectMany(table => ValveInterfaceTableResolver.Resolve(table, catalog))
            .ToArray();
        int found = allBindings.Select(x => x.Row.GlobalAddress).Distinct().Count();
        var diagnostics = new LimitedDiagnostics(32);
        int commentFailures = 0;
        // Table validation is sufficient to annotate a slot, even if its official
        // name is ambiguous or an existing user name prevents renaming it.
        foreach (var group in allBindings.GroupBy(x => x.Row.GlobalAddress))
        {
            if (!AnnotateInterfacePointer(group.Key, group.Select(x => x.Row.Version)))
            {
                diagnostics.Write($"[interfaces] 0x{group.Key:X}: IDA rejected interface comment.");
                commentFailures++;
            }
        }
        IReadOnlyList<ValveInterfaceBinding> bindings = ResolveUnambiguousBindings(allBindings, diagnostics);
        ValveInterfaceDefinition[] definitions = bindings.Select(x => x.Definition!)
            .Distinct()
            .ToArray();

        InterfaceTypeImportSummary typeImport = ImportTypes(hl2SdkPath, platform, definitions, diagnostics);

        int renamed = 0;
        int typed = 0;
        int skipped = found - bindings.Count + typeImport.Skipped + commentFailures;
        foreach (ValveInterfaceBinding binding in bindings)
        {
            ValveInterfaceDefinition definition = binding.Definition!;
            ulong address = binding.Row.GlobalAddress;

            string currentName = GetName(address);
            if (!currentName.Equals(definition.GlobalName, StringComparison.Ordinal))
            {
                if (!ValveInterfaceNaming.CanReplace(currentName))
                {
                    diagnostics.Write(
                        $"[interfaces] 0x{address:X}: preserving explicit name '{currentName}'; wanted '{definition.GlobalName}'.");
                    skipped++;
                    continue;
                }

                ulong owner = GetNameAddress(definition.GlobalName);
                if (owner != BadAddr && owner != address)
                {
                    diagnostics.Write(
                        $"[interfaces] 0x{address:X}: name '{definition.GlobalName}' is already used at 0x{owner:X}; skipped.");
                    skipped++;
                    continue;
                }

                if (!SetName(address, definition.GlobalName))
                {
                    diagnostics.Write(
                        $"[interfaces] 0x{address:X}: IDA rejected name '{definition.GlobalName}'; skipped.");
                    skipped++;
                    continue;
                }
                renamed++;
            }

            string appliedClass = typeImport.ImplementationClasses.GetValueOrDefault(
                definition.ClassName, definition.ClassName);
            TypeApplication application = ApplyPointerType(address, appliedClass);
            if (application == TypeApplication.Applied)
            {
                typed++;
            }
            else if (application == TypeApplication.Unavailable)
            {
                diagnostics.Write(
                    $"[interfaces] 0x{address:X} {definition.GlobalName}: type '{appliedClass} *' is unavailable.");
                skipped++;
            }
        }

        diagnostics.Finish();
        Console.Error.WriteLine(
            $"[interfaces] {Path.GetFileName(binaryPath)}: tables={tables.Count}, found={found}, " +
            $"renamed={renamed}, typed={typed}, vtables={typeImport.VTablesImported}, " +
            $"skipped={skipped}, clang-errors={typeImport.ClangErrors}.");
        return new ValveInterfaceImportResult(true, found, renamed, typed, typeImport.VTablesImported,
            skipped, typeImport.ClangErrors);
    }

    private static IReadOnlyList<ValveInterfaceBinding> ResolveUnambiguousBindings(
        IReadOnlyList<ValveInterfaceBinding> allBindings,
        LimitedDiagnostics diagnostics)
    {
        var candidates = new List<ValveInterfaceBinding>();
        foreach (IGrouping<ulong, ValveInterfaceBinding> addressGroup in
                 allBindings.GroupBy(x => x.Row.GlobalAddress))
        {
            ValveInterfaceDefinition[] definitions = addressGroup
                .Where(x => x.Definition != null)
                .Select(x => x.Definition!)
                .Distinct()
                .ToArray();
            if (definitions.Length != 1)
            {
                string reason = definitions.Length == 0 ? "no unique official mapping" : "conflicting official mappings";
                string versions = string.Join(", ", addressGroup.Select(x => x.Row.Version).Distinct(StringComparer.Ordinal));
                diagnostics.Write($"[interfaces] 0x{addressGroup.Key:X} ({versions}): {reason}; skipped.");
                continue;
            }
            ValveInterfaceBinding binding = addressGroup.First(x => Equals(x.Definition, definitions[0]));
            candidates.Add(binding);
        }

        var ambiguousNames = candidates
            .GroupBy(x => x.Definition!.GlobalName, StringComparer.Ordinal)
            .Where(group => group.Select(x => x.Row.GlobalAddress).Distinct().Count() > 1)
            .Select(group => group.Key)
            .ToHashSet(StringComparer.Ordinal);
        foreach (string name in ambiguousNames.Order(StringComparer.Ordinal))
        {
            diagnostics.Write($"[interfaces] official name '{name}' maps to multiple storage addresses; all skipped.");
        }
        return candidates.Where(x => !ambiguousNames.Contains(x.Definition!.GlobalName)).ToArray();
    }

    private static InterfaceTypeImportSummary ImportTypes(
        string hl2SdkPath,
        SchemaTargetPlatform platform,
        IReadOnlyList<ValveInterfaceDefinition> definitions,
        LimitedDiagnostics diagnostics)
    {
        if (definitions.Count == 0)
        {
            return new InterfaceTypeImportSummary(0, 0, 0);
        }

        string tempDirectory = Path.Combine(Path.GetTempPath(), $"s2atelier-interfaces-{Guid.NewGuid():N}");
        bool keepDirectory = false;
        int errors = 0;
        int skipped = 0;
        var fullCandidates = new HashSet<string>(StringComparer.Ordinal);
        try
        {
            Directory.CreateDirectory(tempDirectory);
            // Current HL2SDK snapshots reference this generated protobuf header from eiface.h,
            // but the interface declarations only use separately forward-declared message types.
            File.WriteAllText(Path.Combine(tempDirectory, "network_connection.pb.h"),
                "#pragma once\n", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            SchemaImport.ConfigureClang(hl2SdkPath, platform, skipLayoutAssertions: true, tempDirectory);

            int groupIndex = 0;
            foreach (IGrouping<string, ValveInterfaceDefinition> group in definitions
                         .Where(x => x.DefinitionHeader != null)
                         .GroupBy(x => x.DefinitionHeader!, StringComparer.OrdinalIgnoreCase))
            {
                string path = Path.Combine(tempDirectory, $"full-{groupIndex++:D3}.hpp");
                WriteHeader(path, [group.Key], group, includeImplementations: true);
                int preflight = SchemaImport.ParseHeader(path, testOnly: true, printDiagnostics: true);
                errors += preflight;
                if (preflight != 0)
                {
                    keepDirectory = true;
                    skipped += group.Select(x => x.ClassName).Distinct(StringComparer.Ordinal).Count();
                    diagnostics.Write(
                        $"[interfaces] IDAClang rejected '{group.Key}' with {preflight} preflight error(s); using forward types only.");
                    continue;
                }

                string[] replaced = group.SelectMany(x => new[] { x.ClassName, VTableName(x.ClassName) })
                    .Concat(group.Select(ValveInterfaceImplementations.Find).OfType<ValveInterfaceImplementation>()
                        .SelectMany(x => new[] { x.ClassName, VTableName(x.ClassName) }))
                    .Distinct(StringComparer.Ordinal)
                    .ToArray();
                SchemaImport.DeleteReplacedTypes(replaced);
                int imported = SchemaImport.ParseHeader(path, testOnly: false, printDiagnostics: true);
                errors += imported;
                if (imported != 0)
                {
                    keepDirectory = true;
                    skipped += group.Select(x => x.ClassName).Distinct(StringComparer.Ordinal).Count();
                    SchemaImport.DeleteReplacedTypes(replaced);
                    diagnostics.Write(
                        $"[interfaces] IDAClang reported {imported} import error(s) for '{group.Key}'; vtable validation will reject incomplete output.");
                    continue;
                }
                fullCandidates.UnionWith(group.Select(x => x.ClassName));
            }

            ValveInterfaceDefinition[] missingDefinitions = definitions
                .Where(x => !TypeExists(x.ClassName))
                .DistinctBy(x => x.ClassName, StringComparer.Ordinal)
                .ToArray();
            if (missingDefinitions.Length > 0)
            {
                string forwardPath = Path.Combine(tempDirectory, "forwards.hpp");
                WriteHeader(forwardPath, ["public/interfaces/interfaces.h"], missingDefinitions);
                int forwardPreflight = SchemaImport.ParseHeader(forwardPath, testOnly: true, printDiagnostics: true);
                errors += forwardPreflight;
                if (forwardPreflight == 0)
                {
                    SchemaImport.DeleteReplacedTypes(missingDefinitions.Select(x => x.ClassName));
                    int forwardImport = SchemaImport.ParseHeader(forwardPath, testOnly: false, printDiagnostics: true);
                    errors += forwardImport;
                    keepDirectory |= forwardImport != 0;
                    skipped += forwardImport == 0 ? 0 : missingDefinitions.Length;
                }
                else
                {
                    keepDirectory = true;
                    skipped += missingDefinitions.Length;
                    diagnostics.Write(
                        $"[interfaces] IDAClang rejected the interface forward declarations with {forwardPreflight} error(s).");
                }
            }

            int vtables = 0;
            foreach (string className in fullCandidates)
            {
                if (HasValidVTable(className))
                {
                    vtables++;
                }
                else
                {
                    SchemaImport.DeleteReplacedTypes([VTableName(className)]);
                    skipped++;
                    diagnostics.Write(
                        $"[interfaces] '{VTableName(className)}' has no valid VFT-marked members; discarded.");
                }
            }
            var implementationClasses = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (ValveInterfaceImplementation mapping in definitions
                         .Where(x => fullCandidates.Contains(x.ClassName))
                         .Select(ValveInterfaceImplementations.Find).OfType<ValveInterfaceImplementation>().Distinct())
            {
                // IDAClang merges inherited slots into Class_vtbl and exposes the
                // corresponding synthetic __vftable member on the derived class.
                // Never rewrite a shared IAppSystem base or a secondary vfptr.
                if (ValveImplementationTypes.Validate(mapping, out string reason))
                {
                    implementationClasses[mapping.InterfaceClass] = mapping.ClassName;
                    diagnostics.Write($"[interfaces] {mapping.InterfaceClass} -> {mapping.ClassName}: " +
                        "implementation layout and primary vftable validated.");
                }
                else
                {
                    skipped++;
                    diagnostics.Write($"[interfaces] {mapping.InterfaceClass} -> {mapping.ClassName}: " +
                        $"{reason}; keeping interface pointer type.");
                }
            }
            return new InterfaceTypeImportSummary(vtables, skipped, errors)
            {
                ImplementationClasses = implementationClasses,
            };
        }
        finally
        {
            if (!keepDirectory)
            {
                try
                {
                    Directory.Delete(tempDirectory, recursive: true);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
            else
            {
                Console.Error.WriteLine($"[interfaces] generated headers retained for diagnosis: {tempDirectory}");
            }
        }
    }

    internal static void WriteHeader(
        string path,
        IEnumerable<string> includes,
        IEnumerable<ValveInterfaceDefinition> definitions,
        bool includeImplementations = false)
    {
        var output = new StringBuilder();
        output.AppendLine("// Generated by S2Atelier Valve interface import.");
        output.AppendLine("#pragma once");
        // Declare SDK pointer-only dependencies in the translation unit before
        // IDAClang can resolve their names from previously imported Local Types.
        output.AppendLine("struct InputContextHandle_t__;");
        // protobuf enums use a 32-bit integer ABI; only the type is needed here.
        output.AppendLine("typedef int ENetworkDisconnectionReason;");
        // The generated CCLCMsg_Move definition is absent from this SDK snapshot.
        // Keep its wrapper opaque: instantiating CNetMessagePB would require the
        // protobuf base layout, while the interface only takes a const reference.
        output.AppendLine("class CCLCMsg_Move;");
        output.AppendLine("template <typename T> class CNetMessagePB;");
        output.AppendLine("template <> class CNetMessagePB<CCLCMsg_Move>;");
        foreach (string include in includes.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            output.Append("#include \"").Append(include.Replace('\\', '/')).AppendLine("\"");
        }
        int index = 0;
        foreach (ValveInterfaceDefinition definition in definitions.Distinct())
        {
            output.Append("extern ").Append(definition.ClassName).Append(" *__s2atelier_interface_")
                .Append(index++.ToString("D3")).AppendLine(";");
            if (includeImplementations && ValveInterfaceImplementations.Find(definition) is { } implementation)
            {
                output.Append("extern ").Append(implementation.ClassName).Append(" *__s2atelier_implementation_")
                    .Append(index++.ToString("D3")).AppendLine(";");
                output.Append("static_assert(sizeof(").Append(implementation.ClassName)
                    .Append(") >= sizeof(").Append(definition.ClassName).AppendLine("));");
            }
        }
        File.WriteAllText(path, output.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static IReadOnlyDictionary<ulong, string> FindVersionStrings(IReadOnlySet<string> versions)
    {
        var result = new Dictionary<ulong, string>();
        nuint total = IdaNative.get_strlist_qty();
        byte* item = stackalloc byte[32];
        for (nuint i = 0; i < total; i++)
        {
            if (IdaNative.get_strlist_item(item, i) == 0)
            {
                continue;
            }
            ulong address = *(ulong*)item;
            int length = *(int*)(item + 8);
            int type = *(int*)(item + 12);
            string value = ReadString(address, length, type);
            if (versions.Contains(value))
            {
                result[address] = value;
            }
        }
        return result;
    }

    private static string ReadString(ulong address, int length, int type)
    {
        var value = new QString();
        try
        {
            nint written = IdaNative.get_strlit_contents(
                &value, address, length < 0 ? nuint.MaxValue : (nuint)length, type, null, 0);
            return written > 0 ? value.Read() : string.Empty;
        }
        finally
        {
            value.Dispose();
        }
    }

    private static bool AnnotateInterfacePointer(ulong address, IEnumerable<string> versions)
    {
        var previous = new QString();
        try
        {
            IdaNative.get_cmt(&previous, address, 1);
            string existing = previous.Read();
            string merged = ValveInterfaceComments.Merge(existing, versions);
            if (merged == existing) return true;
            byte* comment = Utf8.Allocate(merged);
            try { return IdaNative.set_cmt(address, comment, 1) != 0; }
            finally { Utf8.Free(comment); }
        }
        finally { previous.Dispose(); }
    }

    private static TypeApplication ApplyPointerType(ulong address, string className)
    {
        TypeInfo wanted = default;
        var parsedName = new QString();
        byte* declaration = Utf8.Allocate($"{className} *__s2atelier_global;");
        try
        {
            if (IdaNative.parse_decl(&wanted, &parsedName, IdaNative.get_idati(), declaration,
                    PtSilent | PtVariable | PtHigh) == 0)
            {
                return TypeApplication.Unavailable;
            }

            // Confirmed interface slots always receive the current SDK pointer type,
            // including slots carrying an explicit type from an earlier import.
            return IdaNative.apply_tinfo(address, &wanted, TinfoDefinite) != 0
                ? TypeApplication.Applied
                : TypeApplication.Unavailable;
        }
        finally
        {
            Utf8.Free(declaration);
            parsedName.Dispose();
            wanted.Dispose();
        }
    }

    private static bool TypeExists(string name)
    {
        byte* native = Utf8.Allocate(name);
        try
        {
            return IdaNative.get_named_type_tid(native) != BadAddr;
        }
        finally
        {
            Utf8.Free(native);
        }
    }

    private static bool HasValidVTable(string className)
    {
        byte* native = Utf8.Allocate(VTableName(className));
        TypeInfo type = default;
        try
        {
            ulong tid = IdaNative.get_named_type_tid(native);
            if (tid == BadAddr || IdaNative.get_type_by_tid(&type, tid) == 0)
            {
                return false;
            }
            if (IdaNative.get_tinfo_property(type.Typid, GtaIsForward) != 0 ||
                IdaNative.get_tinfo_property(type.Typid, GtaUdtMemberCount) == 0 ||
                (IdaNative.get_tinfo_property(type.Typid, GtaUdtBits) & TaudtVftable) == 0)
            {
                return false;
            }
            if (!RequiredVTableMembers.TryGetValue(className, out string[]? required))
            {
                return true;
            }

            var printed = new QString();
            try
            {
                if (IdaNative.print_tinfo(
                        &printed, null, 0, 0, PrTypeMulti | PrTypeDef, &type, native, null) == 0)
                {
                    return false;
                }
                string declaration = printed.Read();
                return required.All(member => declaration.Contains(member, StringComparison.Ordinal));
            }
            finally
            {
                printed.Dispose();
            }
        }
        finally
        {
            type.Dispose();
            Utf8.Free(native);
        }
    }

    private static string VTableName(string className)
    {
        int separator = className.LastIndexOf("::", StringComparison.Ordinal);
        return separator < 0
            ? className + "_vtbl"
            : className[..(separator + 2)] + className[(separator + 2)..] + "_vtbl";
    }

    private static string GetName(ulong address)
    {
        var name = new QString();
        try
        {
            return IdaNative.get_ea_name(&name, address, 0, null) > 0 ? name.Read() : string.Empty;
        }
        finally
        {
            name.Dispose();
        }
    }

    private static ulong GetNameAddress(string name)
    {
        byte* native = Utf8.Allocate(name);
        try
        {
            return IdaNative.get_name_ea(BadAddr, native);
        }
        finally
        {
            Utf8.Free(native);
        }
    }

    private static bool SetName(ulong address, string name)
    {
        byte* native = Utf8.Allocate(name);
        try
        {
            return IdaNative.set_name(address, native, SnNoCheck | SnNonAuto) != 0;
        }
        finally
        {
            Utf8.Free(native);
        }
    }

    private enum TypeApplication
    {
        Applied,
        Unavailable,
    }

    private sealed record InterfaceTypeImportSummary(int VTablesImported, int Skipped, int ClangErrors)
    {
        public IReadOnlyDictionary<string, string> ImplementationClasses { get; init; }
            = new Dictionary<string, string>();
    }

    private sealed class IdaMemory : IValveInterfaceMemory
    {
        public ulong ReadPointer(ulong address) => IdaNative.get_qword(address);

        public bool IsMapped(ulong address) => IdaNative.is_mapped(address) != 0;

        public bool IsWritable(ulong address)
        {
            void* segment = IdaNative.getseg(address);
            return segment != null && ((((byte*)segment)[42] & SegPermWrite) != 0);
        }

        public IEnumerable<ulong> DataReferencesTo(ulong address) => Xrefs.DataTo(address);
    }

    private sealed class LimitedDiagnostics(int limit)
    {
        private int _seen;

        public void Write(string message)
        {
            if (_seen++ < limit)
            {
                Console.Error.WriteLine(message);
            }
        }

        public void Finish()
        {
            int suppressed = _seen - limit;
            if (suppressed > 0)
            {
                Console.Error.WriteLine($"[interfaces] suppressed {suppressed} additional diagnostic(s).");
            }
        }
    }
}
