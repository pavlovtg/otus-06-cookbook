# Активный контекст

## Текущая задача

`menu-planner` — задачи 9–12 (Frontend BFF + страница планировщика + навигация + тесты) — **завершены**.

## Последнее завершённое

Frontend-часть планировщика меню (задачи 9–12):

- `lib/schemas/meal-plan.ts` — Zod-схемы DTO и Request (MealPlanItem/Slot/Plan)
- `lib/bff/meal-plan.server.ts` — `getMealPlan()` (Server Component, `serverFetch`)
- `lib/bff/meal-plan.ts` — Server Actions `updateMealPlan()` / `clearMealPlan()` (`"use server"`)
- `lib/planner-utils.ts` — конвертация DTO↔Plan, `emptyPlan()`, константы DAY_LABELS/MEAL_KEYS
- `components/features/planner/` — `PlannerRecipeCard` (useDraggable), `PlannerSlot` (useDroppable), `PlannerGrid`, `PlannerPanel`
- `app/planner/page.tsx` — Server Component, redirect если не авторизован
- `app/planner/PlannerPageClient.tsx` — DndContext, автосохранение debounce 300 мс, диалог очистки
- `app/layout.tsx` — пункт «Планировщик» с CalendarIcon (только для авторизованных)
- `lib/icons.tsx` — добавлен `CalendarIcon`
- `tests/unit/meal-plan.schema.test.ts` — 25 тестов Zod-схем
- `tests/bff/meal-plan.bff.test.ts` — 10 тестов BFF-функций
- Установлены `@dnd-kit/core` и `@dnd-kit/utilities`
- Все 453 теста прошли

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
