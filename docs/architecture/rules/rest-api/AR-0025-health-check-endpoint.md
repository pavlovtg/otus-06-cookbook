# AR-0025: Каждый микросервис реализует `GET /api/v1/health`

Источник: ADR-0029

## Правило

Каждый микросервис обязан реализовать эндпоинт `GET /api/v1/health`, возвращающий `200 OK` при готовности сервиса.

Путь соответствует REST URI-шаблону проекта: `/api/v{major_version}/{resource}`.

Эндпоинт используется в `docker-compose.yml` как `healthcheck.test` для `depends_on: condition: service_healthy`.

## Запрещено

- Произвольные пути (`/healthz`, `/health`, `/ping`, `/status`).
- Отсутствие health-check эндпоинта в сервисе, участвующем в Docker Compose.
