# user-auth

## Контекст

Recipes-сервис не имеет аутентификации. Все эндпоинты публичны. ADR-0035 зафиксировал решение: auth реализуется как модуль внутри recipes-сервиса (не отдельный контейнер). ADR-0005 определил JWT (HS256) как механизм аутентификации. AR-0010 требует, чтобы JWT не покидал серверную сторону BFF — в браузер уходит только httpOnly cookie с session-id. AR-0013 требует, чтобы recipes-сервис самостоятельно валидировал JWT.

## Цели / Не-цели

**Цели:**

- Регистрация и вход пользователей через recipes-сервис.
- Выдача JWT (HS256) при успешном логине.
- Хранение JWT в server-side session BFF; передача в браузер только через httpOnly cookie.
- Валидация JWT в recipes-сервисе на каждом защищённом запросе.
- Защита эндпоинтов создания/редактирования/удаления рецептов.
- Два seed-пользователя (user + admin).
- Страницы входа и регистрации в UI.

**Не-цели:**

- Refresh-токены (TTL access-токена 24 ч, повторный логин по истечении).
- Blacklist / revocation токенов.
- Смена и восстановление пароля.
- Разграничение приватных/публичных рецептов по `author_id` — отдельный change.
- S2S-аутентификация.

## Решения

### 1. Auth-модуль в recipes-сервисе

Auth реализуется как набор файлов внутри существующего recipes-сервиса: доменная сущность `User`, Application-сервис `AuthService`, контроллер `AuthController`. Отдельный контейнер не создаётся (ADR-0035).

### 2. Хранение паролей — BCrypt, cost 12

BCrypt с cost ≥ 12 (NFR-0001). Альтернатива Argon2id отклонена: BCrypt достаточен для MVP и не требует дополнительных NuGet-пакетов.

### 3. JWT — HS256, TTL 24 ч

Алгоритм HS256, секрет ≥ 256 бит из переменной окружения `JWT__Secret`. Claims: `sub` (userId), `role`, `iss`, `aud`, `exp`. TTL 24 ч — refresh-токены out of scope. Альтернатива RS256 отклонена: нет нескольких сервисов-потребителей в MVP.

### 4. Session в BFF — iron-session

JWT хранится в server-side session через `iron-session` (Next.js). В браузер уходит только подписанный зашифрованный cookie `sid` (HttpOnly, Secure, SameSite=Lax). На каждом защищённом BFF-запросе JWT извлекается из session и добавляется в заголовок `Authorization: Bearer` перед проксированием на YARP. Альтернатива хранения JWT в localStorage отклонена: нарушает AR-0010.

### 5. Защита маршрутов в BFF — Next.js middleware

Next.js middleware проверяет наличие валидной session для маршрутов `/recipes/new`, `/recipes/[id]/edit`. Если session отсутствует — редирект на `/login`. Серверная валидация JWT остаётся на стороне recipes-сервиса.

### 6. Маршруты API

По стандарту `{VERB} /api/v{version}/{resource}`:

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/logout`
- `GET /api/v1/auth/me`

Через gateway: `/api/cookbook/v1/auth/...` → recipes-сервис `/api/v1/auth/...`.

BFF Route Handlers: `/api/cookbook/v1/auth/...` (проксируют на gateway).

## Риски / Компромиссы

- **Нет инвалидации токенов** → при компрометации токена пользователь остаётся авторизованным до истечения TTL (24 ч). Принято как допустимое для MVP.
- **Один сервис — issuer и validator** → при масштабировании потребуется вынести auth (ADR-0035 это фиксирует). Для MVP приемлемо.
- **iron-session шифрует cookie на сервере** → при смене `SESSION_SECRET` все сессии инвалидируются. Задокументировать в README.
