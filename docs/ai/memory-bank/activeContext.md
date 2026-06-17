# Активный контекст

## Текущая задача

Исправление падающих UI-тестов после добавления авторизации в recipes-сервис.

## Что сделано

- `apps/web/app/login/page.tsx` — заменён `router.push + router.refresh` на `window.location.href` (hard navigation) для корректного обновления серверного layout после логина
- `tests/ui/conftest.py` — добавлены фикстуры `auth_token` (session, JWT через API) и `logged_in_page` (function, логин через UI)
- `tests/ui/test_recipes.py` — все тесты, требующие авторизации, переведены на `logged_in_page`; `_api_create_ingredient` и `_api_create_recipe_with_ingredient` принимают `auth_token`; `test_ingredients_scale_plus_button` принимает `auth_token`

## Ключевые решения

- Auth-модуль — инфраструктурный cross-cutting слой внутри recipes-сервиса
- Пользователи хранятся в схеме `cookbook` той же PostgreSQL
- JWT выпускается и валидируется recipes-сервисом
- S2S не нужен (один доменный сервис в MVP)
- Token blacklist — не реализован, тест скипнут
- Hard navigation после логина нужна, т.к. Next.js layout — серверный компонент и не перерендеривается при soft navigation
- Auth route.ts использовали `/api/v1/auth/...` напрямую к gateway, но gateway знает только `/api/cookbook/**` → исправлено на `/api/cookbook/v1/auth/...`
