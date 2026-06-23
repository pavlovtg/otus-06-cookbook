import httpx
from playwright.sync_api import Page, expect

_API_BASE = "/api/cookbook/v1"


# ── Helpers ───────────────────────────────────────────────────────────────────

def _api_create_recipe(base_url: str, auth_token: str, title: str) -> str:
    resp = httpx.post(
        f"{base_url}{_API_BASE}/recipes",
        json={
            "title": title,
            "description": "Рецепт для UI-теста планировщика",
            "cookingTime": 30,
            "difficulty": "everyday",
            "servings": 2,
            "instructions": "Шаг 1.",
            "ingredients": [],
            "categoryIds": [],
            "isPublic": True,
        },
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert resp.status_code == 201, resp.text
    return resp.json()["id"]


def _api_clear_plan(base_url: str, auth_token: str) -> None:
    httpx.delete(
        f"{base_url}{_API_BASE}/meal-plan",
        headers={"Authorization": f"Bearer {auth_token}"},
    )


# ── Авторизация ───────────────────────────────────────────────────────────────

def test_planner_redirects_anonymous_to_login(page: Page, base_url: str) -> None:
    """Анонимный пользователь перенаправляется на /login."""
    page.goto(f"{base_url}/planner")

    expect(page).to_have_url(f"{base_url}/login", timeout=10_000)


# ── Базовый рендер ────────────────────────────────────────────────────────────

def test_planner_page_shows_heading(logged_in_page: Page, base_url: str) -> None:
    """Авторизованный пользователь видит заголовок «Планировщик меню»."""
    page = logged_in_page
    page.goto(f"{base_url}/planner")

    heading = page.locator("h1", has_text="Планировщик меню")
    expect(heading).to_be_visible()


def test_planner_page_shows_grid(logged_in_page: Page, base_url: str) -> None:
    """Сетка планировщика (.planner-grid) отображается."""
    page = logged_in_page
    page.goto(f"{base_url}/planner")

    grid = page.locator(".planner-grid")
    expect(grid).to_be_visible()


def test_planner_page_shows_panel(logged_in_page: Page, base_url: str) -> None:
    """Панель рецептов (.planner-panel) отображается."""
    page = logged_in_page
    page.goto(f"{base_url}/planner")

    panel = page.locator(".planner-panel")
    expect(panel).to_be_visible()


def test_planner_grid_has_7_day_columns(logged_in_page: Page, base_url: str) -> None:
    """Сетка содержит 7 колонок дней."""
    page = logged_in_page
    page.goto(f"{base_url}/planner")

    grid = page.locator(".planner-grid")
    expect(grid).to_be_visible()

    day_headers = page.locator(".planner-day-header")
    assert day_headers.count() == 7


def test_planner_grid_has_3_meal_rows(logged_in_page: Page, base_url: str) -> None:
    """Сетка содержит 3 строки типов приёма пищи."""
    page = logged_in_page
    page.goto(f"{base_url}/planner")

    meal_headers = page.locator(".planner-meal-header")
    assert meal_headers.count() == 3


# ── Кнопка «Очистить всё» ────────────────────────────────────────────────────

def test_planner_clear_button_is_visible(logged_in_page: Page, base_url: str) -> None:
    """Кнопка «Очистить всё» видна на странице планировщика."""
    page = logged_in_page
    page.goto(f"{base_url}/planner")

    clear_btn = page.locator("button", has_text="Очистить всё")
    expect(clear_btn).to_be_visible()


def test_planner_clear_button_opens_dialog(logged_in_page: Page, base_url: str) -> None:
    """Клик «Очистить всё» открывает диалог подтверждения."""
    page = logged_in_page
    page.goto(f"{base_url}/planner")

    page.locator("button", has_text="Очистить всё").click()

    dialog = page.locator("[role='dialog']")
    expect(dialog).to_be_visible()


def test_planner_clear_dialog_cancel_closes_dialog(logged_in_page: Page, base_url: str) -> None:
    """Кнопка «Отмена» в диалоге закрывает его."""
    page = logged_in_page
    page.goto(f"{base_url}/planner")

    page.locator("button", has_text="Очистить всё").click()

    dialog = page.locator("[role='dialog']")
    expect(dialog).to_be_visible()

    page.locator("[role='dialog'] button", has_text="Отмена").click()

    expect(dialog).not_to_be_visible()


def test_planner_clear_confirm_clears_plan(logged_in_page: Page, base_url: str, auth_token: str) -> None:
    """Подтверждение очистки удаляет все блюда из плана."""
    import uuid
    page = logged_in_page

    # Добавляем рецепт в план через API
    recipe_id = _api_create_recipe(base_url, auth_token, f"Рецепт для очистки {uuid.uuid4().hex[:6]}")
    httpx.put(
        f"{base_url}{_API_BASE}/meal-plan",
        json={
            "slots": [
                {
                    "weekDay": 1,
                    "mealType": 1,
                    "items": [{"recipeId": recipe_id, "servings": 2}],
                }
            ]
        },
        headers={"Authorization": f"Bearer {auth_token}"},
    )

    page.goto(f"{base_url}/planner")
    page.wait_for_load_state("networkidle")

    # Открываем диалог и подтверждаем очистку
    page.locator("button", has_text="Очистить всё").click()
    expect(page.locator("[role='dialog']")).to_be_visible()
    page.locator("[role='dialog'] button", has_text="Очистить").click()

    # Диалог закрывается
    expect(page.locator("[role='dialog']")).not_to_be_visible(timeout=10_000)

    # Слоты не содержат блюд
    slot_items = page.locator(".planner-slot-item")
    expect(slot_items).to_have_count(0, timeout=10_000)


# ── Навигация ─────────────────────────────────────────────────────────────────

def test_planner_link_in_header_visible_for_authenticated(logged_in_page: Page, base_url: str) -> None:
    """Пункт «Планировщик» в шапке виден для авторизованного пользователя."""
    page = logged_in_page
    page.goto(base_url)

    planner_link = page.locator("a[href='/planner']")
    expect(planner_link).to_be_visible()


def test_planner_link_in_header_not_visible_for_anonymous(page: Page, base_url: str) -> None:
    """Пункт «Планировщик» в шапке не виден для анонимного пользователя."""
    page.goto(base_url)

    planner_link = page.locator("a[href='/planner']")
    expect(planner_link).not_to_be_visible()
