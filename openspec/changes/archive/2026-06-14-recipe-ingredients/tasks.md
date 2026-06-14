# recipe-ingredients

## 1. OpenAPI контракт

- [x] 1.1 Добавить схему `RecipeShortDto` (без ингредиентов) в `docs/contracts/cookbook/recipes.yaml`
- [x] 1.2 Добавить схему `RecipeIngredientDto` (id ингредиента, название, количество, единица) в `docs/contracts/cookbook/recipes.yaml`
- [x] 1.3 Расширить `RecipeDto` полем `ingredients: RecipeIngredientDto[]`
- [x] 1.4 Добавить обязательное поле `ingredients: RecipeIngredientRequest[]` в `RecipeRequest` (массив с `ingredientId` и `amount`)
- [x] 1.5 Изменить `GET /api/v1/recipes` — ответ `RecipeShortDto[]` вместо `RecipeDto[]`

## 2. Backend — домен

- [x] 2.1 Добавить в `RecipeConstraints` константу `IngredientsMaxCount = 50`
- [x] 2.2 Добавить в `RecipeConstraints` константы `IngredientAmountMin` (0.001) и `IngredientAmountMax` (100 000)
- [x] 2.3 Создать `RecipeIngredient` (sealed class) с полями `IngredientId` и `Amount`, валидацией `Amount`
- [x] 2.4 Добавить коллекцию `IReadOnlyList<RecipeIngredient> Ingredients` в агрегат `Recipe`
- [x] 2.5 Обновить `Recipe.Create` и `Recipe.Update` — принимать список `RecipeIngredient`, валидировать лимит 50
- [x] 2.6 Создать доменное исключение `RecipeIngredientAmountOutOfRangeException`
- [x] 2.7 Создать доменное исключение `RecipeIngredientsTooManyException`
- [x] 2.8 Создать доменное исключение `IngredientInUseException` — принимает top-10 названий рецептов и общий счётчик использований

## 3. Backend — Application

- [x] 3.1 Добавить метод `GetRecipesUsingIngredientAsync(IngredientId, CancellationToken)` в `IRecipeRepository` — возвращает `(IReadOnlyList<string> TopTitles, int TotalCount)` (top-10 названий + общий count)
- [x] 3.2 Обновить `IRecipeService` — методы `CreateAsync` и `UpdateAsync` принимают список ингредиентов (id + amount)
- [x] 3.3 Обновить `RecipeService.CreateAsync` — маппить входные данные в `RecipeIngredient`, передавать в `Recipe.Create`
- [x] 3.4 Обновить `RecipeService.UpdateAsync` — маппить входные данные в `RecipeIngredient`, передавать в `Recipe.Update`
- [x] 3.5 Обновить `IngredientService.DeleteAsync` — вызывать `GetRecipesUsingIngredientAsync`, при `TotalCount > 0` выбрасывать `IngredientInUseException` с top-10 и счётчиком

## 4. Backend — адаптер PostgreSQL

- [x] 4.1 Создать `RecipeIngredientConfiguration` (`IEntityTypeConfiguration<RecipeIngredient>`) с маппингом на таблицу `cookbook.recipe_ingredients` и каскадным удалением при удалении рецепта
- [x] 4.2 Зарегистрировать конфигурацию в `RecipeRepository.OnModelCreating`
- [x] 4.3 Обновить `RecipeRepository.GetByIdAsync` — добавить `Include` ингредиентов
- [x] 4.4 Реализовать `GetRecipesUsingIngredientAsync` в `RecipeRepository` — запрос: count всех рецептов с данным `IngredientId` + top-10 названий
- [x] 4.5 Создать миграцию EF Core для таблицы `cookbook.recipe_ingredients`
- [x] 4.6 Создать единый класс-оркестратор загрузки seed-данных (определяет порядок и границы транзакций для всех сидеров)
- [x] 4.7 Обновить `RecipeSeeder` — добавить ингредиенты к рецептам

## 5. Backend — адаптер Web

