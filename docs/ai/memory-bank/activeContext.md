# Активный контекст

## Текущая задача

Нет активной задачи.

## Последнее выполненное

Перемещение ADR-0005 и ADR-0006 в домен `rest-api`:

- Файлы перемещены: `docs/adr/general/` → `docs/adr/rest-api/`
- Поле `**Домен**` обновлено в обоих ADR: `general` → `rest-api`
- Обновлены ссылки в: `ARCHITECTURE.md`, `AR-0002`, `api-design.md`, `components.md`, `overview.md`

## Предыдущее выполненное

Рефакторинг skills архитектурной документации (промт `09-update-architecture-skills.md`):

- Симметричная структура «один артефакт — один skill»: `adr`, `ar`, `standard`, `diagram`.
- Удалены `update-architecture-from-{adr,ar,standard}`; шаблон стандарта перенесён в `standard/assets/`.
- В каждом skill добавлен явный блок «Консистентность» (индекс в `ARCHITECTURE.md`, перекрёстные ссылки, `overview.md`, `components.md`).
- Правила «Домен» и «Нумерация» оставлены внутри skills (экономия токенов базового контекста).
- `.clinerules/architecture.md` сведён к 6 триггерам.
- Актуализированы индексы AR/стандартов/ADR в `ARCHITECTURE.md` (AR-0001/0002, стандарты api-design/docker-compose/testing/ci, ADR-0004…0007).


## Предыдущее выполненное

Выполнен промт `docs/ai/prompts/08-adr-from-assigment.md`:

- ADR-0004 — PostgreSQL как СУБД
- ADR-0005 — JWT-аутентификация
- ADR-0006 — REST API как протокол взаимодействия
- ADR-0007 — Docker Compose как среда развёртывания
- Стандарт `docs/standards/api-design.md`
- Стандарт `docs/standards/docker-compose-standard.md`
- Стандарт `docs/standards/testing.md` (включая e2e тесты)
- Стандарт `docs/standards/ci-standard.md` (pipeline блокирует PR)
- AR-0001 `docs/architecture/rules/general/AR-0001-docker-compose-self-contained.md`
- AR-0002 `docs/architecture/rules/general/AR-0002-swagger-ui-live.md`
- Обновлён `docs/architecture/overview.md` — назначение системы, технологии, ссылки на ADR
- Обновлён `docs/architecture/components.md` — Backend, Frontend, Database, Infrastructure

## Предыдущее выполненное

Создан промт `docs/ai/prompts/08-adr-from-assigment.md`:

- Промт генерирует за один запуск: 4 ADR (PostgreSQL, JWT, REST API, Docker Compose), 4 стандарта (`api-design.md`, `docker-compose-standard.md`, `testing.md`, `ci-standard.md`), 1 AR (`AR-docker-compose-self-contained.md`).
- Промт также обновляет `docs/architecture/overview.md` и `docs/architecture/components.md`.

## Предыдущее выполненное

Выполнен промт `docs/ai/prompts/07-add-reporting-workflow.md`:

- Создан `.clinerules/workflows/add-report.md` — workflow для добавления записей в `REPORT.md`.
- Создан `REPORT.md` с 5 разделами; раздел `## Примеры промтов` заполнен таблицей по промтам 01, 03, 04, 05.

## Выполнено

- `.clinerules/language.md` — язык и экономия токенов
- `.clinerules/workflows/create-prompt.md` — workflow создания промтов
- `.clinerules/git.md` — правила git
- `.clineignore` — игнор нерелевантных файлов
- `.gitignore` — игнор для git
- `docs/ai/memory-bank/` — инициализирован
- `docs/ai/prompts/01-markdown-code-style.md` — промт для создания Markdown code style
- `docs/standards/markdown-code-style.md` — стандарт оформления Markdown
- `docs/ai/prompts/02-update-openspec-architecture.md` — промт для системы архитектурной документации
- Система архитектурной документации — создана
- `ARCHITECTURE.md` — переименован из `architecture.md`
- Contributing-документация — `CONTRIBUTING.md`, `docs/contributing/`

## Следующие шаги

-
