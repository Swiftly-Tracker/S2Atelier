using System.Diagnostics;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida;

public sealed record IdaAnalysisResult(
    int Functions, int Segments, int Strings, TimeSpan Elapsed,
    bool PltPatchApplicable = false, int PltPatched = 0, int PltUnresolved = 0,
    bool ConVarNamingApplicable = false, int ConVarNamingFound = 0,
    int ConVarNamingRenamedObjects = 0, int ConVarNamingRenamedHandlers = 0, int ConVarNamingTypedObjects = 0,
    int FnPtrNamingFound = 0, int FnPtrNamingRenamed = 0,
    bool LogChannelNamingApplicable = false, int LogChannelNamingFound = 0, int LogChannelNamingRenamed = 0,
    bool ProtoImportApplicable = false, int ProtoTypesDefined = 0, int ProtoImportErrors = 0,
    bool InterfaceImportApplicable = false, int InterfaceGlobalsFound = 0,
    int InterfaceGlobalsRenamed = 0, int InterfaceTypesApplied = 0,
    int InterfaceVTablesImported = 0, int InterfaceImportSkipped = 0, int InterfaceClangErrors = 0,
    bool SchemaImportApplicable = false, string? SchemaProject = null, int SchemaTypesImported = 0,
    int SchemaVTablesMatched = 0, int SchemaFunctionsBound = 0, int SchemaFunctionsSkipped = 0,
    int SchemaFunctionConflicts = 0, int SchemaClangErrors = 0,
    int SchemaVTableTypesCompleted = 0, int SchemaVTableAddressesBound = 0, int SchemaVTableUnknownSlots = 0, int SchemaVTableConflicts = 0);

public static unsafe class IdaKernel
{
    private static bool _initialized;
    private static int _ownerThread;

    public static bool IsInitialized => _initialized;

    public static IdaSdkVersion SdkVersion { get; private set; }

    public static bool TryInitialize(string idaPath, IdaSdkVersion requested, out string? error)
    {
        error = null;

        if (_initialized)
        {
            return true;
        }

        if (!Directory.Exists(idaPath))
        {
            error = $"'{idaPath}' does not exist. Point --ida-path at an IDA installation.";
            return false;
        }

        string kernelFile = IdaPlatform.LibraryFileName("ida");
        string idalibFile = IdaPlatform.LibraryFileName("idalib");

        if (!File.Exists(Path.Combine(idaPath, kernelFile)) || !File.Exists(Path.Combine(idaPath, idalibFile)))
        {
            error = $"'{kernelFile}' and '{idalibFile}' were not both found in '{idaPath}'.";
            return false;
        }

        foreach (string required in new[] { "cfg", "procs" })
        {
            if (!Directory.Exists(Path.Combine(idaPath, required)))
            {
                error = $"'{idaPath}' has the IDA libraries but no '{required}' directory, so it " +
                        "is not a complete IDA installation.";
                return false;
            }
        }

        IdaPlatform.PreloadSiblingLibraries(idaPath);
        IdaResolver.SetRoot(idaPath);

        nint idaHandle = IdaResolver.Load("ida");
        nint idalibHandle = IdaResolver.Load("idalib");

        if (!IdaBootstrap.TryInitLibrary(idalibHandle, out int status))
        {
            error = $"'{idalibFile}' does not export init_library(); this is not an idalib build.";
            return false;
        }

        if (status != 0)
        {
            error = $"init_library() failed with status {status}. '{idaPath}' must have a valid licence.";
            return false;
        }

        if (!TrySelectVersion(idalibHandle, requested, out var selected, out error))
        {
            return false;
        }

        IdaNative.BindAll(idaHandle, idalibHandle, selected);
        IdaNative.enable_console_messages(0);

        SdkVersion = selected;
        _ownerThread = Environment.CurrentManagedThreadId;
        _initialized = true;
        return true;
    }

