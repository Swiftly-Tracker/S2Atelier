using S2Atelier.Ida.Generated;

namespace S2Atelier;

internal sealed class CliOptions
{
    public List<string> Globs { get; } = [];

    public string? IdaPath { get; private set; }

    public IdaSdkVersion SdkVersion { get; private set; } = IdaSdkVersion.Auto;

    public int Cores { get; private set; } = Environment.ProcessorCount;

    public bool NoSave { get; private set; }

    public bool Progress { get; private set; }

    public bool PatchPlt { get; private set; }

    public bool NameConVars { get; private set; }

    public bool NameFnPtrTables { get; private set; }

    public string? ImportProtobufsDir { get; private set; }

    public string? ImportSchemaPath { get; private set; }

    public string? Hl2SdkPath { get; private set; }

    public string SchemaProject { get; private set; } = "auto";

    public string? Root { get; private set; }

    public bool IsWorker { get; private set; }

    public bool ShowHelp { get; private set; }

    public string? ParseError { get; private set; }

    public static CliOptions Parse(string[] args)
    {
        var options = new CliOptions();

        options.IdaPath = Environment.GetEnvironmentVariable("IDA_PATH");

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--ida-worker":
                    options.IsWorker = true;
                    break;

                case "--ida-path" when i + 1 < args.Length:
                    options.IdaPath = args[++i];
                    break;

                case "--ida-sdk" when i + 1 < args.Length:
                    options.SdkVersion = ParseSdkVersion(args[++i]);
                    break;

                case "--cores" when i + 1 < args.Length:
                    if (int.TryParse(args[++i], out int cores) && cores > 0)
                    {
                        options.Cores = cores;
                    }
                    break;

                case "--root" when i + 1 < args.Length:
                    options.Root = args[++i];
                    break;

                case "--no-save":
                    options.NoSave = true;
                    break;

                case "--progress":
                    options.Progress = true;
                    break;

                case "--patch-plt":
                    options.PatchPlt = true;
                    break;

                case "--name-convars":
                    options.NameConVars = true;
                    break;

                case "--name-fnptr-tables":
                    options.NameFnPtrTables = true;
                    break;

                case "--import-protobufs" when i + 1 < args.Length:
                    options.ImportProtobufsDir = args[++i];
                    break;

                case "--import-schema" when i + 1 < args.Length:
                    options.ImportSchemaPath = args[++i];
                    break;

                case "--import-schema":
                    options.ParseError = "--import-schema requires an sdk.json path.";
                    break;

                case "--hl2sdk" when i + 1 < args.Length:
                    options.Hl2SdkPath = args[++i];
                    break;

                case "--hl2sdk":
                    options.ParseError = "--hl2sdk requires a directory path.";
                    break;

                case "--schema-project" when i + 1 < args.Length:
                    options.SchemaProject = args[++i];
                    break;

                case "--schema-project":
                    options.ParseError = "--schema-project requires 'auto' or a project name.";
                    break;

                case "-h" or "--help":
                    options.ShowHelp = true;
                    break;

                default:
                    options.Globs.Add(args[i]);
                    break;
            }
        }

        return options;
    }

    public bool ValidateSchemaOptions(out string? error)
    {
        error = null;
        if ((ImportSchemaPath == null) != (Hl2SdkPath == null))
        {
            error = "--import-schema and --hl2sdk must be provided together.";
            return false;
        }
        if (ImportSchemaPath == null)
        {
            if (!SchemaProject.Equals("auto", StringComparison.OrdinalIgnoreCase))
            {
                error = "--schema-project requires --import-schema and --hl2sdk.";
                return false;
            }
            return true;
        }

        try
        {
            ImportSchemaPath = Path.GetFullPath(ImportSchemaPath);
            Hl2SdkPath = Path.GetFullPath(Hl2SdkPath!);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            error = $"Invalid schema/HL2SDK path: {ex.Message}";
            return false;
        }
        if (!File.Exists(ImportSchemaPath))
        {
            error = $"Schema JSON does not exist: '{ImportSchemaPath}'.";
            return false;
        }
        if (!Directory.Exists(Hl2SdkPath))
        {
            error = $"HL2SDK directory does not exist: '{Hl2SdkPath}'.";
            return false;
        }
        foreach (string required in new[]
                 {
                     "public", Path.Combine("game", "shared"), Path.Combine("game", "server"),
                     Path.Combine("thirdparty", "protobuf-3.21.8", "src"), "common",
                 })
        {
            if (!Directory.Exists(Path.Combine(Hl2SdkPath, required)))
            {
                error = $"HL2SDK is missing required directory '{required}': '{Hl2SdkPath}'.";
                return false;
            }
        }
        if (string.IsNullOrWhiteSpace(SchemaProject))
        {
            error = "--schema-project must be 'auto' or a non-empty project name.";
            return false;
        }
        SchemaProject = SchemaProject.Trim();

        try
        {
            var database = S2Atelier.Ida.Schema.SchemaDatabase.Load(ImportSchemaPath);
            if (!SchemaProject.Equals("auto", StringComparison.OrdinalIgnoreCase) && !database.HasProject(SchemaProject))
            {
                error = $"Schema project '{SchemaProject}' does not exist in '{ImportSchemaPath}'.";
                return false;
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or
                                   S2Atelier.Ida.Schema.SchemaFormatException or System.Text.Json.JsonException)
        {
            error = $"Cannot use schema JSON '{ImportSchemaPath}': {ex.Message}";
            return false;
        }

        return true;
    }

    private static IdaSdkVersion ParseSdkVersion(string value)
    {
        string normalized = value.Trim().Replace(".", "", StringComparison.Ordinal).ToUpperInvariant();

        if (normalized is "AUTO" or "")
        {
            return IdaSdkVersion.Auto;
        }

        if (!normalized.StartsWith('V'))
        {
            normalized = "V" + normalized;
        }

        return Enum.TryParse(normalized, out IdaSdkVersion sdk) ? sdk : IdaSdkVersion.Auto;
    }
}
