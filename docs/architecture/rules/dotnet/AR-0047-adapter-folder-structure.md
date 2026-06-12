# AR-0047: Структура папок адаптера БД

Источник: ADR-0013, ADR-0014

## Правило

Репозитории располагаются в `Adapters/<Database>/`. Если в адаптере несколько bounded context — для каждого создаётся подпапка с именем bounded context; если bounded context один — подпапка не создаётся. Внутри папки контекста (или `Adapters/<Database>/` при одном контексте):

- `Configurations/` — классы `IEntityTypeConfiguration`
- `Migrations/` — миграции EF Core
- `<BoundedContext>Repository.cs` — реализация репозитория
