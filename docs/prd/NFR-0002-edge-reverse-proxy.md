# NFR-0002: Edge Reverse Proxy

Нефункциональные требования к edge reverse proxy (см. ADR-0020, AR-0011).

## Производительность

| Метрика | Цель |
|---|---|
| Throughput пик (MVP) | 50 RPS |
| Throughput avg (MVP) | 5 RPS |
| Throughput горизонт 12 мес | 200 RPS |
| Latency overhead proxy → upstream | p95 ≤ 10 ms, p99 ≤ 30 ms |
| Latency для cache hit (статика) | p95 ≤ 30 ms |
| Cache hit ratio статики | ≥ 90 % |

## Доступность

| Метрика | Цель |
|---|---|
| Uptime edge proxy | 99.9 % / месяц (≈43 мин даунтайма) |

## Восстановление

| Метрика | Цель |
|---|---|
| RPO (конфиг + кэш) | 0 (конфиг в git; кэш — lossy) |
| RTO | ≤ 5 мин (рестарт контейнера) |

## Объём данных

| Метрика | Цель |
|---|---|
| Размер кэша статики на диске | ≤ 1 GB |

## Безопасность

- TLS-терминация на edge; сертификат self-signed (DNS-имя отсутствует, ACME не применяется).
- API-ответы НЕ кэшируются на edge.
- Заголовки безопасности и rate-limit — out of scope MVP, плановое расширение в горизонте 12 мес.

## Operability

| Метрика | Цель |
|---|---|
| Время применения изменения конфигурации (рестарт контейнера) | ≤ 30 с |
| Логи | stdout контейнера (`docker logs`) |

## Compliance

Не применяется (учебный проект, без PII-обработки на edge).

## Сценарии и use cases

- UC1: загрузка SPA/статики через edge → cache hit.
- UC2: REST API-запросы к BFF через edge → проксирование без кэширования.
- UC3: Swagger UI через edge → проксирование на YARP без кэширования.

## Out of scope (MVP)

- HTTPS через ACME / Let's Encrypt (нет DNS).
- Rate limiting, WAF, защита от bot/DDoS.
- HTTP/2 и HTTP/3 от proxy к upstream.
- Brotli/gzip compression.
- Hot-reload без рестарта.
- Кластеризация / HA, multi-region.
- Кэширование REST API-ответов.
- JWT-валидация и auth на edge (остаются на YARP).
