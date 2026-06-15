# Active Context

## Текущая задача

Багфикс: `CreateRecipe_WithNonExistentCategoryId_Returns400` — исправлен.

## Что было сделано в последней сессии (recipe-categories frontend)

- **6.1–6.2** `lib/schemas/recipe.ts` — добавлен `categoryIds: z.array(z.string().uuid())` в `RecipeShortDtoSchema`, `RecipeDtoSchema`, `RecipeRequestSchema`
- **6.3** `app/(public)/page.tsx` — `Promise.all([getRecipes(), getCategories()])`, `categories` пробрасывается в `RecipeCard`
- **6.4** Инвалидация уже реализована через `window.location.assign` в `CategoryModal` и `DeleteCategoryButton`
- **7.1** `components/features/CategoryTagInput.tsx` — `.tag-input` + `.chip` + `.autocomplete`, замена при совпадении типа
- **7.2** `components/features/RecipeForm.tsx` — добавлен `CategoryTagInput`, загрузка `getCategories()` в `useEffect`, `categoryIds` в state
- **7.2** `app/recipes/[id]/edit/EditRecipeForm.tsx` — `categoryIds` в `initialValues`
- **7.3** `components/features/RecipeCard.tsx` — отображение до 3 тегов категорий из `categoryIds` + `categories[]`
- **7.4** `app/recipes/[id]/page.tsx` — `Promise.all([getRecipe, getCategories])`, все теги в `.detail-tags`
- **7.5** `docs/design/storybook/src/domain/CategoryTagInput.tsx` — компонент для Storybook
- **7.5** `docs/design/storybook/src/stories/CategoryTagInput.stories.tsx` — 5 историй: Empty, AddChip, RemoveChip, ReplaceByType, Playground ★
- **7.5** `docs/design/storybook/src/stories/RecipeCard.stories.tsx` — добавлены `WithTags` и `WithoutTags`
- **8.1–8.2** `tests/ui/test_recipes.py` — 3 новых теста: создание с категориями → карточка, детальная; редактирование → обновление

## Ключевые решения

- `categoryIds` в схемах — required (не optional), т.к. backend всегда возвращает массив
- `RecipeCard` fallback: если `categoryIds` пуст или категории не найдены — показывает тег сложности
- `detail-tags` fallback: если нет категорий — показывает сложность/порции/время
- Замена при совпадении типа реализована в `CategoryTagInput.addCategory`
- E2E тесты используют `data-testid` атрибуты компонента
