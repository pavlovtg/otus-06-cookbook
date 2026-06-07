# NFR-0001: Авторизация и JWT

Нефункциональные требования к подсистеме авторизации (см. ADR-0005, ADR-0021, ADR-0022).

## Производительность

| Метрика | Цель |
|---|---|
| `POST /auth/login` latency | p95 ≤ 300 ms, p99 ≤ 800 ms |
| JWT validation overhead (на YARP) | p95 ≤ 5 ms, p99 ≤ 15 ms |
| Throughput `/auth/login` | ≤ 10 RPS |
| Throughput validate | ≤ 200 RPS |

## Доступность

| Метрика | Цель |
|---|---|
| Uptime `auth-service` | 99 % / месяц |

## Восстановление

| Метрика | Цель |
|---|---|
| RPO (users-data) | ≤ 24 ч |
| RTO (users-data) | ≤ 4 ч |

## Объём данных

| Метрика | Цель |
|---|---|
| Количество пользователей в 1 год | ≤ 10 000 |
| Размер JWT | ≤ 1 KB |

## Безопасность

- Хэш паролей: Argon2id или BCrypt cost ≥ 12.
- TLS обязателен только на edge (внутри Docker Compose plaintext допустим).
- Session cookie: `HttpOnly`, `Secure`, `SameSite=Lax`.
- JWT-алгоритм: HS256; длина секрета ≥ 256 бит.
- Clock skew при валидации `exp` ≤ 60 секунд.
- JWT не покидает серверную сторону BFF (см. ADR-0017).

## Operability

| Метрика | Цель |
|---|---|
| Time-to-integrate нового backend-сервиса с авторизацией | ≤ 1 рабочий день |

## Compliance

Не применяется (учебный проект).

## Сценарии и use cases

- UC1: регистрация + первый логин → получение JWT → запрос к защищённому API через YARP.
- UC4: межсервисный вызов backend↔backend по JWT (S2S, OAuth 2.0 client_credentials).
- UC5: доступ к чужому приватному ресурсу → 403 (проверка `resource.AuthorId == jwt.sub` в Application-слое доменного сервиса).

## Out of scope (MVP)

- Refresh-токены (TTL access-токена 24 ч, по истечении — повторный логин).
- Blacklist / revocation / force-logout всех сессий.
- Ротация подписывающего ключа, JWKS.
- Rate-limit на `/auth/login`.
- Восстановление и смена пароля.
- Админ-операции (бан пользователя, управление клиентами через API).
