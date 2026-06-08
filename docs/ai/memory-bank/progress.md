# Прогресс

## Выполнено

- Архитектурная документация: ADR-0001..ADR-0029, AR-0001..AR-0026, стандарты
- OpenAPI-спецификация: `docs/contracts/cookbook/recipes.yaml`
- Backend `recipes`: гексагональная архитектура, DDD, EF Core, PostgreSQL, тесты
- API Gateway: YARP, маршрутизация `/api/cookbook/...` → `recipes`, тесты
- Frontend `web`: Next.js 15, BFF, Zod, компоненты, тесты
- Инфраструктура: `docker-compose.yml` (сетевая изоляция), `nginx.conf`, Dockerfiles
- **minimal-realization: все 47/47 задач выполнены** (09.06.2026)
  - `docker compose up` — все 5 контейнеров healthy
  - Приложение доступно на `http://localhost:5500`

## Не начато

- Auth-service
- Tailwind CSS / shadcn/ui
- E2E тесты (Playwright)
