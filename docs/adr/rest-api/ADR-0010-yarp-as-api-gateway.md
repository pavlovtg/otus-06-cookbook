# ADR-0010: YARP как реализация API Gateway

- **Статус**: принят
- **Дата**: 2026-06-07

## Контекст

ADR-0008 вводит API Gateway, ADR-0009 размещает на нём Swagger UI. Ограничения: backend-стек .NET, запуск через `docker compose` без облачных зависимостей, статическая конфигурация маршрутов в файле, учебная нагрузка (1–10 RPS), желательны JWT, rate limiting, CORS, OpenTelemetry, Prometheus, размещение Swagger UI на gateway. Альтернативы: Traefik (не входит в .NET-экосистему; JWT — только через ForwardAuth; Swagger UI на gateway требует костылей), Ocelot (темп развития замедлен, Microsoft продвигает YARP; риск устаревания) — отклонены.

## Решение

API Gateway — **YARP** (Yet Another Reverse Proxy) на ASP.NET Core.

- YARP-host — отдельное .NET-приложение в монорепо, запускается как сервис в `docker compose`.
- Маршруты и кластеры — в `appsettings.json`, версионируются в репозитории.
- Cross-cutting задачи (CORS, валидация JWT, rate limiting, корреляция, наблюдаемость) — стандартный ASP.NET-middleware.
- Swagger UI — в том же host-приложении штатными средствами .NET.

## Последствия

- В монорепо появляется новый .NET-проект — API Gateway на YARP.
- Backend-сервисы и gateway используют единый стек .NET.
- Появляется зависимость от пакетов `Yarp.ReverseProxy`, `Microsoft.AspNetCore.Authentication.JwtBearer` и пакетов наблюдаемости.
- Миграция на другую технологию gateway требует нового ADR, заменяющего настоящий.
