# dashboards

## 1. Backend — Domain / Application

- [x] 1.1 Создать view-типы: `DashboardStatsView`, `RecipeRankView`, `CategoryCountView`, `UserRankView`
- [x] 1.2 Создать порт `IDashboardRepository`
- [x] 1.3 Создать `DashboardService` с методом `GetStatsAsync(UserId? userId, bool isAdmin, CancellationToken ct)`

## 2. Backend — Adapter / Postgresql

- [x] 2.1 Реализовать `IDashboardRepository` в `RecipeRepository`: `TotalRecipes`, `MyRecipes`, `MyComments`
- [x] 2.2 Реализовать `Top10ByRating` и `TopFavoritesByRating`
- [x] 2.3 Реализовать `ByMainIngredient` и `ByCuisine`
- [x] 2.4 Реализовать `TotalUsers`, `TotalComments`, `TopUsersByRating`, `TopUsersByComments` (admin)
- [x] 2.5 Реализовать `PlanFill` (словарь заполненности слотов плана меню)

## 3. Backend — Adapter / Web

- [x] 3.1 Создать DTO: `DashboardStatsDto`, `RecipeRankDto`, `CategoryCountDto`, `UserRankDto`
- [x] 3.2 Создать `DashboardController` с `GET /api/v1/dashboard` (`[AllowAnonymous]`)
- [x] 3.3 Зарегистрировать `DashboardService` и `IDashboardRepository` в DI

## 4. OpenAPI-контракт

- [x] 4.1 Добавить эндпоинт `GET /api/v1/dashboard` и схемы DTO в `docs/contracts/cookbook/cookbook.yaml`

## 5. Backend — Тесты

- [x] 5.1 Integration-тест: `TotalRecipes` для гостя
- [x] 5.2 Integration-тест: `MyRecipes` и `MyComments` для авторизованного пользователя
- [x] 5.3 Integration-тест: `TotalUsers`, `TotalComments` для admin
- [x] 5.4 Integration-тест: топ-10 по рейтингу — корректная сортировка
- [x] 5.5 Integration-тест: `ByMainIngredient` — корректная группировка
- [x] 5.6 Integration-тест: `PlanFill` — корректное отображение заполненных слотов
- [x] 5.7 Microservice-тест: `GET /api/v1/dashboard` → 200 для анонимного запроса
- [x] 5.8 Microservice-тест: `GET /api/v1/dashboard` → 200 с расширенными полями для авторизованного пользователя
- [x] 5.9 Microservice-тест: `GET /api/v1/dashboard` → 200 с admin-полями для администратора

## 6. Frontend — BFF и схемы

- [x] 6.1 Создать Zod-схемы в `lib/schemas/dashboard.ts`: `DashboardStatsDtoSchema` и вложенные схемы
- [x] 6.2 Создать `lib/bff/dashboard.server.ts` с функцией `getDashboardStats()` через `serverFetch`
- [x] 6.3 Создать BFF route handler `app/api/cookbook/v1/dashboard/route.ts` (proxy `GET`)

## 7. Frontend — Storybook-компоненты дашборда

- [x] 7.1 Проверить наличие компонентов `Kpi`, `BarChart`, `TopList`, `PlanFill` в `docs/design/storybook/src/dash/` — они уже есть в Storybook; при необходимости перенести в `apps/web/components/features/dashboard/`
- [x] 7.2 Убедиться, что CSS-классы `.kpi-grid`, `.dash-grid`, `.dash-block`, `.dash-chart`, `.mini-week-grid`, `.mini-slot`, `.mini-slot.is-filled`, `.top-list`, `.top-list-row` присутствуют в `apps/web/app/globals.css` (они уже есть в `docs/design/mockup/styles.css` — источнике стилей)

## 8. Frontend — Страница дашборда

- [x] 8.1 Создать страницу `app/dashboard/page.tsx` (Server Component): KPI-карточки, топ-10 списки, мини-сетка плана меню
- [x] 8.2 Создать Client Component `DashboardCharts.tsx` с гистограммами Chart.js (`dynamic import`) — использовать паттерн из макета (`views.dashboard` в `index.html`)
- [x] 8.3 Добавить пункт «Дашборд» в навигацию `layout.tsx` (без ограничений по роли, иконка `ChartIcon`)

## 9. Frontend — Тесты

- [x] 9.1 Unit-тест `DashboardStatsDtoSchema`: валидация корректного ответа для гостя
- [x] 9.2 Unit-тест `DashboardStatsDtoSchema`: валидация корректного ответа для авторизованного пользователя
- [x] 9.3 Unit-тест BFF `getDashboardStats`: мок fetch → корректный парсинг

## 10. E2E API-тесты

- [x] 10.1 Создать `tests/e2e/test_dashboard_api.py` со сценариями из `specs/dashboard-stats/spec.md`
