# Активный контекст

## Текущая задача

Нет активной задачи.

## Статус

`ingredients-crud` — реализовано, закоммичено, все правки применены. Готово к архивированию (`/opsx:archive ingredients-crud`).

## Что было сделано в последней сессии

- `fix(ingredients): render modal portal only when open` — исправлен баг с невидимыми ингредиентами на странице `/ingredients`. Причина: `backdrop-filter: blur()` на каждом `modal-backdrop` (по одному на каждый ингредиент) создавал stacking context и делал контент под ними невидимым. Решение: `{open && createPortal(...)}` вместо постоянного рендера с `opacity: 0`.
