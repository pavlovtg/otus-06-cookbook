# Активный контекст

## Текущая задача

Создан change `recipe-rating` в OpenSpec. Все артефакты готовы к реализации (`/opsx:apply`).

## Последнее завершённое

`user-favorites` секции 1–2:

1. Домен: `UserFavorite.cs` — сущность с `UserId` + `RecipeId`.
2. EF конфигурация: `UserFavoriteConfiguration.cs` — PK `(user_id, recipe_id)`, FK CASCADE на `users` и `recipes`.
3. Миграция: `20260617121414_AddUserFavorites` сгенерирована через `dotnet ef`.
4. `IRecipeRepository`: добавлены `AddFavoriteAsync`, `RemoveFavoriteAsync`, `GetFavoriteIdsAsync`.
5. `RecipeRepository`: реализованы через EF (`AnyAsync` + `AddAsync` / `FirstOrDefaultAsync` + `Remove` / `Where + ToListAsync`).
6. `RecipeShortWithAuthor`: добавлено поле `bool? IsFavorite`.
7. `RecipeService.GetRecipesPagedAsync`: загружает `favoriteIds` через `GetFavoriteIdsAsync` и заполняет `IsFavorite` (null для анонимов).

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
