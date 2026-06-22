import uuid

import httpx
from playwright.sync_api import Page, expect

_API_BASE = "/api/cookbook/v1"

VALID_RECIPE = {
    "title": "UI-тест комментариев",
    "description": "Рецепт для тестирования комментариев",
    "cookingTime": 30,
    "difficulty": "everyday",
    "servings": 2,
    "instructions": "Шаг 1.",
    "ingredients": [],
    "categoryIds": [],
    "isPublic": True,
}


def _api_create_recipe(base_url: str, auth_token: str) -> str:
    """Создаёт рецепт через API и возвращает его id."""
    resp = httpx.post(
        f"{base_url}{_API_BASE}/recipes",
        json={**VALID_RECIPE, "title": f"Комментарии UI {uuid.uuid4().hex[:8]}"},
        headers={"Authorization": f"Bearer {auth_token}"},
    )
    assert resp.status_code == 201, resp.text
    return resp.json()["id"]


def _api_delete_comment_if_exists(base_url: str, auth_token: str, recipe_id: str) -> None:
    """Удаляет существующий комментарий seed-пользователя к рецепту (если есть)."""
    resp = httpx.get(
        f"{base_url}{_API_BASE}/recipes/{recipe_id}/comments",
        params={"pageSize": 100},
    )
    if resp.status_code != 200:
        return
    items = resp.json().get("items", [])
    for item in items:
        httpx.delete(
            f"{base_url}{_API_BASE}/recipes/{recipe_id}/comments/{item['id']}",
            headers={"Authorization": f"Bearer {auth_token}"},
        )


def test_add_comment_appears_in_list(
    logged_in_page: Page, base_url: str, auth_token: str
) -> None:
    """12.1: Авторизованный пользователь добавляет комментарий — он появляется в списке."""
    page = logged_in_page
    recipe_id = _api_create_recipe(base_url, auth_token)

    # Убеждаемся, что у seed-пользователя нет комментария к этому рецепту
    _api_delete_comment_if_exists(base_url, auth_token, recipe_id)

    page.goto(f"{base_url}/recipes/{recipe_id}")
    expect(page.locator(".detail-bar")).to_be_visible()

    # Форма добавления комментария должна быть видна
    textarea = page.locator("textarea.textarea")
    expect(textarea).to_be_visible()

    comment_text = f"Тест комментария {uuid.uuid4().hex[:8]}"
    textarea.fill(comment_text)

    # Нажимаем «Отправить»
    submit_btn = page.locator("button[type=submit]", has_text="Отправить")
    expect(submit_btn).to_be_enabled()
    submit_btn.click()

    # Комментарий должен появиться в списке
    comment_locator = page.locator(".comments-list .comment .text", has_text=comment_text)
    expect(comment_locator).to_be_visible(timeout=10_000)


def test_delete_comment_disappears_from_list(
    logged_in_page: Page, base_url: str, auth_token: str
) -> None:
    """12.2: Авторизованный пользователь удаляет свой комментарий — он исчезает из списка."""
    page = logged_in_page
    recipe_id = _api_create_recipe(base_url, auth_token)

    # Убеждаемся, что у seed-пользователя нет комментария к этому рецепту
    _api_delete_comment_if_exists(base_url, auth_token, recipe_id)

    page.goto(f"{base_url}/recipes/{recipe_id}")
    expect(page.locator(".detail-bar")).to_be_visible()

    # Добавляем комментарий через UI
    textarea = page.locator("textarea.textarea")
    expect(textarea).to_be_visible()

    comment_text = f"Удаляемый комментарий {uuid.uuid4().hex[:8]}"
    textarea.fill(comment_text)

    submit_btn = page.locator("button[type=submit]", has_text="Отправить")
    expect(submit_btn).to_be_enabled()
    submit_btn.click()

    # Ждём появления комментария
    comment_locator = page.locator(".comments-list .comment .text", has_text=comment_text)
    expect(comment_locator).to_be_visible(timeout=10_000)

    # Нажимаем «Удалить» на нашем комментарии
    comment_block = page.locator(".comments-list .comment", has=page.locator(".text", has_text=comment_text))
    delete_btn = comment_block.locator("button", has_text="Удалить")
    expect(delete_btn).to_be_visible()
    delete_btn.click()

    # Комментарий должен исчезнуть из списка
    expect(comment_locator).not_to_be_visible(timeout=10_000)
