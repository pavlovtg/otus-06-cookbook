# Активный контекст

## Текущая задача

Нет активных задач.

## Последнее завершённое

Исправлены упавшие e2e-тесты (11 падений):

1. `tests/e2e/test_recipes_api.py` — добавлен `"isPublic": True` в `VALID_RECIPE` (рецепты создавались приватными, GET без токена → 403).
2. `apps/Cookbook/.../RecipeRepository.cs` — фильтр видимости исправлен: `UserId?` разбит на два случая (`HasValue` / else), чтобы EF Core параметризовал `UserId` (не nullable) через value converter корректно. `EF.Property<Guid?>` вызывал `InvalidCastException` т.к. возвращал тип свойства `UserId?`, а не `Guid?`.

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
