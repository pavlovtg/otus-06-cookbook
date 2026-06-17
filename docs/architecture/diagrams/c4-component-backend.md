# C4 Component — Backend (recipes-сервис)

Источник: ADR-0011, ADR-0012, ADR-0013, ADR-0014, ADR-0035, AR-0006, AR-0007, AR-0013

## Описание

Внутреннее устройство `recipes`-сервиса в гексагональной архитектуре. Domain (агрегаты, VO, доменные события) изолирован от инфраструктуры; Application определяет use-cases и порты. Auth-модуль — инфраструктурный cross-cutting слой: JWT-middleware, issuer и хранение пользователей. Не является доменным портом/адаптером.

## Диаграмма

```plantuml
@startuml
title C4 L3 — Components: recipes-service (Hexagonal)

skinparam rectangle {
  BackgroundColor<<adapter>> #85BBF0
  FontColor<<adapter>> black
  BackgroundColor<<app>> #438DD5
  FontColor<<app>> white
  BackgroundColor<<domain>> #08427B
  FontColor<<domain>> white
  BackgroundColor<<infra>> #6B6B6B
  FontColor<<infra>> white
  BackgroundColor<<ext>> #999999
  FontColor<<ext>> white
  BackgroundColor<<boundary>> transparent
}
skinparam database {
  BackgroundColor #438DD5
  FontColor white
}

rectangle "api-gateway (YARP)\n[Container]" as yarp <<ext>>
database "postgresql\n[PostgreSQL]" as pg

rectangle "recipes-service (.NET 10)" as svc <<boundary>> {

  rectangle "Infrastructure (cross-cutting)" as infra <<boundary>> {
    rectangle "JWT Middleware\nвалидация sig,\niss, aud, exp" as jwt <<infra>>
    rectangle "JWT Issuer\nвыдача токенов\n(login/register)" as issuer <<infra>>
    rectangle "Problem+JSON\nerror mapping" as err <<infra>>
  }

  rectangle "Inbound adapters" as inb <<boundary>> {
    rectangle "HTTP API\nControllers" as http <<adapter>>
  }

  rectangle "Application\n(use cases, ports)" as app <<boundary>> {
    rectangle "Use Cases\nCommands / Queries" as uc <<app>>
    rectangle "Ports\nIRepository,\nIUnitOfWork, ..." as ports <<app>>
  }

  rectangle "Domain" as dom <<boundary>> {
    rectangle "Aggregates,\nValue Objects,\nDomain Events" as model <<domain>>
  }

  rectangle "Outbound adapters" as out <<boundary>> {
    rectangle "EF Core Repository\nDbContext, миграции" as repo <<adapter>>
  }
}

yarp    --> jwt     : HTTPS\n[Bearer JWT]
jwt     --> http    : ClaimsPrincipal\nв HttpContext
http    --> uc      : вызов use case
uc      --> ports
uc      --> model
ports  <|.. repo    : реализует
repo    --> pg      : SQL (EF Core)
issuer  --> pg      : SELECT/INSERT users

http ..> err        : 4xx/5xx
@enduml
```
