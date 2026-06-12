# AR-0040: Инструменты тестирования .NET

Источник: ADR-0033

## Правило

- Unit-тесты: xUnit + FluentAssertions; test-doubles — NSubstitute.
- Integration (DB): xUnit + Testcontainers (PostgreSQL).
- Integration (FileSystem): xUnit; временные директории через `Path.GetTempPath()`.
- Microservice-тесты: xUnit + `WebApplicationFactory` + Testcontainers + WireMock.Net.
- Покрытие измеряется через Coverlet; порог ≥ 80% statements на уровне сервиса.
