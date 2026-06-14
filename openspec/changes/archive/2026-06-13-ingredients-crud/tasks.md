# ingredients-crud

## 1. OpenAPI-контракт

- [x] 1.1 Добавить схемы `IngredientDto`, `CreateIngredientRequest`, `UpdateIngredientRequest`, `IngredientCategory` в `docs/contracts/cookbook/recipes.yaml`
- [x] 1.2 Добавить эндпоинты `/ingredients` (GET, POST) и `/ingredients/{id}` (GET, PUT, DELETE) в контракт

## 2. Backend: Domain

- [x] 2.1 Создать `IngredientCategory.cs` (enum: Vegetables, FruitsAndBerries, MeatAndPoultry, FishAndSeafood, DairyAndEggs, GrainsAndCereals, Legumes, NutsAndSeeds, OilsAndFats, SpicesAndSeasonings, SaucesAndPastes, BakeryAndSweets, Other)
- [x] 2.2 Создать `IngredientId.cs` (typed value object, AR-0019)
- [x] 2.3 Создать `IngredientConstraints.cs` (TitleMinLength=2, TitleMaxLength=200, UnitMaxLength=20, DefaultAmountMin, DefaultAmountMax=100000)
- [x] 2.4 Создать `Ingredient.cs` (агрегат с `Create` / `Update`, валидация через исключения)
- [x] 2.5 Создать типизированные исключения: `IngredientDomainException`, `IngredientTitleTooShortException`, `IngredientTitleTooLongException`, `IngredientUnitTooLongException`, `IngredientDefaultAmountOutOfRangeException`, `IngredientNotFoundException`

## 3. Backend: Application

- [x] 3.1 Создать `IIngredientRepository.cs` (GetAllAsync с фильтрами, GetByIdAsync, CreateAsync, UpdateAsync, DeleteAsync)
- [x] 3.2 Создать `IIngredientService.cs`
- [x] 3.3 Создать `IngredientService.cs` (реализация, AR-0037 — CommitAsync в конце каждого метода)

## 4. Backend: Adapters — PostgreSQL

- [x] 4.1 Создать `IngredientConfiguration.cs` в `Adapters/Postgresql/Configurations/` (AR-0051)
- [x] 4.2 Добавить `DbSet<Ingredient>` в `RecipeRepository` (AR-0048 — один DbContext)
- [x] 4.3 Добавить методы репозитория для `Ingredient` в `RecipeRepository.cs`
- [x] 4.4 Создать EF Core миграцию для таблицы `cookbook.ingredients`
- [x] 4.5 Создать `IngredientSeeder.cs` с 50+ системными ингредиентами (upsert, AR-0054)
- [x] 4.6 Вызвать `IngredientSeeder` из `Program.cs` после миграции

## 5. Backend: Adapters — Web

- [x] 5.1 Создать `IngredientDto.cs` и `IngredientRequest.cs` в `Adapters/Web/Dto/`
- [x] 5.2 Создать `IngredientsController.cs` с CRUD-эндпоинтами и явным перехватом `IngredientDomainException`

## 6. Backend: Тесты

- [x] 6.1 Unit-тесты `Ingredient.Create` — валидные и невалидные данные (`Unit/Domain/`)
- [x] 6.2 Integration-тесты репозитория — CRUD операции (`Integration/Adapters/Postgresql/`)
- [x] 6.3 Microservice-тесты контроллера — все эндпоинты через WebApplicationFactory + Testcontainers (`Microservice/`)

## 7. Frontend: Zod-схемы и BFF

- [x] 7.1 Создать `apps/web/lib/schemas/ingredient.ts` (IngredientSchema, CreateIngredientSchema, UpdateIngredientSchema, IngredientCategory enum)
- [x] 7.2 Создать `apps/web/lib/bff/ingredients.ts` (getIngredients, getIngredient, createIngredient, updateIngredient, deleteIngredient)

## 8. Frontend: UI

- [x] 8.1 Создать страницу `apps/web/app/ingredients/page.tsx` (список с фильтрами по названию и категории)
- [x] 8.2 Реализовать модальное окно создания/редактирования ингредиента
- [x] 8.3 Реализовать диалог подтверждения удаления
- [x] 8.4 Добавить пункт «Ингредиенты» в навигацию (`apps/web/components/features/Header`)

## 9. Frontend: Тесты

- [x] 9.1 Unit-тесты Zod-схем `ingredient.ts`
- [x] 9.2 Unit-тесты BFF-функций с мокированным fetch

## 10. E2E API тесты

- [x] 10.1 Добавить тест `test_ingredients_api.py` в `tests/e2e/`: GET список, POST создание, GET по id, PUT обновление, DELETE удаление
- [x] 10.2 Покрыть сценарии валидации: создание с пустым названием, слишком коротким названием, некорректным количеством

## 11. UI E2E тесты (Playwright)

- [x] 11.1 Добавить тест в `tests/ui/`: открытие страницы `/ingredients`, отображение списка
- [x] 11.2 Тест создания ингредиента через форму
- [x] 11.3 Тест редактирования ингредиента
- [x] 11.4 Тест удаления ингредиента с подтверждением
- [x] 11.5 Тест фильтрации по названию и категории
