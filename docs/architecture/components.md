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

### Frontend (web)

- Single-service Next.js приложение (App Router, React Server Components) — UI + BFF в одном Node-процессе (см. [ADR-0015](../adr/frontend/ADR-0015-nextjs-frontend-meta-framework.md), [AR-0015](rules/frontend/AR-0015-ui-and-bff-single-process.md)).
- React 18+ и TypeScript `strict: true` (см. [ADR-0016](../adr/frontend/ADR-0016-react-typescript-frontend.md), [AR-0011](rules/frontend/AR-0011-frontend-typescript-nodejs.md)).
- SSR публичных страниц (рецепт, поиск) для SEO; SPA-навигация после первой загрузки.
- Адаптивная вёрстка (desktop + mobile); Tailwind CSS + shadcn/ui (см. [ADR-0018](../adr/frontend/ADR-0018-tailwind-shadcn.md)).
- Формы и валидация через Zod + react-hook-form (см. [ADR-0019](../adr/frontend/ADR-0019-zod-schema-validation.md)).
- Включает: список рецептов, карточку рецепта, drag-and-drop планировщик меню, список покупок, дашборд.

#### BFF (логический слой внутри Frontend)

- Не отдельный процесс, а набор серверных модулей в `apps/web/lib/bff/`, Route Handlers и Server Actions (см. [ADR-0017](../adr/frontend/ADR-0017-bff-logical-layer.md)).
- Единственный путь браузера к backend — через BFF (см. [AR-0003](rules/rest-api/AR-0003-frontend-via-api-gateway.md)).
- BFF получает JWT от backend через API Gateway и хранит его на server-side; в браузер уходит только httpOnly + Secure + SameSite signed encrypted cookie (см. [AR-0013](rules/frontend/AR-0013-bff-stateless.md), [AR-0014](rules/frontend/AR-0014-jwt-not-leak-to-browser.md)).
- Агрегирует ответы backend под нужды UI (минимум — страница рецепта одним запросом).
- Не содержит бизнес-логики (см. [AR-0012](rules/frontend/AR-0012-bff-no-business-logic.md)).

### Database

- PostgreSQL — основная СУБД (см. [ADR-0004](../adr/general/ADR-0004-postgresql.md)).
- Seed-данные загружаются автоматически при первом старте: 25+ рецептов, 50+ ингредиентов, 2 пользователя, 20+ комментариев.

### Infrastructure

- Docker Compose объединяет API Gateway, Backend, Frontend и БД в единый стек (см. [ADR-0007](../adr/general/ADR-0007-docker-compose.md), [AR-0001](rules/general/AR-0001-docker-compose-self-contained.md), [стандарт Docker Compose](../standards/docker-compose-standard.md)).
- Наружу публикуется только порт API Gateway; backend-сервисы доступны исключительно во внутренней сети (см. [AR-0004](rules/rest-api/AR-0004-backend-not-exposed.md)).
- GitHub Actions CI: lint + тесты при каждом push и pull request (см. [стандарт CI](../standards/ci-standard.md)).

## Зависимости

_Диаграммы зависимостей хранятся в [diagrams/](diagrams/)._
