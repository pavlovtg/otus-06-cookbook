# dashboards

## Контекст

Приложение содержит полный набор данных: рецепты, рейтинги, комментарии, избранное, план меню, пользователи. Дашборд агрегирует эти данные в одном месте. Три роли — гость, авторизованный пользователь, администратор — получают разный набор виджетов согласно PRD-0001 §11.

Архитектура: гексагональная (Ports & Adapters), backend — .NET 10 / C#, frontend — Next.js App Router + BFF, API — REST через YARP gateway.

## Цели / Не-цели

**Цели:**

- Новый эндпоинт `GET /api/v1/dashboard` — возвращает агрегированную статистику с учётом роли.
- Страница `/dashboard` с KPI-карточками, топ-списками, гистограммами и мини-сеткой плана меню.
- Пункт «Дашборд» в навигации для всех ролей.

**Не-цели:**

- Realtime-обновление данных.
- Экспорт статистики.
- Исторические тренды и сравнение периодов.

## Решения

### 1. Один эндпоинт для всех ролей

`GET /api/v1/dashboard` возвращает разный набор полей в зависимости от JWT-токена (или его отсутствия). Альтернатива — отдельные эндпоинты на роль — отклонена: избыточно для MVP, усложняет BFF.

### 2. Агрегация на backend

Вся агрегация выполняется в Application-сервисе. Frontend получает готовые данные. Альтернатива — агрегировать на frontend — отклонена: требует загрузки полных данных на клиент, дублирует логику.

### 3. Гистограммы через Chart.js

Chart.js уже используется в макете проекта. Альтернатива — Recharts — не используется нигде в проекте, добавляет зависимость.

### 4. Мини-сетка плана — чистый CSS

Сетка 7×3 реализуется через CSS Grid без библиотек. Данные о заполненности (булев массив слотов) приходят с сервера.

### 5. Страница — Server Component + Client Component для графиков

Данные загружаются на сервере. Графики Chart.js требуют браузерного окружения — выносятся в Client Component. Мини-сетка и топ-списки остаются в Server Component.

## Детали реализации

### Domain

Новых доменных типов не требуется. Статистика — вычисляемая проекция, не агрегат.

### Application

**Новые view-типы:**

```csharp
// DashboardStatsView.cs
internal sealed record DashboardStatsView(
    int TotalRecipes,
    int? TotalUsers,           // только admin
    int? TotalComments,        // только admin
    int? MyRecipes,            // только user/admin
    int? MyComments,           // только user
    IReadOnlyList<RecipeRankView> Top10ByRating,
    IReadOnlyList<RecipeRankView>? TopFavoritesByRating,  // только user
    IReadOnlyList<CategoryCountView> ByMainIngredient,
    IReadOnlyList<CategoryCountView> ByCuisine,
    IReadOnlyList<UserRankView>? TopUsersByRating,        // только admin
    IReadOnlyList<UserRankView>? TopUsersByComments,      // только admin
    IReadOnlyDictionary<string, bool>? PlanFill);         // только user

// RecipeRankView.cs
internal sealed record RecipeRankView(Guid RecipeId, string Title, double Rating);

// CategoryCountView.cs
internal sealed record CategoryCountView(string Name, int Count);

// UserRankView.cs
internal sealed record UserRankView(string DisplayName, int Count);
```

**Новый порт:**

```csharp
// Ports/IDashboardRepository.cs
internal interface IDashboardRepository
{
    Task<DashboardStatsView> GetStatsAsync(
        UserId? userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}
```

**Новый сервис:**

```csharp
// DashboardService.cs
internal sealed class DashboardService
{
    Task<DashboardStatsView> GetStatsAsync(UserId? userId, bool isAdmin, CancellationToken ct);
}
```

### Adapters / Postgresql

Метод `GetStatsAsync` в `RecipeRepository` (реализует `IDashboardRepository`):

- `TotalRecipes` — `COUNT` публичных рецептов (для admin — всех).
- `TotalUsers` — `COUNT` пользователей (только admin).
- `TotalComments` — `COUNT` всех комментариев (только admin).
- `MyRecipes` — `COUNT` рецептов текущего пользователя.
- `MyComments` — `COUNT` комментариев текущего пользователя.
- `Top10ByRating` — топ-10 рецептов по среднему рейтингу (`AVG(value) DESC LIMIT 10`).
- `TopFavoritesByRating` — топ-10 из избранного пользователя по рейтингу.
- `ByMainIngredient` / `ByCuisine` — `GROUP BY` категории, `COUNT`, `ORDER BY count DESC`.
- `TopUsersByRating` / `TopUsersByComments` — `GROUP BY user_id`, `COUNT`, `ORDER BY count DESC LIMIT 10`.
- `PlanFill` — словарь `"{dayIndex}_{mealType}" → bool` (true если слот непустой).

