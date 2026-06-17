from playwright.sync_api import Page, expect

# Seed-данные из CookbookSeeder
SEED_EMAIL = "user@cookbook.local"
SEED_PASSWORD = "1234567890"


def _login(page: Page, base_url: str) -> None:
    """Вспомогательная функция: открыть /login и войти под seed-пользователем."""
    page.goto(f"{base_url}/login")
    page.fill("#email", SEED_EMAIL)
    page.fill("#password", SEED_PASSWORD)
    page.click("button[type=submit]")
    # Ждём редиректа на главную
    page.wait_for_url(f"{base_url}/", timeout=10_000)


def test_login_shows_user_chip(page: Page, base_url: str) -> None:
    """После входа в шапке появляется .user-chip с именем пользователя."""
    _login(page, base_url)

    user_chip = page.locator(".user-chip")
    expect(user_chip).to_be_visible()


def test_login_shows_new_recipe_button(page: Page, base_url: str) -> None:
    """После входа на главной странице появляется кнопка «Новый рецепт»."""
    _login(page, base_url)

    new_recipe_btn = page.get_by_text("+ Новый рецепт")
    expect(new_recipe_btn).to_be_visible()


def test_guest_has_no_new_recipe_button(page: Page, base_url: str) -> None:
    """Гость не видит кнопку «Новый рецепт»."""
    page.goto(base_url)

    new_recipe_btn = page.get_by_text("+ Новый рецепт")
    expect(new_recipe_btn).not_to_be_visible()


def test_guest_sees_login_button(page: Page, base_url: str) -> None:
    """Гость видит кнопку «Войти» в шапке."""
    page.goto(base_url)

    login_link = page.get_by_role("link", name="Войти")
    expect(login_link).to_be_visible()
