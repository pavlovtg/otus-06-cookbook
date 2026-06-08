# minimal-realization

## 1. Контракт API (contract-first)

- [ ] 1.1 Создать OpenAPI-спецификацию `docs/contracts/cookbook/recipes.yaml` (OpenAPI 3.0.0) с эндпоинтом `GET /api/recipes/v1`

## 2. Backend: сервис `recipes`

- [ ] 2.1 Создать структуру проекта `apps/Cookbook/src/Recipes/` согласно стандарту `dotnet-project-structure.md`
- [ ] 2.2 Реализовать доменный агрегат `Recipe` (id: Guid, title: string, description: string)
- [ ] 2.3 Создать порт `IRecipeRepository` и use case `GetRecipesQuery`
- [ ] 2.4 Реализовать `RecipesDbContext` со схемой `cookbook` (EF Core, Npgsql)
- [ ] 2.5 Реализовать `RecipeRepository` (адаптер PostgreSQL)
- [ ] 2.6 Создать и применить EF Core миграцию
- [ ] 2.7 Добавить seed-данные через `HasData` (10 рецептов)
- [ ] 2.8 Реализовать `RecipesController` (`ApiController`, `GET /api/recipes/v1`)
- [ ] 2.9 Настроить DI, конфигурацию хоста в `Program.cs`
- [ ] 2.10 Написать тест happy-path: `GET /api/recipes/v1` возвращает 200 с непустым массивом

## 3. API Gateway: сервис `api-gateway`

- [ ] 3.1 Создать структуру проекта `apps/ApiGateway/src/ApiGateway/` согласно стандарту `dotnet-project-structure.md`
- [ ] 3.2 Настроить маршрут `/api/cookbook/{**catch-all}` → `recipes/api/{**catch-all}` (стрип `/cookbook`)
- [ ] 3.3 Настроить `Program.cs` и конфигурацию YARP

## 4. Frontend: сервис `web`

- [ ] 4.1 Создать проект Next.js в `apps/web/` согласно стандарту `frontend-project-structure.md`
- [ ] 4.2 Реализовать `lib/bff/gateway.ts` — HTTP-клиент к `api-gateway`
- [ ] 4.3 Реализовать Zod-схему для `RecipeDto` в `lib/schemas/`
- [ ] 4.4 Реализовать компонент `components/features/RecipeCard.tsx`
- [ ] 4.5 Реализовать страницу `app/(public)/page.tsx` (Server Component, список рецептов)
- [ ] 4.6 Написать тест: страница отображает список рецептов (Vitest + Testing Library)

## 5. Инфраструктура

- [ ] 5.1 Создать `docker-compose.yml` с сервисами: `reverse-proxy`, `api-gateway`, `web`, `recipes`, `postgresql`
- [ ] 5.2 Создать `nginx.conf` — маршрутизация `/api/*` → `api-gateway`, остальное → `web`
- [ ] 5.3 Создать `Dockerfile` для сервиса `recipes` (multi-stage, .NET 10)
- [ ] 5.4 Создать `Dockerfile` для сервиса `api-gateway` (multi-stage, .NET 10)
- [ ] 5.5 Создать `Dockerfile` для сервиса `web` (multi-stage, Node.js LTS)
- [ ] 5.6 Настроить переменные окружения и health-check для `depends_on`

## 6. Проверка

- [ ] 6.1 Запустить `docker compose up` в чистом окружении и убедиться, что список рецептов отображается в браузере
- [ ] 6.2 Обновить `README.md` — инструкция по запуску
