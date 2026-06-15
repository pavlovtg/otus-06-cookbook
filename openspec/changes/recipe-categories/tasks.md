# recipe-categories

## 1. OpenAPI-контракт

- [x] 1.1 Добавить `categoryIds: uuid[]` (required) в `RecipeRequest` в `docs/contracts/cookbook/recipes.yaml`
- [x] 1.2 Добавить `categoryIds: uuid[]` в `RecipeDto` и `RecipeShortDto` в `docs/contracts/cookbook/recipes.yaml`

## 2. Домен (backend)

- [ ] 2.1 Добавить `IReadOnlyList<CategoryId> CategoryIds` в агрегат `Recipe`
- [ ] 2.2 Реализовать валидацию «один тип — одна категория» в `Recipe.Create` и `Recipe.Update` (принимает словарь `CategoryId → CategoryType`)
- [ ] 2.3 Добавить доменные исключения: `RecipeDuplicateCategoryTypeException`
- [ ] 2.4 Написать unit-тесты домена: отклоняет дубликат типа; принимает пустой список; заменяет при совпадении типа

## 3. Инфраструктура (backend)

- [ ] 3.1 Создать EF-миграцию: join-таблица `recipe_categories (recipe_id, category_id)`, FK на `recipes` и `categories`, PK составной
- [ ] 3.2 Добавить `RecipeCategoryConfiguration : IEntityTypeConfiguration` и зарегистрировать в `RecipeRepository.OnModelCreating`
- [ ] 3.3 Обновить `RecipeRepository.GetByIdAsync` и `GetByIdWithDetailsAsync`: добавить `.Include(r => r.CategoryIds)` (или загрузку join-таблицы)
- [ ] 3.4 Реализовать `RecipeRepository.IsUsedInRecipesAsync` (убрать TODO)
- [ ] 3.5 Написать integration-тест репозитория: сохранение и загрузка `recipe_categories` через EF Core

## 4. Application + API (backend)

- [ ] 4.1 Обновить `IRecipeService.CreateAsync` и `UpdateAsync`: добавить параметр `IEnumerable<CategoryId> categoryIds`
- [ ] 4.2 Обновить `RecipeService.CreateAsync` и `UpdateAsync`: загрузить типы категорий из `ICategoryRepository`, передать словарь в `Recipe.Create` / `Recipe.Update`; выбросить `RecipeDomainException` при несуществующем `categoryId`
- [ ] 4.3 Обновить `RecipeRequest`: добавить `IReadOnlyList<Guid> CategoryIds`
- [ ] 4.4 Обновить `RecipeDto`: добавить `IReadOnlyList<Guid> CategoryIds`
- [ ] 4.5 Обновить `RecipeShortDto`: добавить `IReadOnlyList<Guid> CategoryIds`
- [ ] 4.6 Обновить `RecipesController.CreateRecipe` и `UpdateRecipe`: передавать `categoryIds` в `RecipeService`; `RecipeDomainException` → `400`
- [ ] 4.7 Обновить `RecipesController.ToShortDto` и `ToDto`: включить `CategoryIds` в ответ
- [ ] 4.8 Написать microservice-тесты: `POST /api/v1/recipes` с `categoryIds` → `201`; `PUT /api/v1/recipes/{id}` обновляет; `GET /api/v1/recipes/{id}` возвращает `categoryIds`; несуществующий `categoryId` → `400`

## 5. Seed-данные (backend)

- [ ] 5.1 Обновить `CookbookSeeder` / `SeedData`: назначить категории существующим рецептам (идемпотентно)

## 6. BFF (frontend)

- [ ] 6.1 Обновить Zod-схемы `RecipeDto` и `RecipeShortDto`: добавить `categoryIds: z.array(z.string().uuid())`
- [ ] 6.2 Обновить Zod-схему `RecipeRequest`: добавить `categoryIds: z.array(z.string().uuid())`
- [ ] 6.3 На странице списка рецептов: загружать `GET /api/v1/categories` параллельно с рецептами (`Promise.all`)
- [ ] 6.4 Инвалидировать кеш `['categories']` при мутациях каталога категорий (создание, редактирование, удаление)

## 7. UI-компоненты (frontend)

- [ ] 7.1 Реализовать компонент `CategoryTagInput` (`.tag-input` + `.chip` + `.autocomplete`): поиск по справочнику, добавление чипа, удаление чипа, замена при совпадении типа
- [ ] 7.2 Добавить `CategoryTagInput` в форму создания/редактирования рецепта
- [ ] 7.3 Отображать теги категорий (`.tags` + `.tag`) в `RecipeCard` (до 3 тегов)
- [ ] 7.4 Отображать теги категорий на детальной странице рецепта (все теги)
- [ ] 7.5 Добавить Storybook-истории: `RecipeCard` с тегами и без; `CategoryTagInput` — добавление, удаление, замена типа

## 8. Тесты E2E

- [ ] 8.1 E2E: создать рецепт с категориями → категории отображаются в карточке и на детальной странице
- [ ] 8.2 E2E: редактировать рецепт → категории обновляются
