using S2Atelier.Ida;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;
using S2Atelier.Ida.Worker;
using S2Atelier;

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
    ("Valve interface catalog", TestValveInterfaceCatalog),
    ("single Cvar slot with duplicate SDK rows", TestSingleCvarSlot),
    ("known interface implementations", TestKnownInterfaceImplementations),
    ("interface pointer comments", TestInterfacePointerComments),
    ("gated implementation vftable integration", TestGatedImplementationIntegration),
    ("Valve interface table detection", TestValveInterfaceTableDetection),
    ("interface CLI combinations", TestInterfaceCliCombinations),
    ("interface worker protocol", TestInterfaceWorkerProtocol),
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

static void TestValveInterfaceCatalog()
{
    const string interfaces = """
        #define ENGINE_VERSION "Engine001"
        DECLARE_TIER3_INTERFACE( IVEngineServer2, g_pEngineServer );
        #define CVAR_VERSION "Cvar001"
        DECLARE_TIER1_INTERFACE( ICvar, g_pCVar );
        #define NAMESPACED_VERSION "Namespaced001"
        DECLARE_TIER2_INTERFACE( valve::INamespaced, g_pNamespaced );
        #define FORWARD_VERSION "Forward001"
        DECLARE_TIER4_INTERFACE( IForwardOnly, g_pForwardOnly );
        """;
    const string source = """
        ICvar *cvar, *g_pCVar;
        InterfaceGlobals_t g_pInterfaceGlobals[] =
        {
            { ENGINE_VERSION, &g_pEngineServer },
            { CVAR_VERSION, &cvar },
            { CVAR_VERSION, &g_pCVar },
            { NAMESPACED_VERSION, &g_pNamespaced },
            { FORWARD_VERSION, &g_pForwardOnly },
        };
        """;
    var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["public/interfaces/interfaces.h"] = interfaces,
        ["public/eiface.h"] = "class IVEngineServer2;\nabstract_class IVEngineServer2 : public IBaseInterface { public: virtual int GetSteamUniverse() = 0; };",
        ["public/icvar.h"] = "// class ICvar { };\nclass ICvar { public: virtual void Register() = 0; };",
        ["game/shared/namespaced.h"] = "namespace valve { struct INamespaced { virtual void Run() = 0; }; }",
        ["public/forward.h"] = "class IForwardOnly;",
    };

    ValveInterfaceCatalog catalog = ValveInterfaceCatalog.Parse(interfaces, source, headers);
    Equal(5, catalog.Entries.Count);
    ValveInterfaceDefinition engine = catalog.Entries.Single(x => x.Version == "Engine001");
    Equal("IVEngineServer2", engine.ClassName);
    Equal("g_pEngineServer", engine.GlobalName);
    Equal("public/eiface.h", engine.DefinitionHeader);
    Equal("game/shared/namespaced.h",
        catalog.Entries.Single(x => x.ClassName == "valve::INamespaced").DefinitionHeader);
    True(catalog.Entries.Single(x => x.ClassName == "IForwardOnly").DefinitionHeader == null);
    Equal(5, catalog.TableSlots.Count);
    Equal("cvar", catalog.TableSlots[1].Definition!.GlobalName);
    Equal("ICvar", catalog.TableSlots[1].Definition!.ClassName);
    Equal("public/icvar.h", catalog.TableSlots[1].Definition!.DefinitionHeader);
    Equal("g_pCVar", catalog.TableSlots[2].Definition!.GlobalName);

    foreach (string unverifiedDeclaration in new[]
    {
        "", "// ICvar *cvar, *g_pCVar;", "/* ICvar *cvar, *g_pCVar; */",
        "IWrong *cvar;", "ICvar *cvar;\nIWrong *cvar;",
        "void f() {\nICvar *cvar;\n}", "struct Other {\nICvar *cvar;\n};",
    })
    {
        ValveInterfaceCatalog unverified = ValveInterfaceCatalog.Parse(interfaces,
            source.Replace("ICvar *cvar, *g_pCVar;", unverifiedDeclaration), headers);
        True(unverified.TableSlots[1].Definition == null);
        Equal("g_pCVar", unverified.TableSlots[2].Definition!.GlobalName);
    }

    const string ambiguous = """
        #define DUP_A "Duplicate001"
        DECLARE_TIER1_INTERFACE( IA, g_pA );
        #define DUP_B "Duplicate001"
        DECLARE_TIER1_INTERFACE( IB, g_pB );
        """;
    ValveInterfaceCatalog ambiguousCatalog = ValveInterfaceCatalog.Parse(ambiguous, null,
        new Dictionary<string, string> { ["public/interfaces/interfaces.h"] = ambiguous });
    var ambiguousTable = new ValveInterfaceTable(0x9000,
        [new ValveInterfaceTableRow(0x9000, "Duplicate001", 0xA000)]);
    True(ValveInterfaceTableResolver.Resolve(ambiguousTable, ambiguousCatalog)[0].Definition == null);

    string realSdk = Environment.GetEnvironmentVariable("HL2SDK_PATH") ?? @"D:\Code\hl2sdk";
    if (Directory.Exists(realSdk))
    {
        ValveInterfaceCatalog realCatalog = ValveInterfaceCatalog.Load(realSdk);
        ValveInterfaceDefinition realEngine = realCatalog.Entries
            .Single(x => x.Version == "Source2EngineToServer001");
        Equal("IVEngineServer2", realEngine.ClassName);
        Equal("g_pEngineServer", realEngine.GlobalName);
        True(realEngine.DefinitionHeader?.EndsWith("eiface.h", StringComparison.OrdinalIgnoreCase) == true);
        ValveInterfaceDefinition[] cvars = realCatalog.TableSlots
            .Where(x => x.Version == "VEngineCvar007").Select(x => x.Definition!).ToArray();
        Equal("cvar,g_pCVar", string.Join(',', cvars.Select(x => x.GlobalName)));
        True(cvars.All(x => x.ClassName == "ICvar"));
        var actualSingleSlot = new ValveInterfaceTable(0x181934640,
            [new ValveInterfaceTableRow(0x181934640, "VEngineCvar007", 0x1821D0E18)]);
        ValveInterfaceDefinition actualCvar = ValveInterfaceTableResolver.Resolve(actualSingleSlot, realCatalog)
            .Single().Definition!;
        Equal("g_pCVar", actualCvar.GlobalName);
        Equal("ICvar", actualCvar.ClassName);
        Equal("public/icvar.h", actualCvar.DefinitionHeader);
    }
}

