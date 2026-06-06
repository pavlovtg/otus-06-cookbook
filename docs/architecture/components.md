# Компоненты

## Модули

### Backend

- REST API сервер, реализующий бизнес-логику приложения (см. [ADR-0006](../adr/general/ADR-0006-rest-api.md), [стандарт API Design](../standards/api-design.md)).
- OpenAPI/Swagger документация, доступная по живому URL (см. [AR-0002](rules/general/AR-0002-swagger-ui-live.md)).
- JWT-аутентификация и авторизация (см. [ADR-0005](../adr/general/ADR-0005-jwt-authentication.md)).
- Управление миграциями и загрузка seed-данных при старте.
- Покрытие тестами (см. [стандарт тестирования](../standards/testing.md)).

### Frontend

- Веб-приложение с адаптивной вёрсткой (desktop + mobile).
- Взаимодействует с Backend через REST API.
- Включает: список рецептов, карточку рецепта, планировщик меню, список покупок, дашборд.

### Database

- PostgreSQL — основная СУБД (см. [ADR-0004](../adr/general/ADR-0004-postgresql.md)).
- Seed-данные загружаются автоматически при первом старте: 25+ рецептов, 50+ ингредиентов, 2 пользователя, 20+ комментариев.

### Infrastructure

- Docker Compose объединяет Backend, Frontend и БД в единый стек (см. [ADR-0007](../adr/general/ADR-0007-docker-compose.md), [AR-0001](rules/general/AR-0001-docker-compose-self-contained.md), [стандарт Docker Compose](../standards/docker-compose-standard.md)).
- GitHub Actions CI: lint + тесты при каждом push и pull request (см. [стандарт CI](../standards/ci-standard.md)).

## Зависимости

_Диаграммы зависимостей хранятся в [diagrams/](diagrams/)._
