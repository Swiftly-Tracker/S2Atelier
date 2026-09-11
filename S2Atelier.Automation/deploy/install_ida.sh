#!/usr/bin/env bash
set -euo pipefail
# Installs the user-selected IDA 9.4 distribution. Patches require an explicit flag.
root=${S2A_ROOT:-/opt/s2atelier}
mkdir -p "$root"/{sources,tools,ida/linux,ida/windows}
base=https://github.com/Yigods/ida94_b1/releases/download/v9.4-installers
curl -fL --retry 3 -o "$root/tools/ida-linux.run" "$base/ida-pro_94_x64linux.run"
curl -fL --retry 3 -o "$root/tools/ida-windows.exe" "$base/ida-pro_94_x64win.exe"
(cd "$root/tools"; sha256sum -c <<'SUMS'
ec59282dba8e72eca2a750b5514f3d6722022d8b29bfc0cebd29a6403d2db19a  ida-linux.run
e15ffe98ef66cb797e21bf6cb25bd6c37ae7eee2717dd21660e0aaada14a7ddb  ida-windows.exe
SUMS
)
chmod +x "$root/tools/ida-linux.run"
"$root/tools/ida-linux.run" --mode unattended --prefix "$root/ida/linux" --activate_idalib 0
# Build the Wine execution environment first. --init is required by xvfb-run.
docker build -t s2atelier-windows:local -f S2Atelier.Automation/deploy/Dockerfile.windows S2Atelier.Automation/deploy
docker run --rm --init --entrypoint xvfb-run -v "$root/tools:/install:ro" -v "$root/ida/windows:/ida" \
  s2atelier-windows:local -a wine /install/ida-windows.exe --mode unattended --prefix 'Z:\ida' --install_python 0 --activate_idalib 0
if [[ ${1:-} == --apply-source-patches ]]; then
  source_dir="$root/sources/ida94_b1"
  if [[ ! -d "$source_dir/.git" ]]; then git clone https://github.com/Yigods/ida94_b1.git "$source_dir"; fi
  git -C "$source_dir" fetch origin
  git -C "$source_dir" checkout --detach 0f05a79bbb52bdece0c358b758b4f0914d22e96e
  cp "$source_dir"/kg_patch/x64linux/libida*.so "$root/ida/linux/"
  cp "$source_dir"/kg_patch/x64win/ida*.dll "$root/ida/windows/"
  (cd "$root/ida/linux"; python3 "$source_dir/kg_patch/全平台注册机IDA94b1.py")
  cp "$root/ida/linux/idapro.hexlic" "$root/ida/windows/"
fi
printf '%s\n' 'IDA installed. Supply licenses if needed, then run deploy/provision.sh.'
