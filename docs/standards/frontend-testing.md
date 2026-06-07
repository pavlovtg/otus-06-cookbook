# Стандарт тестирования frontend

## Назначение и область применения

Дополняет общий [стандарт тестирования](testing.md) применительно к `apps/web` (UI + BFF на Next.js).

## Правила

### Инструменты

- **Unit / component:** Vitest + `@testing-library/react`.
- **E2E:** Playwright (запускается против `docker compose up`).
- **Покрытие:** Vitest `--coverage` (v8 или istanbul).

### Целевые показатели

- Coverage по statements ≥ **80%** (унифицировано с backend).
- E2E покрывают минимум 3 пользовательских сценария (top-3 use cases): просмотр карточки рецепта, поиск/фильтрация, drag-and-drop планировщика.

### Что тестируется

- **Unit:** Zod-схемы (валидация и edge cases), чистые утилиты `lib/utils/`, BFF-агрегаторы с мокированным `fetch` к gateway.
- **Component:** ключевые UI-компоненты (формы рецепта, карточка, dnd-слот планировщика) — рендер, события, состояния loading/error/empty.
- **E2E:** регистрация/логин, CRUD рецепта, поиск, планировщик меню, генерация списка покупок.

### Что не тестируется в unit/component

- Третьестороний код (Next.js, React, shadcn/ui — доверяем upstream).
- Стилизация Tailwind (визуальная регрессия — out of scope MVP).

### CI

- Unit + component тесты — на каждый push и PR.
- E2E — на PR; должны проходить на свежем `docker compose up`.
- Падение блокирует merge (см. [Стандарт CI](ci-standard.md)).
