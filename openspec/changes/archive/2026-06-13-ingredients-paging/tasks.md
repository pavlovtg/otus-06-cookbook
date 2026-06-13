# ingredients-paging

## 1. Контракт

- [x] 1.1 Обновить `docs/contracts/cookbook/recipes.yaml`: добавить query-параметры `page`, `pageSize`, `title` с ограничениями; изменить ответ `GET /api/v1/ingredients` на `PagedResult<IngredientDto>`

## 2. Интерфейсы

- [x] 2.1 Обновить интерфейс `IIngredientRepository.GetIngredientsAsync` — возвращает `Task<PagedResult<Ingredient>>`, принимает `page`, `pageSize`
- [x] 2.2 Обновить `IIngredientService.GetIngredientsAsync` — принимает `page`, `pageSize`; возвращает `PagedResult<Ingredient>`

## 3. Репозиторий

- [x] 3.1 Реализовать `GetIngredientsAsync` в репозитории через `CountAsync` + `Skip/Take`

## 4. Контроллер

- [x] 4.1 Добавить query-параметры `page`, `pageSize` в `GET /api/v1/ingredients`; реализовать clamp `pageSize` до 1000 и валидацию `page ≥ 1`, `pageSize ≥ 1` → `400`
- [x] 4.2 Добавить валидацию длины `title` ≤ `IngredientConstraints.TitleMaxLength` (200) → `400`

## 5. Frontend схема и BFF

- [x] 5.1 Обновить Zod-схему ответа `GET /api/v1/ingredients` на `PagedResult<IngredientDto>`
- [x] 5.2 Обновить BFF `lib/bff/ingredients.ts` — передавать `page`, `pageSize` в запрос; возвращать `PagedResult`

## 6. UI

- [x] 6.1 Обновить страницу `/ingredients` — читать `page` из `searchParams`, передавать в BFF
- [x] 6.2 Добавить компонент пагинатора на страницу `/ingredients`
- [x] 6.3 Добавить `maxLength={200}` на поле поиска по названию ингредиента

## 7. Тесты

- [x] 7.1 Unit-тесты: валидация параметров пагинации и `title` в контроллере (clamp, `400` при ≤ 0, `400` при title > 200)
- [x] 7.2 Integration-тесты: репозиторий — `Skip/Take` + `CountAsync` возвращает корректный `total` и срез
- [x] 7.3 Microservice-тесты: `GET /api/v1/ingredients` — сценарии из specs (без параметров, с параметрами, pageSize > 1000, page = 0, title > 200)
- [x] 7.4 E2E API-тесты: обновить `tests/e2e/test_ingredients_api.py` под новый формат ответа `PagedResult`
- [x] 7.5 UI E2E-тесты: обновить `tests/ui/test_ingredients.py` — проверить отображение пагинатора и ограничение поля поиска
- [x] 7.6 Frontend unit-тесты: обновить `<resource>.schema.test.ts` под новую Zod-схему `PagedResult`
