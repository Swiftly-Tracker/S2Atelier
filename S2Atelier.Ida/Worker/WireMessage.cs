using System.Text.Json.Serialization;

namespace S2Atelier.Ida.Worker;

internal enum WireKind { Ready, Job, Progress, Done, Failed, Quit }

internal sealed class WireMessage
{
    public required WireKind Kind { get; init; }

    public string? Path { get; init; }
    public bool Save { get; init; }
    public bool PatchPlt { get; init; }
    public bool NameConVars { get; init; }
    public bool NameFnPtrTables { get; init; }
    public string? ImportProtobufsDir { get; init; }
    public string? ImportSchemaPath { get; init; }
    public string? ConVarTypesPath { get; init; }
    public bool NameLogChannels { get; init; }
    public string? VTableBaselineDirectory { get; init; }
    public string? VTableSnapshotDirectory { get; init; }
    public bool NameEntityClasses { get; init; }
    public string? Hl2SdkPath { get; init; }
    public bool ImportInterfaces { get; init; }
    public string SchemaProject { get; init; } = "auto";

    public string? Sdk { get; init; }

    public double Fraction { get; init; }
    public string? Stage { get; init; }
    public ulong Address { get; init; }

    public int Functions { get; init; }
    public int Segments { get; init; }
    public int Strings { get; init; }
    public double Milliseconds { get; init; }
    public bool PltPatchApplicable { get; init; }
    public int PltPatched { get; init; }
    public int PltUnresolved { get; init; }
    public bool ConVarNamingApplicable { get; init; }
    public int ConVarNamingFound { get; init; }
    public int ConVarNamingRenamedObjects { get; init; }
    public int ConVarNamingRenamedHandlers { get; init; }
    public int ConVarNamingTypedObjects { get; init; }
    public int FnPtrNamingFound { get; init; }
    public int FnPtrNamingRenamed { get; init; }
    public bool LogChannelNamingApplicable { get; init; }
    public int LogChannelNamingFound { get; init; }
    public int LogChannelNamingRenamed { get; init; }
    public bool ProtoImportApplicable { get; init; }
    public int ProtoTypesDefined { get; init; }
    public int ProtoImportErrors { get; init; }
    public bool InterfaceImportApplicable { get; init; }
    public int InterfaceGlobalsFound { get; init; }
    public int InterfaceGlobalsRenamed { get; init; }
    public int InterfaceTypesApplied { get; init; }
    public int InterfaceVTablesImported { get; init; }
    public int InterfaceImportSkipped { get; init; }
    public int InterfaceClangErrors { get; init; }
    public bool SchemaImportApplicable { get; init; }
    public string? ImportedSchemaProject { get; init; }
    public int SchemaTypesImported { get; init; }
    public int SchemaVTablesMatched { get; init; }
    public int SchemaFunctionsBound { get; init; }
    public int SchemaFunctionsSkipped { get; init; }
    public int SchemaFunctionConflicts { get; init; }
    public int SchemaClangErrors { get; init; }
    public int SchemaVTableTypesCompleted { get; init; }
    public int SchemaVTableAddressesBound { get; init; }
    public int SchemaVTableUnknownSlots { get; init; }
    public int SchemaVTableConflicts { get; init; }

    public string? Error { get; init; }
}

[JsonSerializable(typeof(WireMessage))]
internal partial class WireJsonContext : JsonSerializerContext;
