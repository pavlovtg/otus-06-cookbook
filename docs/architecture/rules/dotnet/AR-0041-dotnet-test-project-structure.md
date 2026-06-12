# AR-0041: Структура тестовых проектов .NET

Источник: ADR-0033

## Правило

- На каждый сервис — один тестовый проект `<Service>.Tests`.
- Тесты разбиваются по папкам: `Unit/`, `Integration/`, `Microservice/`.
- Тестовый проект не ссылается на production-сборки других сервисов.
- Папка `Unit/` зеркалирует структуру тестируемой сборки: тест класса `Domain/Recipes/Recipe.cs` располагается в `Unit/Domain/Recipes/RecipeTests.cs`.
- Папка `Integration/` стремится зеркалировать структуру основной сборки: тест репозитория `Adapters/Postgresql/RecipesRepository.cs` располагается в `Integration/Adapters/Postgresql/RecipesRepositoryTests.cs`.
- Папка `Microservice/` не имеет фиксированной структуры — тесты организуются по сценариям.
