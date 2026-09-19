using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida.Worker;

public sealed record BatchItem(
    string Path, bool Succeeded, int Functions, int Segments, int Strings, TimeSpan Elapsed, string? Error,
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

public sealed class IdaWorkerPool(string idaPath, IdaSdkVersion sdk, int size) : IDisposable
{
    private readonly List<Worker> _workers = [];

    // Receives worker diagnostics. A live console display must replace this: raw console
    // writes from the worker threads land in the middle of the display's redraws.
    public Action<string> Log { get; set; } = Console.Error.WriteLine;

    public IReadOnlyList<BatchItem> RunBatch(
        IReadOnlyList<string> paths,
        bool save,
        bool patchPlt,
        bool nameConVars,
        bool nameFnPtrTables,
        string? importProtobufsDir,
        Action<string, int>? onStarted,
        Action<int, BatchItem>? onFinished,
        Action<int, double, ulong>? onProgress)
        => RunBatch(paths, save, patchPlt, nameConVars, nameFnPtrTables, importProtobufsDir,
            importSchemaPath: null, hl2SdkPath: null, schemaProject: "auto", importInterfaces: false,
            onStarted: onStarted, onFinished: onFinished, onProgress: onProgress);

    public IReadOnlyList<BatchItem> RunBatch(
        IReadOnlyList<string> paths,
        bool save,
        bool patchPlt = false,
        bool nameConVars = false,
        bool nameFnPtrTables = false,
        string? importProtobufsDir = null,
        string? importSchemaPath = null,
        string? hl2SdkPath = null,
        string schemaProject = "auto",
        Action<string, int>? onStarted = null,
        Action<int, BatchItem>? onFinished = null,
        Action<int, double, ulong>? onProgress = null,
        bool importInterfaces = false,
        Action<int, string>? onStage = null,
        string? convarTypesPath = null,
        bool nameLogChannels = false,
        string? vtableBaselineDirectory = null,
        string? vtableSnapshotDirectory = null)
    {
        if (paths.Count == 0)
        {
            return [];
        }

        int workerCount = Math.Max(1, size);
        var results = new BatchItem?[paths.Count];
        var queue = new ConcurrentQueue<int>(Enumerable.Range(0, paths.Count));

        var threads = new List<Thread>(workerCount);

        for (int i = 0; i < workerCount; i++)
        {
            var worker = new Worker(i, idaPath, sdk, line => Log(line));
            _workers.Add(worker);

            var thread = new Thread(() =>
            {
                while (queue.TryDequeue(out int index))
                {
                    onStarted?.Invoke(paths[index], worker.Index);
                    var item = worker.Run(paths[index], save, patchPlt, nameConVars, nameFnPtrTables,
                        importProtobufsDir, importSchemaPath, hl2SdkPath, schemaProject, importInterfaces,
                        (fraction, address) => onProgress?.Invoke(worker.Index, fraction, address),
                        stage => onStage?.Invoke(worker.Index, stage),
                        convarTypesPath,
                        nameLogChannels,
                        vtableBaselineDirectory,
                        vtableSnapshotDirectory);
                    results[index] = item;
                    onFinished?.Invoke(worker.Index, item);
                }
            })
            { IsBackground = true, Name = $"ida-worker-{i}" };

            threads.Add(thread);
        }

        foreach (var thread in threads)
        {
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return [.. results.Select(r => r!)];
    }

    public void Dispose()
    {
        foreach (var worker in _workers)
        {
            worker.Dispose();
        }

        _workers.Clear();
    }

    private sealed class Worker(int index, string idaPath, IdaSdkVersion sdk, Action<string> log) : IDisposable
    {
        private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

        private Process? _process;

        internal int Index => index;

        internal BatchItem Run(
            string path, bool save, bool patchPlt, bool nameConVars, bool nameFnPtrTables,
            string? importProtobufsDir, string? importSchemaPath, string? hl2SdkPath, string schemaProject,
            bool importInterfaces,
            Action<double, ulong>? onProgress = null,
            Action<string>? onStage = null,
            string? convarTypesPath = null,
            bool nameLogChannels = false,
            string? vtableBaselineDirectory = null,
            string? vtableSnapshotDirectory = null)
        {
            string full = Path.GetFullPath(path);

            if (_process is not { HasExited: false } && !Start())
            {
                return Failed(full, $"Worker {index} could not be started.");
            }

            var process = _process!;

            try
            {
                process.StandardInput.WriteLine(WorkerProtocol.Write(new WireMessage
                {
                    Kind = WireKind.Job,
                    Path = full,
                    Save = save,
                    PatchPlt = patchPlt,
                    NameConVars = nameConVars,
                    NameFnPtrTables = nameFnPtrTables,
                    ImportProtobufsDir = importProtobufsDir,
                    ImportSchemaPath = importSchemaPath,
                    ConVarTypesPath = convarTypesPath,
                    NameLogChannels = nameLogChannels,
                    VTableBaselineDirectory = vtableBaselineDirectory,
                    VTableSnapshotDirectory = vtableSnapshotDirectory,
                    Hl2SdkPath = hl2SdkPath,
                    ImportInterfaces = importInterfaces,
                    SchemaProject = schemaProject,
                }));
                process.StandardInput.Flush();

                while (process.StandardOutput.ReadLine() is { } line)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    var message = WorkerProtocol.Read(line);

                    // idalib normally keeps its message window off because stdout carries the
                    // worker protocol. Schema import briefly enables it so IDAClang diagnostics
                    // are visible; relay those non-protocol lines through the parent's stderr.
                    if (message == null)
                    {
                        log($"[worker {index}] {line}");
                        continue;
                    }

                    if (message.Kind == WireKind.Progress)
                    {
                        if (message.Stage != null)
                        {
                            onStage?.Invoke(message.Stage);
                        }
                        else
                        {
                            onProgress?.Invoke(message.Fraction, message.Address);
                        }
                        continue;
                    }

                    if (message.Kind == WireKind.Done)
                    {
                        return new BatchItem(
                            full, true, message.Functions, message.Segments, message.Strings,
                            TimeSpan.FromMilliseconds(message.Milliseconds), null,
                            PltPatchApplicable: message.PltPatchApplicable,
                            PltPatched: message.PltPatched,
                            PltUnresolved: message.PltUnresolved,
                            ConVarNamingApplicable: message.ConVarNamingApplicable,
                            ConVarNamingFound: message.ConVarNamingFound,
                            ConVarNamingRenamedObjects: message.ConVarNamingRenamedObjects,
                            ConVarNamingRenamedHandlers: message.ConVarNamingRenamedHandlers,
                            ConVarNamingTypedObjects: message.ConVarNamingTypedObjects,
                            FnPtrNamingFound: message.FnPtrNamingFound,
                            FnPtrNamingRenamed: message.FnPtrNamingRenamed,
                            LogChannelNamingApplicable: message.LogChannelNamingApplicable,
                            LogChannelNamingFound: message.LogChannelNamingFound,
                            LogChannelNamingRenamed: message.LogChannelNamingRenamed,
                            ProtoImportApplicable: message.ProtoImportApplicable,
                            ProtoTypesDefined: message.ProtoTypesDefined,
                            ProtoImportErrors: message.ProtoImportErrors,
                            InterfaceImportApplicable: message.InterfaceImportApplicable,
                            InterfaceGlobalsFound: message.InterfaceGlobalsFound,
                            InterfaceGlobalsRenamed: message.InterfaceGlobalsRenamed,
                            InterfaceTypesApplied: message.InterfaceTypesApplied,
                            InterfaceVTablesImported: message.InterfaceVTablesImported,
                            InterfaceImportSkipped: message.InterfaceImportSkipped,
                            InterfaceClangErrors: message.InterfaceClangErrors,
                            SchemaImportApplicable: message.SchemaImportApplicable,
                            SchemaProject: message.ImportedSchemaProject,
                            SchemaTypesImported: message.SchemaTypesImported,
                            SchemaVTablesMatched: message.SchemaVTablesMatched,
                            SchemaFunctionsBound: message.SchemaFunctionsBound,
                            SchemaFunctionsSkipped: message.SchemaFunctionsSkipped,
                            SchemaFunctionConflicts: message.SchemaFunctionConflicts,
                            SchemaClangErrors: message.SchemaClangErrors,
                            SchemaVTableTypesCompleted: message.SchemaVTableTypesCompleted,
                            SchemaVTableAddressesBound: message.SchemaVTableAddressesBound,
                            SchemaVTableUnknownSlots: message.SchemaVTableUnknownSlots,
                            SchemaVTableConflicts: message.SchemaVTableConflicts);
                    }

                    if (message.Kind == WireKind.Failed)
                    {
                        return Failed(full, message.Error ?? "The worker did not say why.",
                            message.SchemaClangErrors, message.InterfaceClangErrors);
                    }
                }

                return Failed(full, $"Worker {index} exited during analysis.");
            }
            catch (IOException ex)
            {
                return Failed(full, $"Lost contact with worker {index}: {ex.Message}");
            }
        }

        private bool Start()
        {
            Kill();

            string? host = Environment.ProcessPath;
            if (string.IsNullOrEmpty(host))
            {
                return false;
            }

            var info = new ProcessStartInfo(host)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardInputEncoding = Utf8NoBom,
                StandardOutputEncoding = Utf8NoBom,
                StandardErrorEncoding = Utf8NoBom,
            };

            string hostName = Path.GetFileNameWithoutExtension(host);
            if (hostName.Equals("dotnet", StringComparison.OrdinalIgnoreCase))
            {
                string? entryDll = Environment.GetCommandLineArgs().FirstOrDefault();

                if (string.IsNullOrEmpty(entryDll) || !entryDll.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                info.ArgumentList.Add(entryDll);
            }

            info.ArgumentList.Add("--ida-worker");
            info.ArgumentList.Add("--ida-path");
            info.ArgumentList.Add(idaPath);

            if (sdk != IdaSdkVersion.Auto)
            {
                info.ArgumentList.Add("--ida-sdk");
                info.ArgumentList.Add(sdk.ToString());
            }

            try
            {
                _process = Process.Start(info);
            }
            catch (Exception)
            {
                return false;
            }

            if (_process == null)
            {
                return false;
            }

            _process.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    log($"[worker {index}] {e.Data}");
                }
            };

            _process.BeginErrorReadLine();

            string? readyLine = _process.StandardOutput.ReadLine();
            var ready = readyLine != null ? WorkerProtocol.Read(readyLine) : null;

            if (ready?.Kind != WireKind.Ready)
            {
                Kill();
                return false;
            }

            return true;
        }

        private static BatchItem Failed(
            string path, string error, int schemaClangErrors = 0, int interfaceClangErrors = 0)
            => new(path, false, 0, 0, 0, TimeSpan.Zero, error,
                InterfaceClangErrors: interfaceClangErrors, SchemaClangErrors: schemaClangErrors);

        private void Kill()
        {
            var process = _process;
            _process = null;

            if (process == null)
            {
                return;
            }

            try
            {
                if (!process.HasExited)
                {
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                process.Dispose();
            }
        }

        public void Dispose()
        {
            var process = _process;

            if (process is { HasExited: false })
            {
                try
                {
                    process.StandardInput.WriteLine(WorkerProtocol.Write(new WireMessage { Kind = WireKind.Quit }));
                    process.StandardInput.Flush();
                    process.WaitForExit(5000);
                }
                catch (Exception)
                {
                }
            }

            Kill();
        }
    }
}
