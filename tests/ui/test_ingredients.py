import time
from playwright.sync_api import Page, expect


def _unique_title(base: str) -> str:
    return f"{base}-{int(time.time() * 1000) % 100000}"


VALID_INGREDIENT = {
    "unit": "г",
    "defaultAmount": "150",
    "category": "vegetables",
}

UPDATED_INGREDIENT = {
    "unit": "шт.",
    "defaultAmount": "3",
    "category": "vegetables",
}


def test_ingredients_page_opens(page: Page, base_url: str) -> None:
    """11.1 Страница /ingredients открывается."""
    page.goto(f"{base_url}/ingredients")

    expect(page).to_have_url(f"{base_url}/ingredients")
    expect(page.locator(".ingredients-list")).to_be_visible()


def test_ingredients_list_shows_items(page: Page, base_url: str) -> None:
    """11.1 Список ингредиентов содержит элементы (системные данные)."""
    page.goto(f"{base_url}/ingredients")

    items = page.locator(".ingredient-item")
    expect(items.first).to_be_visible()


def test_create_ingredient_success(page: Page, base_url: str) -> None:
    """11.2 Успешное создание ингредиента через форму."""
    title = _unique_title("UI-тест морковь")
    page.goto(f"{base_url}/ingredients")

    page.locator("[data-testid='create-ingredient-trigger']").click()

    modal = page.locator(".modal-backdrop.is-open")
    expect(modal).to_be_visible()

    modal.locator("#ingredient-title").fill(title)
    modal.locator("#ingredient-unit").fill(VALID_INGREDIENT["unit"])
    modal.locator("#ingredient-defaultAmount").fill(VALID_INGREDIENT["defaultAmount"])
    modal.locator("#ingredient-category").select_option(VALID_INGREDIENT["category"])

    modal.locator("button[type=submit]").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    page.wait_for_load_state("networkidle")

    # Фильтруем по уникальному названию чтобы найти на любой странице
    page.goto(f"{base_url}/ingredients?title={title}")
    page.wait_for_load_state("networkidle")
    expect(page.locator(".ingredients-list")).to_contain_text(title)


def test_create_ingredient_validation_error(page: Page, base_url: str) -> None:
    """11.2 Ошибка валидации при создании ингредиента с коротким названием."""
    page.goto(f"{base_url}/ingredients")

    page.locator("[data-testid='create-ingredient-trigger']").click()

    modal = page.locator(".modal-backdrop.is-open")

    modal.locator("#ingredient-title").fill("А")
    modal.locator("#ingredient-unit").fill(VALID_INGREDIENT["unit"])
    modal.locator("#ingredient-defaultAmount").fill(VALID_INGREDIENT["defaultAmount"])
    modal.locator("#ingredient-category").select_option(VALID_INGREDIENT["category"])

    modal.locator("button[type=submit]").click()

    expect(modal.locator(".error-text")).to_be_visible()
    expect(modal).to_be_visible()


def test_edit_ingredient_prefills_form(page: Page, base_url: str) -> None:
    """11.3 Форма редактирования предзаполнена данными ингредиента."""
    page.goto(f"{base_url}/ingredients")

    first_item = page.locator(".ingredient-item").first
    ingredient_title = first_item.locator(".ingredient-title").inner_text()

    first_item.locator("[data-testid='edit-ingredient-trigger']").click()

    modal = page.locator(".modal-backdrop.is-open")
    expect(modal).to_be_visible()
    expect(modal.locator("#ingredient-title")).to_have_value(ingredient_title)


def test_edit_ingredient_success(page: Page, base_url: str) -> None:
    """11.3 Успешное редактирование ингредиента."""
    title = _unique_title("UI-тест морковь")
    updated_title = _unique_title("UI-тест морковь обновлённая")
    page.goto(f"{base_url}/ingredients")

    # Создаём ингредиент
    page.locator("[data-testid='create-ingredient-trigger']").click()
    modal = page.locator(".modal-backdrop.is-open")
    modal.locator("#ingredient-title").fill(title)
    modal.locator("#ingredient-unit").fill(VALID_INGREDIENT["unit"])
    modal.locator("#ingredient-defaultAmount").fill(VALID_INGREDIENT["defaultAmount"])
    modal.locator("#ingredient-category").select_option(VALID_INGREDIENT["category"])
    modal.locator("button[type=submit]").click()
    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    page.wait_for_load_state("networkidle")

    # Переходим на страницу с фильтром чтобы найти созданный ингредиент
    page.goto(f"{base_url}/ingredients?title={title}")
    page.wait_for_load_state("networkidle")

    item = page.locator(".ingredient-item", has_text=title).first
    item.locator("[data-testid='edit-ingredient-trigger']").click()

    edit_modal = page.locator(".modal-backdrop.is-open")
    edit_modal.locator("#ingredient-title").fill(updated_title)
    edit_modal.locator("#ingredient-unit").fill(UPDATED_INGREDIENT["unit"])
    edit_modal.locator("button[type=submit]").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    page.wait_for_load_state("networkidle")

    # Фильтруем по обновлённому названию
    page.goto(f"{base_url}/ingredients?title={updated_title}")
    page.wait_for_load_state("networkidle")
    expect(page.locator(".ingredients-list")).to_contain_text(updated_title)


