# Активный контекст

## Текущая задача

**minimal-realization — ЗАВЕРШЕНА** (09.06.2026)

Все 47 задач выполнены. Стек запускается и работает.

## Предыдущая задача

Реализация change `minimal-realization` — MVP кулинарной книги.

## Статус

Все задачи выполнены, кроме 6.1 (проверка `docker compose up` в браузере — ручная).

## Что сделано в этой сессии

- ADR-0027: WireMock.Net для HTTP-мокирования в .NET тестах
- ADR-0028: Сетевая изоляция в Docker Compose (`frontend-net`/`backend-net`)
- ADR-0029: Health-check API обязателен для каждого микросервиса (`GET /api/health/v1`)
- AR-0024: WireMock.Net для HTTP-мокирования
- AR-0025: Health-check эндпоинт `GET /api/health/v1`
- AR-0026: Сетевая изоляция Docker Compose
- Обновлён `docker-compose-standard.md`
- Backend `recipes`: домен, EF Core, контроллер, тесты (Testcontainers + WireMock)
- API Gateway: YARP маршрутизация, тесты (WireMock)
- Frontend `web`: Next.js 15, BFF, Zod-схемы, компоненты, тесты (Vitest)
- Инфраструктура: `docker-compose.yml`, `nginx.conf`, Dockerfiles, health-checks
- README.md обновлён

## Следующий шаг

Проверка `docker compose up` (задача 6.1) — ручная.
