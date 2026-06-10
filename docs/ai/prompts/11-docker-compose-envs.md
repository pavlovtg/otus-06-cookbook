# Docker Compose: вынос конфигурации в env-переменные

## Роль

- Senior DevOps / Platform Engineer, знающий Docker Compose, best practices конфигурирования контейнеров.

## Контекст

- Монорепо `otus-06-cookbook`, файл `docker-compose.yml` в корне.
- 5 сервисов: `reverse-proxy` (nginx), `web` (Next.js), `api-gateway` (.NET), `recipes` (.NET), `postgresql`.
- Текущий `docker-compose.yml`:

```yaml
name: cookbook

services:
  reverse-proxy:
    container_name: cookbook-reverse-proxy
    image: nginx:1.27-alpine
    ports:
      - "5500:80"
    ...

  web:
    container_name: cookbook-web
    environment:
      - GATEWAY_URL=http://api-gateway:8080
    ...

  api-gateway:
    container_name: cookbook-api-gateway
    environment:
      - ASPNETCORE_URLS=http://+:8080
      - ReverseProxy__Clusters__recipes-cluster__Destinations__recipes__Address=http://recipes:8080/
    ...

  recipes:
    container_name: cookbook-recipes
    environment:
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__Recipes=Host=postgresql;Database=recipes;Username=cookbook;Password=cookbook
    ...

  postgresql:
    container_name: cookbook-postgresql
    image: postgres:16-alpine
    environment:
      - POSTGRES_DB=recipes
      - POSTGRES_USER=cookbook
      - POSTGRES_PASSWORD=cookbook
    ports: []  # нет внешнего порта
    ...
```

- Текущий `infrastructure/docker-compose/reverse-proxy/nginx.conf`:

```nginx
events {
    worker_connections 1024;
}

http {
    server {
        listen 80;

        location /api/ {
            proxy_pass http://api-gateway:8080;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }

        location / {
            proxy_pass http://web:3000;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }
    }
}
```

- Стандарт оформления: `docs/standards/docker-compose-standard.md`.
- Архитектурные правила: `docs/architecture/rules/general/`.

## Задача

1. Переписать `docker-compose.yml`: все конфигурируемые параметры заменить на подстановку env-переменных.
2. Создать `.env` в корне репозитория со всеми переменными и их значениями по умолчанию.
3. Параметризовать nginx-конфиг через механизм nginx templates.
4. Обновить `docs/standards/docker-compose-standard.md` — добавить раздел с требованиями к вынесению конфигурации в env.
5. Создать архитектурное правило `docs/architecture/rules/general/AR-XXXX-docker-compose-env-config.md`.
6. Обновить `ARCHITECTURE.md` — добавить запись о новом AR.

## Требования

### Формат подстановки в docker-compose.yml

```yaml
name: ${COMPOSE_NAME:?NO_COMPOSE_NAME}
```

- Сообщение об ошибке: `NO_<VAR_NAME>` (например, `NO_COMPOSE_NAME`).
- Обязательный формат для всех переменных: `${VAR_NAME:?NO_VAR_NAME}`.

### Что выносить в env

`container_name` каждого сервиса собирается из `COMPOSE_NAME` как префикс.

`ASPNETCORE_URLS` для всех .NET-сервисов задаётся единой переменной `ASPNETCORE_URLS`, собранной из `DEFAULT_HTTP_PORT`.

| Параметр | Переменная | Значение по умолчанию |
|---|---|---|
| `name` (stack) | `COMPOSE_NAME` | `cookbook` |
| `container_name` reverse-proxy | `REVERSE_PROXY_CONTAINER_NAME` | `${COMPOSE_NAME}-reverse-proxy` |
| `container_name` web | `WEB_CONTAINER_NAME` | `${COMPOSE_NAME}-web` |
| `container_name` api-gateway | `API_GATEWAY_CONTAINER_NAME` | `${COMPOSE_NAME}-api-gateway` |
| `container_name` recipes | `RECIPES_CONTAINER_NAME` | `${COMPOSE_NAME}-recipes` |
| `container_name` postgresql | `POSTGRESQL_CONTAINER_NAME` | `${COMPOSE_NAME}-postgresql` |
| Внешний порт reverse-proxy | `REVERSE_PROXY_PORT` | `5500` |
| Внутренний HTTP-порт .NET-сервисов | `DEFAULT_HTTP_PORT` | `8080` |
| `ASPNETCORE_URLS` (все .NET-сервисы) | `ASPNETCORE_URLS` | `http://+:${DEFAULT_HTTP_PORT}` |
| `GATEWAY_URL` | `GATEWAY_URL` | `http://api-gateway:${DEFAULT_HTTP_PORT}` |
| `POSTGRES_DB` | `POSTGRES_DB` | `recipes` |
| `POSTGRES_USER` | `POSTGRES_USER` | `cookbook` |
| `POSTGRES_PASSWORD` | `POSTGRES_PASSWORD` | `cookbook` |
| hostname api-gateway | `API_GATEWAY_HOST` | `api-gateway` |
| hostname recipes | `RECIPES_HOST` | `recipes` |
| hostname postgresql | `POSTGRESQL_HOST` | `postgresql` |
| hostname web | `WEB_HOST` | `web` |
| внутренний порт web | `WEB_PORT` | `3000` |

