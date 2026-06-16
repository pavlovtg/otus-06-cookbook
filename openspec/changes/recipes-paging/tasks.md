# recipes-paging

## 1. Контракт API

- [x] 1.1 Обновить `docs/contracts/cookbook/recipes.yaml`: добавить параметры `page` (default: 1) и `pageSize` (default: 18, max: 1000) к `GET /api/v1/recipes`; добавить схему `PagedResultRecipeShortDto` (поля: `items`, `total`, `page`, `pageSize`); изменить тип ответа с `array` на `PagedResultRecipeShortDto`

## 2. Backend — Application

- [x] 2.1 Добавить метод `GetRecipesPagedAsync(int page, int pageSize, CancellationToken)` в `IRecipeRepository`, возвращающий `Task<PagedResult<Recipe>>`
- [x] 2.2 Добавить метод `GetRecipesPagedAsync(int page, int pageSize, CancellationToken)` в `IRecipeService`
- [x] 2.3 Реализовать метод в `RecipeService`: валидация (page ≤ 0 или pageSize ≤ 0 → исключение), clamp pageSize до 1000, делегирование в репозиторий

## 3. Backend — Адаптер PostgreSQL

- [x] 3.1 Реализовать `GetRecipesPagedAsync` в `RecipeRepository`: два запроса — `COUNT(*)` для `total` и `SELECT ... LIMIT/OFFSET` для страницы данных

## 4. Backend — Web-адаптер

- [x] 4.1 Изменить `RecipesController.GetRecipes`: принять `[FromQuery] int page = 1, [FromQuery] int pageSize = 18`; вернуть `PagedResult<RecipeShortDto>` вместо `IAsyncEnumerable`

## 5. Frontend — BFF и схемы

- [x] 5.1 Добавить `RecipePagedResultSchema` в `apps/web/lib/schemas/recipe.ts`
- [x] 5.2 Обновить `getRecipes` в `apps/web/lib/bff/recipes.ts`: принять `page` и `pageSize`, передать в запрос, вернуть `PagedResult<RecipeShortDto>`
- [x] 5.3 Обновить Route Handler `apps/web/app/api/cookbook/v1/recipes/route.ts`: пробросить `page` и `pageSize` из query params в upstream

## 6. Frontend — UI

- [x] 6.1 Перенести компонент `Pagination` из Storybook DS (`docs/design/storybook/src/components/Pagination`) в `apps/web/components/ui/Pagination.tsx`; стили `.pagination` / `.page-btn` уже есть в `apps/web/app/globals.css` (из `docs/design/mockup/styles.css`)
- [x] 6.2 Обновить `apps/web/app/(public)/page.tsx`: читать `page` из searchParams, передавать в `getRecipes`, отображать общее количество рецептов (`total`)
- [x] 6.3 Подключить `Pagination` на странице списка рецептов; при смене страницы обновлять URL (`?page=N`) через `router.push`

## 7. Тесты

- [x] 7.1 Добавить unit-тест `apps/web/tests/bff/recipes.bff.test.ts`: проверить передачу `page`/`pageSize` и парсинг `PagedResult` через Zod-схему
- [x] 7.2 Обновить e2e-тест `tests/e2e/test_recipes_api.py`: проверить пагинацию — ответ содержит `items`, `total`, `page`, `pageSize`; запрос второй страницы возвращает другой набор рецептов
- [x] 7.3 Добавить integration-тест в `apps/Cookbook/tests/Recipes.Tests`: проверить `GetRecipesPagedAsync` с реальной БД (Testcontainers) — корректный `total`, корректная страница
- [x] 7.4 Добавить UI e2e-тест `tests/ui/test_recipes.py`: открыть список рецептов, убедиться что пагинация отображается, нажать кнопку следующей страницы, проверить что отображается другой набор карточек
