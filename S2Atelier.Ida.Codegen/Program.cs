using S2Atelier.Ida.Codegen;

string sdkRoot = ReadOption(args, "--sdk-root") ?? Path.Combine("thirdparty", "ida-sdk");
string outDir = ReadOption(args, "--out") ?? Path.Combine("S2Atelier.Ida", "Generated");

if (!Directory.Exists(sdkRoot))
{
    Console.Error.WriteLine($"SDK root '{sdkRoot}' does not exist. Vendor at least one IDA SDK " +
        "version under it first (a 'major.minor' directory with an 'include' subdirectory).");
    return 1;
}

Directory.CreateDirectory(outDir);

var versionDirs = Directory.GetDirectories(sdkRoot)
    .Where(d => Directory.Exists(Path.Combine(d, "include")))
    .OrderBy(d => d, StringComparer.Ordinal)
    .ToList();

if (versionDirs.Count == 0)
{
    Console.Error.WriteLine($"No vendored SDK versions found under '{sdkRoot}'.");
    return 1;
}

var report = new List<string>();
var versionMembers = new List<string>();

var byName = new Dictionary<string, Dictionary<string, EmittedSymbol>>();

foreach (string versionDir in versionDirs)
{
    string versionName = Path.GetFileName(versionDir);
    string member = "V" + versionName.Replace(".", "", StringComparison.Ordinal);
    string includeDir = Path.Combine(versionDir, "include");
    versionMembers.Add(member);

    var headerFiles = Directory.GetFiles(includeDir, "*.h*")
        .Where(f => f.EndsWith(".hpp", StringComparison.Ordinal) || f.EndsWith(".h", StringComparison.Ordinal))
        .OrderBy(f => f, StringComparer.Ordinal)
        .ToList();

    var filteredByHeader = new Dictionary<string, string>();
    foreach (string path in headerFiles)
    {
        string text = ConditionalFilter.Strip64BitOnly(File.ReadAllText(path));
        text = System.Text.RegularExpressions.Regex.Replace(text, "//[^\n]*", "");
        text = MacroExpander.ExpandExportMacros(text);
        filteredByHeader[Path.GetFileName(path)] = text;
    }

    var (aliases, enums) = TypedefIndex.Build(filteredByHeader.Values);
    var resolver = new TypeResolver(aliases, enums);

    var thisVersion = new Dictionary<string, EmittedSymbol>();
    int excluded = 0;

    foreach (var (headerName, text) in filteredByHeader)
    {
        string module = headerName == "idalib.hpp" ? "idalib" : "ida";

        foreach (var scan in HeaderScanner.ScanAll(text))
        {
            if (scan.IsExcluded)
            {
                report.Add($"[{versionName}] {headerName}: '{scan.Name}' skipped — {scan.ExcludeReason}");
                excluded++;
                continue;
            }

            var decl = scan.Decl!.Value;
            var retRes = resolver.Resolve(decl.ReturnType);
            if (retRes.IsExcluded)
            {
                report.Add($"[{versionName}] {headerName}: '{scan.Name}' skipped — return type: {retRes.ExcludeReason}");
                excluded++;
                continue;
            }

            var paramTypes = new List<string>(decl.ParamTypes.Count);
            string? paramFailure = null;

            foreach (string rawParam in decl.ParamTypes)
            {
                var pr = resolver.Resolve(rawParam);
                if (pr.IsExcluded)
                {
                    paramFailure = pr.ExcludeReason;
                    break;
                }

                paramTypes.Add(pr.CSharpType!);
            }

            if (paramFailure != null)
            {
                report.Add($"[{versionName}] {headerName}: '{scan.Name}' skipped — parameter type: {paramFailure}");
                excluded++;
                continue;
            }

            var symbol = new EmittedSymbol(scan.Name, module, retRes.CSharpType!, paramTypes);

            if (thisVersion.TryGetValue(scan.Name, out var existing))
            {
                if (existing.PointerType != symbol.PointerType)
                {
                    report.Add($"[{versionName}] {headerName}: '{scan.Name}' redeclared with a different " +
                        $"signature ('{existing.PointerType}' vs '{symbol.PointerType}'); keeping the first.");
                }
                continue;
            }

            thisVersion[scan.Name] = symbol;
        }
    }

    foreach (var (name, symbol) in thisVersion)
    {
        if (!byName.TryGetValue(name, out var perVersion))
        {
            byName[name] = perVersion = [];
        }

        perVersion[member] = symbol;
    }

    report.Add($"{versionName} ({member}): {thisVersion.Count} symbols resolved, {excluded} declarations skipped, " +
        $"from {headerFiles.Count} headers.");
}

var accepted = new List<EmittedSymbol>();
var perVersionAccepted = versionMembers.ToDictionary(v => v, _ => new List<EmittedSymbol>());

foreach (var (name, perVersion) in byName.OrderBy(kv => kv.Key, StringComparer.Ordinal))
{
    var distinctShapes = perVersion.Values.Select(s => s.PointerType).Distinct().ToList();

    if (distinctShapes.Count > 1)
    {
        string detail = string.Join(", ", perVersion.Select(kv => $"{kv.Key}={kv.Value.PointerType}"));
        report.Add($"'{name}' excluded from the shared surface — signature differs between versions: {detail}");
        continue;
    }

    var canonical = perVersion.Values.First();
    accepted.Add(canonical with { });

    foreach (var (member, symbol) in perVersion)
    {
        perVersionAccepted[member].Add(symbol);
    }
}

Emitter.EmitSurface(outDir, accepted);

foreach (string member in versionMembers)
{
    Emitter.EmitBinder(outDir, member, perVersionAccepted[member]);
}

Emitter.EmitSdkVersion(outDir, versionMembers);
Emitter.EmitDispatch(outDir, versionMembers);

File.WriteAllText(
    Path.Combine(outDir, "_codegen-report.txt"),
    "S2Atelier.Ida.Codegen report\n" + string.Join('\n', report) + '\n');

Console.WriteLine($"{accepted.Count} symbols on the shared surface across {versionMembers.Count} version(s).");
Console.WriteLine($"Wrote generated bindings to '{outDir}' ({report.Count} report lines; see _codegen-report.txt).");

return 0;

static string? ReadOption(string[] args, string name)
{
    int index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}
