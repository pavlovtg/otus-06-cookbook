# Стандарт: стиль C#-кода

## Назначение и область применения

Минимальный набор обязательных правил оформления C#-кода во всех backend-проектах.
Правила для секции `[*.cs]` фиксируются в корневом `.editorconfig` репозитория (см. [AR-0010](../architecture/rules/general/AR-0010-editorconfig-mandatory.md)).

## Правила

- Отступы — 4 пробела, табы запрещены.
- Окончания строк — LF; кодировка — UTF-8 без BOM.
- В каждом C#-проекте: `<Nullable>enable</Nullable>`, `<ImplicitUsings>enable</ImplicitUsings>`, `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`.
- File-scoped namespace (`namespace Foo.Bar;`), блочный namespace запрещён.
- Порядок `using`: сначала `System.*`, затем остальные — в алфавитном порядке; `using` вне namespace.
- Проверка стиля в CI: `dotnet format --verify-no-changes`.
