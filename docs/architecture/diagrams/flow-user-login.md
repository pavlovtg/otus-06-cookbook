# Flow — User login + защищённый запрос

Источник: ADR-0005, ADR-0017, ADR-0021, AR-0010, AR-0012, AR-0013

## Описание

Sequence-диаграмма пользовательского логина и последующего вызова защищённого ресурса. Браузер отправляет credentials в BFF (Next.js Route Handler); BFF проксирует на YARP → auth-service, получает JWT, кладёт его в server-side session и возвращает браузеру только httpOnly signed encrypted cookie. На последующих запросах JWT не покидает сервер: BFF читает его из session и добавляет Bearer на сторону YARP. Доменный сервис самостоятельно валидирует JWT.

## Диаграмма

```plantuml
@startuml
title Flow — User login + защищённый запрос

skinparam sequenceMessageAlign center
skinparam shadowing false

actor "User\n(Browser)" as B
participant "nginx\n(edge)" as N
participant "Next.js\n(UI + BFF)" as W
participant "YARP\n(API Gateway)" as Y
participant "auth-service" as A
participant "recipe-service" as R
database "auth-postgres" as APG
database "recipe-postgres" as RPG

== 1. Login ==
B -> N: POST /api/auth/login\n{username, password}
N -> W: POST /api/auth/login
W -> W: validate (Zod), CSRF
W -> Y: POST /auth/login
Y -> A: POST /auth/login
A -> APG: SELECT user by username
APG --> A: user + password hash
A -> A: verify password\n(Argon2id / BCrypt)
A -> A: issue JWT (HS256,\niss, aud, exp, sub)
A --> Y: 200 { access_token }
Y --> W: 200 { access_token }
W -> W: store JWT в server-side\nsession (signed encrypted)
W --> N: 204\nSet-Cookie: sid=…\n(HttpOnly, Secure, SameSite=Lax)
N --> B: 204 + cookie
note over B,W
  JWT остаётся на сервере (BFF).
  В браузер уходит только sid-cookie.
end note

== 2. Защищённый запрос ==
B -> N: GET /api/recipes/42\nCookie: sid=…
N -> W: GET /api/recipes/42
W -> W: открыть session\n→ извлечь JWT
W -> Y: GET /api/recipes/42\nAuthorization: Bearer <JWT>
Y -> R: GET /api/recipes/42\nAuthorization: Bearer <JWT>
R -> R: JWT middleware:\nvalidate sig, iss, aud, exp
R -> RPG: SELECT recipe
RPG --> R: row
R --> Y: 200 { recipe }
Y --> W: 200 { recipe }
W -> W: DTO → ViewModel
W --> N: 200 { view model }
N --> B: 200
@enduml
```
