# ingredients-crud

## Контекст

Сервис `recipes` (bounded context `cookbook`) уже содержит агрегат `Recipe` с гексагональной архитектурой (Domain / Application / Adapters). Ингредиенты в системе отсутствуют — нет ни доменной модели, ни API, ни UI. Изменение добавляет новый агрегат `Ingredient` в тот же сервис и ту же БД (`recipes`, схема `cookbook`).

Действующие ADR, ограничивающие дизайн: ADR-0006 (REST), ADR-0011 (.NET 10), ADR-0012 (DDD), ADR-0013 (Hexagonal), ADR-0014 (EF Core), ADR-0015 (Next.js), ADR-0016 (React+TS), ADR-0017 (BFF), ADR-0019 (Zod), ADR-0023 (Contract-First), ADR-0031 (чистый CSS из Storybook).

## Цели / Не-цели

**Цели:**

- Добавить агрегат `Ingredient` с полями `id`, `title`, `unit`, `default_amount`, `category`, `is_system`.
- Реализовать CRUD-эндпоинты: `GET /api/v1/ingredients`, `POST /api/v1/ingredients`, `GET /api/v1/ingredients/{id}`, `PUT /api/v1/ingredients/{id}`, `DELETE /api/v1/ingredients/{id}`.
- Фильтрация списка по названию (`title`) и категории (`category`) через query-параметры.
- Обновить OpenAPI-контракт `docs/contracts/cookbook/recipes.yaml` до реализации (AR-0015).
- Реализовать страницу `/ingredients` по макету из `docs/design/mockup/`.
- Seed-данные: 50+ системных ингредиентов.
- Покрыть сценарии из specs тестами.

**Не-цели:**

- Авторизация и разграничение прав (системный/пользовательский, `author_id`) — после auth-service.
- Проверка использования ингредиента в рецептах при удалении — после реализации ингредиентов в рецепте.
- Пагинация каталога ингредиентов — после реализации пагинации рецептов.

## Решения

### 1. Размещение агрегата — в существующем сервисе `recipes`

`Ingredient` добавляется в сервис `recipes` (bounded context `cookbook`), а не в отдельный сервис. Обоснование: ингредиенты тесно связаны с рецептами (будут использоваться как вложенные сущности), разделение на отдельный сервис преждевременно для учебного проекта. Та же БД `recipes`, схема `cookbook`.

### 2. Contract-First: обновление `recipes.yaml`

Контракт `docs/contracts/cookbook/recipes.yaml` расширяется новыми схемами (`IngredientDto`, `CreateIngredientRequest`, `UpdateIngredientRequest`) и эндпоинтами `/ingredients`. Один файл контракта на bounded context — соответствует стандарту `api-design.md`.

`IngredientCategory` моделируется как enum со строковыми значениями в нижнем регистре (стандарт API).

### 3. Структура CRUD-эндпоинтов

```
GET    /api/v1/ingredients          — список с фильтрацией
POST   /api/v1/ingredients          — создание
GET    /api/v1/ingredients/{id}     — детальный просмотр
PUT    /api/v1/ingredients/{id}     — полное обновление
DELETE /api/v1/ingredients/{id}     — удаление
```

Маршрутизация через gateway: `/api/cookbook/ingredients/**` → `recipes-service/api/ingredients/**` (существующий маршрут `cookbook` уже настроен).

### 4. Валидация — только в домене

Паттерн идентичен `Recipe`: доменный метод `Ingredient.Create` / `Ingredient.Update` выбрасывает типизированные исключения при нарушении инвариантов. Контроллер явно перехватывает `IngredientDomainException` и формирует `400 Bad Request` с `application/problem+json`. Соответствует AR-0034, AR-0035, AR-0036.

Ограничения полей (`IngredientConstraints`): `TitleMinLength = 2`, `TitleMaxLength = 200`, `UnitMaxLength = 20`, `DefaultAmountMin = 0.001m`, `DefaultAmountMax = 100_000m`. Соответствует AR-0039.

### 5. EF Core: отдельная конфигурация и миграция

`IngredientConfiguration` реализует `IEntityTypeConfiguration<Ingredient>` (AR-0051). Новая миграция добавляет таблицу `cookbook.ingredients`. Seed-данные загружаются через отдельный класс-загрузчик `IngredientSeeder` по принципу upsert (AR-0054).

### 6. Frontend: страница `/ingredients`

Страница реализуется по макету `docs/design/mockup/index.html` (секция `views.ingredients`). Форма создания/редактирования — модальное окно. Компоненты и стили — из `docs/design/storybook/src/` (ADR-0031).

BFF-функции в `apps/web/lib/bff/ingredients.ts`. Zod-схемы в `apps/web/lib/schemas/ingredient.ts`.

## Риски / Компромиссы

- **[Компромисс] Один файл контракта на bounded context** → при росте числа ресурсов файл станет большим; приемлемо для текущего масштаба.
- **[Компромисс] PUT вместо PATCH** → полное обновление проще в реализации; достаточно для текущего набора полей.
- **[Риск] Удаление без проверки использования** → временно допустимо, пока ингредиенты не используются в рецептах; проверка добавляется в следующем изменении.

## План миграции

1. Обновить OpenAPI-контракт `docs/contracts/cookbook/recipes.yaml`.
2. Добавить доменную модель `Ingredient`, ограничения `IngredientConstraints`, типизированные исключения.
3. Добавить `IIngredientRepository`, `IIngredientService`, реализации.
4. Создать EF Core миграцию, `IngredientConfiguration`, `IngredientSeeder`.
5. Добавить `IngredientsController`.
6. Обновить BFF и Zod-схемы.
7. Реализовать страницу `/ingredients`.
8. Написать тесты.

Откат: `dotnet ef database update <предыдущая миграция>`.

## Открытые вопросы

Нет.
