#!/usr/bin/env bash
set -euo pipefail
root=${S2A_ROOT:-/opt/s2atelier}
repo="$root/sources/llvm-project"
if [[ ! -d "$repo/.git" ]]; then
  git clone --depth 1 --filter=blob:none --sparse --branch llvmorg-21.1.8 https://github.com/llvm/llvm-project.git "$repo"
fi
git -C "$repo" sparse-checkout set clang/lib/Headers
mkdir -p "$root/tools/clang21/include"
cp -a "$repo/clang/lib/Headers/." "$root/tools/clang21/include/"
git -C "$repo" rev-parse HEAD > "$root/tools/clang21/REVISION"
