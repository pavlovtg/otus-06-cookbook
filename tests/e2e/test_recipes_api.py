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


# ── Recipe Photos (TEST-6) ────────────────────────────────────────────────────

PHOTOS_BASE = "/api/cookbook/v1/photos"

MINIMAL_JPEG = (
    b"\xff\xd8\xff\xe0\x00\x10JFIF\x00\x01\x01\x00\x00\x01\x00\x01\x00\x00"
    b"\xff\xdb\x00C\x00\x08\x06\x06\x07\x06\x05\x08\x07\x07\x07\t\t"
    b"\x08\n\x0c\x14\r\x0c\x0b\x0b\x0c\x19\x12\x13\x0f\x14\x1d\x1a"
    b"\x1f\x1e\x1d\x1a\x1c\x1c $.' \",#\x1c\x1c(7),01444\x1f'9=82<.342\x1e"
    b"\xff\xc0\x00\x0b\x08\x00\x01\x00\x01\x01\x01\x11\x00"
    b"\xff\xc4\x00\x1f\x00\x00\x01\x05\x01\x01\x01\x01\x01\x01\x00\x00"
    b"\x00\x00\x00\x00\x00\x00\x01\x02\x03\x04\x05\x06\x07\x08\t\n\x0b"
    b"\xff\xc4\x00\xb5\x10\x00\x02\x01\x03\x03\x02\x04\x03\x05\x05\x04"
    b"\x04\x00\x00\x01}\x01\x02\x03\x00\x04\x11\x05\x12!1A\x06\x13Qa"
    b"\xff\xda\x00\x08\x01\x01\x00\x00?\x00\xfb\xff\xd9"
)


def _create_recipe(base_url: str) -> dict:
    response = httpx.post(f"{base_url}{BASE}", json=VALID_RECIPE)
    assert response.status_code == 201
    return response.json()


def test_upload_photo_returns_photo_id(base_url: str) -> None:
    recipe = _create_recipe(base_url)
    recipe_id = recipe["id"]

    response = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/photo",
        files={"file": ("photo.jpg", MINIMAL_JPEG, "image/jpeg")},
    )

    assert response.status_code == 200
    data = response.json()
    assert "photoId" in data
    assert data["photoId"] is not None


def test_upload_photo_recipe_has_photo_id(base_url: str) -> None:
    recipe = _create_recipe(base_url)
    recipe_id = recipe["id"]

    httpx.post(
        f"{base_url}{BASE}/{recipe_id}/photo",
        files={"file": ("photo.jpg", MINIMAL_JPEG, "image/jpeg")},
    )

    get_resp = httpx.get(f"{base_url}{BASE}/{recipe_id}")
    assert get_resp.status_code == 200
    data = get_resp.json()
    assert data["photoId"] is not None


def test_get_photo_returns_image(base_url: str) -> None:
    recipe = _create_recipe(base_url)
    recipe_id = recipe["id"]

    upload_resp = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/photo",
        files={"file": ("photo.jpg", MINIMAL_JPEG, "image/jpeg")},
    )
    photo_id = upload_resp.json()["photoId"]

    photo_resp = httpx.get(f"{base_url}{PHOTOS_BASE}/{photo_id}")
    assert photo_resp.status_code == 200
    assert photo_resp.headers["content-type"].startswith("image/")


def test_delete_photo_sets_photo_id_null(base_url: str) -> None:
    recipe = _create_recipe(base_url)
    recipe_id = recipe["id"]

    httpx.post(
        f"{base_url}{BASE}/{recipe_id}/photo",
        files={"file": ("photo.jpg", MINIMAL_JPEG, "image/jpeg")},
    )

    delete_resp = httpx.delete(f"{base_url}{BASE}/{recipe_id}/photo")
    assert delete_resp.status_code == 204

    get_resp = httpx.get(f"{base_url}{BASE}/{recipe_id}")
    data = get_resp.json()
    assert data["photoId"] is None


def test_upload_invalid_format_returns_400(base_url: str) -> None:
    recipe = _create_recipe(base_url)
    recipe_id = recipe["id"]

    response = httpx.post(
        f"{base_url}{BASE}/{recipe_id}/photo",
        files={"file": ("doc.pdf", b"%PDF-1.4 fake", "application/pdf")},
    )

    assert response.status_code == 400
