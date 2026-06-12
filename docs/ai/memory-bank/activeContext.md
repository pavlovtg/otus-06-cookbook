# Активный контекст

## Текущая задача

Нет активных задач.

## Что сделано в этой задаче

Применён AR-0055 к `Program.cs`:

- `Program.cs` — прямой вызов `db.Database.MigrateAsync()` заменён на `app.MigrateDatabaseAsync<Program, RecipeRepository>()` из `Shared.Hosting`

## Следующий шаг

Нет активных задач.
