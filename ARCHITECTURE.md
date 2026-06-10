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
   - есть ссылка `Источник: ADR-NNNN` в AR или стандарте,
   - нужно понять обоснование решения.

## Архитектурные правила (AR)

| Название | Описание для агента | Файл | Домен |
|----------|---------------------|------|-------|
| AR-0001: Docker Compose Self-Contained | Приложение запускается одной командой `docker compose up` без ручной настройки | [AR-0001](docs/architecture/rules/general/AR-0001-docker-compose-self-contained.md) | general |
| AR-0002: Swagger UI доступен по живому URL | Swagger UI обязан быть доступен в запущенном приложении, статический файл не считается | [AR-0002](docs/architecture/rules/general/AR-0002-swagger-ui-live.md) | general |
| AR-0003: `.editorconfig` обязателен | Единый `.editorconfig` в корне репозитория для всех стеков | [AR-0003](docs/architecture/rules/general/AR-0003-editorconfig-mandatory.md) | general |
| AR-0004: API Gateway boundary | Backend доступен только через gateway; gateway без бизнес-логики; backend наружу не публикуется | [AR-0004](docs/architecture/rules/rest-api/AR-0004-api-gateway-boundary.md) | rest-api |
| AR-0005: Backend — только .NET 10 / C# | Все backend-сервисы и утилиты реализуются на .NET 10 / C# | [AR-0005](docs/architecture/rules/dotnet/AR-0005-backend-dotnet-csharp.md) | dotnet |
| AR-0006: Внешние интеграции — через порты и адаптеры | Доступ к БД, HTTP, очередям — только через порты в Application и адаптеры | [AR-0006](docs/architecture/rules/dotnet/AR-0006-ports-and-adapters.md) | dotnet |
| AR-0007: Доменная логика — через DDD-паттерны | Агрегаты, VO, доменные события; запрет анемичной модели | [AR-0007](docs/architecture/rules/backend/AR-0007-ddd-domain-logic.md) | backend |
| AR-0008: Видимость типов — internal по умолчанию | Public только у публичного API библиотек; сервисы — всё internal | [AR-0008](docs/architecture/rules/dotnet/AR-0008-type-visibility-internal.md) | dotnet |
| AR-0009: Frontend и BFF — TypeScript / Node.js | Frontend и BFF только на TypeScript / Node.js LTS; исключение из AR-0005 | [AR-0009](docs/architecture/rules/frontend/AR-0009-frontend-typescript-nodejs.md) | frontend |
| AR-0010: BFF boundary | BFF stateless, без бизнес-логики, без БД; JWT не покидает сервер; UI и BFF — один Next.js-процесс | [AR-0010](docs/architecture/rules/frontend/AR-0010-bff-boundary.md) | frontend |
| AR-0011: Edge reverse proxy обязателен | Весь внешний трафик через nginx; Next.js и YARP не публикуются наружу | [AR-0011](docs/architecture/rules/general/AR-0011-edge-reverse-proxy-mandatory.md) | general |
| AR-0012: Auth-service — единственный issuer JWT | Все JWT выпускает только auth-service; доменные сервисы и BFF — не выпускают | [AR-0012](docs/architecture/rules/rest-api/AR-0012-auth-service-sole-issuer.md) | rest-api |
| AR-0013: JWT-валидация обязательна на downstream | Каждый backend за gateway повторно валидирует JWT (sig, iss, aud, exp) | [AR-0013](docs/architecture/rules/rest-api/AR-0013-jwt-validation-on-downstream.md) | rest-api |
| AR-0014: Ошибки REST API — Problem+JSON | Все 4xx/5xx сериализуются по RFC 7807 с `application/problem+json` | [AR-0014](docs/architecture/rules/rest-api/AR-0014-error-responses-problem-json.md) | rest-api |
| AR-0015: Contract-First — OpenAPI до реализации | Любой API описывается в OpenAPI YAML (≥ 3.0.0) до написания кода; файл в `docs/contracts/<bc>/<service>.yaml` | [AR-0015](docs/architecture/rules/rest-api/AR-0015-contract-first-openapi.md) | rest-api |
| AR-0016: Соглашение о маршрутизации на API Gateway | Маршруты gateway: `/api/{bounded-context}/...` → сервис; gateway стрипает bounded-context-префикс | [AR-0016](docs/architecture/rules/rest-api/AR-0016-gateway-routing-convention.md) | rest-api |
| AR-0017: Database-per-Service — изоляция данных | Каждый сервис владеет собственной БД; прямой доступ к чужой БД запрещён; именование БД и схем — lowercase snake_case | [AR-0017](docs/architecture/rules/database/AR-0017-database-per-service.md) | database |
| AR-0018: Контроллеры вместо Minimal API | HTTP-эндпоинты в .NET реализуются через `ApiController`, Minimal API запрещён | [AR-0018](docs/architecture/rules/dotnet/AR-0018-controllers-over-minimal-api.md) | dotnet |
| AR-0019: Идентификаторы агрегатов — типизированные Value Objects | Идентификаторы агрегатов — `readonly record struct` с `From(Guid)`, валидацией и `ToString("D")` | [AR-0019](docs/architecture/rules/dotnet/AR-0019-ddd-value-object-ids.md) | dotnet |
| AR-0020: Streaming через IAsyncEnumerable вместо буферизации | Репозитории и сервисы возвращают `IAsyncEnumerable<T>`; буферизация только в трёх исключительных случаях | [AR-0020](docs/architecture/rules/dotnet/AR-0020-async-enumerable-streaming.md) | dotnet |
| AR-0021: Изоляция адаптеров — адаптеры не зависят друг от друга | Web-адаптер и Postgresql-адаптер зависят только от Application и Domain; прямые зависимости между адаптерами запрещены | [AR-0021](docs/architecture/rules/dotnet/AR-0021-adapter-layer-isolation.md) | dotnet |
| AR-0022: Internal-контроллеры регистрируются через ControllerFeatureProvider | Контроллеры остаются `internal`; регистрируются через кастомный `ControllerFeatureProvider` в `ApplicationPartManager` | [AR-0022](docs/architecture/rules/dotnet/AR-0022-internal-controllers-feature-provider.md) | dotnet |
| AR-0023: Запрет проверок environment и провайдера в production-коде | `MigrateAsync()` всегда без условий; InMemory запрещён в тестах; тесты используют Testcontainers | [AR-0023](docs/architecture/rules/dotnet/AR-0023-no-environment-checks-in-production.md) | dotnet |
| AR-0024: HTTP-зависимости в тестах мокируются через WireMock.Net | Интеграционные тесты .NET-сервисов используют WireMock.Net; адрес переопределяется через `AddInMemoryCollection`; тест верифицирует запрос к mock | [AR-0024](docs/architecture/rules/dotnet/AR-0024-wiremock-for-http-mocking.md) | dotnet |
| AR-0025: Каждый микросервис реализует `GET /api/health/v1` | Health-check эндпоинт обязателен; путь соответствует REST URI-шаблону; используется в `docker-compose.yml` для `depends_on: service_healthy` | [AR-0025](docs/architecture/rules/rest-api/AR-0025-health-check-endpoint.md) | rest-api |
| AR-0026: Сетевая изоляция в Docker Compose по принципу безопасности | `frontend-net` и `backend-net`; api-gateway в обеих сетях; только reverse-proxy публикует порты на хост | [AR-0026](docs/architecture/rules/general/AR-0026-docker-compose-network-isolation.md) | general |
| AR-0030: CI toolchain — фиксированный набор инструментов | Каждый стек использует строго определённый линтер и тест-раннер: dotnet format, next lint, ruff, markdownlint-cli2, Playwright | [AR-0030](docs/architecture/rules/general/AR-0030-ci-toolchain.md) | general |
| AR-0031: CI — стратегия триггеров | Push → умный запуск по затронутым компонентам; PR в main → полный прогон; e2e/ui-test только при изменении кода или compose | [AR-0031](docs/architecture/rules/general/AR-0031-ci-trigger-strategy.md) | general |
| AR-0032: Docker Compose — конфигурация через env-переменные | Все параметры docker-compose выносятся в `.env`; формат `${VAR:?ERR}`; `container_name` из `COMPOSE_NAME`; `ASPNETCORE_URLS` через `DEFAULT_HTTP_PORT`; `.env` коммитится | [AR-0032](docs/architecture/rules/general/AR-0032-docker-compose-env-config.md) | general |
| AR-0033: Параметризация nginx-конфига через nginx templates | nginx-конфиг параметризуется через `.template`-файл в `/etc/nginx/templates/`; nginx-образ прогоняет `envsubst` при старте | [AR-0033](docs/architecture/rules/general/AR-0033-nginx-templates-config.md) | general |

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
| Стандарт структуры frontend-проекта | Структура `apps/web` для Next.js + BFF, границы UI/BFF, конфигурация | [frontend-project-structure.md](docs/standards/frontend-project-structure.md) |
| Стандарт стиля TypeScript | `strict: true`, запрет `any`, naming, ESLint/Prettier правила | [typescript-code-style.md](docs/standards/typescript-code-style.md) |
| Стандарт тестирования frontend | Vitest + Testing Library + Playwright; целевое покрытие ≥ 80% | [frontend-testing.md](docs/standards/frontend-testing.md) |
| Реестр сервисов и bounded contexts | Канонические имена сервисов, bounded contexts, БД и схем проекта | [service-registry.md](docs/standards/service-registry.md) |

