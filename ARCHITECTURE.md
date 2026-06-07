# Архитектура

Учебный проект OTUS.

## Приоритет документов

При противоречиях между источниками:

1. Явный запрос пользователя
2. OpenSpec (`openspec/`)
3. Архитектурные правила (`docs/architecture/rules/`)
4. Стандарты (`docs/standards/`)
5. ADR (`docs/adr/`)
6. Существующий код
7. Предположения агента

## Политика загрузки контекста для AI-агентов

Агент НЕ читает всю документацию. Алгоритм:

1. Прочитать `ARCHITECTURE.md` (этот файл).
2. Определить тип задачи.
3. Загрузить только нужные AR и стандарты.
4. Читать ADR только если:
   - меняется архитектура,
   - есть ссылка из AR или стандарта,
   - нужно понять обоснование решения.

## Архитектурные правила (AR)

| Название | Описание для агента | Файл | Домен |
|----------|---------------------|------|-------|
| AR-0001: Docker Compose Self-Contained | Приложение запускается одной командой `docker compose up` без ручной настройки | [AR-0001](docs/architecture/rules/general/AR-0001-docker-compose-self-contained.md) | general |
| AR-0002: Swagger UI доступен по живому URL | Swagger UI обязан быть доступен в запущенном приложении, статический файл не считается | [AR-0002](docs/architecture/rules/general/AR-0002-swagger-ui-live.md) | general |
| AR-0003: Frontend взаимодействует с backend только через API Gateway | Прямые вызовы клиентов к backend в обход gateway запрещены | [AR-0003](docs/architecture/rules/rest-api/AR-0003-frontend-via-api-gateway.md) | rest-api |
| AR-0004: Backend-сервисы не публикуются наружу напрямую | Наружу публикуется только порт API Gateway; backend доступен только во внутренней сети | [AR-0004](docs/architecture/rules/rest-api/AR-0004-backend-not-exposed.md) | rest-api |
| AR-0005: API Gateway не содержит бизнес-логики | Gateway отвечает только за cross-cutting задачи, бизнес-логика — в backend-сервисах | [AR-0005](docs/architecture/rules/rest-api/AR-0005-gateway-no-business-logic.md) | rest-api |

## Стандарты

| Название | Описание для агента | Файл |
|----------|---------------------|------|
| Стандарт оформления Markdown | Правила форматирования Markdown-файлов: отступы, заголовки, списки, пустые строки | [markdown-code-style.md](docs/standards/markdown-code-style.md) |
| Стандарт структуры репозитория | Структура папок монорепозитория и назначение каждой папки | [repository-structure.md](docs/standards/repository-structure.md) |
| Стандарт ветвления и релизной политики | Правила работы с ветками по GitHub Flow: именование, PR, защита main | [branch-plan.md](docs/standards/branch-plan.md) |
| Стандарт REST API Design | Правила проектирования REST API и OpenAPI-спецификации | [api-design.md](docs/standards/api-design.md) |
| Стандарт Docker Compose | Правила оформления `docker-compose.yml`, переменных окружения и сервисов | [docker-compose-standard.md](docs/standards/docker-compose-standard.md) |
| Стандарт тестирования | Требования к unit/integration/e2e тестам и покрытию | [testing.md](docs/standards/testing.md) |
| Стандарт CI Pipeline | Обязательные шаги CI (lint, тесты, build) и блокировка PR при падении | [ci-standard.md](docs/standards/ci-standard.md) |

## ADR

| Название | Описание для агента | Файл | Домен | Статус |
|----------|---------------------|------|-------|--------|
| ADR-0001: Markdown как единый формат документации | Markdown выбран как единый формат документации для людей и AI-агентов | [ADR-0001](docs/adr/general/ADR-0001-markdown-documentation.md) | general | принят |
| ADR-0002: Монорепозиторий | Выбран монорепозиторий для единого управления всеми компонентами проекта | [ADR-0002](docs/adr/general/ADR-0002-monorepo.md) | general | принят |
| ADR-0003: Релизная политика и модель ветвления | Выбраны Continuous Delivery и GitHub Flow для управления изменениями | [ADR-0003](docs/adr/general/ADR-0003-release-and-branching.md) | general | принят |
| ADR-0004: PostgreSQL как СУБД | Выбрана PostgreSQL для хранения данных приложения | [ADR-0004](docs/adr/general/ADR-0004-postgresql.md) | general | принят |
| ADR-0005: JWT-аутентификация | Аутентификация пользователей через JWT-токены | [ADR-0005](docs/adr/rest-api/ADR-0005-jwt-authentication.md) | rest-api | принят |
| ADR-0006: REST API как протокол взаимодействия | Взаимодействие frontend и backend через REST API с OpenAPI | [ADR-0006](docs/adr/rest-api/ADR-0006-rest-api.md) | rest-api | принят |
| ADR-0007: Docker Compose как среда развёртывания | Локальный запуск всего приложения через Docker Compose | [ADR-0007](docs/adr/general/ADR-0007-docker-compose.md) | general | принят |
| ADR-0008: API Gateway как единая точка доступа | Единая точка доступа для frontend и внешних клиентов, изолирующая backend | [ADR-0008](docs/adr/rest-api/ADR-0008-api-gateway-single-entry-point.md) | rest-api | принят |
| ADR-0009: Swagger UI публикуется на API Gateway | Каноничный источник OpenAPI-документации для клиентов — на gateway | [ADR-0009](docs/adr/rest-api/ADR-0009-swagger-ui-on-api-gateway.md) | rest-api | принят |
| ADR-0010: YARP как реализация API Gateway | Выбор YARP как технологии реализации API Gateway | [ADR-0010](docs/adr/rest-api/ADR-0010-yarp-as-api-gateway.md) | rest-api | принят |
