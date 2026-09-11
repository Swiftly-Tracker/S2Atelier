#!/usr/bin/env bash
set -euo pipefail
# Run on the Linux VPS from the repository root; no host .NET installation needed.
root=${S2A_ROOT:-/opt/s2atelier}
mkdir -p "$root/app" "$root/tools/atelier" "$root/tools/nuget"
docker run --rm -v "$PWD:/src" -v "$root:$root" -w /src \
  -e NUGET_PACKAGES="$root/tools/nuget" mcr.microsoft.com/dotnet/sdk:10.0 \
  bash -euc "dotnet run --project S2Atelier.Tests -c Release; \
    dotnet publish S2Atelier.csproj -c Release -r linux-x64 -o '$root/tools/atelier/linux'; \
    dotnet publish S2Atelier.csproj -c Release -r win-x64 -o '$root/tools/atelier/windows'; \
    dotnet publish S2Atelier.Automation -c Release -r linux-x64 --self-contained true -o '$root/app'"
docker build -t s2atelier-linux:local -f S2Atelier.Automation/deploy/Dockerfile.linux S2Atelier.Automation/deploy
docker build -t s2atelier-windows:local -f S2Atelier.Automation/deploy/Dockerfile.windows S2Atelier.Automation/deploy
