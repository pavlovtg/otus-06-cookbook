# Стандарт структуры frontend-проекта

## Назначение и область применения

Описывает структуру каталогов и базовые правила проекта `apps/web` — Next.js приложения, объединяющего UI и BFF-слой.

## Правила

### Расположение

Frontend-приложение размещается в `apps/web/`:

```
apps/
  web/
    app/                      # маршруты Next.js (App Router)
    │   ├── (public)/         # публичные маршруты с SSR (рецепт, поиск)
    │   ├── (auth)/           # защищённые маршруты
    │   └── api/              # Route Handlers — BFF HTTP-эндпоинты
    components/               # client + server компоненты UI
    │   ├── ui/               # shadcn/ui-компоненты
    │   └── features/         # доменные компоненты страниц
    lib/
    │   ├── bff/              # серверная логика BFF (запрещено импортировать из client)
    │   │   ├── gateway.ts    # HTTP-клиент к YARP Gateway
    │   │   ├── session.ts    # signed encrypted cookie
    │   │   └── csrf.ts       # CSRF-защита
    │   ├── schemas/          # Zod-схемы и выводимые типы
    │   └── utils/            # чистые хелперы (client+server)
    tests/
    │   ├── unit/             # Vitest + Testing Library
    │   └── e2e/              # Playwright
    public/                   # статика
    .eslintrc.cjs
    tailwind.config.ts
    tsconfig.json
    next.config.ts
    package.json
    Dockerfile
```

### Границы UI / BFF

- Модули из `lib/bff/` запрещено импортировать в client-компоненты (`"use client"`).
- Граница обеспечивается ESLint-правилом `import/no-restricted-paths` или эквивалентом.
- Route Handlers и Server Actions могут импортировать `lib/bff/` свободно.

### Сборка

- Пакетный менеджер — `pnpm`.
- Build — `next build`; запуск — `next start` (или standalone-output в Docker).
- Multi-stage Dockerfile: `node:lts-alpine` deps → builder → runner.

### Конфигурация

- `tsconfig.json` — `"strict": true`, `"noUncheckedIndexedAccess": true`.
- `next.config.ts` — `output: "standalone"` для Docker.
- Tailwind конфигурируется в `tailwind.config.ts`, токены дизайн-системы — там же.

## Связанные AR и ADR

- [ADR-0015: Next.js как frontend meta-framework](../adr/frontend/ADR-0015-nextjs-frontend-meta-framework.md)
- [ADR-0017: BFF как логически выделенный слой](../adr/frontend/ADR-0017-bff-logical-layer.md)
- [ADR-0018: Tailwind CSS + shadcn/ui](../adr/frontend/ADR-0018-tailwind-shadcn.md)
- [AR-0011: Frontend и BFF — TypeScript / Node.js](../architecture/rules/frontend/AR-0011-frontend-typescript-nodejs.md)
- [AR-0015: UI и BFF упакованы в один процесс](../architecture/rules/frontend/AR-0015-ui-and-bff-single-process.md)
- [Стандарт структуры репозитория](repository-structure.md)
