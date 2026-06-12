# Активный контекст

## Текущая задача

Реструктуризация .NET-проектов: единый `Backend.slnx` + bounded context `Shared`.

## Что сделано в этой задаче

- Создан `apps/Backend.slnx` — единый solution для всех .NET-проектов
- Создан bounded context `apps/Shared/` с проектами:
  - `src/Shared.Database`, `src/Shared.Hosting`, `src/Shared.Testing`
  - `tests/Shared.Database.Tests`, `tests/Shared.Hosting.Tests`, `tests/Shared.Testing.Tests`
- `Recipes.csproj` и `ApiGateway.csproj` — добавлены `ProjectReference` на `Shared.Database`, `Shared.Hosting`
- `Recipes.Tests.csproj` и `ApiGateway.Tests.csproj` — добавлены `ProjectReference` на `Shared.Testing`
- CI-скрипты `test-dotnet.sh`, `lint-dotnet.sh` — переведены на `apps/Backend.slnx`
- GitHub Actions `ci-push.yml`, `ci-pr.yml` — `lint-cookbook`/`lint-apigateway` → `lint-backend`, `test-cookbook`/`test-apigateway` → `test-backend`, фильтр `backend` объединяет ApiGateway + Cookbook + Shared
- `apps/Cookbook/Dockerfile`, `apps/ApiGateway/Dockerfile` — `COPY . .` + `dotnet publish <Project>`, контекст сборки `apps/`
- `scripts/jobs/build-dotnet.sh` — `docker build -f apps/*/Dockerfile apps/`
- `docker-compose.yml` — `context: apps`, `dockerfile: ApiGateway/Dockerfile` и `Cookbook/Dockerfile`
- Стандарт `dotnet-project-structure.md` — обновлён

## Следующий шаг

Нет активных задач.
