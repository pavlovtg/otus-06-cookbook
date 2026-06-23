import time
from playwright.sync_api import Page, expect


def _unique_name(base: str) -> str:
    return f"{base}-{int(time.time() * 1000) % 100000}"


def test_categories_page_redirects_anonymous(page: Page, base_url: str) -> None:
    """Анонимный пользователь перенаправляется на /login при попытке открыть /categories."""
    page.goto(f"{base_url}/categories")

    expect(page).to_have_url(f"{base_url}/login?from=%2Fcategories")


def test_categories_page_redirects_non_admin(logged_in_page: Page, base_url: str) -> None:
    """Авторизованный не-admin перенаправляется на / при попытке открыть /categories."""
    logged_in_page.goto(f"{base_url}/categories")

    expect(logged_in_page).to_have_url(f"{base_url}/")


def test_categories_nav_link_hidden_for_non_admin(logged_in_page: Page, base_url: str) -> None:
    """Ссылка «Категории» в навигации не видна не-admin пользователю."""
    logged_in_page.goto(f"{base_url}/")

    expect(logged_in_page.locator("nav a[href='/categories']")).not_to_be_visible()


def test_categories_nav_link_visible_for_admin(admin_page: Page, base_url: str) -> None:
    """Ссылка «Категории» в навигации видна администратору."""
    admin_page.goto(f"{base_url}/")

    expect(admin_page.locator("nav a[href='/categories']")).to_be_visible()


def test_categories_page_opens(admin_page: Page, base_url: str) -> None:
    """10.2 Страница /categories открывается для admin."""
    admin_page.goto(f"{base_url}/categories")

    expect(admin_page).to_have_url(f"{base_url}/categories")
    expect(admin_page.locator("[data-testid='category-group-cuisine']")).to_be_visible()


def test_categories_page_shows_groups(admin_page: Page, base_url: str) -> None:
    """10.2 На странице отображаются все 7 групп категорий."""
    admin_page.goto(f"{base_url}/categories")

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
        expect(admin_page.locator(f"[data-testid='category-group-{t}']")).to_be_visible()


def test_create_category_success(admin_page: Page, base_url: str) -> None:
    """10.2 Успешное создание категории через форму."""
    name = _unique_name("UI-тест кухня")
    admin_page.goto(f"{base_url}/categories")

    admin_page.locator("[data-testid='create-category-trigger']").click()

    modal = admin_page.locator(".modal-backdrop.is-open")
    expect(modal).to_be_visible()

    modal.locator("[data-testid='category-name-input']").fill(name)
    modal.locator("[data-testid='category-type-select']").select_option("cuisine")
    modal.locator("[data-testid='category-submit']").click()

    expect(admin_page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    admin_page.wait_for_load_state("networkidle")

    group = admin_page.locator("[data-testid='category-group-cuisine']")
    expect(group).to_contain_text(name)


def test_create_category_validation_error(admin_page: Page, base_url: str) -> None:
    """10.2 Ошибка валидации при создании категории с пустым именем."""
    admin_page.goto(f"{base_url}/categories")

    admin_page.locator("[data-testid='create-category-trigger']").click()

    modal = admin_page.locator(".modal-backdrop.is-open")
    expect(modal).to_be_visible()

    # Оставляем имя пустым
    modal.locator("[data-testid='category-name-input']").fill("")
    modal.locator("[data-testid='category-submit']").click()

    expect(modal.locator(".error-text")).to_be_visible()
    expect(modal).to_be_visible()


def test_edit_category_prefills_form(admin_page: Page, base_url: str) -> None:
    """10.2 Форма редактирования предзаполнена данными категории."""
    admin_page.goto(f"{base_url}/categories")

    first_tag = admin_page.locator("[data-testid='category-tag']").first
    category_name = first_tag.locator("span").first.inner_text()

    first_tag.locator("[data-testid='edit-category-trigger']").click()

    modal = admin_page.locator(".modal-backdrop.is-open")
    expect(modal).to_be_visible()
    expect(modal.locator("[data-testid='category-name-input']")).to_have_value(category_name)


def test_edit_category_success(admin_page: Page, base_url: str) -> None:
    """10.2 Успешное редактирование категории."""
    name = _unique_name("UI-тест редактирование")
    updated_name = _unique_name("UI-тест обновлённая")
    admin_page.goto(f"{base_url}/categories")

    # Создаём категорию
    admin_page.locator("[data-testid='create-category-trigger']").click()
    modal = admin_page.locator(".modal-backdrop.is-open")
    modal.locator("[data-testid='category-name-input']").fill(name)
    modal.locator("[data-testid='category-type-select']").select_option("cuisine")
    modal.locator("[data-testid='category-submit']").click()
    expect(admin_page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    admin_page.wait_for_load_state("networkidle")

    # Находим тег и редактируем
    group = admin_page.locator("[data-testid='category-group-cuisine']")
    tag = group.locator("[data-testid='category-tag']", has_text=name).first
    tag.locator("[data-testid='edit-category-trigger']").click()

    edit_modal = admin_page.locator(".modal-backdrop.is-open")
    edit_modal.locator("[data-testid='category-name-input']").fill(updated_name)
    edit_modal.locator("[data-testid='category-submit']").click()

    expect(admin_page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    admin_page.wait_for_load_state("networkidle")

    expect(group).to_contain_text(updated_name)


def test_delete_category_with_confirmation(admin_page: Page, base_url: str) -> None:
    """10.2 Удаление категории с подтверждением."""
    name = _unique_name("UI-тест удаление")
    admin_page.goto(f"{base_url}/categories")

    # Создаём категорию
    admin_page.locator("[data-testid='create-category-trigger']").click()
    modal = admin_page.locator(".modal-backdrop.is-open")
    modal.locator("[data-testid='category-name-input']").fill(name)
    modal.locator("[data-testid='category-type-select']").select_option("cuisine")
    modal.locator("[data-testid='category-submit']").click()
    expect(admin_page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    admin_page.wait_for_load_state("networkidle")

    # Удаляем
    group = admin_page.locator("[data-testid='category-group-cuisine']")
    tag = group.locator("[data-testid='category-tag']", has_text=name).first
    tag.locator("[data-testid='delete-category-trigger']").click()

    confirm_modal = admin_page.locator(".modal-backdrop.is-open")
    expect(confirm_modal).to_be_visible()

    confirm_modal.locator("[data-testid='delete-category-confirm']").click()

    expect(admin_page.locator(".modal-backdrop.is-open")).not_to_be_visible()
    admin_page.wait_for_load_state("networkidle")

    expect(group).not_to_contain_text(name)


def test_delete_category_cancel(admin_page: Page, base_url: str) -> None:
    """10.2 Отмена удаления категории."""
    admin_page.goto(f"{base_url}/categories")

    first_tag = admin_page.locator("[data-testid='category-tag']").first
    category_name = first_tag.locator("span").first.inner_text()

    first_tag.locator("[data-testid='delete-category-trigger']").click()

    confirm_modal = admin_page.locator(".modal-backdrop.is-open")
    expect(confirm_modal).to_be_visible()

    confirm_modal.locator("button", has_text="Отмена").click()

    expect(admin_page.locator(".modal-backdrop.is-open")).not_to_be_visible()

    group = admin_page.locator("[data-testid='category-tag']", has_text=category_name)
    expect(group.first).to_be_visible()
