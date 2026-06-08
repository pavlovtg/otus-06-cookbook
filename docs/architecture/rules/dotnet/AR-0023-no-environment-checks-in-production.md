# AR-0023: Запрет проверок environment и провайдера в production-коде

## Правило

Production-код ведёт себя одинаково во всех средах (dev, staging, production, тесты).

Инициализация БД в `Program.cs` всегда вызывает `MigrateAsync()` — без условий по провайдеру или environment.

Тесты используют реальную СУБД через Testcontainers — это гарантирует, что тестируется тот же production-код без изменений.

## Запрещено

- Проверки `IHostEnvironment.IsEnvironment(...)`, `IsDevelopment()`, `IsProduction()` в `Program.cs` и production-коде сервисов.
- Проверки `db.Database.IsRelational()`, `db.Database.IsInMemory()` в production-коде.
- Использование InMemory-провайдера EF Core в тестах — он скрывает реальное поведение БД.
- Подмена провайдера БД в тестах через `ConfigureServices` (замена Npgsql на InMemory).
