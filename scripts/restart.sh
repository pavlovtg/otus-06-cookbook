#!/bin/sh
set -e

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

log() {
  printf "${GREEN}==>${NC} %s\n" "$1"
}

warn() {
  printf "${YELLOW}==>${NC} %s\n" "$1"
}

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT_DIR"

log "Stopping containers..."
docker compose down --remove-orphans

log "Rebuilding images (no cache)..."
docker compose build --no-cache

log "Starting containers (waiting for healthy)..."
docker compose up -d --wait

log "Done. Current status:"
docker compose ps
