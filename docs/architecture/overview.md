# Обзор архитектуры

## Назначение системы

«Книга рецептов» — full-stack приложение для хранения, поиска и планирования рецептов.
Позволяет домашним поварам хранить рецепты в одном месте, планировать меню на неделю и автоматически формировать список покупок.

## Ключевые технологии

- **PostgreSQL** — основная СУБД ([ADR-0004](../adr/general/ADR-0004-postgresql.md))
- **JWT** — аутентификация и авторизация ([ADR-0005](../adr/rest-api/ADR-0005-jwt-authentication.md))
- **REST API + OpenAPI/Swagger** — протокол взаимодействия frontend–backend ([ADR-0006](../adr/rest-api/ADR-0006-rest-api.md))
- **API Gateway** — единая точка доступа frontend и внешних клиентов к backend ([ADR-0008](../adr/rest-api/ADR-0008-api-gateway-single-entry-point.md))
- **YARP** — реализация API Gateway ([ADR-0010](../adr/rest-api/ADR-0010-yarp-as-api-gateway.md))
- **Docker Compose** — среда развёртывания ([ADR-0007](../adr/general/ADR-0007-docker-compose.md))
- **GitHub Actions** — CI pipeline

## Ключевые принципы

- Требования и поведение системы описываются в OpenSpec (`openspec/specs/`).
- Архитектурные решения фиксируются в ADR (`docs/adr/`).
- Архитектурные ограничения описываются в AR (`docs/architecture/rules/`).
- Вся документация ведётся в Markdown (см. [ADR-0001](../adr/general/ADR-0001-markdown-documentation.md)).

## Структура документации

```
ARCHITECTURE.md              # единая точка входа
docs/
├── adr/                     # архитектурные решения
├── architecture/
│   ├── overview.md          # этот файл
│   ├── components.md        # модули и зависимости
│   ├── diagrams/            # диаграммы
│   └── rules/               # архитектурные правила (AR)
└── standards/               # стандарты реализации
openspec/                    # спецификации требований
```

## Связанные документы

- [Компоненты](components.md)
- [Архитектурные правила](rules/)
- [ADR](../adr/)
