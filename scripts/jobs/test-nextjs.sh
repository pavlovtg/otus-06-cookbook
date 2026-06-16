#!/bin/sh
set -e

cd "$(dirname "$0")/../../apps/web"

pnpm test:coverage
