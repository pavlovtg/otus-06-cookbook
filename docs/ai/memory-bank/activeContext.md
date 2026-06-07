# Активный контекст

## Текущая задача

Нет активной задачи.

## Последнее выполненное

Дедупликация и компактизация ADR/AR/standards:

- AR сжаты с 15 до 10: слияние AR-0003+0004+0005 → AR-0004 «API Gateway boundary», AR-0012+0013+0014+0015 → AR-0010 «BFF boundary». AR перенумерованы без дыр.
- В каждом AR оставлены только `Правило` и (опц.) `Запрещено`; шапка убрана; домен = путь файла; вместо «Связанные ADR» — строка `Источник: ADR-NNNN`.
- ADR (19 шт.) приведены к минимальному формату: шапка (Статус, Дата), секции `Контекст` / `Решение` / `Последствия`; альтернативы свёрнуты в одну строку внутри `Контекст`; блоки «Связанные документы» удалены; поле «Домен» удалено (определяется путём).
- Standards (12 шт.): удалены секции «Связанные AR и ADR» / «Связанные документы».
- В `ARCHITECTURE.md` переписаны таблицы AR/ADR/standards под новую нумерацию; добавлен раздел «Формат документов» с правилами формата и плоского направленного графа ссылок (ADR→ADR; AR/standard→ADR через `Источник`; обратные ссылки запрещены).

## Предыдущее выполненное

Зафиксирован frontend-стек проекта (Solution Architect-сессия):

- ADR-0015 — Next.js (App Router, RSC) как frontend meta-framework.
- ADR-0016 — React 18+ и TypeScript `strict: true` как UI-стек.
- ADR-0017 — BFF как логически выделенный слой внутри Next.js-сервиса (не отдельный процесс).
- ADR-0018 — Tailwind CSS + shadcn/ui как система стилей и компонентов.
- ADR-0019 — Zod как единая библиотека валидации схем.
- AR-0011 (frontend) — Frontend и BFF только TypeScript / Node.js LTS; исключение из AR-0006.
- AR-0012 (frontend) — BFF не содержит бизнес-логики.
- AR-0013 (frontend) — BFF stateless (signed encrypted cookie).
- AR-0014 (frontend) — JWT не покидает BFF, в браузер уходит только httpOnly session cookie.
- AR-0015 (frontend) — UI и BFF упакованы в один процесс, граница на уровне кода и ESLint.
- Стандарт `frontend-project-structure.md` — структура `apps/web/` (app, components, lib/bff, tests).
- Стандарт `typescript-code-style.md` — `strict: true`, запрет `any`, ESLint/Prettier.
- Стандарт `frontend-testing.md` — Vitest + Testing Library + Playwright, coverage ≥ 80%.
- Обновлены `ARCHITECTURE.md`, `overview.md`, `components.md`, `techContext.md`, `repository-structure.md`.
- Зафиксированы NFR: LCP ≤ 2.5 s, TTI ≤ 3.0 s, initial JS ≤ 200 KB gzip, Lighthouse ≥ 80, BFF overhead ≤ 30 ms, агрегация ≤ 150 ms.
- Открытые вопросы: способ генерации TS-типов из OpenAPI (отложен), хранение/доставка фото рецептов, OpenAPI BFF↔браузер, точные мажорные версии Node.js/Next.js/React, стратегия CSRF.

## Предыдущее выполненное

Приведение репозитория в соответствие с AR-0010:

- Создан `/.editorconfig` в корне репозитория.
- Секции: `[*]` (UTF-8, LF, trim, final newline), `[*.cs]` (4 пробела), `[*.md]` (2 пробела, trim=false), `[*.yml]`/`[*.yaml]` (2 пробела), `[*.json]` (2 пробела).

## Предыдущее выполненное

Зафиксирован backend-стек проекта:

- ADR-0011 — .NET 10 / C# как платформа и язык backend.
- ADR-0012 — Domain-Driven Design.
- ADR-0013 — Гексагональная архитектура (Ports & Adapters).
- ADR-0014 — EF Core как ORM.
- AR-0006 — Backend только .NET 10 / C#.
- AR-0007 — Внешние интеграции через порты и адаптеры.
- AR-0008 — Доменная логика через DDD-паттерны.
- AR-0009 — Видимость типов internal по умолчанию.
- AR-0010 (general) — `.editorconfig` обязателен, един в корне репо.
- Стандарт `dotnet-project-structure.md` — размещение `apps/<BoundedContext>/{src,tests}`, структура папок сервиса (`Domain/Application/Adapters/Infrastructure/Program.cs`).
- Стандарт `csharp-code-style.md` — минимальные правила C#.
- Обновлены `ARCHITECTURE.md`, `techContext.md`, `repository-structure.md`.
- Отложено: AR про направления зависимостей между слоями; ADR/стандарт по тестированию backend.

## Предыдущее выполненное

Создан воркфлоу `docs/contributing/architecture-workflow.md` («Проектирование»):

- Шаги: выбор Opus → выполнение промта → итерации → фиксация в ADR/AR/diagrams/standards/overview/components.
- Промт Solution Architect: итеративный сбор требований по 7 блокам, измеримые NFR, 2–4 различающихся варианта, без кода.
- Требования: без отдельной ветки, без реализации.
- Ссылка добавлена в `CONTRIBUTING.md` (раздел «Типовые задачи»).

## Предыдущее выполненное

Введение API Gateway в архитектуру (Solution Architect-сессия):

- ADR-0008 — API Gateway как единая точка доступа (без технологий).
- ADR-0009 — Swagger UI публикуется на API Gateway (без технологий и способа сборки).
- ADR-0010 — YARP как реализация API Gateway.
- AR-0003 — Frontend взаимодействует с backend только через API Gateway.
- AR-0004 — Backend-сервисы не публикуются наружу напрямую.
- AR-0005 — API Gateway не содержит бизнес-логики.
- Обновлены `ARCHITECTURE.md`, `docs/architecture/overview.md`, `docs/architecture/components.md`.
- Шаблон ADR (`.clinerules/skills/adr/assets/adr-template.md`) дополнен разделом «Связанные документы»; существующие ADR (0001–0007) приведены к новому стилю.
- Открытый вопрос: способ формирования содержимого Swagger UI на gateway (единый документ vs агрегация спецификаций backend) — отложено.

## Предыдущее выполненное

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