static void TestSingleCvarSlot()
{
    const string interfaces = """
        #define APP_VERSION "VApplication001"
        DECLARE_TIER1_INTERFACE( IApplication, g_pApplication );
        #define CVAR_INTERFACE_VERSION "VEngineCvar007"
        DECLARE_TIER1_INTERFACE( ICVarWrong, wrongCvarName );
        #define TOKEN_VERSION "VStringTokenSystem001"
        DECLARE_TIER1_INTERFACE( ITokenSystem, g_pTokenSystem );
        #define TEST_VERSION "TestScriptMgr001"
        DECLARE_TIER1_INTERFACE( ITestScriptMgr, g_pTestScriptMgr );
        """;
    const string source = """
        ICvar *cvar, *g_pCVar;
        InterfaceGlobals_t g_pInterfaceGlobals[] = {
            { APP_VERSION, &g_pApplication },
            { CVAR_INTERFACE_VERSION, &cvar },
            { CVAR_INTERFACE_VERSION, &g_pCVar },
            { TOKEN_VERSION, &g_pTokenSystem },
            { TEST_VERSION, &g_pTestScriptMgr },
        };
        """;
    var headers = new Dictionary<string, string>
    {
        ["public/icvar.h"] = "abstract_class ICvar : public IAppSystem { public: virtual void Register() = 0; };",
        // The known include should remain unambiguous even with another class match.
        ["game/other.h"] = "class ICvar { };",
    };
    var memory = new FakeInterfaceMemory(
        new Dictionary<ulong, ulong>
        {
            [0x5000] = 0x1000,
            [0x5008] = 0x7000,
            [0x5010] = 0x1100,
            [0x5018] = 0x7010,
            [0x5020] = 0x1200,
            [0x5028] = 0x7020,
            [0x5030] = 0x1300,
            [0x5038] = 0x7030,
        },
        new Dictionary<ulong, IReadOnlyList<ulong>> { [0x1100] = [0x5010] },
        new HashSet<ulong> { 0x7000, 0x7010, 0x7020, 0x7030 },
        [(0x5000, 0x5040)]);
    var strings = new Dictionary<ulong, string>
    {
        [0x1000] = "VApplication001",
        [0x1100] = "VEngineCvar007",
        [0x1200] = "VStringTokenSystem001",
        [0x1300] = "TestScriptMgr001",
    };
    ValveInterfaceTable table = ValveInterfaceTableDetector.Detect(strings, memory).Single();
    foreach (string? sdkSource in new[] { source, null })
    {
        ValveInterfaceCatalog catalog = ValveInterfaceCatalog.Parse(interfaces, sdkSource, headers);
        ValveInterfaceBinding binding = ValveInterfaceTableResolver.Resolve(table, catalog)[1];
        Equal(0x7010UL, binding.Row.GlobalAddress);
        Equal("g_pCVar", binding.Definition!.GlobalName);
        Equal("ICvar", binding.Definition.ClassName);
        Equal("public/icvar.h", binding.Definition.DefinitionHeader);

        if (sdkSource != null)
        {
            var twoSlots = new ValveInterfaceTable(0x5000,
                [table.Rows[1], new ValveInterfaceTableRow(0x5040, "VEngineCvar007", 0x7040)]);
            Equal("cvar,g_pCVar", string.Join(',', ValveInterfaceTableResolver.Resolve(twoSlots, catalog)
                .Select(x => x.Definition!.GlobalName)));
        }
    }

    // The mapping also survives a missing version/declaration in interfaces.h.
    string missingDeclaration = interfaces.Replace(
        "#define CVAR_INTERFACE_VERSION \"VEngineCvar007\"", "").Replace(
        "DECLARE_TIER1_INTERFACE( ICVarWrong, wrongCvarName );", "");
    ValveInterfaceCatalog fallback = ValveInterfaceCatalog.Parse(missingDeclaration, null, headers);
    Equal("g_pCVar", ValveInterfaceTableResolver.Resolve(table, fallback)[1].Definition!.GlobalName);
    True(fallback.Versions.Contains("VEngineCvar007"));
    headers["public/icvar.h"] = "class ICvar;";
    ValveInterfaceCatalog forward = ValveInterfaceCatalog.Parse(interfaces, source, headers);
    True(forward.Entries.Single(x => x.Version == "VEngineCvar007").DefinitionHeader == null);
}