## ADR

| Название | Описание для агента | Файл | Домен |
|----------|---------------------|------|-------|
| ADR-0001: Markdown как единый формат документации | Markdown — единый формат документации для людей и AI | [ADR-0001](docs/adr/general/ADR-0001-markdown-documentation.md) | general |
| ADR-0002: Монорепозиторий | Все компоненты проекта в одном репозитории | [ADR-0002](docs/adr/general/ADR-0002-monorepo.md) | general |
| ADR-0003: Релизная политика и модель ветвления | Continuous Delivery + GitHub Flow | [ADR-0003](docs/adr/general/ADR-0003-release-and-branching.md) | general |
| ADR-0004: PostgreSQL как СУБД | PostgreSQL для хранения данных приложения | [ADR-0004](docs/adr/database/ADR-0004-postgresql.md) | database |
| ADR-0005: JWT-аутентификация | Аутентификация через JWT-токены | [ADR-0005](docs/adr/rest-api/ADR-0005-jwt-authentication.md) | rest-api |
| ADR-0006: REST API как протокол | REST + OpenAPI/Swagger для взаимодействия frontend и backend | [ADR-0006](docs/adr/rest-api/ADR-0006-rest-api.md) | rest-api |
| ADR-0007: Docker Compose как среда развёртывания | Локальный запуск всего приложения через Docker Compose | [ADR-0007](docs/adr/general/ADR-0007-docker-compose.md) | general |
| ADR-0008: API Gateway как единая точка доступа | Единая точка доступа для клиентов; backend изолирован | [ADR-0008](docs/adr/rest-api/ADR-0008-api-gateway-single-entry-point.md) | rest-api |
| ADR-0009: Swagger UI публикуется на API Gateway | Каноничный источник OpenAPI — на gateway | [ADR-0009](docs/adr/rest-api/ADR-0009-swagger-ui-on-api-gateway.md) | rest-api |
| ADR-0010: YARP как реализация API Gateway | YARP/ASP.NET Core как технология gateway | [ADR-0010](docs/adr/rest-api/ADR-0010-yarp-as-api-gateway.md) | rest-api |
| ADR-0011: .NET 10 / C# как платформа backend | Платформа и язык реализации backend | [ADR-0011](docs/adr/dotnet/ADR-0011-dotnet-csharp-backend.md) | dotnet |
| ADR-0012: Domain-Driven Design | Подход к моделированию доменной логики | [ADR-0012](docs/adr/backend/ADR-0012-domain-driven-design.md) | backend |
| ADR-0013: Гексагональная архитектура | Архитектура backend (Ports & Adapters) | [ADR-0013](docs/adr/backend/ADR-0013-hexagonal-architecture.md) | backend |
| ADR-0014: EF Core как ORM | ORM для backend поверх PostgreSQL | [ADR-0014](docs/adr/dotnet/ADR-0014-ef-core-orm.md) | dotnet |
| ADR-0015: Next.js как frontend meta-framework | Next.js App Router + RSC как стек frontend и хост BFF | [ADR-0015](docs/adr/frontend/ADR-0015-nextjs-frontend-meta-framework.md) | frontend |
| ADR-0016: React + TypeScript как UI-стек | React + TypeScript strict как UI-стек | [ADR-0016](docs/adr/frontend/ADR-0016-react-typescript-frontend.md) | frontend |
| ADR-0017: BFF как логический серверный слой | BFF — модули внутри Next.js, не отдельный процесс | [ADR-0017](docs/adr/frontend/ADR-0017-bff-logical-layer.md) | frontend |
| ADR-0018: Tailwind CSS + shadcn/ui | Система стилей и базовых компонентов UI | [ADR-0018](docs/adr/frontend/ADR-0018-tailwind-shadcn.md) | frontend |
| ADR-0019: Zod как валидация схем | Единая библиотека валидации и источник TS-типов | [ADR-0019](docs/adr/frontend/ADR-0019-zod-schema-validation.md) | frontend |
| ADR-0020: Nginx как edge reverse proxy | Edge reverse proxy перед Next.js и YARP с кэшем статики | [ADR-0020](docs/adr/general/ADR-0020-nginx-as-edge-reverse-proxy.md) | general |
| ADR-0021: Auth-service как отдельный сервис | JWT выпускается выделенным сервисом auth-service за YARP | [ADR-0021](docs/adr/rest-api/ADR-0021-dedicated-auth-service.md) | rest-api |
| ADR-0022: S2S через OAuth 2.0 client_credentials | Сервис-клиенты получают JWT через client_credentials у auth-service | [ADR-0022](docs/adr/rest-api/ADR-0022-s2s-client-credentials.md) | rest-api |
| ADR-0023: Contract-First подход к проектированию API | OpenAPI YAML (≥ 3.0.0) создаётся до реализации; файлы в `docs/contracts/<bc>/<service>.yaml` | [ADR-0023](docs/adr/rest-api/ADR-0023-contract-first-openapi.md) | rest-api |
| ADR-0024: Соглашение о маршрутизации на API Gateway | Маршруты gateway: `/api/{bounded-context}/...` → сервис; gateway стрипает bounded-context-префикс | [ADR-0024](docs/adr/rest-api/ADR-0024-gateway-routing-convention.md) | rest-api |
| ADR-0025: Database-per-Service — изоляция данных по сервисам | Каждый сервис владеет собственной БД; доступ к чужой БД запрещён; именование БД и схем — lowercase snake_case | [ADR-0025](docs/adr/database/ADR-0025-database-per-service.md) | database |
| ADR-0026: Соглашение об именовании сервисов и bounded contexts | Все имена сервисов и bounded contexts — lowercase kebab-case; реестр в `service-registry.md` | [ADR-0026](docs/adr/general/ADR-0026-service-naming-convention.md) | general |
| ADR-0027: WireMock.Net для мокирования HTTP-зависимостей в тестах | HTTP-зависимости в интеграционных тестах .NET мокируются через WireMock.Net; адрес переопределяется через конфигурацию | [ADR-0027](docs/adr/dotnet/ADR-0027-wiremock-for-http-mocking.md) | dotnet |
| ADR-0028: Сетевая изоляция сервисов в Docker Compose | Сервисы разбиваются на `frontend-net` и `backend-net` по принципу безопасности; api-gateway в обеих сетях | [ADR-0028](docs/adr/general/ADR-0028-docker-compose-network-isolation.md) | general |
| ADR-0029: Health-check API обязателен для каждого микросервиса | Каждый микросервис реализует `GET /api/health/v1`; путь соответствует REST URI-шаблону | [ADR-0029](docs/adr/general/ADR-0029-health-check-api.md) | general |
| ADR-0030: Стратегия CI | Два режима CI: push → умный запуск по затронутым компонентам; PR в main → полный прогон с блокировкой merge | [ADR-0030](docs/adr/general/ADR-0030-ci-strategy.md) | general |

