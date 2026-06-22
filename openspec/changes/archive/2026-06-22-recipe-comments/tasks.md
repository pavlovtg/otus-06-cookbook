# recipe-comments

## 1. OpenAPI-контракт

- [x] 1.1 Добавить схемы `CommentDto`, `CommentRequest`, `PagedResultCommentDto` в `docs/contracts/cookbook/recipes.yaml`
- [x] 1.2 Добавить эндпоинт `GET /api/v1/recipes/{id}/comments` с параметрами `page` и `pageSize`
- [x] 1.3 Добавить эндпоинт `POST /api/v1/recipes/{id}/comments`
- [x] 1.4 Добавить эндпоинт `DELETE /api/v1/recipes/{id}/comments/{commentId}`

## 2. Доменный слой

- [x] 2.1 Создать `RecipeCommentId` — типизированный Value Object (`readonly record struct`)
- [x] 2.2 Создать `RecipeCommentConstraints` с константами `MinTextLength = 1`, `MaxTextLength = 2000`
- [x] 2.3 Создать агрегат `RecipeComment` с полями `Id`, `RecipeId`, `AuthorId`, `Text`, `CreatedAt` и фабричным методом с валидацией
- [x] 2.4 Создать доменные исключения: `CommentTextEmptyException`, `CommentTextTooLongException`, `CommentAlreadyExistsException`, `CommentNotFoundException`, `CommentForbiddenException`

## 3. Application-слой

- [x] 3.1 Добавить методы `GetCommentsAsync`, `AddCommentAsync`, `DeleteCommentAsync` в интерфейс `IRecipeCommentService`
- [x] 3.2 Создать `CommentDto` и `CommentRequest` в Application-слое
- [x] 3.3 Реализовать `RecipeCommentService` с проверкой прав при удалении (автор комментария / автор рецепта / admin)
- [x] 3.4 Добавить методы репозитория в `IRecipeRepository`: `GetCommentsPagedAsync`, `AddCommentAsync`, `DeleteCommentAsync`, `GetCommentAsync`

## 4. Адаптер PostgreSQL

- [x] 4.1 Создать `RecipeCommentConfiguration` — EF-конфигурация таблицы `recipe_comments` с уникальным индексом `(recipe_id, author_id)` и CASCADE DELETE
- [x] 4.2 Реализовать методы комментариев в `RecipeRepository` (JOIN с `users` для `authorName`, сортировка `createdAt DESC`)
- [x] 4.3 Сгенерировать миграцию `AddRecipeComments`
- [x] 4.4 Добавить 20+ комментариев в `CookbookSeeder`

## 5. Адаптер Web

- [x] 5.1 Создать DTO: `CommentDto`, `CommentRequest` в `Adapters/Web/Dto/`
- [x] 5.2 Добавить эндпоинты комментариев в `RecipesController` (GET, POST, DELETE)
- [x] 5.3 Зарегистрировать `IRecipeCommentService` / `RecipeCommentService` в DI

## 6. Unit-тесты

- [x] 6.1 Тесты `RecipeComment`: пустой текст, текст длиннее 2000 символов, граничные значения (1 и 2000 символов)

## 7. Integration-тесты

- [x] 7.1 Тесты `RecipeRepository`: сохранение комментария, получение постранично, уникальный индекс при дублировании

## 8. Microservice-тесты

- [x] 8.1 `GET /api/v1/recipes/{id}/comments` — публичный доступ, пагинация, пустой список
- [x] 8.2 `POST /api/v1/recipes/{id}/comments` — успех, повтор (400), без JWT (401), превышение длины (400)
- [x] 8.3 `DELETE /api/v1/recipes/{id}/comments/{commentId}` — автор комментария, автор рецепта, чужой пользователь (403), без JWT (401)

## 9. Frontend — BFF

- [x] 9.1 Создать `apps/web/lib/bff/comments.ts` с функциями `getComments`, `addComment`, `deleteComment`
- [x] 9.2 Создать Zod-схемы `CommentDtoSchema`, `CommentRequestSchema`, `PagedResultCommentDtoSchema`
- [x] 9.3 Написать unit-тесты: `comments.schema.test.ts` (Zod-схемы) и `comments.bff.test.ts` (fetch mock)

## 10. Frontend — UI

- [x] 10.1 Реализовать компонент `CommentItem` по макету из `docs/design/mockup/` (стили `.comment`, `.comment-head`, `.avatar` из `styles.css`): аватар с инициалом, имя автора, дата, текст, кнопка удаления
- [x] 10.2 Реализовать секцию комментариев на детальной странице рецепта: список `CommentItem` + пагинация (компонент `Pagination` из Storybook `src/components/`)
- [x] 10.3 Добавить форму добавления комментария (textarea + кнопка «Отправить»); скрыта если пользователь уже оставил комментарий или не авторизован
- [x] 10.4 Интегрировать секцию комментариев в детальную страницу рецепта (`apps/web/app/recipes/[id]/page.tsx`)
- [x] 10.5 Добавить story `CommentItem` в Storybook (`docs/design/storybook/src/domain/`) с вариантами: обычный, с кнопкой удаления, без кнопки удаления

## 11. E2E API-тесты

- [x] 11.1 Создать `tests/e2e/test_comments_api.py`: добавление комментария, получение списка, удаление автором рецепта

## 12. UI E2E-тесты

- [x] 12.1 Создать `tests/ui/test_comments.py`: авторизованный пользователь добавляет комментарий на детальной странице рецепта — комментарий появляется в списке
- [x] 12.2 Добавить сценарий: авторизованный пользователь удаляет свой комментарий — комментарий исчезает из списка
