# AR-0043: Инструменты тестирования frontend

Источник: ADR-0033

## Правило

- Unit / component: Vitest + `@testing-library/react`.
- E2E UI: Playwright против запущенного `docker compose up`.
- Покрытие: Vitest `--coverage` (v8); порог ≥ 80% statements.
