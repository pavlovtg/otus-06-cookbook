# Прогресс

## Выполнено

- Дедупликация и компактизация ADR/AR/standards:
  - AR сжаты с 15 до 10 (слияния AR-0003+0004+0005 → AR-0004, AR-0012+0013+0014+0015 → AR-0010); перенумерованы без дыр.
  - ADR (19) приведены к формату «шапка + Контекст/Решение/Последствия»; альтернативы свёрнуты в строку; убраны блоки «Связанные документы» и поле «Домен».
  - Standards (12): удалены секции «Связанные».
  - `ARCHITECTURE.md`: переписаны таблицы AR/ADR/standards; добавлен раздел «Формат документов» (плоский направленный граф ссылок).
- Зафиксирован frontend-стек (Solution Architect-сессия):
  - ADR-0015: Next.js (App Router, RSC) как frontend meta-framework
  - ADR-0016: React + TypeScript (`strict: true`) как UI-стек
  - ADR-0017: BFF как логически выделенный слой внутри Next.js-сервиса
  - ADR-0018: Tailwind CSS + shadcn/ui
  - ADR-0019: Zod как валидация схем
  - AR-0011: Frontend и BFF — TypeScript / Node.js (исключение из AR-0006)
  - AR-0012: BFF не содержит бизнес-логики
  - AR-0013: BFF stateless (signed encrypted cookie)
  - AR-0014: JWT не покидает BFF
  - AR-0015: UI и BFF в одном процессе
  - Стандарты: `frontend-project-structure.md`, `typescript-code-style.md`, `frontend-testing.md`
  - Обновлены `ARCHITECTURE.md`, `overview.md`, `components.md`, `techContext.md`, `repository-structure.md`
- Зафиксирован backend-стек:
  - ADR-0011..0014, AR-0006..0009, AR-0010 (.editorconfig)
  - Стандарты: `dotnet-project-structure.md`, `csharp-code-style.md`
- Создан воркфлоу `docs/contributing/architecture-workflow.md` («Проектирование») с промтом Solution Architect; ссылка добавлена в `CONTRIBUTING.md`
- API Gateway введён в архитектуру:
  - ADR-0008: API Gateway как единая точка доступа
  - ADR-0009: Swagger UI публикуется на API Gateway
  - ADR-0010: YARP как реализация API Gateway
  - AR-0003: Frontend через API Gateway
  - AR-0004: Backend не публикуется наружу
  - AR-0005: Gateway без бизнес-логики
  - Обновлены `ARCHITECTURE.md`, `overview.md`, `components.md`
  - Шаблон ADR дополнен разделом «Связанные документы»; ADR-0001..0007 приведены к новому стилю
- ADR-0005 и ADR-0006 перемещены в домен `rest-api` (`docs/adr/rest-api/`); обновлены все перекрёстные ссылки
- Рефакторинг skills архитектурной документации: 4 симметричных skill (`adr`, `ar`, `standard`, `diagram`); удалены `update-architecture-from-*`; актуализированы индексы в `ARCHITECTURE.md`
- Добавлены правила-триггеры skills в `.clinerules/architecture.md` (ADR/AR/стандарт → соответствующий skill)
- Инициализирован memory bank
- Создан стандарт Markdown (`docs/standards/markdown-code-style.md`)
- Создана система архитектурной документации (ADR, AR, overview, components, standards)
- Переименован `architecture.md` → `ARCHITECTURE.md`, обновлены все ссылки
- Создана contributing-документация (`CONTRIBUTING.md`, `docs/contributing/`)
- Создан `REPORT.md` и workflow `add-report.md`
- Создан промт `08-adr-from-assigment.md`
- Выполнен промт `08-adr-from-assigment.md`:
  - ADR-0004 — PostgreSQL
  - ADR-0005 — JWT
  - ADR-0006 — REST API
  - ADR-0007 — Docker Compose
  - Стандарты: `api-design.md`, `docker-compose-standard.md`, `testing.md`, `ci-standard.md`
  - AR-0001: Docker Compose Self-Contained
  - AR-0002: Swagger UI Live
  - Обновлены `overview.md` и `components.md`

## В работе

-

## Запланировано

- Реализация кода приложения (backend, frontend, БД)
- OpenSpec спецификации для фич
- Решение по способу формирования содержимого Swagger UI на API Gateway (единый документ vs агрегация спецификаций backend)
