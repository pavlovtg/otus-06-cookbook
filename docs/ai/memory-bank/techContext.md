# Технический контекст

## Стек

- Backend: .NET 10, C#
- ORM: EF Core (Npgsql)
- БД: PostgreSQL
- API Gateway: YARP
- Архитектура backend: гексагональная (Ports & Adapters) + DDD
- Frontend: Next.js (App Router, RSC) + React 18+ + TypeScript (`strict: true`), Node.js LTS
- BFF: логический слой внутри Next.js (Route Handlers, Server Actions, `lib/bff/`)
- UI: Tailwind CSS + shadcn/ui; формы — react-hook-form + Zod
- Валидация схем: Zod (single source of truth)
- Тесты frontend: Vitest + Testing Library + Playwright
- Пакетный менеджер frontend: pnpm
- Линт/формат frontend: ESLint + Prettier

## Инструменты

- Cline (AI-ассистент)
- Git, GitHub
- Docker / Docker Compose

## Конфигурация Cline

- `.clinerules/language.md` — язык и токены
- `.clinerules/git.md` — правила git
- `.clinerules/architecture.md` — правила работы с архитектурными документами
- `.clinerules/memory-bank.md` — правила memory bank
- `.clineignore` — игнор файлов
