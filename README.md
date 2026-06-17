# Кулинарная книга

Учебный проект OTUS — веб-приложение для хранения, поиска и планирования рецептов.

## Стек

| Слой | Технологии |
|------|-----------|
| Frontend | Next.js 15 (App Router, RSC) + TypeScript + Zod |
| API Gateway | ASP.NET Core 10 + YARP |
| Backend | ASP.NET Core 10 + EF Core + PostgreSQL |
| Edge proxy | nginx |
| БД | PostgreSQL 16 |

## Быстрый старт

```bash
git clone git@github.com:pavlovtg/otus-06-cookbook.git
cd otus-06-cookbook
docker compose up
```

Приложение будет доступно по адресу: <http://localhost:5500>

> Seed-данные загружаются автоматически при первом запуске: 25+ рецептов, 50+ ингредиентов, план меню на неделю, комментарии.

## Тестовые учётные данные

| Email | Пароль | Роль |
|-------|--------|------|
| `user@cookbook.local` | `1234567890` | Пользователь |
| `admin@cookbook.local` | `1234567890` | Администратор |
| `renat@cookbook.local` | `1234567890` | Пользователь |
| `ivlev@cookbook.local` | `1234567890` | Администратор |

## Swagger UI

После запуска документация API доступна по адресу:

<http://localhost:5500/api-docs>

## Архитектура

```
Браузер → nginx:5500 → web:3000 (Next.js BFF) → api-gateway:8080 (YARP) → recipes:8080
                                                                          ↓
                                                                   postgresql:5432
```

Сети:

- `frontend-net`: `reverse-proxy`, `web`, `api-gateway`
- `backend-net`: `api-gateway`, `recipes`, `postgresql`

## Запуск тестов

### Все тесты

```bash
sh scripts/test.sh
```

### .NET unit/integration тесты (Backend + API Gateway)

```bash
dotnet test apps/Backend.slnx \
  --collect:"XPlat Code Coverage" \
  /p:Threshold=80 \
  /p:ThresholdType=line \
  /p:ThresholdStat=total
```

Или через скрипт:

```bash
sh scripts/jobs/test-dotnet.sh
```

### Frontend unit-тесты (Next.js + Vitest)

```bash
cd apps/web
pnpm install
pnpm test:coverage
```

Или через скрипт:

```bash
sh scripts/jobs/test-nextjs.sh
```

### E2E API тесты (Playwright / Python)

Требуют запущенного приложения (`docker compose up`). Скрипт поднимает стек автоматически:

```bash
sh scripts/jobs/test-e2e.sh
```

### UI тесты (Playwright / Python)

Требуют запущенного приложения. Скрипт поднимает стек автоматически:

```bash
sh scripts/jobs/test-ui.sh
```

## Разработка

### Backend (recipes)

```bash
cd apps/Cookbook
dotnet test
```

### API Gateway

```bash
cd apps/ApiGateway
dotnet test
```

### Frontend

```bash
cd apps/web
pnpm install
pnpm dev
```
