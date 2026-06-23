# menu-planner

## Контекст

Планировщик меню — новая функциональность. Пользователь перетаскивает рецепты из панели выбора в слоты недельной сетки (7 дней × 3 приёма пищи = 21 слот). В каждый слот можно добавить несколько блюд. План сохраняется на сервере и привязан к аккаунту пользователя.

Стек зафиксирован: recipes-сервис (.NET 10, DDD, гексагональная архитектура), PostgreSQL (схема `cookbook`), Next.js BFF + React Client Components, `@dnd-kit/core` (ADR-0036).

## Цели / Не-цели

**Цели:**

- Хранить план меню пользователя на сервере.
- Страница `/planner`: панель рецептов + сетка с drag-and-drop.
- Автосохранение при каждом изменении слота.
- Пункт «Планировщик» в навигации (только авторизованные).

**Не-цели:**

- Список покупок — отдельный change.
- Несколько планов меню на пользователя.
- Совместное редактирование плана.

## Решения

### Доменная модель (Backend)

**Агрегат `MealPlan`**

- Корень агрегата — `MealPlan` с идентификатором `MealPlanId` (типизированный VO, `readonly record struct`).
- Содержит коллекцию `MealPlanSlot` (21 слот: `WeekDay` × `MealType`).
- Каждый `MealPlanSlot` содержит коллекцию `MealPlanItem` (recipe_id, servings).
- `WeekDay` — нумерованный enum: `Monday = 1`, `Tuesday = 2`, ..., `Sunday = 7`.
- `MealType` — нумерованный enum: `Breakfast = 1`, `Lunch = 2`, `Dinner = 3`.
- `Servings` — Value Object с валидацией диапазона 1–99.
- Доменные ограничения в `MealPlanConstraints`: `MinServings = 1`, `MaxServings = 99`.

**Доменные операции:**

- `MealPlan.AddItem(weekDay, mealType, recipeId, servings)` — добавить блюдо в слот.
- `MealPlan.RemoveItem(weekDay, mealType, itemId)` — удалить блюдо из слота.
- `MealPlan.UpdateServings(weekDay, mealType, itemId, servings)` — изменить порции.
- `MealPlan.Clear()` — очистить все слоты.
- `MealPlan.Replace(slots)` — заменить весь план (используется при PUT).

**Репозиторий:**

- Интерфейс `IMealPlanRepository` в Application-слое.
- Методы: `GetByUserIdAsync(userId, ct)`, `SaveAsync(mealPlan, ct)`.
- Реализация `MealPlanRepository : DbContext, IMealPlanRepository` в `Adapters/Postgresql/`.
- EF Core конфигурация: `MealPlanConfiguration`, `MealPlanSlotConfiguration`, `MealPlanItemConfiguration` в `Adapters/Postgresql/Configurations/`.
- Миграция в `Adapters/Postgresql/Migrations/`.

**Application-сервис `MealPlanService`:**

- `GetAsync(userId, ct)` — получить план; если не существует — создать пустой (lazy init в транзакции).
- `ReplaceAsync(userId, slots, ct)` — заменить весь план; вызывает `CommitAsync` (AR-0037).
- `ClearAsync(userId, ct)` — очистить план; вызывает `CommitAsync`.

### Модель данных (PostgreSQL)

```
meal_plan_slots (
  id uuid PK,
  user_id uuid NOT NULL,
  week_day smallint NOT NULL,   -- WeekDay enum: 1=Monday..7=Sunday
  meal_type smallint NOT NULL,  -- MealType enum: 1=Breakfast, 2=Lunch, 3=Dinner
  UNIQUE (user_id, week_day, meal_type)
)

meal_plan_items (
  id uuid PK,
  slot_id uuid NOT NULL FK → meal_plan_slots,
  recipe_id uuid NOT NULL,
  servings smallint NOT NULL
)
```

Уникальный индекс по `(user_id, week_day, meal_type)` гарантирует ровно один слот на комбинацию пользователь × день × приём пищи.

Альтернатива — хранить план как JSON в одной строке: проще, но теряем возможность JOIN с рецептами на стороне БД и усложняем будущий список покупок.

