# Прогресс

## Статус проекта

В разработке.

## Завершённые фичи

- Список рецептов (главная страница)
- Детальная страница рецепта
- Создание рецепта
- Редактирование рецепта
- Удаление рецепта
- CRUD ингредиентов (backend + тесты + frontend схемы/BFF + E2E тесты)
- `recipe-ingredients` — все 56 задач (секции 1–8) полностью реализованы
- Авторизация (`user-auth`) — JWT, BCrypt, iron-session, BFF, middleware, UI (login/register/user-chip)

## В работе

Нет активных задач.

## Заархивировано

- `recipe-categories` (31/31 задач) → `openspec/changes/archive/2026-06-16-recipe-categories/`
- `user-auth` (76/76 задач) → `openspec/changes/archive/2026-06-17-user-auth/`

## Выполнено (последнее — user-auth)

- Backend: `User`, `UserRole`, `IUserRepository`, `IAuthService`, `AuthService` (BCrypt cost 12, JWT HS256 TTL 24h)
- Backend: `AuthController` (register, login, logout, me), JWT-middleware, `[Authorize]` на CUD рецептов
- Backend: миграция `users`, `SeedUsersAsync` (user + admin)
- Frontend BFF: `lib/bff/auth.ts`, route handlers `/api/cookbook/v1/auth/*`, iron-session
- Frontend UI: `/login`, `/register`, user-chip в шапке, скрытие кнопок для гостей
- Middleware: редирект на `/login` для `/recipes/new` и `/recipes/[id]/edit`
- Тесты: unit (Zod, BFF), microservice (AuthController), E2E API, UI (4 теста auth)
- Багфикс: hard navigation после логина; маршрут `/api/cookbook/v1/auth/...`

## Выполнено (ранее)

- Багфикс `RecipesController`: `CategoryDomainException` не перехватывалась → добавлен catch
- Багфикс unit-тестов фронтенда: фикстуры без `categoryIds` → добавлен `categoryIds: []`
- Багфикс e2e-тестов: `VALID_RECIPE` без `categoryIds` → добавлен `"categoryIds": []`
- Багфикс fallback-тегов: убран fallback из `RecipeCard.tsx` и `page.tsx`
- Фикс UI-тестов ингредиентов: BFF-роуты, `router.push`, `force-dynamic`, SSR/CSR base URL
- Рефакторинг `CookbookSeeder`, UI-тесты фото
