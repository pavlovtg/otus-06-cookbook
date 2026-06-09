# GitHub Actions CI Pipeline

## Роль

- Senior DevOps / Platform Engineer, знающий GitHub Actions, .NET 10, Next.js, Python, Docker Compose.

## Контекст

- Монорепо с тремя приложениями:
  - `apps/Cookbook` — .NET 10 / C#, тесты через `dotnet test`, решение `Cookbook.slnx`
  - `apps/ApiGateway` — .NET 10 / C#, тесты через `dotnet test`, решение `ApiGateway.slnx`
  - `apps/web` — Next.js 15 / TypeScript, пакетный менеджер `pnpm`, lint через `next lint`, тесты через `vitest run`
- E2e тесты — Python / pytest, расположены в `tests/e2e/`, зависимости в `tests/e2e/requirements.txt`
- UI-тесты — Playwright (браузерные), расположены в `tests/ui/` (будущий тип)
- Markdown документация — правила MD009, MD012, MD022, MD032, MD034, MD047, конфиг `.markdownlint.json` в корне
- `docker-compose.yml` в корне поднимает 5 сервисов: `reverse-proxy` (nginx, порт 5500), `web`, `api-gateway`, `recipes`, `postgresql`
- Все сервисы имеют healthcheck; `docker compose up -d --wait` ждёт готовности
- E2e тесты стучатся на `BASE_URL=http://localhost:5500`
- Линтер C#: `dotnet format --verify-no-changes`
- Линтер Python: `ruff check`
- Линтер Markdown: `markdownlint-cli2`
- Образы собираются только локально, push в registry не нужен

## Задача

- Сгенерировать два workflow-файла:
  - `.github/workflows/ci-push.yml` — push на любую ветку кроме `main`, умный запуск по затронутым компонентам
  - `.github/workflows/ci-pr.yml` — PR в `main`, полный прогон всех jobs
- Сгенерировать `.markdownlint.json` с правилами MD009, MD012, MD022, MD032, MD034, MD047.
- Сгенерировать `tests/e2e/requirements.txt` с минимальным набором зависимостей (pytest, httpx).

## Требования

### Общая структура jobs (оба workflow)

```
lint-cookbook ───┐
lint-apigateway ─┤
lint-web ────────┤
lint-markdown ───┤  (параллельно)
lint-python ─────┘

test-cookbook ───┐
test-apigateway ─┤  (параллельно)
test-web ────────┘

build-cookbook  (needs: lint-cookbook, test-cookbook)
build-apigateway (needs: lint-apigateway, test-apigateway)
build-web       (needs: lint-web, test-web)

e2e             (needs: build-cookbook, build-apigateway, build-web)
```

### ci-push.yml — paths-фильтры

Каждый job запускается только при изменении соответствующих путей:

| Job | paths |
|-----|-------|
| `lint-cookbook`, `test-cookbook`, `build-cookbook` | `apps/Cookbook/**` |
| `lint-apigateway`, `test-apigateway`, `build-apigateway` | `apps/ApiGateway/**` |
| `lint-web`, `test-web`, `build-web` | `apps/web/**` |
| `lint-markdown` | `**/*.md` |
| `lint-python` | `tests/e2e/**` |
| `e2e` | `apps/**`, `docker-compose.yml`, `infrastructure/**` |

- Триггер: `push` на все ветки кроме `main`.
- Если paths не совпали — job пропускается (`if: needs.changes.outputs.X == 'true'` через `dorny/paths-filter`).

### ci-pr.yml — полный прогон

- Триггер: `pull_request` с `base: main`.
- Все jobs запускаются без фильтров по paths.
- Падение любого job блокирует merge (через branch protection rules — описать в комментарии).

### Детали jobs

- **lint-cookbook**: `dotnet format --verify-no-changes apps/Cookbook/Cookbook.slnx`
- **lint-apigateway**: `dotnet format --verify-no-changes apps/ApiGateway/ApiGateway.slnx`
- **lint-web**: `pnpm install --frozen-lockfile` + `pnpm lint` в `apps/web`
- **lint-markdown**: `markdownlint-cli2 "**/*.md" --ignore node_modules`
- **lint-python**: `ruff check tests/e2e/`
- **test-cookbook**: `dotnet test apps/Cookbook/Cookbook.slnx`
- **test-apigateway**: `dotnet test apps/ApiGateway/ApiGateway.slnx`
- **test-web**: `pnpm install --frozen-lockfile` + `pnpm test` в `apps/web`
- **build-cookbook**: `docker build apps/Cookbook`
- **build-apigateway**: `docker build apps/ApiGateway`
- **build-web**: `docker build apps/web`
- **e2e**:
  - `docker compose up -d --wait`
  - `pip install -r tests/e2e/requirements.txt`
  - `pytest tests/e2e/` с `BASE_URL=http://localhost:5500`
  - `docker compose down` в `if: always()` шаге (cleanup)

### Прочее

- Runner: `ubuntu-latest`.
- .NET: `10.0.x`, Node: LTS, Python: `3.12`.
- pnpm устанавливается через `pnpm/action-setup`.
- UI-тесты (Playwright) — зарезервировать job `ui-test` с `if: false` и комментарием о будущей реализации.
