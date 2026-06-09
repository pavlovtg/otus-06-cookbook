# Активный контекст

## Текущая задача

**CI Pipeline — промт и архитектурные документы** (09.06.2026)

## Что сделано в этой сессии

- Промт `docs/ai/prompts/10-github-ci.md` — генерация GitHub Actions CI pipeline
- ADR-0030: Стратегия CI (два режима: push по paths, PR в main полный)
- AR-0030: CI toolchain (фиксированный набор инструментов по стекам)
- AR-0031: CI trigger strategy (правила запуска по событиям)
- Обновлён `docs/standards/ci-standard.md` (инструменты, структура jobs, режимы)
- Обновлён `ARCHITECTURE.md` (ADR-0030, AR-0030, AR-0031)

## Следующий шаг

Реализация `.github/workflows/ci-push.yml` и `ci-pr.yml` по промту `10-github-ci.md`.
