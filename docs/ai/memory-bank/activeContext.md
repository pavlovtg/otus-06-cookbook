# Active Context

## Текущая задача

Архивирование change `recipe-categories`.

## Что было сделано в последней сессии

- Заархивирован change `recipe-categories` → `openspec/changes/archive/2026-06-16-recipe-categories/`
- Синхронизированы delta specs с main specs:
  - Созданы новые: `openspec/specs/recipe-category-assign/spec.md`, `openspec/specs/recipe-category-display/spec.md`
  - Обновлены: `recipe-create`, `recipe-detail`, `recipe-edit`

## Ключевые решения

- `categoryIds` в схемах — required (не optional), т.к. backend всегда возвращает массив
- `RecipeCard` fallback убран: пустой `categoryIds` → пустой блок тегов
- Замена при совпадении типа реализована в `CategoryTagInput.addCategory`
- E2E тесты используют `data-testid` атрибуты компонента
