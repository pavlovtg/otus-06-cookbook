import httpx
import uuid

BASE = "/api/cookbook/v1/recipes"
AUTH_BASE = "/api/cookbook/v1/auth"

VALID_RECIPE = {
    "title": "Рецепт для теста комментариев",
    "description": "Описание",
    "cookingTime": 30,
    "difficulty": "everyday",
    "servings": 2,
    "instructions": "Шаги.",
    "ingredients": [],
    "categoryIds": [],
    "isPublic": True,
}


def _create_recipe(base_url: str, auth_token: str) -> dict:
    resp = httpx.post(
        f"{base_url}{BASE}",
        json={**VALID_RECIPE, "title": f"Комментарии {uuid.uuid4().hex[:8]}"},
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert resp.status_code == 201
    return resp.json()


def _register_and_login(base_url: str) -> str:
    """Регистрирует нового пользователя и возвращает JWT."""
    email = f"e2e_comment_{uuid.uuid4().hex[:12]}@example.com"
    password = "P@ssw0rd!"
    httpx.post(
        f"{base_url}{AUTH_BASE}/register",
        json={"email": email, "displayName": "Comment User", "password": password},
    )
    resp = httpx.post(
        f"{base_url}{AUTH_BASE}/login",
        json={"email": email, "password": password},
    )
    assert resp.status_code == 200
    return resp.json()["token"]


# ── POST /recipes/{id}/comments ───────────────────────────────────────────────

def test_add_comment_returns_201(base_url: str, auth_token: str) -> None:
    """Добавление комментария возвращает 201 и поля ответа."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]

    resp = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": "Отличный рецепт!"},
        headers={"Authorization": f"Bearer {auth_token}"},
    )

    assert resp.status_code == 201
    data = resp.json()
    assert data["id"]
    assert data["text"] == "Отличный рецепт!"
    assert data["authorId"]
    assert data["recipeId"] == recipe_id
    assert data["createdAt"]


def test_add_comment_without_auth_returns_401(base_url: str, auth_token: str) -> None:
    """Добавление комментария без JWT возвращает 401."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]

    resp = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": "Без авторизации"},
    )

    assert resp.status_code == 401


def test_add_comment_duplicate_returns_400(base_url: str, auth_token: str) -> None:
    """Повторное добавление комментария тем же пользователем возвращает 400."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    resp1 = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": "Первый комментарий"},
        headers=headers,
    )
    assert resp1.status_code == 201

    resp2 = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": "Второй комментарий того же пользователя"},
        headers=headers,
    )
    assert resp2.status_code == 400


def test_add_comment_too_long_returns_400(base_url: str, auth_token: str) -> None:
    """Комментарий длиннее 2000 символов возвращает 400."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]

    resp = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": "x" * 2001},
        headers={"Authorization": f"Bearer {auth_token}"},
    )

    assert resp.status_code == 400


# ── GET /recipes/{id}/comments ────────────────────────────────────────────────

def test_get_comments_returns_list(base_url: str, auth_token: str) -> None:
    """GET комментариев возвращает список с добавленным комментарием."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}
    comment_text = f"Тест получения {uuid.uuid4().hex[:8]}"

    httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": comment_text},
        headers=headers,
    )

    resp = httpx.get(
        f"{base_url}{BASE}/{recipe_id}/comments",
        params={"page": 1, "pageSize": 50},
    )

    assert resp.status_code == 200
    data = resp.json()
    assert "items" in data
    assert "total" in data
    texts = [c["text"] for c in data["items"]]
    assert comment_text in texts


def test_get_comments_empty_list(base_url: str, auth_token: str) -> None:
    """GET комментариев для рецепта без комментариев возвращает пустой список."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]

    resp = httpx.get(f"{base_url}{BASE}/{recipe_id}/comments")

    assert resp.status_code == 200
    data = resp.json()
    assert data["items"] == []
    assert data["total"] == 0


def test_get_comments_public_access(base_url: str, auth_token: str) -> None:
    """GET комментариев доступен без авторизации."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]

    resp = httpx.get(f"{base_url}{BASE}/{recipe_id}/comments")

    assert resp.status_code == 200


# ── DELETE /recipes/{id}/comments/{commentId} ─────────────────────────────────

def test_delete_comment_by_comment_author_returns_204(base_url: str, auth_token: str) -> None:
    """Автор комментария может удалить свой комментарий."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    add_resp = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": "Удаляемый комментарий"},
        headers=headers,
    )
    assert add_resp.status_code == 201
    comment_id = add_resp.json()["id"]

    del_resp = httpx.delete(
        f"{base_url}{BASE}/{recipe_id}/comments/{comment_id}",
        headers=headers,
    )

    assert del_resp.status_code == 204


def test_delete_comment_by_recipe_author_returns_204(base_url: str, auth_token: str) -> None:
    """Автор рецепта может удалить чужой комментарий."""
    # Создаём рецепт от основного пользователя
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]

    # Второй пользователь добавляет комментарий
    other_token = _register_and_login(base_url)
    add_resp = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": "Комментарий второго пользователя"},
        headers={"Authorization": f"Bearer {other_token}"},
    )
    assert add_resp.status_code == 201
    comment_id = add_resp.json()["id"]

    # Автор рецепта удаляет комментарий
    del_resp = httpx.delete(
        f"{base_url}{BASE}/{recipe_id}/comments/{comment_id}",
        headers={"Authorization": f"Bearer {auth_token}"},
    )

    assert del_resp.status_code == 204


def test_delete_comment_by_other_user_returns_403(base_url: str, auth_token: str) -> None:
    """Чужой пользователь не может удалить комментарий — 403."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]

    # Первый пользователь добавляет комментарий
    add_resp = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": "Комментарий первого пользователя"},
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert add_resp.status_code == 201
    comment_id = add_resp.json()["id"]

    # Второй пользователь пытается удалить
    other_token = _register_and_login(base_url)
    del_resp = httpx.delete(
        f"{base_url}{BASE}/{recipe_id}/comments/{comment_id}",
        headers={"Authorization": f"Bearer {other_token}"},
    )

    assert del_resp.status_code == 403


def test_delete_comment_without_auth_returns_401(base_url: str, auth_token: str) -> None:
    """Удаление комментария без JWT возвращает 401."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    add_resp = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": "Комментарий для теста 401"},
        headers=headers,
    )
    assert add_resp.status_code == 201
    comment_id = add_resp.json()["id"]

    del_resp = httpx.delete(
        f"{base_url}{BASE}/{recipe_id}/comments/{comment_id}",
    )

    assert del_resp.status_code == 401


def test_delete_comment_removes_from_list(base_url: str, auth_token: str) -> None:
    """После удаления комментарий исчезает из списка."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}
    comment_text = f"Удаляемый {uuid.uuid4().hex[:8]}"

    add_resp = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/comments",
        json={"text": comment_text},
        headers=headers,
    )
    assert add_resp.status_code == 201
    comment_id = add_resp.json()["id"]

    httpx.delete(
        f"{base_url}{BASE}/{recipe_id}/comments/{comment_id}",
        headers=headers,
    )

    list_resp = httpx.get(f"{base_url}{BASE}/{recipe_id}/comments")
    assert list_resp.status_code == 200
    texts = [c["text"] for c in list_resp.json()["items"]]
    assert comment_text not in texts
