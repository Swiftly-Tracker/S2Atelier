using Microsoft.Extensions.FileSystemGlobbing;
using S2Atelier.Ida.Worker;
using Spectre.Console;

namespace S2Atelier;

public static class Entrypoint
{
    public static void Main(string[] args)
    {
        var options = CliOptions.Parse(args);

        if (options.IsWorker)
        {
            if (string.IsNullOrWhiteSpace(options.IdaPath))
            {
                Console.Error.WriteLine("--ida-worker requires --ida-path.");
                Environment.Exit(1);
                return;
            }

            Environment.Exit(IdaWorkerProcess.Run(options.IdaPath, options.SdkVersion));
            return;
        }

        if (options.ShowHelp)
        {
            PrintUsage();
            Environment.Exit(0);
            return;
        }

        if (options.ParseError != null)
        {
            Console.Error.WriteLine(options.ParseError);
            Environment.Exit(1);
            return;
        }

        if (options.Globs.Count == 0)
        {
            PrintUsage();
            Environment.Exit(1);
            return;
        }

        if (string.IsNullOrWhiteSpace(options.IdaPath))
        {
            Console.Error.WriteLine("No IDA installation given. Pass --ida-path <dir> or set the IDA_PATH environment variable.");
            Environment.Exit(1);
            return;
        }

        if (!options.ValidateSchemaOptions(out string? schemaError))
        {
            Console.Error.WriteLine(schemaError);
            Environment.Exit(1);
            return;
        }

        if (!options.ValidateConVarOptions(out string? convarError))
        {
            Console.Error.WriteLine(convarError);
            Environment.Exit(1);
            return;
        }

        string root = options.Root ?? Directory.GetCurrentDirectory();

        var matcher = new Matcher();
        foreach (string glob in options.Globs)
        {
            matcher.AddInclude(glob);
        }

        var matches = matcher.GetResultsInFullPath(root)
            .OrderBy(p => p, StringComparer.Ordinal)
            .ToList();

        if (matches.Count == 0)
        {
            Console.Error.WriteLine($"No files under '{root}' matched: {string.Join(' ', options.Globs)}");
            Environment.Exit(1);
            return;
        }

        Console.WriteLine($"Analyzing {matches.Count} binaries with {options.Cores} concurrent worker(s), " +
                           $"SDK {options.SdkVersion}, save={!options.NoSave}, patch-plt={options.PatchPlt}, " +
                           $"name-convars={options.NameConVars}, convar-types={options.ConVarTypesPath ?? "off"}, " +
                           $"name-log-channels={options.NameLogChannels}, " +
                           $"name-entity-classes={options.NameEntityClasses}, " +
                           $"name-fnptr-tables={options.NameFnPtrTables}, " +
                           $"import-protobufs={options.ImportProtobufsDir ?? "off"}, " +
                           $"import-interfaces={options.ImportInterfaces}, " +
                           $"import-schema={options.ImportSchemaPath ?? "off"}, schema-project={options.SchemaProject}.");

        using var pool = new IdaWorkerPool(options.IdaPath, options.SdkVersion, options.Cores);

        var results = options.Progress
            ? RunWithLiveProgress(pool, matches, !options.NoSave, options.PatchPlt, options.NameConVars,
                options.NameFnPtrTables, options.ImportProtobufsDir, options.ImportSchemaPath, options.Hl2SdkPath,
                options.SchemaProject, options.ImportInterfaces, options.Cores, options.ConVarTypesPath,
                options.NameLogChannels, options.VTableBaselineDirectory, options.VTableSnapshotDirectory,
                options.NameEntityClasses)
            : RunPlain(pool, matches, !options.NoSave, options.PatchPlt, options.NameConVars,
                options.NameFnPtrTables, options.ImportProtobufsDir, options.ImportSchemaPath, options.Hl2SdkPath,
                options.SchemaProject, options.ImportInterfaces, options.ConVarTypesPath, options.NameLogChannels,
                options.VTableBaselineDirectory, options.VTableSnapshotDirectory, options.NameEntityClasses);

        Console.WriteLine();

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("File");
        table.AddColumn("Status");
        table.AddColumn("Functions");
        table.AddColumn("Segments");
        table.AddColumn("Strings");
        table.AddColumn("Time");

        if (options.PatchPlt)
        {
            table.AddColumn("PLT");
        }

        if (options.NameConVars)
        {
            table.AddColumn("ConVars");
        }

        if (options.NameLogChannels)
        {
            table.AddColumn("Log channels");
        }

        if (options.NameFnPtrTables)
        {
            table.AddColumn("FnPtrs");
        }

        if (options.ImportProtobufsDir != null)
        {
            table.AddColumn("Protobufs");
        }

        if (options.ImportInterfaces)
        {
            table.AddColumn("Interfaces");
        }

        if (options.ImportSchemaPath != null)
        {
            table.AddColumn("Schema");
        }

        foreach (var item in results)
        {
            var row = new List<string>
            {
                Path.GetFileName(item.Path),
                item.Succeeded ? "[green]ok[/]" : "[red]failed[/]",
                item.Functions.ToString(),
                item.Segments.ToString(),
                item.Strings.ToString(),
                $"{item.Elapsed.TotalSeconds:F1}s",
            };

            if (options.PatchPlt)
            {
                row.Add(item.PltPatchApplicable
                    ? $"{item.PltPatched} patched" + (item.PltUnresolved > 0 ? $", {item.PltUnresolved} unresolved" : "")
                    : "n/a");
            }

            if (options.NameConVars)
            {
                row.Add(item.ConVarNamingApplicable
                    ? $"{item.ConVarNamingFound} found, {item.ConVarNamingRenamedObjects + item.ConVarNamingRenamedHandlers} renamed, " +
                      $"{item.ConVarNamingTypedObjects} typed"
                    : "n/a");
            }

            if (options.NameLogChannels)
            {
                row.Add(item.LogChannelNamingApplicable
                    ? $"{item.LogChannelNamingFound} found, {item.LogChannelNamingRenamed} renamed"
                    : "n/a");
            }

            if (options.NameFnPtrTables)
            {
                row.Add($"{item.FnPtrNamingFound} found, {item.FnPtrNamingRenamed} renamed");
            }

            if (options.ImportProtobufsDir != null)
            {
                row.Add(item.ProtoImportApplicable
                    ? $"{item.ProtoTypesDefined} types" + (item.ProtoImportErrors > 0 ? $", {item.ProtoImportErrors} errors" : "")
                    : "n/a");
            }

            if (options.ImportInterfaces)
            {
                row.Add(item.Succeeded && item.InterfaceImportApplicable
                    ? $"{item.InterfaceGlobalsFound} found, {item.InterfaceGlobalsRenamed} renamed, " +
                      $"{item.InterfaceTypesApplied} typed, {item.InterfaceVTablesImported} vtables" +
                      (item.InterfaceImportSkipped > 0 ? $", {item.InterfaceImportSkipped} skipped" : "") +
                      (item.InterfaceClangErrors > 0 ? $", {item.InterfaceClangErrors} clang errors" : "")
                    : item.InterfaceClangErrors > 0 ? $"{item.InterfaceClangErrors} clang errors" : "n/a");
            }

            if (options.ImportSchemaPath != null)
            {
                row.Add(item.Succeeded && item.SchemaImportApplicable
                    ? $"{item.SchemaProject}: {item.SchemaTypesImported} types, {item.SchemaVTablesMatched} vtables found, {item.SchemaVTableTypesCompleted} completed, {item.SchemaVTableAddressesBound} address-bound, {item.SchemaVTableUnknownSlots} unknown slots, {item.SchemaVTableConflicts} vtable conflicts, " +
                      $"{item.SchemaFunctionsBound} bound/{item.SchemaFunctionsSkipped} skipped/" +
                      $"{item.SchemaFunctionConflicts} conflicts" +
                      (item.SchemaClangErrors > 0 ? $", {item.SchemaClangErrors} clang errors (ignored)" : "")
                    : item.SchemaClangErrors > 0 ? $"{item.SchemaClangErrors} clang errors" : "n/a");
            }

            table.AddRow([.. row]);
        }

        AnsiConsole.Write(table);

        // The table has no room for the reason, and --progress never logs it per item.
        foreach (var item in results.Where(r => options.Progress && !r.Succeeded))
        {
            Console.Error.WriteLine($"[FAILED] {Path.GetFileName(item.Path)}: {item.Error}");
        }

        int failed = results.Count(r => !r.Succeeded);
        Console.WriteLine($"{results.Count - failed}/{results.Count} succeeded.");

        Environment.Exit(failed == 0 ? 0 : 1);
    }

