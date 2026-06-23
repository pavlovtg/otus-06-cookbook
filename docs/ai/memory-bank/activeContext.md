# Активный контекст

## Текущая задача

Багфикс e2e-тестов meal plan — завершён.

## Последнее завершённое

Багфикс: все 10 e2e-тестов `test_meal_plan_api.py` падали с 404.
- Причина: отсутствовал BFF route handler `apps/web/app/api/cookbook/v1/meal-plan/route.ts`
- Создан route handler с `GET`, `PUT`, `DELETE` — проксирует на gateway
- Добавлено исключение `WeekDayOutOfRangeException` (weekDay вне 1–7)
- В `MealPlanController` добавлена валидация weekDay, catch расширен до `MealPlanDomainException`

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
