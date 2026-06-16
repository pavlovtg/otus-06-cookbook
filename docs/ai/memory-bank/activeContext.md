# Активный контекст

## Текущая задача

Рефакторинг тестов Testcontainers — ускорение интеграционных и микросервисных тестов.

## Что сделано

### Shared.Testing

- `PostgresContainerFixture` — базовая фикстура с `WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr())`
- `RepositoryFactory<T>` — фабрика контекстов с `MigrateAsync()`

### Recipes.Tests / Microservice

- `RecipeMicroserviceFixture` — один контейнер + один `WebApplicationFactory` на всю коллекцию
  - `TruncateAsync()` — динамический TRUNCATE всех таблиц схемы `cookbook` (кроме `__EFMigrationsHistory`)
- `RecipeMicroserviceCollection` — `[CollectionDefinition("RecipeMicroservice")]`
- 6 тестовых классов переведены на `[Collection("RecipeMicroservice")]` + `IAsyncLifetime` с `TruncateAsync`

### Recipes.Tests / Integration

- `RecipeIntegrationFixture` — один контейнер + одна миграция на всю коллекцию + `TruncateAsync()`
- `RecipeIntegrationCollection` — `[CollectionDefinition("RecipeIntegration")]`
- `RecipeMigrationFixture` — свежий контейнер на каждый тестовый класс (для тестов миграций)
- `RecipeMigrationTests` — тесты схемы/таблиц + write-read через разные контексты
- 4 тестовых класса репозиториев: `CategoryRepositoryTests`, `IngredientRepositoryTests`, `RecipeRepositoryTests`, `RecipePhotoRepositoryTests`

### Shared.Testing.Tests

- `RepositoryFactoryTests` — обновлён: `WithWaitStrategy` → `WithOutputConsumer`, добавлен write-read тест

## Ключевые решения

- **Изоляция**: `TRUNCATE ... RESTART IDENTITY CASCADE` в `InitializeAsync` каждого тестового класса
- **Динамический список таблиц**: `information_schema.tables WHERE table_schema = 'cookbook' AND table_name != '__EFMigrationsHistory'`
- **Тесты миграций**: отдельная фикстура `RecipeMigrationFixture` (контейнер поднимается заново для каждого класса)
- **Ускорение**: 1 контейнер вместо N на всю коллекцию → экономия ~30-60 сек на старте
