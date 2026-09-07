using S2Atelier.Ida;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

var tests = new (string Name, Action Run)[]
{
    ("flat sdk parse and selection", TestSelection),
    ("conflicting duplicate fails", TestConflict),
    ("golden header features", TestHeader),
    ("vtable ABI names", TestVTableNames),
    ("vtable table boundaries", TestVTableBoundaries),
    ("inheritance ownership", TestOwnership),
    ("function prototype rewrite", TestPrototypeRewrite),
    ("function binding statistics", TestFunctionBindingStatistics),
    ("repository sdk smoke", TestRepositorySdk),
    ("gated IDA schema integration", TestGatedIdaIntegration),
};

int failures = 0;
foreach ((string name, Action run) in tests)
{
    try
    {
        run();
        Console.WriteLine($"PASS {name}");
    }
    catch (Exception ex)
    {
        failures++;
        Console.Error.WriteLine($"FAIL {name}: {ex.Message}");
    }
}
return failures == 0 ? 0 : 1;

static void TestSelection()
{
    using TempJson fixture = new(TestData.FixtureJson);
    SchemaDatabase database = SchemaDatabase.Load(fixture.Path);
    SchemaSelection client = database.Select("auto", @"C:\game\client.dll")!;
    Equal("client", client.Project);
    Equal(24, client.Classes["Shared_t"].Size);
    True(client.Classes.ContainsKey("MissingOuter") && client.Classes["MissingOuter"].Synthetic);
    True(client.Classes.ContainsKey("MissingOuter::Inner"));
    Equal(ulong.MaxValue, client.Classes["Base"].NameHash);

    SchemaSelection server = database.Select("auto", "/game/libserver.so")!;
    Equal("server", server.Project);
    Equal(32, server.Classes["Shared_t"].Size);
    True(database.Select("auto", "/game/libnot_a_project.so") == null);
}

static void TestConflict()
{
    const string json = """
        {"classes":[
          {"name":"C","name_hash":1,"project":"client","size":4,"alignment":4,"is_struct":true,"has_chainer":false,"fields_count":0,"fields":[]},
          {"name":"C","name_hash":1,"project":"client","size":8,"alignment":4,"is_struct":true,"has_chainer":false,"fields_count":0,"fields":[]}
        ],"enums":[]}
        """;
    using TempJson fixture = new(json);
    Throws<SchemaFormatException>(() => SchemaDatabase.Load(fixture.Path));
}

static void TestHeader()
{
    using TempJson fixture = new(TestData.FixtureJson);
    SchemaSelection selection = SchemaDatabase.Load(fixture.Path).Select("client", "client.dll")!;
    var polymorphic = new HashSet<string>(StringComparer.Ordinal) { "Derived" };
    string header = SchemaHeaderGenerator.Generate(selection, SchemaTargetPlatform.WindowsMsvc, polymorphic).Text;
    Contains(header, "void *__vftable;");
    Equal(1, Count(header, "void *__vftable;"));
    Contains(header, "class MissingOuter");
    Contains(header, "class alignas(8) Inner");
    Contains(header, "static MissingOuter::Inner __s2_force_nested_MissingOuter_0;");
    Contains(header, "m_grid[3][2]");
    Contains(header, "m_flagA : 1");
    Contains(header, "CUtlDelegate<R(A1, A2, A3, A4, A5)>");
    Contains(header, "#include \"Color.h\"");
    Contains(header, "#include \"tier1/KeyValues.h\"");
    Contains(header, "#include \"shareddefs.h\"");
    Contains(header, "struct alignas(8) __s2_opaque_MysteryAtomic_5_8_8");
    Contains(header, "GOOGLE_PROTOBUF_INCLUDED_network_5fconnection_2eproto");
    True(!header.Contains("#pragma once", StringComparison.Ordinal));
    Contains(header, "static_assert(offsetof(Base, __vftable) == 0)");
    Contains(header, "static_assert(sizeof(Derived) == 40)");
    Contains(header, "class alignas(8) MultiDerived : public Base, public Unrelated");
    Contains(header, "__s2_opaque_MysteryAtomic_5_8_8");

    SchemaSelection server = SchemaDatabase.Load(fixture.Path).Select("server", "server.dll")!;
    string linux = SchemaHeaderGenerator.Generate(server, SchemaTargetPlatform.LinuxItanium).Text;
    Contains(linux, "ELF x64 schema import requires 64-bit pointers");
    Contains(linux, "static_assert(sizeof(Shared_t) == 32)");
}

