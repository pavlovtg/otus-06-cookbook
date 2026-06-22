# Активный контекст

## Текущая задача

Нет активных изменений. Чейндж `recipe-comments` заархивирован.

## Последнее завершённое

Архивирование чейнджа `recipe-comments` (35/35 задач):

- Delta specs синхронизированы с основными:
  - Добавлены новые specs: `recipe-comment-add`, `recipe-comment-delete`, `recipe-comment-list`
  - Обновлён `recipe-detail/spec.md` — добавлены требования и сценарии секции комментариев
- Архив: `openspec/changes/archive/2026-06-22-recipe-comments/`

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
