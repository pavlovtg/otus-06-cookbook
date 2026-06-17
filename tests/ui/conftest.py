import os
import httpx
import pytest
from playwright.sync_api import Page


BASE_URL_DEFAULT = "http://localhost:5500"

SEED_EMAIL = "user@cookbook.local"
SEED_PASSWORD = "1234567890"

_API_AUTH = "/api/cookbook/v1/auth"


@pytest.fixture(scope="session")
def base_url() -> str:
    return os.environ.get("BASE_URL", BASE_URL_DEFAULT)


@pytest.fixture(scope="session")
def auth_token(base_url: str) -> str:
    """Регистрирует seed-пользователя (если не существует) и возвращает JWT."""
    # Пробуем залогиниться
    resp = httpx.post(
        f"{base_url}{_API_AUTH}/login",
        json={"email": SEED_EMAIL, "password": SEED_PASSWORD},
    )
    if resp.status_code == 200:
        return resp.json()["token"]

    # Если 401 — пробуем зарегистрироваться
    reg = httpx.post(
        f"{base_url}{_API_AUTH}/register",
        json={
            "email": SEED_EMAIL,
            "password": SEED_PASSWORD,
            "displayName": "Seed User",
        },
    )
    assert reg.status_code in (200, 201, 409), reg.text

    resp2 = httpx.post(
        f"{base_url}{_API_AUTH}/login",
        json={"email": SEED_EMAIL, "password": SEED_PASSWORD},
    )
    assert resp2.status_code == 200, resp2.text
    return resp2.json()["token"]


@pytest.fixture
def logged_in_page(page: Page, base_url: str) -> Page:
    """Открывает /login, входит под seed-пользователем и возвращает page."""
    page.goto(f"{base_url}/login")
    page.fill("#email", SEED_EMAIL)
    page.fill("#password", SEED_PASSWORD)
    page.click("button[type=submit]")
    page.wait_for_url(f"{base_url}/", timeout=15_000)
    return page