    private static IReadOnlyList<BatchItem> RunPlain(
        IdaWorkerPool pool, IReadOnlyList<string> paths, bool save, bool patchPlt, bool nameConVars,
        bool nameFnPtrTables, string? importProtobufsDir, string? importSchemaPath, string? hl2SdkPath,
        string schemaProject, bool importInterfaces, string? convarTypesPath, bool nameLogChannels,
        string? vtableBaseline, string? vtableSnapshot, bool nameEntityClasses)
        => pool.RunBatch(
            paths,
            save,
            patchPlt,
            nameConVars,
            nameFnPtrTables,
            importProtobufsDir,
            importSchemaPath,
            hl2SdkPath,
            schemaProject,
            onStarted: (path, worker) => Console.WriteLine($"[worker {worker}] {Path.GetFileName(path)}: analyzing..."),
            onFinished: (_, item) => Console.WriteLine(item.Succeeded
                ? $"[done]   {Path.GetFileName(item.Path)}: {item.Functions} functions, {item.Segments} segments, " +
                  $"{item.Strings} strings ({item.Elapsed.TotalSeconds:F1}s)" +
                  (patchPlt && item.PltPatchApplicable ? $" [plt: {item.PltPatched} patched, {item.PltUnresolved} unresolved]" : "") +
                  (nameConVars && item.ConVarNamingApplicable ? $" [convars: {item.ConVarNamingFound} found, " +
                    $"{item.ConVarNamingRenamedObjects} objects + {item.ConVarNamingRenamedHandlers} handlers renamed, " +
                    $"{item.ConVarNamingTypedObjects} typed]" : "") +
                  (nameLogChannels && item.LogChannelNamingApplicable
                    ? $" [log channels: {item.LogChannelNamingFound} found, {item.LogChannelNamingRenamed} renamed]" : "") +
                  (nameFnPtrTables ? $" [fnptrs: {item.FnPtrNamingFound} found, {item.FnPtrNamingRenamed} renamed]" : "") +
                  (importProtobufsDir != null && item.ProtoImportApplicable
                    ? $" [protobufs: {item.ProtoTypesDefined} types, {item.ProtoImportErrors} errors]" : "")
                  + (importInterfaces && item.InterfaceImportApplicable
                    ? $" [interfaces: {item.InterfaceGlobalsFound} found, {item.InterfaceGlobalsRenamed} renamed, " +
                      $"{item.InterfaceTypesApplied} typed, {item.InterfaceVTablesImported} vtables, " +
                      $"{item.InterfaceImportSkipped} skipped, {item.InterfaceClangErrors} clang errors]" : "")
                  + (importSchemaPath != null && item.SchemaImportApplicable
                    ? $" [schema {item.SchemaProject}: {item.SchemaTypesImported} types, " +
                      $"{item.SchemaVTablesMatched} vtables found, {item.SchemaVTableTypesCompleted} completed, {item.SchemaVTableAddressesBound} address-bound, {item.SchemaVTableUnknownSlots} unknown slots, {item.SchemaVTableConflicts} vtable conflicts, {item.SchemaFunctionsBound} bound, " +
                      $"{item.SchemaFunctionsSkipped} skipped, {item.SchemaFunctionConflicts} conflicts" +
                      (item.SchemaClangErrors > 0 ? $", {item.SchemaClangErrors} clang errors ignored" : "") + "]" : "")
                : $"[FAILED] {Path.GetFileName(item.Path)}: {item.Error}"),
            importInterfaces: importInterfaces,
            convarTypesPath: convarTypesPath,
            nameLogChannels: nameLogChannels,
            vtableBaselineDirectory: vtableBaseline,
            vtableSnapshotDirectory: vtableSnapshot,
            nameEntityClasses: nameEntityClasses);

