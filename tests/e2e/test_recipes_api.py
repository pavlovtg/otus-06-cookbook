import httpx
import uuid


BASE = "/api/cookbook/v1/recipes"

VALID_RECIPE = {
    "title": "Тестовый борщ",
    "description": "Классический украинский борщ",
    "cookingTime": 120,
    "difficulty": "everyday",
    "servings": 6,
    "instructions": "1. Сварить бульон. 2. Добавить овощи.",
    "ingredients": [],
}

VALID_INGREDIENT = {
    "title": "Тестовая морковь",
    "unit": "г",
    "defaultAmount": 100.0,
    "category": "vegetables",
}


def test_recipes_list_is_not_empty(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")

    assert response.status_code == 200
    recipes = response.json()
    assert isinstance(recipes, list)
    assert len(recipes) > 0


def test_recipes_list_items_have_required_fields(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    recipes = response.json()

    for recipe in recipes:
        assert "id" in recipe
        assert "title" in recipe
        assert "description" in recipe
        assert "cookingTime" in recipe
        assert "difficulty" in recipe


def test_create_recipe_returns_201(base_url: str) -> None:
    response = httpx.post(f"{base_url}{BASE}", json=VALID_RECIPE)

    assert response.status_code == 201
    data = response.json()
    assert data["title"] == VALID_RECIPE["title"]
    assert data["cookingTime"] == VALID_RECIPE["cookingTime"]
    assert data["difficulty"] == VALID_RECIPE["difficulty"]
    assert data["servings"] == VALID_RECIPE["servings"]
    assert "id" in data


def test_create_recipe_returns_400_on_empty_title(base_url: str) -> None:
    payload = {**VALID_RECIPE, "title": ""}
    response = httpx.post(f"{base_url}{BASE}", json=payload)

    assert response.status_code == 400


def test_get_recipe_by_id_returns_200(base_url: str) -> None:
    create_resp = httpx.post(f"{base_url}{BASE}", json=VALID_RECIPE)
    recipe_id = create_resp.json()["id"]

    response = httpx.get(f"{base_url}{BASE}/{recipe_id}")

    assert response.status_code == 200
    data = response.json()
    assert data["id"] == recipe_id
    assert data["title"] == VALID_RECIPE["title"]


def test_get_recipe_by_unknown_id_returns_400(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}/{uuid.uuid4()}")

    assert response.status_code == 400


def test_update_recipe_returns_204(base_url: str) -> None:
    create_resp = httpx.post(f"{base_url}{BASE}", json=VALID_RECIPE)
    recipe_id = create_resp.json()["id"]

    updated = {**VALID_RECIPE, "title": "Обновлённый борщ", "cookingTime": 90}
    response = httpx.put(f"{base_url}{BASE}/{recipe_id}", json=updated)

    assert response.status_code == 204

    get_resp = httpx.get(f"{base_url}{BASE}/{recipe_id}")
    data = get_resp.json()
    assert data["title"] == "Обновлённый борщ"
    assert data["cookingTime"] == 90


def test_delete_recipe_returns_204(base_url: str) -> None:
    create_resp = httpx.post(f"{base_url}{BASE}", json=VALID_RECIPE)
    recipe_id = create_resp.json()["id"]

    response = httpx.delete(f"{base_url}{BASE}/{recipe_id}")

    assert response.status_code == 204


def test_delete_unknown_recipe_returns_400(base_url: str) -> None:
    response = httpx.delete(f"{base_url}{BASE}/{uuid.uuid4()}")

    assert response.status_code == 400


# ── Ingredients in recipes (8.11, 8.12) ──────────────────────────────────────

INGREDIENTS_BASE = "/api/cookbook/v1/ingredients"


def _create_ingredient(base_url: str) -> dict:
    response = httpx.post(f"{base_url}{INGREDIENTS_BASE}", json=VALID_INGREDIENT)
    assert response.status_code == 201
    return response.json()


def test_create_recipe_with_ingredients_returns_ingredients_in_get(base_url: str) -> None:
    ingredient = _create_ingredient(base_url)

    recipe_payload = {
        **VALID_RECIPE,
        "ingredients": [{"ingredientId": ingredient["id"], "amount": 150.0}],
    }
    create_resp = httpx.post(f"{base_url}{BASE}", json=recipe_payload)
    assert create_resp.status_code == 201
    recipe_id = create_resp.json()["id"]

    get_resp = httpx.get(f"{base_url}{BASE}/{recipe_id}")
    assert get_resp.status_code == 200

    data = get_resp.json()
    assert "ingredients" in data
    assert len(data["ingredients"]) == 1
    assert data["ingredients"][0]["ingredientId"] == ingredient["id"]
    assert data["ingredients"][0]["amount"] == 150.0


def test_update_recipe_removes_ingredient(base_url: str) -> None:
    ingredient = _create_ingredient(base_url)

    recipe_payload = {
        **VALID_RECIPE,
        "ingredients": [{"ingredientId": ingredient["id"], "amount": 100.0}],
    }
    create_resp = httpx.post(f"{base_url}{BASE}", json=recipe_payload)
    assert create_resp.status_code == 201
    recipe_id = create_resp.json()["id"]

    update_payload = {**VALID_RECIPE, "ingredients": []}
    update_resp = httpx.put(f"{base_url}{BASE}/{recipe_id}", json=update_payload)
    assert update_resp.status_code == 204

    get_resp = httpx.get(f"{base_url}{BASE}/{recipe_id}")
    data = get_resp.json()
    assert data["ingredients"] == []
