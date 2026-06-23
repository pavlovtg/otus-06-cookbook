# shopping-list

## 1. Контракт API

- [ ] 1.1 Добавить эндпоинт `GET /api/v1/shopping-list` в `docs/contracts/cookbook/recipes.yaml` — описание запроса, ответа `ShoppingListGroupDto[]`, кодов 200/401

## 2. Backend: Application

- [ ] 2.1 Создать `ShoppingListItemView.cs` — record с полями `IngredientId`, `Title`, `Amount`, `Unit`
- [ ] 2.2 Создать `ShoppingListGroupView.cs` — record с полями `Category`, `Items`
- [ ] 2.3 Создать `Ports/IShoppingListRepository.cs` — метод `GetShoppingListAsync(UserId, CancellationToken)`
- [ ] 2.4 Создать `ShoppingListService.cs` — метод `GetAsync(UserId, CancellationToken)`, агрегация в памяти

## 3. Backend: Adapters / Postgresql

- [ ] 3.1 Добавить `IShoppingListRepository` в список интерфейсов `RecipeRepository`
- [ ] 3.2 Реализовать `GetShoppingListAsync`: загрузка плана с Include `MealPlanSlots → MealPlanItems → Recipe → RecipeIngredients → Ingredient`
- [ ] 3.3 Агрегация: группировка по `IngredientId`, суммирование `amount * (item.Servings / recipe.Servings)`
- [ ] 3.4 Группировка по `Ingredient.Category`, сортировка групп по порядку `IngredientCategory` enum, внутри — по `Ingredient.Title`

## 4. Backend: Adapters / Web

- [ ] 4.1 Создать `Dto/ShoppingListItemDto.cs`
- [ ] 4.2 Создать `Dto/ShoppingListGroupDto.cs`
- [ ] 4.3 Создать `ShoppingListController.cs` — `GET /api/v1/shopping-list`, `[Authorize]`, маппинг view → DTO
- [ ] 4.4 Зарегистрировать `ShoppingListService` в DI

## 5. Backend: Тесты

- [ ] 5.1 Integration-тест `GetShoppingListAsync`: корректная агрегация (несколько слотов, несколько рецептов)
- [ ] 5.2 Integration-тест `GetShoppingListAsync`: пересчёт количества при нестандартных порциях
- [ ] 5.3 Integration-тест `GetShoppingListAsync`: дедупликация одинаковых ингредиентов из разных слотов
- [ ] 5.4 Integration-тест `GetShoppingListAsync`: пустой план → пустой список
- [ ] 5.5 Integration-тест `GetShoppingListAsync`: группировка и сортировка
- [ ] 5.6 Microservice-тест: `GET /api/v1/shopping-list` → 200 с заполненным планом
- [ ] 5.7 Microservice-тест: `GET /api/v1/shopping-list` → 200 с пустым массивом для пустого плана
- [ ] 5.8 Microservice-тест: `GET /api/v1/shopping-list` → 401 без авторизации

## 6. Frontend: Схемы и BFF

- [ ] 6.1 Создать `lib/schemas/shopping-list.ts` — `ShoppingListItemSchema`, `ShoppingListGroupSchema`
- [ ] 6.2 Создать `lib/bff/shopping-list.server.ts` — `getShoppingList()` через `serverFetch`
- [ ] 6.3 Создать `app/api/cookbook/v1/shopping-list/route.ts` — Route Handler (proxy `GET` к backend через YARP)

## 7. Frontend: Страница

- [ ] 7.1 Создать `app/shopping-list/page.tsx` — Server Component, загрузка данных через `getShoppingList()`
- [ ] 7.2 Рендер таблицы: `.shopping-table` → `.shopping-group-head` (категория, uppercase, `--accent-link-b`) → `.shopping-row` (название / `.qty` / `.unit`) — по макету `docs/design/mockup/styles.css`
- [ ] 7.3 Заголовок страницы: `Список <span class="t-gradient">покупок</span>`, кнопки `btn-ghost` (Скопировать, иконка `copy`) и `btn-primary` (Распечатать, иконка `print`) — по макету `docs/design/mockup/index.html` (`views.shoppingList`)
- [ ] 7.4 Состояние пустого списка: `.state` с сообщением «Добавьте блюда в планировщик» и кнопкой-ссылкой на `/planner`
- [ ] 7.5 Создать `app/shopping-list/ShoppingListActions.tsx` — Client Component с кнопками «Скопировать» и «Распечатать»
- [ ] 7.6 Реализовать копирование: `navigator.clipboard.writeText()` с текстовым форматом (категория → ингредиент, количество, единица)
- [ ] 7.7 Реализовать печать: `window.print()` (print-стили уже есть в `globals.css`: `.header`, `.btn` скрываются)
- [ ] 7.8 Добавить кнопку-ссылку «Список покупок» на страницу планировщика (`app/planner/`) — по макету: `btn-ghost btn-sm` с иконкой `cart`

## 8. Storybook

- [ ] 8.1 Добавить/обновить `src/domain/ShoppingList` в `docs/design/storybook/` — компонент принимает `ShoppingListGroup[]`, рендерит `.shopping-table` по макету
- [ ] 8.2 Добавить story `ShoppingList.stories.tsx` — состояния: заполненный список, пустой список

## 9. Frontend: Тесты

- [ ] 9.1 Unit-тест `ShoppingListGroupSchema`: валидация корректного ответа
- [ ] 9.2 Unit-тест `ShoppingListGroupSchema`: отклонение невалидного ответа
- [ ] 9.3 Unit-тест BFF `getShoppingList`: мок fetch → корректный парсинг

## 10. E2E API-тесты

- [ ] 10.1 Создать `tests/e2e/test_shopping_list_api.py`: авторизованный пользователь с заполненным планом → 200, корректная агрегация
- [ ] 10.2 E2E: авторизованный пользователь с пустым планом → 200, пустой массив
- [ ] 10.3 E2E: неавторизованный запрос → 401
