# Стандарт: структура .NET-проектов backend

## Назначение и область применения

Описывает структуру .NET-решений и проектов backend-сервисов в монорепо.
Применяется ко всем backend-сервисам и backend-утилитам, реализуемым на .NET 10 / C#.

## Размещение в репозитории

```text
apps/
└── <BoundedContext>/
    ├── <BoundedContext>.slnx
    ├── src/
    │   ├── <Service>/
    │   │   └── <Service>.csproj
    │   └── <AnotherService>/
    │       └── <AnotherService>.csproj
    └── tests/
        ├── <Service>.Tests/
        │   └── <Service>.Tests.csproj
        └── <AnotherService>.Tests/
            └── <AnotherService>.Tests.csproj
```

- `<BoundedContext>` — bounded context (в терминах DDD), группирует связанные сервисы. Имя в PascalCase.
- `<Service>` — отдельный сервис или утилита внутри bounded context. Имя в PascalCase.
- `<BoundedContext>.slnx` — solution-файл нового формата, содержит все проекты bounded context (и `src/`, и `tests/`).
- Исходники сервисов — только под `src/`. Тесты — только под `tests/`.

## Один проект — один сервис

Каждый сервис — **один** `*.csproj`. Слои гексагональной архитектуры выражаются папками внутри проекта, а не отдельными проектами.

```text
src/<Service>/
├── <Service>.csproj
├── Program.cs
├── Domain/
├── Application/
├── Adapters/
│   ├── Web/
│   └── Postgresql/
└── Infrastructure/
```

| Папка | Назначение |
|-------|------------|
| `Domain/` | Доменная модель: агрегаты, сущности, value objects, доменные события, доменные сервисы. Не зависит от внешних технологий. |
| `Application/` | Сценарии (use cases), команды/запросы, порты (интерфейсы для внешнего мира). |
| `Adapters/Web/` | Входящий HTTP-адаптер: контроллеры/minimal API, DTO, mapping. |
| `Adapters/Postgresql/` | Исходящий адаптер persistence: EF Core `DbContext`, `IEntityTypeConfiguration<T>`, миграции, реализации репозиториев. |
| `Infrastructure/` | Технические утилиты: логирование, конфигурация, time provider, общие DI-extensions. Без бизнес-логики. |
| `Program.cs` | Composition root: регистрация DI, конфигурация хоста, сборка всех слоёв. |

Namespace должен соответствовать пути папок: `<Service>.Domain.*`, `<Service>.Application.*`, `<Service>.Adapters.Web.*` и т. д.

## Тесты

Тестовые проекты размещаются под `apps/<BoundedContext>/tests/<Service>.Tests/<Service>.Tests.csproj`.

## Связанные документы

- [ADR-0011: .NET 10 / C# как платформа backend](../adr/backend/ADR-0011-dotnet-csharp-backend.md)
- [ADR-0013: Гексагональная архитектура](../adr/backend/ADR-0013-hexagonal-architecture.md)
- [AR-0007: Внешние интеграции — через порты и адаптеры](../architecture/rules/backend/AR-0007-ports-and-adapters.md)
- [AR-0009: Видимость типов — internal по умолчанию](../architecture/rules/backend/AR-0009-type-visibility-internal.md)
- [Стандарт структуры репозитория](repository-structure.md)
