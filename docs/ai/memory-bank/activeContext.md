# Активный контекст

## Текущая задача

**Создание локальных CI-скриптов** (10.06.2026)

## Что сделано в этой сессии

- Создано 10 job-скриптов в `scripts/jobs/`
- Создано 4 агрегирующих скрипта: `lint.sh`, `test.sh`, `build.sh`, `run-ci.sh`
- Все скрипты: `#!/bin/sh`, `set -e`, `chmod +x`
- `test-e2e.sh` и `test-ui.sh`: `trap 'docker compose down' EXIT` для cleanup

## Следующий шаг

Нет активных задач.
