# Прогресс

## Завершено

- Storybook компонентов: `docs/design/storybook/` (React 19 + Storybook 8 Vite), порт mockup → React 1:1, примитивы + домен + дашборд, ★ Playground на каждую группу, build OK
- HTML-прототип MVP: `docs/design/mockup/`
- STYLE_GUIDE из Tradeo Fintech SaaS (Dribbble + webflow): `docs/design/guide/STYLE_GUIDE.md`, мудборд + structure_selects
- Инициализация проекта (монорепо, структура, ADR)
- MVP: Cookbook API (DDD, Hexagonal, EF Core, PostgreSQL)
- ApiGateway (YARP)
- Frontend (Next.js 15, BFF, Zod)
- Docker Compose (5 сервисов, healthcheck, nginx reverse proxy)
- Архитектурные документы: ADR-0001..ADR-0033, AR-0005..AR-0039
- Стандарты: ci-standard, csharp-code-style, markdown-code-style и др.
- CI Pipeline:
  - `.github/workflows/ci-push.yml` (paths-фильтры, dorny/paths-filter)
  - `.github/workflows/ci-pr.yml` (полный прогон)
  - `.markdownlint.json`
  - `tests/e2e/requirements.txt`
  - `.editorconfig` — C# правила стиля
- Локальные CI-скрипты: `scripts/jobs/` (10 job-скриптов) + `scripts/` (lint/test/build/run-ci)
- HTML-прототип MVP: `docs/design/mockup/index.html` + `styles.css` (single-file SPA, fakeApi, все сценарии MVP)
- AR-0041 применён к Recipes.Tests и ApiGateway.Tests: файлы разложены по `Unit/`, `Integration/`, `Microservice/`

## В работе

Нет.

## Выполнено (последнее)

- Оптимизация Docker build context: добавлены `apps/Cookbook/.dockerignore` и `apps/ApiGateway/.dockerignore` (исключают `bin/`, `obj/`, `TestResults/`, `tests/`).

## Запланировано

- UI-тесты (Playwright) — `tests/ui/` (job зарезервирован в CI с `if: false`)
- Auth Service (ADR-0021, ADR-0022)
