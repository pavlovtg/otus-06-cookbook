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
        assert "servings" in recipe
        assert "instructions" in recipe


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
