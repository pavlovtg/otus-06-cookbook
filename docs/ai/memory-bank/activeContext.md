# Активный контекст

## Текущая задача

**Тесты и рефакторинг Cookbook/ApiGateway** (10.06.2026)

## Что сделано в этой сессии

- `ApiGateway.Tests/RoutingTests.cs` — добавлен тест `UnknownRoute_Returns404`
- `RecipeRepository.cs` — слит с `RecipesDbContext`: теперь наследует `DbContext`, содержит маппинг и `IRecipeRepository`; добавлена константа `DefaultSchema = "cookbook"`
- `RecipesDbContext.cs` — удалён
- `Program.cs`, `RecipesDbContextFactory.cs` — переключены на `RecipeRepository`, inline строки заменены на `HistoryRepository.DefaultTableName` + `RecipeRepository.DefaultSchema`
- Миграции (`Designer.cs`, `Snapshot`) — обновлены `[DbContext(typeof(RecipeRepository))]`
- Тесты `MigrateIdempotencyTests.cs` — используют `RecipeRepository` и константы

## Следующий шаг

Нет активных задач.
