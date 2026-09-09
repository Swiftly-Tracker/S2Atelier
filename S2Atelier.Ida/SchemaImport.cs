using System.Runtime.InteropServices;
using System.Text;
using S2Atelier.Ida.Generated;
using S2Atelier.Ida.Schema;

namespace S2Atelier.Ida;

public sealed record SchemaImportResult(
    bool Applicable,
    string? Project = null,
    int TypesImported = 0,
    int VTablesMatched = 0,
    int FunctionsBound = 0,
    int FunctionsSkipped = 0,
    int FunctionConflicts = 0,
    int ClangErrors = 0, int VTableTypesCompleted = 0, int VTableAddressesBound = 0,
    int VTableUnknownSlots = 0, int VTableConflicts = 0);

public sealed class SchemaImportException(string message, int clangErrors = 0) : Exception(message)
{
    public int ClangErrors { get; } = clangErrors;
}

public static unsafe class SchemaImport
{
    private const int HtiTest = 0x00000020;
    private const int HtiFile = 0x00000040;
    private const int HtiNoWarnings = 0x00000100;
    private const int HtiIgnoreErrors = 0x00000200;
    private const int HtiDeclarations = 0x00000400;
    private const int NtfType = 0x00000001;
    private const int NtfNoBase = 0x00000002;
    private const int PtSilent = 0x00000001;
    private const int PtVariable = 0x00000008;
    private const int PtHigh = 0x00000080;
    private const uint TinfoDefinite = 0x0001;
    // tinfo_t::gta_prop_t and sta_prop_t are stable across the supported IDA SDK 9.2/9.3.
    private const int GtaFunctionArgumentCount = 23;
    private const int StaFunctionArgumentName = 30;
    private const int StaFunctionArgumentType = 31;
    private static readonly IdaVTableMemory VTableMemory = new();

