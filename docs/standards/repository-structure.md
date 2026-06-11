# Стандарт структуры репозитория

## Назначение и область применения

Описывает структуру папок монорепозитория `otus-06-cookbook`.
Применяется ко всем компонентам: документации, коду, спецификациям, правилам AI-агента.

## Правила

Репозиторий организован по следующей структуре:

```
.clinerules/          # правила и скилы AI-агента
docs/
├── adr/
│   ├── general/      # общие ADR (не привязаны к конкретному домену)
│   ├── <domain>/     # домен: сервис, bounded context, технология
│   └── archive/      # устаревшие ADR
├── ai/
│   ├── memory-bank/  # контекст между сессиями AI-агента
│   └── prompts/      # история промтов для агента
├── architecture/
│   ├── overview.md   # обзор архитектуры
│   ├── components.md # описание компонентов
│   ├── diagrams/     # архитектурные диаграммы
│   └── rules/        # архитектурные правила (AR)
│       ├── general/  # общие AR
│       └── <domain>/ # домен: сервис, bounded context, технология
├── contributing/     # руководства для участников репозитория
├── design/
│   ├── guide/        # style guide продукта
│   ├── mockup/       # макет интерфейса
│   └── storybook/    # Storybook компонентов для разработчиков
└── standards/        # стандарты реализации
apps/                 # приложения (сервисы, фронтенд и т.д.)
├── <BoundedContext>/ # backend bounded context (.NET)
│   ├── <BoundedContext>.slnx
│   ├── src/          # исходники сервисов bounded context
│   └── tests/        # тестовые проекты bounded context
└── web/              # frontend + BFF (Next.js, TypeScript) — см. стандарт frontend-project-structure
infrastructure/
└── docker-compose/   # конфигурационные файлы сервисов для docker compose
    └── <service>/    # конфиги конкретного сервиса (nginx.conf, init.sql и т.д.)
scripts/              # скрипты запуска CI и вспомогательные скрипты
├── jobs/             # отдельные шаги CI (lint, build, test по стеку)
tests/                # тесты
openspec/             # спецификации и схемы (OpenSpec)
ARCHITECTURE.md       # единая точка входа в архитектурную документацию
CONTRIBUTING.md       # руководство для участников репозитория
README.md             # описание проекта
.clineignore          # правила игнорирования файлов для AI-агента
.gitignore            # правила игнорирования файлов для git
```

### Назначение папок

| Папка | Назначение |
|-------|-----------|
| `.clinerules/` | Правила поведения AI-агента, скилы, workflows |
| `docs/adr/` | Architecture Decision Records — зафиксированные архитектурные решения |
| `docs/ai/memory-bank/` | Контекст между сессиями AI-агента |
| `docs/ai/prompts/` | История промтов, использованных для настройки агента |
| `docs/architecture/` | Обзор архитектуры, компоненты, диаграммы, AR |
| `docs/contributing/` | Руководства для участников: процессы реализации фич и технических задач |
| `docs/design/guide/` | Style guide продукта: цвета, типографика, токены |
| `docs/design/mockup/` | Макет интерфейса (статический прототип) |
| `docs/design/storybook/` | Storybook компонентов — живой каталог UI для разработчиков |
| `docs/standards/` | Стандарты реализации — обязательные соглашения по коду и документации |
| `apps/` | Исходный код приложений и сервисов |
| `infrastructure/docker-compose/` | Конфигурационные файлы сервисов для docker compose (nginx.conf, init.sql и т.д.) |
| `scripts/` | Скрипты запуска CI и вспомогательные скрипты |
| `scripts/jobs/` | Отдельные шаги CI: lint, build, test по стеку |
| `tests/` | Тесты (интеграционные, e2e и т.д.) |
| `openspec/` | Спецификации изменений, схемы, шаблоны OpenSpec |
| `ARCHITECTURE.md` | Единая точка входа: индексы AR, стандартов, ADR |
| `CONTRIBUTING.md` | Руководство для участников: типовые задачи и процессы |
