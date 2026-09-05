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

    public string? Sdk { get; init; }

    public double Fraction { get; init; }
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
    public int FnPtrNamingFound { get; init; }
    public int FnPtrNamingRenamed { get; init; }
    public bool ProtoImportApplicable { get; init; }
    public int ProtoTypesDefined { get; init; }
    public int ProtoImportErrors { get; init; }

    public string? Error { get; init; }
}

[JsonSerializable(typeof(WireMessage))]
internal partial class WireJsonContext : JsonSerializerContext;
