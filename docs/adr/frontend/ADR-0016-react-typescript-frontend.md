# ADR-0016: React + TypeScript как UI-стек frontend

- **Статус**: принят
- **Домен**: frontend
- **Дата**: 2026-06-07

## Контекст

В рамках [ADR-0015](ADR-0015-nextjs-frontend-meta-framework.md) выбран Next.js как meta-фреймворк. Next.js построен поверх React, но это решение фиксируется отдельно, чтобы:

- Зафиксировать выбор именно React (а не альтернативного UI-движка под Next.js).
- Зафиксировать TypeScript (`strict: true`) как единственный язык frontend-домена.
- Связать выбор с NFR (100% статическая типизация, AI-friendliness).

## Рассмотренные варианты

- **React + TypeScript** — стандарт индустрии, крупнейший AI-датасет, прямое соответствие Next.js.
- **Vue + TypeScript** — рассмотрен в [ADR-0015](ADR-0015-nextjs-frontend-meta-framework.md) (Nuxt) и отклонён.
- **Svelte + TypeScript** — рассмотрен в [ADR-0015](ADR-0015-nextjs-frontend-meta-framework.md) (SvelteKit) и отклонён.
- **JavaScript без TypeScript** — нарушает NFR «100% статическая типизация».

## Решение

Frontend реализуется на **React 18+** на языке **TypeScript** с конфигурацией `strict: true`. Использование JavaScript-файлов в продакшен-коде запрещено (допустимо только в конфигах сборки при необходимости).

## Последствия

- Все компоненты, серверные модули BFF и тесты — на TypeScript.
- ESLint-правила запрещают `any` без явного обоснования (`// eslint-disable-next-line` с комментарием).
- Типы доменных DTO описываются вручную или генерируются из OpenAPI gateway (выбор способа — отдельной задачей, OQ).
- React Server Components применяются по умолчанию; клиентские компоненты помечаются директивой `"use client"`.

## Связанные документы

- [ADR-0015: Next.js как frontend meta-framework](ADR-0015-nextjs-frontend-meta-framework.md)
- [AR-0011: Frontend и BFF — TypeScript / Node.js](../../architecture/rules/frontend/AR-0011-frontend-typescript-nodejs.md)
- [Стандарт стиля TypeScript](../../standards/typescript-code-style.md)
