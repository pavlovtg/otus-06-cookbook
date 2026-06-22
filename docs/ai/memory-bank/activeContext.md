# Активный контекст

## Текущая задача

`recipe-comments` — секции 9 (Frontend BFF) и 10 (Frontend UI) выполнены. Осталось: 11 (E2E API), 12 (UI E2E).

## Последнее завершённое

Реализованы задачи 9.1–9.3 и 10.1–10.5 чейнджа `recipe-comments`:

- `apps/web/lib/bff/comments.ts` — Zod-схемы (`CommentDtoSchema`, `CommentRequestSchema`, `PagedResultCommentDtoSchema`) + функции `getComments`, `addComment`, `deleteComment`
- `apps/web/tests/unit/comments.schema.test.ts` — тесты Zod-схем
- `apps/web/tests/unit/comments.bff.test.ts` — fetch mock тесты
- `apps/web/components/features/CommentItem.tsx` — компонент по макету (`.comment`, `.avatar`, `.comment-head`)
- `apps/web/components/features/CommentsSection.tsx` — секция с пагинацией, формой добавления (скрыта если не авторизован или уже оставил комментарий), удалением
- `apps/web/app/recipes/[id]/page.tsx` — интегрирована `CommentsSection` после `detail-grid`
- `docs/design/storybook/src/stories/CommentItem.stories.tsx` — stories: `Default`, `WithDelete`, `WithoutDelete`

Все 394 unit-теста прошли.

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
