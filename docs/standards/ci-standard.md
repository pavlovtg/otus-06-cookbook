# Стандарт: CI Pipeline

Источник: ADR-0030

## Платформа

- CI реализован на GitHub Actions.
- Конфигурация хранится в `.github/workflows/`.
- Два workflow-файла: `ci-push.yml` (push в feature-ветки) и `ci-pr.yml` (PR в `main`).

## Режимы запуска

### Push в feature-ветку

- Запускаются только jobs для затронутых компонентов (paths-фильтры через `dorny/paths-filter`).
- Компоненты: `cookbook` (`apps/Cookbook/**`), `apigateway` (`apps/ApiGateway/**`), `web` (`apps/web/**`).
- `lint-markdown` — только при изменении `**/*.md`.
- `lint-python` — только при изменении `tests/e2e/**`.
- `e2e`, `ui-test` — только при изменении `apps/**`, `docker-compose.yml`, `infrastructure/**`.

### PR в `main`

- Запускаются все jobs без фильтров.
- Падение любого job блокирует merge (branch protection rules).

## Инструменты

| Стек | Линтер | Тест-раннер |
|------|--------|-------------|
| C# (.NET 10) | `dotnet format --verify-no-changes` | `dotnet test` |
| Next.js | `pnpm lint` (`next lint`) | `pnpm test` (vitest) |
| Python | `ruff check` | `pytest` |
| Markdown | `markdownlint-cli2` | — |
| UI (браузер) | — | Playwright |

## Структура jobs

```
lint-cookbook ───┐
lint-apigateway ─┤
lint-web ────────┤  (параллельно)
lint-markdown ───┤
lint-python ─────┘

test-cookbook ───┐
test-apigateway ─┤  (параллельно)
test-web ────────┘

build-cookbook   (needs: lint-cookbook, test-cookbook)
build-apigateway (needs: lint-apigateway, test-apigateway)
build-web        (needs: lint-web, test-web)

e2e              (needs: build-*)
ui-test          (needs: build-*)
```

## E2e тесты

- Инструмент: pytest, расположение `tests/e2e/`.
- Окружение: `docker compose up -d --wait`, `BASE_URL=http://localhost:5500`.
- Cleanup: `docker compose down` в `if: always()`.

## UI-тесты

- Инструмент: Playwright, расположение `tests/ui/`.
- Запускаются на собранном docker-compose окружении.

## Обязательные шаги

- **Lint** — статический анализ; pipeline падает при ошибках.
- **Test** — все unit и integration тесты; pipeline падает при провале.
- **Build** — сборка Docker-образов; pipeline падает при ошибке сборки.
- Падающий pipeline блокирует merge pull request в `main`.
