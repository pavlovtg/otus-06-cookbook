# AR-0009: Frontend и BFF — TypeScript / Node.js

Источник: ADR-0015, ADR-0016

## Правило

Frontend-сервис и BFF-слой реализуются на **TypeScript** (`strict: true`) на рантайме **Node.js LTS**. Frontend и BFF исключены из действия AR-0005 («Backend — только .NET 10 / C#»).

## Запрещено

- Реализация frontend / BFF на других языках и рантаймах (Python, Go, Rust, .NET и т. п.).
- `any` в продакшен-коде без `// eslint-disable-next-line` с комментарием-обоснованием.
- JavaScript-файлы в продакшен-коде frontend / BFF (допустимо только для конфигов сборки, если иное технически невозможно).