static void TestInterfacePointerComments()
{
    const string expected = "Valve interface \"VEngineCvar007\"";
    Equal(expected, ValveInterfaceComments.Merge("", ["VEngineCvar007"]));
    Equal(expected, ValveInterfaceComments.Merge(expected, ["VEngineCvar007", "VEngineCvar007"]));
    Equal("User note\n" + expected, ValveInterfaceComments.Merge("User note", ["VEngineCvar007"]));
    Equal("User note\r\n" + expected,
        ValveInterfaceComments.Merge("User note\r\n", ["VEngineCvar007"]));
    string multiple = ValveInterfaceComments.Merge("User note", ["Version002", "Version001", "Version002"]);
    Equal("User note\nValve interface \"Version001\"\nValve interface \"Version002\"", multiple);
    Equal(multiple, ValveInterfaceComments.Merge(multiple, ["Version002", "Version001"]));
    Equal("Different user text", ValveInterfaceComments.Merge("Different user text", []));
}

static void TestKnownInterfaceImplementations()
{
    Equal(144, System.Runtime.InteropServices.Marshal.SizeOf<IdaUdtMember>());
    Equal((nint)64, System.Runtime.InteropServices.Marshal.OffsetOf<IdaUdtMember>("Type"));
    Equal((nint)132, System.Runtime.InteropServices.Marshal.OffsetOf<IdaUdtMember>("Flags"));
    Equal(3, ValveInterfaceImplementations.Known.Count);
    using TempDirectory temp = new();
    foreach (ValveInterfaceImplementation mapping in ValveInterfaceImplementations.Known)
    {
        var definition = new ValveInterfaceDefinition("VERSION", mapping.Version,
            mapping.InterfaceClass, "g_pTest", mapping.Header);
        Equal(mapping, ValveInterfaceImplementations.Find(definition));
        True(ValveInterfaceImplementations.SlotNameMatches(mapping, "Connect", "Connect"));
        True(ValveInterfaceImplementations.SlotNameMatches(mapping, "dtr_" + mapping.InterfaceClass, "dtr_" + mapping.ClassName));
        True(!ValveInterfaceImplementations.SlotNameMatches(mapping, "Connect", "Disconnect"));
        True(!ValveInterfaceImplementations.SlotNameMatches(mapping, "dtr_" + mapping.InterfaceClass, "dtr_Unrelated"));
        True(ValveInterfaceImplementations.Find(definition with { Version = "DifferentVersion" }) == null);
        True(ValveInterfaceImplementations.Find(definition with { ClassName = "OtherInterface" }) == null);
        True(ValveInterfaceImplementations.Find(definition with { DefinitionHeader = null }) == null);
        string path = System.IO.Path.Combine(temp.Path, "implementation.hpp");
        ValveInterfaceImport.WriteHeader(path, [mapping.Header], [definition], includeImplementations: true);
        string generated = File.ReadAllText(path);
        Contains(generated, $"#include \"{mapping.Header}\"");
        Contains(generated, $"extern {mapping.ClassName} *__s2atelier_implementation_");
        Contains(generated, $"static_assert(sizeof({mapping.ClassName}) >= sizeof({mapping.InterfaceClass}));");
        ValveInterfaceImport.WriteHeader(path, [mapping.Header], [definition]);
        True(!File.ReadAllText(path).Contains("__s2atelier_implementation_", StringComparison.Ordinal));
    }
}

