# user-auth

## 1. OpenAPI контракт

- [x] 1.1 Добавить эндпоинты `POST /api/v1/auth/register`, `POST /api/v1/auth/login`, `POST /api/v1/auth/logout`, `GET /api/v1/auth/me` в `docs/contracts/cookbook/recipes.yaml`
- [x] 1.2 Добавить схемы `RegisterRequest`, `LoginRequest`, `AuthResponse`, `UserDto` в контракт
- [x] 1.3 Пометить эндпоинты создания/редактирования/удаления рецептов как требующие Bearer-авторизации

## 2. Backend — домен и приложение

- [x] 2.1 Создать доменную сущность `User` с полями `id`, `email`, `displayName`, `passwordHash`, `role`
- [x] 2.2 Создать `UserRole` enum (`user`, `admin`) и `UserConstraints` с ограничениями полей
- [x] 2.3 Создать доменные исключения: `UserEmailAlreadyTakenException`, `UserNotFoundException`, `InvalidCredentialsException`
- [x] 2.4 Создать порт `IUserRepository` с методами `GetByEmailAsync`, `CreateAsync`
- [x] 2.5 Создать порт `IAuthService` с методами `RegisterAsync`, `LoginAsync`, `LogoutAsync` (только для авторизованных)
- [x] 2.6 Реализовать `AuthService`: хэширование пароля BCrypt (cost 12), выдача JWT (HS256, TTL 24 ч), `LogoutAsync` (завершение сессии на стороне BFF)

## 3. Backend — адаптер БД

- [x] 3.1 Создать `UserConfiguration` (EF Core `IEntityTypeConfiguration<User>`)
- [x] 3.2 Добавить `DbSet<User>` в `RecipeRepository` и зарегистрировать конфигурацию
- [x] 3.3 Создать миграцию для таблицы `users`
- [x] 3.4 Реализовать методы `IUserRepository` в `RecipeRepository`
- [x] 3.5 Добавить `SeedUsersAsync` в `CookbookSeeder` (2 пользователя: user + admin)

## 4. Backend — HTTP-адаптер

- [x] 4.1 Создать `AuthController` с эндпоинтами `POST /api/v1/auth/register`, `POST /api/v1/auth/login`, `POST /api/v1/auth/logout` (требует авторизации), `GET /api/v1/auth/me` (требует авторизации)
- [x] 4.2 Создать DTO: `RegisterRequest`, `LoginRequest`, `AuthResponse`, `UserDto`
- [x] 4.3 Добавить JWT-middleware в `Program.cs` (валидация sig, iss, aud, exp)
- [x] 4.4 Добавить `[Authorize]` на эндпоинты создания/редактирования/удаления рецептов
- [x] 4.5 Зарегистрировать `IUserRepository`, `IAuthService`, `AuthService` в DI
- [x] 4.6 Добавить переменную `JWT__Secret` в `.env` и `docker-compose.yml`

## 5. Backend — тесты

- [x] 5.1 Доработать `RecipeMicroserviceFixture`: добавить вспомогательный метод `LoginAsAsync(role)` / `GetAuthHeaderAsync()`, который регистрирует тестового пользователя через `POST /api/v1/auth/register` и возвращает Bearer-токен для использования в тестах
- [x] 5.2 Unit-тесты `AuthService`: тестируем через microservice-тесты
- [x] 5.3 Integration-тесты `UserRepository`: тестируем через microservice-тесты
- [x] 5.4 Microservice-тесты `AuthController` (`AuthControllerTests`): регистрация нового пользователя, дублирующийся email, логин с верными данными, логин с неверным паролем, `GET /api/v1/auth/me` с токеном, `GET /api/v1/auth/me` без токена, `POST /api/v1/auth/logout` с токеном, `POST /api/v1/auth/logout` без токена
- [x] 5.5 Обновить существующие microservice-тесты рецептов: добавить авторизацию через `RecipeMicroserviceFixture.GetAuthHeaderAsync()` для защищённых эндпоинтов (создание, редактирование, удаление)

## 6. Frontend — BFF

- [ ] 6.1 Установить `iron-session` в `apps/web`
- [ ] 6.2 Создать `lib/bff/auth.ts` с функциями `login`, `register`, `logout`, `getMe`
- [ ] 6.3 Создать Zod-схемы `LoginRequest`, `RegisterRequest`, `UserDto` в `lib/schemas/auth.ts`
- [ ] 6.4 Создать Route Handler `app/api/cookbook/v1/auth/login/route.ts` (проксирует на gateway, сохраняет JWT в session)
- [ ] 6.5 Создать Route Handler `app/api/cookbook/v1/auth/register/route.ts`
- [ ] 6.6 Создать Route Handler `app/api/cookbook/v1/auth/logout/route.ts` (проксирует `POST /api/v1/auth/logout` с Bearer на gateway, очищает session)
- [ ] 6.7 Создать Route Handler `app/api/cookbook/v1/auth/me/route.ts`
- [ ] 6.8 Добавить `SESSION_SECRET` в `.env` и `docker-compose.yml`
- [ ] 6.9 Обновить существующие BFF-функции рецептов: добавить `Authorization: Bearer` из session

## 7. Frontend — middleware и защита маршрутов

- [ ] 7.1 Создать `middleware.ts` в `apps/web`: редирект на `/login` для `/recipes/new` и `/recipes/[id]/edit` без session

## 8. Frontend — UI (по макету `docs/design/mockup/index.html` + `styles.css`, компоненты из `docs/design/storybook/`)

- [ ] 8.1 Создать страницу `/login` — форма входа (email, password) в стиле `.auth-card` из макета; ссылка «Нет аккаунта? Зарегистрироваться»
- [ ] 8.2 Создать страницу `/register` — форма регистрации (displayName, email, password) в стиле `.auth-card`; ссылка «Уже есть аккаунт? Войти»
- [ ] 8.3 Обновить шапку (`layout.tsx`): для авторизованных — `.user-chip` с аватаром, именем, тегом роли и кнопкой «Выйти»; для гостей — кнопки «Войти» / «Зарегистрироваться» (см. `#user-slot` в макете)
- [ ] 8.4 Скрыть кнопку «Новый рецепт» для неавторизованных пользователей
- [ ] 8.5 Скрыть кнопки «Редактировать» и «Удалить» рецепт для неавторизованных пользователей

## 9. Frontend — тесты

- [ ] 9.1 Unit-тесты Zod-схем `auth.ts`
- [ ] 9.2 Unit-тесты BFF `auth.bff.test.ts` (мокированный fetch)

## 10. E2E тесты

- [ ] 10.1 E2E API тест: регистрация → логин → запрос к защищённому эндпоинту
- [ ] 10.2 E2E API тест: попытка создать рецепт без токена → 401
- [ ] 10.3 E2E API тест: логин → логаут → повторный запрос к защищённому эндпоинту → 401
