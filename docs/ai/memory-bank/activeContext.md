# Активный контекст

## Текущая задача

Применён AR-0041 к тестовым проектам Recipes.Tests и ApiGateway.Tests.

## Что сделано в этой задаче

- `apps/Cookbook/tests/Recipes.Tests/Unit/Domain/` — перемещены `RecipeTests.cs`, `RecipeIdTests.cs`
- `apps/Cookbook/tests/Recipes.Tests/Integration/Adapters/Postgresql/` — перемещён `MigrateIdempotencyTests.cs`
- `apps/Cookbook/tests/Recipes.Tests/Microservice/` — перемещены `GetRecipesTests.cs`, `RecipesCrudTests.cs`; namespace обновлён на `Recipes.Tests.Microservice`
- `apps/ApiGateway/tests/ApiGateway.Tests/Microservice/` — перемещён `RoutingTests.cs`; namespace обновлён на `ApiGateway.Tests.Microservice`

## Следующий шаг

Нет активных задач.
