import httpx
import uuid

BASE = "/api/cookbook/v1/recipes"

VALID_RECIPE = {
    "title": "Рецепт для теста рейтинга",
    "description": "Описание",
    "cookingTime": 30,
    "difficulty": "easy",
    "servings": 2,
    "instructions": "Шаги.",
    "ingredients": [],
    "categoryIds": [],
    "isPublic": True,
}


def _create_recipe(base_url: str, auth_token: str) -> dict:
    resp = httpx.post(
        f"{base_url}{BASE}",
        json={**VALID_RECIPE, "title": f"Рейтинг {uuid.uuid4().hex[:8]}"},
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert resp.status_code == 201
    return resp.json()


def test_set_rating_updates_average_rating_in_list(base_url: str, auth_token: str) -> None:
    """Логин → выставить оценку → проверить averageRating в списке."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    # Выставляем оценку 4
    put_resp = httpx.put(
        f"{base_url}{BASE}/{recipe_id}/rating",
        json={"value": 4},
        headers=headers,
    )
    assert put_resp.status_code == 200

    data = put_resp.json()
    assert data["averageRating"] is not None
    assert abs(data["averageRating"] - 4.0) < 0.01
    assert data["myRating"] == 4


def test_delete_rating_clears_average_rating(base_url: str, auth_token: str) -> None:
    """Выставить оценку → удалить → averageRating = null в ответе на GET списка."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    # Выставляем оценку
    put_resp = httpx.put(
        f"{base_url}{BASE}/{recipe_id}/rating",
        json={"value": 3},
        headers=headers,
    )
    assert put_resp.status_code == 200

    # Удаляем оценку
    del_resp = httpx.delete(
        f"{base_url}{BASE}/{recipe_id}/rating",
        headers=headers,
    )
    assert del_resp.status_code == 204

    # Проверяем averageRating = null в списке рецептов
    list_resp = httpx.get(
        f"{base_url}{BASE}",
        params={"pageSize": 1000},
        headers=headers,
    )
    assert list_resp.status_code == 200
    items = list_resp.json()["items"]
    found = next((r for r in items if r["id"] == recipe_id), None)
    assert found is not None, "Рецепт не найден в списке"
    assert found["averageRating"] is None


def test_rating_full_flow(base_url: str, auth_token: str) -> None:
    """Полный сценарий: логин → оценка → проверка в списке → удаление → averageRating = null."""
    recipe = _create_recipe(base_url, auth_token)
    recipe_id = recipe["id"]
    headers = {"Authorization": f"Bearer {auth_token}"}

    # 1. Выставляем оценку 5
    put_resp = httpx.put(
        f"{base_url}{BASE}/{recipe_id}/rating",
        json={"value": 5},
        headers=headers,
    )
    assert put_resp.status_code == 200
    summary = put_resp.json()
    assert summary["averageRating"] is not None
    assert summary["myRating"] == 5

    # 2. Проверяем averageRating в списке рецептов
    list_resp = httpx.get(
        f"{base_url}{BASE}",
        params={"pageSize": 1000},
        headers=headers,
    )
    assert list_resp.status_code == 200
    items = list_resp.json()["items"]
    found = next((r for r in items if r["id"] == recipe_id), None)
    assert found is not None, "Рецепт не найден в списке"
    assert found["averageRating"] is not None
    assert abs(found["averageRating"] - 5.0) < 0.01

    # 3. Удаляем оценку
    del_resp = httpx.delete(
        f"{base_url}{BASE}/{recipe_id}/rating",
        headers=headers,
    )
    assert del_resp.status_code == 204

    # 4. Проверяем averageRating = null в списке
    list_resp2 = httpx.get(
        f"{base_url}{BASE}",
        params={"pageSize": 1000},
        headers=headers,
    )
    assert list_resp2.status_code == 200
    items2 = list_resp2.json()["items"]
    found2 = next((r for r in items2 if r["id"] == recipe_id), None)
    assert found2 is not None, "Рецепт не найден в списке после удаления оценки"
    assert found2["averageRating"] is None
