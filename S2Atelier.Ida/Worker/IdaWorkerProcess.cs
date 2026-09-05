using S2Atelier.Ida.Generated;

namespace S2Atelier.Ida.Worker;

public static class IdaWorkerProcess
{
    public static int Run(string idaPath, IdaSdkVersion sdk)
    {
        if (!IdaKernel.TryInitialize(idaPath, sdk, out string? error))
        {
            Console.Error.WriteLine(error);
            return 1;
        }

        Send(new WireMessage { Kind = WireKind.Ready, Sdk = IdaKernel.SdkVersion.ToString() });

        string? line;
        while ((line = Console.In.ReadLine()) != null)
        {
            var message = WorkerProtocol.Read(line);
            if (message == null)
            {
                continue;
            }

            if (message.Kind == WireKind.Quit)
            {
                break;
            }

            if (message.Kind != WireKind.Job || string.IsNullOrEmpty(message.Path))
            {
                continue;
            }

            RunJob(message.Path, message.Save, message.PatchPlt, message.NameConVars, message.NameFnPtrTables,
                message.ImportProtobufsDir);
        }

        return 0;
    }

    private static void RunJob(
        string path, bool save, bool patchPlt, bool nameConVars, bool nameFnPtrTables, string? importProtobufsDir)
    {
        if (!File.Exists(path))
        {
            Send(new WireMessage { Kind = WireKind.Failed, Error = $"No such file: '{path}'." });
            return;
        }

        try
        {
            var result = IdaKernel.Open(path, save, patchPlt, nameConVars, nameFnPtrTables, importProtobufsDir,
                (fraction, address) =>
                    Send(new WireMessage { Kind = WireKind.Progress, Fraction = fraction, Address = address }));

            Send(new WireMessage
            {
                Kind = WireKind.Done,
                Functions = result.Functions,
                Segments = result.Segments,
                Strings = result.Strings,
                Milliseconds = result.Elapsed.TotalMilliseconds,
                PltPatchApplicable = result.PltPatchApplicable,
                PltPatched = result.PltPatched,
                PltUnresolved = result.PltUnresolved,
                ConVarNamingApplicable = result.ConVarNamingApplicable,
                ConVarNamingFound = result.ConVarNamingFound,
                ConVarNamingRenamedObjects = result.ConVarNamingRenamedObjects,
                ConVarNamingRenamedHandlers = result.ConVarNamingRenamedHandlers,
                FnPtrNamingFound = result.FnPtrNamingFound,
                FnPtrNamingRenamed = result.FnPtrNamingRenamed,
                ProtoImportApplicable = result.ProtoImportApplicable,
                ProtoTypesDefined = result.ProtoTypesDefined,
                ProtoImportErrors = result.ProtoImportErrors,
            });
        }
        catch (Exception ex)
        {
            Send(new WireMessage { Kind = WireKind.Failed, Error = ex.Message });
        }
    }

    private static void Send(WireMessage message)
    {
        Console.Out.WriteLine(WorkerProtocol.Write(message));
        Console.Out.Flush();
    }
}
