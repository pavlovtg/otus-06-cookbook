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
| AR-0006: Backend — только .NET 10 / C# | Все backend-сервисы и утилиты реализуются на .NET 10 / C# | [AR-0006](docs/architecture/rules/backend/AR-0006-backend-dotnet-csharp.md) | backend |
| AR-0007: Внешние интеграции — через порты и адаптеры | Доступ к БД, HTTP, очередям — только через порты в Application и адаптеры | [AR-0007](docs/architecture/rules/backend/AR-0007-ports-and-adapters.md) | backend |
| AR-0008: Доменная логика — через DDD-паттерны | Агрегаты, VO, доменные события; запрет анемичной модели | [AR-0008](docs/architecture/rules/backend/AR-0008-ddd-domain-logic.md) | backend |
| AR-0009: Видимость типов — internal по умолчанию | Public только у публичного API библиотек; сервисы — всё internal | [AR-0009](docs/architecture/rules/backend/AR-0009-type-visibility-internal.md) | backend |
| AR-0010: .editorconfig — обязательный источник кодстайла | Единый `.editorconfig` в корне репозитория для всех стеков | [AR-0010](docs/architecture/rules/general/AR-0010-editorconfig-mandatory.md) | general |

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
| Стандарт структуры .NET-проектов | Размещение .NET-решений и проектов backend в монорепо, структура папок сервиса | [dotnet-project-structure.md](docs/standards/dotnet-project-structure.md) |
| Стандарт стиля C#-кода | Минимальные правила оформления C#: отступы, nullable, namespace, format в CI | [csharp-code-style.md](docs/standards/csharp-code-style.md) |

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
| ADR-0011: .NET 10 / C# как платформа backend | Платформа и язык реализации backend-сервисов | [ADR-0011](docs/adr/backend/ADR-0011-dotnet-csharp-backend.md) | backend | принят |
| ADR-0012: Domain-Driven Design | Подход к моделированию доменной логики backend | [ADR-0012](docs/adr/backend/ADR-0012-domain-driven-design.md) | backend | принят |
| ADR-0013: Гексагональная архитектура | Архитектура backend-сервисов (Ports & Adapters) | [ADR-0013](docs/adr/backend/ADR-0013-hexagonal-architecture.md) | backend | принят |
| ADR-0014: EF Core как ORM | ORM для backend-сервисов поверх PostgreSQL | [ADR-0014](docs/adr/backend/ADR-0014-ef-core-orm.md) | backend | принят |
