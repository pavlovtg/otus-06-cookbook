# AR-0063: BFF Server Components используют serverFetch

## Правило

Серверные BFF-функции, вызываемые из Server Components, размещаются в файлах с суффиксом `.server.ts` в `lib/bff/`, в первой строке содержат `import "server-only"` и используют `serverFetch` из `lib/server-fetch.ts` для запросов к `SERVER_BASE`. `serverFetch` автоматически добавляет заголовок `Authorization: Bearer <token>` из iron-session. Клиентские мутации (POST/PUT/DELETE через `CLIENT_BASE`) остаются в `lib/bff/<name>.ts` без зависимости от `server-only`/`session`.

## Запрещено

- Прямой вызов `fetch(SERVER_BASE/...)` в серверных BFF-функциях.
- Импорт `server-fetch.ts` или `session.ts` из файлов, которые могут попасть в клиентский бандл (любой `*.ts` без `import "server-only"`, импортируемый из Client Components).
- Динамический `await import("@/lib/server-fetch")` как способ изоляции server-only кода — не работает, webpack трассирует зависимость в клиентский бандл.
- Ручное чтение сессии и формирование заголовка `Authorization` внутри BFF-функций.
