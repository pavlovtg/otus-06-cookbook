import httpx

BASE = "/api/cookbook/v1/meal-plan"

VALID_RECIPE = {
    "title": "Тестовый рецепт для плана",
    "description": "Рецепт для E2E-тестов планировщика",
    "cookingTime": 30,
    "difficulty": "everyday",
    "servings": 2,
    "instructions": "1. Приготовить.",
    "ingredients": [],
    "categoryIds": [],
    "isPublic": True,
}

RECIPES_BASE = "/api/cookbook/v1/recipes"


def _create_recipe(base_url: str, auth_token: str) -> str:
    resp = httpx.post(
        f"{base_url}{RECIPES_BASE}",
        json=VALID_RECIPE,
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert resp.status_code == 201, resp.text
    return resp.json()["id"]


def _make_plan(recipe_id: str) -> dict:
    return {
        "slots": [
            {
                "weekDay": 1,
                "mealType": 1,
                "items": [{"recipeId": recipe_id, "servings": 2}],
            }
        ]
    }


# ── GET /api/v1/meal-plan ─────────────────────────────────────────────────────

def test_get_meal_plan_without_auth_returns_401(base_url: str) -> None:
    resp = httpx.get(f"{base_url}{BASE}")

    assert resp.status_code == 401


def test_get_meal_plan_returns_200_with_valid_structure(base_url: str, auth_token: str) -> None:
    resp = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {auth_token}"},
    )

    assert resp.status_code == 200
    data = resp.json()
    assert "slots" in data
    assert isinstance(data["slots"], list)


# ── PUT /api/v1/meal-plan ─────────────────────────────────────────────────────

def test_put_meal_plan_without_auth_returns_401(base_url: str, auth_token: str) -> None:
    recipe_id = _create_recipe(base_url, auth_token)

    resp = httpx.put(
        f"{base_url}{BASE}",
        json=_make_plan(recipe_id),
    )

    assert resp.status_code == 401


def test_put_meal_plan_returns_200_with_updated_plan(base_url: str, auth_token: str) -> None:
    recipe_id = _create_recipe(base_url, auth_token)
    headers = {"Authorization": f"Bearer {auth_token}"}

    resp = httpx.put(
        f"{base_url}{BASE}",
        json=_make_plan(recipe_id),
        headers=headers,
    )

    assert resp.status_code == 200
    data = resp.json()
    assert "slots" in data
    assert isinstance(data["slots"], list)


def test_put_meal_plan_invalid_servings_returns_400(base_url: str, auth_token: str) -> None:
    recipe_id = _create_recipe(base_url, auth_token)
    headers = {"Authorization": f"Bearer {auth_token}"}

    invalid_plan = {
        "slots": [
            {
                "weekDay": 1,
                "mealType": 1,
                "items": [{"recipeId": recipe_id, "servings": 0}],
            }
        ]
    }

    resp = httpx.put(
        f"{base_url}{BASE}",
        json=invalid_plan,
        headers=headers,
    )

    assert resp.status_code == 400


def test_put_meal_plan_invalid_week_day_returns_400(base_url: str, auth_token: str) -> None:
    recipe_id = _create_recipe(base_url, auth_token)
    headers = {"Authorization": f"Bearer {auth_token}"}

    invalid_plan = {
        "slots": [
            {
                "weekDay": 8,
                "mealType": 1,
                "items": [{"recipeId": recipe_id, "servings": 2}],
            }
        ]
    }

    resp = httpx.put(
        f"{base_url}{BASE}",
        json=invalid_plan,
        headers=headers,
    )

    assert resp.status_code == 400


# ── DELETE /api/v1/meal-plan ──────────────────────────────────────────────────

def test_delete_meal_plan_without_auth_returns_401(base_url: str) -> None:
    resp = httpx.delete(f"{base_url}{BASE}")

    assert resp.status_code == 401


def test_delete_meal_plan_returns_204(base_url: str, auth_token: str) -> None:
    headers = {"Authorization": f"Bearer {auth_token}"}

    resp = httpx.delete(f"{base_url}{BASE}", headers=headers)

    assert resp.status_code == 204


def test_delete_meal_plan_clears_slots(base_url: str, auth_token: str) -> None:
    recipe_id = _create_recipe(base_url, auth_token)
    headers = {"Authorization": f"Bearer {auth_token}"}

    # Сначала сохраняем план
    httpx.put(
        f"{base_url}{BASE}",
        json=_make_plan(recipe_id),
        headers=headers,
    )

    # Очищаем
    del_resp = httpx.delete(f"{base_url}{BASE}", headers=headers)
    assert del_resp.status_code == 204

    # После очистки GET возвращает пустые слоты
    get_resp = httpx.get(f"{base_url}{BASE}", headers=headers)
    assert get_resp.status_code == 200
    data = get_resp.json()
    total_items = sum(len(slot["items"]) for slot in data["slots"])
    assert total_items == 0


# ── Round-trip ────────────────────────────────────────────────────────────────

def test_put_then_get_returns_same_data(base_url: str, auth_token: str) -> None:
    recipe_id = _create_recipe(base_url, auth_token)
    headers = {"Authorization": f"Bearer {auth_token}"}

    plan = _make_plan(recipe_id)

    put_resp = httpx.put(f"{base_url}{BASE}", json=plan, headers=headers)
    assert put_resp.status_code == 200

    get_resp = httpx.get(f"{base_url}{BASE}", headers=headers)
    assert get_resp.status_code == 200

    data = get_resp.json()
    # Находим слот понедельника/завтрака
    slot = next(
        (s for s in data["slots"] if s["weekDay"] == 1 and s["mealType"] == 1),
        None,
    )
    assert slot is not None
    assert len(slot["items"]) == 1
    assert slot["items"][0]["recipeId"] == recipe_id
    assert slot["items"][0]["servings"] == 2