    private static bool TrySelectVersion(
        nint idalib, IdaSdkVersion requested, out IdaSdkVersion selected, out string? error)
    {
        error = null;

        if (requested != IdaSdkVersion.Auto)
        {
            selected = requested;

            if (!IdaNative.GeneratedVersions.Contains(requested))
            {
                error = $"--ida-sdk {requested} was requested, but this build only has bindings for {Describe()}.";
                return false;
            }

            return true;
        }

        selected = IdaSdkVersion.Auto;

        if (!IdaBootstrap.TryProbeVersion(idalib, out var version))
        {
            error = "The installed idalib does not report a version through get_library_version().";
            return false;
        }

        selected = IdaBootstrap.ToSdkVersion(version);

        if (selected == IdaSdkVersion.Auto || !IdaNative.GeneratedVersions.Contains(selected))
        {
            error = $"IDA {version.Major}.{version.Minor} is installed, but this build only has " +
                    $"bindings for {Describe()}. Vendor that SDK under thirdparty/ida-sdk and " +
                    "re-run S2Atelier.Ida.Codegen, or pass --ida-sdk explicitly.";
            return false;
        }

        return true;
    }

    private static string Describe()
        => string.Join(", ", IdaNative.GeneratedVersions.Select(v => v.ToString()));

    private static readonly TimeSpan ReportInterval = TimeSpan.FromMilliseconds(60);

    public static IdaAnalysisResult Open(
        string path, bool save, bool patchPlt, bool nameConVars, bool nameFnPtrTables,
        string? importProtobufsDir, Action<double, ulong>? onProgress)
        => Open(path, save, patchPlt, nameConVars, nameFnPtrTables, importProtobufsDir,
            importSchemaPath: null, hl2SdkPath: null, schemaProject: "auto", onProgress: onProgress,
            importInterfaces: false);

