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
    "categoryIds": [],
}

VALID_INGREDIENT = {
    "title": "Тестовая морковь",
    "unit": "г",
    "defaultAmount": 100.0,
    "category": "vegetables",
}


def test_recipes_list_returns_paged_result(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")

    assert response.status_code == 200
    data = response.json()
    assert "items" in data
    assert "total" in data
    assert "page" in data
    assert "pageSize" in data
    assert isinstance(data["items"], list)
    assert data["total"] > 0
    assert data["page"] == 1


def test_recipes_list_is_not_empty(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")

    assert response.status_code == 200
    data = response.json()
    assert len(data["items"]) > 0


def test_recipes_list_items_have_required_fields(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    data = response.json()

    for recipe in data["items"]:
        assert "id" in recipe
        assert "title" in recipe
        assert "description" in recipe
        assert "cookingTime" in recipe
        assert "difficulty" in recipe


def test_recipes_list_page2_returns_different_items(base_url: str) -> None:
    # Создаём два рецепта, чтобы гарантировать наличие хотя бы 2 записей
    r1 = httpx.post(f"{base_url}{BASE}", json={**VALID_RECIPE, "title": "Пагинация рецепт 1"})
    r2 = httpx.post(f"{base_url}{BASE}", json={**VALID_RECIPE, "title": "Пагинация рецепт 2"})
    assert r1.status_code == 201
    assert r2.status_code == 201

    resp1 = httpx.get(f"{base_url}{BASE}", params={"page": 1, "pageSize": 1})
    resp2 = httpx.get(f"{base_url}{BASE}", params={"page": 2, "pageSize": 1})

    assert resp1.status_code == 200
    assert resp2.status_code == 200

    data1 = resp1.json()
    data2 = resp2.json()

    assert data1["page"] == 1
    assert data2["page"] == 2
    assert data1["pageSize"] == 1
    assert data2["pageSize"] == 1
    assert len(data1["items"]) == 1
    assert len(data2["items"]) == 1
    assert data1["items"][0]["id"] != data2["items"][0]["id"]


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

# Минимальный валидный JPEG 1×1 px (полные таблицы Хаффмана)
MINIMAL_JPEG = bytes([
    0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01,
    0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0xFF, 0xDB, 0x00, 0x43,
    0x00, 0x08, 0x06, 0x06, 0x07, 0x06, 0x05, 0x08, 0x07, 0x07, 0x07, 0x09,
    0x09, 0x08, 0x0A, 0x0C, 0x14, 0x0D, 0x0C, 0x0B, 0x0B, 0x0C, 0x19, 0x12,
    0x13, 0x0F, 0x14, 0x1D, 0x1A, 0x1F, 0x1E, 0x1D, 0x1A, 0x1C, 0x1C, 0x20,
    0x24, 0x2E, 0x27, 0x20, 0x22, 0x2C, 0x23, 0x1C, 0x1C, 0x28, 0x37, 0x29,
    0x2C, 0x30, 0x31, 0x34, 0x34, 0x34, 0x1F, 0x27, 0x39, 0x3D, 0x38, 0x32,
    0x3C, 0x2E, 0x33, 0x34, 0x32, 0xFF, 0xC0, 0x00, 0x0B, 0x08, 0x00, 0x01,
    0x00, 0x01, 0x01, 0x01, 0x11, 0x00, 0xFF, 0xC4, 0x00, 0x1F, 0x00, 0x00,
    0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00,
    0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
    0x09, 0x0A, 0x0B, 0xFF, 0xC4, 0x00, 0xB5, 0x10, 0x00, 0x02, 0x01, 0x03,
    0x03, 0x02, 0x04, 0x03, 0x05, 0x05, 0x04, 0x04, 0x00, 0x00, 0x01, 0x7D,
    0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12, 0x21, 0x31, 0x41, 0x06,
    0x13, 0x51, 0x61, 0x07, 0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xA1, 0x08,
    0x23, 0x42, 0xB1, 0xC1, 0x15, 0x52, 0xD1, 0xF0, 0x24, 0x33, 0x62, 0x72,
    0x82, 0x09, 0x0A, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x25, 0x26, 0x27, 0x28,
    0x29, 0x2A, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x43, 0x44, 0x45,
    0x46, 0x47, 0x48, 0x49, 0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59,
    0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x73, 0x74, 0x75,
    0x76, 0x77, 0x78, 0x79, 0x7A, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89,
    0x8A, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0xA2, 0xA3,
    0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6,
    0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9,
    0xCA, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xE1, 0xE2,
    0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xF1, 0xF2, 0xF3, 0xF4,
    0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFF, 0xDA, 0x00, 0x08, 0x01, 0x01,
    0x00, 0x00, 0x3F, 0x00, 0xFB, 0xD2, 0x8A, 0x28, 0x03, 0xFF, 0xD9,
])


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


# ── Search & Sort ─────────────────────────────────────────────────────────────

def test_search_by_single_word(base_url: str) -> None:
    unique = uuid.uuid4().hex[:8]
    httpx.post(f"{base_url}{BASE}", json={**VALID_RECIPE, "title": f"Рецепт {unique}"})

    resp = httpx.get(f"{base_url}{BASE}", params={"q": unique, "pageSize": 1000})

    assert resp.status_code == 200
    data = resp.json()
    ids_found = [r["title"] for r in data["items"]]
    assert any(unique in t for t in ids_found)


def test_search_by_multiple_words(base_url: str) -> None:
    word1 = uuid.uuid4().hex[:8]
    word2 = uuid.uuid4().hex[:8]
    httpx.post(f"{base_url}{BASE}", json={**VALID_RECIPE, "title": f"{word1} {word2} суп"})

    resp = httpx.get(f"{base_url}{BASE}", params={"q": f"{word1} {word2}", "pageSize": 1000})

    assert resp.status_code == 200
    data = resp.json()
    titles = [r["title"] for r in data["items"]]
    assert any(word1 in t and word2 in t for t in titles)


def test_search_empty_result(base_url: str) -> None:
    nonexistent = "zzz_" + uuid.uuid4().hex

    resp = httpx.get(f"{base_url}{BASE}", params={"q": nonexistent})

    assert resp.status_code == 200
    data = resp.json()
    assert data["total"] == 0
    assert data["items"] == []


def test_sort_title_asc(base_url: str) -> None:
    prefix = uuid.uuid4().hex[:8]
    httpx.post(f"{base_url}{BASE}", json={**VALID_RECIPE, "title": f"{prefix} Б"})
    httpx.post(f"{base_url}{BASE}", json={**VALID_RECIPE, "title": f"{prefix} А"})

    resp = httpx.get(f"{base_url}{BASE}", params={"q": prefix, "sort": "title_asc", "pageSize": 1000})

    assert resp.status_code == 200
    titles = [r["title"] for r in resp.json()["items"]]
    assert titles == sorted(titles)


def test_sort_title_desc(base_url: str) -> None:
    prefix = uuid.uuid4().hex[:8]
    httpx.post(f"{base_url}{BASE}", json={**VALID_RECIPE, "title": f"{prefix} А"})
    httpx.post(f"{base_url}{BASE}", json={**VALID_RECIPE, "title": f"{prefix} Б"})

    resp = httpx.get(f"{base_url}{BASE}", params={"q": prefix, "sort": "title_desc", "pageSize": 1000})

    assert resp.status_code == 200
    titles = [r["title"] for r in resp.json()["items"]]
    assert titles == sorted(titles, reverse=True)
