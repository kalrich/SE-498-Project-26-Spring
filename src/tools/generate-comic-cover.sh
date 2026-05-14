#!/usr/bin/env bash
set -euo pipefail

if [ "$#" -lt 1 ]; then
  echo "Usage: tools/generate-comic-cover.sh path/to/comic.pdf [cover-name]"
  exit 1
fi

pdf_path="$1"
cover_name="${2:-}"

if [ ! -f "$pdf_path" ]; then
  echo "PDF not found: $pdf_path"
  exit 1
fi

if ! command -v qlmanage >/dev/null 2>&1; then
  echo "qlmanage is required on macOS to generate the cover image."
  exit 1
fi

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cover_dir="$repo_root/Project498.WebServer/wwwroot/images/covers"
tmp_dir="$(mktemp -d)"

cleanup() {
  rm -rf "$tmp_dir"
}
trap cleanup EXIT

if [ -z "$cover_name" ]; then
  filename="$(basename "$pdf_path")"
  cover_name="${filename%.*}_COVER.png"
fi

qlmanage -t -s 900 -o "$tmp_dir" "$pdf_path" >/dev/null 2>&1

generated_cover="$(find "$tmp_dir" -type f -name '*.png' | head -n 1)"
if [ -z "$generated_cover" ]; then
  echo "Could not generate a cover image for: $pdf_path"
  exit 1
fi

mkdir -p "$cover_dir"
cp "$generated_cover" "$cover_dir/$cover_name"

echo "/images/covers/$cover_name"
