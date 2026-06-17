# AR-0063: BFF Server Components используют serverFetch

## Правило

Серверные BFF-функции в `lib/bff/*.ts`, вызываемые из Server Components, обязаны использовать `serverFetch` из `lib/server-fetch.ts` вместо нативного `fetch` для запросов к `SERVER_BASE`. `serverFetch` автоматически добавляет заголовок `Authorization: Bearer <token>` из iron-session. Поскольку `lib/server-fetch.ts` использует `next/headers`, импорт должен быть динамическим (`await import("@/lib/server-fetch")`), чтобы не попасть в клиентский бандл.

## Запрещено

- Прямой вызов `fetch(SERVER_BASE/...)` в BFF-функциях, вызываемых из Server Components.
- Статический импорт `server-fetch.ts` в файлах, которые могут быть импортированы из Client Components.
- Ручное чтение сессии и формирование заголовка `Authorization` внутри BFF-функций.
