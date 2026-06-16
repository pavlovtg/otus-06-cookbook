from playwright.sync_api import Page, expect


def test_main_page_loads(page: Page, base_url: str) -> None:
    response = page.goto(base_url)

    assert response is not None
    assert response.status == 200
    assert page.title() != ""


def test_main_page_shows_recipes_without_query_params(page: Page, base_url: str) -> None:
    """Главная страница без параметров URL показывает список рецептов (регрессия)."""
    # Открываем чистый URL без ?page=, ?q=, ?sort=
    page.goto(base_url)

    cards = page.locator(".recipe-card")
    expect(cards.first).to_be_visible()
    assert cards.count() > 0, (
        "Ожидали карточки рецептов на главной странице без параметров URL"
    )
