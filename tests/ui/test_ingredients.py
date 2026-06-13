from playwright.sync_api import Page, expect


VALID_INGREDIENT = {
    "title": "UI-тест морковь",
    "unit": "г",
    "defaultAmount": "150",
    "category": "vegetables",
}

UPDATED_INGREDIENT = {
    "title": "UI-тест морковь обновлённая",
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
    page.goto(f"{base_url}/ingredients")

    page.locator("[data-testid='create-ingredient-trigger']").click()

    expect(page.locator(".modal-backdrop.is-open")).to_be_visible()

    page.fill("#ingredient-title", VALID_INGREDIENT["title"])
    page.fill("#ingredient-unit", VALID_INGREDIENT["unit"])
    page.fill("#ingredient-defaultAmount", VALID_INGREDIENT["defaultAmount"])
    page.select_option("#ingredient-category", VALID_INGREDIENT["category"])

    page.locator("button[type=submit]").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    expect(page.locator(".ingredients-list")).to_contain_text(VALID_INGREDIENT["title"])


def test_create_ingredient_validation_error(page: Page, base_url: str) -> None:
    """11.2 Ошибка валидации при создании ингредиента с коротким названием."""
    page.goto(f"{base_url}/ingredients")

    page.locator("[data-testid='create-ingredient-trigger']").click()

    page.fill("#ingredient-title", "А")
    page.fill("#ingredient-unit", VALID_INGREDIENT["unit"])
    page.fill("#ingredient-defaultAmount", VALID_INGREDIENT["defaultAmount"])
    page.select_option("#ingredient-category", VALID_INGREDIENT["category"])

    page.locator("button[type=submit]").click()

    expect(page.locator(".error-text")).to_be_visible()
    expect(page.locator(".modal-backdrop.is-open")).to_be_visible()


def test_edit_ingredient_prefills_form(page: Page, base_url: str) -> None:
    """11.3 Форма редактирования предзаполнена данными ингредиента."""
    page.goto(f"{base_url}/ingredients")

    first_item = page.locator(".ingredient-item").first
    ingredient_title = first_item.locator(".ingredient-title").inner_text()

    first_item.locator("[data-testid='edit-ingredient-trigger']").click()

    expect(page.locator(".modal-backdrop.is-open")).to_be_visible()
    expect(page.locator("#ingredient-title")).to_have_value(ingredient_title)


def test_edit_ingredient_success(page: Page, base_url: str) -> None:
    """11.3 Успешное редактирование ингредиента."""
    page.goto(f"{base_url}/ingredients")

    page.locator("[data-testid='create-ingredient-trigger']").click()
    page.fill("#ingredient-title", VALID_INGREDIENT["title"])
    page.fill("#ingredient-unit", VALID_INGREDIENT["unit"])
    page.fill("#ingredient-defaultAmount", VALID_INGREDIENT["defaultAmount"])
    page.select_option("#ingredient-category", VALID_INGREDIENT["category"])
    page.locator("button[type=submit]").click()
    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()

    item = page.locator(".ingredient-item", has_text=VALID_INGREDIENT["title"])
    item.locator("[data-testid='edit-ingredient-trigger']").click()

    page.fill("#ingredient-title", UPDATED_INGREDIENT["title"])
    page.fill("#ingredient-unit", UPDATED_INGREDIENT["unit"])
    page.locator("button[type=submit]").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    expect(page.locator(".ingredients-list")).to_contain_text(UPDATED_INGREDIENT["title"])


def test_delete_ingredient_with_confirmation(page: Page, base_url: str) -> None:
    """11.4 Удаление ингредиента с подтверждением."""
    page.goto(f"{base_url}/ingredients")

    page.locator("[data-testid='create-ingredient-trigger']").click()
    page.fill("#ingredient-title", VALID_INGREDIENT["title"])
    page.fill("#ingredient-unit", VALID_INGREDIENT["unit"])
    page.fill("#ingredient-defaultAmount", VALID_INGREDIENT["defaultAmount"])
    page.select_option("#ingredient-category", VALID_INGREDIENT["category"])
    page.locator("button[type=submit]").click()
    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()

    item = page.locator(".ingredient-item", has_text=VALID_INGREDIENT["title"])
    item.locator("[data-testid='delete-ingredient-trigger']").click()

    expect(page.locator(".modal-backdrop.is-open")).to_be_visible()

    page.locator("button", has_text="Удалить").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    expect(page.locator(".ingredients-list")).not_to_contain_text(VALID_INGREDIENT["title"])


def test_delete_ingredient_cancel(page: Page, base_url: str) -> None:
    """11.4 Отмена удаления ингредиента."""
    page.goto(f"{base_url}/ingredients")

    first_item = page.locator(".ingredient-item").first
    ingredient_title = first_item.locator(".ingredient-title").inner_text()

    first_item.locator("[data-testid='delete-ingredient-trigger']").click()

    expect(page.locator(".modal-backdrop.is-open")).to_be_visible()

    page.locator("button", has_text="Отмена").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    expect(page.locator(".ingredients-list")).to_contain_text(ingredient_title)


def test_filter_ingredients_by_title(page: Page, base_url: str) -> None:
    """11.5 Фильтрация ингредиентов по названию."""
    page.goto(f"{base_url}/ingredients")

    page.fill("[data-testid='filter-title']", "морк")

    items = page.locator(".ingredient-item")
    count = items.count()
    for i in range(count):
        title = items.nth(i).locator(".ingredient-title").inner_text()
        assert "морк" in title.lower()


def test_filter_ingredients_by_category(page: Page, base_url: str) -> None:
    """11.5 Фильтрация ингредиентов по категории."""
    page.goto(f"{base_url}/ingredients")

    page.select_option("[data-testid='filter-category']", "vegetables")

    items = page.locator(".ingredient-item")
    expect(items.first).to_be_visible()

    count = items.count()
    for i in range(count):
        category = items.nth(i).get_attribute("data-category")
        assert category == "vegetables"
