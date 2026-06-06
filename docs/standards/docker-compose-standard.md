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

- Все сервисы объединены в одну сеть Docker Compose (создаётся автоматически).
- Данные БД хранятся в именованнованных томах для сохранения между перезапусками.

## Связанные документы

- [ADR-0007: Docker Compose](../adr/general/ADR-0007-docker-compose.md)
- [AR-0001: Docker Compose Self-Contained](../architecture/rules/general/AR-0001-docker-compose-self-contained.md)
