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
with an ELF input and with each installed supported IDA SDK (9.2, 9.3 and 9.4).

Native assertions cover UDT/pointer ABI layouts, MSVC COL offsets, Itanium address
points, binding discovery and deduplication, duplicate and unknown slots, exact
function prototypes, VFT flags, primary/secondary class pointers, unchanged shared
bases, foreign binding conflicts, adopting older VFT definitions, and importing
again after replacing schema classes. Secondary type names use IDA's required
`Class_XXXX_vtbl` convention (minimum four hexadecimal offset digits).

The HL2SDK test parses a synthetic SDK into independent temporary TILs, checks
MSVC/Itanium destructor slots, partial/inherited definitions (including `unk`
names), secondary subobjects, const/volatile methods, dependency relocation, exact VFT
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

SDK resolution also tests 64 duplicate tables: dependency traversal is performed once
per successful source prototype per Resolve call, while all output slots are preserved.
Directly constructed `this` pointers must compare equal to the parser-produced type;
unknown owners are rejected and portable prototypes remain compact after source TIL disposal.
Schema import logs `[schema-timing]` per phase and reports dependency-walk counts and
SDK prototype resolution time. Fixture timings are not end-to-end game benchmarks.

Override ownership tests distinguish a derived implementation inherited by descendants
from an unchanged base implementation, using the actual function-address owners. They
reject unrelated/unknown or nonzero-offset ownership and preserve user edits. The selected
owner must occur among the observed tables; the SDK declaration owner is not substituted.

For an existing database **copy**, set `S2ATELIER_REBIND_IDB`, `IDA_PATH`, `HL2SDK_PATH`,
and `S2ATELIER_SDK_JSON`, then run the test executable to rebind SDK functions without
reimporting schema. It writes `<database>.repair.json`. Set `S2ATELIER_VERIFY_BINDINGS=1`
to reopen the saved copy and verify every renamed function without saving changes.
