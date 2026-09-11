#!/usr/bin/env bash
set -euo pipefail
root=${S2A_ROOT:-/opt/s2atelier}
mkdir -p "$root/tools/xwin"
cd "$root/tools/xwin"
archive=xwin-0.10.0-x86_64-unknown-linux-musl.tar.gz
url="https://github.com/Jake-Shadle/xwin/releases/download/0.10.0/$archive"
curl -fL --retry 3 -o "$archive" "$url"
curl -fL --retry 3 -o "$archive.sha256" "$url.sha256"
expected=$(cat "$archive.sha256")
printf '%s  %s\n' "$expected" "$archive" | sha256sum -c -
tar -xzf "$archive" --strip-components=1
curl -fL --retry 3 --max-time 60 https://aka.ms/vs/17/release/channel -o channel.json
./xwin --accept-license --timeout 60 --manifest "$PWD/channel.json" --cache-dir "$root/tools/xwin-cache" splat --output "$root/tools/msvc"
