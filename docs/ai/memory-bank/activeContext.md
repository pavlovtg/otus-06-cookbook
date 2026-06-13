# Активный контекст

## Текущая задача

`ingredients-crud` — CRUD ингредиентов (proposal + specs + design + tasks готовы).

## Что сделано в этой задаче

- Создан change `openspec/changes/ingredients-crud/`
- `proposal.md` — мотивация, список изменений, 4 новые возможности
- `specs/ingredient-list/spec.md`, `specs/ingredient-create/spec.md`, `specs/ingredient-edit/spec.md`, `specs/ingredient-delete/spec.md`
- `design.md` — архитектурные решения (агрегат в сервисе `recipes`, contract-first, валидация в домене, EF Core, frontend)
- `tasks.md` — 11 групп задач (контракт, domain, application, postgresql, web, тесты backend, frontend схемы/BFF/UI/тесты, e2e API, UI e2e)

## Следующий шаг

Реализация: `/opsx:apply ingredients-crud`
