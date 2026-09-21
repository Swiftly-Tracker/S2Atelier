using S2Atelier.Ida;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;
using S2Atelier.Ida.Worker;
using S2Atelier;

if (Environment.GetEnvironmentVariable("S2ATELIER_REBIND_IDB") is { Length: > 0 } repairPath)
    return SdkBindingRepair.Run(repairPath);

var tests = new (string Name, Action Run)[]
{
    ("flat sdk parse and selection", TestSelection),
    ("conflicting duplicate fails", TestConflict),
    ("golden header features", TestHeader),
    ("aligned non-primary base", TestAlignedSecondaryBase),
    ("vtable ABI names", TestVTableNames),
    ("vtable table boundaries", TestVTableBoundaries),
    ("vtable bounded unknown slots", TestVTableUnknownSlots),
    ("vtable naming and partial failures", TestVTableTypes),
    ("gated native schema vtable types", TestNativeSchemaVTables),
    ("SDK vtable matching and ownership", SdkVTableTests.Managed),
    ("vtable drift against the previous build", SdkVTableTests.Drift),
    ("vtable slot owners", SdkVTableTests.SlotOwners),
    ("module health against the baseline", SdkVTableTests.Health),
    ("global vars eras and convar flags", SdkVTableTests.GlobalVars),
    ("gated native SDK vtables", SdkVTableTests.Native),
    ("inheritance ownership", TestOwnership),
    ("function prototype rewrite", TestPrototypeRewrite),
    ("function binding statistics", TestFunctionBindingStatistics),
    ("constructor selection", TestConstructorSelection),
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
        Console.Error.WriteLine($"FAIL {name}: {ex}");
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
    Contains(header, "#if __has_include(\"netmessages.pb.h\")");
    Contains(header, "#include \"netmessages.pb.h\"");
    Contains(header, "#if __has_include(\"gameevents.pb.h\")");
    Contains(header, "#include \"gameevents.pb.h\"");
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

static void TestAlignedSecondaryBase()
{
    // Mirrors CPathQueryComponent: the second base has no schema alignment but holds a
    // 16-byte-aligned member, so MSVC places it at 16 rather than directly after the first base.
    using TempJson fixture = new("""
    {
      "classes": [
        {"name":"Component","name_hash":1,"project":"server","size":8,"alignment":8,"is_struct":false,"has_chainer":false,
         "base_classes_count":0,"base_classes":[],"fields_count":0,"fields":[]},
        {"name":"QueryUtil","name_hash":2,"project":"server","size":32,"alignment":255,"is_struct":false,"has_chainer":false,
         "base_classes_count":0,"base_classes":[],"fields_count":1,
         "fields":[{"name":"m_position","name_hash":3,"kind":"ref","type":"VectorAligned","offset":16,"size":16,"alignment":16,"networked":false}]},
        {"name":"QueryComponent","name_hash":4,"project":"server","size":64,"alignment":255,"is_struct":false,"has_chainer":false,
         "base_classes_count":2,"base_classes":["Component","QueryUtil"],"fields_count":0,"fields":[]}
      ],
      "enums": []
    }
    """);
    SchemaSelection selection = SchemaDatabase.Load(fixture.Path).Select("server", "server.dll")!;
    string header = SchemaHeaderGenerator.Generate(selection, SchemaTargetPlatform.WindowsMsvc).Text;
    Contains(header, "class alignas(16) QueryUtil {");
    string nl = Environment.NewLine;
    Contains(header, $"class alignas(16) QueryComponent : public Component, public QueryUtil {{{nl}public:{nl}" +
        $"    unsigned char __pad_0[16];{nl}}};");
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
    Equal(0x2010UL, slices[0].AddressPoint);
    Equal(0x2028UL, slices[1].AddressPoint);
}

static void TestVTableUnknownSlots()
{
    var memory = new FakeVTableMemory(new Dictionary<ulong, ulong>
    {
        [0x1000] = 0x5000, [0x1008] = 0, [0x1010] = 0x5000, [0x1018] = 0xDEAD,
        [0x1020] = 0x5010,
    }, new HashSet<ulong> { 0x5000, 0x5010 }, new HashSet<ulong>());
    Equal(1, VTableEntryScanner.ScanMsvc(memory, 0x1000, endExclusive: 0x1020).Count);
    var slots = VTableEntryScanner.ScanMsvc(memory, 0x1000, endExclusive: 0x1020, trustedExtent: true);
    Equal("20480,0,20480,57005", string.Join(',', slots));
    Equal(2, VTableEntryScanner.ScanMsvc(memory, 0x1000, 2, 0x1020, true).Count);
    Equal(0, VTableEntryScanner.ScanMsvc(memory, ulong.MaxValue - 3).Count);
    var adjacent = new FakeVTableMemory(new Dictionary<ulong, ulong>
    {
        [0x2000] = 0, [0x2008] = 0x3000, [0x2010] = 0x5000,
        [0x2018] = 0, [0x2020] = 0x3000, [0x2028] = 0x5010,
    }, new HashSet<ulong> { 0x5000, 0x5010 }, new HashSet<ulong> { 0x3000 });
    Equal(1, VTableEntryScanner.ScanItaniumTables(adjacent, 0x2000).Single().Functions.Count);
    Equal(1, VTableEntryScanner.ScanItaniumTables(adjacent, 0x2000, endExclusive: 0x2018).Count);
}

static void TestVTableTypes()
{
    SchemaVTable[] tables =
    [
        new("ns::Derived", 0x1000, 0, "ns::Derived", [], [0x5000, 0x5000]),
        new("ns::Derived", 0x1100, 16, "Base", ["Base"], [0x5000]),
        new("ns::Derived", 0x1200, 16, "Base", ["Base"], [0x5000]),
        new("ns::Derived", 0x1300, null, null, [], [0]),
    ];
    var names = VTableTypeBinder.AssignNames(tables.Concat([tables[0]])).Select(x => x.Name).ToArray();
    Equal("ns::Derived_vtbl,ns::Derived_0010_vtbl,ns::Derived_0010_ea_1200_vtbl,ns::Derived_ea_1300_vtbl", string.Join(',', names));
    Equal("slot_2", VTableTypeBinder.SlotName(null, 2));
    Equal("vfn_Derived_Foo_0", VTableTypeBinder.SlotName("Derived::Foo", 0));
    Equal("ns::Derived::vfn_385", VTableTypeBinder.FunctionName("ns::Derived", 385));
    Equal("vfn_385", VTableTypeBinder.SlotName("ns::Derived::vfn_385", 385));
    Equal("vfn_sub_1000_3", VTableTypeBinder.SlotName("sub_1000", 3));
    var editor = new FakeVTableTypeEditor();
    var result = VTableTypeBinder.Bind(tables, editor);
    Equal(new VTableTypeSummary(3, 2, 1, 1), result);
    Equal(4, editor.Attempts.Count);
    Equal(2, editor.Attempts[0].Functions.Count);
    var reserved = VTableTypeBinder.AssignNames(new[]
    {
        tables[0] with { AddressPoint = 0x900 },
        tables[0] with { ExistingTypeName = "ns::Derived_vtbl" },
    }).Select(x => x.Name).ToArray();
    Equal("ns::Derived_ea_900_vtbl,ns::Derived_vtbl", string.Join(',', reserved));
}

static unsafe void TestNativeSchemaVTables()
{
    string? binary = Environment.GetEnvironmentVariable("S2ATELIER_TEST_VTABLES");
    if (string.IsNullOrWhiteSpace(binary))
    {
        Console.WriteLine("SKIP native schema vtables: set IDA_PATH and S2ATELIER_TEST_VTABLES (PE/ELF input).");
        return;
    }
    string ida = Environment.GetEnvironmentVariable("IDA_PATH") ?? throw new Exception("IDA_PATH required");
    True(IdaKernel.TryInitialize(ida, IdaSdkVersion.Auto, out string? error), error);
    using var temporary = new TempDirectory();
    string copy = System.IO.Path.Combine(temporary.Path, System.IO.Path.GetFileName(binary));
    File.Copy(binary, copy);
    byte* path = Utf8.Allocate(copy);
    try { Equal(0, IdaNative.open_database(path, 0, null)); }
    finally { Utf8.Free(path); }
    try
    {
        IdaNative.auto_wait();
        Equal(56, System.Runtime.InteropServices.Marshal.SizeOf<IdaUdtData>());
        Equal(32, System.Runtime.InteropServices.Marshal.SizeOf<IdaPointerData>());
        Equal((nint)44, System.Runtime.InteropServices.Marshal.OffsetOf<IdaUdtData>("Flags"));
        ulong start = 0x600000000;
        byte* segmentName = Utf8.Allocate("s2_vtable_test"), segmentClass = Utf8.Allocate("DATA");
        try { True(IdaNative.add_segm(0, start, start + 0x1000, segmentName, segmentClass, 0) != 0); }
        finally { Utf8.Free(segmentName); Utf8.Free(segmentClass); }
        byte[] zeros = new byte[0x1000];
        fixed (byte* data = zeros) IdaNative.put_bytes(start, data, (nuint)zeros.Length);
        True(IdaNative.get_func_qty() > 0);
        ulong target = *(ulong*)IdaNative.getn_func(0);
        TypeInfo prototype = default;
        byte* declaration = Utf8.Allocate("int __fastcall __s2_native_method(void *self, int value);");
        try
        {
            True(IdaNative.parse_decl(&prototype, null, IdaNative.get_idati(), declaration, 1 | 8 | 128) != 0);
            True(IdaNative.apply_tinfo(target, &prototype, 1) != 0);
        }
        finally { Utf8.Free(declaration); prototype.Dispose(); }

        // Construct the same explicit-vptr/inheritance representation emitted by the schema header.
        MakeClass("S2TestBase", false);
        MakeClass("S2TestOther", false);
        MakeClass("S2TestDerived", true);
        var classModels = new[]
        {
            new SchemaClass("S2TestBase", 0, "test", 8, 8, false, false, [], []),
            new SchemaClass("S2TestOther", 0, "test", 8, 8, false, false, [], []),
            new SchemaClass("S2TestDerived", 0, "test", 16, 8, false, false, ["S2TestBase", "S2TestOther"], []),
        }.ToDictionary(x => x.Name);
        var selection = new SchemaSelection("test", classModels, new Dictionary<string, SchemaEnum>(),
            new HashSet<string>(), new HashSet<string>(), new HashSet<string>());
        SchemaVTableTypes.PrepareClassVptrs(selection, classModels.Keys.ToHashSet(), message => throw new Exception(message));
        True(ValveImplementationTypes.Load("S2TestBase", out TypeInfo baseBefore));
        True(ValveImplementationTypes.Load("S2TestDerived", out TypeInfo derivedBefore));
        True(IdaNative.detach_tinfo_t(&baseBefore) != 0);
        True(IdaNative.detach_tinfo_t(&derivedBefore) != 0);
        var diagnostics = new List<string>();
        var editor = new SchemaVTableTypes(diagnostics.Add);
        SchemaVTable[] tables =
        [
            new("S2TestDerived", start, 0, "S2TestDerived", [], [target, target, 0]),
            new("S2TestDerived", start + 0x40, 8, "S2TestOther", ["S2TestOther"], [target]),
        ];
        try
        {
            for (int pass = 0; pass < 2; pass++)
            {
                foreach (SchemaVTable table in tables)
                {
                    ulong[] entries = table.Functions.ToArray();
                    fixed (ulong* bytes = entries) IdaNative.put_bytes(table.AddressPoint, bytes, (nuint)entries.Length * 8);
                }
                VTableTypeSummary result = VTableTypeBinder.Bind(tables, editor);
                True(diagnostics.Count == 0, string.Join("\n", diagnostics));
                Equal(new VTableTypeSummary(2, 2, 1, 0), result);
                foreach (var (table, name) in VTableTypeBinder.AssignNames(tables))
                {
                    True(ValveImplementationTypes.Load(name, out TypeInfo vtable));
                    TypeInfo applied = default;
                    try
                    {
                        uint ordinal = SchemaVTableTypes.Ordinal(name);
                        Equal(table.AddressPoint, IdaNative.get_vftable_ea(ordinal));
                        Equal(ordinal, IdaNative.get_vftable_ordinal(table.AddressPoint));
                        True((IdaNative.get_tinfo_property(vtable.Typid, 306) & 0x100) != 0);
                        Equal((nuint)table.Functions.Count, IdaNative.get_tinfo_property(vtable.Typid, 16));
                        True(IdaNative.get_tinfo(&applied, table.AddressPoint) != 0);
                        True(IdaNative.compare_tinfo(applied.Typid, vtable.Typid, 0) != 0);
                        True(SchemaVTableTypes.Metadata(vtable.Typid)?.ClassName == table.ClassName);
                        for (ulong slot = 0; slot < (ulong)table.Functions.Count; slot++)
                        {
                            True(ValveImplementationTypes.ReadMember(vtable.Typid, slot, out IdaUdtMember member));
                            try
                            {
                                Equal(slot * 64, member.Offset);
                                Equal(64UL, member.Size);
                                if (table.Functions[(int)slot] != 0)
                                {
                                    True(IdaNative.get_tinfo_property(member.Type.Typid, 6) != 0);
                                    TypeInfo expected = default;
                                    try
                                    {
                                        True(IdaNative.get_tinfo(&expected, target) != 0);
                                        ulong pointed = (ulong)IdaNative.get_tinfo_property(member.Type.Typid, 9);
                                        True(IdaNative.compare_tinfo(expected.Typid, pointed, 0) != 0);
                                    }
                                    finally { expected.Dispose(); }
                                }
                            }
                            finally { member.Dispose(); }
                        }
                    }
                    finally { vtable.Dispose(); applied.Dispose(); }
                }
                True(ValveImplementationTypes.Load("S2TestDerived", out TypeInfo derived));
                True(ValveImplementationTypes.Load("S2TestBase", out TypeInfo baseAfter));
                try
                {
                    True(SchemaVTableTypes.SameLayout(derivedBefore.Typid, derived.Typid));
                    True(IdaNative.compare_tinfo(baseBefore.Typid, baseAfter.Typid, 0) != 0);
                    foreach (ulong offset in new ulong[] { 0, 64 })
                    {
                        True(ValveImplementationTypes.ReadMember(derived.Typid, offset, out IdaUdtMember vptr, true));
                        try
                        {
                            Equal(offset, vptr.Offset);
                            True((vptr.Flags & 0x100) != 0);
                            ulong pointed = (ulong)IdaNative.get_tinfo_property(vptr.Type.Typid, 9);
                            True((IdaNative.get_tinfo_property(pointed, 306) & 0x100) != 0);
                        }
                        finally { vptr.Dispose(); }
                    }
                }
                finally { baseAfter.Dispose(); derived.Dispose(); }
            }
            // A different class cannot take an occupied table address.
            True(editor.Complete(tables[0] with { ClassName = "S2Other" }, "S2Other_vtbl").Conflict);
            True(editor.Complete(tables[0] with { ClassName = "S2Other", ExistingTypeName = "S2TestDerived_vtbl" },
                "S2TestDerived_vtbl").Conflict);
            var saved = SchemaVTableTypes.Existing(selection).ToArray();
            Equal(2, saved.Length);
            Equal(3, saved.Single(x => x.Address == start).Slots);
            Equal(8UL, SchemaVTableTypes.ResolveLayout(tables[1] with { ObjectOffset = null }).ObjectOffset!.Value);
            True(SchemaVTableTypes.ResolveLayout(tables[1] with { ObjectOffset = 7 }).ObjectOffset == null);

            SetName(start, "??_7S2TestDerived@@6B@");
            SetName(start + 0x40, "??_7S2TestDerived@@6BS2TestOther@@@");
            // MSVC x64 COL with image-relative self/type/hierarchy references.
            WritePointers(start + 0x38, [start + 0x200]);
            uint[] locator = [1, 8, 0, 0x300, 0x320, 0x200];
            fixed (uint* bytes = locator) IdaNative.put_bytes(start + 0x200, bytes, 24);
            SchemaImport.VTableScan msvc = SchemaImport.ScanVTables(selection, SchemaTargetPlatform.WindowsMsvc);
            Equal(2, msvc.Tables.Count);
            Equal(3, msvc.Tables.Single(x => x.AddressPoint == start).Functions.Count);
            Equal(8UL, msvc.Tables.Single(x => x.AddressPoint == start + 0x40).ObjectOffset!.Value);

            WritePointers(start + 0x100, [0, start + 0x300, target, unchecked((ulong)-8L), start + 0x300, target]);
            SetName(start + 0x100, "_ZTV13S2TestDerived");
            SchemaImport.VTableScan itanium = SchemaImport.ScanVTables(selection, SchemaTargetPlatform.LinuxItanium);
            True(itanium.Tables.Any(x => x.AddressPoint == start + 0x110 && x.ObjectOffset == 0));
            True(itanium.Tables.Any(x => x.AddressPoint == start + 0x128 && x.ObjectOffset == 8));
            True(!itanium.Tables.Any(x => x.AddressPoint == start + 0x100));

            // A root's own vptr is physically replaced, while inherited ones above were synthesized.
            SchemaVTable root = new("S2TestBase", start + 0x80, 0, "S2TestBase", [], [target]);
            WritePointers(root.AddressPoint, [target]);
            diagnostics.Clear();
            True(editor.Complete(root, "S2TestBase_vtbl").Bound, string.Join("\n", diagnostics));
            True(diagnostics.Count == 0, string.Join("\n", diagnostics));
            True(ValveImplementationTypes.Load("S2TestBase_vtbl", out TypeInfo rootVtable));
            try
            {
                // Simulate an older unbound VFT type without importer metadata; adopt it by canonical name.
                byte* emptyComment = Utf8.Allocate("");
                try { Equal((nuint)0, IdaNative.set_tinfo_property4(&rootVtable, 5, (nuint)emptyComment, 0, 0, 0)); }
                finally { Utf8.Free(emptyComment); }
                IdaNative.set_vftable_ea(SchemaVTableTypes.Ordinal("S2TestBase_vtbl"), ulong.MaxValue);
            }
            finally { rootVtable.Dispose(); }
            True(editor.Complete(root, "S2TestBase_vtbl").Bound, string.Join("\n", diagnostics));

            // Recreate imported classes as the header replacement pass does, then rebind all tables.
            SchemaImport.DeleteReplacedTypes(classModels.Keys);
            MakeClass("S2TestBase", false);
            MakeClass("S2TestOther", false);
            MakeClass("S2TestDerived", true);
            SchemaVTableTypes.PrepareClassVptrs(selection, classModels.Keys.ToHashSet(), message => throw new Exception(message));
            diagnostics.Clear();
            Equal(new VTableTypeSummary(3, 3, 1, 0), VTableTypeBinder.Bind(tables.Concat([root]), editor));
            True(diagnostics.Count == 0, string.Join("\n", diagnostics));
        }
        finally { baseBefore.Dispose(); derivedBefore.Dispose(); }
    }
    finally { IdaNative.close_database(0); }

    static void SetName(ulong address, string name)
    {
        byte* native = Utf8.Allocate(name);
        try { True(IdaNative.set_name(address, native, 0x2100) != 0); }
        finally { Utf8.Free(native); }
    }

    static void WritePointers(ulong address, ulong[] values)
    {
        fixed (ulong* bytes = values) IdaNative.put_bytes(address, bytes, (nuint)values.Length * 8);
    }

    static void MakeClass(string name, bool derived)
    {
        IdaUdtData data = IdaUdtData.Allocate(derived ? 2 : 1);
        TypeInfo type = default;
        try
        {
            data.TotalSize = data.UnpaddedSize = derived ? 16U : 8U;
            data.Alignment = 8;
            data.Flags = 0x80 | 0x400;
            for (int i = 0; i < (int)data.Count; i++)
            {
                ref IdaUdtMember member = ref data.Members[i];
                member.Offset = (ulong)i * 64;
                member.Size = 64;
                member.Name = SchemaVTableTypes.String(derived ? "base" + i : "__vftable");
                if (derived)
                {
                    True(ValveImplementationTypes.Load(i == 0 ? "S2TestBase" : "S2TestOther", out member.Type));
                    member.Flags = 0x20;
                }
                else
                {
                    TypeInfo empty = new() { Typid = 1 };
                    True(SchemaVTableTypes.Pointer(ref empty, out member.Type));
                }
            }
            True(IdaNative.create_tinfo(&type, 0x0D, 0x0D, &data) != 0);
            True(SchemaVTableTypes.Save(ref type, name));
        }
        finally { type.Dispose(); data.Dispose(); }
    }
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

static void TestConstructorSelection()
{
    VptrWriter[] writers =
    [
        new(0x100, ["Base"], false, null, false),                // root constructor: no base call to confirm it
        new(0x200, ["Derived"], true, 0x100, false),             // calls the base constructor, then stores its vtable
        new(0x300, ["Derived"], false, null, true),              // deleting destructor reached from a vtable
        new(0x400, ["Leaf", "Derived", "Base"], true, 0x900, false), // inlined destructor chain after a member call
        new(0x500, ["Other"], true, 0x100, false),               // overloads: two constructors for one class
        new(0x600, ["Other"], true, 0x200, false),
        new(0x700, ["Thunked"], true, 0x200, true),              // referenced by an unnamed table
        new(0x800, ["Base"], true, 0x100, false),                // base call writes the same class: not a constructor
    ];
    var selected = ConstructorAnalysis.SelectConstructors(writers);
    Equal("Derived:512", string.Join(',', selected.OrderBy(x => x.Key).Select(x => $"{x.Key}:{x.Value}")));
    Equal("CCSPlayerPawn::CCSPlayerPawn", ConstructorAnalysis.ConstructorName("CCSPlayerPawn"));
    Equal("ns::Foo<a::B>::Foo", ConstructorAnalysis.ConstructorName("ns::Foo<a::B>"));
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
    SchemaVTable[] tables =
    [
        new("Base", 0x1000, 0, "Base", [], [10, 30, 40]),
        new("Derived", 0x2000, 0, "Derived", [], [10, 30, 40]),
        // The same function in another class's secondary table sits at a different offset.
        new("Other", 0x3000, 8, "Base", ["Base"], [30, 10]),
    ];
    var editor = new FakeFunctionTypeEditor(new HashSet<ulong> { 30 });
    VTableBindingSummary result = VTableFunctionBinder.Bind(owners,
        new HashSet<ulong> { 40 }, new HashSet<ulong>(), selection.Classes, editor, tables: tables);
    Equal(1, result.Bound);
    Equal(2, result.Skipped);
    Equal(1, result.Conflicts);
    Equal(0, result.Named);
    Equal("10:Base,30:Base", string.Join(',', editor.Attempts.Select(x => $"{x.Address}:{x.Owner}")));
    Equal("", string.Join(',', editor.Names.Select(x => $"{x.Address}:{x.Name}")));

    SchemaVTable[] consistent =
    [
        new("Base", 0x1000, 0, "Base", [], [10, 30, 40]),
        new("Derived", 0x2000, 0, "Derived", [], [10, 30, 40]),
    ];
    editor = new FakeFunctionTypeEditor(new HashSet<ulong> { 30 });
    result = VTableFunctionBinder.Bind(owners,
        new HashSet<ulong> { 40 }, new HashSet<ulong>(), selection.Classes, editor, tables: consistent);
    Equal(2, result.Named);
    Equal("10:Base::vfn_0,30:Base::vfn_1", string.Join(',', editor.Names.Select(x => $"{x.Address}:{x.Name}")));

    // A thunk referenced only by one class's secondary table is scoped by that class.
    var thunkOwners = new Dictionary<ulong, HashSet<string>> { [50] = new(StringComparer.Ordinal) { "Base" } };
    editor = new FakeFunctionTypeEditor(new HashSet<ulong>());
    result = VTableFunctionBinder.Bind(thunkOwners, new HashSet<ulong>(), new HashSet<ulong>(), selection.Classes,
        editor, tables: [new SchemaVTable("Derived", 0x4000, 16, "Base", ["Base"], [0, 50])]);
    Equal("50:Derived::Base::vfn_1", string.Join(',', editor.Names.Select(x => $"{x.Address}:{x.Name}")));
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
    Contains(standaloneError!, "requires --import-schema, --import-interfaces, --name-entity-classes or --type-globals");

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
        SchemaVTableTypesCompleted = 7,
        SchemaVTableAddressesBound = 6,
        SchemaVTableUnknownSlots = 5,
        SchemaVTableConflicts = 4,
    };
    WireMessage doneRoundTrip = WorkerProtocol.Read(WorkerProtocol.Write(done))!;
    True(doneRoundTrip.InterfaceImportApplicable);
    Equal(114, doneRoundTrip.InterfaceGlobalsFound);
    Equal(113, doneRoundTrip.InterfaceGlobalsRenamed);
    Equal(113, doneRoundTrip.InterfaceTypesApplied);
    Equal(16, doneRoundTrip.InterfaceVTablesImported);
    Equal(4, doneRoundTrip.InterfaceImportSkipped);
    Equal(3, doneRoundTrip.InterfaceClangErrors);
    Equal(7, doneRoundTrip.SchemaVTableTypesCompleted);
    Equal(6, doneRoundTrip.SchemaVTableAddressesBound);
    Equal(5, doneRoundTrip.SchemaVTableUnknownSlots);
    Equal(4, doneRoundTrip.SchemaVTableConflicts);
    Equal(0, WorkerProtocol.Read("{\"Kind\":3}")!.SchemaVTableAddressesBound);
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
    public bool CanReadPointer(ulong address) => pointers.ContainsKey(address);
}

sealed class FakeFunctionTypeEditor(IReadOnlySet<ulong> failures) : IVirtualFunctionTypeEditor
{
    public List<(ulong Address, string Owner)> Attempts { get; } = [];

    public List<(ulong Address, string Name)> Names { get; } = [];

    public bool TryBindThis(ulong address, string owner)
    {
        Attempts.Add((address, owner));
        return !failures.Contains(address);
    }

    public bool TryName(ulong address, string name)
    {
        Names.Add((address, name));
        return true;
    }
}

sealed class FakeVTableTypeEditor : IVTableTypeEditor
{
    public List<SchemaVTable> Attempts { get; } = [];
    public VTableTypeEditResult Complete(SchemaVTable table, string typeName)
    {
        Attempts.Add(table);
        return table.AddressPoint switch
        {
            0x1100 => new(false, false, 0, true),
            0x1200 => new(true, false),
            0x1300 => new(true, true, 1),
            _ => new(true, true),
        };
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
