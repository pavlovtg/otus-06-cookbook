import os
import uuid

import httpx
import pytest

AUTH_BASE = "/api/cookbook/v1/auth"


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