    public static IdaAnalysisResult Open(
        string path, bool save, bool patchPlt = false, bool nameConVars = false, bool nameFnPtrTables = false,
        string? importProtobufsDir = null, string? importSchemaPath = null, string? hl2SdkPath = null,
        string schemaProject = "auto", Action<double, ulong>? onProgress = null, bool importInterfaces = false,
        Action<string>? onStage = null, string? convarTypesPath = null, bool nameLogChannels = false,
        string? vtableBaselineDirectory = null, string? vtableSnapshotDirectory = null, bool nameEntityClasses = false,
        bool typeGlobals = false)
    {
        AssertOwner();

        string full = Path.GetFullPath(path);
        byte* native = Utf8.Allocate(full);
        var clock = Stopwatch.StartNew();

        try
        {
            int status = IdaNative.open_database(native, 0, null);
            if (status != 0)
            {
                throw new InvalidOperationException($"open_database() failed with status {status} for '{full}'.");
            }
        }
        finally
        {
            Utf8.Free(native);
        }

        bool completed = false;
        try
        {
            bool runInterfaces = importInterfaces && hl2SdkPath != null;
            bool runSchema = importSchemaPath != null && hl2SdkPath != null;
            bool runProtobufs = !string.IsNullOrEmpty(importProtobufsDir);
            int passCount = new[]
                    { runInterfaces, runSchema, runInterfaces || runSchema, patchPlt, nameConVars, nameLogChannels,
                      nameFnPtrTables, runProtobufs, nameEntityClasses, typeGlobals }
                .Count(x => x);
            // Auto-analysis is only part of the job: the later passes can take minutes on large
            // binaries, so they share the rest of the bar instead of leaving it at 100%.
            double analysisShare = passCount == 0 ? 1.0 : 0.5;
            int passIndex = 0;
            void BeginPass(string stage)
            {
                onStage?.Invoke(stage);
                onProgress?.Invoke(analysisShare + (1.0 - analysisShare) * passIndex++ / passCount, 0);
            }

            onStage?.Invoke("auto-analysis");
            DriveAnalysis(onProgress == null ? null : (fraction, address) => onProgress(fraction * analysisShare, address));
            IdaNative.build_strlist();

            if (runInterfaces) BeginPass("interfaces");
            var interfaceResult = runInterfaces
                ? ValveInterfaceImport.Run(full, hl2SdkPath!)
                : new ValveInterfaceImportResult(false);

            // Both the schema pass and the interface step name slots from hl2sdk; one gate holds a drifted SDK
            // class back in both and records the module's snapshot.
            string module = Path.GetFileName(full);
            module = module.EndsWith(".i64", StringComparison.OrdinalIgnoreCase) ? module[..^4] : module;
            VTableDrift? drift = vtableBaselineDirectory == null && vtableSnapshotDirectory == null
                ? null
                : new VTableDrift(module, vtableBaselineDirectory == null
                    ? null
                    : Path.Combine(vtableBaselineDirectory, VTableDrift.SnapshotName(module)));
            var health = new ModuleHealth(module);
            if (runInterfaces)
            {
                health.Count("interfaces.globals", interfaceResult.GlobalsRenamed);
                health.Count("interfaces.vtables", interfaceResult.VTablesImported);
            }

            if (runSchema) BeginPass("schema");
            var schemaResult = runSchema
                ? SchemaImport.Run(full, importSchemaPath!, hl2SdkPath!, schemaProject, drift)
                : new SchemaImportResult(false);
            if (schemaResult.Applicable)
            {
                health.Count("schema.types", schemaResult.TypesImported);
                health.Count("schema.vtables", schemaResult.VTablesMatched);
                health.Count("schema.functions-bound", schemaResult.FunctionsBound);
                health.Warn("schema-layout", schemaResult.LayoutMismatches ?? []);
            }

            if (runInterfaces || runSchema)
            {
                // Classes that implement SDK interfaces, whether or not the module has schema classes.
                BeginPass("interface vtables");
                SdkInterfaceSummary implementations = SdkInterfaceBinding.Run(hl2SdkPath!, SchemaImport.DetectTargetPlatform(),
                    schemaResult.SchemaClasses ?? new HashSet<string>(), drift, Console.Error.WriteLine);
                Console.Error.WriteLine($"[interface-vtables] {Path.GetFileName(full)}: tables={implementations.Tables}, " +
                    $"bound={implementations.Binding.Bound}, skipped={implementations.Binding.Skipped}, " +
                    $"conflicts={implementations.Binding.Conflicts}.");
                foreach (string held in drift?.Held ?? [])
                {
                    Console.Error.WriteLine($"[vtable-drift] {held}");
                }

                health.Count("interface-vtables.tables", implementations.Tables);
                health.Count("interface-vtables.bound", implementations.Binding.Bound);
                health.Warn("vtable-drift", drift?.Held ?? []);

                if (drift != null && vtableSnapshotDirectory != null)
                {
                    drift.Write(Path.Combine(vtableSnapshotDirectory, VTableDrift.SnapshotName(module)));
                }

                // Whatever the SDK does not name keeps a slot name of its own, in every class's vtable.
                var namedSlots = VTableSlotNaming.Run(SchemaImport.DetectTargetPlatform(), Console.Error.WriteLine);
                Console.Error.WriteLine($"[vtable-slots] {Path.GetFileName(full)}: tables={namedSlots.Tables}, " +
                    $"named={namedSlots.Named}, ambiguous={namedSlots.Ambiguous}.");
                health.Count("vtable-slots.tables", namedSlots.Tables);

                Console.Error.WriteLine($"[types] {TemplateAliases.Run()} template alias(es) created.");
                // The imports parse with IDAClang; the later passes, like the GUI, use the legacy parser and
                // reach template instantiations through the aliases.
                SchemaImport.ResetParser();
            }

            if (nameEntityClasses)
            {
                BeginPass("entity classes");
                var entities = EntityClassNaming.Apply(hl2SdkPath, SchemaImport.DetectTargetPlatform(),
                    vtableSnapshotDirectory == null ? null : Path.Combine(vtableSnapshotDirectory, EntityClassNaming.GraphName(module)),
                    Console.Error.WriteLine);
                Console.Error.WriteLine($"[entity-classes] {Path.GetFileName(full)}: layout={entities.Layout}, " +
                    $"classes={entities.ClassesFound}, abstract-infos={entities.AbstractInfos}, named={entities.Named}, " +
                    $"typed={entities.Typed}, infos-named={entities.InfosNamed}, schema-bindings={entities.SchemaBindingsNamed}, " +
                    $"data-maps={entities.DataMapsNamed}, skipped={entities.Skipped}, " +
                    $"round-trip-failures={entities.RoundTripFailures}, dangling-bases={entities.DanglingBases}.");
                health.Count("entity-classes.classes", entities.ClassesFound);
                health.Count("entity-classes.typed", entities.Typed);
                health.Count("entity-classes.schema-bindings", entities.SchemaBindingsNamed);
                health.Count("entity-classes.data-maps", entities.DataMapsNamed);
                var entityWarnings = new List<string>();
                if (entities.Layout.StartsWith("drift", StringComparison.Ordinal)) entityWarnings.Add(entities.Layout);
                if (entities.RoundTripFailures > 0) entityWarnings.Add($"{entities.RoundTripFailures} round-trip failure(s)");
                if (entities.DanglingBases > 0) entityWarnings.Add($"{entities.DanglingBases} base info(s) that are no class info");
                health.Warn("entity-classes", entityWarnings);
            }

            if (typeGlobals)
            {
                BeginPass("global vars");
                var globals = GlobalVarsTyping.Apply(module, hl2SdkPath, SchemaImport.DetectTargetPlatform(),
                    Console.Error.WriteLine);
                static string Hex(ulong? value) => value is ulong v ? $"0x{v:X}" : "-";
                Console.Error.WriteLine($"[globals] {Path.GetFileName(full)}: era={globals.Era}, types={globals.Types}, " +
                    $"gpGlobals={Hex(globals.GpGlobals)}, default={Hex(globals.FallbackGlobals)}, " +
                    $"warning-func={Hex(globals.WarningFunc)}, client-offset={Hex(globals.ClientOffset)}, " +
                    $"server-offset={Hex(globals.ServerOffset)}, time-scope-helpers={globals.Helpers}, skipped={globals.Skipped}.");
                foreach (string warning in globals.Warnings)
                {
                    Console.Error.WriteLine($"[globals] {warning}");
                }

                health.Count("globals.found", (globals.GpGlobals != null ? 1 : 0) + (globals.ClientOffset != null ? 1 : 0) +
                                              (globals.ServerOffset != null ? 1 : 0));
                health.Count("globals.time-scope-helpers", globals.Helpers);
                health.Warn("globals", globals.Warnings);
            }

            if (patchPlt) BeginPass("plt");
            var pltResult = patchPlt
                ? PltPatcher.Run()
                : new PltPatchResult(false, 0, 0);

            if (nameConVars) BeginPass("convars");
            var s2fResult = nameConVars
                ? ConVarNaming.Run(convarTypesPath == null ? null : ConVarNaming.LoadDumpedTypes(convarTypesPath))
                : new ConVarNamingResult(false, 0, 0, 0, 0, 0);

            if (nameLogChannels) BeginPass("log channels");
            var logResult = nameLogChannels
                ? LogChannelNaming.Run()
                : new LogChannelNamingResult(false, 0, 0);

            if (nameFnPtrTables) BeginPass("fnptr tables");
            var fnPtrResult = nameFnPtrTables
                ? FnPtrNaming.Run()
                : new FnPtrNamingResult(false, 0, 0);

            if (runProtobufs) BeginPass("protobufs");
            var protoResult = runProtobufs
                ? ProtoImport.Run(importProtobufsDir!)
                : new ProtoImportResult(false, 0, 0, 0);

            if (pltResult.Applicable) health.Count("plt.patched", pltResult.Patched);
            if (s2fResult.Applicable)
            {
                health.Count("convars.found", s2fResult.Found);
                health.Count("convars.typed", s2fResult.TypedObjects);
            }

            if (logResult.Applicable) health.Count("log-channels.found", logResult.Found);
            if (nameFnPtrTables) health.Count("fnptr-tables.found", fnPtrResult.Found);
            if (protoResult.Applicable) health.Count("protobufs.types", protoResult.TypesDefined);
            if (vtableSnapshotDirectory != null)
            {
                var baseline = vtableBaselineDirectory == null
                    ? null
                    : ModuleHealth.Load(Path.Combine(vtableBaselineDirectory, ModuleHealth.FileName(module)));
                var report = health.Write(Path.Combine(vtableSnapshotDirectory, ModuleHealth.FileName(module)), baseline);
                foreach (string regression in report.Regressions)
                {
                    Console.Error.WriteLine($"[health] {Path.GetFileName(full)}: fell since the baseline: {regression}");
                }
            }

            onStage?.Invoke(save ? "saving" : "closing");
            onProgress?.Invoke(1.0, 0);

            var result = new IdaAnalysisResult(
                Functions: (int)IdaNative.get_func_qty(),
                Segments: IdaNative.get_segm_qty(),
                Strings: (int)IdaNative.get_strlist_qty(),
                Elapsed: clock.Elapsed,
                PltPatchApplicable: pltResult.Applicable,
                PltPatched: pltResult.Patched,
                PltUnresolved: pltResult.Unresolved,
                ConVarNamingApplicable: s2fResult.Applicable,
                ConVarNamingFound: s2fResult.Found,
                ConVarNamingRenamedObjects: s2fResult.RenamedObjects,
                ConVarNamingRenamedHandlers: s2fResult.RenamedHandlers,
                ConVarNamingTypedObjects: s2fResult.TypedObjects,
                FnPtrNamingFound: fnPtrResult.Found,
                FnPtrNamingRenamed: fnPtrResult.Renamed,
                LogChannelNamingApplicable: logResult.Applicable,
                LogChannelNamingFound: logResult.Found,
                LogChannelNamingRenamed: logResult.Renamed,
                ProtoImportApplicable: protoResult.Applicable,
                ProtoTypesDefined: protoResult.TypesDefined,
                ProtoImportErrors: protoResult.Errors,
                InterfaceImportApplicable: interfaceResult.Applicable,
                InterfaceGlobalsFound: interfaceResult.GlobalsFound,
                InterfaceGlobalsRenamed: interfaceResult.GlobalsRenamed,
                InterfaceTypesApplied: interfaceResult.TypesApplied,
                InterfaceVTablesImported: interfaceResult.VTablesImported,
                InterfaceImportSkipped: interfaceResult.Skipped,
                InterfaceClangErrors: interfaceResult.ClangErrors,
                SchemaImportApplicable: schemaResult.Applicable,
                SchemaProject: schemaResult.Project,
                SchemaTypesImported: schemaResult.TypesImported,
                SchemaVTablesMatched: schemaResult.VTablesMatched,
                SchemaFunctionsBound: schemaResult.FunctionsBound,
                SchemaFunctionsSkipped: schemaResult.FunctionsSkipped,
                SchemaFunctionConflicts: schemaResult.FunctionConflicts,
                SchemaClangErrors: schemaResult.ClangErrors,
                SchemaVTableTypesCompleted: schemaResult.VTableTypesCompleted,
                SchemaVTableAddressesBound: schemaResult.VTableAddressesBound,
                SchemaVTableUnknownSlots: schemaResult.VTableUnknownSlots,
                SchemaVTableConflicts: schemaResult.VTableConflicts);
            completed = true;
            return result;
        }
        finally
        {
            SchemaImport.ResetParser();
            // Exceptions leave the pipeline incomplete, so partial changes are never persisted.
            IdaNative.close_database(save && completed ? (byte)1 : (byte)0);
        }
    }

