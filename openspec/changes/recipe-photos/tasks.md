# Tasks: recipe-photos

## Инфраструктура

- [ ] **INFRA-1** Проверить nginx: убедиться, что образ собран с `ngx_cache_purge`; настроить `proxy_cache` для `/api/cookbook/photos/`; разрешить `PURGE` от внутренней сети Docker.

## Контракт

- [ ] **CONTRACT-1** Обновить `docs/contracts/cookbook/recipes.yaml`: добавить `hasPhoto: boolean` в `RecipeDto` и `RecipeShortDto`.
- [ ] **CONTRACT-2** Добавить в контракт endpoints: `POST /api/v1/recipes/{id}/photo`, `DELETE /api/v1/recipes/{id}/photo`, `GET /api/v1/photos/{photoId}`, `GET /api/v1/photos/{photoId}/thumbnail`.

## Backend — домен

- [ ] **DOMAIN-1** Создать сущность `RecipePhoto` (`RecipePhotoId`, `RecipeId`, `OriginalData: byte[]`, `ThumbnailData: byte[]`).
- [ ] **DOMAIN-2** Добавить nullable `RecipePhotoId` в агрегат `Recipe`; обновить фабричный метод и метод обновления.
- [ ] **DOMAIN-3** Добавить валидацию в `RecipePhoto.Create`: допустимые MIME (`image/jpeg`, `image/png`), максимальный размер 10 МБ.
- [ ] **DOMAIN-4** Объявить `IRecipePhotoRepository` с методами: `SaveAsync`, `GetOriginalAsync` (только `original_data`), `GetThumbnailAsync` (только `thumbnail_data`), `DeleteAsync`.

## Backend — инфраструктура

- [ ] **INFRA-2** Добавить `RecipePhotoConfiguration` (EF Core `IEntityTypeConfiguration`): таблица `recipe_photos`, CASCADE DELETE.
- [ ] **INFRA-3** Создать EF Core миграцию: таблица `recipe_photos`, FK `recipes.photo_id`.
- [ ] **INFRA-4** Реализовать `RecipePhotoRepository`: `GetOriginalAsync` — `SELECT original_data`, `GetThumbnailAsync` — `SELECT thumbnail_data`.
- [ ] **INFRA-5** Добавить `SixLabors.ImageSharp` в зависимости Cookbook-сервиса.

## Backend — Application

- [ ] **APP-1** Реализовать `UploadRecipePhotoCommand` / handler: принять `IFormFile`, сгенерировать thumbnail 400×300 px (crop center) через ImageSharp, сохранить через репозиторий, обновить `Recipe.PhotoId`.
- [ ] **APP-2** Реализовать `DeleteRecipePhotoCommand` / handler: удалить фото через репозиторий, обнулить `Recipe.PhotoId`.
- [ ] **APP-3** Обновить маппинг `Recipe → RecipeDto` и `Recipe → RecipeShortDto`: добавить `hasPhoto = recipe.PhotoId != null`.

## Backend — API

- [ ] **API-1** Добавить `PhotosController`: `GET /api/v1/photos/{photoId}` (original), `GET /api/v1/photos/{photoId}/thumbnail` — публичные, без JWT, `Cache-Control: public, max-age=86400`.
- [ ] **API-2** Расширить `RecipesController`: `POST /api/v1/recipes/{id}/photo` (multipart), `DELETE /api/v1/recipes/{id}/photo` — требуют JWT, только автор или admin.

## Seed

- [ ] **SEED-1** Добавить директорию `apps/seed/photos/` с JPEG-заглушками для 25+ рецептов.
- [ ] **SEED-2** Расширить `CookbookSeeder`: загружать фото из `apps/seed/photos/`, генерировать thumbnail, сохранять в `recipe_photos`, обновлять `photo_id` у рецептов. Операция идемпотентна.

## Frontend — схемы и BFF

- [ ] **FE-1** Обновить Zod-схемы `RecipeDto` и `RecipeShortDto`: добавить `hasPhoto: z.boolean()`.
- [ ] **FE-2** Добавить BFF-функции: `uploadRecipePhoto(recipeId, file)`, `deleteRecipePhoto(recipeId)`.
- [ ] **FE-3** После успешного `uploadRecipePhoto` и `deleteRecipePhoto` отправлять PURGE-запрос из BFF в nginx на URL фото и thumbnail.
- [ ] **FE-4** Добавить утилиту `getRecipePhotoUrl(recipeId)` и `getRecipeThumbnailUrl(recipeId)` — формируют URL по `recipeId`.

## Frontend — UI

- [ ] **UI-1** Обновить компонент `RecipeCard`: если `hasPhoto = true` — рендерить `<img src={thumbnailUrl}>`, иначе — заглушку (SVG-градиент из `photo.tsx`).
- [ ] **UI-2** Обновить детальную страницу рецепта: если `hasPhoto = true` — рендерить `<img src={photoUrl}>`, иначе — заглушку.
- [ ] **UI-3** Добавить на детальную страницу кнопки управления фото (видны только автору и admin): «Загрузить фото» (если `hasPhoto = false`), «Заменить фото» и «Удалить фото» (если `hasPhoto = true`).
- [ ] **UI-4** Добавить Storybook-истории: `RecipeCard` с фото и без; блок управления фото на детальной странице.

## Тесты

- [ ] **TEST-1** Unit (backend): `RecipePhoto.Create` — валидация MIME и размера.
- [ ] **TEST-2** Unit (backend): `RecipePhotoService` — логика замены фото (старое удаляется, новое сохраняется).
- [ ] **TEST-3** Unit (frontend): Zod-схемы с `hasPhoto`; `getRecipePhotoUrl` / `getRecipeThumbnailUrl`.
- [ ] **TEST-4** Integration (DB): сохранение фото; `GetOriginalAsync` не загружает `thumbnail_data`; `GetThumbnailAsync` не загружает `original_data`; CASCADE DELETE при удалении рецепта.
- [ ] **TEST-5** Microservice: `POST /photo` → `GET /photos/{id}` возвращает фото; `DELETE /photo` → `hasPhoto = false`; недопустимый формат → `400`; превышение размера → `400`.
- [ ] **TEST-6** E2E API: загрузка фото, `hasPhoto = true` в ответе; удаление фото, `hasPhoto = false`.
- [ ] **TEST-7** UI E2E: карточка рецепта с фото рендерит `<img>`; кнопки управления фото видны автору и скрыты для гостя.
