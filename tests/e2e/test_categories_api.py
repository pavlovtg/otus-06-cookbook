import httpx
import uuid

BASE = "/api/cookbook/v1/categories"

VALID_CATEGORY = {
    "name": "Тестовая категория",
    "description": "Описание тестовой категории",
    "type": "cuisine",
}


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


def test_create_category_returns_201(base_url: str) -> None:
    payload = {**VALID_CATEGORY, "name": f"Тест-{uuid.uuid4().hex[:8]}"}
    response = httpx.post(f"{base_url}{BASE}", json=payload)

    assert response.status_code == 201
    data = response.json()
    assert data["name"] == payload["name"]
    assert data["type"] == payload["type"]
    assert "id" in data


def test_create_category_appears_in_list(base_url: str) -> None:
    payload = {**VALID_CATEGORY, "name": f"Тест-{uuid.uuid4().hex[:8]}"}
    create_resp = httpx.post(f"{base_url}{BASE}", json=payload)
    assert create_resp.status_code == 201
    category_id = create_resp.json()["id"]

    list_resp = httpx.get(f"{base_url}{BASE}")
    ids = [c["id"] for c in list_resp.json()]
    assert category_id in ids


def test_update_category_returns_204(base_url: str) -> None:
    payload = {**VALID_CATEGORY, "name": f"Тест-{uuid.uuid4().hex[:8]}"}
    create_resp = httpx.post(f"{base_url}{BASE}", json=payload)
    assert create_resp.status_code == 201
    category_id = create_resp.json()["id"]

    updated_name = f"Обновлённая-{uuid.uuid4().hex[:8]}"
    update_resp = httpx.put(
        f"{base_url}{BASE}/{category_id}",
        json={**payload, "name": updated_name},
    )
    assert update_resp.status_code == 204

    list_resp = httpx.get(f"{base_url}{BASE}")
    names = [c["name"] for c in list_resp.json()]
    assert updated_name in names


def test_delete_category_returns_204(base_url: str) -> None:
    payload = {**VALID_CATEGORY, "name": f"Тест-{uuid.uuid4().hex[:8]}"}
    create_resp = httpx.post(f"{base_url}{BASE}", json=payload)
    assert create_resp.status_code == 201
    category_id = create_resp.json()["id"]

    delete_resp = httpx.delete(f"{base_url}{BASE}/{category_id}")
    assert delete_resp.status_code == 204

    list_resp = httpx.get(f"{base_url}{BASE}")
    ids = [c["id"] for c in list_resp.json()]
    assert category_id not in ids
