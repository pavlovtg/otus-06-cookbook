# AR-0051: Конфигурация модели EF Core через IEntityTypeConfiguration

Источник: ADR-0014

## Правило

Конфигурация каждой сущности выносится в отдельный класс `IEntityTypeConfiguration<T>`, расположенный в папке `Configurations/`. В `OnModelCreating` допустим только вызов `modelBuilder.ApplyConfiguration(new XConfiguration())`.

## Запрещено

- Конфигурировать модель inline внутри `OnModelCreating`.
- Объединять конфигурации нескольких сущностей в одном классе.
