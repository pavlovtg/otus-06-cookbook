#!/bin/sh
set -e

JOBS_DIR="$(dirname "$0")/jobs"

sh "$JOBS_DIR/test-dotnet.sh"
sh "$JOBS_DIR/test-nextjs.sh"
sh "$JOBS_DIR/test-e2e.sh"
sh "$JOBS_DIR/test-ui.sh"
