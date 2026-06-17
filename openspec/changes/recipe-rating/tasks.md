# recipe-rating

## 1. Domain

- [ ] 1.1 Создать `RecipeRating.cs` — value object с полями `UserId`, `Value` (int 1–5), фабричный метод `Create` с валидацией
- [ ] 1.2 Создать `RatingValueOutOfRangeException.cs` — доменное исключение для значений вне диапазона 1–5
- [ ] 1.3 Добавить в `Recipe`: поле `float? AverageRating`, коллекцию `IReadOnlyList<RecipeRating> Ratings`, метод `SetAverageRating(float? value)`
- [ ] 1.4 Добавить в `RecipeConstraints.cs` константы `RatingMin = 1` и `RatingMax = 5`

## 2. Application

- [ ] 2.1 Добавить в `IRecipeRepository` методы: `UpsertRatingAsync`, `DeleteRatingAsync`, `GetAverageRatingAsync`, `GetMyRatingAsync`
- [ ] 2.2 Добавить значение `RatingDesc` в `RecipeSortOrder`
- [ ] 2.3 Добавить в `RecipeShortWithAuthor` поля `float? AverageRating` и `int? MyRating`
- [ ] 2.4 Добавить в `IRecipeService` методы `SetRatingAsync` и `DeleteRatingAsync`
- [ ] 2.5 Реализовать `RecipeService.SetRatingAsync`: upsert оценки → пересчёт AVG → `Recipe.SetAverageRating` → `CommitAsync`
- [ ] 2.6 Реализовать `RecipeService.DeleteRatingAsync`: удаление оценки (400 если нет) → пересчёт AVG → `Recipe.SetAverageRating` → `CommitAsync`
- [ ] 2.7 Обновить `RecipeService.GetRecipesPagedAsync`: передавать `AverageRating` и `MyRating` в `RecipeShortWithAuthor`

## 3. Database

- [ ] 3.1 Создать `RecipeRatingConfiguration.cs` — EF конфигурация таблицы `recipe_ratings` (PK составной, FK на users и recipes с CASCADE)
- [ ] 3.2 Добавить `float? AverageRating` в `RecipeConfiguration.cs` (колонка `average_rating`)
- [ ] 3.3 Сгенерировать миграцию `AddRecipeRatings` через `dotnet ef migrations add`

## 4. Repository

- [ ] 4.1 Реализовать `RecipeRepository.UpsertRatingAsync` — upsert через EF (find + update или add)
- [ ] 4.2 Реализовать `RecipeRepository.DeleteRatingAsync` — поиск и удаление, возврат `bool`
- [ ] 4.3 Реализовать `RecipeRepository.GetAverageRatingAsync` — `AVG` через EF LINQ
- [ ] 4.4 Реализовать `RecipeRepository.GetMyRatingAsync` — поиск по `(userId, recipeId)`
- [ ] 4.5 Обновить `RecipeRepository.GetRecipesPagedAsync`: добавить `RatingDesc` в сортировку (`ORDER BY average_rating DESC NULLS LAST`), передавать `AverageRating` и `MyRating`

## 5. Web Adapter

- [ ] 5.1 Добавить `float? AverageRating` и `int? MyRating` в `RecipeShortDto` и `RecipeDto`
- [ ] 5.2 Создать `RatingRequest.cs` — DTO с полем `int Value`
- [ ] 5.3 Создать `RatingSummaryDto.cs` — DTO с полями `float? AverageRating` и `int? MyRating`
- [ ] 5.4 Добавить в `RecipesController` эндпоинты `PUT /api/v1/recipes/{id}/rating` и `DELETE /api/v1/recipes/{id}/rating`
- [ ] 5.5 Обновить маппинг в `RecipesController`: передавать `AverageRating` и `MyRating` из `RecipeShortWithAuthor` в `RecipeShortDto`
- [ ] 5.6 Обновить `GlobalExceptionHandler`: обрабатывать `RatingValueOutOfRangeException` → 400

## 6. OpenAPI Contract

- [x] 6.1 Добавить в `docs/contracts/cookbook/recipes.yaml` операции `PUT /recipes/{id}/rating` и `DELETE /recipes/{id}/rating`
- [x] 6.2 Добавить поля `averageRating` и `myRating` в схемы `RecipeShortDto` и `RecipeDto`
- [x] 6.3 Добавить значение `rating_desc` в enum параметра `sort`

## 7. Seed Data

- [ ] 7.1 Добавить в `CookbookSeeder.cs` вставку оценок для демо-рецептов (идемпотентно через upsert)
- [ ] 7.2 После вставки оценок пересчитать и сохранить `average_rating` для каждого рецепта

## 8. Backend Tests

- [ ] 8.1 Unit: `RecipeRating.Create` — валидные значения (1, 3, 5) создаются без исключений
- [ ] 8.2 Unit: `RecipeRating.Create` — значения 0 и 6 бросают `RatingValueOutOfRangeException`
- [ ] 8.3 Unit: `Recipe.SetAverageRating` обновляет поле `AverageRating`
- [ ] 8.4 Microservice: `PUT /api/v1/recipes/{id}/rating` авторизованный → 200 с корректными `averageRating` и `myRating`
- [ ] 8.5 Microservice: `PUT /api/v1/recipes/{id}/rating` повторная оценка заменяет предыдущую → 200
- [ ] 8.6 Microservice: `PUT /api/v1/recipes/{id}/rating` неавторизованный → 401
- [ ] 8.7 Microservice: `PUT /api/v1/recipes/{id}/rating` значение 0 или 6 → 400
- [ ] 8.8 Microservice: `DELETE /api/v1/recipes/{id}/rating` существующая оценка → 204
- [ ] 8.9 Microservice: `DELETE /api/v1/recipes/{id}/rating` оценки нет → 400
- [ ] 8.10 Microservice: `GET /api/v1/recipes?sort=rating_desc` — рецепты отсортированы по убыванию `averageRating`

## 9. Frontend BFF

- [ ] 9.1 Создать `apps/web/lib/bff/ratings.ts` с функциями `setRating` и `deleteRating`
- [ ] 9.2 Добавить поля `averageRating` и `myRating` в `RecipeShortDtoSchema` и `RecipeDtoSchema`
- [ ] 9.3 Создать `RatingSummaryDtoSchema` в `apps/web/lib/bff/ratings.ts`

## 10. Frontend Tests

- [ ] 10.1 `ratings.schema.test.ts`: Zod-схема `RatingSummaryDtoSchema` валидирует корректные и некорректные данные
- [ ] 10.2 `ratings.bff.test.ts`: `setRating` вызывает `PUT` с правильным телом; `deleteRating` вызывает `DELETE`
- [ ] 10.3 `recipes.schema.test.ts`: обновлённые схемы принимают поля `averageRating` и `myRating`

## 11. Frontend UI

- [ ] 11.1 Портировать компонент `StarsRating` из Storybook в `apps/web/components/StarsRating.tsx`
- [ ] 11.2 Добавить отображение `averageRating` и `myRating` на карточке рецепта в списке
- [ ] 11.3 Добавить интерактивный виджет `StarsRating` на детальной странице рецепта (только для авторизованных)
- [ ] 11.4 Добавить переключатель сортировки `rating_desc` в сайдбар страницы списка рецептов

## 12. E2E API Tests

- [ ] 12.1 Добавить в `tests/e2e/` тест: логин → выставить оценку → проверить `averageRating` в списке → удалить оценку → проверить `averageRating = null`
