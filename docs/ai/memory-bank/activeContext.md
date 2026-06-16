# Активный контекст

## Текущая задача

Исправление падающих UI-тестов в `tests/ui/test_recipes.py`.

## Что сделано

### Тест 1: `test_recipe_detail_back_button_returns_to_list`

- Причина: `backHref = "/?page=1"` при клике с первой страницы, тест ожидал `/`
- Фикс: assertion изменён на regex `^base_url(/\?page=\d+|/?)$`

### Тесты 2/3: `test_create_recipe_with_categories_shows_tags_in_card`, `test_recipe_without_categories_card_shows_no_tags`

- Причина 1: рецепты сортируются по `Title ASC`, страница = 18 карточек. Названия "Тест..." начинаются на "Т" → попадают на страницу 3+.
- Фикс 1: добавлена `_navigate_to_recipe_card` — перебирает страницы пагинации пока не найдёт карточку.
- Причина 2: `wait_for_load_state("networkidle")` не ждёт обновления DOM при Next.js client-side navigation — цикл пропускал страницы.
- Фикс 2: заменено на `page.wait_for_url(lambda url, p=current_page: f"page={p}" in url)` + `button[aria-label='Вперёд']` вместо `has_text="→"`.

## Ключевые решения

- Тесты исправлены без изменения приложения
- `_navigate_to_recipe_card` — устойчивый паттерн для тестов с пагинацией
- При client-side navigation нужно ждать изменения URL, а не `networkidle`
