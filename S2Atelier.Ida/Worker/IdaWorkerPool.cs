using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida.Worker;

public sealed record BatchItem(
    string Path, bool Succeeded, int Functions, int Segments, int Strings, TimeSpan Elapsed, string? Error,
    bool PltPatchApplicable = false, int PltPatched = 0, int PltUnresolved = 0,
    bool ConVarNamingApplicable = false, int ConVarNamingFound = 0,
    int ConVarNamingRenamedObjects = 0, int ConVarNamingRenamedHandlers = 0,
    int FnPtrNamingFound = 0, int FnPtrNamingRenamed = 0,
    bool ProtoImportApplicable = false, int ProtoTypesDefined = 0, int ProtoImportErrors = 0);

public sealed class IdaWorkerPool(string idaPath, IdaSdkVersion sdk, int size) : IDisposable
{
    private readonly List<Worker> _workers = [];

    public IReadOnlyList<BatchItem> RunBatch(
        IReadOnlyList<string> paths,
        bool save,
        bool patchPlt = false,
        bool nameConVars = false,
        bool nameFnPtrTables = false,
        string? importProtobufsDir = null,
        Action<string, int>? onStarted = null,
        Action<int, BatchItem>? onFinished = null,
        Action<int, double, ulong>? onProgress = null)
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
            var worker = new Worker(i, idaPath, sdk);
            _workers.Add(worker);

            var thread = new Thread(() =>
            {
                while (queue.TryDequeue(out int index))
                {
                    onStarted?.Invoke(paths[index], worker.Index);
                    var item = worker.Run(paths[index], save, patchPlt, nameConVars, nameFnPtrTables,
                        importProtobufsDir, (fraction, address) => onProgress?.Invoke(worker.Index, fraction, address));
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

    private sealed class Worker(int index, string idaPath, IdaSdkVersion sdk) : IDisposable
    {
        private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

        private Process? _process;

        internal int Index => index;

        internal BatchItem Run(
            string path, bool save, bool patchPlt, bool nameConVars, bool nameFnPtrTables,
            string? importProtobufsDir, Action<double, ulong>? onProgress = null)
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
                }));
                process.StandardInput.Flush();

                while (process.StandardOutput.ReadLine() is { } line)
                {
                    var message = WorkerProtocol.Read(line);

                    if (message?.Kind == WireKind.Progress)
                    {
                        onProgress?.Invoke(message.Fraction, message.Address);
                        continue;
                    }

                    if (message?.Kind == WireKind.Done)
                    {
                        return new BatchItem(full, true, message.Functions, message.Segments,
                            message.Strings, TimeSpan.FromMilliseconds(message.Milliseconds), null,
                            message.PltPatchApplicable, message.PltPatched, message.PltUnresolved,
                            message.ConVarNamingApplicable, message.ConVarNamingFound,
                            message.ConVarNamingRenamedObjects, message.ConVarNamingRenamedHandlers,
                            message.FnPtrNamingFound, message.FnPtrNamingRenamed,
                            message.ProtoImportApplicable, message.ProtoTypesDefined, message.ProtoImportErrors);
                    }

                    if (message?.Kind == WireKind.Failed)
                    {
                        return Failed(full, message.Error ?? "The worker did not say why.");
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

#pragma warning disable IL3000
                string? entryDll = Assembly.GetEntryAssembly()?.Location;
#pragma warning restore IL3000

                if (string.IsNullOrEmpty(entryDll))
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
                if (e.Data != null)
                {
                    Console.Error.WriteLine($"[worker {index}] {e.Data}");
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

        private static BatchItem Failed(string path, string error) => new(path, false, 0, 0, 0, TimeSpan.Zero, error);

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
