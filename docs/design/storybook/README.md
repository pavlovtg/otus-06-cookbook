# Storybook макета «Книга рецептов»

UI-прототип MVP в Storybook 8 (React + Vite). Без реального бэка — данные из `src/mock/fakeApi.ts`.

## Запуск

```bash
cd docs/design/storybook
npm install
npm run dev      # http://localhost:6006
npm run build    # сборка статического Storybook -> storybook-static/
npm run preview  # http-server storybook-static -p 6007
```

## Структура

- `src/components/` — UI-примитивы (`Button`, `Badge`, `RatingStars`, `Pager`, `Modal`, `Spinner`, `Notice`, `AppShell`, `RecipeCard`).
- `src/screens/` — экраны MVP (`RecipeList`, `RecipeDetail`, `RecipeForm`, `MealPlanner`, `ShoppingList`, `Favorites`, `Dashboard`, `LoginScreen`, `AppDemo`).
- `src/mock/` — мок-данные и `fakeApi.*` (≥27 рецептов, ≥55 ингредиентов, 2 пользователя, план меню, ≥23 комментариев).
- `src/styles/` — токены и компонентные стили, готовые к замене.
- `.storybook/preview-head.html` — CDN Chart.js для дашборда.

## Истории

- **Primitives/** — `Button`, `Badge`, `RatingStars`, `Pager`, `Misc` (Spinner / Notice / Modal).
- **Screens/** — каждый экран отдельно + `AppDemo (full)` — полное приложение с hash-роутингом.

## Покрытие MVP

Авторизация, CRUD рецепта, фильтры/поиск/сортировка, избранное, рейтинг, комментарии, масштабирование порций, планировщик меню с DnD, авто-генерация списка покупок, дашборд (Chart.js).

## Стилизация

Все ключевые узлы помечены комментариями `{/* style: ... */}` в JSX и `/* style: ... */` в CSS — удобно заменять визуальный язык.

## Тестовые пользователи

- `anna@test.ru` / `pass1234`
- `boris@test.ru` / `pass1234`