Все запросы выполняются в одном методе через несколько EF Core запросов в рамках одного `DbContext`.

### Adapters / Web

**Новые DTO:**

```csharp
// Dto/DashboardStatsDto.cs
internal sealed record DashboardStatsDto(
    int TotalRecipes,
    int? TotalUsers,
    int? TotalComments,
    int? MyRecipes,
    int? MyComments,
    IReadOnlyList<RecipeRankDto> Top10ByRating,
    IReadOnlyList<RecipeRankDto>? TopFavoritesByRating,
    IReadOnlyList<CategoryCountDto> ByMainIngredient,
    IReadOnlyList<CategoryCountDto> ByCuisine,
    IReadOnlyList<UserRankDto>? TopUsersByRating,
    IReadOnlyList<UserRankDto>? TopUsersByComments,
    IReadOnlyDictionary<string, bool>? PlanFill);

// Dto/RecipeRankDto.cs
internal sealed record RecipeRankDto(Guid RecipeId, string Title, double Rating);

// Dto/CategoryCountDto.cs
internal sealed record CategoryCountDto(string Name, int Count);

// Dto/UserRankDto.cs
internal sealed record UserRankDto(string DisplayName, int Count);
```

**Новый контроллер:**

```csharp
// DashboardController.cs
[ApiController]
[Route("api/v1/dashboard")]
internal sealed class DashboardController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetStats(CancellationToken ct);
}
```

### OpenAPI-контракт

Новый эндпоинт добавляется в `docs/contracts/cookbook/cookbook.yaml`:

```yaml
/api/v1/dashboard:
  get:
    summary: Статистика дашборда
    security: []   # опциональный JWT
    responses:
      '200':
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/DashboardStatsDto'
```

### Frontend

- **Zod-схема**: `lib/schemas/dashboard.ts` — `DashboardStatsDtoSchema` и вложенные схемы.
- **BFF server**: `lib/bff/dashboard.server.ts` — `getDashboardStats()` через `serverFetch`.
- **BFF route**: `app/api/cookbook/v1/dashboard/route.ts` — proxy `GET`.
- **Страница**: `app/dashboard/page.tsx` — Server Component, загружает данные, рендерит KPI, топ-списки, мини-сетку.
- **Client-компонент**: `DashboardCharts.tsx` — гистограммы Chart.js (`"use client"`, `dynamic import`).
- **Навигация**: добавить пункт «Дашборд» в `layout.tsx` без ограничений по роли.

## План тестирования

### Integration-тесты (backend, Testcontainers)

- `RecipeRepository.GetStatsAsync`: корректный подсчёт `TotalRecipes` для гостя.
- `RecipeRepository.GetStatsAsync`: `MyRecipes` и `MyComments` для авторизованного пользователя.
- `RecipeRepository.GetStatsAsync`: `TotalUsers` и `TotalComments` для admin.
- `RecipeRepository.GetStatsAsync`: топ-10 по рейтингу — корректная сортировка.
- `RecipeRepository.GetStatsAsync`: `ByMainIngredient` — корректная группировка.
- `RecipeRepository.GetStatsAsync`: `PlanFill` — корректное отображение заполненных слотов.

### Microservice-тесты (backend, WebApplicationFactory + Testcontainers)

- `GET /api/v1/dashboard` → 200 для анонимного запроса (только гостевые поля).
- `GET /api/v1/dashboard` → 200 с расширенными полями для авторизованного пользователя.
- `GET /api/v1/dashboard` → 200 с admin-полями для администратора.

### Unit-тесты (frontend)

- `DashboardStatsDtoSchema`: валидация корректного ответа для гостя.
- `DashboardStatsDtoSchema`: валидация корректного ответа для авторизованного пользователя.
- BFF `getDashboardStats`: мок fetch → корректный парсинг.

### E2E API-тесты

- `tests/e2e/test_dashboard_api.py`: сценарии из `specs/dashboard-stats/spec.md`.

## Риски / Компромиссы

- **Производительность агрегации** → несколько запросов к БД при каждом открытии страницы. Для MVP приемлемо; при необходимости добавить кэш.
- **Chart.js в бандле** → изолируется в Client Component с `dynamic import`, не блокирует SSR.
