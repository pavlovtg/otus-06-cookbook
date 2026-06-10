#!/bin/sh
set -e

cd "$(dirname "$0")/../.."

dotnet format apps/Cookbook/Cookbook.slnx --verify-no-changes
dotnet format apps/ApiGateway/ApiGateway.slnx --verify-no-changes
