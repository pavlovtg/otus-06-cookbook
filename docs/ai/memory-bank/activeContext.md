# Active Context

## Текущая задача

Реализованы seed-данные с фотографиями рецептов.

## Что было сделано в последней сессии

- Переписан `scripts/seed-photos.py`: поиск на `povar.ru/xmlsearch?query=`, скачивание 60 фото (jpeg/png)
- Скачаны фото для всех 60 рецептов в `apps/seed/photos/`
- Обновлён `scripts/seed-photos-result.json` с маппингом RecipeId → PhotoId
- Добавлен `SeedData.RecipePhotoSeeds` — массив `(RecipeId, RecipePhotoId)[]` для 60 рецептов
- Обновлён `CookbookSeeder.SeedPhotosAsync`: конвертация webp/неизвестных форматов в JPEG через ImageSharp
- Добавлен `ImageSharpThumbnailGenerator.ConvertToJpeg()`
- `FindPhotoFile` теперь ищет `.jpg`, `.jpeg`, `.png`, `.webp`

## Ключевые решения

- Фото: PNG/JPEG, ≤ 10 МБ, 1 фото на рецепт для MVP
- Thumbnail: 400×300 px, crop center, ImageSharp
- API: `POST /api/v1/recipes/{recipeId}/photos` (JWT), `GET /api/v1/recipes/{recipeId}/photos/{photoId}` (публично)
- Seed-фото: `apps/seed/photos/` — 60 файлов `.jpeg`/`.png`
