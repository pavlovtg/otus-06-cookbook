import httpx
import uuid

BASE = "/api/cookbook/v1/categories"

VALID_CATEGORY = {
    "name": "Тестовая категория",
    "description": "Описание тестовой категории",
    "type": "cuisine",
}


def _auth_headers(token: str) -> dict:
    return {"Authorization": f"Bearer {token}"}


def _create_category(base_url: str, admin_token: str, payload: dict | None = None) -> dict:
    body = payload or {**VALID_CATEGORY, "name": f"Тест-{uuid.uuid4().hex[:8]}"}
    response = httpx.post(f"{base_url}{BASE}", json=body, headers=_auth_headers(admin_token))
    assert response.status_code == 201
    return response.json()


# ── GET /categories — публичный ──────────────────────────────────────────────

def test_categories_list_returns_200(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")

    assert response.status_code == 200
    data = response.json()
    assert isinstance(data, list)


def test_categories_list_items_have_required_fields(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")

    assert response.status_code == 200
    data = response.json()
    for category in data:
        assert "id" in category
        assert "name" in category
        assert "type" in category


# ── POST /categories — только admin ─────────────────────────────────────────

def test_create_category_returns_401_when_not_authenticated(base_url: str) -> None:
    payload = {**VALID_CATEGORY, "name": f"Тест-{uuid.uuid4().hex[:8]}"}
    response = httpx.post(f"{base_url}{BASE}", json=payload)

    assert response.status_code == 401


def test_create_category_returns_403_when_not_admin(base_url: str, auth_token: str) -> None:
    payload = {**VALID_CATEGORY, "name": f"Тест-{uuid.uuid4().hex[:8]}"}
    response = httpx.post(f"{base_url}{BASE}", json=payload, headers=_auth_headers(auth_token))

    assert response.status_code == 403


def test_create_category_returns_201(base_url: str, admin_token: str) -> None:
    payload = {**VALID_CATEGORY, "name": f"Тест-{uuid.uuid4().hex[:8]}"}
    response = httpx.post(f"{base_url}{BASE}", json=payload, headers=_auth_headers(admin_token))

    assert response.status_code == 201
    data = response.json()
    assert data["name"] == payload["name"]
    assert data["type"] == payload["type"]
    assert "id" in data


def test_create_category_appears_in_list(base_url: str, admin_token: str) -> None:
    payload = {**VALID_CATEGORY, "name": f"Тест-{uuid.uuid4().hex[:8]}"}
    create_resp = httpx.post(f"{base_url}{BASE}", json=payload, headers=_auth_headers(admin_token))
    assert create_resp.status_code == 201
    category_id = create_resp.json()["id"]

    list_resp = httpx.get(f"{base_url}{BASE}")
    ids = [c["id"] for c in list_resp.json()]
    assert category_id in ids


# ── PUT /categories/{id} — только admin ─────────────────────────────────────

def test_update_category_returns_401_when_not_authenticated(base_url: str, admin_token: str) -> None:
    category = _create_category(base_url, admin_token)

    response = httpx.put(
        f"{base_url}{BASE}/{category['id']}",
        json={**VALID_CATEGORY, "name": f"Обновлённая-{uuid.uuid4().hex[:8]}"},
    )
    assert response.status_code == 401


def test_update_category_returns_403_when_not_admin(base_url: str, admin_token: str, auth_token: str) -> None:
    category = _create_category(base_url, admin_token)

    response = httpx.put(
        f"{base_url}{BASE}/{category['id']}",
        json={**VALID_CATEGORY, "name": f"Обновлённая-{uuid.uuid4().hex[:8]}"},
        headers=_auth_headers(auth_token),
    )
    assert response.status_code == 403


def test_update_category_returns_204(base_url: str, admin_token: str) -> None:
    category = _create_category(base_url, admin_token)

    updated_name = f"Обновлённая-{uuid.uuid4().hex[:8]}"
    update_resp = httpx.put(
        f"{base_url}{BASE}/{category['id']}",
        json={**VALID_CATEGORY, "name": updated_name},
        headers=_auth_headers(admin_token),
    )
    assert update_resp.status_code == 204

    list_resp = httpx.get(f"{base_url}{BASE}")
    names = [c["name"] for c in list_resp.json()]
    assert updated_name in names


# ── DELETE /categories/{id} — только admin ──────────────────────────────────

def test_delete_category_returns_401_when_not_authenticated(base_url: str, admin_token: str) -> None:
    category = _create_category(base_url, admin_token)

    response = httpx.delete(f"{base_url}{BASE}/{category['id']}")
    assert response.status_code == 401


def test_delete_category_returns_403_when_not_admin(base_url: str, admin_token: str, auth_token: str) -> None:
    category = _create_category(base_url, admin_token)

    response = httpx.delete(
        f"{base_url}{BASE}/{category['id']}",
        headers=_auth_headers(auth_token),
    )
    assert response.status_code == 403


def test_delete_category_returns_204(base_url: str, admin_token: str) -> None:
    category = _create_category(base_url, admin_token)

    delete_resp = httpx.delete(
        f"{base_url}{BASE}/{category['id']}",
        headers=_auth_headers(admin_token),
    )
    assert delete_resp.status_code == 204

    list_resp = httpx.get(f"{base_url}{BASE}")
    ids = [c["id"] for c in list_resp.json()]
    assert category["id"] not in ids
