# AR-0011: Edge reverse proxy обязателен

Источник: ADR-0020

## Правило

- Весь внешний HTTP/HTTPS-трафик к приложению проходит через edge reverse proxy (Nginx).
- В `docker-compose.yml` наружу публикуются ТОЛЬКО порты edge-сервиса (nginx); Next.js и API Gateway (YARP) запускаются без `ports:` и доступны исключительно во внутренней сети Docker.
- Маршрутизация на edge: `/api/*` и `/swagger` → API Gateway; остальные пути → Next.js.
- Кэширование статики выполняется на edge (`proxy_cache`); API-ответы не кэшируются.
- Access-логи edge пишутся в stdout контейнера.
- TLS-терминация выполняется на edge.

## Запрещено

- `ports:` у Next.js, API Gateway или backend-сервисов в `docker-compose.yml` с пробросом на хост.
- Прямое обращение клиентов к Next.js или YARP в обход edge.
- Кэширование REST API-ответов на edge.
- Размещение бизнес-логики, авторизации (валидации JWT) и аутентификации в конфигурации edge — эти задачи остаются на API Gateway.
