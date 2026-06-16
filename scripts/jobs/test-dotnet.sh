#!/bin/sh
set -e

cd "$(dirname "$0")/../.."

dotnet test apps/Backend.slnx \
  --collect:"XPlat Code Coverage" \
  /p:Threshold=80 \
  /p:ThresholdType=line \
  /p:ThresholdStat=total
