# C4 Component — Backend (доменный сервис)

Источник: ADR-0011, ADR-0012, ADR-0013, ADR-0014, AR-0006, AR-0007, AR-0013

## Описание

Внутреннее устройство типового доменного backend-сервиса (например, `recipe-service`) в гексагональной архитектуре. Domain (агрегаты, VO, доменные события) изолирован от инфраструктуры; Application определяет use-cases и порты к домен-нужной инфраструктуре (репозитории и т.п.). Аутентификация/авторизация — инфраструктурный cross-cutting слой: JWT-middleware и S2S-клиент к auth-service в нём не являются доменными портами/адаптерами.

## Диаграмма

```plantuml
@startuml
title C4 L3 — Components: recipe-service (Hexagonal)

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
rectangle "auth-service\n[Container]" as auth <<ext>>
database "recipe-postgres\n[PostgreSQL]" as pg

rectangle "recipe-service (.NET 10)" as svc <<boundary>> {

  rectangle "Infrastructure (cross-cutting)" as infra <<boundary>> {
    rectangle "JWT Middleware\nвалидация sig,\niss, aud, exp" as jwt <<infra>>
    rectangle "Auth S2S client\nполучение JWT по\nclient_credentials" as s2s <<infra>>
    rectangle "Problem+JSON\nerror mapping" as err <<infra>>
  }

  rectangle "Inbound adapters" as inb <<boundary>> {
    rectangle "HTTP API\nMinimal API /\nControllers" as http <<adapter>>
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

yarp --> jwt        : HTTPS\n[Bearer JWT]
jwt  --> http       : ClaimsPrincipal\nв HttpContext
http --> uc         : вызов use case
uc   --> ports
uc   --> model
ports <|.. repo     : реализует
repo --> pg         : SQL (EF Core)

http ..> err        : 4xx/5xx
s2s  --> auth       : HTTP /auth/token\n(используется,\nкогда сервис сам\nвызывает другие)
@enduml
```
