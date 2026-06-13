# Активный контекст

## Текущая задача

Нет активных задач.

## Что сделано в этой задаче

Исправлен падающий UI-тест `test_delete_recipe_cancel`.

- `DeleteRecipeButton.tsx`: добавлен `data-testid="delete-recipe-trigger"` на кнопку-триггер удаления
- `tests/ui/test_recipes.py`: селектор `.detail-toolbar button` с `has_text="Удалить"` заменён на `[data-testid='delete-recipe-trigger']`

Причина: в `.detail-toolbar` рендерились два `<button>Удалить</button>` — триггер и кнопка подтверждения в модале, что нарушало strict-режим Playwright.

## Следующий шаг

Нет активных задач.