- [x] 5.1 Создать `RecipeShortDto` (без ингредиентов)
- [x] 5.2 Создать `RecipeIngredientDto` (ingredientId, title, amount, unit)
- [x] 5.3 Создать `RecipeIngredientRequest` (ingredientId, amount)
- [x] 5.4 Расширить `RecipeDto` полем `Ingredients: RecipeIngredientDto[]`
- [x] 5.5 Расширить `RecipeRequest` обязательным полем `Ingredients: RecipeIngredientRequest[]`
- [x] 5.6 Обновить `RecipesController.GetRecipes` — возвращать `RecipeShortDto`
- [x] 5.7 Обновить `RecipesController.GetRecipe` — возвращать `RecipeDto` с ингредиентами
- [x] 5.8 Обновить `RecipesController.CreateRecipe` и `UpdateRecipe` — маппить `Ingredients` из запроса, перехватывать новые доменные исключения → `400`
- [x] 5.9 Обновить `IngredientsController.DeleteIngredient` — перехватывать `IngredientInUseException` → `400` с top-10 названий рецептов и общим счётчиком в `detail`

## 6. Frontend — схемы и BFF

- [x] 6.1 Добавить `RecipeIngredientDtoSchema` в `apps/web/lib/schemas/recipe.ts`
- [x] 6.2 Добавить `RecipeIngredientRequestSchema` в `apps/web/lib/schemas/recipe.ts`
- [x] 6.3 Добавить `RecipeShortDtoSchema` в `apps/web/lib/schemas/recipe.ts`
- [x] 6.4 Расширить `RecipeDtoSchema` полем `ingredients: RecipeIngredientDtoSchema[]`
- [x] 6.5 Расширить `RecipeRequestSchema` обязательным полем `ingredients: RecipeIngredientRequestSchema[]`
- [x] 6.6 Обновить `apps/web/lib/bff/recipes.ts` — `getRecipes` использует `RecipeShortDtoSchema`

## 7. Frontend — UI

- [x] 7.1 Добавить секцию ингредиентов на детальную страницу рецепта (`apps/web/app/recipes/[id]/page.tsx`)
- [x] 7.2 Добавить управление ингредиентами в форму создания рецепта (`apps/web/app/recipes/new/`)
- [x] 7.3 Добавить управление ингредиентами в форму редактирования рецепта (`apps/web/app/recipes/[id]/edit/`)
- [x] 7.4 Добавить отображение ошибки при попытке удалить ингредиент, используемый в рецептах — показывать сообщение с перечнем до 10 названий рецептов и общим счётчиком из поля `detail` ответа API

## 8. Тесты

- [x] 8.1 Unit-тест: `RecipeIngredient` — валидация amount (граничные значения: 0, 0.001, 100 000, 100 001)
- [x] 8.2 Unit-тест: `Recipe.Create` / `Recipe.Update` с ингредиентами — успех и превышение лимита 50
- [x] 8.3 Unit-тест: `IngredientService.DeleteAsync` — блокировка при использовании в рецептах
- [x] 8.4 Интеграционный тест репозитория: создание рецепта с ингредиентами, загрузка по id
- [x] 8.5 Интеграционный тест репозитория: `GetRecipesUsingIngredientAsync` — возвращает корректный top-10 и total count
- [x] 8.6 Microservice-тест: `POST /api/v1/recipes` с ингредиентами → `GET /api/v1/recipes/{id}` возвращает ингредиенты
- [x] 8.7 Microservice-тест: `PUT /api/v1/recipes/{id}` — обновление списка ингредиентов
- [x] 8.8 Microservice-тест: `DELETE /api/v1/ingredients/{id}` — блокировка при использовании → `400` с перечнем рецептов и счётчиком
- [x] 8.9 Unit-тест frontend: `RecipeIngredientDtoSchema` — валидация корректных и некорректных данных
- [x] 8.10 Unit-тест frontend: `getRecipes` BFF — возвращает `RecipeShortDto[]`
- [x] 8.11 E2E API тест: добавление ингредиентов при создании рецепта, проверка в ответе
- [x] 8.12 E2E API тест: убирание ингредиента при редактировании рецепта
