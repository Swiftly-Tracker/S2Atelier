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
                message.ImportProtobufsDir, message.ImportSchemaPath, message.Hl2SdkPath, message.SchemaProject,
                message.ImportInterfaces);
        }

        return 0;
    }

    private static void RunJob(
        string path, bool save, bool patchPlt, bool nameConVars, bool nameFnPtrTables, string? importProtobufsDir,
        string? importSchemaPath, string? hl2SdkPath, string schemaProject, bool importInterfaces)
    {
        if (!File.Exists(path))
        {
            Send(new WireMessage { Kind = WireKind.Failed, Error = $"No such file: '{path}'." });
            return;
        }

        try
        {
            var result = IdaKernel.Open(path, save, patchPlt, nameConVars, nameFnPtrTables, importProtobufsDir,
                importSchemaPath, hl2SdkPath, schemaProject,
                (fraction, address) =>
                    Send(new WireMessage { Kind = WireKind.Progress, Fraction = fraction, Address = address }),
                importInterfaces);

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
                InterfaceImportApplicable = result.InterfaceImportApplicable,
                InterfaceGlobalsFound = result.InterfaceGlobalsFound,
                InterfaceGlobalsRenamed = result.InterfaceGlobalsRenamed,
                InterfaceTypesApplied = result.InterfaceTypesApplied,
                InterfaceVTablesImported = result.InterfaceVTablesImported,
                InterfaceImportSkipped = result.InterfaceImportSkipped,
                InterfaceClangErrors = result.InterfaceClangErrors,
                SchemaImportApplicable = result.SchemaImportApplicable,
                ImportedSchemaProject = result.SchemaProject,
                SchemaTypesImported = result.SchemaTypesImported,
                SchemaVTablesMatched = result.SchemaVTablesMatched,
                SchemaFunctionsBound = result.SchemaFunctionsBound,
                SchemaFunctionsSkipped = result.SchemaFunctionsSkipped,
                SchemaFunctionConflicts = result.SchemaFunctionConflicts,
                SchemaClangErrors = result.SchemaClangErrors,
                SchemaVTableTypesCompleted = result.SchemaVTableTypesCompleted,
                SchemaVTableAddressesBound = result.SchemaVTableAddressesBound,
                SchemaVTableUnknownSlots = result.SchemaVTableUnknownSlots,
                SchemaVTableConflicts = result.SchemaVTableConflicts,
            });
        }
        catch (SchemaImportException ex)
        {
            Send(new WireMessage { Kind = WireKind.Failed, Error = ex.Message, SchemaClangErrors = ex.ClangErrors });
        }
        catch (ValveInterfaceImportException ex)
        {
            Send(new WireMessage { Kind = WireKind.Failed, Error = ex.Message, InterfaceClangErrors = ex.ClangErrors });
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
