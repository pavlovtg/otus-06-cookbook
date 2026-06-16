import re

from playwright.sync_api import Page, expect


BASE_URL_KEY = "base_url"

VALID_RECIPE = {
    "title": "UI-тест борщ",
    "description": "Классический борщ для UI-теста",
    "cookingTime": "60",
    "servings": "4",
    "instructions": "1. Сварить бульон. 2. Добавить овощи.",
}


def test_recipe_list_shows_cards(page: Page, base_url: str) -> None:
    """10.1 Список рецептов отображается с карточками."""
    page.goto(base_url)

    cards = page.locator(".recipe-card")
    expect(cards.first).to_be_visible()


def test_recipe_list_card_has_title(page: Page, base_url: str) -> None:
    """10.1 Карточка рецепта содержит название."""
    page.goto(base_url)

    card = page.locator(".recipe-card").first
    title = card.locator("h3")
    expect(title).to_be_visible()
    assert title.inner_text() != ""


def test_recipe_card_click_navigates_to_detail(page: Page, base_url: str) -> None:
    """10.2 Клик на карточку переходит на детальную страницу."""
    page.goto(base_url)

    card = page.locator(".recipe-card").first
    card.click()

    expect(page).to_have_url(re.compile(r"/recipes/"))
    expect(page.locator(".detail-bar")).to_be_visible()


def test_recipe_detail_page_shows_info(page: Page, base_url: str) -> None:
    """10.3 Детальная страница рецепта отображает информацию."""
    page.goto(base_url)
    page.locator(".recipe-card").first.click()

    expect(page.locator(".detail-bar .title")).to_be_visible()
    expect(page.locator(".instructions-text")).to_be_visible()


def test_recipe_detail_back_button_returns_to_list(page: Page, base_url: str) -> None:
    """10.4 Кнопка «Назад» возвращает к списку."""
    page.goto(base_url)
    page.locator(".recipe-card").first.click()

    page.locator(".back-btn").click()

    # /?page=1 и / — одно и то же (первая страница)
    expect(page).to_have_url(re.compile(r"^" + re.escape(base_url) + r"(/\?page=\d+|/?)$"))
    expect(page.locator(".recipes-grid")).to_be_visible()


def test_create_recipe_success(page: Page, base_url: str) -> None:
    """10.5 Успешное создание рецепта."""
    page.goto(f"{base_url}/recipes/new")

    page.fill("#title", VALID_RECIPE["title"])
    page.fill("#description", VALID_RECIPE["description"])
    page.fill("#cookingTime", VALID_RECIPE["cookingTime"])
    page.fill("#servings", VALID_RECIPE["servings"])
    page.select_option("#difficulty", "everyday")
    page.fill("#instructions", VALID_RECIPE["instructions"])

    page.click("button[type=submit]")

    expect(page).to_have_url(re.compile(r"/recipes/(?!new)"))
    expect(page.locator(".detail-bar .title")).to_contain_text(VALID_RECIPE["title"])


def test_create_recipe_validation_error(page: Page, base_url: str) -> None:
    """10.6 Ошибка валидации при создании рецепта с пустым названием."""
    page.goto(f"{base_url}/recipes/new")

    page.fill("#title", "")
    page.fill("#description", VALID_RECIPE["description"])
    page.fill("#cookingTime", VALID_RECIPE["cookingTime"])
    page.fill("#servings", VALID_RECIPE["servings"])
    page.fill("#instructions", VALID_RECIPE["instructions"])

    page.click("button[type=submit]")

    expect(page.locator(".error-text")).to_be_visible()
    expect(page).to_have_url(f"{base_url}/recipes/new")


def test_edit_recipe_prefills_form(page: Page, base_url: str) -> None:
    """10.7 Форма редактирования предзаполнена данными рецепта."""
    page.goto(base_url)
    page.locator(".recipe-card").first.click()

    recipe_title = page.locator(".detail-bar .title").inner_text()

    page.locator("a", has_text="Редактировать").click()

    expect(page.locator("#title")).to_have_value(recipe_title)


def test_delete_recipe_cancel(page: Page, base_url: str) -> None:
    """10.9 Отмена удаления рецепта."""
    page.goto(base_url)
    page.locator(".recipe-card").first.click()

    page.locator("[data-testid='delete-recipe-trigger']").click()

    expect(page.locator(".modal-backdrop.is-open")).to_be_visible()

    page.locator("button", has_text="Отмена").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    expect(page.locator(".detail-bar")).to_be_visible()


# ── Recipe Photos UI (TEST-7) ─────────────────────────────────────────────────

