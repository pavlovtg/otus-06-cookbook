# Активный контекст

## Текущая задача

Нет активных задач.

## Что сделано в этой задаче

Обновлена конфигурация docker-compose для использования секции `DatabaseConnection`.

- `.env` — добавлена переменная `POSTGRESQL_PORT=5432`
- `docker-compose.yml` → сервис `recipes`:
  - убрана `ConnectionStrings__Recipes`
  - добавлены `DatabaseConnection__Host/Port/Database/Username/Password` через переменные `.env`

## Следующий шаг

Нет активных задач.
