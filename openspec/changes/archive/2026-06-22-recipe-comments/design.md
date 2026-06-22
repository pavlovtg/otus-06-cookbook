# recipe-comments

## Контекст

Приложение поддерживает рецепты с рейтингами и избранным, но не имеет комментариев. Комментарии — требование MVP (PRD-0001). Архитектура backend — гексагональная (Ports & Adapters) + DDD, ORM — EF Core, БД — PostgreSQL. Frontend — Next.js App Router с BFF-слоем в `lib/bff/`. Контракт API описывается в `docs/contracts/cookbook/recipes.yaml` до реализации (AR-0015).

## Цели / Не-цели

**Цели:**

- Новая таблица `recipe_comments` в схеме `cookbook`.
- Три новых эндпоинта: получение (с пагинацией), добавление и удаление комментария.
- Доменный объект `RecipeComment` с валидацией ограничений.
- BFF-функции и UI-секция комментариев на детальной странице рецепта.
- Seed-данные: 20+ комментариев.
- Unit, integration, microservice, E2E API и UI E2E тесты.

**Не-цели:**

- Редактирование комментария.
- Уведомления об ответах на комментарии.

## Решения

### Хранение комментариев

Комментарии хранятся в таблице `recipe_comments` в схеме `cookbook` той же PostgreSQL (AR-0017, AR-0025). Уникальный индекс `(recipe_id, author_id)` обеспечивает ограничение «один комментарий на рецепт от пользователя» на уровне БД.

Альтернатива — проверка только в Application-слое — отклонена: уникальный индекс надёжнее при параллельных запросах.

### Доменная модель

`RecipeComment` — агрегат с типизированным `RecipeCommentId` (AR-0019). Ограничения текста (1–2000 символов) объявлены в `RecipeCommentConstraints` (AR-0039). Доменные исключения: `CommentTextEmptyException`, `CommentTextTooLongException`, `CommentAlreadyExistsException`, `CommentNotFoundException`, `CommentForbiddenException` (AR-0036).

### Права доступа при удалении

Проверка прав выполняется в Application-слое (`RecipeCommentService`): удалить может автор комментария, автор рецепта или администратор. Для этого сервис получает и рецепт, и комментарий, сравнивает `authorId`. Это соответствует существующему паттерну в `RecipeService`.

### Пагинация

Список комментариев возвращается через `PagedResult<CommentDto>` (существующий тип в Application-слое). Размер страницы по умолчанию — 10, максимум — 1000 (в соответствии со стандартом API Design). Сортировка: `createdAt DESC`.

### API-контракт

Три новых пути в `docs/contracts/cookbook/recipes.yaml`:

- `GET /api/v1/recipes/{id}/comments` — публичный, параметры `page` и `pageSize`, возвращает `PagedResult<CommentDto>`.
- `POST /api/v1/recipes/{id}/comments` — требует JWT, тело `CommentRequest`.
- `DELETE /api/v1/recipes/{id}/comments/{commentId}` — требует JWT.

`CommentDto` содержит: `id`, `authorId`, `authorName`, `text`, `createdAt`. `authorName` денормализуется через JOIN с таблицей `users` в репозитории.

### Frontend

BFF-файл `lib/bff/comments.ts` (AR-0057) содержит три функции: `getComments`, `addComment`, `deleteComment`. Секция комментариев на детальной странице рецепта — Client Component (интерактивная форма и пагинация). Первая страница комментариев загружается через Server Component при первом рендере.

### Тестирование

**Unit-тесты** (`Recipes.Tests/Unit/`):

- `RecipeComment` — валидация текста: пустой, превышение 2000 символов, граничные значения.

**Integration-тесты** (`Recipes.Tests/Integration/`):

- `RecipeRepository` — сохранение и получение комментариев через реальный Testcontainers-экземпляр PostgreSQL; уникальный индекс при дублировании.

**Microservice-тесты** (`Recipes.Tests/Microservice/`):

- `GET /api/v1/recipes/{id}/comments` — публичный доступ, пагинация, пустой список.
- `POST /api/v1/recipes/{id}/comments` — успех, повтор (409), без JWT (401), превышение длины (400).
- `DELETE /api/v1/recipes/{id}/comments/{commentId}` — автор комментария, автор рецепта, чужой пользователь (403), без JWT (401).

**E2E API-тесты** (`tests/e2e/test_comments_api.py`):

- Полный сценарий: добавление комментария авторизованным пользователем, получение списка, удаление автором рецепта.

**UI E2E-тесты** (`tests/ui/test_comments.py`):

- Авторизованный пользователь добавляет комментарий на детальной странице рецепта — комментарий появляется в списке.
- Авторизованный пользователь удаляет свой комментарий — комментарий исчезает из списка.

## Риски / Компромиссы

- **Денормализация `authorName`** → при смене `display_name` пользователя старые комментарии покажут актуальное имя (JOIN, не копия). Это желаемое поведение.
- **Уникальный индекс на уровне БД** → при гонке двух одновременных запросов один получит ошибку БД, которую Application-слой преобразует в `CommentAlreadyExistsException`.
