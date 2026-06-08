# AR-0024: HTTP-зависимости в тестах мокируются через WireMock.Net

Источник: ADR-0027

## Правило

В интеграционных тестах .NET-сервисов HTTP-зависимости (upstream-сервисы, внешние API) мокируются через `WireMock.Net`.

Адрес зависимости переопределяется через `AddInMemoryCollection` в `WithWebHostBuilder` — production-конфигурация не изменяется.

Тест верифицирует входящий запрос к mock: метод, путь, при необходимости — заголовки и тело.

## Запрещено

- In-process fake server (`WebApplication` на случайном порту) как замена WireMock.
- Подмена `HttpMessageHandler` через DI или YARP internals.
- Тесты без верификации запроса к upstream — они не проверяют корректность интеграции.