def test_recipe_card_with_photo_renders_img(page: Page, base_url: str) -> None:
    """TEST-7: Карточка рецепта с фото рендерит <img>."""
    page.goto(base_url)

    # Ищем карточку с <img> (рецепт с фото из seed-данных)
    cards_with_img = page.locator(".recipe-card .photo img")
    if cards_with_img.count() > 0:
        expect(cards_with_img.first).to_be_visible()
        src = cards_with_img.first.get_attribute("src")
        assert src is not None
        assert "/api/cookbook/v1/photos/" in src


def test_recipe_card_without_photo_renders_svg(page: Page, base_url: str) -> None:
    """TEST-7: Карточка рецепта без фото рендерит SVG-заглушку."""
    page.goto(base_url)

    # Ищем карточку с SVG (рецепт без фото)
    cards_with_svg = page.locator(".recipe-card .photo svg")
    if cards_with_svg.count() > 0:
        expect(cards_with_svg.first).to_be_visible()


def test_recipe_detail_photo_actions_visible(page: Page, base_url: str) -> None:
    """TEST-7: Кнопки управления фото видны на детальной странице."""
    page.goto(base_url)
    page.locator(".recipe-card").first.click()

    expect(page).to_have_url(re.compile(r"/recipes/"))

    # Кнопки управления фото должны присутствовать
    photo_actions = page.locator(".photo-actions")
    expect(photo_actions).to_be_visible()

    # Должна быть хотя бы одна кнопка управления фото
    buttons = photo_actions.locator("button")
    expect(buttons.first).to_be_visible()


# ── Photo buttons: upload / replace / delete (TEST-7.1–7.6) ──────────────────

PHOTO_FILE = "apps/seed/photos/4384def7-6e8f-45a8-97f3-db9acadeb427.png"
PHOTO_FILE_2 = "apps/seed/photos/0e2005ec-07ed-4b46-afe4-20920dc29698.jpeg"


def _create_recipe_and_open(page: Page, base_url: str, title: str) -> None:
    """Создаёт рецепт через UI и открывает его детальную страницу."""
    page.goto(f"{base_url}/recipes/new")
    page.fill("#title", title)
    page.fill("#description", "Описание для теста фото")
    page.fill("#cookingTime", "30")
    page.fill("#servings", "2")
    page.select_option("#difficulty", "everyday")
    page.fill("#instructions", "Шаг 1.")
    page.click("button[type=submit]")
    expect(page).to_have_url(re.compile(r"/recipes/(?!new)"))


def test_upload_photo_button_visible_when_no_photo(page: Page, base_url: str) -> None:
    """TEST-7.1: Новый рецепт без фото показывает кнопку «Загрузить фото»."""
    _create_recipe_and_open(page, base_url, "Тест фото 7.1")

    upload_btn = page.locator(".photo-actions button", has_text="Загрузить фото")
    expect(upload_btn).to_be_visible()

    # Кнопок «Заменить» и «Удалить» быть не должно
    expect(page.locator(".photo-actions button", has_text="Заменить фото")).not_to_be_visible()
    expect(page.locator(".photo-actions button", has_text="Удалить фото")).not_to_be_visible()


def test_upload_photo_uploads_file(page: Page, base_url: str) -> None:
    """TEST-7.2: Загрузка фото через кнопку «Загрузить фото»."""
    _create_recipe_and_open(page, base_url, "Тест фото 7.2")

    # Загружаем файл через скрытый input
    page.locator(".photo-actions input[type=file]").set_input_files(PHOTO_FILE)

    # После загрузки должны появиться «Заменить фото» и «Удалить фото»
    expect(page.locator(".photo-actions button", has_text="Заменить фото")).to_be_visible(timeout=15000)
    expect(page.locator(".photo-actions button", has_text="Удалить фото")).to_be_visible()
    expect(page.locator(".photo-actions button", has_text="Загрузить фото")).not_to_be_visible()

    # Фото должно отображаться в галерее
    expect(page.locator(".main-photo img")).to_be_visible()


def test_replace_photo_button_visible_when_has_photo(page: Page, base_url: str) -> None:
    """TEST-7.3: После загрузки фото видна кнопка «Заменить фото», но не «Загрузить фото»."""
    _create_recipe_and_open(page, base_url, "Тест фото 7.3")

    page.locator(".photo-actions input[type=file]").set_input_files(PHOTO_FILE)

    replace_btn = page.locator(".photo-actions button", has_text="Заменить фото")
    expect(replace_btn).to_be_visible(timeout=15000)
    expect(replace_btn).to_be_enabled()

    expect(page.locator(".photo-actions button", has_text="Загрузить фото")).not_to_be_visible()


