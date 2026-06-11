# Активный контекст

## Текущая задача

**STYLE_GUIDE из шота Tradeo Fintech SaaS** (11.06.2026)

## Что сделано в этой сессии

- Собраны кадры шота `dribbble.com/shots/26253584-Tradeo-Fintech-Saas-Website` через Playwright headful Chromium (мимо AWS WAF).
- Найдена живая реализация: `tradeo-saas.webflow.io/home-v1`. Сняты full-page + 12 посекционных кропов и computed-styles токены (цвета, радиусы, шрифты, тени, градиенты).
- Создан `docs/design/moodboard/tradeo-fintech/` с 4 PNG-кадрами шота + JPEG-секциями + `sources.md`.
- Создан `docs/design/guide/STYLE_GUIDE.md` — декомпозиция в два слоя (A: подача обложки на purple-фоне; B: dark product UI), палитра, типографика Inter 400/500-only, радиусы 100/12/8, край через inset+outer shadow (не border), каталог компонентов, fintech дата-виз, анти-паттерны + чек-лист.
- Создан `docs/design/guide/structure_selects/` — 7 структурных кадров под дизайн-генератор.

## Следующий шаг

Использовать STYLE_GUIDE для создания макета и Next.js-реализации (отдельная задача).
