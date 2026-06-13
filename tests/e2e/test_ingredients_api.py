import httpx
import uuid


BASE = "/api/cookbook/v1/ingredients"

VALID_INGREDIENT = {
    "title": "Тестовая морковь",
    "unit": "г",
    "defaultAmount": 100.0,
    "category": "vegetables",
}


def test_ingredients_list_returns_200(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")

    assert response.status_code == 200
    ingredients = response.json()
    assert isinstance(ingredients, list)


def test_ingredients_list_items_have_required_fields(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    ingredients = response.json()

    for ingredient in ingredients:
        assert "id" in ingredient
        assert "title" in ingredient
        assert "unit" in ingredient
        assert "defaultAmount" in ingredient
        assert "category" in ingredient
        assert "isSystem" in ingredient


def test_create_ingredient_returns_201(base_url: str) -> None:
    response = httpx.post(f"{base_url}{BASE}", json=VALID_INGREDIENT)

    assert response.status_code == 201
    data = response.json()
    assert data["title"] == VALID_INGREDIENT["title"]
    assert data["unit"] == VALID_INGREDIENT["unit"]
    assert data["defaultAmount"] == VALID_INGREDIENT["defaultAmount"]
    assert data["category"] == VALID_INGREDIENT["category"]
    assert data["isSystem"] is False
    assert "id" in data


def test_create_ingredient_returns_400_on_short_title(base_url: str) -> None:
    payload = {**VALID_INGREDIENT, "title": "А"}
    response = httpx.post(f"{base_url}{BASE}", json=payload)

    assert response.status_code == 400


def test_create_ingredient_returns_400_on_empty_title(base_url: str) -> None:
    payload = {**VALID_INGREDIENT, "title": ""}
    response = httpx.post(f"{base_url}{BASE}", json=payload)

    assert response.status_code == 400


def test_create_ingredient_returns_400_on_zero_amount(base_url: str) -> None:
    payload = {**VALID_INGREDIENT, "defaultAmount": 0}
    response = httpx.post(f"{base_url}{BASE}", json=payload)

    assert response.status_code == 400


def test_create_ingredient_returns_400_on_invalid_category(base_url: str) -> None:
    payload = {**VALID_INGREDIENT, "category": "unknown_category"}
    response = httpx.post(f"{base_url}{BASE}", json=payload)

    assert response.status_code == 400


def test_get_ingredient_by_id_returns_200(base_url: str) -> None:
    create_resp = httpx.post(f"{base_url}{BASE}", json=VALID_INGREDIENT)
    ingredient_id = create_resp.json()["id"]

    response = httpx.get(f"{base_url}{BASE}/{ingredient_id}")

    assert response.status_code == 200
    data = response.json()
    assert data["id"] == ingredient_id
    assert data["title"] == VALID_INGREDIENT["title"]


def test_get_ingredient_by_unknown_id_returns_400(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}/{uuid.uuid4()}")

    assert response.status_code == 400


def test_update_ingredient_returns_204(base_url: str) -> None:
    create_resp = httpx.post(f"{base_url}{BASE}", json=VALID_INGREDIENT)
    ingredient_id = create_resp.json()["id"]

    updated = {**VALID_INGREDIENT, "title": "Обновлённая морковь", "unit": "шт."}
    response = httpx.put(f"{base_url}{BASE}/{ingredient_id}", json=updated)

    assert response.status_code == 204

    get_resp = httpx.get(f"{base_url}{BASE}/{ingredient_id}")
    data = get_resp.json()
    assert data["title"] == "Обновлённая морковь"
    assert data["unit"] == "шт."


def test_update_ingredient_returns_400_on_short_title(base_url: str) -> None:
    create_resp = httpx.post(f"{base_url}{BASE}", json=VALID_INGREDIENT)
    ingredient_id = create_resp.json()["id"]

    payload = {**VALID_INGREDIENT, "title": "А"}
    response = httpx.put(f"{base_url}{BASE}/{ingredient_id}", json=payload)

    assert response.status_code == 400


def test_delete_ingredient_returns_204(base_url: str) -> None:
    create_resp = httpx.post(f"{base_url}{BASE}", json=VALID_INGREDIENT)
    ingredient_id = create_resp.json()["id"]

    response = httpx.delete(f"{base_url}{BASE}/{ingredient_id}")

    assert response.status_code == 204

    get_resp = httpx.get(f"{base_url}{BASE}/{ingredient_id}")
    assert get_resp.status_code == 400


def test_delete_unknown_ingredient_returns_400(base_url: str) -> None:
    response = httpx.delete(f"{base_url}{BASE}/{uuid.uuid4()}")

    assert response.status_code == 400


def test_get_ingredients_filter_by_title(base_url: str) -> None:
    httpx.post(f"{base_url}{BASE}", json=VALID_INGREDIENT)

    response = httpx.get(f"{base_url}{BASE}", params={"title": "Тестовая"})

    assert response.status_code == 200
    data = response.json()
    assert all("Тестовая" in item["title"] for item in data)


def test_get_ingredients_filter_by_category(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}", params={"category": "vegetables"})

    assert response.status_code == 200
    data = response.json()
    assert all(item["category"] == "vegetables" for item in data)


def test_get_ingredients_filter_by_invalid_category_returns_400(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}", params={"category": "invalid_category"})

    assert response.status_code == 400
