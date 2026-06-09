# AR-0030: CI toolchain — фиксированный набор инструментов

Источник: ADR-0030

## Правило

Каждый стек использует строго определённый линтер и тест-раннер в CI:

| Стек | Линтер | Тест-раннер |
|------|--------|-------------|
| C# (.NET) | `dotnet format --verify-no-changes` | `dotnet test` |
| Next.js (TypeScript) | `next lint` (через `pnpm lint`) | `vitest run` (через `pnpm test`) |
| Python | `ruff check` | `pytest` |
| Markdown | `markdownlint-cli2` | — |
| UI (браузер) | — | Playwright |

## Запрещено

- Использовать альтернативные линтеры или тест-раннеры без обновления этого правила.
- Пропускать lint или test шаг для любого стека в CI.