def test_replace_photo_uploads_new_file(page: Page, base_url: str) -> None:
    """TEST-7.4: Кнопка «Заменить фото» загружает новый файл — фото остаётся видимым."""
    _create_recipe_and_open(page, base_url, "Тест фото 7.4")

    # Загружаем первое фото
    page.locator(".photo-actions input[type=file]").set_input_files(PHOTO_FILE)
    img = page.locator(".main-photo img")
    expect(img).to_be_visible(timeout=15000)

    # Заменяем на второе фото через кнопку «Заменить фото»
    expect(page.locator(".photo-actions button", has_text="Заменить фото")).to_be_visible()
    page.locator(".photo-actions input[type=file]").set_input_files(PHOTO_FILE_2)

    # После замены фото по-прежнему отображается, кнопки «Заменить»/«Удалить» видны
    expect(img).to_be_visible(timeout=15000)
    expect(page.locator(".photo-actions button", has_text="Заменить фото")).to_be_visible()
    expect(page.locator(".photo-actions button", has_text="Удалить фото")).to_be_visible()
    expect(page.locator(".photo-actions button", has_text="Загрузить фото")).not_to_be_visible()


def test_delete_photo_cancel(page: Page, base_url: str) -> None:
    """TEST-7.5: Отмена удаления фото — фото остаётся."""
    _create_recipe_and_open(page, base_url, "Тест фото 7.5")

    page.locator(".photo-actions input[type=file]").set_input_files(PHOTO_FILE)
    expect(page.locator(".main-photo img")).to_be_visible(timeout=15000)

    # Отклоняем confirm-диалог
    page.on("dialog", lambda d: d.dismiss())
    page.locator(".photo-actions button", has_text="Удалить фото").click()

    # Фото и кнопки «Заменить»/«Удалить» остаются
    expect(page.locator(".main-photo img")).to_be_visible()
    expect(page.locator(".photo-actions button", has_text="Удалить фото")).to_be_visible()


def test_delete_photo_confirm(page: Page, base_url: str) -> None:
    """TEST-7.6: Подтверждение удаления фото — фото удаляется, появляется «Загрузить фото»."""
    _create_recipe_and_open(page, base_url, "Тест фото 7.6")

    page.locator(".photo-actions input[type=file]").set_input_files(PHOTO_FILE)
    expect(page.locator(".main-photo img")).to_be_visible(timeout=15000)
    expect(page.locator(".photo-actions button", has_text="Удалить фото")).to_be_visible()

    # Подтверждаем confirm-диалог
    page.on("dialog", lambda d: d.accept())
    page.locator(".photo-actions button", has_text="Удалить фото").click()

    # Фото удалено — должна появиться кнопка «Загрузить фото»
    expect(page.locator(".photo-actions button", has_text="Загрузить фото")).to_be_visible(timeout=15000)
    expect(page.locator(".photo-actions button", has_text="Удалить фото")).not_to_be_visible()
    expect(page.locator(".main-photo img")).not_to_be_visible()


# ── Recipe Categories UI (8.1–8.2) ───────────────────────────────────────────

def _navigate_to_recipe_card(page: Page, base_url: str, recipe_id: str) -> None:
    """Переходит на страницу пагинации, где находится карточка рецепта."""
    page.goto(base_url)
    current_page = 1
    for _ in range(20):
        if page.locator(f"a[href*='/recipes/{recipe_id}']").count() > 0:
            return
        next_btn = page.locator("button[aria-label='Вперёд']")
        if next_btn.is_disabled():
            break
        current_page += 1
        next_btn.click()
        page.wait_for_url(lambda url, p=current_page: f"page={p}" in url, timeout=5000)


def _create_recipe_with_categories(page: Page, base_url: str, title: str) -> None:
    """Создаёт рецепт с категориями через UI."""
    page.goto(f"{base_url}/recipes/new")
    page.fill("#title", title)
    page.fill("#description", "Описание для теста категорий")
    page.fill("#cookingTime", "30")
    page.fill("#servings", "2")
    page.select_option("#difficulty", "everyday")
    page.fill("#instructions", "Шаг 1.")

    # Добавляем категорию через CategoryTagInput
    cat_input = page.locator("[data-testid='category-search-input']")
    cat_input.fill("вар")
    # Ждём появления подсказок и выбираем первую
    suggestion = page.locator("[data-testid='category-suggestion']").first
    expect(suggestion).to_be_visible(timeout=5000)
    suggestion.click()

    page.click("button[type=submit]")
    expect(page).to_have_url(re.compile(r"/recipes/(?!new)"), timeout=10000)


