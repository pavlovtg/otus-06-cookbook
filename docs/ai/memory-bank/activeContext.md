# Активный контекст

## Текущая задача

Нет активных задач.

## Что сделано в этой задаче

Создан `RecipeMicroserviceHost` для microservice-тестов.

- Создан `Microservice/RecipeMicroserviceHost.cs`: наследует `WebApplicationFactory<Program>`, принимает connection string, конфигурирует `DatabaseConnection` через JSON-стрим, содержит `EnsureServer()` с polling health-check
- Обновлён `RecipesCrudTests.cs`: `WebApplicationFactory<Program>` заменён на `RecipeMicroserviceHost`
- Сборка: succeeded, 0 warnings

## Следующий шаг

Нет активных задач.
