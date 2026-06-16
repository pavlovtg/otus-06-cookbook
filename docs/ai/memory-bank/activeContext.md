# Активный контекст

## Текущая задача

Добавление измерения и контроля code coverage в GitHub CI (порог ≥ 80%).

## Что сделано

### Frontend (apps/web)

- `vitest.config.ts` — добавлена секция `coverage` с провайдером `v8` и порогами 80% по lines/functions/branches/statements
- `package.json` — добавлен скрипт `test:coverage: vitest run --coverage`

### Backend (.NET) — все 5 тестовых проектов

Добавлен пакет `coverlet.collector` v6.0.4:

- `apps/Cookbook/tests/Recipes.Tests/Recipes.Tests.csproj`
- `apps/ApiGateway/tests/ApiGateway.Tests/ApiGateway.Tests.csproj`
- `apps/Shared/tests/Shared.Database.Tests/Shared.Database.Tests.csproj`
- `apps/Shared/tests/Shared.Hosting.Tests/Shared.Hosting.Tests.csproj`
- `apps/Shared/tests/Shared.Testing.Tests/Shared.Testing.Tests.csproj`

### CI Workflows

- `.github/workflows/ci-pr.yml` — `test-backend` использует `--collect:"XPlat Code Coverage" /p:Threshold=80 /p:ThresholdType=line /p:ThresholdStat=total`; `test-web` использует `pnpm test:coverage`
- `.github/workflows/ci-push.yml` — аналогично

### Локальные скрипты

- `scripts/jobs/test-dotnet.sh` — синхронизирован с CI (coverage-флаги)
- `scripts/jobs/test-nextjs.sh` — синхронизирован с CI (`pnpm test:coverage`)

## Ключевые решения

- **Backend**: Coverlet через `--collect:"XPlat Code Coverage"` + MSBuild-свойства `/p:Threshold=80` — падает при coverage < 80%
- **Frontend**: Vitest `thresholds` в конфиге — падает при coverage < 80% автоматически при запуске `--coverage`
- **Branch protection**: `test-backend` и `test-web` уже являются required checks в ci-pr.yml; нужно убедиться, что они добавлены в Settings → Branches → main в GitHub UI
