# Стандарт: Docker Compose

## Запуск

- Все необходимые переменные окружения задаются в `docker-compose.yml` или в `.env`-файле, включённом в репозиторий.

## Именование

- Имя стека задаётся через `COMPOSE_PROJECT_NAME` или поле `name` в `docker-compose.yml`.
- Имена сервисов — kebab-case: `backend`, `frontend`, `db`.
- Имена контейнеров задаются явно.

## Зависимости сервисов

- Порядок запуска задаётся через `depends_on` с условием `service_healthy` или `service_started`.
- Backend не стартует до готовности БД (`depends_on: db`).
- Frontend не стартует до готовности Backend (`depends_on: backend`), если требует API при сборке.

## Сети и тома

- Сервисы разбиваются на сети по принципу безопасности и изоляции (AR-0026).
- Данные БД хранятся в именованных томах для сохранения между перезапусками.

## Конфигурационные файлы сервисов

- Конфиги, монтируемые в контейнеры через `volumes`, хранятся в `infrastructure/docker-compose/<service>/`.
- Пример: `infrastructure/docker-compose/reverse-proxy/nginx.conf.template`.
- Исключение: `docker-compose.yml` остаётся в корне репозитория.

## Конфигурация через env-переменные

- Все конфигурируемые параметры (имена контейнеров, порты, хосты, credentials) выносятся в `.env`.
- Формат подстановки в `docker-compose.yml` обязателен для всех переменных: `${VAR_NAME:?NO_VAR_NAME}`.
- `container_name` каждого сервиса собирается из `COMPOSE_NAME` как префикс: `${COMPOSE_NAME}-<service>`.
- `ASPNETCORE_URLS` для всех .NET-сервисов задаётся единой переменной через `DEFAULT_HTTP_PORT`.
- Секция `environment` использует map-синтаксис (`KEY: VALUE`); list-синтаксис (`- KEY=VALUE`) запрещён.
