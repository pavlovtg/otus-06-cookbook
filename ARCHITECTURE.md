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
| ADR-0005: JWT-аутентификация | Аутентификация пользователей через JWT-токены | [ADR-0005](docs/adr/general/ADR-0005-jwt-authentication.md) | general | принят |
| ADR-0006: REST API как протокол взаимодействия | Взаимодействие frontend и backend через REST API с OpenAPI | [ADR-0006](docs/adr/general/ADR-0006-rest-api.md) | general | принят |
| ADR-0007: Docker Compose как среда развёртывания | Локальный запуск всего приложения через Docker Compose | [ADR-0007](docs/adr/general/ADR-0007-docker-compose.md) | general | принят |
