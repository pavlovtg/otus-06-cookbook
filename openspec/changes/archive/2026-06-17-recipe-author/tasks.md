# recipe-author

## 1. Домен и база данных

- [x] 1.1 Добавить поля `is_public` (bool, not null) и `author_id` (uuid, nullable) в доменную модель `Recipe`
- [x] 1.2 Добавить миграцию EF Core: колонки `is_public` (default `true`) и `author_id` в таблицу `recipes`
- [x] 1.3 Обновить `IEntityTypeConfiguration<Recipe>` для новых полей
- [x] 1.4 Обновить seed-данные: проставить `is_public = true` и `author_id` для всех существующих рецептов

## 2. Application и фильтрация

- [x] 2.1 Обновить команду создания рецепта: принимать `isPublic`, записывать `author_id` из JWT-токена
- [x] 2.2 Обновить команду редактирования рецепта: принимать `isPublic`
- [x] 2.3 Добавить фильтрацию в запрос списка рецептов: скрывать приватные рецепты от посторонних
- [x] 2.4 Добавить проверку доступа в запрос детального рецепта: возвращать `403` для чужого приватного рецепта

## 3. OpenAPI-контракт

- [x] 3.1 Добавить поле `isPublic` (bool, required) в схему `RecipeRequest`
- [x] 3.2 Добавить поля `isPublic` (bool) и `authorName` (string, nullable) в схемы `RecipeShortDto` и `RecipeDto`
- [x] 3.3 Добавить ответ `403` в эндпоинт `GET /api/v1/recipes/{id}`

## 4. Web-адаптер (контроллер)

- [x] 4.1 Обновить маппинг `RecipeRequest` → команда создания: передавать `isPublic`
- [x] 4.2 Обновить маппинг `RecipeRequest` → команда редактирования: передавать `isPublic`
- [x] 4.3 Обновить маппинг `Recipe` → `RecipeShortDto` и `RecipeDto`: добавить `isPublic` и `authorName` (JOIN с пользователями)

## 5. Frontend — BFF и Zod-схемы

- [x] 5.1 Обновить Zod-схему рецепта: добавить `isPublic` (boolean) и `authorName` (string nullable)
- [x] 5.2 Обновить BFF `apps/web/lib/bff/recipes.ts`: передавать `isPublic` при создании и редактировании
- [x] 5.3 Обновить unit-тесты Zod-схем (`recipes.schema.test.ts`) и BFF (`recipes.bff.test.ts`)

## 6. Frontend — UI (макет: `docs/design/mockup/`, компоненты: `docs/design/storybook/src/domain/RecipeCard`)

- [x] 6.1 Форма создания рецепта: добавить чекбокс «Публичный (виден всем)» (`name="is_public"`) — по образцу макета (`<label class="checkbox">`)
- [x] 6.2 Форма редактирования рецепта: добавить тот же чекбокс с предзаполнением текущего значения `isPublic`
- [x] 6.3 Карточка рецепта (`RecipeCard`): добавить `.author-inline` с иконкой `user` и `authorName` в `.recipe-card .footer` — по образцу макета
- [x] 6.4 Карточка рецепта: добавить `.photo-private` (иконка замка `lock`, класс `tag-private`) поверх фото при `isPublic = false` — по образцу макета
- [x] 6.5 Детальная страница: добавить `authorName` в `.detail-bar .meta` (иконка `user`) — по образцу макета
- [x] 6.6 Детальная страница: добавить тег `tag-private` с иконкой `lock` и текстом «Приватный» в `.detail-bar .meta` при `isPublic = false` — по образцу макета
- [x] 6.7 Обработать `403` на детальной странице: показывать сообщение об ошибке доступа
- [x] 6.8 Обновить Storybook: добавить story/вариант для `RecipeCard` с `isPublic = false` (тег «Приватный», иконка замка) и с `authorName`

## 7. Тесты

- [x] 7.1 Unit-тесты доменной модели: создание рецепта с `is_public` и `author_id`
- [x] 7.2 Integration-тесты репозитория: фильтрация приватных рецептов
- [x] 7.3 Microservice-тесты: `GET /api/v1/recipes` скрывает приватные рецепты; `GET /api/v1/recipes/{id}` возвращает `403` для чужого приватного
- [x] 7.4 E2E API-тесты: сценарии из specs `recipe-visibility`