### API

Три эндпоинта в recipes-сервисе (bounded context `cookbook`):

- `GET /api/v1/meal-plan` — получить план текущего пользователя.
- `PUT /api/v1/meal-plan` — заменить весь план (идемпотентно).
- `DELETE /api/v1/meal-plan` — очистить план.

`PUT` принимает полный снимок плана (все 21 слот). Это упрощает клиент: не нужно отслеживать diff, достаточно отправить текущее состояние после каждого изменения.

Альтернатива — PATCH по отдельному слоту: меньше трафика, но сложнее клиент и сложнее обеспечить консистентность при быстрых изменениях.

### Frontend

- Страница `/planner` — Client Component (требует DnD и интерактивность).
- BFF `lib/bff/meal-plan.ts` — `getMealPlan()` (Server Component), `updateMealPlan()` / `clearMealPlan()` (Server Actions).
- DnD: `DndContext` на уровне страницы, `useDraggable` на карточке рецепта в панели, `useDroppable` на каждом слоте, `DragOverlay` для floating-карточки.
- Панель рецептов: горизонтальный скролл, переключатели «Все / Избранное / Мои», поиск.
- Автосохранение: после каждого drop/remove/servings-change вызывается Server Action `updateMealPlan` с debounce 300 мс.

### Навигация

Пункт «Планировщик» добавляется в компонент шапки. Отображается только авторизованным пользователям — через серверную проверку сессии в Server Component шапки.

## Риски / Компромиссы

- **PUT всего плана при каждом изменении** → при медленном соединении возможны гонки. Митигация: debounce 300 мс на клиенте; Server Action идемпотентен.
- **Lazy init 21 слота** → первый GET чуть медленнее. Митигация: создание слотов в одной транзакции, уникальный индекс по `(user_id, week_day, meal_type)`.
- **Client Component для всей страницы** → нет SSR для содержимого планировщика. Принято: DnD требует клиентского состояния; начальные данные загружаются через Server Action при монтировании.

## План тестирования

### Unit-тесты (xUnit + NSubstitute)

Расположение: `Recipes.Tests/Unit/Domain/MealPlan/`

- `MealPlanTests` — доменные операции агрегата: добавление/удаление блюда, изменение порций, очистка, валидация `Servings` (граничные значения 1 и 99).
- `MealPlanServiceTests` — Application-сервис с замоканным `IMealPlanRepository`: get (существующий / несуществующий план), replace, clear.

### Integration-тесты (xUnit + Testcontainers)

Расположение: `Recipes.Tests/Integration/Adapters/Postgresql/`

- `MealPlanRepositoryTests` — CRUD через реальный PostgreSQL в контейнере: сохранение нового плана, обновление, чтение, lazy init слотов.

### Microservice-тесты (xUnit + WebApplicationFactory + Testcontainers)

Расположение: `Recipes.Tests/Microservice/`

- `GetMealPlanTests` — `GET /api/v1/meal-plan`: авторизованный (пустой план, заполненный план), неавторизованный (401).
- `ReplaceMealPlanTests` — `PUT /api/v1/meal-plan`: добавление блюда, изменение порций, удаление блюда, несколько блюд в слоте, неавторизованный (401).
- `ClearMealPlanTests` — `DELETE /api/v1/meal-plan`: очистка заполненного плана, очистка пустого плана, неавторизованный (401).

### Frontend unit-тесты (Vitest + Testing Library)

- `meal-plan.schema.test.ts` — Zod-схемы запроса/ответа планировщика.
- `meal-plan.bff.test.ts` — BFF-функции с мокированным fetch.

### E2E API-тесты (pytest + httpx)

Файл: `tests/e2e/test_meal_plan_api.py`

- Сценарии из specs: получение пустого плана, добавление блюда, изменение порций, очистка, доступ без авторизации.

### UI E2E-тесты (Playwright)

Файл: `tests/ui/test_planner.py`

- Открытие страницы `/planner` авторизованным пользователем.
- Drag-and-drop рецепта в слот, проверка отображения.
- Изменение порций в слоте.
- Очистка плана через кнопку «Очистить всё».
