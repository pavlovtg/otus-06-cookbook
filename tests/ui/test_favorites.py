import httpx
from playwright.sync_api import Page, expect

_API_BASE = "/api/cookbook/v1"


# ── Helpers ───────────────────────────────────────────────────────────────────

def _api_create_recipe(base_url: str, auth_token: str, title: str) -> str:
    """Создаёт рецепт через API и возвращает его id."""
    resp = httpx.post(
        f"{base_url}{_API_BASE}/recipes",
        json={
            "title": title,
            "description": "Рецепт для UI-теста избранного",
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


def _navigate_to_recipe_card(page: Page, base_url: str, recipe_id: str) -> None:
    """Листает страницы пагинации, пока не найдёт карточку рецепта."""
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


# ── Переключатель режима «Избранное» ─────────────────────────────────────────

def test_favorites_mode_toggle_visible_for_authenticated(logged_in_page: Page, base_url: str) -> None:
    """8.1: Переключатель «Избранное» виден в сайдбаре для авторизованного пользователя."""
    page = logged_in_page
    page.goto(base_url)

    favorites_toggle = page.locator("[data-mode='favorites']")
    expect(favorites_toggle).to_be_visible()


def test_favorites_mode_toggle_not_visible_for_anonymous(page: Page, base_url: str) -> None:
    """Переключатель «Избранное» не виден для анонимного пользователя."""
    page.goto(base_url)

    favorites_toggle = page.locator("[data-mode='favorites']")
    expect(favorites_toggle).not_to_be_visible()


def test_click_favorites_mode_changes_url(logged_in_page: Page, base_url: str) -> None:
    """Клик на «Избранное» добавляет mode=favorites в URL."""
    page = logged_in_page
    page.goto(base_url)

    page.locator("[data-mode='favorites']").click()
    page.wait_for_url(lambda url: "mode=favorites" in url, timeout=5000)

    assert "mode=favorites" in page.url


def test_favorites_mode_shows_favorites_heading(logged_in_page: Page, base_url: str) -> None:
    """В режиме «Избранное» пункт сайдбара «Избранное» становится активным."""
    page = logged_in_page
    page.goto(f"{base_url}/?mode=favorites")

    active = page.locator("[data-mode='favorites'].is-active")
    expect(active).to_be_visible()


def test_click_all_recipes_mode_returns_to_normal(logged_in_page: Page, base_url: str) -> None:
    """Клик на «Все рецепты» убирает mode=favorites из URL."""
    page = logged_in_page
    page.goto(f"{base_url}/?mode=favorites")

    page.locator("[data-mode='all']").click()
    page.wait_for_load_state("networkidle")

    assert "mode=favorites" not in page.url


def test_favorites_mode_empty_state_shown_when_no_favorites(logged_in_page: Page, base_url: str) -> None:
    """Пустой стейт «В избранном пусто» отображается, если избранных нет."""
    page = logged_in_page

    # Регистрируем нового пользователя без избранного через API
    import uuid
    import httpx as _httpx
    email = f"fav_empty_{uuid.uuid4().hex[:10]}@example.com"
    password = "P@ssw0rd!"
    _httpx.post(
        f"{base_url}{_API_BASE}/auth/register",
        json={"email": email, "displayName": "Fav Empty User", "password": password},
    )
    resp = _httpx.post(
        f"{base_url}{_API_BASE}/auth/login",
        json={"email": email, "password": password},
    )
    assert resp.status_code == 200

    # Логинимся под новым пользователем через UI
    page.goto(f"{base_url}/login")
    page.fill("#email", email)
    page.fill("#password", password)
    page.click("button[type=submit]")
    page.wait_for_url(f"{base_url}/", timeout=15_000)

    page.goto(f"{base_url}/?mode=favorites")
    page.wait_for_load_state("networkidle")

    state = page.locator(".state")
    expect(state).to_be_visible()
    expect(state.locator(".state-eyebrow")).to_contain_text("В избранном пусто")


# ── Иконка-сердечко на карточке ──────────────────────────────────────────────

def test_favorite_icon_visible_on_card_for_authenticated(logged_in_page: Page, base_url: str, auth_token: str) -> None:
    """Иконка-сердечко (.btn-icon.photo-fav) видна на карточке для авторизованного пользователя."""
    page = logged_in_page
    import uuid
    recipe_id = _api_create_recipe(base_url, auth_token, f"Тест сердечко {uuid.uuid4().hex[:6]}")

    _navigate_to_recipe_card(page, base_url, recipe_id)

    card_link = page.locator(f"a[href*='/recipes/{recipe_id}']")
    expect(card_link).to_be_visible(timeout=10000)

    fav_btn = card_link.locator(".btn-icon.photo-fav")
    expect(fav_btn).to_be_visible()


def test_favorite_icon_not_visible_for_anonymous(page: Page, base_url: str) -> None:
    """Иконка-сердечко не видна для анонимного пользователя."""
    page.goto(base_url)

    cards = page.locator(".recipe-card")
    expect(cards.first).to_be_visible()

    fav_btns = page.locator(".btn-icon.photo-fav")
    expect(fav_btns).to_have_count(0)


def test_click_favorite_icon_adds_is_on_class(logged_in_page: Page, base_url: str, auth_token: str) -> None:
    """Клик на сердечко добавляет класс is-on (добавление в избранное)."""
    page = logged_in_page
    import uuid
    recipe_id = _api_create_recipe(base_url, auth_token, f"Тест добавить избранное {uuid.uuid4().hex[:6]}")

    _navigate_to_recipe_card(page, base_url, recipe_id)

    card_link = page.locator(f"a[href*='/recipes/{recipe_id}']")
    expect(card_link).to_be_visible(timeout=10000)

    fav_btn = card_link.locator(".btn-icon.photo-fav")
    expect(fav_btn).to_be_visible()

    # Убеждаемся, что изначально не в избранном
    initial_classes = fav_btn.get_attribute("class") or ""
    if "is-on" not in initial_classes:
        fav_btn.click()
        page.wait_for_timeout(500)
        expect(fav_btn).to_have_class("btn-icon photo-fav is-on")


def test_click_favorite_icon_twice_removes_is_on_class(logged_in_page: Page, base_url: str, auth_token: str) -> None:
    """Двойной клик на сердечко снимает класс is-on (удаление из избранного)."""
    page = logged_in_page
    import uuid
    recipe_id = _api_create_recipe(base_url, auth_token, f"Тест убрать избранное {uuid.uuid4().hex[:6]}")

    _navigate_to_recipe_card(page, base_url, recipe_id)

    card_link = page.locator(f"a[href*='/recipes/{recipe_id}']")
    expect(card_link).to_be_visible(timeout=10000)

    fav_btn = card_link.locator(".btn-icon.photo-fav")
    expect(fav_btn).to_be_visible()

    # Первый клик — добавляем
    fav_btn.click()
    page.wait_for_timeout(500)
    expect(fav_btn).to_have_class("btn-icon photo-fav is-on")

    # Второй клик — убираем
    fav_btn.click()
    page.wait_for_timeout(500)
    # После удаления класс is-on должен исчезнуть
    classes = fav_btn.get_attribute("class") or ""
    assert "is-on" not in classes, f"Ожидали отсутствие is-on после второго клика, получили: {classes}"


def test_added_recipe_appears_in_favorites_mode(logged_in_page: Page, base_url: str, auth_token: str) -> None:
    """Рецепт, добавленный в избранное, отображается в режиме «Избранное»."""
    page = logged_in_page
    import uuid
    recipe_id = _api_create_recipe(base_url, auth_token, f"Тест режим избранное {uuid.uuid4().hex[:6]}")

    _navigate_to_recipe_card(page, base_url, recipe_id)

    card_link = page.locator(f"a[href*='/recipes/{recipe_id}']")
    expect(card_link).to_be_visible(timeout=10000)

    fav_btn = card_link.locator(".btn-icon.photo-fav")
    # Добавляем в избранное, если ещё не добавлено
    classes = fav_btn.get_attribute("class") or ""
    if "is-on" not in classes:
        fav_btn.click()
        page.wait_for_timeout(500)

    # Переходим в режим «Избранное»
    page.goto(f"{base_url}/?mode=favorites&pageSize=1000")
    page.wait_for_load_state("networkidle")

    expect(page.locator(f"a[href*='/recipes/{recipe_id}']")).to_be_visible(timeout=10000)
