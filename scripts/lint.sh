#!/bin/sh
set -e

JOBS_DIR="$(dirname "$0")/jobs"

sh "$JOBS_DIR/lint-markdown.sh"
sh "$JOBS_DIR/lint-dotnet.sh"
sh "$JOBS_DIR/lint-nextjs.sh"
sh "$JOBS_DIR/lint-python.sh"
