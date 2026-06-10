#!/bin/sh
set -e

cd "$(dirname "$0")/../.."

docker build -f apps/Cookbook/Dockerfile .
docker build -f apps/ApiGateway/Dockerfile .