def test_delete_ingredient_with_confirmation(page: Page, base_url: str) -> None:
    """11.4 Удаление ингредиента с подтверждением."""
    title = _unique_title("UI-тест удаление")
    page.goto(f"{base_url}/ingredients")

    # Создаём ингредиент
    page.locator("[data-testid='create-ingredient-trigger']").click()
    modal = page.locator(".modal-backdrop.is-open")
    modal.locator("#ingredient-title").fill(title)
    modal.locator("#ingredient-unit").fill(VALID_INGREDIENT["unit"])
    modal.locator("#ingredient-defaultAmount").fill(VALID_INGREDIENT["defaultAmount"])
    modal.locator("#ingredient-category").select_option(VALID_INGREDIENT["category"])
    modal.locator("button[type=submit]").click()
    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    page.wait_for_load_state("networkidle")

    # Переходим на страницу с фильтром чтобы найти созданный ингредиент
    page.goto(f"{base_url}/ingredients?title={title}")
    page.wait_for_load_state("networkidle")

    item = page.locator(".ingredient-item", has_text=title).first
    item.locator("[data-testid='delete-ingredient-trigger']").click()

    confirm_modal = page.locator(".modal-backdrop.is-open")
    expect(confirm_modal).to_be_visible()

    confirm_modal.locator("button", has_text="Удалить").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    # Полагаемся на auto-retrying assertion, чтобы переждать router.refresh()
    # и не словить "Execution context was destroyed" из-за навигации.
    expect(page.locator(".ingredient-item", has_text=title)).to_have_count(0)


def test_delete_ingredient_cancel(page: Page, base_url: str) -> None:
    """11.4 Отмена удаления ингредиента."""
    page.goto(f"{base_url}/ingredients")

    first_item = page.locator(".ingredient-item").first
    ingredient_title = first_item.locator(".ingredient-title").inner_text()

    first_item.locator("[data-testid='delete-ingredient-trigger']").click()

    confirm_modal = page.locator(".modal-backdrop.is-open")
    expect(confirm_modal).to_be_visible()

    confirm_modal.locator("button", has_text="Отмена").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    expect(page.locator(".ingredients-list")).to_contain_text(ingredient_title)


def test_filter_ingredients_by_title(page: Page, base_url: str) -> None:
    """11.5 Фильтрация ингредиентов по названию."""
    page.goto(f"{base_url}/ingredients")

    page.fill("[data-testid='filter-title']", "морк")
    page.locator("button[type=submit]", has_text="Найти").click()
    page.wait_for_load_state("networkidle")

    items = page.locator(".ingredient-item")
    count = items.count()
    for i in range(count):
        title = items.nth(i).locator(".ingredient-title").inner_text()
        assert "морк" in title.lower()


def test_filter_ingredients_by_category(page: Page, base_url: str) -> None:
    """11.5 Фильтрация ингредиентов по категории."""
    page.goto(f"{base_url}/ingredients")

    page.select_option("[data-testid='filter-category']", "vegetables")
    page.locator("button[type=submit]", has_text="Найти").click()
    page.wait_for_load_state("networkidle")

    items = page.locator(".ingredient-item")
    expect(items.first).to_be_visible()

    count = items.count()
    for i in range(count):
        category = items.nth(i).get_attribute("data-category")
        assert category == "vegetables"


def test_search_field_has_max_length_200(page: Page, base_url: str) -> None:
    """11.6 Поле поиска ограничено 200 символами."""
    page.goto(f"{base_url}/ingredients")

    search_input = page.locator("[data-testid='filter-title']")
    expect(search_input).to_be_visible()

    max_length = search_input.get_attribute("maxlength")
    assert max_length == "200"


def test_paginator_not_visible_when_single_page(page: Page, base_url: str) -> None:
    """11.7 Пагинатор не отображается, если результатов меньше одной страницы."""
    unique = f"zzz-no-such-ingredient-{int(time.time())}"
    page.goto(f"{base_url}/ingredients?title={unique}")
    page.wait_for_load_state("networkidle")

    paginator = page.locator("[data-testid='paginator']")
    assert paginator.count() == 0 or not paginator.is_visible()