static void TestVTableNames()
{
    True(VTableAnalysis.TryDescribe(0x1000, "??_7Derived@@6BBase@@@",
        "const Derived::`vftable'{for `Base'}", out VTableDescriptor msvc));
    Equal("Derived", msvc.ClassName);
    Equal("Base", msvc.SecondaryBaseName);

    True(VTableAnalysis.TryDescribe(0x2000, "_ZTVN3foo7DerivedE", "vtable for foo::Derived",
        out VTableDescriptor itanium));
    Equal("foo::Derived", itanium.ClassName);
    Equal(VTableAbi.Itanium, itanium.Abi);
}

static void TestVTableBoundaries()
{
    var msvc = new FakeVTableMemory(
        new Dictionary<ulong, ulong> { [0x1000] = 0x5000, [0x1008] = 0x5010, [0x1010] = 0xDEAD },
        new HashSet<ulong> { 0x5000, 0x5010 }, new HashSet<ulong>());
    Equal("20480,20496", string.Join(',', VTableEntryScanner.ScanMsvc(msvc, 0x1000)));

    // offset/typeinfo, primary function, secondary offset/typeinfo, secondary function, boundary.
    var itanium = new FakeVTableMemory(
        new Dictionary<ulong, ulong>
        {
            [0x2000] = 0,
            [0x2008] = 0x3000,
            [0x2010] = 0x6000,
            [0x2018] = unchecked((ulong)-8L),
            [0x2020] = 0x3000,
            [0x2028] = 0x6010,
            [0x2030] = 0,
        },
        new HashSet<ulong> { 0x6000, 0x6010 }, new HashSet<ulong> { 0x3000 });
    Equal("24576,24592", string.Join(',', VTableEntryScanner.ScanItanium(itanium, 0x2000)));
    IReadOnlyList<VTableSlice> slices = VTableEntryScanner.ScanItaniumTables(itanium, 0x2000);
    Equal(2, slices.Count);
    Equal(0L, slices[0].OffsetToTop);
    Equal(-8L, slices[1].OffsetToTop);
}

static void TestOwnership()
{
    using TempJson fixture = new(TestData.FixtureJson);
    SchemaSelection selection = SchemaDatabase.Load(fixture.Path).Select("client", "client.dll")!;
    Equal("Base", VTableAnalysis.ResolveSharedFunctionOwner(["Derived", "Base"], selection.Classes));
    Equal("Base", VTableAnalysis.ResolveSharedFunctionOwner(["Derived", "DerivedSibling"], selection.Classes));
    True(VTableAnalysis.ResolveSharedFunctionOwner(["Base", "Unrelated"], selection.Classes) == null);
}

static void TestPrototypeRewrite()
{
    Equal("long __fastcall __s2_vfunc_marker(Derived *__s2_this, int value, void (*cb)(int, int));",
        VTableAnalysis.RewriteFirstParameter(
            "long __fastcall __s2_vfunc_marker(Base *old, int value, void (*cb)(int, int));",
            "__s2_vfunc_marker", "Derived", 3));
    Equal("void __cdecl __s2_vfunc_marker(Base *__s2_this);",
        VTableAnalysis.RewriteFirstParameter("void __cdecl __s2_vfunc_marker(void);",
            "__s2_vfunc_marker", "Base", 0));
    Equal("void __cdecl __s2_vfunc_marker(Base *__s2_this, long x);",
        VTableAnalysis.RewriteFirstParameter("void __cdecl __s2_vfunc_marker(void (*cb)(int, int), long x);",
            "__s2_vfunc_marker", "Base", 2));
}

static void TestFunctionBindingStatistics()
{
    using TempJson fixture = new(TestData.FixtureJson);
    SchemaSelection selection = SchemaDatabase.Load(fixture.Path).Select("client", "client.dll")!;
    var owners = new Dictionary<ulong, HashSet<string>>
    {
        [30] = new(StringComparer.Ordinal) { "Base" },
        [10] = new(StringComparer.Ordinal) { "Derived", "Base" },
        [40] = new(StringComparer.Ordinal) { "Base" },
        [20] = new(StringComparer.Ordinal) { "Base", "Unrelated" },
    };
    var editor = new FakeFunctionTypeEditor(new HashSet<ulong> { 30 });
    VTableBindingSummary result = VTableFunctionBinder.Bind(owners,
        new HashSet<ulong> { 40 }, new HashSet<ulong>(), selection.Classes, editor);
    Equal(1, result.Bound);
    Equal(2, result.Skipped);
    Equal(1, result.Conflicts);
    Equal("10:Base,30:Base", string.Join(',', editor.Attempts.Select(x => $"{x.Address}:{x.Owner}")));
}

