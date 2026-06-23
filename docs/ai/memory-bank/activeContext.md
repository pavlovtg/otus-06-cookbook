# Активный контекст

## Текущая задача

Адаптивная мобильная верстка — завершено.

## Последнее завершённое

Мобильная адаптивность (Вариант A — только CSS):

- `globals.css`: добавлен блок `@media (max-width: 480px)`:
  - `.nav a span { display: none }` — nav только иконки на мобиле
  - `.user-chip > span:not(.avatar) { display: none }` — скрыть имя/роль в шапке
  - `.detail-bar { flex-wrap: wrap }` + title/meta на отдельные строки
  - `.auth-shell { padding: 24px }` — меньше отступ на auth-страницах
  - `.kpi-grid { grid-template-columns: 1fr }` — 1 колонна на дашборде
  - `.shopping-row { grid-template-columns: 1fr auto }` — 2 колонны в списке покупок
  - `.page-heading`, `.detail-toolbar`, `.detail-actions` — стек на мобиле
  - `.form-actions .btn { width: 100% }` — кнопки форм на всю ширину

## Ранее завершённое

Ограничение доступа к категориям рецептов + правка тестов:

- `CategoriesController.cs`: `[Authorize]` + `IsAdmin()` на POST/PUT/DELETE → 403 если не admin
- `middleware.ts`: `/categories` добавлен в `ADMIN_PATHS`
- `layout.tsx`: ссылка «Категории» только при `user?.role === "admin"`
- `categories/page.tsx`: серверный guard → `redirect("/")` если не admin
- Тесты: `CategoriesCrudTests.cs`, `test_categories_api.py`, `test_categories.py` — обновлены

## Ранее завершённое

Рефакторинг страницы `/ingredients`:

- `pageSize=20`, порядок `IngredientCategory` приведён к алфавитному порядку БД
- Кнопки редактирования/удаления скрыты для неавторизованных
- Системные ингредиенты — только admin; флаг `isSystem` — только admin
- Backend: `[Authorize]` + проверки в `IngredientService`

## Ключевые решения

- Auth-модуль — инфраструктурный cross-cutting слой внутри recipes-сервиса
- Пользователи хранятся в схеме `cookbook` той же PostgreSQL
- JWT выпускается и валидируется recipes-сервисом
- S2S не нужен (один доменный сервис в MVP)
- Token blacklist — не реализован, тест скипнут
- Hard navigation после логина нужна, т.к. Next.js layout — серверный компонент
- iron-session хранит JWT в зашифрованной cookie `cookbook_session`
- 403 на детальной странице → UI-сообщение вместо `notFound()`
- `serverFetch` в `lib/server-fetch.ts` — обёртка для Server Components с авторизацией
- `CommentsSection` — Client Component; первая страница — SSR, смена страниц — клиент
- DnD планировщика — `@dnd-kit/core` + `@dnd-kit/utilities` (ADR-0036, AR-0064)
- Все API-запросы: nginx → Next.js BFF route handlers → gateway → recipes
- `IngredientCategory` в БД — snake_case, сортировка алфавитная
- Адаптивность — только CSS (`globals.css`), без изменений TSX; брейкпойнты: 1100px / 768px / 480px
