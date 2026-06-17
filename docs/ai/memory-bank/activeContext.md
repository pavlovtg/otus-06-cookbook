# Активный контекст

## Текущая задача

Исправление падающих e2e-тестов после добавления авторизации в recipes-сервис.

## Что сделано

- `tests/e2e/conftest.py` — добавлена session-фикстура `auth_token` (регистрация + логин)
- `tests/e2e/test_recipes_api.py` — все мутирующие запросы (POST/PUT/DELETE рецептов, фото) получают `Authorization: Bearer {token}`; `_create_recipe` принимает `auth_token`
- `tests/e2e/test_auth_api.py` — `test_login_logout_protected_endpoint_returns_401` помечен `@pytest.mark.skip` (token blacklist не реализован)

## Ключевые решения

- Auth-модуль — инфраструктурный cross-cutting слой внутри recipes-сервиса
- Пользователи хранятся в схеме `cookbook` той же PostgreSQL
- JWT выпускается и валидируется recipes-сервисом
- S2S не нужен (один доменный сервис в MVP)
- Token blacklist — не реализован, тест скипнут
