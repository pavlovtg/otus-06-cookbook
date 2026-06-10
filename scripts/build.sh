#!/bin/sh
set -e

JOBS_DIR="$(dirname "$0")/jobs"

sh "$JOBS_DIR/build-dotnet.sh"
sh "$JOBS_DIR/build-nextjs.sh"
