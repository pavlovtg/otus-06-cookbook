# Прогресс

## Статус проекта

В разработке.

## Завершённые фичи

- Список рецептов (главная страница)
- Детальная страница рецепта
- Создание рецепта
- Редактирование рецепта
- Удаление рецепта
- CRUD ингредиентов (backend + тесты + frontend схемы/BFF + E2E тесты)
- `recipe-ingredients` — все 56 задач (секции 1–8) полностью реализованы

## В работе

Нет активных задач.

## Выполнено (последнее)

- `recipe-ingredients` секция 8 — все тесты (8.1–8.12):
  - Unit: `RecipeIngredientTests`, `RecipeWithIngredientsTests`
  - Integration: `RecipeRepositoryTests` (ингредиенты + `GetRecipesUsingIngredientAsync`)
  - Microservice: POST/GET/PUT с ингредиентами, DELETE блокировка
  - Frontend unit: `RecipeIngredientDtoSchema`
  - E2E: создание с ингредиентами, убирание ингредиента
  - Фикс: `RecipeRequest` с `Ingredients: []` во всех тестах, `VALID_RECIPE` в e2e
