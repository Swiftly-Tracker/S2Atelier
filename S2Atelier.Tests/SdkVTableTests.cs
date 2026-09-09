using S2Atelier.Ida;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

internal static class SdkVTableTests
{
    private static void Check(bool condition, string message)
    { if (!condition) throw new Exception(message); }

    internal static void Managed()
    {
        var classes = new[] { Class("Base"), Class("Other"), Class("Derived", "Base", "Other") }.ToDictionary(x => x.Name);
        var table = new SchemaVTable("Derived", 0x1000, 0, "Derived", [], [1, 2, 3, 4]);
        SdkPortableType prototype = new([0], [0], [0]);
        SdkVTableDefinition Def(string name, params string[] methods) => new(name, 0, name + ".h",
            methods.Select(m => new SdkSlotDefinition(m, prototype)).ToArray());
        var definitions = new Dictionary<(string, ulong), SdkVTableDefinition>
        { [("Base", 0)] = Def("Base", "Known", "unk001"), [("Other", 0)] = Def("Other", "Secondary") };
        var messages = new List<string>();
        var partial = SdkVTableMatching.Match(table, classes, definitions, messages.Add);
        Check(partial.Count == 2 && partial[1].Slot.Name == "unk001", "base partial/unk mapping");
        definitions[("Derived", 0)] = Def("Derived", "Override", "unk999", "More", "Last");
        var full = SdkVTableMatching.Match(table, classes, definitions, messages.Add);
        Check(full.Count == 4 && full[0].Slot.Name == "Override" && full[1].Slot.Name == "unk999", "derived priority");
        var secondary = SdkVTableMatching.Match(table with { ObjectOffset = 8, ThisType = "Other" }, classes, definitions, messages.Add);
        Check(secondary.Count == 1 && secondary[0].Slot.Name == "Secondary", "secondary uses resolved base");
        Check(SdkVTableMatching.Match(table with { ObjectOffset = null }, classes, definitions, messages.Add).Count == 0,
            "unresolved layout must not guess");
        var shortTable = SdkVTableMatching.Match(table with { Functions = [1, 2] }, classes, definitions, messages.Add);
        Check(shortTable.Count == 2 && shortTable[0].Definition.ClassName == "Base" && messages.Count == 1, "length conflict fallback");
        var incompatible = SdkVTableMatching.Match(table, classes, definitions, messages.Add, d => d.ClassName != "Derived");
        Check(incompatible.Count == 2 && incompatible[0].Definition.ClassName == "Base", "layout conflict falls back to base");
        Check(SdkVTableMatching.Match(table, classes, new Dictionary<(string, ulong), SdkVTableDefinition>(), messages.Add).Count == 0,
            "missing/failed definitions fallback");
        var used = new HashSet<string>();
        Check(SdkVTableMatching.UniqueName("Known", 0, used) == "Known", "SDK spelling preserved");
        Check(SdkVTableMatching.UniqueName("Known", 2, used) == "Known_slot_2", "overload disambiguation");
        var ownership = new SdkFunctionOwnership("Base::Known", "fingerprint", "base.h");
        string comment = SdkFunctionBinding.MergeOwnership("user comment", ownership);
        Check(SdkFunctionBinding.ReadOwnership(comment) == ownership, "ownership roundtrip");
        Check(SdkFunctionBinding.MergeOwnership(comment, ownership) == comment, "ownership idempotent");
        Check(SdkFunctionBinding.ReadOwnership(SdkFunctionBinding.Marker + "invalid") == null, "bad metadata");
            Check(!SdkFunctionBinding.CanUpdateName("UserName", ownership) &&
            SdkFunctionBinding.CanUpdateName("Base::Known", ownership) &&
            SdkFunctionBinding.CanUpdateName("sub_1000", null), "user naming protection");
        Check(!SdkFunctionBinding.CanUpdateName("sub_UserEdited", ownership), "edited auto-looking name protected");
        var headers = new Dictionary<string, string> { ["a.h"] = "// class Wrong {};\nclass Right { virtual void F(); };" };
        Check(ValveInterfaceCatalog.FindDefinitionHeader("Right", headers) == "a.h" &&
            ValveInterfaceCatalog.FindDefinitionHeader("Wrong", headers) == null, "discovery ignores comments");
        headers["b.h"] = "class Right { virtual void G(); };";
        Check(ValveInterfaceCatalog.FindDefinitionHeader("Right", headers) == null, "ambiguous header skipped");
    }

