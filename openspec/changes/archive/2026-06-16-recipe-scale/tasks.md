# recipe-scale

## 1. Client Component для блока ингредиентов

- [x] 1.1 Создать `apps/web/components/features/IngredientsCard.tsx` с директивой `"use client"`, принимающий props `ingredients: RecipeIngredientDto[]` и `baseServings: number`
- [x] 1.2 Добавить `useState<number>` для `currentServings`, инициализировать значением `baseServings`
- [x] 1.3 Реализовать контрол `.servings-control` (CSS-класс из `styles.css`): кнопки «−»/«+» с иконками `MinusIcon`/`PlusIcon` из `components/icons.tsx`, значение в `.value`; кнопка «−» `disabled` при `currentServings === 1`, «+» при `currentServings === 99`
- [x] 1.4 Реализовать пересчёт количества: `Math.round(baseAmount × (currentServings / baseServings) × 100) / 100`; отображать под списком подпись `на N порц.` (класс `.t-micro`)
- [x] 1.5 Использовать Storybook-компонент `src/domain/ServingsControl` как визуальный reference при реализации

## 2. Интеграция в детальную страницу

- [x] 2.1 Заменить статичный блок ингредиентов в `apps/web/app/recipes/[id]/page.tsx` на `<IngredientsCard ingredients={recipe.ingredients} baseServings={recipe.servings} />`

## 3. Тесты

- [x] 3.1 Создать `apps/web/tests/IngredientsCard.test.tsx`: проверить начальное отображение порций и количеств ингредиентов
- [x] 3.2 Добавить сценарий: клик «+» увеличивает порции и пересчитывает количества
- [x] 3.3 Добавить сценарий: клик «−» уменьшает порции и пересчитывает количества
- [x] 3.4 Добавить сценарий: кнопка «−» `disabled` при `currentServings === 1`
- [x] 3.5 Добавить сценарий: кнопка «+» `disabled` при `currentServings === 99`
- [x] 3.6 Добавить UI E2E тест в `tests/ui/test_recipes.py`: открыть детальную страницу рецепта с ингредиентами, нажать «+», проверить изменение отображаемого количества ингредиента
