# Прогресс

## Статус проекта

В разработке.

## Завершённые фичи

- Список рецептов (главная страница)
- Детальная страница рецепта
- Создание рецепта
- Редактирование рецепта
- Удаление рецепта
- CRUD ингредиентов (backend + тесты + frontend схемы/BFF + E2E тесты)
- `recipe-ingredients` — все 56 задач (секции 1–8) полностью реализованы

## В работе

Нет активных задач.

## Выполнено (prompt 16-cookbook-seeder)

- Рефакторинг загрузки начальных данных: `CookbookSeeder` (оркестратор, 3 транзакции)
- Удалены `IngredientSeeder` и `RecipeSeeder`
- `SeedData.Recipes` наполнены ингредиентами (все 10 рецептов)
- Обновлён AR-0054 (оркестратор, порядок транзакций, идемпотентность)
- Тесты: `CookbookSeederTests` (7 кейсов), удалены старые тесты сидеров

## Выполнено (последнее)

- Фикс UI-тестов ингредиентов и формы рецепта (четыре этапа):
  1. Добавлены BFF-роуты Next.js: `app/api/cookbook/v1/ingredients/route.ts` (GET, POST), `app/api/cookbook/v1/ingredients/[id]/route.ts` (GET, PUT, DELETE)
  2. Заменён `router.refresh()` на `router.push(pathname+search)` в `IngredientModal.tsx` и `DeleteIngredientButton.tsx`
  3. Добавлен `export const dynamic = "force-dynamic"` в 4 страницы: `(public)/page.tsx`, `ingredients/page.tsx`, `recipes/[id]/page.tsx`, `recipes/[id]/edit/page.tsx`
  4. `getIngredients`/`getIngredient` в `lib/bff/ingredients.ts` — выбор base по `typeof window`: SSR → `SERVER_BASE`, CSR → `CLIENT_BASE` (иначе браузер не мог достучаться до `http://api-gateway`)