static void TestRepositorySdk()
{
    string path = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "temp", "sdk.json"));
    if (!File.Exists(path))
    {
        return;
    }
    SchemaSelection selection = SchemaDatabase.Load(path).Select("client", "client.dll")!;
    SchemaHeaderResult generated = SchemaHeaderGenerator.Generate(selection, SchemaTargetPlatform.WindowsMsvc);
    True(selection.TypeCount > 800);
    True(!selection.SyntheticClassNames.Contains("std"));
    True(generated.Text.Length > 100_000);
}

static void TestGatedIdaIntegration()
{
    string? idaPath = Environment.GetEnvironmentVariable("IDA_PATH");
    string? hl2Sdk = Environment.GetEnvironmentVariable("HL2SDK_PATH") ?? @"D:\Code\hl2sdk";
    string sdkJson = Environment.GetEnvironmentVariable("S2ATELIER_SDK_JSON") ??
        System.IO.Path.GetFullPath(System.IO.Path.Combine(AppContext.BaseDirectory,
            "..", "..", "..", "..", "temp", "sdk.json"));
    string?[] binaries =
    [
        Environment.GetEnvironmentVariable("S2ATELIER_TEST_CLIENT"),
        Environment.GetEnvironmentVariable("S2ATELIER_TEST_SERVER"),
        Environment.GetEnvironmentVariable("S2ATELIER_TEST_ELF"),
    ];
    if (string.IsNullOrWhiteSpace(idaPath) || !Directory.Exists(hl2Sdk) || !File.Exists(sdkJson) ||
        binaries.All(string.IsNullOrWhiteSpace))
    {
        return;
    }
    if (!IdaKernel.TryInitialize(idaPath, IdaSdkVersion.Auto, out string? error))
    {
        throw new Exception(error);
    }
    foreach (string binary in binaries.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!))
    {
        IdaAnalysisResult result = IdaKernel.Open(binary, save: false, importSchemaPath: sdkJson,
            hl2SdkPath: hl2Sdk, schemaProject: "auto");
        True(result.SchemaImportApplicable);
        Equal(0, result.SchemaClangErrors);
        True(result.SchemaTypesImported > 0);
        True(result.SchemaVTablesMatched > 0);
    }
}

static int Count(string value, string needle)
{
    int count = 0;
    int offset = 0;
    while ((offset = value.IndexOf(needle, offset, StringComparison.Ordinal)) >= 0)
    {
        count++;
        offset += needle.Length;
    }
    return count;
}

static void Contains(string value, string needle)
{
    if (!value.Contains(needle, StringComparison.Ordinal))
    {
        throw new Exception($"Expected output to contain '{needle}'.");
    }
}

static void True(bool value)
{
    if (!value) throw new Exception("Expected true.");
}

static void Equal<T>(T expected, T actual)
{
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
        throw new Exception($"Expected '{expected}', got '{actual}'.");
    }
}

static void Throws<T>(Action action) where T : Exception
{
    try
    {
        action();
    }
    catch (T)
    {
        return;
    }
    throw new Exception($"Expected {typeof(T).Name}.");
}

