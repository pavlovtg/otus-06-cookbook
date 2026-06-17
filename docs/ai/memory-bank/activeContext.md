# Активный контекст

## Текущая задача

`recipe-rating` — реализация секций 3 и 4 завершена. Следующие: секции 2.4–2.7, 5.

## Последнее завершённое

`recipe-rating` секции 3 и 4 (+ частично 2):

1. `RecipeRatingConfiguration.cs` — EF конфигурация таблицы `recipe_ratings`, составной PK `(user_id, recipe_id)`, FK CASCADE.
2. `RecipeConfiguration.cs` — добавлена колонка `average_rating` (`float?`).
3. Миграция `AddRecipeRatings` сгенерирована через `dotnet ef`.
4. `RecipeRating.UpdateValue` — метод обновления значения с валидацией.
5. `IRecipeRepository` — добавлены `UpsertRatingAsync`, `DeleteRatingAsync`, `GetAverageRatingAsync`, `GetMyRatingAsync`.
6. `RecipeRepository` — реализованы все 4 метода рейтинга.
7. `RecipeSortOrder.RatingDesc` — добавлено значение.
8. `GetRecipesPagedAsync` — добавлена сортировка `RatingDesc` (NULLS LAST через `OrderByDescending(null→0, 1).ThenByDescending`).
9. `RecipeShortWithAuthor` — добавлены поля `float? AverageRating` и `int? MyRating`.

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
