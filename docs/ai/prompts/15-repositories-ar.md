# Промт: Зафиксировать AR для репозиториев .NET/DDD

## Роль

- Архитектурный агент, создающий Architecture Rules (AR) для .NET/DDD проектов.

## Контекст

- Проект использует .NET, C#, EF Core, DDD, Hexagonal Architecture.
- Существующие ADR: ADR-0014 (EF Core как ORM), ADR-0032 (управление транзакциями из Application).
- AR-0037 уже зафиксирован: репозитории реализуют `IUnitOfWorkRepository`, коммит — из Application.
- Для создания AR использовать skill `ar`.

## Задача

Выполнить два шага:

### Шаг 1. Реализовать хелпер миграции в Shared.Hosting

Создать статический extension-класс `HostMigrationExtensions` в проекте `Shared.Hosting` с методом:

```csharp
public static async Task<IHost> MigrateDatabaseAsync<TProgram, TContext>(this IHost host)
    where TContext : DbContext
{
    await using var scope = host.Services.CreateAsyncScope();
    var context = scope.ServiceProvider.GetRequiredService<TContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<TProgram>>();
    logger.LogInformation("Migrating database...");
    await context.Database.MigrateAsync();
    logger.LogInformation("Database migrated");
    return host;
}
```

### Шаг 2. Создать набор AR

Создать AR, покрывающие требования к репозиториям (см. раздел «Требования»).
Каждое логически самостоятельное требование — отдельный AR.
Использовать skill `ar` для каждого AR.

## Требования

- Формат AR: секции `Правило` / (опц.) `Запрещено`; опц. строка `Источник: ADR-NNNN`.
- Покрыть следующие требования:

### 1. Структура папок адаптера

- Репозитории располагаются в `Adapters/<Database>/`.
- Если в адаптере несколько bounded context — для каждого создаётся подпапка с именем bounded context.
- Если bounded context один — подпапка не создаётся.
- Внутри папки контекста (или `Adapters/<Database>/` если контекст один):
  - `/Configurations` — классы `IEntityTypeConfiguration`
  - `/Migrations` — миграции EF Core
  - `<BoundedContext>Repository.cs` — реализация репозитория

### 2. Соответствие bounded context / DbContext / Repository

- 1 bounded context → 1 `DbContext` → 1 репозиторий.
- `<BoundedContext>Repository` наследует `DbContext` (является одновременно репозиторием и контекстом БД).

### 3. Управление транзакциями

- Уже зафиксировано в AR-0037. Новый AR не создавать, только сослаться при необходимости.

### 4. Асинхронные интерфейсы с CancellationToken

- Все методы интерфейсов репозиториев — асинхронные.
- Каждый метод принимает `CancellationToken`, который отменяет операцию.

### 5. Возвращаемые коллекции

- Методы репозиториев возвращают `IEnumerable<T>` (или `IAsyncEnumerable<T>`), а не материализованные списки (`List<T>`, `Array`).
- Материализация допустима только если необходима для реализации.

### 6. Конфигурация модели через IEntityTypeConfiguration

- Конфигурация модели — только через `modelBuilder.ApplyConfiguration(new XConfiguration())`.
- Для каждой сущности — отдельный класс `IEntityTypeConfiguration` в папке `Configurations`.
- Запрещено конфигурировать модель inline в `OnModelCreating`.

### 7. Миграции в папке Migrations

- Все миграции EF Core располагаются в папке `Migrations` внутри папки адаптера.

### 8. Запрет HasData

- Запрещено использовать `HasData` для seed-данных — нестабилен и вызывает ошибки при миграциях.

### 9. Загрузка начальных данных (Seeding)

- Загрузка данных — отдельный шаг после применения миграций.
- Реализуется через специальный класс-загрузчик по принципу Upsert: сравнивает существующие данные с загружаемыми, затем выполняет upsert.
- Предпочтительно использовать API репозитория; допустимо выполнять SQL-скрипт (для больших объёмов данных).

### 10. Миграция схемы при старте приложения

- Миграция схемы выполняется на этапе конфигурирования приложения, до поднятия эндпоинтов.
- Для запуска миграции использовать хелпер `MigrateDatabaseAsync<TProgram, TContext>` из `Shared.Hosting` (реализован в Шаге 1).

### 11. Тестирование репозитория

- В каждом тесте — новый инстанс `DbContext`.
- Если тест выполняет запись, а затем читает результат через репозиторий — это обязаны быть разные инстансы `DbContext` (кеши контекста мешают проверить реальную запись в БД).
- Подготовка БД в тесте также использует отдельный инстанс `DbContext`.
