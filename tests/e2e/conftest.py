import os
import uuid

import httpx
import pytest

AUTH_BASE = "/api/cookbook/v1/auth"
ADMIN_BASE = "/api/cookbook/v1"


@pytest.fixture(scope="session")
def base_url() -> str:
    return os.environ.get("BASE_URL", "http://localhost:5500")


@pytest.fixture(scope="session")
def auth_token(base_url: str) -> str:
    email = f"e2e_{uuid.uuid4().hex[:12]}@example.com"
    password = "P@ssw0rd!"
    httpx.post(
        f"{base_url}{AUTH_BASE}/register",
        json={"email": email, "displayName": "E2E User", "password": password},
    )
    resp = httpx.post(
        f"{base_url}{AUTH_BASE}/login",
        json={"email": email, "password": password},
    )
    return resp.json()["token"]


@pytest.fixture(scope="session")
def admin_token(base_url: str) -> str:
    """Возвращает JWT администратора (seed-admin из docker-compose)."""
    admin_email = os.environ.get("ADMIN_EMAIL", "admin@cookbook.local")
    admin_password = os.environ.get("ADMIN_PASSWORD", "1234567890")
    resp = httpx.post(
        f"{base_url}{AUTH_BASE}/login",
        json={"email": admin_email, "password": admin_password},
    )
    assert resp.status_code == 200, f"Admin login failed: {resp.text}"
    return resp.json()["token"]
