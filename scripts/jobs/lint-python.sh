#!/bin/sh
set -e

cd "$(dirname "$0")/../.."

docker run --rm \
  -v "$(pwd):/src" -w /src \
  ghcr.io/astral-sh/ruff:latest check tests/e2e tests/ui
