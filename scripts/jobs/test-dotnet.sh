#!/bin/sh
set -e

cd "$(dirname "$0")/../.."

dotnet test apps/Cookbook/Cookbook.slnx
dotnet test apps/ApiGateway/ApiGateway.slnx
