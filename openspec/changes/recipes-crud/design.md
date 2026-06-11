# recipes-crud

## Контекст

Текущий сервис `Recipes` реализует только `GET /api/recipes/v1` (список). Доменная модель `Recipe` содержит `Id`, `Title`, `Description`. Контракт `docs/contracts/cookbook/recipes.yaml` описывает только этот эндпоинт. Фронтенд (`apps/web`) отображает список рецептов на главной странице без навигации на детальную страницу.

Изменение затрагивает три слоя: backend (Domain + Application + Adapters), OpenAPI-контракт и frontend (BFF + UI). Это cross-cutting изменение с миграцией БД.

Действующие ADR, ограничивающие дизайн: ADR-0006 (REST), ADR-0011 (.NET 10), ADR-0012 (DDD), ADR-0013 (Hexagonal), ADR-0014 (EF Core), ADR-0015 (Next.js), ADR-0016 (React+TS), ADR-0017 (BFF), ADR-0019 (Zod), ADR-0023 (Contract-First).

ADR-0018 (Tailwind + shadcn/ui) **пересматривается** в рамках этого изменения — см. Открытые вопросы.

## Цели / Не-цели

**Цели:**

- Расширить доменную модель `Recipe` полями `CookingTime`, `Difficulty`, `Servings`, `Instructions`.
- Реализовать CRUD-эндпоинты: `POST`, `GET /{id}`, `PUT /{id}`, `DELETE /{id}`.
- Обновить OpenAPI-контракт до реализации (AR-0015).
- Реализовать страницы UI: список (обновлённый), детальная, создание, редактирование.
- UI реализуется по макетам из `docs/design/mockup/` (источник поведения и экранов) с использованием компонентов из `docs/design/storybook/` (источник React-компонентов и стилей).
- Валидация данных рецепта — только в доменном слое через типизированные исключения.
- Покрыть все сценарии из specs e2e/ui-тестами.

**Не-цели:**

- Авторизация и разграничение доступа (этап 17).
- Загрузка фото (этап 11).
- Категории, теги, ингредиенты (этапы 9, 12).
- Пагинация и поиск (этапы 13, 14).

## Решения

### 1. Contract-First: OpenAPI до реализации

Сначала обновляется `docs/contracts/cookbook/recipes.yaml` с новыми схемами `RecipeDto`, `CreateRecipeRequest`, `UpdateRecipeRequest`. Затем реализуется код. Соответствует AR-0015.

`Difficulty` моделируется как C# `enum` (`Easy`, `Everyday`, `Festive`, `Restaurant`, `Signature`). В БД хранится как `varchar` для читаемости.

### 2. Структура CRUD-эндпоинтов

```
GET    /api/recipes/v1          — список (существующий)
POST   /api/recipes/v1          — создание
GET    /api/recipes/v1/{id}     — детальный просмотр
PUT    /api/recipes/v1/{id}     — полное обновление
DELETE /api/recipes/v1/{id}     — удаление
```

Маршрут соответствует AR-0016. Gateway стрипает `/api/cookbook/` и проксирует на сервис.

### 3. Валидация — только в домене через типизированные исключения

Вся валидация бизнес-правил сосредоточена в доменном слое (`Recipe.Create`, `Recipe.Update`). DataAnnotations на DTO не используются — это нарушало бы DDD (ADR-0012).

Паттерн обработки ошибок:
- Доменный метод выбрасывает типизированное исключение (`RecipeDomainException` или специализированные подклассы) при нарушении инварианта.
- **Контроллер явно перехватывает доменные исключения** в `catch`-блоке и формирует `400 Bad Request` с `application/problem+json`.
- Глобальный `IExceptionHandler` регистрируется один — как fallback для непредвиденных исключений, возвращает `500 Internal Server Error` с generic-сообщением. Никаких других `IExceptionHandler` не добавляется.
- Это решение фиксируется как новые архитектурные правила (AR).

### 4. EF Core миграция

Добавляется новая миграция для расширения таблицы `cookbook.recipes`. Существующие seed-данные полностью заменяются: старые записи удаляются, новые добавляются с заполненными полями. Миграция применяется автоматически при старте.