def test_create_recipe_with_categories_shows_tags_in_card(page: Page, base_url: str) -> None:
    """8.1: Создать рецепт с категориями → категории отображаются в карточке."""
    title = "Тест категорий 8.1"
    _create_recipe_with_categories(page, base_url, title)

    # Запоминаем ID рецепта из URL детальной страницы
    recipe_id = page.url.split("/recipes/")[1].split("?")[0]

    # Переходим на страницу пагинации, где находится карточка
    _navigate_to_recipe_card(page, base_url, recipe_id)

    # Находим карточку по ссылке на конкретный рецепт
    card_link = page.locator(f"a[href*='/recipes/{recipe_id}']")
    expect(card_link).to_be_visible(timeout=10000)
    card = card_link.locator(".recipe-card")

    # В карточке должны быть теги категорий
    tags = card.locator(".tags .tag")
    expect(tags.first).to_be_visible()


def test_create_recipe_with_categories_shows_tags_on_detail(page: Page, base_url: str) -> None:
    """8.1: Создать рецепт с категориями → категории отображаются на детальной странице."""
    title = "Тест категорий 8.1 detail"
    _create_recipe_with_categories(page, base_url, title)

    # Уже на детальной странице после создания
    expect(page).to_have_url(re.compile(r"/recipes/(?!new)"))

    # В .detail-tags должны быть теги категорий
    detail_tags = page.locator(".detail-tags .tag")
    expect(detail_tags.first).to_be_visible()


def test_recipe_without_categories_card_shows_no_tags(page: Page, base_url: str) -> None:
    """8.3: Рецепт без категорий — карточка не показывает теги."""
    title = "Тест без категорий 8.3 карточка"
    # Создаём рецепт без категорий
    page.goto(f"{base_url}/recipes/new")
    page.fill("#title", title)
    page.fill("#description", "Описание без категорий")
    page.fill("#cookingTime", "90")
    page.fill("#servings", "6")
    page.select_option("#difficulty", "everyday")
    page.fill("#instructions", "Шаг 1.")
    page.click("button[type=submit]")
    expect(page).to_have_url(re.compile(r"/recipes/(?!new)"), timeout=10000)

    # Запоминаем ID рецепта из URL детальной страницы
    recipe_id = page.url.split("/recipes/")[1].split("?")[0]

    # Переходим на страницу пагинации, где находится карточка
    _navigate_to_recipe_card(page, base_url, recipe_id)

    # Находим карточку по ссылке на конкретный рецепт
    card_link = page.locator(f"a[href*='/recipes/{recipe_id}']")
    expect(card_link).to_be_visible(timeout=10000)
    card = card_link.locator(".recipe-card")

    # В карточке не должно быть тегов
    tags = card.locator(".tags .tag")
    expect(tags).to_have_count(0)


def test_recipe_without_categories_detail_shows_no_tags(page: Page, base_url: str) -> None:
    """8.3: Рецепт без категорий — детальная страница не показывает теги."""
    title = "Тест без категорий 8.3 детальная"
    # Создаём рецепт без категорий
    page.goto(f"{base_url}/recipes/new")
    page.fill("#title", title)
    page.fill("#description", "Описание без категорий")
    page.fill("#cookingTime", "90")
    page.fill("#servings", "6")
    page.select_option("#difficulty", "everyday")
    page.fill("#instructions", "Шаг 1.")
    page.click("button[type=submit]")
    expect(page).to_have_url(re.compile(r"/recipes/(?!new)"), timeout=10000)

    # На детальной странице .detail-tags не должен содержать тегов
    detail_tags = page.locator(".detail-tags .tag")
    expect(detail_tags).to_have_count(0)


# ── Pagination UI (7.4) ───────────────────────────────────────────────────────

def test_pagination_is_visible_on_recipes_list(page: Page, base_url: str) -> None:
    """7.4: Пагинация отображается на странице списка рецептов (seed > 18 рецептов)."""
    page.goto(base_url)

    pagination = page.locator(".pagination")
    expect(pagination).to_be_visible()


def test_pagination_next_page_shows_different_cards(page: Page, base_url: str) -> None:
    """7.4: Нажатие кнопки следующей страницы показывает другой набор карточек."""
    page.goto(base_url)

    # Собираем id карточек на первой странице
    cards_page1 = page.locator(".recipe-card")
    expect(cards_page1.first).to_be_visible()
    titles_page1 = [cards_page1.nth(i).locator("h3").inner_text() for i in range(cards_page1.count())]

    # Нажимаем кнопку «Вперёд»
    next_btn = page.locator(".pagination .page-btn[aria-label='Вперёд']")
    expect(next_btn).to_be_visible()
    expect(next_btn).to_be_enabled()
    next_btn.click()

    # Ждём обновления страницы
    page.wait_for_url(lambda url: "page=2" in url, timeout=5000)

    # Собираем карточки на второй странице
    cards_page2 = page.locator(".recipe-card")
    expect(cards_page2.first).to_be_visible()
    titles_page2 = [cards_page2.nth(i).locator("h3").inner_text() for i in range(cards_page2.count())]

    # Наборы карточек должны отличаться
    assert titles_page1 != titles_page2


