#!/bin/sh
set -e

cd "$(dirname "$0")/../.."

trap 'docker compose down' EXIT

docker compose up -d --wait

docker run --rm \
  -v "$(pwd):/src" -w /src \
  --network host \
  -e BASE_URL=http://localhost:5500 \
  mcr.microsoft.com/playwright/python:v1.60.0-jammy \
  sh -c "pip install -r tests/e2e/requirements.txt -q && pytest tests/e2e"