### Структура `.env`

```dotenv
# Stack
COMPOSE_NAME=cookbook

# Container names (собираются из COMPOSE_NAME)
REVERSE_PROXY_CONTAINER_NAME=${COMPOSE_NAME}-reverse-proxy
WEB_CONTAINER_NAME=${COMPOSE_NAME}-web
API_GATEWAY_CONTAINER_NAME=${COMPOSE_NAME}-api-gateway
RECIPES_CONTAINER_NAME=${COMPOSE_NAME}-recipes
POSTGRESQL_CONTAINER_NAME=${COMPOSE_NAME}-postgresql

# Ports
REVERSE_PROXY_PORT=5500
DEFAULT_HTTP_PORT=8080
WEB_PORT=3000

# Hostnames (internal Docker network)
API_GATEWAY_HOST=api-gateway
RECIPES_HOST=recipes
POSTGRESQL_HOST=postgresql
WEB_HOST=web

# Application config
ASPNETCORE_URLS=http://+:${DEFAULT_HTTP_PORT}
GATEWAY_URL=http://${API_GATEWAY_HOST}:${DEFAULT_HTTP_PORT}

# Database
POSTGRES_DB=recipes
POSTGRES_USER=cookbook
POSTGRES_PASSWORD=cookbook
```

### Параметризация nginx через templates

Использовать механизм nginx templates: файл монтируется в `/etc/nginx/templates/` с расширением `.template` — nginx-образ автоматически прогоняет `envsubst` при старте и кладёт результат в `/etc/nginx/conf.d/`.

- Переименовать `nginx.conf` → `nginx.conf.template`.
- Обновить volume в `docker-compose.yml`:

```yaml
volumes:
  - ./infrastructure/docker-compose/reverse-proxy/nginx.conf.template:/etc/nginx/templates/default.conf.template
```

- В шаблоне использовать переменные `${API_GATEWAY_HOST}`, `${DEFAULT_HTTP_PORT}`, `${WEB_HOST}`, `${WEB_PORT}`.
- Nginx-переменные (`$host`, `$remote_addr`, `$proxy_add_x_forwarded_for`) экранировать как `${dollar}host` или использовать `NGINX_ENVSUBST_TEMPLATE_DIR` с явным списком переменных для подстановки через переменную окружения `NGINX_ENVSUBST_OUTPUT_DIR`.
- Передать нужные переменные в сервис `reverse-proxy` через `environment` в `docker-compose.yml`.

Пример шаблона `nginx.conf.template`:

```nginx
events {
    worker_connections 1024;
}

http {
    server {
        listen 80;

        location /api/ {
            proxy_pass http://${API_GATEWAY_HOST}:${DEFAULT_HTTP_PORT};
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }

        location / {
            proxy_pass http://${WEB_HOST}:${WEB_PORT};
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        }
    }
}
```

> Важно: `$host`, `$remote_addr`, `$proxy_add_x_forwarded_for` — это nginx-переменные, не env. Чтобы `envsubst` их не трогал, передавать в `environment` сервиса только нужные переменные и использовать флаг `NGINX_ENVSUBST_TEMPLATE_DIR` или явно указывать список переменных для подстановки.

### Требования к стандарту (`docker-compose-standard.md`)

Добавить раздел:

- Все конфигурируемые параметры (имена, порты, хосты, credentials) выносятся в `.env`.
- Формат подстановки: `${VAR_NAME:?NO_VAR_NAME}` — обязателен для всех переменных в `docker-compose.yml`.
- `container_name` каждого сервиса собирается из `COMPOSE_NAME` как префикс: `${COMPOSE_NAME}-<service>`.
- `ASPNETCORE_URLS` для всех .NET-сервисов задаётся единой переменной через `DEFAULT_HTTP_PORT`.
- nginx-конфиг параметризуется через механизм nginx templates (`/etc/nginx/templates/*.template`).
- `.env` содержит значения по умолчанию для локальной разработки и коммитится в репозиторий.

### Требования к AR

- Домен: `general`.
- Правило: все конфигурируемые параметры docker-compose выносятся в env-переменные с обязательным форматом `${VAR:?ERR}`; `container_name` собирается из `COMPOSE_NAME`; `ASPNETCORE_URLS` задаётся единой переменной через `DEFAULT_HTTP_PORT`; nginx-конфиг параметризуется через nginx templates.
- Запрещено: хардкодить имена контейнеров, порты, хосты, credentials непосредственно в `docker-compose.yml` или `nginx.conf`.
- Источник: `ADR-0007`.
