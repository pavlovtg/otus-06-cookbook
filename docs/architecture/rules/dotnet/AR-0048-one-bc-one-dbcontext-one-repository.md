# AR-0048: 1 bounded context → 1 DbContext → 1 репозиторий

Источник: ADR-0012, ADR-0014

## Правило

Каждый bounded context имеет ровно один `DbContext` и ровно один репозиторий. `<BoundedContext>Repository` наследует `DbContext`, являясь одновременно репозиторием и контекстом БД.

## Запрещено

- Использовать один `DbContext` для нескольких bounded context.
- Создавать несколько репозиториев для одного bounded context.
- Выносить `DbContext` в отдельный класс, не связанный с репозиторием.