def test_pagination_active_page_button_highlighted(page: Page, base_url: str) -> None:
    """7.4: Кнопка текущей страницы имеет класс is-active."""
    page.goto(base_url)

    active_btn = page.locator(".pagination .page-btn.is-active")
    expect(active_btn).to_be_visible()
    assert active_btn.inner_text() == "1"


# ── Search & Sort UI (5.1–5.2) ───────────────────────────────────────────────

def test_search_filters_recipe_list(page: Page, base_url: str) -> None:
    """5.1 Ввод поискового запроса фильтрует список рецептов."""
    page.goto(base_url)

    # Запоминаем исходное количество карточек
    cards = page.locator(".recipe-card")
    expect(cards.first).to_be_visible()
    initial_count = cards.count()

    # Вводим заведомо несуществующий запрос
    search_input = page.locator("input.search-input")
    expect(search_input).to_be_visible()
    search_input.fill("xyzzy_no_such_recipe_12345")

    # Ждём debounce (300 мс) + навигации
    page.wait_for_timeout(600)
    page.wait_for_load_state("networkidle")

    # Либо карточек стало меньше, либо показан пустой стейт
    new_count = page.locator(".recipe-card").count()
    state_visible = page.locator(".state").is_visible()
    assert new_count < initial_count or state_visible, (
        f"Ожидали фильтрацию: было {initial_count} карточек, стало {new_count}, state={state_visible}"
    )


def test_sort_changes_card_order(page: Page, base_url: str) -> None:
    """5.2 Переключение сортировки меняет порядок карточек."""
    page.goto(base_url)

    # Убеждаемся, что карточек достаточно для проверки порядка
    cards = page.locator(".recipe-card")
    expect(cards.first).to_be_visible()
    assert cards.count() >= 2, "Нужно минимум 2 карточки для проверки сортировки"

    # Сортировка А → Я
    sort_asc = page.locator(".aside-item", has_text="А → Я")
    expect(sort_asc).to_be_visible()
    sort_asc.click()
    page.wait_for_load_state("networkidle")

    cards_asc = page.locator(".recipe-card")
    expect(cards_asc.first).to_be_visible()
    titles_asc = [cards_asc.nth(i).locator("h3").inner_text() for i in range(cards_asc.count())]

    # Сортировка Я → А
    sort_desc = page.locator(".aside-item", has_text="Я → А")
    expect(sort_desc).to_be_visible()
    sort_desc.click()
    page.wait_for_load_state("networkidle")

    cards_desc = page.locator(".recipe-card")
    expect(cards_desc.first).to_be_visible()
    titles_desc = [cards_desc.nth(i).locator("h3").inner_text() for i in range(cards_desc.count())]

    # Порядок должен отличаться
    assert titles_asc != titles_desc, (
        "Ожидали разный порядок карточек при сортировке А→Я и Я→А"
    )


def test_edit_recipe_categories_update(page: Page, base_url: str) -> None:
    """8.2: Редактировать рецепт → категории обновляются."""
    title = "Тест категорий 8.2"
    _create_recipe_with_categories(page, base_url, title)

    # Открываем редактирование
    page.locator("a", has_text="Редактировать").click()
    expect(page).to_have_url(re.compile(r"/recipes/.+/edit"), timeout=10000)

    # Проверяем, что категория предзаполнена (чип присутствует)
    chip = page.locator("[data-testid='category-chip']").first
    expect(chip).to_be_visible()

    # Добавляем ещё одну категорию другого типа
    cat_input = page.locator("[data-testid='category-search-input']")
    cat_input.fill("итал")
    suggestion = page.locator("[data-testid='category-suggestion']").first
    expect(suggestion).to_be_visible(timeout=5000)
    suggestion.click()

    # Сохраняем
    page.click("button[type=submit]")
    expect(page).to_have_url(re.compile(r"/recipes/(?!.+/edit)"), timeout=10000)

    # На детальной странице должно быть минимум 2 тега
    detail_tags = page.locator(".detail-tags .tag")
    expect(detail_tags).to_have_count(2, timeout=5000)
