# Активный контекст

## Текущая задача

Исправление health check пути у web-сервиса.

## Что сделано в этой задаче

- `apps/web/app/api/health/v1/route.ts` → перемещён в `apps/web/app/api/v1/health/route.ts`
- Путь `/api/v1/health` теперь соответствует ADR-0029 и `WEB_HEALTH_URL` в `.env`

## Следующий шаг

Нет активных задач.