    private static IReadOnlyList<BatchItem> RunWithLiveProgress(
        IdaWorkerPool pool, IReadOnlyList<string> paths, bool save, bool patchPlt, bool nameConVars,
        bool nameFnPtrTables, string? importProtobufsDir, string? importSchemaPath, string? hl2SdkPath,
        string schemaProject, bool importInterfaces, int cores, string? convarTypesPath, bool nameLogChannels,
        string? vtableBaseline, string? vtableSnapshot, bool nameEntityClasses)
    {
        IReadOnlyList<BatchItem> results = [];
        var previousLog = pool.Log;
        var logLock = new object();
        // Worker output must go through Spectre so the live display is redrawn below it.
        pool.Log = line =>
        {
            lock (logLock)
            {
                AnsiConsole.WriteLine(line);
            }
        };

        try
        {
            AnsiConsole.Progress()
                .Columns(
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new RemainingTimeColumn(),
                    new SpinnerColumn())
                .Start(ctx =>
                {
                    var tasks = new ProgressTask[cores];
                    var names = new string[cores];
                    for (int i = 0; i < cores; i++)
                    {
                        tasks[i] = ctx.AddTask($"worker {i}: idle", autoStart: false, maxValue: 100);
                        names[i] = string.Empty;
                    }

                    results = pool.RunBatch(
                        paths,
                        save,
                        patchPlt,
                        nameConVars,
                        nameFnPtrTables,
                        importProtobufsDir,
                        importSchemaPath,
                        hl2SdkPath,
                        schemaProject,
                        onStarted: (path, worker) =>
                        {
                            var task = tasks[worker];
                            names[worker] = Markup.Escape(Path.GetFileName(path));
                            task.Description = names[worker];
                            task.Value = 0;
                            if (!task.IsStarted)
                            {
                                task.StartTask();
                            }
                        },
                        onFinished: (worker, item) =>
                        {
                            string name = Markup.Escape(Path.GetFileName(item.Path));
                            tasks[worker].Value = 100;
                            tasks[worker].Description = item.Succeeded ? $"{name} [green]done[/]" : $"{name} [red]failed[/]";
                        },
                        onProgress: (worker, fraction, _) => tasks[worker].Value = fraction * 100,
                        importInterfaces: importInterfaces,
                        onStage: (worker, stage) =>
                            tasks[worker].Description = $"{names[worker]} [grey]{Markup.Escape(stage)}[/]",
                        convarTypesPath: convarTypesPath,
                        nameLogChannels: nameLogChannels,
                        vtableBaselineDirectory: vtableBaseline,
                        vtableSnapshotDirectory: vtableSnapshot,
                        nameEntityClasses: nameEntityClasses);
                });
        }
        finally
        {
            pool.Log = previousLog;
        }

        return results;
    }

