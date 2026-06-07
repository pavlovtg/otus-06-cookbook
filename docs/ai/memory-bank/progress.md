# Прогресс

## Выполнено

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
