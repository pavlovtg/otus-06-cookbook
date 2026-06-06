# Contributing Guide: описания типовых задач и CONTRIBUTING.md

## Роль

- AI-агент (Cline), создающий документацию для участников репозитория.

## Контекст

- Проект `otus-06-cookbook` — учебный монорепозиторий «Книга рецептов».
- Модель ветвления: GitHub Flow ([docs/standards/branch-plan.md](../../standards/branch-plan.md)).
- Структура репозитория: [docs/standards/repository-structure.md](../../standards/repository-structure.md).
- Типовые задачи:

  - **Реализация фичи** — новая функциональность, требует OpenSpec change.
  - **Техническая задача** — chore, refactor, bug, hotfix; без OpenSpec.

## Задача

### 1. Создать описание процесса реализации фичи

Создать файл `docs/contributing/feature-workflow.md`:

- Заголовок: «Реализация фичи».
- Шаги:

  1. Создать ветку от `main`: `feature/<short-description>`.
  2. Создать OpenSpec change: использовать скил `openspec-propose` — описать фичу, получить proposal, design, spec, tasks.
  3. Реализовать задачи из tasks: использовать скил `openspec-apply-change`.
  4. Создать Pull Request в `main` с обязательным review.
  5. После слияния удалить ветку.

- Добавить ссылки: branch-plan, openspec (`openspec/`).

### 2. Создать описание процесса технической задачи

Создать файл `docs/contributing/chore-workflow.md`:

- Заголовок: «Техническая задача».
- Применяется для: chore, refactor, bug, hotfix.
- Шаги:

  1. Создать ветку от `main` с нужным префиксом: `chore/`, `refactor/`, `bug/`, `hotfix/`.
  2. Выполнить задачу (без OpenSpec). Для создания технических промтов использовать skill - create-prompt.
  3. Создать Pull Request в `main` с обязательным review.
  4. После слияния удалить ветку.

- Добавить ссылку: branch-plan.

### 3. Создать `CONTRIBUTING.md` в корне репозитория

Содержание:

- Раздел «Структура проекта» — ссылка на [docs/standards/repository-structure.md](docs/standards/repository-structure.md).
- Раздел «Ветвление» — краткое описание GitHub Flow + ссылка на [docs/standards/branch-plan.md](docs/standards/branch-plan.md).
- Раздел «Типовые задачи» — ссылки:

  - Реализация фичи → [docs/contributing/feature-workflow.md](docs/contributing/feature-workflow.md)
  - Техническая задача → [docs/contributing/chore-workflow.md](docs/contributing/chore-workflow.md)

## Требования

- Все документы на русском языке.
- Применять принципы экономии токенов.
- `CONTRIBUTING.md` — для людей, лаконично.
- `feature-workflow.md` и `chore-workflow.md` — пошаговые инструкции, без лишних слов.
