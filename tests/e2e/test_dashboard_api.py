import httpx
import pytest

BASE = "/api/cookbook/v1/dashboard"
AUTH_BASE = "/api/cookbook/v1/auth"


@pytest.fixture(scope="module")
def admin_token(base_url: str) -> str:
    """Получить токен администратора (предполагается, что admin@example.com существует)."""
    resp = httpx.post(
        f"{base_url}{AUTH_BASE}/login",
        json={"email": "admin@example.com", "password": "Admin1234!"},
    )
    if resp.status_code != 200:
        pytest.skip("Администратор недоступен — пропускаем admin-тесты")
    return resp.json()["token"]


# ── Гость ────────────────────────────────────────────────────────────────────


def test_dashboard_guest_returns_200(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    assert response.status_code == 200


def test_dashboard_guest_has_total_recipes(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    data = response.json()
    assert "totalRecipes" in data
    assert isinstance(data["totalRecipes"], int)
    assert data["totalRecipes"] >= 0


def test_dashboard_guest_has_top10_by_rating(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    data = response.json()
    assert "top10ByRating" in data
    assert isinstance(data["top10ByRating"], list)
    assert len(data["top10ByRating"]) <= 10


def test_dashboard_guest_top10_items_have_required_fields(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    data = response.json()
    for item in data["top10ByRating"]:
        assert "id" in item
        assert "title" in item
        assert "averageRating" in item


def test_dashboard_guest_has_by_main_ingredient(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    data = response.json()
    assert "byMainIngredient" in data
    assert isinstance(data["byMainIngredient"], list)


def test_dashboard_guest_has_by_cuisine(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    data = response.json()
    assert "byCuisine" in data
    assert isinstance(data["byCuisine"], list)


def test_dashboard_guest_category_items_have_required_fields(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    data = response.json()
    for item in data["byMainIngredient"] + data["byCuisine"]:
        assert "categoryName" in item
        assert "recipeCount" in item
        assert isinstance(item["recipeCount"], int)
        assert item["recipeCount"] >= 0


def test_dashboard_guest_top10_sorted_by_rating_desc(base_url: str) -> None:
    response = httpx.get(f"{base_url}{BASE}")
    data = response.json()
    ratings = [item["averageRating"] for item in data["top10ByRating"]]
    assert ratings == sorted(ratings, reverse=True)


# ── Авторизованный пользователь ───────────────────────────────────────────────


def test_dashboard_auth_returns_200(base_url: str, auth_token: str) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert response.status_code == 200


def test_dashboard_auth_has_my_recipes(base_url: str, auth_token: str) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    data = response.json()
    assert "myRecipes" in data
    assert isinstance(data["myRecipes"], int)
    assert data["myRecipes"] >= 0


def test_dashboard_auth_has_my_comments(base_url: str, auth_token: str) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    data = response.json()
    assert "myComments" in data
    assert isinstance(data["myComments"], int)
    assert data["myComments"] >= 0


def test_dashboard_auth_has_top_favorites_by_rating(
    base_url: str, auth_token: str
) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    data = response.json()
    assert "topFavoritesByRating" in data
    assert isinstance(data["topFavoritesByRating"], list)
    assert len(data["topFavoritesByRating"]) <= 10


def test_dashboard_auth_has_plan_fill(base_url: str, auth_token: str) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    data = response.json()
    # planFill может быть null или словарём
    assert "planFill" in data
    if data["planFill"] is not None:
        assert isinstance(data["planFill"], dict)
        for value in data["planFill"].values():
            assert isinstance(value, bool)


# ── Администратор ─────────────────────────────────────────────────────────────


def test_dashboard_admin_returns_200(base_url: str, admin_token: str) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {admin_token}"},
    )
    assert response.status_code == 200


def test_dashboard_admin_has_total_users(base_url: str, admin_token: str) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {admin_token}"},
    )
    data = response.json()
    assert "totalUsers" in data
    assert isinstance(data["totalUsers"], int)
    assert data["totalUsers"] >= 0


def test_dashboard_admin_has_total_comments(base_url: str, admin_token: str) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {admin_token}"},
    )
    data = response.json()
    assert "totalComments" in data
    assert isinstance(data["totalComments"], int)
    assert data["totalComments"] >= 0


def test_dashboard_admin_has_top_users_by_rating(
    base_url: str, admin_token: str
) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {admin_token}"},
    )
    data = response.json()
    assert "topUsersByRating" in data
    assert isinstance(data["topUsersByRating"], list)
    assert len(data["topUsersByRating"]) <= 10


def test_dashboard_admin_has_top_users_by_comments(
    base_url: str, admin_token: str
) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {admin_token}"},
    )
    data = response.json()
    assert "topUsersByComments" in data
    assert isinstance(data["topUsersByComments"], list)
    assert len(data["topUsersByComments"]) <= 10


def test_dashboard_admin_user_rank_items_have_required_fields(
    base_url: str, admin_token: str
) -> None:
    response = httpx.get(
        f"{base_url}{BASE}",
        headers={"Authorization": f"Bearer {admin_token}"},
    )
    data = response.json()
    for item in (data.get("topUsersByRating") or []) + (
        data.get("topUsersByComments") or []
    ):
        assert "id" in item
        assert "displayName" in item
        assert "commentCount" in item
