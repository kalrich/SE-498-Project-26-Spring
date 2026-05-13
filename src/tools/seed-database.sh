#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/.." && pwd)"

cd "$repo_root"

docker compose up -d db

echo "Waiting for Postgres..."
until docker compose exec -T db pg_isready -U postgres >/dev/null 2>&1; do
  sleep 1
done

echo "Loading schema.sql..."
docker compose exec -T db psql -U postgres -d project498 < Project498.WebApi/Database/schema.sql

echo "Loading seed.sql..."
docker compose exec -T db psql -U postgres -d project498 < Project498.WebApi/Database/seed.sql

if [ -f Project498.WebApi/Database/dc-comics.seed.sql ]; then
  echo "Loading dc-comics.seed.sql..."
  docker compose exec -T db psql -U postgres -d project498 < Project498.WebApi/Database/dc-comics.seed.sql
fi

echo "Database is ready."
