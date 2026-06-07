# C4 Component — Frontend (web)

Источник: ADR-0015, ADR-0017, AR-0010

## Описание

Внутреннее устройство контейнера `web` (Next.js): граница между client-кодом (UI) и серверным BFF-слоем. Браузер ходит только в Route Handlers `/api/*`; `lib/bff/*` и модуль сессии — серверные, никогда не импортируются из client. JWT не покидает сервер: в браузер уходит только httpOnly signed encrypted cookie.

## Диаграмма

```plantuml
@startuml
title C4 L3 — Components: web (Next.js)

skinparam rectangle {
  BackgroundColor<<component>> #85BBF0
  FontColor<<component>> black
  BackgroundColor<<server>> #438DD5
  FontColor<<server>> white
  BackgroundColor<<ext>> #999999
  FontColor<<ext>> white
  BackgroundColor<<boundary>> transparent
}

rectangle "Browser\n[Client]" as browser <<ext>>
rectangle "api-gateway (YARP)\n[Container]" as yarp <<ext>>

rectangle "web (Next.js)" as web <<boundary>> {

  rectangle "Client bundle" as clientSide <<boundary>> {
    rectangle "UI Pages (RSC)\n[Server Components]\nSSR публичных страниц,\nдашборд, планировщик" as ui <<component>>
    rectangle "Client Components\n['use client']\nИнтерактив, формы\n(react-hook-form + Zod)" as clientUi <<component>>
  }

  rectangle "Server-side (BFF)" as serverSide <<boundary>> {
    rectangle "Route Handlers\napp/api/*/route.ts\nЕдиная точка входа\nдля браузера" as routes <<server>>
    rectangle "Server Actions" as actions <<server>>
    rectangle "lib/bff/*\nАгрегация, DTO→VM,\nсерверная валидация,\nCSRF, security headers" as bff <<server>>
    rectangle "Session module\nsigned encrypted\nhttpOnly cookie,\nхранит JWT server-side" as session <<server>>
    rectangle "HTTP client → YARP\nfetch с Bearer JWT" as httpClient <<server>>
  }
}

browser --> ui         : HTTPS (SSR / RSC)
browser --> clientUi   : Hydration
clientUi --> routes    : fetch /api/*
clientUi --> actions   : Server Action
ui --> bff             : вызовы на сервере
routes --> bff
actions --> bff
bff --> session        : read/write
bff --> httpClient
session ..> browser    : Set-Cookie (httpOnly)
httpClient --> yarp    : Bearer JWT\n(JWT не уходит в браузер)
@enduml
```
