# user-favorites

## 1. Схема БД и миграция

- [x] 1.1 Создать миграцию EF Core: таблица `user_favorites` (`user_id uuid`, `recipe_id uuid`, уникальный индекс `(user_id, recipe_id)`, `ON DELETE CASCADE` на FK к `users` и `recipes`)

## 2. Репозиторий

- [x] 2.1 Добавить методы `AddFavorite(UserId, RecipeId)` и `RemoveFavorite(UserId, RecipeId)` в `IRecipeRepository`
- [x] 2.2 Добавить метод `GetFavoriteIds(UserId)` в `IRecipeRepository`
- [x] 2.3 Реализовать `AddFavorite` (upsert/ignore) и `RemoveFavorite` (delete, идемпотентно) в `RecipeRepository`
- [x] 2.4 Реализовать JOIN `user_favorites` при запросе списка рецептов для заполнения `IsFavorite`

## 3. Контракт

- [x] 3.1 Добавить эндпоинты `POST /api/v1/recipes/{id}/favorites` и `DELETE /api/v1/recipes/{id}/favorites` в `docs/contracts/cookbook/recipes.yaml`
- [x] 3.2 Добавить параметр `favorites` (boolean) в `GET /api/v1/recipes` в контракте
- [x] 3.3 Добавить поле `isFavorite` (boolean, nullable) в схему `RecipeShortDto` в контракте

## 4. Backend — сервис и контроллер

- [x] 4.1 Добавить параметр `favorites` в `IRecipeService.GetRecipes` и реализовать фильтрацию
- [x] 4.2 Добавить поле `IsFavorite` в `RecipeShortDto`
- [x] 4.3 Добавить эндпоинты `POST` и `DELETE` для `favorites` в `RecipesController` (требуют авторизации)
- [x] 4.4 Добавить параметр `favorites` в `RecipesController.GetRecipes`

## 5. Backend — тесты

- [x] 5.1 Unit-тесты `RecipeService`: фильтрация по `favorites=true`, поле `isFavorite` в результате
- [x] 5.2 Integration-тесты репозитория: `AddFavorite`, `RemoveFavorite`, идемпотентность, каскадное удаление при удалении рецепта/пользователя
- [x] 5.3 Microservice-тесты: `POST /api/v1/recipes/{id}/favorites` (авторизован / не авторизован), `DELETE /api/v1/recipes/{id}/favorites`, `GET /api/v1/recipes?favorites=true`

## 6. Seed-данные

- [x] 6.1 Добавить в `CookbookSeeder` записи `user_favorites` для `ivlev@cookbook.local` и `renat@cookbook.local` (по 3–5 рецептов каждому)

## 7. Frontend BFF

- [x] 7.1 Добавить поле `isFavorite` в Zod-схему `RecipeShortDtoSchema` в `recipes.ts`
- [x] 7.2 Добавить параметр `favorites` в `getRecipes` BFF-функцию
- [x] 7.3 Создать `apps/web/lib/bff/favorites.ts` с функциями `addFavorite(recipeId)` и `removeFavorite(recipeId)`
- [x] 7.4 Создать Route Handler `apps/web/app/api/recipes/[id]/favorites/route.ts` (POST/DELETE → backend)
- [x] 7.5 Unit-тесты: `favorites.bff.test.ts` (mock fetch), `recipes.schema.test.ts` (поле `isFavorite`)

## 8. Frontend UI

Реализация по макету `docs/design/mockup/index.html` и токенам `docs/design/mockup/styles.css`. Компоненты разрабатываются в Storybook (`docs/design/storybook/`) перед интеграцией в `apps/web`.

- [x] 8.1 Добавить переключатель «Все рецепты» / «Избранное» в сайдбар страницы рецептов (`.aside-item[data-mode]`, только для авторизованных)
- [x] 8.2 Добавить иконку-сердечко на карточку рецепта (Client Component, класс `btn-icon photo-fav`, `is-on` при активном, только для авторизованных)
- [x] 8.3 Добавить кнопку «В избранное» / «В избранном» на детальную страницу рецепта (`.detail-actions`, `btn-primary` / `btn-ghost`)
- [x] 8.4 Реализовать toggle-логику: клик вызывает Route Handler, обновляет состояние (без оптимистичного обновления)
- [x] 8.5 При активном режиме «Избранное» передавать `favorites=true` в BFF; отображать пустое состояние (`.state` «В избранном пусто» / «Сохраните любимое»)
- [x] 8.6 Добавить/обновить Storybook-истории для `RecipeCard` (состояния `isFavorite: true/false`) и сайдбара

## 9. E2E тесты

- [x] 9.1 E2E API-тест: добавить/удалить избранное, получить список с `favorites=true` (`tests/e2e/test_favorites_api.py`)
- [x] 9.2 UI E2E-тест: переключение режима «Избранное», иконка на карточке (`tests/ui/test_favorites.py`)
