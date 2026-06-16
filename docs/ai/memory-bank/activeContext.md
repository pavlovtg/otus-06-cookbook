# Активный контекст

## Текущая задача

Исправление ошибки nginx `host not found in upstream "api-gateway"` при `docker compose up --build`.

## Что сделано

### Fix: nginx lazy DNS resolution

- Причина: nginx резолвит upstream-хосты статически при старте, до того как Docker DNS регистрирует контейнеры.
- Фикс в `infrastructure/docker-compose/reverse-proxy/nginx.conf.template`:
  - Добавлен `resolver 127.0.0.11 valid=10s ipv6=off;` (Docker встроенный DNS).
  - Все `proxy_pass ${VAR}` заменены на `set $var ${VAR}; proxy_pass $var;` — nginx резолвит DNS лениво, при каждом запросе.
