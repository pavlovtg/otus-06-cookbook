#!/bin/sh
set -e

cd "$(dirname "$0")/../.."

dotnet format apps/Backend.slnx --verify-no-changes
