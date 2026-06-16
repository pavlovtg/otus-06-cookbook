# Активный контекст

## Текущая задача

Снижение порогов coverage для lines/statements и добавление unit-тестов для UI-компонентов.

## Что сделано

### vitest.config.ts

- Пороги `lines` и `statements` снижены до `0` (RSC/Server Components не тестируемы в jsdom)
- Пороги `functions` и `branches` остались `80%`

### Новые тесты (apps/web/tests/unit/)

- `Button.test.tsx` — Button, IconButton, AsyncButton (21 тест)
- `Modal.test.tsx` — Modal (13 тестов)
- `SearchInput.test.tsx` — SearchInput controlled/uncontrolled, suggestions (13 тестов)
- `Toast.test.tsx` — Toast, ToastStack, useToasts (13 тестов)
- `Tag.test.tsx` — Tag, Chip (11 тестов)
- `Pagination.test.tsx` — Pagination controlled/uncontrolled, ellipsis (14 тестов)
- `Skeleton.test.tsx` — Skeleton, EmptyState (14 тестов)
- `icons.test.tsx` — все 23 иконки × 2 теста (46 тестов)

### Итог

- Test Files: 17 passed (17)
- Tests: 262 passed (262)
- Пороги functions/branches: выполнены (components/ui — 100%/100%)

## Ключевые решения

- **lines/statements = 0**: app/** содержит RSC, которые не запускаются в jsdom — покрытие там 0%, но это ожидаемо
- **functions/branches = 80%**: покрыты через unit-тесты UI-компонентов
- **Паттерн тестов**: render + Testing Library assertions, без моков Next.js
