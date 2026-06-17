# Активный контекст

## Текущая задача

Нет активных задач.

## Последнее завершённое

Скрыты кнопки редактирования рецепта для не-авторов и не-админов:

1. Backend: добавлено поле `AuthorId: Guid?` в `RecipeDto` и `RecipeShortDto`; маппер в `RecipesController` заполняет его из `recipe.AuthorId?.Value`.
2. Контракт: `docs/contracts/cookbook/recipes.yaml` — добавлено поле `authorId` (uuid, nullable) в схемы `RecipeDto` и `RecipeShortDto`.
3. Frontend Zod-схема: `authorId: z.string().uuid().nullable().optional()` добавлен в `RecipeDtoSchema` и `RecipeShortDtoSchema`.
4. Frontend страница `recipes/[id]/page.tsx`: `isAuthenticated` заменён на `canEdit = isOwner || isAdmin`, где `isOwner = recipe.authorId === session.user.id`, `isAdmin = session.user?.role === "admin"`. Кнопки «Редактировать», «Удалить» и `RecipePhotoActions` показываются только при `canEdit`.

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
