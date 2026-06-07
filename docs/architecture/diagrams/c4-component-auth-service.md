# C4 Component — auth-service

Источник: ADR-0021, ADR-0022, AR-0012

## Описание

Внутреннее устройство `auth-service` — единственного issuer JWT. Содержит два публичных эндпоинта: `/auth/login` (пользовательский логин по password) и `/auth/token` (OAuth 2.0 `client_credentials` для S2S). Доменный bounded context `Identity` (Users, Clients, Credentials) живёт за гексагональными портами; подпись JWT — инфраструктурная задача (HS256, секрет из конфигурации).

## Диаграмма

```plantuml
@startuml
title C4 L3 — Components: auth-service

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
database "auth-postgres\n[PostgreSQL]\nсхема identity" as pg

rectangle "auth-service (.NET 10)" as svc <<boundary>> {

  rectangle "Inbound adapters" as inb <<boundary>> {
    rectangle "POST /auth/login\nUser login\n(password)" as login <<adapter>>
    rectangle "POST /auth/token\nOAuth 2.0\nclient_credentials" as token <<adapter>>
  }

  rectangle "Application" as app <<boundary>> {
    rectangle "Use Cases\nIssueUserToken,\nIssueServiceToken" as uc <<app>>
    rectangle "Ports\nIUserRepository,\nIClientRepository,\nIPasswordHasher,\nITokenIssuer" as ports <<app>>
  }

  rectangle "Domain (Identity)" as dom <<boundary>> {
    rectangle "User, Credentials\n(Argon2id / BCrypt)" as user <<domain>>
    rectangle "Client (S2S)\nclient_id, secret hash,\nallowed scopes" as client <<domain>>
  }

  rectangle "Infrastructure" as infra <<boundary>> {
    rectangle "EF Core Repository" as repo <<adapter>>
    rectangle "JWT Issuer\nHS256, secret из env\n(iss, aud, exp, sub,\nscope)" as issuer <<infra>>
    rectangle "Password Hasher\nArgon2id / BCrypt" as hasher <<infra>>
    rectangle "Problem+JSON\nerror mapping" as err <<infra>>
  }
}

yarp --> login     : HTTPS\n(username/password)
yarp --> token     : HTTPS\n(client_id/secret,\nscope)
login --> uc
token --> uc
uc --> ports
uc --> user
uc --> client
ports <|.. repo    : реализует
ports <|.. issuer  : реализует ITokenIssuer
ports <|.. hasher  : реализует IPasswordHasher
repo --> pg        : SQL (EF Core)
login ..> err
token ..> err
@enduml
```
