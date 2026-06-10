# Активный контекст

## Текущая задача

**Docker Compose: вынос конфигурации в env-переменные** (10.06.2026)

## Что сделано в этой сессии

- Переписан `docker-compose.yml`: все параметры заменены на `${VAR:?NO_VAR}`
- Создан `.env` в корне со всеми переменными и значениями по умолчанию
- `nginx.conf` переименован в `nginx.conf.template`; хосты/порты параметризованы через env
- Обновлён `docs/standards/docker-compose-standard.md` — добавлен раздел «Конфигурация через env-переменные»
- Создан `AR-0032-docker-compose-env-config.md` (домен `general`, источник ADR-0007)
- Создан `AR-0033-nginx-templates-config.md` (домен `general`, источник ADR-0020)
- Обновлён `ARCHITECTURE.md` — добавлены AR-0032 и AR-0033

## Следующий шаг

Нет активных задач.
