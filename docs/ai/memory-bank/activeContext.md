# Active Context

## Текущая задача

`ingredients-paging` — реализована пагинация списка ингредиентов.

## Что было сделано в последней сессии

- Реализована пагинация `GET /api/v1/ingredients`:
  - `PagedResult<T>` в `Application`; репозиторий возвращает `PagedResult<Ingredient>` через `CountAsync` + `Skip/Take`
  - Контроллер: параметры `page`, `pageSize`, `title`; clamp pageSize до 1000; валидация → 400
  - BFF: `getIngredients` принимает `page/pageSize`, возвращает `PagedIngredient`
  - Страница `/ingredients`: читает `page` из `searchParams`, компонент `Paginator`, `maxLength={200}` на поле поиска
  - Тесты: integration (репозиторий), microservice (контроллер), E2E API, UI E2E, frontend unit (Zod-схема)
