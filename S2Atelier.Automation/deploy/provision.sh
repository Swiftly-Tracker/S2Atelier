#!/usr/bin/env bash
set -euo pipefail
# Debian 13, root. Docker must already be installed. Run from repository root.
root=${S2A_ROOT:-/opt/s2atelier}
export DEBIAN_FRONTEND=noninteractive
apt-get update
apt-get install -y git curl unzip python3 cmake g++ make libicu76 7zip
mkdir -p "$root"/{sources,tools,jobs,ida/linux,ida/windows,app}
sync_repo() {
  local name=$1 url=$2 branch=$3
  if [[ ! -d "$root/sources/$name/.git" ]]; then
    git clone --single-branch --branch "$branch" "$url" "$root/sources/$name"
  else
    git -C "$root/sources/$name" pull --ff-only
  fi
  git -C "$root/sources/$name" submodule update --init --recursive
}
sync_repo hl2sdk https://github.com/alliedmodders/hl2sdk.git cs2
sync_repo CS2-Dumps https://github.com/Swiftly-Tracker/CS2-Dumps.git main
cmake -S "$root/sources/hl2sdk/thirdparty/protobuf-3.21.8/cmake" -B "$root/tools/protobuf-build" \
  -Dprotobuf_BUILD_TESTS=OFF -DCMAKE_BUILD_TYPE=Release
cmake --build "$root/tools/protobuf-build" --target protoc -j2
curl -fL --retry 3 -o "$root/tools/downloader.zip" \
  https://github.com/Swiftly-Tracker/SteamDepotDownload/releases/latest/download/SteamDepotDownload-linux-x64.zip
unzip -o "$root/tools/downloader.zip" -d "$root/tools/downloader"
chmod +x "$root/tools/downloader/SteamDepotDownload.App"
sha256sum "$root/tools/downloader.zip" > "$root/tools/downloader.sha256"
bash S2Atelier.Automation/deploy/clang_headers.sh
bash S2Atelier.Automation/deploy/windows_sdk.sh
bash S2Atelier.Automation/deploy/build.sh
# Provision IDA separately into ida/linux and ida/windows before enabling the service.
test -f "$root/ida/linux/libidalib.so"
test -f "$root/ida/windows/idalib.dll"
python3 S2Atelier.Automation/deploy/configure_clang.py "$root/ida/linux" "$root/ida/windows"
mkdir -p "$root/state/windows" "$root/state/linux"
docker run --rm --entrypoint python3 -v "$root/ida/linux:/ida:ro" -v "$root/state/linux:/state" \
  -v "$PWD/S2Atelier.Automation/deploy/accept_eula.py:/accept_eula.py:ro" \
  s2atelier-linux:local /accept_eula.py /ida /state
if [[ ! -f "$root/service.env" ]]; then
  umask 077
  python3 - "$root" <<'PY'
import pathlib, secrets, sys
root = pathlib.Path(sys.argv[1])
(root / 'service.env').write_text(f'S2A_ROOT={root}\nS2A_API_KEY={secrets.token_hex(32)}\nASPNETCORE_URLS=http://127.0.0.1:5080\n')
PY
fi
install -m 644 S2Atelier.Automation/deploy/s2atelier.service /etc/systemd/system/s2atelier.service
systemctl daemon-reload
systemctl enable --now s2atelier
