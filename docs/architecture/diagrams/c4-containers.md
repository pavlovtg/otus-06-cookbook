# C4 Containers — Cookbook

Источник: ADR-0007, ADR-0008, ADR-0010, ADR-0015, ADR-0017, ADR-0020, ADR-0035

## Описание

Контейнерная диаграмма стека, разворачиваемого через Docker Compose. Наружу опубликован только nginx; за ним — Next.js (UI + BFF в одном процессе) и YARP (API Gateway). YARP является чистым прокси и маршрутизирует к `recipes`-сервису. `recipes`-сервис содержит модуль аутентификации и выпускает JWT. Одна PostgreSQL для всех данных.

## Диаграмма

```plantuml
@startuml
title C4 L2 — Containers: Cookbook

skinparam rectangle {
  BackgroundColor<<container>> #438DD5
  FontColor<<container>> white
  BackgroundColor<<person>> #08427B
  FontColor<<person>> white
  BackgroundColor<<boundary>> transparent
}
skinparam database {
  BackgroundColor #438DD5
  FontColor white
}

rectangle "Конечный пользователь" as user <<person>>

rectangle "Cookbook (Docker Compose)" as sys <<boundary>> {
  rectangle "nginx\n[Container: nginx]" as nginx <<container>>
  rectangle "web\n[Container: Next.js / Node.js]" as web <<container>>
  rectangle "api-gateway\n[Container: YARP / ASP.NET Core]" as yarp <<container>>

  rectangle "recipes\n[Container: .NET 10 / C#]\n(включает auth-модуль)" as recipe <<container>>
  database "postgresql\n[PostgreSQL]" as pg
}

user   --> nginx  : HTTPS
nginx  --> web    : HTTP (UI, /api/*)
nginx  --> yarp   : HTTP (/api прокси,\nSwagger UI)
web    --> yarp   : Server-side HTTP\n[Bearer JWT]
yarp   --> recipe : HTTP /api/*\n[Bearer JWT]
recipe --> pg     : SQL (EF Core)
@enduml
```
