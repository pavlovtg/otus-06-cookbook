# Компоненты

## Модули

### API Gateway

- Единая точка доступа frontend и внешних клиентов к backend (см. [ADR-0008](../adr/rest-api/ADR-0008-api-gateway-single-entry-point.md)).
- Реализация — YARP на ASP.NET Core (см. [ADR-0010](../adr/rest-api/ADR-0010-yarp-as-api-gateway.md)).
- Публикует Swagger UI как каноничный источник OpenAPI-документации (см. [ADR-0009](../adr/rest-api/ADR-0009-swagger-ui-on-api-gateway.md), [AR-0002](rules/general/AR-0002-swagger-ui-live.md)).
- Cross-cutting задачи: маршрутизация, валидация JWT на входе, CORS, rate limiting, наблюдаемость.
- Не содержит бизнес-логики (см. [AR-0005](rules/rest-api/AR-0005-gateway-no-business-logic.md)).

### Backend

- Один или несколько REST API сервисов, реализующих бизнес-логику приложения (см. [ADR-0006](../adr/rest-api/ADR-0006-rest-api.md), [стандарт API Design](../standards/api-design.md)).
- Доступен клиентам только через API Gateway (см. [AR-0003](rules/rest-api/AR-0003-frontend-via-api-gateway.md), [AR-0004](rules/rest-api/AR-0004-backend-not-exposed.md)).
- JWT-аутентификация и авторизация (см. [ADR-0005](../adr/rest-api/ADR-0005-jwt-authentication.md)).
- Управление миграциями и загрузка seed-данных при старте.
- Покрытие тестами (см. [стандарт тестирования](../standards/testing.md)).

### Frontend

- Веб-приложение с адаптивной вёрсткой (desktop + mobile).
- Взаимодействует с Backend исключительно через API Gateway (см. [AR-0003](rules/rest-api/AR-0003-frontend-via-api-gateway.md)).
- Включает: список рецептов, карточку рецепта, планировщик меню, список покупок, дашборд.

### Database

- PostgreSQL — основная СУБД (см. [ADR-0004](../adr/general/ADR-0004-postgresql.md)).
- Seed-данные загружаются автоматически при первом старте: 25+ рецептов, 50+ ингредиентов, 2 пользователя, 20+ комментариев.

### Infrastructure

- Docker Compose объединяет API Gateway, Backend, Frontend и БД в единый стек (см. [ADR-0007](../adr/general/ADR-0007-docker-compose.md), [AR-0001](rules/general/AR-0001-docker-compose-self-contained.md), [стандарт Docker Compose](../standards/docker-compose-standard.md)).
- Наружу публикуется только порт API Gateway; backend-сервисы доступны исключительно во внутренней сети (см. [AR-0004](rules/rest-api/AR-0004-backend-not-exposed.md)).
- GitHub Actions CI: lint + тесты при каждом push и pull request (см. [стандарт CI](../standards/ci-standard.md)).

## Зависимости

_Диаграммы зависимостей хранятся в [diagrams/](diagrams/)._
