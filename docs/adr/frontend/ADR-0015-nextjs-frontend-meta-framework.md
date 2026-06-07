# ADR-0015: Next.js как frontend meta-framework

- **Статус**: принят
- **Домен**: frontend
- **Дата**: 2026-06-07

## Контекст

Проекту нужен production-like frontend, удовлетворяющий следующим требованиям:

- SSR для публичных страниц рецепта и поиска (SEO-готовность).
- BFF как логически выделенный слой: httpOnly cookie с сессией, агрегация данных страницы рецепта, прокси к YARP Gateway.
- LCP p75 ≤ 2.5 s, TTI ≤ 3.0 s, initial JS ≤ 200 KB gzip, Lighthouse Performance ≥ 80.
- Срок MVP — 1 неделя; команда 1 разработчик + AI; главный критерий выбора — уверенность AI-ассистента в стеке.
- BFF реализуется не на .NET (frontend-домен исключён из AR-0006).

## Рассмотренные варианты

- **Next.js (App Router, RSC) + React + TS** — крупнейший AI-датасет среди meta-фреймворков, богатая экосистема (dnd-kit, react-hook-form, shadcn/ui), Server Components и Server Actions покрывают BFF-слой из коробки.
- **Nuxt 3 + Vue 3 + TS** — простая модель, отличный DX, но меньший AI-датасет и более слабая экосистема dnd/UI-китов.
- **SvelteKit + Svelte 5 + TS** — минимальный bundle, но AI часто путает Svelte 4/5, узкая экосистема готовых компонентов и dnd.
- **SPA + отдельный BFF-сервис (React/Vue + Fastify/NestJS)** — максимальное разделение слоёв, но за 1 неделю одним разработчиком SSR с нуля — высокий риск срыва срока.
- **Server-driven (HTML-over-the-wire, HTMX/Turbo)** — минимум JS, но drag-and-drop планировщик не вписывается в парадигму.

## Решение

Frontend реализуется как single-service приложение на **Next.js (App Router, React Server Components, Server Actions, Route Handlers)** на рантайме **Node.js LTS**. BFF реализуется как набор серверных модулей внутри того же приложения (см. [ADR-0017](ADR-0017-bff-logical-layer.md)).

Точные мажорные версии Node.js и Next.js фиксируются при первоначальной настройке проекта; используется текущий Node LTS и актуальный мажорный Next.

## Последствия

- Один контейнер `web` в Docker Compose — минимум инфраструктурного кода.
- Server Components / Server Actions / Route Handlers покрывают задачи BFF без отдельного сервиса.
- Vendor lock-in на Next.js принят сознательно; учитывая горизонт «портфолио / архив» (см. требования), это допустимо.
- Запрещены альтернативные meta-фреймворки и пользовательские SSR-стек без отдельного нового ADR.
- Требуется зафиксировать AR и стандарты frontend-домена.

## Связанные документы

- [ADR-0016: React + TypeScript как UI-стек](ADR-0016-react-typescript-frontend.md)
- [ADR-0017: BFF как логически выделенный слой](ADR-0017-bff-logical-layer.md)
- [ADR-0018: Tailwind CSS + shadcn/ui](ADR-0018-tailwind-shadcn.md)
- [ADR-0019: Zod как валидация схем](ADR-0019-zod-schema-validation.md)
- [AR-0011: Frontend и BFF — TypeScript / Node.js](../../architecture/rules/frontend/AR-0011-frontend-typescript-nodejs.md)
- [AR-0015: UI и BFF упакованы в один процесс](../../architecture/rules/frontend/AR-0015-ui-and-bff-single-process.md)
- [Стандарт структуры frontend-проекта](../../standards/frontend-project-structure.md)
