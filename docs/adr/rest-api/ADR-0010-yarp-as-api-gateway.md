# ADR-0010: YARP как реализация API Gateway

- **Статус**: принят
- **Домен**: rest-api
- **Дата**: 2026-06-07

## Контекст

ADR-0008 вводит API Gateway как единую точку доступа, ADR-0009 размещает на нём Swagger UI. Требуется выбрать конкретную технологию реализации gateway со следующими ограничениями:

- backend-стек — .NET, несколько микросервисов;
- запуск через `docker compose`, self-contained, без облачных и managed-зависимостей;
- статическая конфигурация маршрутов в файле, версионируется в репозитории;
- учебная нагрузка (порядок 1–10 RPS), SLA не требуется;
- желательны валидация JWT на входе, rate limiting, CORS, access-логи, Prometheus и OpenTelemetry;
- между gateway и backend — доверенная зона (без mTLS);
- поддержка размещения Swagger UI на gateway (см. ADR-0009).

## Рассмотренные варианты

- **YARP (Yet Another Reverse Proxy)** — библиотека Microsoft на ASP.NET Core. Нативный для .NET-стека; маршруты и кластеры в `appsettings`-файле; cross-cutting middleware (JWT, CORS, rate limiting) — стандартные ASP.NET; интеграция с OpenTelemetry и Prometheus через стандартные пакеты; Swagger UI размещается штатными средствами Swashbuckle в том же host-приложении. Минус — требует минимальный host-проект, не «готовый бинарник».
- **Traefik** — готовый контейнер; маршрутизация, TLS, rate limit, метрики, tracing из коробки; не входит в .NET-экосистему; валидация JWT — только через ForwardAuth (внешний auth-сервис); размещение Swagger UI на gateway требует отдельного контейнера/костылей.
- **Ocelot** — .NET-шлюз с декларативным JSON-конфигом; JWT, rate limit, CORS из коробки; темп развития замедлен, Microsoft продвигает YARP как направление; риск устаревания и проблем совместимости с актуальными версиями .NET.

## Решение

В качестве реализации API Gateway выбран **YARP**.

- YARP-host — отдельное .NET-приложение в монорепо, запускается как сервис в `docker compose`.
- Маршруты и кластеры описываются в `appsettings.json` (или эквивалентном файле), версионируются в репозитории.
- Cross-cutting задачи (CORS, валидация JWT на входе, rate limiting, корреляция, наблюдаемость) реализуются стандартным ASP.NET-middleware.
- Swagger UI размещается в том же host-приложении штатными средствами .NET; способ формирования содержимого определяется отдельным решением (см. ADR-0009).

## Последствия

- В монорепо появляется новый .NET-проект — API Gateway на YARP.
- Backend-сервисы и gateway используют единый стек .NET, что упрощает разработку и общие пакеты (логирование, OpenTelemetry, JWT).
- Появляется зависимость от пакетов `Yarp.ReverseProxy`, `Microsoft.AspNetCore.Authentication.JwtBearer` и пакетов наблюдаемости.
- При необходимости миграции на другую технологию gateway потребуется новый ADR, заменяющий настоящий.

## Связанные документы

- [ADR-0008: API Gateway как единая точка доступа](ADR-0008-api-gateway-single-entry-point.md).
- [ADR-0009: Swagger UI публикуется на API Gateway](ADR-0009-swagger-ui-on-api-gateway.md).
