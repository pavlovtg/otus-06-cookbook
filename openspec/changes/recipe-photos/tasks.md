# Tasks: recipe-photos

## Инфраструктура

- [ ] **INFRA-1** Проверить nginx: убедиться, что образ собран с `ngx_cache_purge`; настроить `proxy_cache` для `/api/cookbook/photos/`; разрешить `PURGE` от внутренней сети Docker.

## Контракт

- [x] **CONTRACT-1** Обновить `docs/contracts/cookbook/recipes.yaml`: добавить `photoId: uuid | null` (required) в `RecipeDto` и `RecipeShortDto`.
- [x] **CONTRACT-2** Добавить в контракт endpoints: `POST /api/v1/recipes/{id}/photo`, `DELETE /api/v1/recipes/{id}/photo`, `GET /api/v1/photos/{photoId}`, `GET /api/v1/photos/{photoId}/thumbnail`.

## Backend — домен

- [x] **DOMAIN-1** Создать сущность `RecipePhoto` (`RecipePhotoId`, `RecipeId`, `OriginalData: byte[]`, `ThumbnailData: byte[]`).
- [x] **DOMAIN-2** Добавить nullable `RecipePhotoId` в агрегат `Recipe`; обновить фабричный метод и метод обновления.
- [x] **DOMAIN-3** Добавить валидацию в `RecipePhoto.Create` через `RecipePhotoValidator`: допустимые MIME (`image/jpeg`, `image/png`), максимальный размер 10 МБ.
- [x] **DOMAIN-4** Объявить `IRecipePhotoRepository` с методами: `SaveAsync`, `GetOriginalAsync` (только `original_data`), `GetThumbnailAsync` (только `thumbnail_data`), `DeleteAsync`.

## Backend — инфраструктура

- [x] **INFRA-2** Добавить `RecipePhotoConfiguration` (EF Core `IEntityTypeConfiguration`): таблица `recipe_photos`, CASCADE DELETE.
- [x] **INFRA-3** Создать EF Core миграцию: таблица `recipe_photos`, FK `recipes.photo_id`.
- [x] **INFRA-4** Реализовать `IRecipePhotoRepository` в `RecipeRepository`: `GetOriginalAsync` — `SELECT original_data`, `GetThumbnailAsync` — `SELECT thumbnail_data`.
- [x] **INFRA-5** Добавить `SixLabors.ImageSharp` 3.1.12 в зависимости Cookbook-сервиса.

## Backend — Application

- [x] **APP-1** Реализовать `RecipePhotoService.UploadAsync`: fail-fast валидация MIME, материализация потока, проверка размера, генерация thumbnail через `ImageSharpThumbnailGenerator`, сохранение, обновление `Recipe.PhotoId`.
- [x] **APP-2** Реализовать `RecipePhotoService.DeleteAsync`: удалить фото через репозиторий, обнулить `Recipe.PhotoId`.
- [x] **APP-3** Обновить маппинг `Recipe → RecipeDto` и `Recipe → RecipeShortDto`: добавить `photoId = recipe.PhotoId?.Value`.

## Backend — API

- [x] **API-1** Добавить `PhotosController`: `GET /api/v1/photos/{photoId}` (original), `GET /api/v1/photos/{photoId}/thumbnail` — публичные, без JWT, `Cache-Control: public, max-age=86400`.
- [x] **API-2** Расширить `RecipesController`: `POST /api/v1/recipes/{id}/photo` (multipart), `DELETE /api/v1/recipes/{id}/photo`.

## Seed

- [ ] **SEED-1** Добавить директорию `apps/seed/photos/` с JPEG-заглушками для 25+ рецептов.
- [ ] **SEED-2** Расширить `CookbookSeeder`: загружать фото из `apps/seed/photos/`, генерировать thumbnail, сохранять в `recipe_photos`, обновлять `photo_id` у рецептов. Операция идемпотентна.

## Frontend — схемы и BFF

- [ ] **FE-1** Обновить Zod-схемы `RecipeDto` и `RecipeShortDto`: добавить `photoId: z.string().uuid().nullable()`.
- [ ] **FE-2** Добавить BFF-функции: `uploadRecipePhoto(recipeId, file)`, `deleteRecipePhoto(recipeId)`.
- [ ] **FE-3** После успешного `uploadRecipePhoto` и `deleteRecipePhoto` отправлять PURGE-запрос из BFF в nginx на URL фото и thumbnail.
- [ ] **FE-4** Добавить утилиту `getRecipePhotoUrl(photoId)` и `getRecipeThumbnailUrl(photoId)` — формируют URL `/api/cookbook/photos/{photoId}` и `/api/cookbook/photos/{photoId}/thumbnail`.

## Frontend — UI

- [ ] **UI-1** Обновить компонент `RecipeCard`: если `photoId != null` — рендерить `<img src={thumbnailUrl}>`, иначе — заглушку (SVG-градиент из `photo.tsx`).
- [ ] **UI-2** Обновить детальную страницу рецепта: если `photoId != null` — рендерить `<img src={photoUrl}>`, иначе — заглушку.
- [ ] **UI-3** Добавить на детальную страницу кнопки управления фото (видны только автору и admin): «Загрузить фото» (если `photoId = null`), «Заменить фото» и «Удалить фото» (если `photoId != null`).
- [ ] **UI-4** Добавить Storybook-истории: `RecipeCard` с фото и без; блок управления фото на детальной странице.

## Тесты

- [x] **TEST-1** Unit (backend): `RecipePhoto.Create` — валидация MIME и размера.
- [ ] **TEST-2** Unit (backend): `RecipePhotoService` — покрыто микросервисными тестами.
- [ ] **TEST-3** Unit (frontend): Zod-схемы с `photoId`; `getRecipePhotoUrl` / `getRecipeThumbnailUrl`.
- [ ] **TEST-4** Integration (DB): сохранение фото; `GetOriginalAsync` не загружает `thumbnail_data`; `GetThumbnailAsync` не загружает `original_data`; CASCADE DELETE при удалении рецепта.
- [x] **TEST-5** Microservice: `POST /photo` → `GET /photos/{id}` возвращает фото; `DELETE /photo` → `photoId = null`; недопустимый формат → `400`; фото не найдено → `400`.
- [ ] **TEST-6** E2E API: загрузка фото, `photoId != null` в ответе; удаление фото, `photoId = null`.
- [ ] **TEST-7** UI E2E: карточка рецепта с фото рендерит `<img>`; кнопки управления фото видны автору и скрыты для гостя.
