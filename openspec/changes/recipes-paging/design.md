# recipes-paging

## Контекст

`GET /api/v1/recipes` возвращает плоский массив всех рецептов без ограничений. Ингредиенты уже имеют пагинацию — `PagedResult<T>` существует в Application-слое. PRD требует 18 рецептов на странице; стандарт api-design задаёт параметры `page`/`pageSize`, максимум 1000, ответ `{ items, total, page, pageSize }`.

## Цели / Не-цели

**Цели:**

- `GET /api/v1/recipes` принимает `page` и `pageSize`; возвращает `PagedResult<RecipeShortDto>`.
- `pageSize` по умолчанию — 18; максимум — 1000 (clamp, не ошибка).
- Frontend отображает компонент пагинации и общее количество рецептов.
- Контракт `docs/contracts/cookbook/recipes.yaml` обновлён.

**Не-цели:**

- Фильтрация и сортировка рецептов.
- Курсорная пагинация.

## Решения

### 1. Переход с `IAsyncEnumerable` на `PagedResult<Recipe>` в репозитории

`IRecipeRepository.GetRecipesAsync` заменяется на `GetRecipesPagedAsync(int page, int pageSize, CancellationToken)`, возвращающий `Task<PagedResult<Recipe>>`.

**Почему**: `IAsyncEnumerable` не позволяет вернуть `total` без полного перебора. `PagedResult<T>` уже существует в Application-слое и используется для ингредиентов — переиспользуем паттерн.

### 2. Два запроса к БД: COUNT + SELECT

Репозиторий выполняет два запроса в рамках одного вызова: `COUNT(*)` для получения общего количества и `SELECT ... LIMIT/OFFSET` для получения страницы данных.

**Почему**: простая и понятная реализация; для учебного проекта с небольшим объёмом данных накладные расходы на второй запрос незначительны.

### 3. Clamp pageSize на уровне сервиса

`RecipeService` применяет `Math.Clamp(pageSize, 1, 1000)` и `Math.Max(page, 1)` перед передачей в репозиторий. Значения ≤ 0 возвращают `400 Bad Request`.

### 4. Контракт: новая схема `PagedResultRecipeShortDto`

Аналогично `PagedResultIngredientDto` в существующем контракте. Ответ `GET /api/v1/recipes` меняется с `array` на `PagedResultRecipeShortDto`.

**Breaking change**: единственный клиент — BFF (`apps/web`), обновляется в рамках этого же изменения.

### 5. Frontend: page из searchParams

Страница `/` читает `?page=N` из URL search params и передаёт в BFF. Компонент `Pagination` из Storybook DS рендерит навигацию. При смене страницы — `router.push` с обновлённым `?page`. Номер страницы хранится в URL, поэтому при нажатии «Назад» после просмотра рецепта пользователь возвращается на ту же страницу списка.

## Риски / Компромиссы

- **Breaking change контракта** → единственный потребитель — BFF, обновляется синхронно в одном PR.
