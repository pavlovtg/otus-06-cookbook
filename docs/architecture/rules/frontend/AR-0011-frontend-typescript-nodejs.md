# AR-0011: Frontend и BFF — TypeScript / Node.js

- **Домен**: frontend
- **Статус**: активно

## Назначение

Фиксирует стек реализации frontend-домена и явно исключает frontend и BFF из действия [AR-0006](../backend/AR-0006-backend-dotnet-csharp.md) («Backend — только .NET 10 / C#»).

## Когда применяется

При создании любых модулей и сервисов, относящихся к frontend-домену (UI + BFF).

## Правило

Frontend-сервис и BFF-слой реализуются на **TypeScript** (`strict: true`) на рантайме **Node.js LTS**.

Использование других языков и рантаймов (Python, Go, Rust, .NET и т. п.) для frontend / BFF запрещено без отдельного нового ADR.

## Разрешено

- TypeScript-конфиги (`*.ts`, `*.tsx`).
- JavaScript-файлы только для конфигов сборки, если иное технически невозможно.
- Использование актуальных мажорных версий React и Next.js (см. [ADR-0015](../../../adr/frontend/ADR-0015-nextjs-frontend-meta-framework.md), [ADR-0016](../../../adr/frontend/ADR-0016-react-typescript-frontend.md)).

## Запрещено

- Реализация frontend / BFF на других языках и рантаймах.
- Использование `any` в продакшен-коде без явного `eslint-disable` с комментарием-обоснованием.
- JavaScript-файлы в продакшен-коде frontend / BFF.

## Связанные ADR

- [ADR-0015: Next.js как frontend meta-framework](../../../adr/frontend/ADR-0015-nextjs-frontend-meta-framework.md)
- [ADR-0016: React + TypeScript как UI-стек frontend](../../../adr/frontend/ADR-0016-react-typescript-frontend.md)
