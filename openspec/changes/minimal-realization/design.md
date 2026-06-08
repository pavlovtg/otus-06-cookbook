# minimal-realization

## Контекст

Репозиторий содержит только архитектурную документацию — кода нет. Все ключевые решения зафиксированы в ADR-0001..0026. Этот change реализует минимальный рабочий срез: один backend-сервис, один frontend, одна страница.

## Цели / Не-цели

**Цели:**

- Рабочее приложение: `docker compose up` → список рецептов в браузере
- Соответствие зафиксированным ADR и AR
- OpenAPI-контракт создаётся до реализации (AR-0015)
- Минимальный набор тестов (happy-path)

**Не-цели:**

- Авторизация, пользователи, роли
- Фото, теги, категории, планировщик, список покупок, время приготовления, сложность
- Полный CRUD рецептов
- CI pipeline (отдельный change)

## Решения

### Топология сервисов

Согласно ADR-0007, ADR-0008, ADR-0010, ADR-0020, ADR-0026 и стандарту `service-registry.md`:

| Сервис | Образ | Роль |
|--------|-------|------|
| `reverse-proxy` | nginx | Edge reverse proxy; `/api/*` → `api-gateway`, остальное → `web`; HTTP порт 80 |
| `api-gateway` | .NET 10 / YARP | Маршрутизирует `/api/cookbook/...` → `recipes/api/...` (AR-0016) |
| `web` | Next.js | Frontend + BFF; BFF вызывает `api-gateway` через `lib/bff/gateway.ts` |
| `recipes` | .NET 10 | Сервис рецептов; bounded context `cookbook` |
| `postgresql` | PostgreSQL | СУБД; БД `recipes`, схема `cookbook` (AR-0017) |

Сервис `authorization` в этом change не поднимается — все эндпоинты публичные.

### Contract-First: OpenAPI-спецификация

Согласно AR-0015 и ADR-0023, до написания кода создаётся файл `docs/contracts/cookbook/recipes.yaml` (OpenAPI 3.0.0). Спецификация описывает единственный эндпоинт:

```
GET /api/recipes/v1
```

Ответ: массив объектов `RecipeDto` с полями `id` (uuid), `title` (string), `description` (string).

### Маршрутизация

Согласно AR-0016, ADR-0024 и стандарту `api-design.md` (формат URI: `/api/{resource}/v{major_version}`):

- Внешний URL: `GET /api/cookbook/recipes/v1`
- `api-gateway` стрипает `/cookbook` → проксирует на `recipes`: `GET /api/recipes/v1`

### Модель данных

Агрегат `Recipe` содержит: `id` (Guid), `title` (string), `description` (string). Остальные поля добавляются в следующих change.

### База данных

Согласно AR-0017, ADR-0025 и стандарту `service-registry.md`:

- БД: `recipes` (lowercase snake_case)
- Схема: `cookbook` (bounded context, lowercase snake_case)
- Дефолтная схема `public` не используется
- Только сервис `recipes` имеет прямой доступ к БД

### Backend: сервис `recipes`

Структура согласно стандарту `dotnet-project-structure.md`. HTTP-эндпоинты реализуются через `ApiController` (AR-0018):

```
apps/Cookbook/
  src/Recipes/
    Domain/Recipe.cs
    Application/
      Ports/IRecipeRepository.cs
      Queries/GetRecipesQuery.cs
    Adapters/
      Web/RecipesController.cs        ← ApiController
      Postgresql/
        RecipesDbContext.cs
        RecipeRepository.cs
        Migrations/
    Infrastructure/
    Program.cs
  tests/Recipes.Tests/
    GetRecipesTests.cs
```

Seed-данные: реализуются через EF Core `HasData` — 10 рецептов.

### Frontend: сервис `web`

Структура согласно стандарту `frontend-project-structure.md`:

- `app/(public)/page.tsx` — Server Component, вызывает BFF
- `lib/bff/gateway.ts` — HTTP-клиент к `api-gateway`
- `components/features/RecipeCard.tsx` — карточка рецепта (id, title, description)

Данные загружаются на сервере (RSC), клиентский JS минимален.

## Риски / Компромиссы

- [Риск] nginx без TLS — браузер работает по HTTP → Митигация: TLS откладывается на отдельный change
- [Риск] YARP без `authorization` — неполная топология → Митигация: `api-gateway` стартует без auth-маршрутов
- [Компромисс] Seed через `HasData` — миграции пересоздаются при изменении данных → Приемлемо для MVP

## Открытые вопросы

- Порт публикации `reverse-proxy` (80 vs нестандартный) — решается в `docker-compose.yml`
- Стратегия health-check для `depends_on` в Compose
