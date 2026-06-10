from playwright.sync_api import Page


def test_main_page_loads(page: Page, base_url: str) -> None:
    response = page.goto(base_url)

    assert response is not None
    assert response.status == 200
    assert page.title() != ""
