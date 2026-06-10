# Активный контекст

## Текущая задача

**Добавление e2e и UI тестов** (10.06.2026)

## Что сделано в этой сессии

- Создан `tests/e2e/conftest.py` — фикстура `base_url` из env
- Создан `tests/e2e/test_recipes_api.py` — pytest + httpx, проверяет `GET /api/recipes/v1` возвращает непустой список
- Обновлён `tests/e2e/requirements.txt` — добавлен `pytest-playwright`
- Создан `tests/ui/requirements.txt` — pytest + pytest-playwright
- Создан `tests/ui/conftest.py` — фикстура `base_url` из env
- Создан `tests/ui/test_main_page.py` — Playwright, открывает главную страницу и проверяет статус 200
- Обновлён `.github/workflows/ci-push.yml`:
  - Добавлен `ui` output в `changes` job с paths-фильтром `tests/ui/**` + инфраструктура
  - `python` фильтр расширен на `tests/ui/**`
  - `lint-python` теперь проверяет `tests/e2e/` и `tests/ui/`
  - `ui-test` job реализован по аналогии с `e2e`: docker compose up → pip install → playwright install chromium → pytest tests/ui/

## Следующий шаг

Нет активных задач.