static class TestData
{
    public const string FixtureJson = """
    {
      "classes": [
        {
          "name":"Base","name_hash":18446744073709551615,"project":"client","size":16,"alignment":8,
          "is_struct":false,"has_chainer":false,"base_classes_count":0,"base_classes":[],"fields_count":1,
          "fields":[{"name":"m_value","name_hash":1,"kind":"ref","type":"int32","offset":8,"size":4,"alignment":4,"networked":false}]
        },
        {
          "name":"Derived","name_hash":2,"project":"client","size":40,"alignment":8,
          "is_struct":false,"has_chainer":false,"base_classes_count":1,"base_classes":["Base"],"fields_count":1,
          "fields":[{"name":"m_grid","name_hash":2,"kind":"fixed_array","type":"uint32[2]","offset":16,"size":24,"alignment":4,"element_count":3,"element_size":8,"element_alignment":4,"networked":false}]
        },
        {
          "name":"Flags","name_hash":3,"project":"client","size":2,"alignment":1,"is_struct":true,"has_chainer":false,
          "fields_count":3,"fields":[
            {"name":"m_flagA","name_hash":3,"kind":"bitfield","type":"bitfield","offset":0,"size":0,"alignment":255,"count":1,"networked":false},
            {"name":"m_flagB","name_hash":4,"kind":"bitfield","type":"bitfield","offset":0,"size":0,"alignment":255,"count":7,"networked":false},
            {"name":"m_tail","name_hash":5,"kind":"ref","type":"uint8","offset":1,"size":1,"alignment":1,"networked":false}
          ]
        },
        {
          "name":"Atoms","name_hash":4,"project":"client","size":16,"alignment":8,"is_struct":true,"has_chainer":false,
          "fields_count":2,"fields":[
            {"name":"m_atom","name_hash":6,"kind":"atomic","type":"MysteryAtomic","templated":"MysteryAtomic< 5 >","template":[{"type":"literal","value":5}],"offset":0,"size":8,"alignment":8,"networked":false},
            {"name":"m_pointer","name_hash":7,"kind":"ptr","type":"Base","offset":8,"size":8,"alignment":8,"networked":false}
          ]
        },
        {"name":"MissingOuter::Inner","name_hash":5,"project":"client","size":8,"alignment":8,"is_struct":true,"has_chainer":false,"fields_count":1,
         "fields":[{"name":"m_e","name_hash":8,"kind":"ref","type":"WideEnum","offset":0,"size":8,"alignment":8,"networked":false}]},
        {"name":"Shared_t","name_hash":6,"project":"client","size":24,"alignment":8,"is_struct":true,"has_chainer":false,"fields_count":0,"fields":[]},
        {"name":"Shared_t","name_hash":6,"project":"server","size":32,"alignment":8,"is_struct":true,"has_chainer":false,"fields_count":0,"fields":[]},
        {"name":"Unrelated","name_hash":7,"project":"client","size":8,"alignment":8,"is_struct":true,"has_chainer":false,"fields_count":0,"fields":[]},
        {"name":"MultiDerived","name_hash":9,"project":"client","size":24,"alignment":8,"is_struct":false,"has_chainer":false,"base_classes_count":2,"base_classes":["Base","Unrelated"],"fields_count":0,"fields":[]},
        {"name":"DerivedSibling","name_hash":10,"project":"client","size":16,"alignment":8,"is_struct":false,"has_chainer":false,"base_classes_count":1,"base_classes":["Base"],"fields_count":0,"fields":[]},
        {"name":"Base","name_hash":18446744073709551615,"project":"client","size":16,"alignment":8,
          "is_struct":false,"has_chainer":false,"base_classes_count":0,"base_classes":[],"fields_count":1,
          "fields":[{"name":"m_value","name_hash":1,"kind":"ref","type":"int32","offset":8,"size":4,"alignment":4,"networked":false}]}
      ],
      "enums": [
        {"name":"WideEnum","name_hash":8,"project":"client","size":8,"alignment":8,"fields_count":2,
         "fields":[{"name":"Negative","value":-1},{"name":"Huge","value":18446744073709551615}]}
      ]
    }
    """;
}

sealed class TempJson : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"s2atelier-test-{Guid.NewGuid():N}.json");
    public TempJson(string text) => File.WriteAllText(Path, text);
    public void Dispose() => File.Delete(Path);
}

sealed class FakeVTableMemory(
    IReadOnlyDictionary<ulong, ulong> pointers,
    IReadOnlySet<ulong> functions,
    IReadOnlySet<ulong> mapped) : IVTableMemory
{
    public ulong ReadPointer(ulong address) => pointers.TryGetValue(address, out ulong value) ? value : 0;
    public bool IsMapped(ulong address) => mapped.Contains(address) || functions.Contains(address);
    public bool IsFunctionStart(ulong address) => functions.Contains(address);
}

sealed class FakeFunctionTypeEditor(IReadOnlySet<ulong> failures) : IVirtualFunctionTypeEditor
{
    public List<(ulong Address, string Owner)> Attempts { get; } = [];

    public bool TryBindThis(ulong address, string owner)
    {
        Attempts.Add((address, owner));
        return !failures.Contains(address);
    }
}
