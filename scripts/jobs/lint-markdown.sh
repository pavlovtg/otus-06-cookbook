#!/bin/sh
set -e

cd "$(dirname "$0")/../.."

markdownlint-cli2 "**/*.md" "#**/node_modules/**"