    private static SchemaClass Class(string name, params string[] bases) =>
        new(name, 0, "test", 32, 8, false, false, bases, []);

    internal static unsafe void Native()
    {
        string? binary = Environment.GetEnvironmentVariable("S2ATELIER_TEST_VTABLES");
        string? ida = Environment.GetEnvironmentVariable("IDA_PATH");
        if (string.IsNullOrEmpty(binary) || string.IsNullOrEmpty(ida))
        { Console.WriteLine("SKIP native SDK vtables: set IDA_PATH and S2ATELIER_TEST_VTABLES."); return; }
        Check(IdaKernel.TryInitialize(ida, IdaSdkVersion.Auto, out var error), error ?? "IDA init");
        string directory = Path.Combine(Path.GetTempPath(), "s2-sdk-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(directory, "public", "tier0"));
        string copy = Path.Combine(directory, Path.GetFileName(binary));
        File.Copy(binary, copy);
        byte* nativePath = Utf8.Allocate(copy);
        try { Check(IdaNative.open_database(nativePath, 0, null) == 0, "open fixture"); }
        finally { Utf8.Free(nativePath); }
        try
        {
            IdaNative.auto_wait();
            var platform = SchemaImport.DetectTargetPlatform();
            File.WriteAllText(Path.Combine(directory, "public", "tier0", "platform.h"), "#pragma once\n");
            File.WriteAllText(Path.Combine(directory, "public", "test.h"), """
                struct SdkPayload { int value; };
                class SdkBase { public:
                    virtual SdkPayload *Known(SdkPayload *value) = 0;
                    virtual void unk001() = 0;
                    virtual ~SdkBase() = default;
                };
                class SdkOther { public: virtual bool OtherCall(double amount) const = 0; };
                class SdkDerived : public SdkBase, public SdkOther { public: virtual int More(int value) = 0; };
                """);
            string localHeader = Path.Combine(directory, "local.hpp");
            File.WriteAllText(localHeader, """
                struct SdkBase { void *__vftable; unsigned char schemaFields[24]; };
                struct SdkOther { void *__vftable; };
                struct SdkDerived : SdkBase, SdkOther { unsigned char derivedFields[16]; };
                struct SdkMissing : SdkBase { };
                """);
            SchemaImport.ConfigureClang(directory, platform, true);
            Check(SchemaImport.ParseHeader(localHeader, false, true) == 0, "import fixture schema layouts");
            var classes = new[] { Class("SdkBase"), Class("SdkOther"), Class("SdkDerived", "SdkBase", "SdkOther"),
                Class("SdkMissing", "SdkBase") }.ToDictionary(x => x.Name);
            var selection = new SchemaSelection("test", classes, new Dictionary<string, SchemaEnum>(),
                new HashSet<string>(), new HashSet<string>(), new HashSet<string>());
            Check(IdaNative.get_func_qty() > 0, "fixture must have a function");
            ulong target = *(ulong*)IdaNative.getn_func(0);
            var tables = new[]
            {
                new SchemaVTable("SdkBase", 0x610000000, 0, "SdkBase", [], [target, 0, 0, 0, 0, 0]),
                new SchemaVTable("SdkMissing", 0x610000100, 0, "SdkMissing", [], [0, 0, 0, 0, 0, 0]),
                new SchemaVTable("SdkDerived", 0x610000200, 32, "SdkOther", ["SdkOther"], [0]),
            };
            var diagnostics = new List<string>();
            using var sdk = Hl2SdkVTables.Load(directory, platform, selection, tables, diagnostics.Add);
            Check(sdk.Definitions.ContainsKey(("SdkBase", 0)), "SDK parse failed: " + string.Join('\n', diagnostics));
            var baseDefinition = sdk.Definitions[("SdkBase", 0)];
            Check(baseDefinition.Slots.Count == (platform == SchemaTargetPlatform.WindowsMsvc ? 3 : 4), "destructor ABI slot count");
            var slots = sdk.Resolve(tables, selection);
            Check(slots.TryGetValue((tables[0].AddressPoint, 0), out var known) && known.Name == "Known",
                "SDK Known slot: " + string.Join('\n', diagnostics));
            Check(slots[(tables[0].AddressPoint, 1)].Name == "unk001", "SDK unk slot");
            Check(slots[(tables[1].AddressPoint, 0)].SourceClass == "SdkBase", "inherited SDK slot");
            Check(slots[(tables[2].AddressPoint, 0)].Name == "OtherCall", "secondary SDK slot");
            Check(slots[(tables[2].AddressPoint, 0)].TryGetFunction(out var constFunction), "const method prototype");
            TypeInfo constThis = new() { Typid = (ulong)IdaNative.get_tinfo_property(constFunction.Typid, 25) };
            TypeInfo constObject = new() { Typid = (ulong)IdaNative.get_tinfo_property(constThis.Typid, 9) };
            try { Check((IdaNative.get_tinfo_property(constObject.Typid, 1) & 0x40) != 0, "const this qualifier preserved"); }
            finally { constObject.Dispose(); constThis.Dispose(); constFunction.Dispose(); }
            var derivedTable = new SchemaVTable("SdkDerived", 0x610000300, 0, "SdkDerived", [], new ulong[8]);
            var derivedSlots = sdk.Resolve([derivedTable], selection);
            Check(derivedSlots.Values.Any(x => x.Name == "More" && x.SourceClass == "SdkDerived"), "full derived definition");
            Check(ValveImplementationTypes.Load("SdkPayload", out var payload), "prototype dependency imported");
            payload.Dispose();
            Check(ValveImplementationTypes.Load("SdkBase", out var schema), "schema preserved");
            try { Check(IdaNative.get_tinfo_size(null, schema.Typid, 0) == 32, "SDK replaced schema layout"); }
            finally { schema.Dispose(); }
            sdk.Dispose(); // Portable output must remain valid after freeing every source TIL.
            Check(known!.TryGetFunction(out var function), "portable prototype after TIL disposal");
            function.Dispose();

            byte* segmentName = Utf8.Allocate("sdk_vtables"), segmentClass = Utf8.Allocate("DATA");
            try { Check(IdaNative.add_segm(0, tables[0].AddressPoint, tables[0].AddressPoint + 0x1000,
                segmentName, segmentClass, 0) != 0, "create SDK table segment"); }
            finally { Utf8.Free(segmentName); Utf8.Free(segmentClass); }
            foreach (var table in tables)
            {
                ulong[] entries = table.Functions.ToArray();
                fixed (ulong* data = entries) IdaNative.put_bytes(table.AddressPoint, data, (nuint)entries.Length * 8);
            }
            for (int pass = 0; pass < 2; pass++)
            {
                SchemaVTableTypes.PrepareClassVptrs(selection, classes.Keys.ToHashSet(), diagnostics.Add);
                var completed = VTableTypeBinder.Bind(tables, new SchemaVTableTypes(diagnostics.Add, slots));
                Check(completed.Completed == 3 && completed.Bound == 3, "SDK VFT completion/address binding: " + string.Join('\n', diagnostics));
                Check(completed.UnknownSlots == tables.Sum(t => t.Functions.Count) - slots.Count, "only uncovered slots are unknown");
                Check(ValveImplementationTypes.Load("SdkBase_vtbl", out var vtable), "completed SDK table");
                try
                {
                    Check(ValveImplementationTypes.ReadMember(vtable.Typid, 0, out var member), "read SDK member");
                    try
                    {
                        Check(member.Name.Read() == "Known" && member.Comment.Read().Contains("HL2SDK"), "SDK name/source on vtable");
                        Check(known.TryGetFunction(out var expected), "SDK expected prototype");
                        TypeInfo pointed = new() { Typid = (ulong)IdaNative.get_tinfo_property(member.Type.Typid, 9) };
                        try { Check(IdaNative.compare_tinfo(pointed.Typid, expected.Typid, 0) != 0, "exact SDK VFT prototype"); }
                        finally { pointed.Dispose(); expected.Dispose(); }
                    }
                    finally { member.Dispose(); }
                }
                finally { vtable.Dispose(); }
                if (pass == 0)
                {
                    SchemaImport.DeleteReplacedTypes(classes.Keys);
                    Check(SchemaImport.ParseHeader(localHeader, false, true) == 0, "reimport schema layouts");
                }
            }

            IdaNative.set_aflags(target, IdaNative.get_aflags(target) & ~0x02000000u);
            SetName(target, "sub_SdkFixture");
            var result = SdkFunctionBinding.Bind(tables, slots, new HashSet<ulong>(), new HashSet<ulong>(), diagnostics.Add);
            Check(result.Bound == 1, "function not bound: " + string.Join('\n', diagnostics));
            string? fingerprint = SdkFunctionBinding.FingerprintAt(target);
            Check(fingerprint != null && SchemaVTableTypes.NameAt(target).Contains("Known"), "function name/prototype");
            result = SdkFunctionBinding.Bind(tables, slots, new HashSet<ulong>(), new HashSet<ulong>(), diagnostics.Add);
            Check(result.Bound == 1 && SdkFunctionBinding.FingerprintAt(target) == fingerprint, "repeat import");
            var updatedSlots = slots.ToDictionary(x => x.Key, x => x.Value);
            byte* updateDeclaration = Utf8.Allocate("double UpdatedKnown(SdkBase *self, double amount);");
            TypeInfo update = default;
            try
            {
                Check(IdaNative.parse_decl(&update, null, IdaNative.get_idati(), updateDeclaration, 1 | 8 | 128) != 0, "SDK update parse");
                var updateType = SdkPortableType.Capture(ref update, IdaNative.get_idati());
                Check(updateType != null, "SDK update snapshot");
                updatedSlots[(tables[0].AddressPoint, 0)] = known with { Name = "UpdatedKnown", Prototype = updateType! };
            }
            finally { update.Dispose(); Utf8.Free(updateDeclaration); }
            result = SdkFunctionBinding.Bind(tables, updatedSlots, new HashSet<ulong>(), new HashSet<ulong>(), diagnostics.Add);
            Check(result.Bound == 1 && SchemaVTableTypes.NameAt(target).Contains("UpdatedKnown") &&
                SdkFunctionBinding.FingerprintAt(target) != fingerprint, "SDK update applies to owned name/prototype");
            SetName(target, "UserSdkName");
            byte* edited = Utf8.Allocate("int UserSdkName(int edited);");
            TypeInfo user = default;
            try
            {
                Check(IdaNative.parse_decl(&user, null, IdaNative.get_idati(), edited, 1 | 8 | 128) != 0, "user prototype parse");
                Check(IdaNative.apply_tinfo(target, &user, 1) != 0, "user prototype apply");
            }
            finally { user.Dispose(); Utf8.Free(edited); }
            string? editedFingerprint = SdkFunctionBinding.FingerprintAt(target);
            SdkFunctionBinding.Bind(tables, slots, new HashSet<ulong>(), new HashSet<ulong>(), diagnostics.Add);
            Check(SchemaVTableTypes.NameAt(target) == "UserSdkName" && SdkFunctionBinding.FingerprintAt(target) == editedFingerprint,
                "user changes overwritten");
            var conflictTables = tables.Select((t, i) => i == 1 ? t with { Functions = [target, 0, 0, 0, 0, 0] } : t).ToArray();
            result = SdkFunctionBinding.Bind(conflictTables, slots, new HashSet<ulong>(), new HashSet<ulong>(), diagnostics.Add);
            Check(result.Conflicts == 1, "shared this conflict not rejected");
            result = SdkFunctionBinding.Bind([tables[0] with { Functions = [target, target, 0, 0, 0, 0] }], slots,
                new HashSet<ulong>(), new HashSet<ulong>(), diagnostics.Add);
            Check(result.Conflicts == 1, "shared address with different SDK methods not rejected");
            result = SdkFunctionBinding.Bind(tables, slots, new HashSet<ulong> { target }, new HashSet<ulong>(), diagnostics.Add);
            Check(result.Bound == 0, "purecall must not be typed");

            File.WriteAllText(Path.Combine(directory, "public", "broken.h"),
                "class SdkBroken { public: virtual MissingReturn Broken() = 0; };\n");
            using (var isolated = Hl2SdkVTables.Load(directory, platform, selection,
                [tables[0], new("SdkBroken", 0x610000400, 0, "SdkBroken", [], [0])], diagnostics.Add))
            {
                Check(isolated.Definitions.ContainsKey(("SdkBase", 0)) && !isolated.Definitions.ContainsKey(("SdkBroken", 0)) &&
                    diagnostics.Any(x => x.Contains("broken.h") && x.Contains("parse error")), "header parse failure isolation");
            }

            string realSdk = Environment.GetEnvironmentVariable("HL2SDK_PATH") ?? @"D:\Code\hl2sdk";
            if (Directory.Exists(realSdk) && (platform == SchemaTargetPlatform.WindowsMsvc || !OperatingSystem.IsWindows()))
            {
                var entityClasses = new[] { Class("CEntityInstance") }.ToDictionary(x => x.Name);
                var entitySelection = selection with { Classes = entityClasses };
                var entityTable = new SchemaVTable("CEntityInstance", 0x620000000, 0, "CEntityInstance", [], new ulong[128]);
                using var real = Hl2SdkVTables.Load(realSdk, platform, entitySelection, [entityTable], diagnostics.Add);
                Check(real.Definitions.TryGetValue(("CEntityInstance", 0), out var entity), "real SDK parse: " + string.Join('\n', diagnostics));
                Check(entity!.Slots.Any(x => x.Name == "GetNetworkSerializerInfo") && entity.Slots.Any(x => x.Name == "unk001"),
                    "real SDK names");
                byte* entityDecl = Utf8.Allocate("struct CEntityInstance { void *__vftable; unsigned char schemaFields[112]; };");
                try { Check(IdaNative.parse_decls(IdaNative.get_idati(), entityDecl, null, 0x400) == 0, "entity schema stub"); }
                finally { Utf8.Free(entityDecl); }
                var entitySlots = real.Resolve([entityTable], entitySelection);
                Console.WriteLine($"Real HL2SDK CEntityInstance: {entitySlots.Count}/{entity.Slots.Count} SDK prototypes relocated.");
                foreach (string message in diagnostics.Where(x => x.Contains("dependency ") || x.Contains("CEntityInstance slot")))
                    Console.WriteLine(message);
                Check(entitySlots.Values.Any(x => x.Name == "GetNetworkSerializerInfo") &&
                    entitySlots.Values.Any(x => x.Name == "unk001"), "real SDK prototypes: " + string.Join('\n', diagnostics));
                Check(ValveImplementationTypes.Load("CEntityInstance", out var entitySchema), "entity schema still available");
                try { Check(IdaNative.get_tinfo_size(null, entitySchema.Typid, 0) == 120, "entity schema layout preserved"); }
                finally { entitySchema.Dispose(); }
            }
            else Console.WriteLine("SKIP real SDK vtable smoke: HL2SDK_PATH unavailable or cross-target Linux system headers unavailable on Windows.");
        }
        finally
        {
            IdaNative.close_database(0);
            Directory.Delete(directory, true);
        }
        static void SetName(ulong address, string value)
        {
            byte* text = Utf8.Allocate(value);
            try { Check(IdaNative.set_name(address, text, 1 | 0x40) != 0, "set test name"); }
            finally { Utf8.Free(text); }
        }
    }
}
