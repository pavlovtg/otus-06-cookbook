import time
from playwright.sync_api import Page, expect


def _unique_name(base: str) -> str:
    return f"{base}-{int(time.time() * 1000) % 100000}"


def test_categories_page_opens(page: Page, base_url: str) -> None:
    """10.2 Страница /categories открывается."""
    page.goto(f"{base_url}/categories")

    expect(page).to_have_url(f"{base_url}/categories")
    expect(page.locator("[data-testid='category-group-cuisine']")).to_be_visible()


def test_categories_page_shows_groups(page: Page, base_url: str) -> None:
    """10.2 На странице отображаются все 7 групп категорий."""
    page.goto(f"{base_url}/categories")

    types = [
        "meal_role",
        "cooking_method",
        "main_ingredient",
        "cuisine",
        "meal_time",
        "dietary",
        "serving_form",
    ]
    for t in types:
        expect(page.locator(f"[data-testid='category-group-{t}']")).to_be_visible()


def test_create_category_success(page: Page, base_url: str) -> None:
    """10.2 Успешное создание категории через форму."""
    name = _unique_name("UI-тест кухня")
    page.goto(f"{base_url}/categories")

    page.locator("[data-testid='create-category-trigger']").click()

    modal = page.locator(".modal-backdrop.is-open")
    expect(modal).to_be_visible()

    modal.locator("[data-testid='category-name-input']").fill(name)
    modal.locator("[data-testid='category-type-select']").select_option("cuisine")
    modal.locator("[data-testid='category-submit']").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    page.wait_for_load_state("networkidle")

    group = page.locator("[data-testid='category-group-cuisine']")
    expect(group).to_contain_text(name)


def test_create_category_validation_error(page: Page, base_url: str) -> None:
    """10.2 Ошибка валидации при создании категории с пустым именем."""
    page.goto(f"{base_url}/categories")

    page.locator("[data-testid='create-category-trigger']").click()

    modal = page.locator(".modal-backdrop.is-open")
    expect(modal).to_be_visible()

    # Оставляем имя пустым
    modal.locator("[data-testid='category-name-input']").fill("")
    modal.locator("[data-testid='category-submit']").click()

    expect(modal.locator(".error-text")).to_be_visible()
    expect(modal).to_be_visible()


def test_edit_category_prefills_form(page: Page, base_url: str) -> None:
    """10.2 Форма редактирования предзаполнена данными категории."""
    page.goto(f"{base_url}/categories")

    first_tag = page.locator("[data-testid='category-tag']").first
    category_name = first_tag.locator("span").first.inner_text()

    first_tag.locator("[data-testid='edit-category-trigger']").click()

    modal = page.locator(".modal-backdrop.is-open")
    expect(modal).to_be_visible()
    expect(modal.locator("[data-testid='category-name-input']")).to_have_value(category_name)


def test_edit_category_success(page: Page, base_url: str) -> None:
    """10.2 Успешное редактирование категории."""
    name = _unique_name("UI-тест редактирование")
    updated_name = _unique_name("UI-тест обновлённая")
    page.goto(f"{base_url}/categories")

    # Создаём категорию
    page.locator("[data-testid='create-category-trigger']").click()
    modal = page.locator(".modal-backdrop.is-open")
    modal.locator("[data-testid='category-name-input']").fill(name)
    modal.locator("[data-testid='category-type-select']").select_option("cuisine")
    modal.locator("[data-testid='category-submit']").click()
    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    page.wait_for_load_state("networkidle")

    # Находим тег и редактируем
    group = page.locator("[data-testid='category-group-cuisine']")
    tag = group.locator("[data-testid='category-tag']", has_text=name).first
    tag.locator("[data-testid='edit-category-trigger']").click()

    edit_modal = page.locator(".modal-backdrop.is-open")
    edit_modal.locator("[data-testid='category-name-input']").fill(updated_name)
    edit_modal.locator("[data-testid='category-submit']").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    page.wait_for_load_state("networkidle")

    expect(group).to_contain_text(updated_name)


def test_delete_category_with_confirmation(page: Page, base_url: str) -> None:
    """10.2 Удаление категории с подтверждением."""
    name = _unique_name("UI-тест удаление")
    page.goto(f"{base_url}/categories")

    # Создаём категорию
    page.locator("[data-testid='create-category-trigger']").click()
    modal = page.locator(".modal-backdrop.is-open")
    modal.locator("[data-testid='category-name-input']").fill(name)
    modal.locator("[data-testid='category-type-select']").select_option("cuisine")
    modal.locator("[data-testid='category-submit']").click()
    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    page.wait_for_load_state("networkidle")

    # Удаляем
    group = page.locator("[data-testid='category-group-cuisine']")
    tag = group.locator("[data-testid='category-tag']", has_text=name).first
    tag.locator("[data-testid='delete-category-trigger']").click()

    confirm_modal = page.locator(".modal-backdrop.is-open")
    expect(confirm_modal).to_be_visible()

    confirm_modal.locator("[data-testid='delete-category-confirm']").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    page.wait_for_load_state("networkidle")

    expect(group).not_to_contain_text(name)


def test_delete_category_cancel(page: Page, base_url: str) -> None:
    """10.2 Отмена удаления категории."""
    page.goto(f"{base_url}/categories")

    first_tag = page.locator("[data-testid='category-tag']").first
    category_name = first_tag.locator("span").first.inner_text()

    first_tag.locator("[data-testid='delete-category-trigger']").click()

    confirm_modal = page.locator(".modal-backdrop.is-open")
    expect(confirm_modal).to_be_visible()

    confirm_modal.locator("button", has_text="Отмена").click()

    expect(page.locator(".modal-backdrop.is-open")).not_to_be_visible()

    group = page.locator("[data-testid='category-tag']", has_text=category_name)
    expect(group.first).to_be_visible()