## Диаграммы

| Название | Описание для агента | Файл | Тип C4 | Источник |
|----------|---------------------|------|--------|----------|
| C4 Context — Cookbook | Системный контекст: единственный внешний актор — пользователь в браузере; внешних систем нет | [c4-context.md](docs/architecture/diagrams/c4-context.md) | L1 Context | ADR-0008 |
| C4 Containers — Cookbook | Контейнеры Docker Compose: nginx → Next.js (UI+BFF) / YARP → auth-service, доменные сервисы; у каждого сервиса своя PG | [c4-containers.md](docs/architecture/diagrams/c4-containers.md) | L2 Container | ADR-0007, ADR-0008, ADR-0010, ADR-0015, ADR-0017, ADR-0020, ADR-0021 |
| C4 Component — Frontend (web) | Внутреннее устройство Next.js: граница client UI ↔ серверный BFF, JWT хранится server-side | [c4-component-frontend.md](docs/architecture/diagrams/c4-component-frontend.md) | L3 Component | ADR-0015, ADR-0017, AR-0010 |
| C4 Component — Backend | Гексагональная архитектура доменного сервиса: Domain / Application / Adapters; auth — инфраструктурный cross-cutting слой | [c4-component-backend.md](docs/architecture/diagrams/c4-component-backend.md) | L3 Component | ADR-0011, ADR-0012, ADR-0013, ADR-0014, AR-0006, AR-0007, AR-0013 |
| C4 Component — auth-service | Устройство единственного issuer JWT: /auth/login, /auth/token, Identity domain, JWT issuer | [c4-component-auth-service.md](docs/architecture/diagrams/c4-component-auth-service.md) | L3 Component | ADR-0021, ADR-0022, AR-0012 |
| Flow — User login + защищённый запрос | Логин через BFF и последующий вызов защищённого ресурса; JWT не покидает сервер, в браузер только httpOnly cookie | [flow-user-login.md](docs/architecture/diagrams/flow-user-login.md) | Dynamic | ADR-0005, ADR-0017, ADR-0021, AR-0010, AR-0012, AR-0013 |
| Flow — S2S client_credentials | Межсервисный вызов: service-A получает JWT у auth-service по client_credentials и зовёт service-B напрямую внутри Compose-сети | [flow-s2s-client-credentials.md](docs/architecture/diagrams/flow-s2s-client-credentials.md) | Dynamic | ADR-0022, AR-0012, AR-0013 |

## Формат документов

Документы AR/ADR/standard оптимизированы под загрузку агентом: минимум перекрёстных ссылок, описание для агента ведётся ТОЛЬКО в таблицах выше.

- **ADR**: шапка (Статус, Дата) + секции `Контекст` / `Решение` / `Последствия`. Альтернативы — одной строкой внутри `Контекст`. Immutable.
- **AR**: секции `Правило` / (опц.) `Запрещено`. Опц. строка `Источник: ADR-NNNN[, ADR-MMMM]` сразу после заголовка.
- **Standard**: секция `Правила`. Опц. строка `Источник: ADR-NNNN`.
- **Diagram**: опц. строка `Источник: ADR-NNNN[, AR-MMMM]` + `## Описание` + блок PlantUML. Без `## Тип` и `## Связанные компоненты`.
- **Домен** документа = путь файла (`backend/`, `frontend/`, `rest-api/`, `general/`); внутри файла не дублируется.
- **Граф ссылок плоский и направленный**: ADR → ADR (только в `Контекст` или поле «Заменяет»); AR/standard/diagram → ADR/AR (через `Источник`). Обратные ссылки запрещены.
