# AR-0016: Соглашение о маршрутизации на API Gateway

Источник: ADR-0024

## Правило

Маршруты на API Gateway MUST строиться по шаблону `{gateway}/api/{bounded-context}/... → {service}/api/...`. Gateway стрипает префикс `/{bounded-context}` перед проксированием к сервису.

## Запрещено

- Публиковать на gateway маршруты, не содержащие bounded-context-префикса.
- Включать bounded-context-префикс в маршруты самого сервиса (сервис не знает о своём месте в gateway).
