# AR-0014: Ошибки REST API — Problem+JSON (RFC 7807)

## Правило

Все ошибочные ответы REST API (4xx и 5xx) сериализуются по RFC 7807 с `Content-Type: application/problem+json` и обязательными полями `type`, `title`, `status`, `detail`, `instance`. Правило распространяется на все backend-сервисы, BFF и API Gateway.

## Запрещено

- Возврат произвольных JSON-структур ошибок (`{ "error": "..." }`, `{ "message": "..." }`).
- HTML/text-ответы при 4xx/5xx от защищённых маршрутов.
- Утечка стек-трейсов, имён внутренних типов и SQL в полях `detail`/`title`.
