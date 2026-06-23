import httpx
import uuid


BASE = "/api/cookbook/v1/ingredients"

VALID_INGREDIENT = {
    "title": "Тестовая морковь",
    "unit": "г",
    "defaultAmount": 100.0,
    "category": "vegetables",
}


def _get_paged(base_url: str, **params) -> dict:
    response = httpx.get(f"{base_url}{BASE}", params=params)
    assert response.status_code == 200
    data = response.json()
    assert "items" in data
    assert "total" in data
    assert "page" in data
    assert "pageSize" in data
    return data


def _auth_headers(auth_token: str) -> dict:
    return {"Authorization": f"Bearer {auth_token}"}


def _create_ingredient(base_url: str, auth_token: str, payload: dict | None = None) -> dict:
    body = payload or VALID_INGREDIENT
    response = httpx.post(f"{base_url}{BASE}", json=body, headers=_auth_headers(auth_token))
    assert response.status_code == 201
    return response.json()


def test_ingredients_list_returns_200(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")

    assert response.status_code == 200
    data = response.json()
    assert isinstance(data, dict)
    assert "items" in data
    assert "total" in data
    assert "page" in data
    assert "pageSize" in data


def test_ingredients_list_default_page_and_page_size(base_url: str) -> None:
    data = _get_paged(base_url)

    assert data["page"] == 1
    assert data["pageSize"] == 100


def test_ingredients_list_items_have_required_fields(base_url: str) -> None:
    data = _get_paged(base_url)

    for ingredient in data["items"]:
        assert "id" in ingredient
        assert "title" in ingredient
        assert "unit" in ingredient
        assert "defaultAmount" in ingredient
        assert "category" in ingredient
        assert "isSystem" in ingredient


def test_ingredients_list_with_explicit_page_and_page_size(base_url: str) -> None:
    data = _get_paged(base_url, page=2, pageSize=50)

    assert data["page"] == 2
    assert data["pageSize"] == 50


def test_ingredients_list_page_size_clamped_to_1000(base_url: str) -> None:
    data = _get_paged(base_url, pageSize=5000)

    assert data["pageSize"] == 1000


def test_ingredients_list_returns_400_when_page_is_zero(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}", params={"page": 0})
    assert response.status_code == 400


def test_ingredients_list_returns_400_when_page_size_is_zero(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}", params={"pageSize": 0})
    assert response.status_code == 400


def test_ingredients_list_returns_400_when_title_exceeds_200_chars(base_url: str) -> None:
    long_title = "А" * 201
    response = httpx.get(f"{base_url}{BASE}", params={"title": long_title})
    assert response.status_code == 400


def test_create_ingredient_returns_201(base_url: str, auth_token: str) -> None:
    response = httpx.post(f"{base_url}{BASE}", json=VALID_INGREDIENT, headers=_auth_headers(auth_token))

    assert response.status_code == 201
    data = response.json()
    assert data["title"] == VALID_INGREDIENT["title"]
    assert data["unit"] == VALID_INGREDIENT["unit"]
    assert data["defaultAmount"] == VALID_INGREDIENT["defaultAmount"]
    assert data["category"] == VALID_INGREDIENT["category"]
    assert data["isSystem"] is False
    assert "id" in data


def test_create_ingredient_returns_400_on_short_title(base_url: str, auth_token: str) -> None:
    payload = {**VALID_INGREDIENT, "title": "А"}
    response = httpx.post(f"{base_url}{BASE}", json=payload, headers=_auth_headers(auth_token))

    assert response.status_code == 400


def test_create_ingredient_returns_400_on_empty_title(base_url: str, auth_token: str) -> None:
    payload = {**VALID_INGREDIENT, "title": ""}
    response = httpx.post(f"{base_url}{BASE}", json=payload, headers=_auth_headers(auth_token))

    assert response.status_code == 400


def test_create_ingredient_returns_400_on_zero_amount(base_url: str, auth_token: str) -> None:
    payload = {**VALID_INGREDIENT, "defaultAmount": 0}
    response = httpx.post(f"{base_url}{BASE}", json=payload, headers=_auth_headers(auth_token))

    assert response.status_code == 400


def test_create_ingredient_returns_400_on_invalid_category(base_url: str, auth_token: str) -> None:
    payload = {**VALID_INGREDIENT, "category": "unknown_category"}
    response = httpx.post(f"{base_url}{BASE}", json=payload, headers=_auth_headers(auth_token))

    assert response.status_code == 400


def test_get_ingredient_by_id_returns_200(base_url: str, auth_token: str) -> None:
    ingredient = _create_ingredient(base_url, auth_token)
    ingredient_id = ingredient["id"]

    response = httpx.get(f"{base_url}{BASE}/{ingredient_id}")

    assert response.status_code == 200
    data = response.json()
    assert data["id"] == ingredient_id
    assert data["title"] == VALID_INGREDIENT["title"]


def test_get_ingredient_by_unknown_id_returns_400(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}/{uuid.uuid4()}")

    assert response.status_code == 400


def test_update_ingredient_returns_204(base_url: str, auth_token: str) -> None:
    ingredient = _create_ingredient(base_url, auth_token)
    ingredient_id = ingredient["id"]

    updated = {**VALID_INGREDIENT, "title": "Обновлённая морковь", "unit": "шт."}
    response = httpx.put(
        f"{base_url}{BASE}/{ingredient_id}",
        json=updated,
        headers=_auth_headers(auth_token),
    )

    assert response.status_code == 204

    get_resp = httpx.get(f"{base_url}{BASE}/{ingredient_id}")
    data = get_resp.json()
    assert data["title"] == "Обновлённая морковь"
    assert data["unit"] == "шт."


def test_update_ingredient_returns_400_on_short_title(base_url: str, auth_token: str) -> None:
    ingredient = _create_ingredient(base_url, auth_token)
    ingredient_id = ingredient["id"]

    payload = {**VALID_INGREDIENT, "title": "А"}
    response = httpx.put(
        f"{base_url}{BASE}/{ingredient_id}",
        json=payload,
        headers=_auth_headers(auth_token),
    )

    assert response.status_code == 400


def test_delete_ingredient_returns_204(base_url: str, auth_token: str) -> None:
    ingredient = _create_ingredient(base_url, auth_token)
    ingredient_id = ingredient["id"]

    response = httpx.delete(
        f"{base_url}{BASE}/{ingredient_id}",
        headers=_auth_headers(auth_token),
    )

    assert response.status_code == 204

    get_resp = httpx.get(f"{base_url}{BASE}/{ingredient_id}")
    assert get_resp.status_code == 400


def test_delete_unknown_ingredient_returns_400(base_url: str, auth_token: str) -> None:
    response = httpx.delete(
        f"{base_url}{BASE}/{uuid.uuid4()}",
        headers=_auth_headers(auth_token),
    )

    assert response.status_code == 400


def test_get_ingredients_filter_by_title(base_url: str, auth_token: str) -> None:
    _create_ingredient(base_url, auth_token)

    data = _get_paged(base_url, title="Тестовая")
    assert all("Тестовая" in item["title"] for item in data["items"])


def test_get_ingredients_filter_by_category(base_url: str) -> None:
    data = _get_paged(base_url, category="vegetables")
    assert all(item["category"] == "vegetables" for item in data["items"])


def test_get_ingredients_filter_by_invalid_category_returns_400(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}", params={"category": "invalid_category"})

    assert response.status_code == 400
