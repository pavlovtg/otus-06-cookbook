import httpx
import uuid

BASE = "/api/cookbook/v1/recipes"
AUTH_BASE = "/api/cookbook/v1/auth"

VALID_RECIPE_PUBLIC = {
    "title": "Публичный рецепт",
    "description": "Виден всем пользователям",
    "cookingTime": 30,
    "difficulty": "easy",
    "servings": 2,
    "instructions": "1. Приготовить.",
    "ingredients": [],
    "categoryIds": [],
    "isPublic": True,
}

VALID_RECIPE_PRIVATE = {
    "title": "Приватный рецепт",
    "description": "Виден только автору",
    "cookingTime": 30,
    "difficulty": "easy",
    "servings": 2,
    "instructions": "1. Приготовить.",
    "ingredients": [],
    "categoryIds": [],
    "isPublic": False,
}


def _register_and_login(base_url: str) -> str:
    """Регистрирует нового пользователя и возвращает токен."""
    email = f"vis_{uuid.uuid4().hex[:12]}@example.com"
    password = "P@ssw0rd!"
    httpx.post(
        f"{base_url}{AUTH_BASE}/register",
        json={"email": email, "displayName": "Visibility User", "password": password},
    )
    resp = httpx.post(
        f"{base_url}{AUTH_BASE}/login",
        json={"email": email, "password": password},
    )
    return resp.json()["token"]


def _create_recipe(base_url: str, token: str, payload: dict) -> dict:
    resp = httpx.post(
        f"{base_url}{BASE}",
        json=payload,
        headers={"Authorization": f"Bearer {token}"},
    )
    assert resp.status_code == 201
    return resp.json()


# ── 1. Публичный рецепт виден анонимно ───────────────────────────────────────

def test_public_recipe_visible_anonymously(base_url: str, auth_token: str) -> None:
    unique = uuid.uuid4().hex[:8]
    recipe = _create_recipe(
        base_url, auth_token, {**VALID_RECIPE_PUBLIC, "title": f"Публичный {unique}"}
    )
    recipe_id = recipe["id"]

    resp = httpx.get(f"{base_url}{BASE}/{recipe_id}")

    assert resp.status_code == 200
    data = resp.json()
    assert data["id"] == recipe_id
    assert data["isPublic"] is True


# ── 2. Публичный рецепт присутствует в списке анонимно ───────────────────────

def test_public_recipe_in_list_anonymously(base_url: str, auth_token: str) -> None:
    unique = uuid.uuid4().hex[:8]
    recipe = _create_recipe(
        base_url, auth_token, {**VALID_RECIPE_PUBLIC, "title": f"Публичный список {unique}"}
    )
    recipe_id = recipe["id"]

    resp = httpx.get(f"{base_url}{BASE}", params={"q": unique, "pageSize": 100})

    assert resp.status_code == 200
    ids = [r["id"] for r in resp.json()["items"]]
    assert recipe_id in ids


# ── 3. Приватный рецепт не виден анонимно в списке ───────────────────────────

def test_private_recipe_hidden_from_anonymous_list(base_url: str, auth_token: str) -> None:
    unique = uuid.uuid4().hex[:8]
    recipe = _create_recipe(
        base_url, auth_token, {**VALID_RECIPE_PRIVATE, "title": f"Приватный список {unique}"}
    )
    recipe_id = recipe["id"]

    resp = httpx.get(f"{base_url}{BASE}", params={"q": unique, "pageSize": 100})

    assert resp.status_code == 200
    ids = [r["id"] for r in resp.json()["items"]]
    assert recipe_id not in ids


# ── 4. Приватный рецепт возвращает 403 для анонимного запроса ────────────────

def test_private_recipe_returns_403_for_anonymous(base_url: str, auth_token: str) -> None:
    recipe = _create_recipe(base_url, auth_token, VALID_RECIPE_PRIVATE)
    recipe_id = recipe["id"]

    resp = httpx.get(f"{base_url}{BASE}/{recipe_id}")

    assert resp.status_code == 403


# ── 5. Автор видит свой приватный рецепт в списке ────────────────────────────

def test_author_sees_own_private_recipe_in_list(base_url: str) -> None:
    token = _register_and_login(base_url)
    unique = uuid.uuid4().hex[:8]
    recipe = _create_recipe(
        base_url, token, {**VALID_RECIPE_PRIVATE, "title": f"Мой приватный {unique}"}
    )
    recipe_id = recipe["id"]

    resp = httpx.get(
        f"{base_url}{BASE}",
        params={"q": unique, "pageSize": 100},
        headers={"Authorization": f"Bearer {token}"},
    )

    assert resp.status_code == 200
    ids = [r["id"] for r in resp.json()["items"]]
    assert recipe_id in ids


# ── 6. Автор получает 200 на свой приватный рецепт ───────────────────────────

def test_author_can_access_own_private_recipe(base_url: str) -> None:
    token = _register_and_login(base_url)
    recipe = _create_recipe(base_url, token, VALID_RECIPE_PRIVATE)
    recipe_id = recipe["id"]

    resp = httpx.get(
        f"{base_url}{BASE}/{recipe_id}",
        headers={"Authorization": f"Bearer {token}"},
    )

    assert resp.status_code == 200
    data = resp.json()
    assert data["id"] == recipe_id
    assert data["isPublic"] is False


# ── 7. Другой пользователь получает 403 на чужой приватный рецепт ────────────

def test_other_user_gets_403_on_private_recipe(base_url: str) -> None:
    owner_token = _register_and_login(base_url)
    other_token = _register_and_login(base_url)

    recipe = _create_recipe(base_url, owner_token, VALID_RECIPE_PRIVATE)
    recipe_id = recipe["id"]

    resp = httpx.get(
        f"{base_url}{BASE}/{recipe_id}",
        headers={"Authorization": f"Bearer {other_token}"},
    )

    assert resp.status_code == 403


# ── 8. Другой пользователь не видит чужой приватный рецепт в списке ──────────

def test_other_user_cannot_see_private_recipe_in_list(base_url: str) -> None:
    owner_token = _register_and_login(base_url)
    other_token = _register_and_login(base_url)

    unique = uuid.uuid4().hex[:8]
    recipe = _create_recipe(
        base_url, owner_token, {**VALID_RECIPE_PRIVATE, "title": f"Чужой приватный {unique}"}
    )
    recipe_id = recipe["id"]

    resp = httpx.get(
        f"{base_url}{BASE}",
        params={"q": unique, "pageSize": 100},
        headers={"Authorization": f"Bearer {other_token}"},
    )

    assert resp.status_code == 200
    ids = [r["id"] for r in resp.json()["items"]]
    assert recipe_id not in ids


# ── 9. Ответ содержит поля isPublic и authorName ─────────────────────────────

def test_recipe_response_contains_is_public_and_author_name(base_url: str, auth_token: str) -> None:
    recipe = _create_recipe(base_url, auth_token, VALID_RECIPE_PUBLIC)
    recipe_id = recipe["id"]

    resp = httpx.get(f"{base_url}{BASE}/{recipe_id}")

    assert resp.status_code == 200
    data = resp.json()
    assert "isPublic" in data
    assert "authorName" in data


# ── 10. Список рецептов содержит поля isPublic и authorName ──────────────────

def test_recipe_list_items_contain_is_public_and_author_name(base_url: str) -> None:
    resp = httpx.get(f"{base_url}{BASE}")

    assert resp.status_code == 200
    items = resp.json()["items"]
    assert len(items) > 0
    for item in items:
        assert "isPublic" in item
        assert "authorName" in item
