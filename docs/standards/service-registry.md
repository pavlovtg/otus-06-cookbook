# Стандарт: Реестр сервисов и bounded contexts

Источник: ADR-0026

## Правила

Все имена сервисов и bounded contexts — lowercase kebab-case (ADR-0026). Реестр ниже является единственным источником истины для имён сервисов в проекте.

## Реестр

| Сервис | Bounded Context | Роль |
|--------|-----------------|------|
| `reverse-proxy` | — | Edge reverse proxy (nginx) |
| `api-gateway` | `api-gateway` | API Gateway (YARP) |
| `web` | — | Frontend + BFF (Next.js) |
| `postgresql` | — | СУБД (PostgreSQL) |
| `recipes` | `cookbook` | Сервис рецептов (включает auth-модуль) |

## Именование БД и схем

Согласно ADR-0025, БД именуется по имени сервиса (lowercase snake_case), схема — по bounded context (lowercase snake_case):

| Сервис | БД | Схема |
|--------|----|-------|
| `recipes` | `recipes` | `cookbook` |
