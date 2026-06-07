# ADR-0014: EF Core как ORM

- **Статус**: принят
- **Домен**: backend
- **Дата**: 2026-06-07

## Контекст

Backend-сервисы реализуются на .NET 10 / C# ([ADR-0011](ADR-0011-dotnet-csharp-backend.md)) и используют PostgreSQL ([ADR-0004](../general/ADR-0004-postgresql.md)).
Необходим единый способ доступа к данным с поддержкой миграций, маппинга на доменную модель (DDD, [ADR-0012](ADR-0012-domain-driven-design.md)) и интеграции с гексагональной архитектурой ([ADR-0013](ADR-0013-hexagonal-architecture.md)).

## Рассмотренные варианты

- **EF Core** — официальный ORM от Microsoft, нативная поддержка PostgreSQL (Npgsql), LINQ, миграции, поддержка value objects через owned types и value converters, change tracking.
- **Dapper** — микро-ORM, быстрый, гибкий, но не предоставляет миграций и маппинга на агрегаты «из коробки».
- **Чистый Npgsql / ADO.NET** — максимальный контроль, но высокая стоимость поддержки.

## Решение

В качестве основного ORM используется **EF Core** (версия, поставляемая с .NET 10) с провайдером **Npgsql**.

- EF Core используется только внутри outbound-адаптера persistence (`Adapters/Postgresql`).
- DbContext, конфигурации сущностей, миграции — внутри адаптера.
- Доменная модель не знает про EF Core; маппинг — через configuration-классы (`IEntityTypeConfiguration<T>`).

## Последствия

- Миграции схемы БД выполняются средствами EF Core.
- Доступен LINQ и change tracking, но необходимо контролировать N+1 и leaky abstractions из домена в инфраструктуру.
- При появлении сценариев с критичной производительностью допустимо точечное использование Dapper / raw SQL внутри того же адаптера.

## Связанные документы

- [ADR-0004: PostgreSQL как СУБД](../general/ADR-0004-postgresql.md).
- [ADR-0011: .NET 10 / C# как платформа backend](ADR-0011-dotnet-csharp-backend.md).
- [ADR-0013: Гексагональная архитектура](ADR-0013-hexagonal-architecture.md).