### 5. Frontend: страницы и BFF

Источники для реализации UI:
- **`docs/design/mockup/index.html`** — источник поведения, экранов и UX-сценариев (навигация, состояния, взаимодействия).
- **`docs/design/storybook/src/`** — источник готовых React-компонентов и CSS-стилей.

Новые страницы Next.js (App Router):
- `/` — обновлённый список с расширенными карточками
- `/recipes/[id]` — детальная страница (RSC, данные через BFF)
- `/recipes/new` — форма создания (Client Component с React Hook Form + Zod)
- `/recipes/[id]/edit` — форма редактирования (Client Component)

BFF-функции в `apps/web/lib/bff/gateway.ts`: `getRecipe(id)`, `createRecipe(data)`, `updateRecipe(id, data)`, `deleteRecipe(id)`.

Zod-схемы в `apps/web/lib/schemas/recipe.ts`: `RecipeDetailSchema`, `CreateRecipeSchema`, `UpdateRecipeSchema`.

### 6. UI: Storybook как источник компонентов, отказ от Tailwind

ADR-0018 (Tailwind + shadcn/ui) заменяется новым ADR. Причина: `docs/design/storybook/` содержит полную готовую систему компонентов на чистом CSS с семантическими классами и CSS-переменными. Tailwind несовместим с этой системой — использование обоих создаёт два конкурирующих механизма стилизации.

Решение:
- `docs/design/storybook/src/styles.css` копируется в `apps/web/app/globals.css` — единственный источник стилей.
- Компоненты из `docs/design/storybook/src/` переносятся в `apps/web/components/` с адаптацией типов (замена `Recipe` из `mocks.ts` на Zod-схемы из `apps/web/lib/schemas/`).
- Tailwind и shadcn/ui удаляются из зависимостей `apps/web`.

### 7. Тестирование: e2e/ui-тесты для всех сценариев из specs

Каждый сценарий из specs (`recipe-create`, `recipe-detail`, `recipe-edit`, `recipe-delete`, `recipe-list`) покрывается e2e-тестом (Playwright, `tests/e2e/`) или ui-тестом (Playwright, `tests/ui/`). Интеграционные тесты backend (`Recipes.Tests`) покрывают CRUD-эндпоинты через `WebApplicationFactory` + Testcontainers.

## Риски / Компромиссы

- **[Риск] Tailwind уже используется в `apps/web`** → проверено: в `package.json` Tailwind отсутствует, в `layout.tsx` нет импорта — риск минимален, удаление безопасно.
- **[Компромисс] PUT вместо PATCH** → полное обновление проще в реализации и достаточно для текущего объёма полей.
- **[Компромисс] Копирование компонентов из Storybook** → при изменении дизайна нужно синхронизировать вручную; приемлемо для учебного проекта.

## План миграции

1. Записать новый ADR, superseding ADR-0018 (чистый CSS из Storybook вместо Tailwind).
2. Записать новые AR: «Доменная валидация через типизированные исключения» и «Один глобальный fallback IExceptionHandler».
3. Обновить OpenAPI-контракт.
4. Расширить доменную модель, добавить типизированные исключения.
5. Создать EF Core миграцию, заменить seed-данные.
6. Реализовать новые методы репозитория и сервиса.
7. Добавить эндпоинты в контроллер с явным перехватом доменных исключений, обновить DTO.
8. Обновить BFF и Zod-схемы.
9. Перенести `styles.css` и компоненты из Storybook, реализовать страницы UI по макетам из `docs/design/mockup/`.
10. Написать e2e/ui-тесты для всех сценариев из specs.

Откат: `dotnet ef database update <предыдущая миграция>`.

## Открытые вопросы

- **ADR-0018 (Tailwind + shadcn/ui)** нужно заменить новым ADR «Чистый CSS из Storybook как система стилей фронтенда». Шаг `adr` запишет superseding ADR.
- **Новые AR**: «Доменная валидация через типизированные исключения; контроллер явно перехватывает и формирует Problem+JSON» и «Один глобальный fallback IExceptionHandler для непредвиденных ошибок». Шаг `adr` запишет AR.
