# recipe-rating

## Контекст

В системе уже реализованы: рецепты, ингредиенты, категории, авторизация (JWT), избранное. Структура backend — гексагональная (Domain / Application / Adapters), ORM — EF Core + PostgreSQL. Frontend — Next.js App Router + BFF (`lib/bff/`). Рейтинг рецептов отсутствует: нет таблицы, нет эндпоинтов, нет UI-компонентов.

Текущие DTO (`RecipeShortDto`, `RecipeDto`) не содержат полей рейтинга. `RecipeSortOrder` содержит только `TitleAsc` / `TitleDesc`.

## Цели / Не-цели

**Цели:**

- Новая таблица `recipe_ratings` (user_id, recipe_id, value).
- Денормализованное поле `average_rating` (float) в таблице `recipes` — обновляется при каждом upsert/delete оценки.
- Эндпоинты `PUT /api/v1/recipes/{id}/rating` и `DELETE /api/v1/recipes/{id}/rating`.
- Поля `averageRating` и `myRating` в ответах списка и детальной страницы рецептов.
- Сортировка `rating_desc` в `GET /api/v1/recipes`.
- UI: виджет звёзд на карточке и детальной странице; BFF-функции в `lib/bff/ratings.ts`.
- Seed-данные: оценки для демо-рецептов.
- Тесты: unit (домен), microservice (эндпоинты), e2e API, frontend unit (BFF + Zod).

**Не-цели:**

- Агрегированная статистика рейтингов (дашборд — отдельная фича).
- Нотификации при получении оценки.

## Решения

### 1. Хранение рейтингов — таблица + денормализация

Таблица `recipe_ratings` с составным PK `(user_id, recipe_id)` и полем `value` (smallint, 1–5). При каждом upsert или delete оценки Application-сервис пересчитывает среднее через SQL (`AVG`) и сохраняет в `Recipe.AverageRating` (`float?`, nullable — null если оценок нет). Это позволяет сортировать по `average_rating` без JOIN.

**Альтернатива:** вычислять AVG на лету через LEFT JOIN при каждом запросе списка — отклонено, усложняет сортировку и снижает производительность при росте данных.

### 2. Доменная сущность RecipeRating — по аналогии с RecipeIngredient

`RecipeRating` — value object внутри агрегата `Recipe` (аналогично `RecipeIngredient`): класс с полями `UserId` и `Value` (smallint, 1–5), валидация в статическом фабричном методе `Create`. Исключение `RatingValueOutOfRangeException` для значений вне диапазона 1–5.

`Recipe` дополняется:

- `float? AverageRating` — денормализованное среднее.
- `IReadOnlyList<RecipeRating> Ratings` — коллекция оценок (аналогично `Ingredients`).
- Метод `SetAverageRating(float? value)` — вызывается из Application после пересчёта.

### 3. Расширение IRecipeRepository

Новые методы:

- `UpsertRatingAsync(UserId, RecipeId, int value, CancellationToken)` — upsert через EF.
- `DeleteRatingAsync(UserId, RecipeId, CancellationToken)` — удаление оценки; возвращает `bool` (false если оценки не было).
- `GetAverageRatingAsync(RecipeId, CancellationToken)` → `float?` — пересчёт среднего после изменения.
- `GetMyRatingAsync(UserId, RecipeId, CancellationToken)` → `int?` — личная оценка пользователя.

`RecipeShortWithAuthor` дополняется полями `float? AverageRating` и `int? MyRating`.

### 4. Эндпоинты

```
PUT    /api/v1/recipes/{id}/rating   body: { "value": 1-5 }  → 200 RatingSummaryDto
DELETE /api/v1/recipes/{id}/rating                           → 204 No Content
```

`PUT` реализует upsert (создаёт или заменяет). `DELETE` удаляет оценку текущего пользователя; если оценки не было — `400 Bad Request`. После каждой операции Application пересчитывает `AverageRating` и сохраняет в `Recipe`.

### 5. Расширение DTO

`RecipeShortDto` и `RecipeDto` дополняются:

- `float? AverageRating` — null если нет оценок.
- `int? MyRating` — null если пользователь не оценивал или не авторизован.

### 6. Сортировка rating_desc

`RecipeSortOrder` дополняется значением `RatingDesc`. В репозитории — `ORDER BY average_rating DESC NULLS LAST` (по денормализованному полю, без JOIN).

### 7. Frontend BFF

Новый файл `lib/bff/ratings.ts` с функциями:

- `setRating(recipeId, value)` — `PUT` через `serverFetch`.
- `deleteRating(recipeId)` — `DELETE` через `serverFetch`.

Zod-схемы `RecipeShortDtoSchema` и `RecipeDtoSchema` дополняются полями `averageRating` (nullable number) и `myRating` (nullable number).

### 8. UI-компонент StarsRating

Компонент `StarsRating` из Storybook (`src/domain/StarsRating`) портируется в `apps/web/components/`. Используется на карточке (read-only для среднего, личная оценка — текстом) и на детальной странице (интерактивный виджет для авторизованных).

### 9. Тестирование

**Backend — unit (Domain):**

- `RecipeRating.Create` с валидными значениями (1, 3, 5).
- `RecipeRating.Create` с невалидными значениями (0, 6) → `RatingValueOutOfRangeException`.
- `Recipe.SetAverageRating` обновляет поле.

**Backend — microservice (Testcontainers + WebApplicationFactory):**

- `PUT /api/v1/recipes/{id}/rating` — авторизованный пользователь выставляет оценку → 200.
- `PUT /api/v1/recipes/{id}/rating` — повторная оценка заменяет предыдущую → 200.
- `PUT /api/v1/recipes/{id}/rating` — неавторизованный → 401.
- `PUT /api/v1/recipes/{id}/rating` — значение 0 или 6 → 400.
- `DELETE /api/v1/recipes/{id}/rating` — удаление существующей оценки → 204.
- `DELETE /api/v1/recipes/{id}/rating` — оценки нет → 400.
- `GET /api/v1/recipes?sort=rating_desc` — рецепты отсортированы по убыванию рейтинга.

**E2E API (pytest + httpx):**

- Полный сценарий: логин → выставить оценку → проверить `averageRating` в списке → удалить оценку → проверить `averageRating = null`.

**Frontend — unit (Vitest):**

- `ratings.schema.test.ts`: Zod-схема `RatingSummaryDtoSchema` валидирует корректные и некорректные данные.
- `ratings.bff.test.ts`: `setRating` вызывает `PUT` с правильным телом; `deleteRating` вызывает `DELETE`.
- `recipes.schema.test.ts`: обновлённые схемы `RecipeShortDtoSchema` и `RecipeDtoSchema` принимают поля `averageRating` и `myRating`.

## Риски / Компромиссы

- **[Согласованность]** `average_rating` обновляется в той же транзакции, что и upsert/delete оценки → данные всегда консистентны.
- **[Миграция]** Новые таблица и колонка — безопасный forward-only деплой; rollback — DROP TABLE + DROP COLUMN.
- **[Seed-данные]** Оценки добавляются в `CookbookSeeder` идемпотентно; после вставки всех оценок пересчитывается `average_rating` для каждого рецепта.
