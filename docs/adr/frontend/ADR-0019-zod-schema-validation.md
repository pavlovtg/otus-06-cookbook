# ADR-0019: Zod как библиотека валидации схем

- **Статус**: принят
- **Домен**: frontend
- **Дата**: 2026-06-07

## Контекст

Frontend требует единого источника правды для:

- валидации форм на клиенте;
- парсинга входа в Server Actions и Route Handlers BFF;
- вывода TypeScript-типов из схем (без дублирования).

## Рассмотренные варианты

- **Zod** — де-факто стандарт в Next.js / TypeScript-экосистеме, огромный AI-датасет, интеграция с `react-hook-form` через `@hookform/resolvers/zod`.
- **Valibot** — tree-shakable, меньший bundle, но меньший AI-датасет и меньше готовых рецептов под Next.js / react-hook-form.
- **Yup** — legacy, хуже типизирован, не рекомендуется для новых TS-проектов в 2025.
- **ArkType / Effect Schema** — продвинутая типизация, но малый AI-датасет, противоречит главному критерию выбора.

## Решение

Используется **Zod** как единственная библиотека валидации схем и источник TypeScript-типов для DTO, форм и BFF-эндпоинтов. Альтернативные библиотеки валидации запрещены без отдельного нового ADR.

## Последствия

- Каждая форма и каждый BFF-эндпоинт парсят вход через Zod-схему.
- Типы DTO выводятся через `z.infer<typeof Schema>` — без ручного дублирования.
- Интеграция с `react-hook-form` — через `@hookform/resolvers/zod`.
- Bundle Zod (~50 KB) включён в бюджет initial JS; контролируется через `next build`-отчёт.

## Связанные документы

- [ADR-0015: Next.js как frontend meta-framework](ADR-0015-nextjs-frontend-meta-framework.md)
- [ADR-0016: React + TypeScript как UI-стек frontend](ADR-0016-react-typescript-frontend.md)
- [ADR-0017: BFF как логически выделенный слой](ADR-0017-bff-logical-layer.md)
