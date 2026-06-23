# Активный контекст

## Текущая задача

Багфикс страницы списка покупок — завершён.

## Последнее завершённое

Исправление 4 ошибок на странице `/shopping-list`:

- `ShoppingListController.cs` + `IngredientCategoryDtoExtensions.cs`: категории теперь отдаются в snake_case через `ToDtoString()` (JSON-сериализация enum с `[JsonStringEnumMemberName]`)
- `shopping-list/page.tsx`: русские названия категорий через `IngredientCategoryLabels`; количество с точностью 2 знака
- `ShoppingListActions.tsx`: русские категории и точность 2 знака при копировании
- `layout.tsx`: добавлена вкладка «Список покупок» с `CartIcon` — только для авторизованных

## Ранее завершённое

Исправление 4 падающих UI-тестов планировщика:

- `PlannerGrid.tsx`: добавлены классы `planner-day-header` (к `planner-head-cell`) и `planner-meal-header` (к `planner-meal-label`) — тесты искали именно эти классы
- `globals.css`: добавлен `isolation: isolate` к `.planner-panel` — draggable-карточки dnd-kit создавали stacking context и перекрывали `modal-backdrop` (z-index: 100), блокируя клики по кнопкам диалога

## Ранее завершённое

Улучшение UI планировщика меню:

- `PlannerRecipeCard` — убраны inline-размеры фото (48×48), теперь CSS управляет через `aspect-ratio: 16/10`
- `PlannerSlot` — добавлена миниатюра рецепта (`aspect-ratio: 16/9`), кнопка удаления перемещена в правый верхний угол (`position: absolute; top: 4px; right: 4px`), контрол порций — в правый нижний (`position: absolute; bottom: 4px; right: 4px`)
- `PlannerGrid` — добавлен prop `recipePhotos: Record<string, string | undefined>`
- `PlannerPageClient` — формирует `recipePhotos` из `recipes` и передаёт в `PlannerGrid`
- `globals.css` — карточка панели `180px`, ячейки сетки `minmax(160px, 1fr)`, слот `min-height: 180px`, layout `planner-slot-item` переработан на `flex-direction: column` с абсолютным позиционированием кнопки и порций

## Ключевые решения

- Auth-модуль — инфраструктурный cross-cutting слой внутри recipes-сервиса
- Пользователи хранятся в схеме `cookbook` той же PostgreSQL
- JWT выпускается и валидируется recipes-сервисом
- S2S не нужен (один доменный сервис в MVP)
- Token blacklist — не реализован, тест скипнут
- Hard navigation после логина нужна, т.к. Next.js layout — серверный компонент и не перерендеривается при soft navigation
- Auth route.ts использовали `/api/v1/auth/...` напрямую к gateway, но gateway знает только `/api/cookbook/**` → исправлено на `/api/cookbook/v1/auth/...`
- iron-session хранит JWT в зашифрованной cookie `cookbook_session`
- `isPublic` и `authorName` добавлены в Zod-схемы `RecipeShortDtoSchema`, `RecipeDtoSchema`, `RecipeRequestSchema`
- 403 на детальной странице обрабатывается через проверку `err.message.includes("403")` → показывает UI-сообщение вместо `notFound()`
- `serverFetch(url, init?)` в `lib/server-fetch.ts` — обёртка для Server Components, автоматически добавляет `Authorization` из `getSession()`. `getRecipe`/`getRecipes` используют `serverFetch` — автор видит свои приватные рецепты.
- `CommentsSection` — Client Component; первая страница комментариев загружается в Server Component (`getComments` с `.catch`) и передаётся как `initialData`; смена страниц — клиентский fetch
- Один комментарий на пользователя на рецепт — уникальный индекс `(recipe_id, author_id)` в таблице `recipe_comments`
- DnD планировщика — `@dnd-kit/core` + `@dnd-kit/utilities` (ADR-0036, AR-0064); `useDraggable` на карточке, `useDroppable` на слоте, `DragOverlay` для floating-карточки
- Все API-запросы идут через nginx → Next.js BFF route handlers → gateway → recipes; BFF route handler обязателен для каждого ресурса
