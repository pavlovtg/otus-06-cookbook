# shopping-list

## Контекст

Планировщик меню реализован: пользователь составляет план на неделю (7 дней × 3 приёма пищи). Каждый слот содержит рецепты с указанием порций. Ингредиенты рецептов хранятся в БД с единицами измерения и категориями. Список покупок — производная от плана меню: агрегация ингредиентов с пересчётом количеств по порциям.

Архитектура: гексагональная (Ports & Adapters), backend — .NET 10 / C#, frontend — Next.js App Router + BFF, API — REST через YARP gateway.

## Цели / Не-цели

**Цели:**

- Новый эндпоинт `GET /api/v1/shopping-list` — агрегирует ингредиенты из текущего плана меню пользователя.
- Агрегация: суммирование количеств одинаковых ингредиентов с учётом порций из слотов.
- Группировка по категориям ингредиентов, сортировка по алфавиту внутри группы.
- Страница `/shopping-list` с таблицей, кнопками «Скопировать» и «Распечатать».

**Не-цели:**

- Редактирование списка покупок вручную (вычеркивание, добавление).
- Сохранение истории списков покупок.
- Интеграция с магазинами или сервисами доставки.

## Решения

### 1. Агрегация на backend, не на frontend

Агрегация выполняется в Application-сервисе на backend. Frontend получает готовый сгруппированный список.

Альтернатива — агрегировать на frontend из данных плана меню и рецептов. Отклонена: требует загрузки полных данных рецептов на клиент, дублирует логику, нарушает принцип единственной ответственности BFF.

### 2. Отдельный эндпоинт, не расширение meal-plan

`GET /api/v1/shopping-list` — самостоятельный ресурс, не вложенный в `/meal-plan`. Список покупок — отдельная концепция, не часть плана меню.

### 3. Агрегация в памяти Application-сервиса

Репозиторий загружает план меню пользователя вместе с ингредиентами рецептов через EF Core Include. Агрегация (SUM с учётом `scale = item.Servings / recipe.Servings`) выполняется в Application-сервисе.

Выбрана агрегация в памяти: проще поддерживать, достаточно производительна для объёма данных MVP (≤ 21 слот × несколько рецептов).

### 4. Формат ответа — список групп

```json
[
  {
    "category": "Овощи",
    "items": [
      { "ingredientId": "...", "title": "Картофель", "amount": 4.0, "unit": "шт" }
    ]
  }
]
```

Группировка по категории ингредиента, сортировка групп по порядку `IngredientCategory` enum, внутри группы — по алфавиту.

### 5. Копирование и печать — на клиенте

`navigator.clipboard.writeText()` и `window.print()` — стандартные браузерные API, не требуют серверной поддержки.

## Детали реализации

### Domain

Новых доменных типов не требуется. Список покупок — вычисляемая проекция, не агрегат. Используются существующие типы: `MealPlan`, `MealPlanSlot`, `MealPlanItem`, `Recipe`, `RecipeIngredient`, `Ingredient`.

### Application

**Новые view-типы:**

```csharp
// ShoppingListGroupView.cs
internal sealed record ShoppingListGroupView(
    string Category,
    IReadOnlyList<ShoppingListItemView> Items);

// ShoppingListItemView.cs
internal sealed record ShoppingListItemView(
    Guid IngredientId,
    string Title,
    decimal Amount,
    string Unit);
```

**Новый порт:**

```csharp
// Ports/IShoppingListRepository.cs
internal interface IShoppingListRepository
{
    Task<IReadOnlyList<ShoppingListGroupView>> GetShoppingListAsync(
        UserId userId,
        CancellationToken cancellationToken = default);
}
```

**Новый сервис:**

```csharp
// ShoppingListService.cs
internal sealed class ShoppingListService
{
    Task<IReadOnlyList<ShoppingListGroupView>> GetAsync(UserId userId, CancellationToken ct);
}
```

Сервис загружает план через `IShoppingListRepository`, агрегирует ингредиенты в памяти, группирует и сортирует результат.

### Adapters / Postgresql

Метод `GetShoppingListAsync` в `RecipeRepository` (реализует `IShoppingListRepository`):

- Загружает план меню пользователя через EF Core с Include: `MealPlanSlots → MealPlanItems → Recipe → RecipeIngredients → Ingredient`.
- Агрегирует в памяти: группирует по `IngredientId`, суммирует `amount * (item.Servings / recipe.Servings)`.
- Группирует по `Ingredient.Category`, сортирует группы по порядку `IngredientCategory` enum, внутри — по `Ingredient.Title`.
- Возвращает `IReadOnlyList<ShoppingListGroupView>`.

### Adapters / Web

**Новые DTO:**

```csharp
// Dto/ShoppingListGroupDto.cs
internal sealed record ShoppingListGroupDto(string Category, IReadOnlyList<ShoppingListItemDto> Items);

// Dto/ShoppingListItemDto.cs
internal sealed record ShoppingListItemDto(Guid IngredientId, string Title, decimal Amount, string Unit);
```

**Новый контроллер:**

```csharp
// ShoppingListController.cs
[ApiController]
[Route("api/v1/shopping-list")]
[Authorize]
internal sealed class ShoppingListController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetShoppingList(CancellationToken ct);
}
```

### Frontend

- **Zod-схема**: `lib/schemas/shopping-list.ts` — `ShoppingListGroupSchema`, `ShoppingListItemSchema`.
- **BFF server**: `lib/bff/shopping-list.server.ts` — `getShoppingList()` через `serverFetch`.
- **BFF route**: `app/api/cookbook/v1/shopping-list/route.ts` — proxy `GET`.
- **Страница**: `app/shopping-list/page.tsx` — Server Component, загружает данные через BFF, рендерит таблицу.
- **Client-компонент**: `ShoppingListActions.tsx` — кнопки «Скопировать» и «Распечатать» (требуют `"use client"`).
- **CSS**: print-стили уже есть в `globals.css` (`.header`, `.btn` скрываются при печати).

## План тестирования

### Integration-тесты (backend, Testcontainers)

- `RecipeRepository.GetShoppingListAsync`: корректная агрегация по реальной БД.
- `RecipeRepository.GetShoppingListAsync`: пересчёт количества при нестандартных порциях.
- `RecipeRepository.GetShoppingListAsync`: дедупликация одинаковых ингредиентов из разных слотов.
- `RecipeRepository.GetShoppingListAsync`: пустой план → пустой список.
- `RecipeRepository.GetShoppingListAsync`: группировка и сортировка.

### Microservice-тесты (backend, WebApplicationFactory + Testcontainers)

- `GET /api/v1/shopping-list` → 200 с корректным телом для авторизованного пользователя с заполненным планом.
- `GET /api/v1/shopping-list` → 200 с пустым массивом для пустого плана.
- `GET /api/v1/shopping-list` → 401 для неавторизованного запроса.

### Unit-тесты (frontend)

- `ShoppingListGroupSchema`: валидация корректного ответа.
- `ShoppingListGroupSchema`: отклонение невалидного ответа.
- BFF `getShoppingList`: мок fetch → корректный парсинг.

### E2E API-тесты

- `tests/e2e/test_shopping_list_api.py`: сценарии из `specs/shopping-list-get/spec.md`.

## Риски / Компромиссы

- **Производительность при большом плане** → план ограничен 21 слотом; агрегация в памяти приемлема.
- **Clipboard API недоступен в HTTP-контексте** → в production — HTTPS через nginx; в dev — localhost считается secure context.
- **Список не обновляется в реальном времени** → страница загружает данные при открытии; для MVP достаточно.
