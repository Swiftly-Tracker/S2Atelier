# Tests

Run the managed suite with .NET 10:

```powershell
dotnet run --project S2Atelier.Tests
```

The native schema and HL2SDK vtable tests are opt-in. They need a licensed IDA/idalib installation
and a PE x64 or ELF x64 input containing at least one recognized function:

```powershell
$env:IDA_PATH = 'D:\Software\ida93sp2'
$env:S2ATELIER_TEST_VTABLES = 'D:\inputs\sample.dll'
dotnet run --project S2Atelier.Tests
```

The input is copied into a disposable temporary directory. The test creates a
scratch segment and synthetic schema classes/tables in that database, then closes
without saving. It never opens or modifies the original input's database. Repeat
with an ELF input and with each installed supported IDA SDK (9.2 and 9.3).

Native assertions cover UDT/pointer ABI layouts, MSVC COL offsets, Itanium address
points, binding discovery and deduplication, duplicate and unknown slots, exact
function prototypes, VFT flags, primary/secondary class pointers, unchanged shared
bases, foreign binding conflicts, adopting older VFT definitions, and importing
again after replacing schema classes. Secondary type names use IDA's required
`Class_XXXX_vtbl` convention (minimum four hexadecimal offset digits).

The HL2SDK test parses a synthetic SDK into independent temporary TILs, checks
MSVC/Itanium destructor slots, partial/inherited definitions (including `unk`
names), secondary subobjects, const methods, dependency relocation, exact VFT
prototypes and address binding. It also checks repeat imports after schema
replacement, shared-function conflicts, purecall exclusion, and protection of
edited function names/types. No temporary TIL references may survive disposal.

With `HL2SDK_PATH` (default `D:\Code\hl2sdk`), the native test additionally parses
the real `CEntityInstance` and relocates its virtual prototypes while preserving
an existing schema layout. This smoke test is skipped for an ELF target on
Windows, where the native MSVC headers cannot supply Linux system headers;
synthetic ELF ABI tests still run. Missing native prerequisites print `SKIP`.

Managed tests cover definition discovery/ambiguity, SDK and inherited slot
priority, table-length/layout conflicts, fallback, member-name disambiguation,
and ownership metadata. SDK header failures are isolated to their header group;
the importer reports SDK/inherited/fallback slot counts and retains failed
headers for diagnosis.

Without `S2ATELIER_TEST_VTABLES`, the native vtable test prints an explicit `SKIP`.
The older game/HL2SDK integration tests remain separately gated by
`S2ATELIER_TEST_CLIENT`, `S2ATELIER_TEST_SERVER`, `S2ATELIER_TEST_ELF`, and
`S2ATELIER_TEST_INTERFACES`, together with `HL2SDK_PATH` and `S2ATELIER_SDK_JSON`.
