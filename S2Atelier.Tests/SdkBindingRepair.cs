using System.Text.Json;
using S2Atelier.Ida;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

// Opt-in repair of an existing database COPY; no schema reimport or full analysis rerun.
internal static unsafe class SdkBindingRepair
{
    internal static int Run(string path)
    {
        string ida = Environment.GetEnvironmentVariable("IDA_PATH") ?? throw new Exception("IDA_PATH required");
        string sdk = Environment.GetEnvironmentVariable("HL2SDK_PATH") ?? throw new Exception("HL2SDK_PATH required");
        string json = Environment.GetEnvironmentVariable("S2ATELIER_SDK_JSON") ?? throw new Exception("S2ATELIER_SDK_JSON required");
        if (!IdaKernel.TryInitialize(ida, IdaSdkVersion.Auto, out var error)) throw new Exception(error);
        byte* native = Utf8.Allocate(path);
        try { if (IdaNative.open_database(native, 0, null) != 0) throw new Exception("Cannot open database copy"); }
        finally { Utf8.Free(native); }
        bool complete = false;
        try
        {
            IdaNative.auto_wait();
            if (Environment.GetEnvironmentVariable("S2ATELIER_VERIFY_BINDINGS") == "1")
            {
                using var report = JsonDocument.Parse(File.ReadAllText(path + ".repair.json"));
                int verified = 0;
                foreach (var change in report.RootElement.GetProperty("changes").EnumerateArray())
                {
                    ulong address = Convert.ToUInt64(change.GetProperty("address").GetString()![2..], 16);
                    string wanted = change.GetProperty("after").GetString()!;
                    if (SchemaVTableTypes.NameAt(address) != wanted) throw new Exception($"Saved name mismatch at {address:X}");
                    void* function = IdaNative.get_func(address);
                    if (function == null || *(ulong*)function != address) throw new Exception($"Not a function start: {address:X}");
                    verified++;
                }
                Console.WriteLine($"SAVED_REPAIR_VERIFIED names={verified}");
                return 0;
            }
            var selection = SchemaDatabase.Load(json).Select("server", path) ?? throw new Exception("server schema missing");
            var platform = SchemaImport.DetectTargetPlatform();
            var scan = SchemaImport.ScanVTables(selection, platform);
            var tables = scan.Tables.Select(SchemaVTableTypes.ResolveLayout).ToArray();
            using var definitions = Hl2SdkVTables.Load(sdk, platform, selection, tables, Console.Error.WriteLine);
            var slots = definitions.Resolve(tables, selection);
            var addresses = tables.SelectMany(t => t.Functions).Where(a => a != 0).Distinct().ToArray();
            var before = addresses.ToDictionary(a => a, SchemaVTableTypes.NameAt);
            var unresolved = tables.Where(t => t.ThisType == null).SelectMany(t => t.Functions).ToHashSet();
            var bound = SdkFunctionBinding.Bind(tables, slots, scan.PureCallAddresses, unresolved, Console.Error.WriteLine);
            var changes = addresses.Select(a => new { address = $"0x{a:X}", before = before[a], after = SchemaVTableTypes.NameAt(a) })
                .Where(c => c.before != c.after).ToArray();
            var network = tables.SelectMany(t => t.Functions.Select((a, index) => new
                { address = $"0x{a:X}", owner = t.ThisType, slot = slots.GetValueOrDefault((t.AddressPoint, index))?.Name,
                    name = SchemaVTableTypes.NameAt(a) })).Where(r => r.slot?.StartsWith("NetworkStateChanged") == true).ToArray();
            File.WriteAllText(path + ".repair.json", JsonSerializer.Serialize(new { binding = bound, changes, network },
                new JsonSerializerOptions { WriteIndented = true }));
            Console.WriteLine($"SDK_REPAIR renamed={changes.Length}, bound={bound.Bound}, conflicts={bound.Conflicts}");
            foreach (var change in changes.Where(c => c.after.Contains("NetworkStateChanged"))) Console.WriteLine(JsonSerializer.Serialize(change));
            complete = true;
        }
        finally { IdaNative.close_database(complete ? (byte)1 : (byte)0); }
        return 0;
    }
}
