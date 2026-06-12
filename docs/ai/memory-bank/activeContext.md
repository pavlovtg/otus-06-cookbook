# Активный контекст

## Текущая задача

Нет активных задач.

## Что сделано в этой задаче

Исправлен баг: `System.ArgumentException: Must specify valid information for parsing in the string` при старте сервиса.

- Причина: миграция `AddRecipeFields` добавляла колонку `difficulty` с `defaultValue: ""`, старые строки получали пустую строку, `Enum.Parse<Difficulty>("")` падал.
- Исправление: `defaultValue: ""` → `defaultValue: "easy"` в `20260612000000_AddRecipeFields.cs`.

## Следующий шаг

Нет активных задач.
