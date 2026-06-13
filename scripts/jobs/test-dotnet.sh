#!/bin/sh
set -e

cd "$(dirname "$0")/../.."

dotnet test apps/Backend.slnx
