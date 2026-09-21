using System.Text.Json;

namespace S2Atelier.Ida;

/// <summary>
/// What each pass found in a module, and what it had to hold back. Written next to the vtable snapshot, it is
/// the next build's baseline: a count that falls well below the previous build's means a pass stopped
/// recognising a code shape (or the SDK stopped parsing), not that the game lost those things. Warnings are
/// the SDK disagreements of this build: drifted vtables, layouts and entity class types.
/// </summary>
internal sealed class ModuleHealth(string module)
{
    // A count regresses when it falls by more than this share and by at least MinimumDrop.
    private const int RegressionPercent = 10;
    private const int MinimumDrop = 5;

    internal sealed record Report(string Module, SortedDictionary<string, int> Counts, List<string> Warnings,
        List<string> Regressions);

    private readonly SortedDictionary<string, int> _counts = new(StringComparer.Ordinal);
    private readonly List<string> _warnings = [];

    internal static string FileName(string module) => module + ".health.json";

    internal void Count(string name, int value) => _counts[name] = value;

    internal void Warn(string category, IEnumerable<string> messages)
        => _warnings.AddRange(messages.Select(x => $"[{category}] {x}"));

    /// <summary>The counts that fell well below the baseline's, one line each.</summary>
    internal List<string> Regressions(Report? baseline)
    {
        var regressions = new List<string>();
        foreach ((string name, int before) in baseline?.Counts ?? [])
        {
            int now = _counts.GetValueOrDefault(name);
            if (before - now >= MinimumDrop && (long)(before - now) * 100 > (long)before * RegressionPercent)
            {
                regressions.Add($"{name}: {before} -> {now} ({(now - before) * 100 / before}%)");
            }
        }

        return regressions;
    }

    internal static Report? Load(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try { return JsonSerializer.Deserialize<Report>(File.ReadAllText(path)); }
        catch (JsonException) { return null; }
    }

    internal Report Write(string path, Report? baseline)
    {
        var report = new Report(module, _counts, _warnings, Regressions(baseline));
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        File.WriteAllText(path, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));
        return report;
    }
}