    public static SchemaImportResult Run(
        string binaryPath,
        string sdkJsonPath,
        string hl2SdkPath,
        string requestedProject)
    {
        SchemaTargetPlatform platform = DetectTargetPlatform();
        SchemaDatabase database = SchemaDatabase.Load(sdkJsonPath);
        SchemaSelection? selection = database.Select(requestedProject, binaryPath);
        if (selection == null)
        {
            Console.Error.WriteLine(
                $"[schema] {Path.GetFileName(binaryPath)}: no sdk.json project named '{SchemaDatabase.InferProject(binaryPath)}'; skipped.");
            return new SchemaImportResult(false);
        }

        VTableScan scan = ScanVTables(selection, platform);
        SchemaHeaderResult header = SchemaHeaderGenerator.Generate(selection, platform, scan.PolymorphicClasses);
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"s2atelier-schema-{Guid.NewGuid():N}");
        string tempHeader = Path.Combine(tempDirectory, "schema.hpp");
        bool keepHeader = false;
        try
        {
            Directory.CreateDirectory(tempDirectory);
            File.WriteAllText(tempHeader, header.Text, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            // Current HL2SDK snapshots include this generated protobuf header unconditionally,
            // although schema layout only needs the replacement enum emitted by our preamble.
            File.WriteAllText(Path.Combine(tempDirectory, "network_connection.pb.h"),
                "#pragma once\n", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            ConfigureClang(hl2SdkPath, platform, skipLayoutAssertions: false);
            int preflightErrors = ParseHeader(tempHeader, testOnly: true, printDiagnostics: true);
            if (preflightErrors != 0)
            {
                keepHeader = true;
                Console.Error.WriteLine(
                    $"[schema] {Path.GetFileName(binaryPath)}: IDAClang found {preflightErrors} preflight error(s); " +
                    "continuing with a best-effort import; diagnostics are printed separately.");
                Console.Error.WriteLine($"[schema] generated header retained for diagnosis: {tempHeader}");
            }

            // A clean translation unit can safely replace all prior schema definitions. If the
            // preflight failed, preserve existing types and let HTI_NER add every valid declaration.
            if (preflightErrors == 0)
            {
                DeleteReplacedTypes(header.ImportedTypeNames);
            }

            // Layout assertions belong to preflight. Suppress them during the write pass so an
            // SDK-version mismatch cannot prevent otherwise valid declarations from reaching IDA.
            ConfigureClang(hl2SdkPath, platform, skipLayoutAssertions: true);
            int importErrors = ParseHeader(tempHeader, testOnly: false, printDiagnostics: preflightErrors == 0);
            int clangErrors = Math.Max(preflightErrors, importErrors);
            keepHeader |= importErrors != 0;
            if (importErrors != 0 && preflightErrors == 0)
            {
                Console.Error.WriteLine(
                    $"[schema] {Path.GetFileName(binaryPath)}: IDAClang ignored {importErrors} import error(s); " +
                    $"generated header retained for diagnosis: {tempHeader}");
            }
            int importedTypes = CountAvailableTypes(header.ImportedTypeNames);

            SchemaVTableTypes.PrepareClassVptrs(selection, scan.PolymorphicClasses, Console.Error.WriteLine);
            scan = ResolveTableLayouts(scan);
            VTableBindingSummary binding = BindFunctions(scan, selection);
            VTableTypeSummary types = VTableTypeBinder.Bind(scan.Tables, new SchemaVTableTypes(Console.Error.WriteLine));
            Console.Error.WriteLine(
                $"[schema] {Path.GetFileName(binaryPath)}: project={selection.Project}, types={importedTypes}, " +
                $"vtables-found={scan.MatchedVTables}, vtable-types={types.Completed}, vtable-addresses-bound={types.Bound}, " +
                $"unknown-slots={types.UnknownSlots}, vtable-conflicts={types.Conflicts}, bound={binding.Bound}, skipped={binding.Skipped}, " +
                $"conflicts={binding.Conflicts}, clang-errors={clangErrors}" +
                (clangErrors == 0 ? "." : " (ignored; valid declarations were imported)."));
            return new SchemaImportResult(true, selection.Project, importedTypes, scan.MatchedVTables,
                binding.Bound, binding.Skipped, binding.Conflicts, clangErrors,
                types.Completed, types.Bound, types.UnknownSlots, types.Conflicts);
        }
        finally
        {
            if (!keepHeader)
            {
                try
                {
                    Directory.Delete(tempDirectory, recursive: true);
                }
                catch (IOException)
                {
                    // The generated header is disposable; import success must not depend on temp cleanup.
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
    }

    internal static SchemaTargetPlatform DetectTargetPlatform()
    {
        byte* buffer = stackalloc byte[256];
        nuint length = IdaNative.get_file_type_name(buffer, 256);
        string format = length == 0 ? string.Empty : Encoding.UTF8.GetString(buffer, checked((int)Math.Min(length, 255)));
        SchemaTargetPlatform platform;
        if (format.Contains("ELF", StringComparison.OrdinalIgnoreCase))
        {
            platform = SchemaTargetPlatform.LinuxItanium;
        }
        else if (format.Contains("Portable executable", StringComparison.OrdinalIgnoreCase) ||
                 format.Contains("PE", StringComparison.OrdinalIgnoreCase))
        {
            platform = SchemaTargetPlatform.WindowsMsvc;
        }
        else
        {
            throw new SchemaImportException($"Schema import supports only PE x64 and ELF x64, not '{format}'.");
        }

        nuint count = IdaNative.get_func_qty();
        for (nuint i = 0; i < count; i++)
        {
            void* function = IdaNative.getn_func(i);
            if (function != null)
            {
                if (IdaNative.get_func_bitness(function) != 2)
                {
                    throw new SchemaImportException("Schema import supports only 64-bit databases.");
                }
                return platform;
            }
        }
        throw new SchemaImportException("Cannot verify schema platform bitness because IDA found no functions.");
    }

    internal static void ConfigureClang(
        string hl2SdkPath,
        SchemaTargetPlatform platform,
        bool skipLayoutAssertions)
    {
        string[] includeDirectories =
        [
            hl2SdkPath,
            Path.Combine(hl2SdkPath, "public"),
            Path.Combine(hl2SdkPath, "game"),
            Path.Combine(hl2SdkPath, "game", "shared"),
            Path.Combine(hl2SdkPath, "game", "server"),
            Path.Combine(hl2SdkPath, "game", "client"),
            Path.Combine(hl2SdkPath, "public", "tier0"),
            Path.Combine(hl2SdkPath, "public", "tier1"),
            Path.Combine(hl2SdkPath, "public", "mathlib"),
            Path.Combine(hl2SdkPath, "public", "entity2"),
            Path.Combine(hl2SdkPath, "public", "engine"),
            Path.Combine(hl2SdkPath, "public", "game", "server"),
            Path.Combine(hl2SdkPath, "thirdparty", "protobuf-3.21.8", "src"),
            Path.Combine(hl2SdkPath, "common"),
        ];

        var arguments = new List<string>
        {
            "-x", "c++", "-std=c++17", "-ferror-limit=100", "-Wno-c++11-narrowing", "-Wno-invalid-offsetof",
        };
        if (skipLayoutAssertions)
        {
            arguments.Add("-DS2ATELIER_SCHEMA_SKIP_LAYOUT_ASSERTS=1");
        }
        if (platform == SchemaTargetPlatform.WindowsMsvc)
        {
            arguments.AddRange(
            [
                "-target", "x86_64-pc-windows-msvc19.16.27045", "-D_CRT_STDIO_LEGACY_WIDE_SPECIFIERS",
                "-D_CRT_SECURE_NO_WARNINGS", "-D_CRT_NONSTDC_NO_DEPRECATE", "-D_CRT_SECURE_NO_DEPRECATE",
                "-D_CRT_DECLARE_NONSTDC_NAMES", "-DDEBUG", "-D_DEBUG", "-DWIN32", "-D_WINDOWS",
                "-DCOMPILER_MSVC", "-DCOMPILER_MSVC64", "-DX64BITS", "-DPLATFORM_64BITS", "-D_WIN32=1",
                "-D_ALLOW_COMPILER_AND_STL_VERSION_MISMATCH=1", "-D__unaligned=",
                "-D_CRT_USE_BUILTIN_OFFSETOF=1",
            ]);
        }
        else
        {
            arguments.AddRange(
            [
                "-target", "x86_64-unknown-linux-gnu", "-DPOSIX", "-DLINUX", "-DCOMPILER_GCC",
                "-DPLATFORM_64BITS", "-DX64BITS", "-D_CRT_USE_BUILTIN_OFFSETOF=1",
            ]);
        }
        foreach (string directory in includeDirectories.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            arguments.Add("-I" + QuoteArgument(directory));
        }

        string argv = string.Join(' ', arguments);
        byte* parser = Utf8.Allocate("clang");
        byte* nativeArgv = Utf8.Allocate(argv);
        try
        {
            if (IdaNative.select_parser_by_name(parser) == 0)
            {
                throw new SchemaImportException("IDAClang parser 'clang' is not installed in this IDA instance.");
            }
            int status = IdaNative.set_parser_argv(parser, nativeArgv);
            if (status != 0)
            {
                throw new SchemaImportException($"set_parser_argv(\"clang\") failed with status {status}.");
            }
        }
        finally
        {
            Utf8.Free(nativeArgv);
            Utf8.Free(parser);
        }
    }

    internal static int ParseHeader(string path, bool testOnly, bool printDiagnostics)
    {
        byte* parser = Utf8.Allocate("clang");
        byte* input = Utf8.Allocate(path);
        try
        {
            int flags = HtiFile | HtiNoWarnings | HtiIgnoreErrors | HtiDeclarations | (testOnly ? HtiTest : 0);
            if (printDiagnostics)
            {
                IdaNative.enable_console_messages(1);
            }
            try
            {
                return IdaNative.parse_decls_with_parser_ext(parser, IdaNative.get_idati(), input, flags);
            }
            finally
            {
                if (printDiagnostics)
                {
                    IdaPlatform.FlushNativeOutput();
                    IdaNative.enable_console_messages(0);
                    // Some IDAClang diagnostics do not terminate their last line. Keep the next
                    // JSON worker message from being appended to it and becoming unparsable.
                    Console.Out.WriteLine();
                    Console.Out.Flush();
                }
            }
        }
        finally
        {
            Utf8.Free(input);
            Utf8.Free(parser);
        }
    }

    private static int CountAvailableTypes(IEnumerable<string> names)
    {
        void* idati = IdaNative.get_idati();
        int count = 0;
        foreach (string name in names.Distinct(StringComparer.Ordinal))
        {
            byte* native = Utf8.Allocate(name);
            try
            {
                if (IdaNative.get_named_type(idati, native, NtfType | NtfNoBase,
                        null, null, null, null, null, null) == 1)
                {
                    count++;
                }
            }
            finally
            {
                Utf8.Free(native);
            }
        }
        return count;
    }

    internal static void DeleteReplacedTypes(IEnumerable<string> names)
    {
        void* idati = IdaNative.get_idati();
        foreach (string name in names.OrderByDescending(x => x.Count(c => c == ':')).ThenBy(x => x, StringComparer.Ordinal))
        {
            byte* native = Utf8.Allocate(name);
            try
            {
                IdaNative.del_named_type(idati, native, NtfType);
            }
            finally
            {
                Utf8.Free(native);
            }
        }
    }

    internal static VTableScan ScanVTables(SchemaSelection selection, SchemaTargetPlatform platform)
    {
        IdaNative.auto_wait();
        var functionOwners = new Dictionary<ulong, HashSet<string>>();
        var polymorphic = new HashSet<string>(StringComparer.Ordinal);
        var pureCalls = new HashSet<ulong>();
        var unresolvedThis = new HashSet<ulong>();
        var descriptors = new List<VTableDescriptor>();
        var boundaries = new SortedSet<ulong>();
        var found = new Dictionary<ulong, SchemaVTable>();
        var existing = SchemaVTableTypes.Existing(selection).ToDictionary(x => x.Address);
        nuint nameCount = IdaNative.get_nlist_size();
        for (nuint i = 0; i < nameCount; i++)
        {
            byte* rawPointer = IdaNative.get_nlist_name(i);
            if (rawPointer == null) continue;
            string rawName = Marshal.PtrToStringUTF8((nint)rawPointer) ?? string.Empty;
            ulong address = IdaNative.get_nlist_ea(i);
            boundaries.Add(address);
            if (IsPureCallName(rawName)) pureCalls.Add(address);
            if (!rawName.StartsWith(platform == SchemaTargetPlatform.WindowsMsvc ? "??_7" : "_ZTV", StringComparison.Ordinal))
                continue;
            if (VTableAnalysis.TryDescribe(address, rawName, Demangle(rawName), out VTableDescriptor descriptor) &&
                selection.Classes.ContainsKey(descriptor.ClassName)) descriptors.Add(descriptor);
        }
        foreach (var saved in existing.Values) boundaries.Add(saved.Address);
        foreach (VTableDescriptor descriptor in descriptors.OrderBy(x => x.Address))
        {
            // Symbols supply a hard stopping boundary, not proof that every byte before it is a slot.
            ulong? end = boundaries.GetViewBetween(descriptor.Address + 1, ulong.MaxValue).FirstOrDefault();
            if (end == 0) end = null;
            IReadOnlyList<VTableSlice> slices;
            if (descriptor.Abi == VTableAbi.Msvc)
            {
                var functions = VTableEntryScanner.ScanMsvc(VTableMemory, descriptor.Address, endExclusive: end);
                slices = [new(0, functions, descriptor.Address, functions.Count == 1024)];
            }
            else slices = VTableEntryScanner.ScanItaniumTables(VTableMemory, descriptor.Address, endExclusive: end);
            foreach (VTableSlice slice in slices.Where(x => x.Functions.Count > 0))
            {
                ulong? offset = descriptor.Abi == VTableAbi.Itanium ? checked((ulong)-slice.OffsetToTop)
                    : ReadMsvcObjectOffset(slice.AddressPoint) ?? (descriptor.SecondaryBaseName == null ? 0UL : null);
                string? owner = descriptor.Abi == VTableAbi.Msvc
                    ? descriptor.SecondaryBaseName ?? descriptor.ClassName
                    : ResolveBaseAtOffset(descriptor.ClassName, slice.OffsetToTop, selection);
                if (owner != null && !selection.Classes.ContainsKey(owner)) owner = null;
                found.TryAdd(slice.AddressPoint, new(descriptor.ClassName, slice.AddressPoint, offset,
                    owner, owner == null || owner == descriptor.ClassName ? [] : [owner], slice.Functions,
                    existing.TryGetValue(slice.AddressPoint, out var prior) ? prior.Name : null, slice.Truncated));
            }
        }
        // Bound types supply a trusted slot count, including null/unknown entries. Read them before replacement.
        foreach (var saved in existing.Values)
        {
            ulong end = saved.Address + (ulong)saved.Slots * 8;
            ulong next = boundaries.GetViewBetween(saved.Address + 1, ulong.MaxValue).FirstOrDefault();
            if (next != 0) end = Math.Min(end, next);
            var slots = VTableEntryScanner.ScanMsvc(VTableMemory, saved.Address, endExclusive: end, trustedExtent: true);
            if (slots.Count == 0) continue;
            if (found.TryGetValue(saved.Address, out SchemaVTable? scanned))
                found[saved.Address] = scanned with { Functions = slots.Count >= scanned.Functions.Count ? slots : scanned.Functions };
            else found.Add(saved.Address, new(saved.Metadata.ClassName, saved.Address, saved.Metadata.ObjectOffset,
                saved.Metadata.ThisType, saved.Metadata.BasePath, slots, saved.Name));
        }
        foreach (SchemaVTable table in found.Values)
        {
            polymorphic.Add(table.ClassName);
            if (table.ThisType != null) polymorphic.Add(table.ThisType);
            if (table.Truncated) Console.Error.WriteLine($"[schema] {table.ClassName} vtable 0x{table.AddressPoint:X}: scan limit reached.");
        }
        var expected = new HashSet<string>(polymorphic, StringComparer.Ordinal);
        var queue = new Queue<string>(expected);
        while (queue.TryDequeue(out string? name))
            if (selection.Classes.TryGetValue(name, out SchemaClass? schema) && schema.BaseClasses.Count > 0 &&
                expected.Add(schema.BaseClasses[0])) queue.Enqueue(schema.BaseClasses[0]);
        var matchedClasses = found.Values.Select(x => x.ClassName).ToHashSet(StringComparer.Ordinal);
        string[] missing = expected.Except(matchedClasses).Order(StringComparer.Ordinal).ToArray();
        if (missing.Length > 0)
            Console.Error.WriteLine($"[schema] {missing.Length} inferred polymorphic class(es) have no actual vtable: " +
                string.Join(", ", missing.Take(16)) + (missing.Length > 16 ? ", ..." : "") + ".");
        return new VTableScan(functionOwners, polymorphic, pureCalls, unresolvedThis, found.Count, found.Values.ToArray());
    }

    private static ulong? ReadMsvcObjectOffset(ulong addressPoint)
    {
        if (addressPoint < 8 || !VTableMemory.CanReadPointer(addressPoint - 8)) return null;
        ulong locator = IdaNative.get_qword(addressPoint - 8);
        if (locator == 0 || locator > ulong.MaxValue - 23 || IdaNative.is_mapped(locator + 23) == 0 ||
            IdaNative.is_mapped(locator) == 0 || IdaNative.get_dword(locator) != 1) return null;
        // PE x64 RTTICompleteObjectLocator uses image-relative references; validate its self RVA.
        uint self = IdaNative.get_dword(locator + 20);
        if (self > locator || IdaNative.is_mapped(locator - self) == 0) return null;
        uint type = IdaNative.get_dword(locator + 12), hierarchy = IdaNative.get_dword(locator + 16);
        if (IdaNative.is_mapped(locator - self + type) == 0 || IdaNative.is_mapped(locator - self + hierarchy) == 0)
            return null;
        return IdaNative.get_dword(locator + 4);
    }

    private static VTableScan ResolveTableLayouts(VTableScan scan)
    {
        var tables = new List<SchemaVTable>();
        var owners = new Dictionary<ulong, HashSet<string>>();
        var unresolved = new HashSet<ulong>();
        foreach (SchemaVTable table in scan.Tables)
        {
            SchemaVTable resolved = SchemaVTableTypes.ResolveLayout(table);
            tables.Add(resolved);
            ulong[] functions = resolved.Functions.Where(IsFunctionStart).ToArray();
            if (resolved.ThisType == null) unresolved.UnionWith(functions);
            else AddVTable(functions, resolved.ClassName, resolved.ThisType, owners, new HashSet<string>());
        }
        return scan with { Tables = tables, FunctionOwners = owners, UnresolvedThisAddresses = unresolved };
    }

    private static void AddVTable(
        IReadOnlyList<ulong> functions,
        string className,
        string owner,
        IDictionary<ulong, HashSet<string>> functionOwners,
        ISet<string> polymorphic)
    {
        polymorphic.Add(className);
        polymorphic.Add(owner);
        foreach (ulong function in functions)
        {
            if (!functionOwners.TryGetValue(function, out HashSet<string>? owners))
            {
                owners = new HashSet<string>(StringComparer.Ordinal);
                functionOwners.Add(function, owners);
            }
            owners.Add(owner);
        }
    }

    private static string? ResolveBaseAtOffset(string className, long offsetToTop, SchemaSelection selection)
    {
        if (offsetToTop == 0)
        {
            return className;
        }
        ulong wanted = unchecked((ulong)-offsetToTop);
        foreach ((string name, ulong offset) in EnumerateBaseOffsets(className, 0, selection, new HashSet<string>(StringComparer.Ordinal)))
        {
            if (offset == wanted)
            {
                return name;
            }
        }
        return null;
    }

    private static IEnumerable<(string Name, ulong Offset)> EnumerateBaseOffsets(
        string className, ulong objectOffset, SchemaSelection selection, ISet<string> visiting)
    {
        if (!visiting.Add(className) || !selection.Classes.TryGetValue(className, out SchemaClass? type))
        {
            yield break;
        }
        ulong cursor = 0;
        for (int i = 0; i < type.BaseClasses.Count; i++)
        {
            string baseName = type.BaseClasses[i];
            if (!selection.Classes.TryGetValue(baseName, out SchemaClass? baseType))
            {
                continue;
            }
            ulong alignment = (ulong)(baseType.Alignment is 1 or 2 or 4 or 8 or 16 ? baseType.Alignment : 1);
            ulong relative = i == 0 ? 0 : (cursor + alignment - 1) & ~(alignment - 1);
            ulong absolute = objectOffset + relative;
            yield return (baseName, absolute);
            foreach (var nested in EnumerateBaseOffsets(baseName, absolute, selection,
                         new HashSet<string>(visiting, StringComparer.Ordinal)))
            {
                yield return nested;
            }
            cursor = relative + (ulong)baseType.Size;
        }
    }

    private static bool IsFunctionStart(ulong address)
    {
        if (address == 0 || IdaNative.is_mapped(address) == 0)
        {
            return false;
        }
        void* function = IdaNative.get_func(address);
        return function != null && *(ulong*)function == address;
    }

    private static string? Demangle(string rawName)
    {
        byte* native = Utf8.Allocate(rawName);
        var output = new QString();
        try
        {
            return IdaNative.demangle_name(&output, native, 0, 2) > 0 ? output.Read() : null;
        }
        finally
        {
            output.Dispose();
            Utf8.Free(native);
        }
    }

    private static VTableBindingSummary BindFunctions(VTableScan scan, SchemaSelection selection)
    {
        var diagnostics = new LimitedDiagnostics(32);
        VTableBindingSummary result = VTableFunctionBinder.Bind(scan.FunctionOwners, scan.PureCallAddresses,
            scan.UnresolvedThisAddresses, selection.Classes, new IdaFunctionTypeEditor(diagnostics),
            (address, owners) => diagnostics.Write(
                $"[schema] vfunc 0x{address:X}: unrelated vtables disagree on this type ({string.Join(", ", owners.Order())}); skipped."));
        diagnostics.Finish();
        return result;
    }

    private static bool TryBindThisParameter(ulong address, string owner, LimitedDiagnostics diagnostics)
    {
        TypeInfo original = default;
        try
        {
            if (IdaNative.get_tinfo(&original, address) == 0 && IdaNative.guess_tinfo(&original, address) == 0)
            {
                diagnostics.Write($"[schema] vfunc 0x{address:X}: no usable prototype; skipped.");
                return false;
            }
            nuint rawCount = IdaNative.get_tinfo_property(original.Typid, GtaFunctionArgumentCount);
            if (rawCount > 255)
            {
                diagnostics.Write($"[schema] vfunc 0x{address:X}: existing type is not a function prototype; skipped.");
                return false;
            }

            if (rawCount > 0)
            {
                TypeInfo thisType = default;
                var thisName = new QString();
                byte* thisDeclaration = Utf8.Allocate($"{owner} *__s2_this;");
                try
                {
                    if (IdaNative.parse_decl(&thisType, &thisName, IdaNative.get_idati(), thisDeclaration,
                            PtSilent | PtVariable | PtHigh) == 0)
                    {
                        diagnostics.Write(
                            $"[schema] vfunc 0x{address:X}: this type '{owner} *' is unavailable; skipped.");
                        return false;
                    }
                    if (IdaNative.set_tinfo_property4(&original, StaFunctionArgumentType, 0,
                            (nuint)(void*)&thisType, 0, 0) != 0)
                    {
                        diagnostics.Write($"[schema] vfunc 0x{address:X}: could not replace argument 0 type; skipped.");
                        return false;
                    }

                    byte* nativeThisName = Utf8.Allocate("this");
                    try
                    {
                        if (IdaNative.set_tinfo_property4(&original, StaFunctionArgumentName, 0,
                                (nuint)nativeThisName, 0, 0) != 0)
                        {
                            diagnostics.Write($"[schema] vfunc 0x{address:X}: could not name argument 0 'this'; skipped.");
                            return false;
                        }
                    }
                    finally
                    {
                        Utf8.Free(nativeThisName);
                    }
                    return IdaNative.apply_tinfo(address, &original, TinfoDefinite) != 0;
                }
                finally
                {
                    Utf8.Free(thisDeclaration);
                    thisName.Dispose();
                    thisType.Dispose();
                }
            }

            const string marker = "__s2_vfunc_marker";
            byte* markerNative = Utf8.Allocate(marker);
            var printed = new QString();
            string declaration;
            try
            {
                if (IdaNative.print_tinfo(&printed, null, 0, 0, 0, &original, markerNative, null) == 0)
                {
                    return false;
                }
                declaration = VTableAnalysis.RewriteFirstParameter(printed.Read(), marker, owner, checked((int)rawCount));
                if (!declaration.TrimEnd().EndsWith(';'))
                {
                    declaration += ";";
                }
            }
            finally
            {
                printed.Dispose();
                Utf8.Free(markerNative);
            }

            TypeInfo replacement = default;
            var parsedName = new QString();
            byte* nativeDeclaration = Utf8.Allocate(declaration);
            try
            {
                if (IdaNative.parse_decl(&replacement, &parsedName, IdaNative.get_idati(), nativeDeclaration,
                        PtSilent | PtVariable | PtHigh) == 0)
                {
                    diagnostics.Write(
                        $"[schema] vfunc 0x{address:X}: IDA could not add a this parameter to zero-argument prototype; " +
                        $"declaration={declaration}; skipped.");
                    return false;
                }

                byte* thisName = Utf8.Allocate("this");
                try
                {
                    if (IdaNative.set_tinfo_property4(&replacement, StaFunctionArgumentName, 0,
                            (nuint)thisName, 0, 0) != 0)
                    {
                        diagnostics.Write($"[schema] vfunc 0x{address:X}: could not name argument 0 'this'; skipped.");
                        return false;
                    }
                }
                finally
                {
                    Utf8.Free(thisName);
                }

                return IdaNative.apply_tinfo(address, &replacement, TinfoDefinite) != 0;
            }
            finally
            {
                Utf8.Free(nativeDeclaration);
                parsedName.Dispose();
                replacement.Dispose();
            }
        }
        finally
        {
            original.Dispose();
        }
    }

    private static bool IsPureCallName(string name)
        => name.Contains("purecall", StringComparison.OrdinalIgnoreCase) ||
           name.Contains("pure_virtual", StringComparison.OrdinalIgnoreCase);

    private static string QuoteArgument(string value)
        => value.Any(char.IsWhiteSpace) ? $"\"{value.Replace("\"", "\\\"", StringComparison.Ordinal)}\"" : value;

    internal sealed record VTableScan(
        IReadOnlyDictionary<ulong, HashSet<string>> FunctionOwners,
        IReadOnlySet<string> PolymorphicClasses,
        IReadOnlySet<ulong> PureCallAddresses,
        IReadOnlySet<ulong> UnresolvedThisAddresses,
        int MatchedVTables,
        IReadOnlyList<SchemaVTable> Tables)
    {
        public int FunctionAddressCount => FunctionOwners.Keys.Concat(UnresolvedThisAddresses).Distinct().Count();
    }

    private sealed class IdaVTableMemory : IVTableMemory
    {
        public ulong ReadPointer(ulong address) => IdaNative.get_qword(address);
        public bool IsMapped(ulong address) => IdaNative.is_mapped(address) != 0;
        public bool IsFunctionStart(ulong address) => SchemaImport.IsFunctionStart(address);
        public bool CanReadPointer(ulong address) => address <= ulong.MaxValue - 7 && IsMapped(address) && IsMapped(address + 7);
    }

    private sealed class IdaFunctionTypeEditor(LimitedDiagnostics diagnostics) : IVirtualFunctionTypeEditor
    {
        public bool TryBindThis(ulong address, string owner) => TryBindThisParameter(address, owner, diagnostics);
    }

    private sealed class LimitedDiagnostics(int limit)
    {
        private int _seen;

        public void Write(string message)
        {
            if (_seen++ < limit)
            {
                Console.Error.WriteLine(message);
            }
        }

        public void Finish()
        {
            int suppressed = _seen - limit;
            if (suppressed > 0)
            {
                Console.Error.WriteLine($"[schema] suppressed {suppressed} additional per-function diagnostic(s).");
            }
        }
    }
}
