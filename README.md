# Кулинарная книга

Учебный проект OTUS — веб-приложение для просмотра рецептов.

## Стек

- **Frontend**: Next.js 15 (App Router, RSC) + TypeScript + Zod
- **API Gateway**: ASP.NET Core 10 + YARP
- **Backend**: ASP.NET Core 10 + EF Core + PostgreSQL
- **Edge proxy**: nginx

## Запуск

```bash
docker compose up
```

Приложение будет доступно по адресу: <http://localhost:5500>

## Архитектура

```
Браузер → nginx:5500 → web:3000 (Next.js BFF) → api-gateway:8080 (YARP) → recipes:8080
                                                                          ↓
                                                                   postgresql:5432
```

Сети:
- `frontend-net`: `reverse-proxy`, `web`, `api-gateway`
- `backend-net`: `api-gateway`, `recipes`, `postgresql`

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
pnpm test
pnpm dev
```