static unsafe void TestGatedImplementationIntegration()
{
    string? binary = Environment.GetEnvironmentVariable("S2ATELIER_TEST_INTERFACES");
    if (string.IsNullOrWhiteSpace(binary)) return;
    string idaPath = Environment.GetEnvironmentVariable("IDA_PATH")
        ?? throw new InvalidOperationException("IDA_PATH is required for the implementation integration test.");
    string sdk = Environment.GetEnvironmentVariable("HL2SDK_PATH") ?? @"D:\Code\hl2sdk";
    True(IdaKernel.TryInitialize(idaPath, IdaSdkVersion.Auto, out string? error), error);
    byte* native = Utf8.Allocate(binary);
    try { Equal(0, IdaNative.open_database(native, 0, null)); }
    finally { Utf8.Free(native); }
    try
    {
        IdaNative.auto_wait();
        IdaNative.build_strlist();
        for (int pass = 0; pass < 2; pass++)
        {
            ValveInterfaceImportResult result = ValveInterfaceImport.Run(binary, sdk);
            True(result.Applicable);
            Equal(0, result.ClangErrors);
            foreach (ValveInterfaceImplementation mapping in ValveInterfaceImplementations.Known)
            {
                True(ValveImplementationTypes.Validate(mapping, out string reason), $"{mapping.ClassName}: {reason}");
                string global = mapping.InterfaceClass switch
                {
                    "ICvar" => "g_pCVar",
                    "ISchemaSystem" => "g_pSchemaSystem",
                    _ => "g_pHostStateMgr",
                };
                byte* name = Utf8.Allocate(global);
                TypeInfo applied = default, implementation = default;
                try
                {
                    ulong address = IdaNative.get_name_ea(ulong.MaxValue, name);
                    True(address != ulong.MaxValue, $"Missing {global}");
                    True(IdaNative.get_tinfo(&applied, address) != 0);
                    True(ValveImplementationTypes.Load(mapping.ClassName, out implementation));
                    ulong pointed = (ulong)IdaNative.get_tinfo_property(applied.Typid, 9);
                    True(IdaNative.compare_tinfo(pointed, implementation.Typid, 0) != 0, $"{global} is not {mapping.ClassName} *");
                    var comment = new QString();
                    try
                    {
                        True(IdaNative.get_cmt(&comment, address, 1) > 0);
                        Equal(1, Count(comment.Read(), $"Valve interface \"{mapping.Version}\""));
                    }
                    finally { comment.Dispose(); }
                }
                finally { Utf8.Free(name); applied.Dispose(); implementation.Dispose(); }
            }
            TypeInfo host = default;
            try
            {
                True(ValveImplementationTypes.Load("CHostStateMgr", out host));
                True(ValveImplementationTypes.ReadMember(host.Typid, 1, out IdaUdtMember secondaryBase));
                try
                {
                    True(secondaryBase.Offset > 0 && (secondaryBase.Flags & 0x20) != 0);
                    True(ValveImplementationTypes.ReadMember(host.Typid, secondaryBase.Offset,
                        out IdaUdtMember secondary, vftable: true));
                    try
                    {
                        Equal(secondaryBase.Offset, secondary.Offset);
                        Equal(64UL, secondary.Size);
                        True((secondary.Flags & 0x100) != 0);
                    }
                    finally { secondary.Dispose(); }
                }
                finally { secondaryBase.Dispose(); }
            }
            finally { host.Dispose(); }
        }
    }
    finally { IdaNative.close_database(0); }
}

