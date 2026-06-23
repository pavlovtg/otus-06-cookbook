# Активный контекст

## Текущая задача

Нет активных изменений.

## Последнее завершённое

Пересоздана миграция `AddMealPlan` (`20260622235509_AddMealPlan.cs`):

**Проблема:** старая миграция была патчем поверх кривого состояния (артефакт `MealPlanId1`, `DropForeignKey`/`DropIndex` в `Up`).

**Исправления:**
- `MealPlanSlotConfiguration.cs` — добавлена явная связь `HasOne<MealPlan>().WithMany(p => p.Slots).HasForeignKey("meal_plan_id").OnDelete(Cascade)`; убрано явное объявление shadow property `Guid?`; FK-имя — snake_case `"meal_plan_id"`
- `MealPlanConfiguration.cs` — без изменений (конвертация `MealPlanId` осталась прежней)
- Миграция пересоздана: чистый `CreateTable` для трёх таблиц, все колонки в snake_case, нет `MealPlanId1`

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
