# NFR-0001: Авторизация и JWT

Нефункциональные требования к подсистеме авторизации (см. ADR-0005, ADR-0035).

## Производительность

| Метрика | Цель |
|---|---|
| `POST /auth/login` latency | p95 ≤ 300 ms, p99 ≤ 800 ms |
| JWT validation overhead (в recipes-сервисе) | p95 ≤ 5 ms, p99 ≤ 15 ms |
| Throughput `/auth/login` | ≤ 10 RPS |
| Throughput validate | ≤ 200 RPS |

## Объём данных

| Метрика | Цель |
|---|---|
| Количество пользователей в 1 год | ≤ 10 000 |
| Размер JWT | ≤ 1 KB |

## Безопасность

- Хэш паролей: BCrypt cost ≥ 12.
- TLS обязателен только на edge (внутри Docker Compose plaintext допустим).
- Session cookie: `HttpOnly`, `Secure`, `SameSite=Lax`.
- JWT-алгоритм: HS256; длина секрета ≥ 256 бит.
- Clock skew при валидации `exp` ≤ 60 секунд.
- JWT не покидает серверную сторону BFF (см. ADR-0017).

## Сценарии и use cases

- UC1: регистрация + первый логин → получение JWT → запрос к защищённому API через YARP.
- UC5: доступ к чужому приватному ресурсу → 403 (проверка `resource.AuthorId == jwt.sub` в Application-слое).

## Out of scope (MVP)

- Refresh-токены (TTL access-токена 24 ч, по истечении — повторный логин).
- Blacklist / revocation / force-logout всех сессий.
- Ротация подписывающего ключа, JWKS.
- Rate-limit на `/auth/login`.
- Восстановление и смена пароля.
- Админ-операции (бан пользователя).
- S2S-аутентификация (нет межсервисных вызовов в MVP).
