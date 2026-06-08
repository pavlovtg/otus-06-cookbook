# AR-0018: Контроллеры вместо Minimal API

Источник: ADR-0011

## Правило

Все HTTP-эндпоинты в .NET-сервисах MUST реализовываться через явные контроллеры (`ApiController`), а не через Minimal API (`app.MapGet` и аналоги).

## Запрещено

- Использовать Minimal API (`MapGet`, `MapPost`, `MapPut`, `MapDelete` и т.п.) для реализации эндпоинтов сервисов.