    private static void PrintUsage()
    {
        Console.WriteLine("""
            S2Atelier - concurrent IDA-headless (idalib) auto-analysis over globbed binaries.

            Usage:
              s2atelier <glob> [<glob> ...] [options]

            Options:
              --ida-path <dir>   Root of the IDA installation (ida/idalib + cfg/ + procs/).
                                  Falls back to the IDA_PATH environment variable.
              --ida-sdk <ver>    Which generated bindings to bind against: auto (default), 9.2, 9.3.
              --cores <n>        Concurrent worker processes. Default: all logical processors.
              --root <dir>       Base directory glob patterns are matched against. Default: cwd.
              --no-save          Do not save the resulting database.
              --progress         Show a live per-worker progress bar instead of log lines.
              --patch-plt        Patch broken PLT stubs after analysis (ELF64/.so only; a no-op
                                  on anything else). Fixes missing xrefs to imported functions on
                                  binaries linked with mold, where IDA reports "Could not patch
                                  the PLT stub". Port of github.com/GAMMACASE/PltPatcher.
              --name-convars     Find Source engine ConVar/ConCommand registrations and rename
                                  the objects (cvar_/cmd_) and command handlers (cmd_..._callback), with
                                  descriptions written back as comments. Best-effort heuristic
                                  naming, not guaranteed accurate.
              --convar-types <convars.json>
                                  With --name-convars, take each convar's value type from a runtime
                                  dump (CS2-Dumps' convars.json, datatype_raw) instead of recovering
                                  it from the registration. Convars the dump does not list keep the
                                  recovered type; disagreements are reported.
              --name-log-channels
                                  Name the globals LoggingSystem_RegisterLoggingChannel results are
                                  stored in LOG_<CHANNEL NAME>, typed LoggingChannelID_t when the SDK
                                  headers were imported.
              --name-entity-classes
                                  Name and type every entity class's CEntityClass static and
                                  CEntityClassInfo, its GetEntityClassInternal accessor, cached
                                  pointer, guards, callbacks, schema binding and data map. With
                                  --hl2sdk the layout comes from entity2/entityclass.h and is checked
                                  against the binary first; a layout the binary contradicts is
                                  reported and its types are not applied. With --snapshot,
                                  the entity class graph is written next to the snapshot.
              --name-fnptr-tables
                                  Find name-resolution cascades anywhere in the binary and rename
                                  the resolved sub_X functions: both "cmp arg, &sub_X ; ... ;
                                  return "Name"" chains (function pointer -> name) and
                                  "strcmp(arg, "Name") ; je ... ; return &sub_X" chains
                                  (name -> function pointer). Best-effort heuristic naming, not
                                  guaranteed accurate.
              --import-protobufs <dir>
                                  Parse every protoc-generated *.pb.h header under <dir>
                                  (recursively) and add a local type per message/enum to each
                                  database's Local Types, mirroring the real compiled
                                  google::protobuf::Message layout (the "_Impl_" struct: hasbits,
                                  cached size, then fields in their real declared order/type -
                                  RepeatedField/RepeatedPtrField/ExtensionSet sizes are
                                  hand-verified against protobuf 3.21.8's runtime headers, not
                                  guessed). Point this at the actual generated proto/ build
                                  output, not the .proto sources - a directory of .proto files
                                  alone has no compiled layout to read. Never applied to any
                                  address, so it can't mislabel real memory.
              --import-schema <sdk.json>
                                  Import the current binary project's schema classes/enums into
                                  Local Types, detect RTTI vtables, and bind virtual-function this
                                  parameters. Must match the binary platform/game build and be used
                                  together with --hl2sdk. Only 64-bit PE/ELF inputs are supported.
              --import-interfaces
                                  Locate the validated ConnectInterfaces global table, rename its
                                  automatic unk_/qword_ pointer slots to the official HL2SDK g_p...
                                  names, apply interface pointer types, and import reliable virtual
                                  tables through IDAClang. Requires --hl2sdk; sdk.json is not needed.
              --hl2sdk <dir>      HL2SDK root used by --import-interfaces and/or --import-schema.
              --vtable-baseline <dir>
                                  With --hl2sdk, compare the SDK-named vtables with the previous
                                  build's snapshot (<dir>/<binary>.vtables.json). A class whose slots moved
                                  so the SDK would rename a known function keeps its SDK names back; its
                                  vtable is commented and it is reported until the SDK agrees again.
              --vtable-snapshot <dir>
                                  With --hl2sdk, write this build's snapshot to <dir>, the baseline
                                  of the next build.
              --schema-project <auto|project>
                                  Project roots to import. Default auto derives client/server/etc.
                                  from the binary filename (including libNAME.so).
              -h, --help         Show this help.

            Example:
              s2atelier "bin/**/*.dll" "bin/**/*.so" --ida-path "C:\\IDA" --cores 4
              s2atelier "server.dll" --ida-path "D:\\Software\\ida93sp2" --import-interfaces --hl2sdk "D:\\Code\\hl2sdk"
              s2atelier "bin/client.dll" --ida-path "C:\\IDA" --import-schema "sdk.json" --hl2sdk "D:\\Code\\hl2sdk"
            """);
    }
}
