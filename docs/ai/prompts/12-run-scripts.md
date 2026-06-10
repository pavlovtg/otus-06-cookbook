# Промт: создание локальных CI-скриптов

## Роль

- Инженер, пишущий shell-скрипты для локального воспроизведения CI-пайплайна.

## Контекст

- Монорепо с тремя стеками: .NET (`apps/Cookbook`, `apps/ApiGateway`), Next.js (`apps/web`), Python (`tests/e2e`, `tests/ui`).
- CI-пайплайн состоит из трёх этапов: `lint`, `test`, `build`.
- Разработчик хочет запускать каждый job и весь пайплайн локально без параметров.

## Задача

- Создать sh-скрипты, зеркально повторяющие CI-пайплайн.
- Структура файлов:

```
/scripts/jobs/
  lint-markdown.sh
  lint-dotnet.sh
  lint-nextjs.sh
  lint-python.sh
  test-dotnet.sh
  test-nextjs.sh
  test-e2e.sh
  test-ui.sh
  build-dotnet.sh
  build-nextjs.sh
/scripts/
  lint.sh       — запускает все lint-jobs
  test.sh       — запускает все test-jobs
  build.sh      — запускает все build-jobs
  run-ci.sh     — запускает lint → test → build
```

## Требования

- Формат: `sh` (shebang `#!/bin/sh`).
- Запуск без параметров.
- Каждый job-скрипт выполняет ровно одну проверку/сборку.
- Агрегирующие скрипты (`lint.sh`, `test.sh`, `build.sh`, `run-ci.sh`) вызывают job-скрипты последовательно.
- При ошибке любого шага скрипт завершается с ненулевым кодом (`set -e` или явная проверка `$?`).
- Пути — относительно корня репозитория; скрипты запускаются из корня.
- Скрипты должны быть исполняемыми (`chmod +x`).
- Без лишних комментариев в коде.
