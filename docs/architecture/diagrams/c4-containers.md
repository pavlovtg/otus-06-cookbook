# C4 Containers — Cookbook

Источник: ADR-0007, ADR-0008, ADR-0010, ADR-0015, ADR-0017, ADR-0020, ADR-0021

## Описание

Контейнерная диаграмма стека, разворачиваемого через Docker Compose. Наружу опубликован только nginx; за ним — Next.js (UI + BFF в одном процессе) и YARP (API Gateway). YARP маршрутизирует к auth-service и доменным backend-сервисам. У каждого сервиса своя БД PostgreSQL; если в одном сервисе несколько bounded contexts — они разделяются схемами. Единственный issuer JWT — auth-service.

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

  rectangle "auth-service\n[Container: .NET 10 / C#]" as auth <<container>>
  database "auth-postgres\n[PostgreSQL]" as authPg

  rectangle "recipe-service\n[Container: .NET 10 / C#]" as recipe <<container>>
  database "recipe-postgres\n[PostgreSQL]" as recipePg
}

user  --> nginx  : HTTPS
nginx --> web    : HTTP (UI, /api/*)
nginx --> yarp   : HTTP (/api прокси,\nSwagger UI)
web   --> yarp   : Server-side HTTP\n[Bearer JWT]
yarp  --> auth   : HTTP /auth/*
yarp  --> recipe : HTTP /api/*\n[Bearer JWT]
recipe --> auth  : S2S: client_credentials\n(получение JWT)
auth   --> authPg   : SQL (EF Core)
recipe --> recipePg : SQL (EF Core)
@enduml
```
