import uuid

import httpx

AUTH_BASE = "/api/cookbook/v1/auth"
RECIPES_BASE = "/api/cookbook/v1/recipes"

VALID_RECIPE = {
    "title": "Тестовый борщ",
    "description": "Классический украинский борщ",
    "cookingTime": 120,
    "difficulty": "everyday",
    "servings": 6,
    "instructions": "1. Сварить бульон. 2. Добавить овощи.",
    "ingredients": [],
    "categoryIds": [],
}


def _unique_email() -> str:
    return f"test_{uuid.uuid4().hex[:12]}@example.com"


def _register(base_url: str, email: str, password: str = "P@ssw0rd!") -> httpx.Response:
    return httpx.post(
        f"{base_url}{AUTH_BASE}/register",
        json={"email": email, "displayName": "Test User", "password": password},
    )


def _login(base_url: str, email: str, password: str = "P@ssw0rd!") -> httpx.Response:
    return httpx.post(
        f"{base_url}{AUTH_BASE}/login",
        json={"email": email, "password": password},
    )


# ── 10.1 ─────────────────────────────────────────────────────────────────────

def test_register_login_access_protected_endpoint(base_url: str) -> None:
    """Регистрация → логин → запрос к защищённому эндпоинту с токеном → 201."""
    email = _unique_email()

    register_resp = _register(base_url, email)
    assert register_resp.status_code in (200, 201), register_resp.text

    login_resp = _login(base_url, email)
    assert login_resp.status_code == 200, login_resp.text
    token = login_resp.json()["token"]
    assert token

    create_resp = httpx.post(
        f"{base_url}{RECIPES_BASE}",
        json=VALID_RECIPE,
        headers={"Authorization": f"Bearer {token}"},
    )
    assert create_resp.status_code == 201, create_resp.text


# ── 10.2 ─────────────────────────────────────────────────────────────────────

def test_create_recipe_without_token_returns_401(base_url: str) -> None:
    """Попытка создать рецепт без токена → 401."""
    response = httpx.post(f"{base_url}{RECIPES_BASE}", json=VALID_RECIPE)
    assert response.status_code == 401, response.text


# ── 10.3 ─────────────────────────────────────────────────────────────────────

def test_login_logout_protected_endpoint_returns_401(base_url: str) -> None:
    """Логин → логаут → повторный запрос к защищённому эндпоинту → 401."""
    email = _unique_email()

    _register(base_url, email)

    login_resp = _login(base_url, email)
    assert login_resp.status_code == 200, login_resp.text
    token = login_resp.json()["token"]
    assert token

    logout_resp = httpx.post(
        f"{base_url}{AUTH_BASE}/logout",
        headers={"Authorization": f"Bearer {token}"},
    )
    assert logout_resp.status_code in (200, 204), logout_resp.text

    create_resp = httpx.post(
        f"{base_url}{RECIPES_BASE}",
        json=VALID_RECIPE,
        headers={"Authorization": f"Bearer {token}"},
    )
    assert create_resp.status_code == 401, create_resp.text
