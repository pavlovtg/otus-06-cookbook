# AR-0022: Internal-контроллеры регистрируются через ControllerFeatureProvider

Источник: ADR-0013

## Правило

Контроллеры остаются `internal` в соответствии с AR-0008.

Для регистрации `internal`-контроллеров в ASP.NET Core используется кастомный `ControllerFeatureProvider`, переопределяющий метод `IsController`.

Провайдер регистрируется в `Program.cs` через `AddControllers().ConfigureApplicationPartManager(...)`.

## Запрещено

- Делать контроллеры `public` ради обхода ограничения видимости.
- Использовать рефлексию или другие механизмы регистрации контроллеров в обход `ApplicationPartManager`.
