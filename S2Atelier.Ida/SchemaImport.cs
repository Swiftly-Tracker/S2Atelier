using System.Diagnostics;
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
        var stageClock = Stopwatch.StartNew();
        void Stage(string name)
        {
            Console.Error.WriteLine($"[schema-timing] {name}: {stageClock.Elapsed.TotalSeconds:F3}s");
            stageClock.Restart();
        }
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
        Stage("load/scan/header");
        string tempDirectory = Path.Combine(Path.GetTempPath(), $"s2atelier-schema-{Guid.NewGuid():N}");
        string tempHeader = Path.Combine(tempDirectory, "schema.hpp");
        bool keepHeader = false;
        try
        {
            Directory.CreateDirectory(tempDirectory);
            File.WriteAllText(tempHeader, header.Text, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            ConfigureClang(hl2SdkPath, platform, skipLayoutAssertions: false, tempDirectory);
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
            ConfigureClang(hl2SdkPath, platform, skipLayoutAssertions: true, tempDirectory);
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
            Stage("schema preflight/import");

            SchemaVTableTypes.PrepareClassVptrs(selection, scan.PolymorphicClasses, Console.Error.WriteLine);
            scan = ResolveTableLayouts(scan);
            Stage("class vptr/layout resolution");
            using var sdk = Hl2SdkVTables.Load(hl2SdkPath, platform, selection, scan.Tables, Console.Error.WriteLine);
            Stage("SDK header load");
            var slots = sdk.Resolve(scan.Tables, selection);
            Stage("SDK prototype resolution");
            VTableBindingSummary sdkBinding = SdkFunctionBinding.Bind(scan.Tables, slots,
                scan.PureCallAddresses, scan.UnresolvedThisAddresses, Console.Error.WriteLine);
            Stage("SDK function binding");
            // SDK candidates, including protected/conflicting addresses, must not be rewritten by fallback.
            var sdkAddresses = scan.Tables.SelectMany(table => table.Functions
                .Where((_, index) => slots.ContainsKey((table.AddressPoint, index)))).ToHashSet();
            VTableBindingSummary fallback = BindFunctions(scan with
            {
                FunctionOwners = scan.FunctionOwners.Where(x => !sdkAddresses.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Value),
                UnresolvedThisAddresses = scan.UnresolvedThisAddresses.Except(sdkAddresses).ToHashSet(),
            }, selection);
            var binding = new VTableBindingSummary(sdkBinding.Bound + fallback.Bound,
                sdkBinding.Skipped + fallback.Skipped, sdkBinding.Conflicts + fallback.Conflicts, fallback.Named);
            Stage("fallback function binding");
            var constructorDiagnostics = new LimitedDiagnostics(32);
            ConstructorNamingSummary constructors = ConstructorNaming.Apply(selection, platform, constructorDiagnostics);
            constructorDiagnostics.Finish();
            Stage("constructor naming");
            var argumentDiagnostics = new LimitedDiagnostics(32);
            ArgumentPropagationSummary arguments =
                ArgumentPropagation.Run(ConVarNaming.ArgumentRegisters(), argumentDiagnostics);
            argumentDiagnostics.Finish();
            Stage("argument propagation");
            VTableTypeSummary types = VTableTypeBinder.Bind(scan.Tables, new SchemaVTableTypes(Console.Error.WriteLine, slots));
            Stage("vtable type binding");
            Console.Error.WriteLine(
                $"[schema] {Path.GetFileName(binaryPath)}: project={selection.Project}, types={importedTypes}, " +
                $"vtables-found={scan.MatchedVTables}, vtable-types={types.Completed}, vtable-addresses-bound={types.Bound}, " +
                $"unknown-slots={types.UnknownSlots}, vtable-conflicts={types.Conflicts}, bound={binding.Bound}, skipped={binding.Skipped}, " +
                $"conflicts={binding.Conflicts}, slot-names={binding.Named}, " +
                $"constructors={constructors.Found}, constructors-named={constructors.Named}, " +
                $"argument-candidates={arguments.Candidates}, arguments-typed={arguments.Typed}, " +
                $"arguments-without-common-base={arguments.NoCommonBase}, clang-errors={clangErrors}" +
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
        bool skipLayoutAssertions,
        string? generatedIncludeDirectory = null)
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
            "-x", "c++", "-std=c++17", "-U__tuple", "-frtti", "-ferror-limit=100", "-Wno-c++11-narrowing", "-Wno-invalid-offsetof",
            // Layout assertions name private SDK members. "#define private public" cannot reach headers
            // that the leading includes already pulled in (e.g. entityidentity.h via eiface.h).
            "-fno-access-control",
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
                "-Dstricmp=strcasecmp", "-Dstrnicmp=strncasecmp",
            ]);
        }
        if (generatedIncludeDirectory != null) arguments.Add("-I" + QuoteArgument(generatedIncludeDirectory));
        // eiface.h/igameevents.h declare interface methods that pass CNetMessagePB<T> (see
        // netmessage.h) for a couple of protobuf message types, by value of the template's base.
        // hl2sdk ships only the .proto source for those, not compiled headers, so without this
        // T is incomplete at the point those declarations need it. Compile the small, fixed set
        // this SDK actually references with the SDK's own protoc (it patches codegen to drop
        // "final" from message classes - CNetMessagePB<T> inherits T, which a stock protoc's
        // "final" would make illegal - so a generic protoc release is not a substitute).
        arguments.Add("-I" + QuoteArgument(NetworkProtobufHeaders(hl2SdkPath)));
        foreach (string directory in includeDirectories.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            arguments.Add("-I" + QuoteArgument(directory));
        }

        // Container installations do not have the compiler discovery/registry setup of
        // a developer workstation. Allow explicitly provisioned compiler headers.
        string? resourceDirectory = Environment.GetEnvironmentVariable("S2ATELIER_CLANG_RESOURCE_DIR");
        if (!string.IsNullOrWhiteSpace(resourceDirectory))
        {
            arguments.Add("-I" + QuoteArgument(Path.Combine(resourceDirectory, "include")));
        }
        string? systemIncludes = Environment.GetEnvironmentVariable("S2ATELIER_CLANG_INCLUDE_PATH");
        foreach (string directory in (systemIncludes ?? "").Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            // IDAClang accepts joined -I arguments; standalone -isystem is
            // filtered out and would turn the following path into another input.
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

    // IDA saves the selected source parser and its arguments in the database, and the GUI parses every
    // declaration the user types (e.g. a variable type in the decompiler) with them. ConfigureClang
    // points IDAClang at temporary headers and layout-only macros, which break those declarations, so
    // every saved database ends with IDAClang's arguments cleared and the legacy parser selected.
    internal static void ResetParser()
    {
        byte* clang = Utf8.Allocate("clang");
        byte* empty = Utf8.Allocate(string.Empty);
        byte* legacy = Utf8.Allocate("legacy");
        try
        {
            IdaNative.set_parser_argv(clang, empty);
            // Selecting "legacy" by name stores it explicitly; the documented empty name reports
            // success in IDA 9.3 but leaves the current parser selected.
            IdaNative.select_parser_by_name(legacy);
        }
        finally
        {
            Utf8.Free(legacy);
            Utf8.Free(empty);
            Utf8.Free(clang);
        }

        // get_selected_parser_name reports the legacy parser as an empty name.
        QString name = default;
        try
        {
            if (IdaNative.get_selected_parser_name(&name) != 0 && name.Read().Length != 0)
            {
                Console.Error.WriteLine($"[clang] could not select the legacy source parser; '{name.Read()}' remains selected.");
            }
        }
        finally
        {
            name.Dispose();
        }
    }

    // (source .proto path relative to hl2SdkPath, in dependency order so a single protoc
    // invocation can compile all of them - protoc only emits output for files listed explicitly).
    private static readonly string[] NetworkProtoSources =
    [
        Path.Combine("common", "networkbasetypes.proto"),
        Path.Combine("common", "valveextensions.proto"),
        Path.Combine("common", "network_connection.proto"),
        Path.Combine("common", "source2_steam_stats.proto"),
        Path.Combine("common", "netmessages.proto"),
        Path.Combine("game", "shared", "gameevents.proto"),
    ];

    // One compile per hl2SdkPath for the process lifetime: an IDA worker analyzes many binaries
    // against the same SDK, and protoc's own startup cost dwarfs compiling these six small files.
    private static readonly Dictionary<string, string?> NetworkProtobufDirectoryCache = new(StringComparer.OrdinalIgnoreCase);
    private static readonly object NetworkProtobufDirectoryLock = new();

    private static string NetworkProtobufHeaders(string hl2SdkPath)
    {
        lock (NetworkProtobufDirectoryLock)
        {
            if (NetworkProtobufDirectoryCache.TryGetValue(hl2SdkPath, out string? cached))
            {
                return cached ?? NetworkProtobufFallbackStub();
            }
            string? compiled = CompileNetworkProtobufHeaders(hl2SdkPath);
            NetworkProtobufDirectoryCache[hl2SdkPath] = compiled;
            return compiled ?? NetworkProtobufFallbackStub();
        }
    }

    private static string? CompileNetworkProtobufHeaders(string hl2SdkPath)
    {
        string protoc = Path.Combine(hl2SdkPath, "devtools", "bin",
            OperatingSystem.IsWindows() ? "protoc.exe" : Path.Combine("linux", "protoc"));
        string[] sources = NetworkProtoSources.Select(x => Path.Combine(hl2SdkPath, x)).ToArray();
        if (!File.Exists(protoc) || sources.Any(x => !File.Exists(x)))
        {
            return null;
        }

        string directory = Path.Combine(Path.GetTempPath(), "s2atelier-sdk-protos-" +
            Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(hl2SdkPath)))[..16]);
        string marker = Path.Combine(directory, "netmessages.pb.h");
        // Content is fully determined by hl2SdkPath's own checked-in .proto files; a prior
        // compile in this or an earlier process is already correct, and workers share this cache.
        if (File.Exists(marker))
        {
            return directory;
        }

        try
        {
            Directory.CreateDirectory(directory);
            var info = new ProcessStartInfo(protoc)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            info.ArgumentList.Add("-I" + Path.Combine(hl2SdkPath, "common"));
            info.ArgumentList.Add("-I" + Path.Combine(hl2SdkPath, "game", "shared"));
            info.ArgumentList.Add("-I" + Path.Combine(hl2SdkPath, "thirdparty", "protobuf-3.21.8", "src"));
            info.ArgumentList.Add("--cpp_out=" + directory);
            foreach (string source in sources) info.ArgumentList.Add(source);

            using Process? process = Process.Start(info);
            if (process == null)
            {
                return null;
            }
            process.WaitForExit();
            return process.ExitCode == 0 && File.Exists(marker) ? directory : null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.ComponentModel.Win32Exception)
        {
            return null;
        }
    }

    private static string? _fallbackStubDirectory;

    // hl2sdk checkouts without devtools/bin (or an unexpected .proto layout) fall back to a stub
    // that only satisfies the hard, unconditional "#include" in eiface.h/inetchannel.h - the same
    // gap this used to paper over before real headers were available, minus the accurate enum.
    private static string NetworkProtobufFallbackStub()
    {
        if (_fallbackStubDirectory != null)
        {
            return _fallbackStubDirectory;
        }
        string directory = Path.Combine(Path.GetTempPath(), "s2atelier-network-proto-stub");
        Directory.CreateDirectory(directory);
        string stub = Path.Combine(directory, "network_connection.pb.h");
        if (!File.Exists(stub))
        {
            File.WriteAllText(stub, "#pragma once\ntypedef int ENetworkDisconnectionReason;\n",
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        }
        _fallbackStubDirectory = directory;
        return directory;
    }

    internal static int ParseHeader(string path, bool testOnly, bool printDiagnostics, void* targetTil = null)
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
                return IdaNative.parse_decls_with_parser_ext(parser, targetTil == null ? IdaNative.get_idati() : targetTil, input, flags);
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
        // MSVC can give every vftable of a class the same unqualified symbol. Without a readable
        // locator such a table's subobject is unknown; assuming offset 0 makes it a second primary table.
        var ambiguousUnqualified = descriptors.Where(x => x.Abi == VTableAbi.Msvc && x.SecondaryBaseName == null)
            .GroupBy(x => x.ClassName, StringComparer.Ordinal).Where(x => x.Count() > 1)
            .Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
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
                    : ReadMsvcObjectOffset(slice.AddressPoint) ??
                      (descriptor.SecondaryBaseName == null && !ambiguousUnqualified.Contains(descriptor.ClassName) ? 0UL : null);
                string? owner = descriptor.Abi == VTableAbi.Msvc
                    // An unqualified symbol only names the complete class for its primary table.
                    ? descriptor.SecondaryBaseName ?? (offset == 0 ? descriptor.ClassName : null)
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
            // Statically linked CRTs often lack a FLIRT match, leaving _purecall unnamed. It is still
            // recognizable: every pure-virtual slot points at it and it never returns (it aborts).
            foreach (ulong function in table.Functions)
                if (IsFunctionStart(function) && IdaNative.func_does_return(function) == 0) pureCalls.Add(function);
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
        // The image base itself (the PE header) is usually not loaded, so validate the referenced
        // type descriptor and class hierarchy instead of requiring the base to be mapped.
        uint self = IdaNative.get_dword(locator + 20);
        if (self > locator) return null;
        ulong imageBase = locator - self;
        uint type = IdaNative.get_dword(locator + 12), hierarchy = IdaNative.get_dword(locator + 16);
        ulong typeName = imageBase + type + 16;
        if (type == 0 || hierarchy == 0 || IdaNative.is_mapped(typeName + 3) == 0 ||
            IdaNative.is_mapped(imageBase + hierarchy + 15) == 0 ||
            IdaNative.get_byte(typeName) != '.' || IdaNative.get_byte(typeName + 1) != '?' ||
            IdaNative.get_byte(typeName + 2) != 'A')
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
                $"[schema] vfunc 0x{address:X}: unrelated vtables disagree on this type ({string.Join(", ", owners.Order())}); skipped."),
            scan.Tables);
        diagnostics.Finish();
        return result;
    }

    // argumentName null keeps the name IDA gave argument 0, for functions not known to be methods.
    internal static bool TryBindThisParameter(ulong address, string owner, LimitedDiagnostics diagnostics,
        string? argumentName = "this")
    {
        if (!SdkFunctionBinding.CanUpdateType(address))
        {
            diagnostics.Write($"[schema] vfunc 0x{address:X}: explicit/edited prototype preserved.");
            return false;
        }
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

                    byte* nativeThisName = argumentName == null ? null : Utf8.Allocate(argumentName);
                    try
                    {
                        if (nativeThisName != null && IdaNative.set_tinfo_property4(&original, StaFunctionArgumentName, 0,
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

                // The rewritten declaration names the parameter with a placeholder; a1 is IDA's own name for it.
                byte* thisName = Utf8.Allocate(argumentName ?? "a1");
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
        private readonly HashSet<ulong> _typed = [];

        public bool TryBindThis(ulong address, string owner)
            => TryBindThisParameter(address, owner, diagnostics) && _typed.Add(address);

        public bool TryName(ulong address, string name)
            => SdkFunctionBinding.TryNameFunction(address, name, _typed.Contains(address), "vtable slot", diagnostics.Write);
    }

    internal sealed class LimitedDiagnostics(int limit)
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
