# recipes-search

## 1. Backend — фильтрация и сортировка

- [x] 1.1 Добавить параметры `q` (string?) и `sort` (enum: `title_asc`, `title_desc`) в `IRecipeRepository.GetRecipesPagedAsync`
- [x] 1.2 Реализовать фильтрацию в `RecipeRepository`: LIKE по словам запроса по полям `title`, `description`, `categories.name`, `ingredients.title` (AND-логика)
- [x] 1.3 Реализовать сортировку в `RecipeRepository`: `OrderBy(title)` / `OrderByDescending(title)` в зависимости от `sort`
- [x] 1.4 Пробросить параметры `q` и `sort` через `IRecipeService` и `RecipeService`
- [x] 1.5 Добавить query-параметры `q` и `sort` в `RecipesController.GetRecipes`

## 2. Backend — тесты

- [x] 2.1 Добавить unit-тесты `RecipeService` для поиска и сортировки
- [x] 2.2 Добавить integration-тесты `RecipeRepository` для фильтрации по каждому полю (title, description, category, ingredient)
- [x] 2.3 Добавить e2e API-тесты в `tests/e2e/test_recipes_api.py`: поиск по одному слову, по нескольким словам, пустой результат, сортировка `title_asc`, сортировка `title_desc`

## 3. Frontend BFF

- [ ] 3.1 Обновить `lib/bff/recipes.ts`: `getRecipes` принимает `q` и `sort`, пробрасывает в upstream URL
- [ ] 3.2 Обновить `app/api/cookbook/v1/recipes/route.ts`: проброс параметров `q` и `sort` в gateway
- [ ] 3.3 Добавить unit-тесты BFF: `recipes.bff.test.ts` — проверить что `q` и `sort` попадают в URL запроса

## 4. Frontend UI

> Макет: `docs/design/mockup/index.html` + `styles.css` — эталон внешнего вида и CSS-классов.
> Компоненты разрабатываются в Storybook (`docs/design/storybook/`), затем переносятся в `apps/web`.
> Стиль: поисковый инпут — класс `search-input` внутри `search-wrap`; автодополнение — класс `autocomplete` + `autocomplete-item`; переключатель сортировки — `aside-item` в сайдбаре (см. макет, секция «Сортировка»).

- [ ] 4.1 Добавить поисковый инпут на страницу `app/(public)/page.tsx`; значение читается из `searchParams.q`; стиль по макету (`.search-wrap` + `.search-input` + иконка поиска)
- [ ] 4.2 Добавить переключатель сортировки (`title_asc` / `title_desc`) в сайдбар; значение читается из `searchParams.sort`; стиль — `.aside-item` с `.is-active` для активного варианта
- [ ] 4.3 При изменении поиска или сортировки обновлять URL (router.push с новыми searchParams), сбрасывать `page=1`
- [ ] 4.4 Реализовать автодополнение: при вводе ≥ 2 символов последнего слова показывать подсказки из загруженных категорий и ингредиентов; стиль — `.autocomplete` + `.autocomplete-item` + `.kind` (см. макет)
- [ ] 4.5 При выборе подсказки подставлять слово в поисковую строку
- [ ] 4.6 Добавить story в Storybook для поискового инпута с автодополнением (`docs/design/storybook/src/stories/`)

## 5. Frontend — тесты

- [ ] 5.1 Добавить UI e2e-тест в `tests/ui/test_recipes.py`: ввод поискового запроса фильтрует список рецептов
- [ ] 5.2 Добавить UI e2e-тест: переключение сортировки меняет порядок карточек
