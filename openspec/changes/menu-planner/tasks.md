# menu-planner

## 1. OpenAPI-контракт

- [ ] 1.1 Описать схемы `MealPlanDto`, `MealPlanSlotDto`, `MealPlanItemDto` в `docs/contracts/cookbook/cookbook.yaml`
- [ ] 1.2 Описать эндпоинт `GET /api/v1/meal-plan` (200, 401)
- [ ] 1.3 Описать эндпоинт `PUT /api/v1/meal-plan` (200, 400, 401)
- [ ] 1.4 Описать эндпоинт `DELETE /api/v1/meal-plan` (204, 401)

## 2. Backend: доменная модель

- [ ] 2.1 Создать enum `WeekDay` (Monday=1..Sunday=7) в Domain
- [ ] 2.2 Создать enum `MealType` (Breakfast=1, Lunch=2, Dinner=3) в Domain
- [ ] 2.3 Создать Value Object `MealPlanId` (`readonly record struct`)
- [ ] 2.4 Создать Value Object `Servings` с валидацией диапазона 1–99
- [ ] 2.5 Создать константы `MealPlanConstraints` (MinServings, MaxServings)
- [ ] 2.6 Создать сущность `MealPlanItem` (recipe_id, servings)
- [ ] 2.7 Создать сущность `MealPlanSlot` (WeekDay, MealType, коллекция MealPlanItem)
- [ ] 2.8 Создать агрегат `MealPlan` с операциями: AddItem, RemoveItem, UpdateServings, Clear, Replace

## 3. Backend: Application-слой

- [ ] 3.1 Создать интерфейс `IMealPlanRepository` с методами `GetByUserIdAsync`, `SaveAsync`
- [ ] 3.2 Создать `MealPlanService` с методами `GetAsync` (lazy init), `ReplaceAsync`, `ClearAsync`
- [ ] 3.3 Создать DTO-модели запроса/ответа для API (MealPlanDto, MealPlanSlotDto, MealPlanItemDto)

## 4. Backend: адаптер PostgreSQL

- [ ] 4.1 Создать EF Core конфигурацию `MealPlanConfiguration`
- [ ] 4.2 Создать EF Core конфигурацию `MealPlanSlotConfiguration`
- [ ] 4.3 Создать EF Core конфигурацию `MealPlanItemConfiguration`
- [ ] 4.4 Добавить `DbSet` в существующий `DbContext`
- [ ] 4.5 Создать реализацию `MealPlanRepository`
- [ ] 4.6 Создать миграцию EF Core (таблицы `meal_plan_slots`, `meal_plan_items`, уникальный индекс)

## 5. Backend: HTTP-контроллер

- [ ] 5.1 Создать `MealPlanController` с эндпоинтами GET, PUT, DELETE `/api/v1/meal-plan`
- [ ] 5.2 Добавить маршрут на API Gateway (`/api/cookbook/v1/meal-plan`)

## 6. Backend: seed-данные

- [ ] 6.1 Добавить готовый план меню на неделю для seed-пользователя в `CookbookSeeder`

## 7. Backend: тесты

- [ ] 7.1 Unit-тесты агрегата `MealPlan` (`MealPlanTests`)
- [ ] 7.2 Unit-тесты `MealPlanService` с замоканным репозиторием (`MealPlanServiceTests`)
- [ ] 7.3 Integration-тесты репозитория (`MealPlanRepositoryTests`)
- [ ] 7.4 Microservice-тесты `GET /api/v1/meal-plan` (`GetMealPlanTests`)
- [ ] 7.5 Microservice-тесты `PUT /api/v1/meal-plan` (`ReplaceMealPlanTests`)
- [ ] 7.6 Microservice-тесты `DELETE /api/v1/meal-plan` (`ClearMealPlanTests`)

## 8. Storybook: компоненты планировщика

- [ ] 8.1 Добавить компонент `PlannerRecipeCard` в `src/domain/` (миниатюра + название, draggable) по макету `docs/design/mockup/index.html` (`.planner-recipe`)
- [ ] 8.2 Добавить компонент `PlannerSlot` в `src/domain/` (слот сетки, dragover-подсветка) по макету (`.planner-slot`, `.planner-slot-item`)
- [ ] 8.3 Добавить компонент `PlannerGrid` в `src/domain/` (сетка 7×3 с заголовками) по макету (`.planner-grid`)
- [ ] 8.4 Добавить компонент `PlannerPanel` в `src/domain/` (горизонтальный скролл + переключатели + поиск) по макету (`.planner-panel`)
- [ ] 8.5 Добавить story `Planner.stories.tsx` с Playground в `src/stories/`
- [ ] 8.6 Добавить mock-данные плана меню в `src/mocks.ts`

## 9. Frontend: BFF

- [ ] 9.1 Создать Zod-схемы для MealPlan (запрос/ответ) в `lib/bff/meal-plan.ts`
- [ ] 9.2 Реализовать `getMealPlan()` (Server Component)
- [ ] 9.3 Реализовать Server Actions `updateMealPlan()` и `clearMealPlan()`

## 10. Frontend: страница планировщика

- [ ] 10.1 Создать страницу `/planner` (Client Component, защита авторизацией)
- [ ] 10.2 Реализовать панель выбора рецептов (горизонтальный скролл, переключатели Все/Избранное/Мои, поиск) по макету и Storybook-компоненту `PlannerPanel`
- [ ] 10.3 Реализовать сетку 7×3 по макету и Storybook-компоненту `PlannerGrid`
- [ ] 10.4 Подключить `@dnd-kit/core`: `DndContext`, `useDraggable`, `useDroppable`, `DragOverlay`
- [ ] 10.5 Реализовать карточку блюда в слоте (название, поле порций, кнопка удаления) по макету (`.planner-slot-item`)
- [ ] 10.6 Реализовать автосохранение с debounce 300 мс
- [ ] 10.7 Реализовать кнопку «Очистить всё» с диалогом подтверждения

## 11. Frontend: навигация

- [ ] 11.1 Добавить пункт «Планировщик» в компонент шапки (только для авторизованных)

## 12. Frontend: тесты

- [ ] 12.1 Unit-тесты Zod-схем (`meal-plan.schema.test.ts`)
- [ ] 12.2 Unit-тесты BFF-функций с мокированным fetch (`meal-plan.bff.test.ts`)

## 13. E2E-тесты

- [ ] 13.1 E2E API-тесты планировщика (`tests/e2e/test_meal_plan_api.py`)
- [ ] 13.2 UI E2E-тесты планировщика (`tests/ui/test_planner.py`)
