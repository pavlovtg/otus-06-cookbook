# Активный контекст

## Текущая задача

Пересмотр архитектурного решения по auth — замена выделенного auth-service на модуль внутри recipes-сервиса.

## Что сделано

- ADR-0021 (dedicated auth-service) и ADR-0022 (S2S client_credentials) — архивированы
- Создан ADR-0035: auth как модуль внутри recipes-сервиса
- AR-0012 (auth-service sole issuer) — удалён
- AR-0013 — переформулирован: JWT-валидация в recipes-сервисе; YARP — чистый прокси
- Диаграммы `c4-component-auth-service.md` и `flow-s2s-client-credentials.md` — удалены
- Обновлены: `c4-containers.md`, `flow-user-login.md`, `c4-component-backend.md`
- Обновлены: `service-registry.md`, `NFR-0001-auth.md`
- Обновлён `ARCHITECTURE.md`: таблицы AR, ADR, диаграмм

## Ключевые решения

- Auth-модуль — инфраструктурный cross-cutting слой внутри recipes-сервиса
- Пользователи хранятся в схеме `cookbook` той же PostgreSQL
- JWT выпускается и валидируется recipes-сервисом
- S2S не нужен (один доменный сервис в MVP)
