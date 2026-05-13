#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/.." && pwd)"

cd "$repo_root/Project498.WebServer"

ApiBaseUrl="${ApiBaseUrl:-http://localhost:8082/}" dotnet run
