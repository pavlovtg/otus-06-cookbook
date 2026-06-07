# Flow — S2S через OAuth 2.0 client_credentials

Источник: ADR-0022, AR-0012, AR-0013

## Описание

Sequence-диаграмма межсервисного вызова. S2S-трафик идёт по внутренней сети Docker Compose **напрямую**, в обход YARP (API Gateway — точка входа для внешних клиентов, а не для backend↔backend). Сервис-инициатор получает JWT у auth-service по grant `client_credentials`, кэширует его до истечения `exp`, и зовёт downstream-сервис напрямую с `Authorization: Bearer`. Downstream-сервис валидирует JWT тем же middleware, что и пользовательский (общий issuer и формат токена).

## Диаграмма

```plantuml
@startuml
title Flow — S2S: client_credentials + downstream call

skinparam sequenceMessageAlign center
skinparam shadowing false

participant "service-A\n(initiator)" as A
participant "auth-service" as AUTH
database "auth-postgres" as APG
participant "service-B\n(downstream)" as B

== 1. Получение S2S JWT ==
A -> A: проверить cache:\nесть ли валидный JWT?
alt JWT отсутствует или истёк
  A -> AUTH: POST /auth/token\ngrant=client_credentials\nclient_id, client_secret,\nscope=recipes.read
  AUTH -> APG: SELECT client by id
  APG --> AUTH: client + secret hash + scopes
  AUTH -> AUTH: verify secret\ncheck scope allowed
  AUTH -> AUTH: issue JWT (HS256,\niss, aud, exp,\nsub=client_id, scope)
  AUTH --> A: 200 { access_token, expires_in }
  A -> A: cache JWT\n(до exp − skew)
end

== 2. Вызов downstream ==
A -> B: GET /api/recipes\nAuthorization: Bearer <JWT>
B -> B: JWT middleware:\nvalidate sig, iss, aud, exp\ncheck scope
B --> A: 200 { ... }
@enduml
```
