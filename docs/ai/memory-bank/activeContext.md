# Активный контекст

## Текущая задача

Багфикс UI-тестов ингредиентов — завершено.

## Последнее завершённое

Рефакторинг страницы `/ingredients`:

- `pageSize=20` — фронт явно передаёт `pageSize: 20` в `getIngredients`
- Порядок категорий в `IngredientCategory` (Zod enum) приведён к алфавитному порядку строк БД (`bakery_and_sweets` → `vegetables`) — группы и дропдаун совпадают с сортировкой бэкенда
- Неавторизованный пользователь не видит кнопки «Редактировать» / «Удалить» / «+ Новый ингредиент»
- Системные ингредиенты (`isSystem=true`) может редактировать/удалять только admin
- Признак `isSystem` может менять только admin — чекбокс в `IngredientModal` виден только при `isAdmin=true`
- Backend: `[Authorize]` на POST/PUT/DELETE; `IngredientService` проверяет `isAdmin` перед изменением системных ингредиентов и флага `isSystem`; новое исключение `IngredientSystemFlagForbiddenException` → 403

## Ранее завершённое

Багфикс страницы списка покупок:

- `ShoppingListController.cs` + `IngredientCategoryDtoExtensions.cs`: категории теперь отдаются в snake_case через `ToDtoString()` (JSON-сериализация enum с `[JsonStringEnumMemberName]`)
- `shopping-list/page.tsx`: русские названия категорий через `IngredientCategoryLabels`; количество с точностью 2 знака
- `ShoppingListActions.tsx`: русские категории и точность 2 знака при копировании
- `layout.tsx`: добавлена вкладка «Список покупок» с `CartIcon` — только для авторизованных

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
- `IngredientCategory` в БД хранится как строка (snake_case), сортировка алфавитная → `vegetables` последние; порядок в Zod enum приведён к тому же алфавитному порядку
