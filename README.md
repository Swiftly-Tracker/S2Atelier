# S2Atelier

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![Build Status](https://img.shields.io/github/actions/workflow/status/Swiftly-Tracker/S2Atelier/build.yml?branch=main)](https://github.com/Swiftly-Tracker/S2Atelier/actions)
[![Release](https://img.shields.io/github/v/release/Swiftly-Tracker/S2Atelier?include_prereleases)](https://github.com/Swiftly-Tracker/S2Atelier/releases)

Concurrent IDA-headless (idalib) auto-analysis over globbed Source 2 game binaries, built in C#.

Drives one IDA worker process per core against a batch of binaries, then layers naming/typing passes on top of IDA's own auto-analysis: ConVar/ConCommand recovery, entity class typing, log channel naming, function-pointer table naming, PLT stub repair, and import of protobufs, HL2SDK interfaces and schema classes. Each pass is best-effort and heuristic where the binary alone can't prove the answer - naming passes report what they found instead of silently guessing.

## What it does

- **Batch idalib analysis** - opens every glob match in its own IDA worker, `--cores` at a time, and saves the resulting database.
- **ConVar/ConCommand naming** - finds Source engine ConVar/ConCommand registrations, renames the objects (`cvar_`/`cmd_`) and command handlers (`cmd_..._callback`), and writes descriptions back as comments. `--convar-types` can source each convar's value type from a CS2-Dumps `convars.json` instead of recovering it from the registration.
- **Entity class naming/typing** - names and types every entity class's `CEntityClass` static, `CEntityClassInfo`, accessor, cached pointer, guards, callbacks, schema binding and data map. With `--hl2sdk` the layout is checked against the binary before it's applied.
- **CGlobalVars typing** - finds and types a module's `CGlobalVars` instance(s) and time-scope helpers for its build era.
- **Log channel naming** - names the globals `LoggingSystem_RegisterLoggingChannel` results are stored in.
- **Function-pointer table naming** - finds name-resolution cascades anywhere in the binary and renames the resolved functions on both ends of the chain.
- **PLT stub repair** - patches broken PLT stubs on ELF64/`.so` binaries linked with mold (a C# port of [PltPatcher](https://github.com/GAMMACASE/PltPatcher)).
- **Protobuf import** - parses protoc-generated `.pb.h` headers and reproduces the real compiled `google::protobuf::Message` layout as Local Types.
- **HL2SDK interface import** - locates the validated `ConnectInterfaces` table, renames its pointer slots to the official `g_p...` names, and imports virtual tables through IDAClang.
- **Schema import** - imports a build's schema classes/enums from a CS2-Dumps `sdk.json`, detects RTTI vtables, and binds virtual-function `this` parameters.
- **Baseline/snapshot drift protection** - `--snapshot` records what each pass found; `--baseline` compares against the previous build so a class whose vtable slots moved keeps its old SDK names (commented) until the SDK agrees again, instead of silently mis-renaming.

## Supported IDA versions

`--ida-sdk` selects which generated bindings to bind against: `auto` (default, detected from the installation), `9.2`, `9.3`, `9.4`.

## Install / Build

### Prerequisites

- [IDA Pro](https://hex-rays.com/ida-pro) 9.2, 9.3 or 9.4 with idalib, licensed for headless/library use
- **.NET 10 SDK** (only to build from source; releases are self-contained)

### Prebuilt

Grab an archive for your platform from the [latest release](https://github.com/Swiftly-Tracker/S2Atelier/releases/latest):

| Archive                                                                                                                    | Platform |
| -------------------------------------------------------------------------------------------------------------------------- | -------- |
| [`S2Atelier-win-x64.zip`](https://github.com/Swiftly-Tracker/S2Atelier/releases/latest/download/S2Atelier-win-x64.zip)     | Windows  |
| [`S2Atelier-linux-x64.zip`](https://github.com/Swiftly-Tracker/S2Atelier/releases/latest/download/S2Atelier-linux-x64.zip) | Linux    |

Those links always resolve to the newest stable release. On Linux, `chmod +x S2Atelier` after unzipping. Each archive is self-contained - no separate .NET runtime install needed.

### From source

```bash
git clone https://github.com/Swiftly-Tracker/S2Atelier.git
cd S2Atelier
dotnet build S2Atelier.slnx -c Release
```

Output lands in `bin/Release/net10.0/`. To produce a standalone binary that relaunches itself as its own IDA worker (see [Architecture](#architecture)):

```bash
dotnet publish S2Atelier.csproj -c Release \
  -r linux-x64 --self-contained true \
  -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true \
  -o out/linux-x64
```

## Usage

```
s2atelier <glob> [<glob> ...] [options]
```

| Flag                               | Description                                                                                                                                                                                      |
| ---------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| `--ida-path <dir>`                 | Root of the IDA installation (`ida`/`idalib` + `cfg/` + `procs/`). Falls back to the `IDA_PATH` environment variable.                                                                            |
| `--ida-sdk <ver>`                  | Generated bindings to bind against: `auto` (default), `9.2`, `9.3`, `9.4`.                                                                                                                       |
| `--cores <n>`                      | Concurrent worker processes. Default: all logical processors.                                                                                                                                    |
| `--root <dir>`                     | Base directory glob patterns are matched against. Default: cwd.                                                                                                                                  |
| `--no-save`                        | Do not save the resulting database.                                                                                                                                                              |
| `--progress`                       | Show a live per-worker progress bar instead of log lines.                                                                                                                                        |
| `--patch-plt`                      | Patch broken PLT stubs after analysis (ELF64/`.so` only). Fixes missing xrefs to imported functions on binaries linked with mold.                                                                |
| `--name-convars`                   | Find and rename ConVar/ConCommand registrations, with descriptions written back as comments. Heuristic, not guaranteed accurate.                                                                 |
| `--convar-types <convars.json>`    | With `--name-convars`, take each convar's value type from a runtime dump (CS2-Dumps' `convars.json`) instead of recovering it. Disagreements are reported.                                       |
| `--name-log-channels`              | Name logging channel globals `LOG_<CHANNEL NAME>`, typed `LoggingChannelID_t` when SDK headers were imported.                                                                                    |
| `--name-entity-classes`            | Name and type every entity class's static, accessor, guards, callbacks, schema binding and data map. Uses `--hl2sdk` layout when given; a layout the binary contradicts is reported and skipped. |
| `--type-globals`                   | Find the module's `CGlobalVars` and type it for its build era.                                                                                                                                   |
| `--name-fnptr-tables`              | Find name-resolution cascades and rename the resolved functions on both ends of the chain. Heuristic, not guaranteed accurate.                                                                   |
| `--import-protobufs <dir>`         | Parse every `*.pb.h` header under `<dir>` and add a Local Type per message/enum matching the real compiled layout. Point at the compiled proto build output, not `.proto` sources.               |
| `--import-schema <sdk.json>`       | Import the binary's schema classes/enums into Local Types, detect RTTI vtables, and bind virtual-function `this` parameters. Requires `--hl2sdk`. 64-bit PE/ELF only.                            |
| `--import-interfaces`              | Locate `ConnectInterfaces`, rename its pointer slots to official `g_p...` names, apply interface pointer types, and import virtual tables through IDAClang. Requires `--hl2sdk`.                 |
| `--hl2sdk <dir>`                   | HL2SDK root used by `--import-interfaces` and/or `--import-schema`.                                                                                                                              |
| `--snapshot <dir>`                 | Write this build's snapshot (vtables, per-pass health, entity graph) to `<dir>`, the baseline of the next build.                                                                                 |
| `--baseline <dir>`                 | Compare with the previous build's snapshot in `<dir>`; hold back SDK renames that the current vtable layout contradicts until the SDK agrees again.                                              |
| `--schema-project <auto\|project>` | Project roots to import. Default `auto` derives client/server/etc. from the binary filename (including `libNAME.so`).                                                                            |
| `-h`, `--help`                     | Show help.                                                                                                                                                                                       |

Examples:

```bash
s2atelier "bin/**/*.dll" "bin/**/*.so" --ida-path "C:\IDA" --cores 4
s2atelier "server.dll" --ida-path "D:\Software\ida93sp2" --import-interfaces --hl2sdk "D:\Code\hl2sdk"
s2atelier "bin/client.dll" --ida-path "C:\IDA" --import-schema "sdk.json" --hl2sdk "D:\Code\hl2sdk"
```

## Architecture

```
S2Atelier/
├── src/
│   ├── Entrypoint.cs                # CLI entry point, batch orchestration, reporting
│   └── CliOptions.cs                # Flag parsing and validation
├── S2Atelier.Ida/                   # Analysis passes, driven through generated P/Invoke bindings
│   ├── Worker/                      # Worker pool + one-database-per-process IPC protocol
│   ├── Schema/                      # sdk.json reader, dependency resolver, header writer
│   └── Generated/                   # Codegen output: bindings for each vendored IDA SDK version
├── S2Atelier.Ida.Codegen/           # Header scanner that generates the bindings above from thirdparty/ida-sdk
├── S2Atelier.Tests/                 # Test suite
├── S2Atelier.Automation/            # Hosted job service driving the release pipeline (see below)
└── thirdparty/
    └── ida-sdk/                     # Vendored Hex-Rays IDA SDK headers, one directory per major.minor
```

CI (`.github/workflows/build.yml`) versions with GitVersion, runs `S2Atelier.Tests`, publishes self-contained `win-x64`/`linux-x64` builds, and tags/releases on push to `main`.

Each publish is a single self-contained binary; a worker relaunches that exact binary as its own subprocess (`--ida-worker`) rather than depending on a separate runtime install.

## Automation

`S2Atelier.Automation` is a small hosted API that runs the end-to-end release pipeline on a dedicated Linux host: given a [CS2-Dumps](https://github.com/Swiftly-Tracker) commit and target platform, it drives the Windows/Linux IDA containers, runs the full analysis+import passes, and publishes verified archives as a GitHub release. `deploy/` holds the provisioning scripts (IDA install, EULA acceptance, clang headers, Docker images) used to stand up that host.

## Community

- **Issues**: [Report bugs and request features](https://github.com/Swiftly-Tracker/S2Atelier/issues)
- **Security**: [Report privately](https://github.com/Swiftly-Tracker/S2Atelier/security/advisories/new) - never in a public issue

## Acknowledgements

All of the acknowledgements can be seen in [THIRDPARTY.md](THIRDPARTY.md)

## License

GPL-3.0. See [LICENSE](LICENSE).

---

<div align="center">
  <strong>Made with ❤️ by the Swiftly Development team</strong>
</div>
