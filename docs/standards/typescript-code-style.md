# Стандарт стиля TypeScript

## Назначение и область применения

Применяется ко всему TypeScript-коду в репозитории, в первую очередь — к `apps/web` (frontend + BFF).

## Правила

### Конфигурация TypeScript

- `"strict": true` (обязательно).
- `"noUncheckedIndexedAccess": true`.
- `"noImplicitAny": true` (входит в strict).
- `"exactOptionalPropertyTypes": true` (рекомендовано).

### Запрещено

- `any` без `// eslint-disable-next-line` и комментария-обоснования.
- Type assertions `as` для приведения несовместимых типов; для рантайм-парсинга использовать Zod.
- Default-export для модулей с несколькими публичными API (использовать named exports).

### Обязательно

- Все публичные функции и компоненты имеют явные типы параметров и возвращаемого значения, если они не очевидны.
- Типы DTO выводятся из Zod-схем через `z.infer<typeof Schema>`.
- Server-only модули помечаются комментарием `// server-only` в начале файла или импортом из `lib/bff/`.

### Форматирование и линт

- ESLint — `next/core-web-vitals` + `@typescript-eslint/recommended-type-checked` + правила импорта.
- Prettier — единая конфигурация в корне или `apps/web`.
- `.editorconfig` — корневой (см. [AR-0010](../architecture/rules/general/AR-0010-editorconfig-mandatory.md)).
- Линт и форматирование блокируют PR (см. [Стандарт CI](ci-standard.md)).

### Именование

- Компоненты — `PascalCase`.
- Hooks — `useXxx`.
- Типы / интерфейсы / Zod-схемы — `PascalCase`.
- Файлы компонентов — `PascalCase.tsx`; модули — `kebab-case.ts`.

## Связанные AR и ADR

- [ADR-0016: React + TypeScript как UI-стек frontend](../adr/frontend/ADR-0016-react-typescript-frontend.md)
- [ADR-0019: Zod как валидация схем](../adr/frontend/ADR-0019-zod-schema-validation.md)
- [AR-0010: .editorconfig — обязательный источник кодстайла](../architecture/rules/general/AR-0010-editorconfig-mandatory.md)
- [AR-0011: Frontend и BFF — TypeScript / Node.js](../architecture/rules/frontend/AR-0011-frontend-typescript-nodejs.md)
