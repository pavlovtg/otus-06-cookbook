import httpx
import uuid

BASE = "/api/cookbook/v1/recipes"
AUTH_BASE = "/api/cookbook/v1/auth"

VALID_RECIPE = {
    "title": "Тестовый рецепт для избранного",
    "description": "Рецепт для E2E-тестов избранного",
    "cookingTime": 30,
    "difficulty": "everyday",
    "servings": 2,
    "instructions": "1. Приготовить.",
    "ingredients": [],
    "categoryIds": [],
    "isPublic": True,
}


def _create_recipe(base_url: str, auth_token: str) -> dict:
    resp = httpx.post(
        f"{base_url}{BASE}",
        json={**VALID_RECIPE, "title": f"Избранное {uuid.uuid4().hex[:8]}"},
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert resp.status_code == 201
    return resp.json()


# ── POST /recipes/{id}/favorites ─────────────────────────────────────────────

def test_add_favorite_returns_201(base_url: str, auth_token: str) -> None:
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]

    resp = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/favorites",
        headers={"Authorization": f"Bearer {auth_token}"},
    )

    assert resp.status_code == 201


def test_add_favorite_without_auth_returns_401(base_url: str, auth_token: str) -> None:
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]

    resp = httpx.post(f"{base_url}{BASE}/{recipe_id}/favorites")

    assert resp.status_code == 401


def test_add_favorite_idempotent(base_url: str, auth_token: str) -> None:
    """Повторный POST не возвращает ошибку."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    resp1 = httpx.post(f"{base_url}{BASE}/{recipe_id}/favorites", headers=headers)
    resp2 = httpx.post(f"{base_url}{BASE}/{recipe_id}/favorites", headers=headers)

    assert resp1.status_code == 201
    assert resp2.status_code in (200, 201, 204)


# ── DELETE /recipes/{id}/favorites ───────────────────────────────────────────

def test_remove_favorite_returns_204(base_url: str, auth_token: str) -> None:
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    httpx.post(f"{base_url}{BASE}/{recipe_id}/favorites", headers=headers)

    resp = httpx.delete(
        f"{base_url}{BASE}/{recipe_id}/favorites",
        headers=headers,
    )

    assert resp.status_code == 204


def test_remove_favorite_without_auth_returns_401(base_url: str, auth_token: str) -> None:
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]

    resp = httpx.delete(f"{base_url}{BASE}/{recipe_id}/favorites")

    assert resp.status_code == 401


def test_remove_favorite_idempotent(base_url: str, auth_token: str) -> None:
    """Повторный DELETE не возвращает ошибку."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    httpx.post(f"{base_url}{BASE}/{recipe_id}/favorites", headers=headers)
    httpx.delete(f"{base_url}{BASE}/{recipe_id}/favorites", headers=headers)

    resp2 = httpx.delete(f"{base_url}{BASE}/{recipe_id}/favorites", headers=headers)

    assert resp2.status_code in (200, 204)


# ── GET /recipes?favorites=true ───────────────────────────────────────────────

def test_favorites_list_requires_auth(base_url: str) -> None:
    resp = httpx.get(f"{base_url}{BASE}", params={"favorites": "true"})

    assert resp.status_code == 401


def test_favorites_list_contains_added_recipe(base_url: str, auth_token: str) -> None:
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    httpx.post(f"{base_url}{BASE}/{recipe_id}/favorites", headers=headers)

    resp = httpx.get(
        f"{base_url}{BASE}",
        params={"favorites": "true", "pageSize": 1000},
        headers=headers,
    )

    assert resp.status_code == 200
    ids = [r["id"] for r in resp.json()["items"]]
    assert recipe_id in ids


def test_favorites_list_excludes_removed_recipe(base_url: str, auth_token: str) -> None:
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    httpx.post(f"{base_url}{BASE}/{recipe_id}/favorites", headers=headers)
    httpx.delete(f"{base_url}{BASE}/{recipe_id}/favorites", headers=headers)

    resp = httpx.get(
        f"{base_url}{BASE}",
        params={"favorites": "true", "pageSize": 1000},
        headers=headers,
    )

    assert resp.status_code == 200
    ids = [r["id"] for r in resp.json()["items"]]
    assert recipe_id not in ids


def test_favorites_list_items_have_is_favorite_true(base_url: str, auth_token: str) -> None:
    """Все рецепты в списке ?favorites=true имеют isFavorite=true."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    httpx.post(f"{base_url}{BASE}/{recipe_id}/favorites", headers=headers)

    resp = httpx.get(
        f"{base_url}{BASE}",
        params={"favorites": "true", "pageSize": 1000},
        headers=headers,
    )

    assert resp.status_code == 200
    items = resp.json()["items"]
    assert len(items) > 0
    for item in items:
        assert item.get("isFavorite") is True