static void TestValveInterfaceTableDetection()
{
    string[] versions = ["Engine001", "Cvar001", "Namespaced001", "Forward001"];
    var strings = new Dictionary<ulong, string>
    {
        [0x1000] = versions[0],
        [0x1100] = versions[1],
        [0x1200] = versions[2],
        [0x1300] = versions[3],
    };
    var pointers = new Dictionary<ulong, ulong>
    {
        [0x5000] = 0x1000,
        [0x5008] = 0x7000,
        [0x5010] = 0x1100,
        [0x5018] = 0x7010,
        [0x5020] = 0x1100,
        [0x5028] = 0x7020,
        [0x5030] = 0x1200,
        [0x5038] = 0x7030,
        [0x5040] = 0x1300,
        [0x5048] = 0x7040,
        // An ordinary isolated reference and a four-row-looking sequence with one read-only target.
        [0x6000] = 0x1000,
        [0x6008] = 0x7100,
        [0x6100] = 0x1000,
        [0x6108] = 0x7200,
        [0x6110] = 0x1100,
        [0x6118] = 0x7210,
        [0x6120] = 0x1200,
        [0x6128] = 0x7220,
        [0x6130] = 0x1300,
        [0x6138] = 0x7230,
    };
    var references = new Dictionary<ulong, IReadOnlyList<ulong>>
    {
        [0x1000] = [0x5000, 0x6000, 0x6100],
        [0x1100] = [0x5010, 0x5020, 0x6110],
        [0x1200] = [0x5030, 0x6120],
        [0x1300] = [0x5040, 0x6130],
    };
    var writable = new HashSet<ulong>
        { 0x7000, 0x7010, 0x7020, 0x7030, 0x7040, 0x7100, 0x7200, 0x7210, 0x7230 };
    var memory = new FakeInterfaceMemory(pointers, references, writable,
        [(0x5000, 0x5050), (0x6000, 0x6010), (0x6100, 0x6140)]);

    IReadOnlyList<ValveInterfaceTable> tables = ValveInterfaceTableDetector.Detect(strings, memory);
    Equal(1, tables.Count);
    Equal(5, tables[0].Rows.Count);
    Equal(0x5000UL, tables[0].Address);

    const string interfaces = """
        #define ENGINE_VERSION "Engine001"
        DECLARE_TIER3_INTERFACE( IVEngineServer2, g_pEngineServer );
        #define CVAR_VERSION "Cvar001"
        DECLARE_TIER1_INTERFACE( ICvar, g_pCVar );
        #define NAMESPACED_VERSION "Namespaced001"
        DECLARE_TIER2_INTERFACE( INamespaced, g_pNamespaced );
        #define FORWARD_VERSION "Forward001"
        DECLARE_TIER4_INTERFACE( IForwardOnly, g_pForwardOnly );
        """;
    const string source = """
        ICvar *cvar, *g_pCVar;
        InterfaceGlobals_t g_pInterfaceGlobals[] = {
          { ENGINE_VERSION, &g_pEngineServer }, { CVAR_VERSION, &cvar },
          { CVAR_VERSION, &g_pCVar }, { NAMESPACED_VERSION, &g_pNamespaced },
          { FORWARD_VERSION, &g_pForwardOnly },
        };
        """;
    ValveInterfaceCatalog catalog = ValveInterfaceCatalog.Parse(interfaces, source,
        new Dictionary<string, string> { ["public/interfaces/interfaces.h"] = interfaces });
    IReadOnlyList<ValveInterfaceBinding> bindings = ValveInterfaceTableResolver.Resolve(tables[0], catalog);
    Equal("g_pEngineServer", bindings[0].Definition!.GlobalName);
    Equal("cvar", bindings[1].Definition!.GlobalName);
    Equal("g_pCVar", bindings[2].Definition!.GlobalName);
    foreach (ValveInterfaceTableRow[] cvarRows in new[]
    {
        new[] { tables[0].Rows[1] },
        new[] { tables[0].Rows[1], tables[0].Rows[2], tables[0].Rows[1] },
    })
    {
        var mismatched = new ValveInterfaceTable(0x5000, cvarRows);
        True(ValveInterfaceTableResolver.Resolve(mismatched, catalog).All(x => x.Definition == null));
    }
    True(ValveInterfaceNaming.CanReplace("qword_180001000"));
    True(ValveInterfaceNaming.CanReplace("unk_7FF000"));
    True(!ValveInterfaceNaming.CanReplace("importantGlobal"));
}

