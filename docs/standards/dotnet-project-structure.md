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

### Размещение файлов тестов

**Unit-тесты** зеркалируют путь тестируемого класса внутри тестовой сборки:

```text
src/<Service>/Domain/Recipe.cs
→ tests/<Service>.Tests/Domain/RecipeTests.cs

src/<Service>/Application/RecipeService.cs
→ tests/<Service>.Tests/Application/RecipeServiceTests.cs
```

**Интеграционные тесты адаптеров** зеркалируют путь адаптера внутри тестовой сборки:

```text
src/<Service>/Adapters/Postgresql/RecipeRepository.cs
→ tests/<Service>.Tests/Adapters/Postgresql/RecipeRepositoryTests.cs

src/<Service>/Adapters/Web/RecipesController.cs
→ tests/<Service>.Tests/Adapters/Web/RecipesControllerTests.cs
```
