# categories-crud

## Контекст

Проект использует гексагональную архитектуру (Ports & Adapters) + DDD на .NET 10. Уже реализованы сущности `Recipe` и `Ingredient` по одному паттерну: агрегат в `Domain/`, порты в `Application/Ports/`, сервис в `Application/`, контроллер в `Adapters/Web/`, репозиторий в `Adapters/Postgresql/`. Категории — новая независимая сущность в том же bounded context `cookbook`.

Frontend: Next.js App Router + BFF в `lib/bff/`, Zod-схемы в `lib/schemas/`, страницы в `app/`. Паттерн реализован для ингредиентов и рецептов.

UI-дизайн: макет в `docs/design/mockup/` (dark-тема, Inter 400/500, акцент purple `#9968ff`, радиусы 8/12/100px, тени вместо border). Компоненты разрабатываются через Storybook (`docs/design/storybook/`). Страница категорий в макете реализована как `views.categories` — карточки с тегами, сгруппированные по типу, кнопки редактирования/удаления на каждом теге, модальная форма создания/редактирования.

## Цели / Не-цели

**Цели:**

- Реализовать CRUD для категорий по тому же паттерну, что `Ingredient`.
- Добавить seed-данные (~60 категорий по 7 типам).
- Страница `/categories` с отображением и управлением (без ограничений по ролям — auth не реализован).
- OpenAPI-контракт до кода (AR-0015).
- Покрытие тестами ≥ 80% (unit + integration + microservice + e2e).

**Не-цели:**

- Привязка категорий к рецептам (отдельная задача).
- Ролевые ограничения (admin-only) — реализуются после внедрения auth.
- Поиск/фильтрация категорий на странице (MVP: простой список).

## Решения

### 1. Category как отдельный агрегат в том же сервисе

`Category` живёт в `apps/Cookbook/src/Recipes/Domain/` рядом с `Ingredient`. Альтернатива — отдельный микросервис — избыточна для MVP; категории тесно связаны с рецептами и ингредиентами.

### 2. `type` — enum в домене, immutable

`CategoryType` — C# enum (`MealRole`, `CookingMethod`, `MainIngredient`, `Cuisine`, `MealTime`, `Dietary`, `ServingForm`). Значение задаётся при создании и не меняется. В API передаётся как строка (snake_case). Альтернатива — строка без enum — теряет типобезопасность.

### 3. Запрет удаления используемых категорий

Проверка в `CategoryService.DeleteAsync`: если категория присутствует хотя бы в одном рецепте — бросается доменное исключение `CategoryInUseException` → `400 Bad Request`. Паттерн идентичен `IngredientInUseException`.

### 4. Ограничение 1000 категорий

Проверка в `CategoryService.CreateAsync`: если количество категорий ≥ 1000 — бросается доменное исключение `CategoryLimitExceededException` → `400 Bad Request`. Константа в `CategoryConstraints`.

### 5. Контракт: раздел `categories` в файле сервиса Cookbook

Один OpenAPI-файл на сервис (AR-0015). Эндпоинты категорий добавляются в существующий контракт-файл сервиса `recipes` (`docs/contracts/cookbook/`), а не в отдельный файл.

### 6. Frontend: отдельный BFF-файл `lib/bff/categories.ts`

Один файл на ресурс (AR-0057). Zod-схема в `lib/schemas/category.ts`. Страница — Server Component с Client-компонентами для форм.

### 7. UI: следует макету и Storybook

Страница `/categories` реализуется по макету `docs/design/mockup/index.html` (`views.categories`):

- Заголовок + кнопка «Новая категория».
- 7 карточек (`.card.card-pad-lg`) — по одной на тип.
- Внутри каждой карточки — теги (`.tag`) с кнопками редактирования и удаления.
- Форма создания/редактирования — модальное окно (`.modal`).
- Стили берутся из `docs/design/storybook/src/styles.css` (единственный источник, ADR-0031).
- Новые компоненты добавляются в Storybook (`docs/design/storybook/`).

## Тестирование

Следует стратегии из ADR-0033 и стандартам AR-0040–AR-0046:

- **Unit (backend)**: доменная логика `Category` — валидация полей, `CategoryConstraints`, доменные исключения. Инструменты: xUnit + FluentAssertions + NSubstitute.
- **Integration DB (backend)**: `CategoryRepository` — CRUD, upsert seed, проверка ограничений. Инструменты: Testcontainers + EF Core.
- **Microservice (backend)**: `CategoriesController` — все эндпоинты, граничные случаи (лимит 1000, удаление используемой). Инструменты: WebApplicationFactory + Testcontainers + WireMock.Net.
- **Unit (frontend)**: Zod-схема `category.ts`, BFF-функции `categories.ts` с мокированным fetch. Инструменты: Vitest + Testing Library. Два файла: `category.schema.test.ts` и `category.bff.test.ts` (AR-0058).
- **E2E API**: сценарии из specs — list, create, edit, delete, лимит, удаление используемой. Инструменты: pytest + httpx в `tests/e2e/`.
- **UI E2E**: страница `/categories` — отображение, создание, редактирование, удаление. Инструменты: Playwright в `tests/ui/`.

## Риски / Компромиссы

- [Риск] Seed-данные (~60 записей) могут конфликтовать при повторном запуске → Митигация: `CookbookSeeder` использует upsert по `name + type` (AR-0054).
- [Компромисс] Без ролей любой пользователь может создавать/удалять категории → Принято как временное решение до реализации auth.
- [Риск] При добавлении связи рецепт↔категория потребуется миграция → Митигация: таблица `categories` создаётся сейчас, связующая таблица — в следующей задаче.
