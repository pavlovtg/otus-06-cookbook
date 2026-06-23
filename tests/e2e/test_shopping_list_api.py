import httpx

BASE = "/api/cookbook/v1/shopping-list"
MEAL_PLAN_BASE = "/api/cookbook/v1/meal-plan"
RECIPES_BASE = "/api/cookbook/v1/recipes"
INGREDIENTS_BASE = "/api/cookbook/v1/ingredients"


def _create_ingredient(base_url: str, auth_token: str) -> str:
    resp = httpx.post(
        f"{base_url}{INGREDIENTS_BASE}",
        json={"title": "Мука E2E", "unit": "г", "defaultAmount": 100.0, "category": "vegetables"},
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert resp.status_code == 201, resp.text
    return resp.json()["id"]


def _create_recipe_with_ingredient(base_url: str, auth_token: str, ingredient_id: str) -> str:
    resp = httpx.post(
        f"{base_url}{RECIPES_BASE}",
        json={
            "title": "Рецепт для списка покупок E2E",
            "description": "E2E тест shopping list",
            "cookingTime": 20,
            "difficulty": "everyday",
            "servings": 2,
            "instructions": "1. Смешать.",
            "ingredients": [{"ingredientId": ingredient_id, "amount": 300, "unit": "г"}],
            "categoryIds": [],
            "isPublic": True,
        },
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert resp.status_code == 201, resp.text
    return resp.json()["id"]


def _set_meal_plan(base_url: str, auth_token: str, recipe_id: str) -> None:
    resp = httpx.put(
        f"{base_url}{MEAL_PLAN_BASE}",
        json={
            "slots": [
                {
                    "weekDay": 1,
                    "mealType": 1,
                    "items": [{"recipeId": recipe_id, "servings": 2}],
                }
            ]
        },
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert resp.status_code == 200, resp.text


def _clear_meal_plan(base_url: str, auth_token: str) -> None:
    resp = httpx.delete(
        f"{base_url}{MEAL_PLAN_BASE}",
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert resp.status_code == 204, resp.text


# ── GET /api/v1/shopping-list ─────────────────────────────────────────────────

def test_shopping_list_without_auth_returns_401(base_url: str) -> None:
    resp = httpx.get(f"{base_url}{BASE}")

    assert resp.status_code == 401


def test_shopping_list_with_empty_plan_returns_200_and_empty_array(
    base_url: str, auth_token: str
) -> None:
    _clear_meal_plan(base_url, auth_token)

    resp = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {auth_token}"},
    )

    assert resp.status_code == 200
    assert resp.json() == []


def test_shopping_list_with_filled_plan_returns_200_and_aggregated_items(
    base_url: str, auth_token: str
) -> None:
    ingredient_id = _create_ingredient(base_url, auth_token)
    recipe_id = _create_recipe_with_ingredient(base_url, auth_token, ingredient_id)
    _set_meal_plan(base_url, auth_token, recipe_id)

    resp = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {auth_token}"},
    )

    assert resp.status_code == 200
    groups = resp.json()
    assert isinstance(groups, list)
    assert len(groups) > 0

    all_items = [item for group in groups for item in group["items"]]
    matched = [item for item in all_items if item["ingredientId"] == ingredient_id]
    assert len(matched) == 1
    assert matched[0]["amount"] == 300.0
    assert matched[0]["unit"] == "г"