static void TestInterfaceCliCombinations()
{
    CliOptions missingSdk = CliOptions.Parse(["server.dll", "--import-interfaces"]);
    True(missingSdk.ImportInterfaces);
    True(!missingSdk.ValidateSchemaOptions(out string? missingError));
    Contains(missingError!, "requires --hl2sdk");

    using TempDirectory sdk = new();
    Directory.CreateDirectory(System.IO.Path.Combine(sdk.Path, "public", "interfaces"));
    File.WriteAllText(System.IO.Path.Combine(sdk.Path, "public", "interfaces", "interfaces.h"), "// fixture");

    CliOptions standalone = CliOptions.Parse(["server.dll", "--hl2sdk", sdk.Path]);
    True(!standalone.ValidateSchemaOptions(out string? standaloneError));
    Contains(standaloneError!, "requires --import-schema or --import-interfaces");

    CliOptions interfacesOnly = CliOptions.Parse(
        ["server.dll", "--import-interfaces", "--hl2sdk", sdk.Path]);
    True(interfacesOnly.ValidateSchemaOptions(out string? validError), validError);
    True(interfacesOnly.ImportSchemaPath == null);
}

static void TestInterfaceWorkerProtocol()
{
    var job = new WireMessage
    {
        Kind = WireKind.Job,
        Path = "server.dll",
        ImportInterfaces = true,
        Hl2SdkPath = @"D:\Code\hl2sdk",
    };
    WireMessage jobRoundTrip = WorkerProtocol.Read(WorkerProtocol.Write(job))!;
    True(jobRoundTrip.ImportInterfaces);
    Equal(@"D:\Code\hl2sdk", jobRoundTrip.Hl2SdkPath);

    var done = new WireMessage
    {
        Kind = WireKind.Done,
        InterfaceImportApplicable = true,
        InterfaceGlobalsFound = 114,
        InterfaceGlobalsRenamed = 113,
        InterfaceTypesApplied = 113,
        InterfaceVTablesImported = 16,
        InterfaceImportSkipped = 4,
        InterfaceClangErrors = 3,
    };
    WireMessage doneRoundTrip = WorkerProtocol.Read(WorkerProtocol.Write(done))!;
    True(doneRoundTrip.InterfaceImportApplicable);
    Equal(114, doneRoundTrip.InterfaceGlobalsFound);
    Equal(113, doneRoundTrip.InterfaceGlobalsRenamed);
    Equal(113, doneRoundTrip.InterfaceTypesApplied);
    Equal(16, doneRoundTrip.InterfaceVTablesImported);
    Equal(4, doneRoundTrip.InterfaceImportSkipped);
    Equal(3, doneRoundTrip.InterfaceClangErrors);
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
            hl2SdkPath: hl2Sdk, schemaProject: "auto", importInterfaces: true);
        True(result.InterfaceImportApplicable);
        True(result.InterfaceGlobalsFound >= 4);
        True(result.InterfaceVTablesImported > 0);
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

static void True(bool value, string? message = null)
{
    if (!value) throw new Exception(message ?? "Expected true.");
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

sealed class TempDirectory : IDisposable
{
    public string Path { get; } = System.IO.Path.Combine(
        System.IO.Path.GetTempPath(), $"s2atelier-test-{Guid.NewGuid():N}");

    public TempDirectory() => Directory.CreateDirectory(Path);

    public void Dispose() => Directory.Delete(Path, recursive: true);
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

sealed class FakeInterfaceMemory(
    IReadOnlyDictionary<ulong, ulong> pointers,
    IReadOnlyDictionary<ulong, IReadOnlyList<ulong>> references,
    IReadOnlySet<ulong> writable,
    IReadOnlyList<(ulong Start, ulong End)> mappedRanges) : IValveInterfaceMemory
{
    public ulong ReadPointer(ulong address) => pointers.TryGetValue(address, out ulong value) ? value : 0;

    public bool IsMapped(ulong address)
        => writable.Contains(address) || mappedRanges.Any(range => address >= range.Start && address < range.End);

    public bool IsWritable(ulong address) => writable.Contains(address);

    public IEnumerable<ulong> DataReferencesTo(ulong address)
        => references.TryGetValue(address, out IReadOnlyList<ulong>? result) ? result : [];
}
