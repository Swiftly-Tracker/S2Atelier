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

    public string? Root { get; private set; }

    public bool IsWorker { get; private set; }

    public bool ShowHelp { get; private set; }

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
