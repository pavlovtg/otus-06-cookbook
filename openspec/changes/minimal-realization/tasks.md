# minimal-realization

## 1. Контракт API (contract-first)

- [x] 1.1 Создать OpenAPI-спецификацию `docs/contracts/cookbook/recipes.yaml` (OpenAPI 3.0.0) с эндпоинтом `GET /api/recipes/v1`

## 2. Backend: сервис `recipes`

- [x] 2.1 Создать структуру проекта `apps/Cookbook/src/Recipes/` согласно стандарту `dotnet-project-structure.md`
- [x] 2.2 Реализовать доменный агрегат `Recipe` (id: Guid, title: string, description: string)
- [x] 2.3 Создать порт `IRecipeRepository` и use case `GetRecipesQuery`
- [x] 2.4 Реализовать `RecipesDbContext` со схемой `cookbook` (EF Core, Npgsql)
- [x] 2.5 Реализовать `RecipeRepository` (адаптер PostgreSQL)
- [x] 2.6 Создать и применить EF Core миграцию
- [x] 2.7 Добавить seed-данные через `HasData` (10 рецептов)
- [x] 2.8 Реализовать `RecipesController` (`ApiController`, `GET /api/recipes/v1`)
- [x] 2.9 Настроить DI, конфигурацию хоста в `Program.cs`
- [x] 2.10 Написать тест happy-path: `GET /api/recipes/v1` возвращает 200 с непустым массивом

## 3. API Gateway: сервис `api-gateway`

- [x] 3.1 Создать структуру проекта `apps/ApiGateway/src/ApiGateway/` согласно стандарту `dotnet-project-structure.md`
- [x] 3.2 Настроить маршрут `/api/cookbook/{**catch-all}` → `recipes/api/{**catch-all}` (стрип `/cookbook`)
- [x] 3.3 Настроить `Program.cs` и конфигурацию YARP

## 4. Frontend: сервис `web`

- [x] 4.1 Создать проект Next.js в `apps/web/` согласно стандарту `frontend-project-structure.md`
- [x] 4.2 Реализовать `lib/bff/gateway.ts` — HTTP-клиент к `api-gateway`
- [x] 4.3 Реализовать Zod-схему для `RecipeDto` в `lib/schemas/`
- [x] 4.4 Реализовать компонент `components/features/RecipeCard.tsx`
- [x] 4.5 Реализовать страницу `app/(public)/page.tsx` (Server Component, список рецептов)
- [x] 4.6 Написать тест: страница отображает список рецептов (Vitest + Testing Library)

## 5. Инфраструктура

- [x] 5.1 Создать `docker-compose.yml` с сервисами: `reverse-proxy`, `api-gateway`, `web`, `recipes`, `postgresql`
- [x] 5.2 Создать `nginx.conf` — весь трафик → `web` (BFF добавляет Authorization header)
- [x] 5.3 Создать `Dockerfile` для сервиса `recipes` (multi-stage, .NET 10)
- [x] 5.4 Создать `Dockerfile` для сервиса `api-gateway` (multi-stage, .NET 10)
- [x] 5.5 Создать `Dockerfile` для сервиса `web` (multi-stage, Node.js LTS)
- [x] 5.6 Настроить переменные окружения и health-check (`/api/health/v1`) для `depends_on`

## 6. Проверка

- [x] 6.1 Запустить `docker compose up` в чистом окружении и убедиться, что список рецептов отображается в браузере
- [x] 6.2 Обновить `README.md` — инструкция по запуску