    private static void DriveAnalysis(Action<double, ulong>? onProgress)
    {
        if (onProgress == null)
        {
            IdaNative.auto_wait();
            return;
        }

        var (start, end) = GetAddressSpan();
        ulong span = end > start ? end - start : 0;

        var clock = Stopwatch.StartNew();
        TimeSpan next = TimeSpan.Zero;
        double reported = 0.0;

        onProgress(0.0, start);

        while (IdaNative.auto_make_step(start, end) != 0)
        {
            if (clock.Elapsed < next)
            {
                continue;
            }

            next = clock.Elapsed + ReportInterval;

            if (span == 0)
            {
                continue;
            }

            AutoDisplay display;
            if (IdaNative.get_auto_display(&display) == 0)
            {
                continue;
            }

            ulong ea = display.Ea;
            if (ea < start || ea > end)
            {
                continue;
            }

            double fraction = (double)(ea - start) / span;
            if (fraction <= reported)
            {
                continue;
            }

            reported = fraction;
            onProgress(fraction, ea);
        }

        IdaNative.auto_wait();
        onProgress(1.0, end);
    }

    private static (ulong Start, ulong End) GetAddressSpan()
    {
        int qty = IdaNative.get_segm_qty();
        if (qty == 0)
        {
            return (0, 0);
        }

        void* first = IdaNative.getnseg(0);
        void* last = IdaNative.getnseg(qty - 1);

        if (first == null || last == null)
        {
            return (0, 0);
        }

        ulong start = *(ulong*)first;
        ulong end = *((ulong*)last + 1);
        return (start, end);
    }

    private static void AssertOwner()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("The IDA kernel has not been initialized.");
        }

        if (Environment.CurrentManagedThreadId != _ownerThread)
        {
            throw new InvalidOperationException(
                "idalib is single-threaded and must be called from the thread that initialized it.");
        }
    }
}
