# ADR-0017: BFF как логически выделенный слой внутри frontend-сервиса

- **Статус**: принят
- **Домен**: frontend
- **Дата**: 2026-06-07

## Контекст

Frontend должен:

- Хранить JWT вне браузера — токен живёт на серверной стороне, в браузер уходит только httpOnly + Secure + SameSite cookie с подписанной сессией.
- Агрегировать данные нескольких backend-эндпоинтов под нужды UI (минимум: страница рецепта = рецепт + комментарии + рейтинг + автор одним запросом).
- Выполнять SSR публичных страниц и проксировать запросы к YARP Gateway.

Это классический паттерн BFF (Backend-for-Frontend). Возникает вопрос упаковки: отдельный сервис или часть frontend-приложения.

В сводном документе требований зафиксировано: BFF — отдельная **сущность** (логически), не обязан быть отдельным **процессом**. Приоритет — срок 1 неделя.

## Рассмотренные варианты

- **BFF как набор серверных модулей внутри Next.js-приложения** (Server Components, Server Actions, Route Handlers) — один контейнер, минимум инфраструктуры, граница «UI vs BFF» — на уровне кода.
- **BFF как отдельный Node-сервис (Fastify/Hono/NestJS)** + SPA — максимальное разделение, но удваивает количество сборок и Dockerfile, риск срыва срока за 1 неделю.

## Решение

BFF реализуется как **логически выделенный слой внутри Next.js-сервиса**:

- Серверная логика BFF размещается в специально маркированных модулях (директория `lib/bff/` или эквивалент), запрещённых к импорту из client-компонентов.
- HTTP-эндпоинты BFF — Route Handlers (`app/api/*/route.ts`).
- Мутации — Server Actions.
- Агрегация данных страниц — в Server Components / Route Handlers.
- BFF не содержит бизнес-логики; вся бизнес-логика — в backend-сервисах за YARP Gateway.

## Последствия

- Один контейнер `web` обслуживает и UI, и BFF.
- Граница «UI vs BFF» соблюдается через структуру каталогов и ESLint-правила, а не через сеть.
- BFF stateless: сессии — в signed encrypted cookie (см. [AR-0013](../../architecture/rules/frontend/AR-0013-bff-stateless.md)).
- JWT никогда не покидает серверную сторону (см. [AR-0014](../../architecture/rules/frontend/AR-0014-jwt-not-leak-to-browser.md)).
- BFF — единственная точка, из которой делаются HTTP-вызовы к YARP Gateway со стороны frontend-домена.
- Запрещено помещать доменную/бизнес-логику в BFF (см. [AR-0012](../../architecture/rules/frontend/AR-0012-bff-no-business-logic.md)).

## Связанные документы

- [ADR-0015: Next.js как frontend meta-framework](ADR-0015-nextjs-frontend-meta-framework.md)
- [AR-0012: BFF не содержит бизнес-логики](../../architecture/rules/frontend/AR-0012-bff-no-business-logic.md)
- [AR-0013: BFF stateless](../../architecture/rules/frontend/AR-0013-bff-stateless.md)
- [AR-0014: JWT не покидает BFF](../../architecture/rules/frontend/AR-0014-jwt-not-leak-to-browser.md)
- [AR-0015: UI и BFF упакованы в один процесс](../../architecture/rules/frontend/AR-0015-ui-and-bff-single-process.md)
- [AR-0003: Frontend взаимодействует с backend только через API Gateway](../../architecture/rules/rest-api/AR-0003-frontend-via-api-gateway.md)
- [Стандарт структуры frontend-проекта](../../standards/frontend-project-structure.md)
