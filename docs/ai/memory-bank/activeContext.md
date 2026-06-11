# Активный контекст

## Текущая задача

Завершена реализация change `recipes-crud` — полный CRUD рецептов.

## Что сделано

### Backend (.NET)

- Расширен агрегат `Recipe`: `CookingTime`, `Difficulty` (enum), `Servings`, `Instructions`
- EF Core миграция `20260612000000_AddRecipeFields`
- CRUD-методы в репозитории, сервисе, контроллере
- Интеграционные тесты: 38/38 проходят

### Frontend (Next.js)

- Zod-схемы: `RecipeDtoSchema`, `RecipeRequestSchema`, `DifficultySchema`
- BFF: `getRecipe`, `createRecipe`, `updateRecipe`, `deleteRecipe`
- UI: CSS из Storybook, компоненты `RecipeCard`, `RecipeForm`
- Страницы: `/`, `/recipes/[id]`, `/recipes/new`, `/recipes/[id]/edit`
- Unit-тесты: 16/16 проходят

### Тесты

- E2E API: `tests/e2e/test_recipes_api.py` — 9 тестов
- UI Playwright: `tests/ui/test_recipes.py` — 8 тестов

## Следующий шаг

Архивировать change `recipes-crud` командой `/opsx:archive`.
