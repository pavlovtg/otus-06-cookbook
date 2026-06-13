# AR-0053: Запрет HasData для seed-данных

Источник: ADR-0014

## Правило

Seed-данные не загружаются через `HasData` в конфигурации модели.

## Запрещено

- Использовать `HasData` в любом классе `IEntityTypeConfiguration` или в `OnModelCreating`.
