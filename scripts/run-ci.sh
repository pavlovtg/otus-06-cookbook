#!/bin/sh
set -e

SCRIPTS_DIR="$(dirname "$0")"

sh "$SCRIPTS_DIR/lint.sh"
sh "$SCRIPTS_DIR/test.sh"
sh "$SCRIPTS_DIR/build.sh"
