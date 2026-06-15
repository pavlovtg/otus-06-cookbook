# Active Context

## Текущая задача

Багфикс: `categoryIds` не возвращались в списке рецептов (`GET /api/v1/recipes`).

## Что было сделано в последней сессии

- **Баг**: `RecipeRepository.GetRecipesAsync` не делал `.Include(r => r.Categories)` → `recipe.Categories` пустой → `categoryIds: []` в `RecipeShortDto`
- **Тест**: добавлен `GetRecipesList_AfterCreateWithCategory_ContainsCategoryId` в `RecipeCategoryTests.cs` — воспроизводит баг
- **Фикс**: `RecipeRepository.cs` — добавлен `.Include(r => r.Categories)` в `GetRecipesAsync`
- Все 11 тестов `RecipeCategoryTests` проходят

## Ключевые решения

- `categoryIds` в схемах — required (не optional), т.к. backend всегда возвращает массив
- `RecipeCard` fallback: если `categoryIds` пуст или категории не найдены — показывает тег сложности
- `detail-tags` fallback: если нет категорий — показывает сложность/порции/время
- Замена при совпадении типа реализована в `CategoryTagInput.addCategory`
- E2E тесты используют `data-testid` атрибуты компонента
