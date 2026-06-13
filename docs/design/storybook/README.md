# Cookbook DS — Storybook

Живой плейбук компонентов для приложения `apps/web`. Порт из `docs/design/mockup/` (CSS + JSX) на React + Storybook 8 (Vite).

## Запуск

```bash
cd docs/design/storybook
pnpm install
pnpm storybook        # dev: http://localhost:6006
pnpm build-storybook  # static: ./storybook-static
```

### Docker

```bash
cd docs/design/storybook
docker compose up        # http://localhost:6006
```

## Структура

- `src/styles.css`, `src/motion.css` — 1:1 копия из mockup, единственный источник стилей.
- `src/icons.tsx`, `src/photo.tsx` — иконки и плейсхолдер-фото рецепта.
- `src/mocks.ts` — seed-данные (рецепты, ингредиенты, категории, комментарии, избранное, план меню).
- `src/components/` — примитивы: `Button`, `Input`, `Tag`, `SearchInput`, `Segmented`, `Pagination`, `Modal`, `Toast`, `Skeleton`.
- `src/domain/` — доменные: `RecipeCard`, `StarsRating`, `ServingsControl`, `IngredientList`, `CommentItem`, `Header`, `Planner`, `ShoppingList`.
- `src/dash/` — дашборд: `Kpi`, `BarChart`, `TopList`, `PlanFill`.
- `src/stories/` — истории (одна на группу) с `Playground` ★ для интерактивных сценариев.

## Использование из `apps/web`

Сейчас это отдельный пакет `@cookbook/ds`. При интеграции:

1. Скопировать `src/styles.css` и `src/motion.css` в `apps/web/app/globals.css` (или импортировать как раздельные CSS).
2. Перенести компоненты в `apps/web/components/ui/` (примитивы) и `apps/web/components/features/` (домен).
3. Заменить mock-данные на реальные BFF-вызовы (`apps/web/lib/bff/`).
4. Stories остаются в этом пакете как design reference и регрессионная проверка.

## Стиль

Следует `docs/design/guide/STYLE_GUIDE.md`: Inter 400/500, акцент purple `#9968ff`, радиусы `8 / 12 / 100px`, край через inset+outer shadow без border.
