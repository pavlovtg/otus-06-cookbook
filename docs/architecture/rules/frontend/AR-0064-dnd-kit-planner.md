# AR-0064: DnD в планировщике — только @dnd-kit/core и @dnd-kit/utilities

Источник: ADR-0036

## Правило

- Drag-and-drop в планировщике меню реализуется исключительно через `@dnd-kit/core` и `@dnd-kit/utilities`.
- Используются хуки `useDraggable` (карточка рецепта в панели), `useDroppable` (слот сетки) и компонент `DragOverlay`.
- Визуальная подсветка слота при наведении — через CSS-класс `is-dragover` из `globals.css`; inline-стили для drag-состояний запрещены.

## Запрещено

- Подключать `@dnd-kit/sortable` и `@dnd-kit/modifiers` без отдельного ADR.
- Использовать нативный HTML5 DnD API (`draggable`, `ondragstart`, `ondrop`) в компонентах планировщика.
- Использовать `react-dnd`, `react-beautiful-dnd` или любые другие DnD-библиотеки.
