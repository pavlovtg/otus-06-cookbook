# AR-0014: JWT не покидает BFF

- **Домен**: frontend
- **Статус**: активно

## Назначение

JWT backend используется только серверной стороной BFF для авторизованных запросов в YARP Gateway. В браузер уходит исключительно подписанный зашифрованный session cookie, не содержащий читаемого JWT в открытом виде.

## Когда применяется

Везде, где BFF получает, использует или хранит JWT backend.

## Правило

- JWT хранится только в server-side контексте BFF (внутри signed encrypted cookie, расшифровываемого только BFF).
- В ответах браузеру не должно присутствовать JWT в `Authorization`-заголовке, в JSON-теле, в `Set-Cookie` в читаемом виде, в HTML и в JS-бандле.
- Браузер не отправляет `Authorization: Bearer ...` напрямую в API Gateway — все запросы идут через BFF.

## Разрешено

- Передача JWT в `Authorization: Bearer ...` в server-to-server вызовах BFF → YARP Gateway.
- Передача session cookie между браузером и BFF (httpOnly + Secure + SameSite).

## Запрещено

- Сохранение JWT в `localStorage`, `sessionStorage`, нешифрованном cookie.
- Возврат JWT в JSON-ответах BFF клиенту.
- Прокидывание raw JWT в client-side React-компоненты.
- CI должен проверять отсутствие секретов backend и raw JWT в браузерном бандле.

## Связанные ADR

- [ADR-0005: JWT-аутентификация](../../../adr/rest-api/ADR-0005-jwt-authentication.md)
- [ADR-0017: BFF как логически выделенный слой](../../../adr/frontend/ADR-0017-bff-logical-layer.md)
- [AR-0013: BFF stateless](AR-0013-bff-stateless.md)
