# AR-0015: UI и BFF упакованы в один процесс

- **Домен**: frontend
- **Статус**: активно

## Назначение

UI и BFF реализуются в одном Node-процессе (Next.js-сервис), но разделены логически на уровне исходного кода и структуры каталогов.

## Когда применяется

При проектировании структуры `apps/web` и любых импортов между UI и BFF.

## Правило

- Frontend-сервис собирается и запускается как один Next.js-процесс (контейнер `web` в Docker Compose).
- Серверная логика BFF размещается в выделенных модулях (`lib/bff/`, `app/api/*/route.ts`, server actions).
- Client-компоненты помечаются директивой `"use client"`.
- BFF-модули не импортируются из client-компонентов; нарушение должно ловиться `import/no-restricted-paths` или эквивалентом ESLint.

## Разрешено

- Импорт client-only модулей и компонентов в server components / route handlers.
- Совместное использование чистых TypeScript-типов и Zod-схем между client и server слоями.

## Запрещено

- Импорт BFF-модулей (содержащих секреты, токены, прямые fetch к gateway) в client-компоненты.
- Выделение BFF в отдельный процесс или сервис без отдельного нового ADR.

## Связанные ADR

- [ADR-0015: Next.js как frontend meta-framework](../../../adr/frontend/ADR-0015-nextjs-frontend-meta-framework.md)
- [ADR-0017: BFF как логически выделенный слой](../../../adr/frontend/ADR-0017-bff-logical-layer.md)
