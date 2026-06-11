# Активный контекст

## Текущая задача

**Обновление стандарта структуры репозитория** (11.06.2026)

## Что сделано в этой сессии

- Создан Storybook-проект в `docs/design/storybook/` (React 19 + Storybook 8 + Vite).
- Порт `docs/design/mockup/` (single-file SPA) в React-компоненты 1:1 по стилю:
  - `src/styles.css`, `src/motion.css` — копия из mockup (единственный источник стилей).
  - `src/icons.tsx`, `src/photo.tsx`, `src/mocks.ts` — иконки, плейсхолдер-фото, seed-данные.
  - `src/components/` — примитивы: Button, Input, Tag, SearchInput, Segmented, Pagination, Modal, Toast, Skeleton.
  - `src/domain/` — доменные: RecipeCard, StarsRating, ServingsControl, IngredientList, CommentItem, Header, Planner, ShoppingList.
  - `src/dash/` — Kpi, BarChart (SVG), TopList, PlanFill.
- Истории по группам: `Foundation/*` (токены, иконки), `Primitives/*` (Button, Forms, Navigation, Feedback), `Domain/*` (RecipeCard, Recipe, Header, Planner, Dashboard). Каждая группа имеет ★ Playground с интерактивом (drag-and-drop планировщика, избранное на карточках, рейтинг звёздами, тосты).
- `pnpm build-storybook` → `storybook-static/` собирается без ошибок (8.12 s).
- README с инструкцией запуска и планом интеграции в `apps/web`.

## Следующий шаг

Перенести компоненты из `docs/design/storybook/src/` в `apps/web/components/` и подключить реальные BFF-вызовы.
