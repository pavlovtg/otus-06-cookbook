import httpx


def test_recipes_list_is_not_empty(base_url: str) -> None:
    response = httpx.get(f"{base_url}/api/cookbook/recipes/v1")

    assert response.status_code == 200
    recipes = response.json()
    assert isinstance(recipes, list)
    assert len(recipes) > 0
