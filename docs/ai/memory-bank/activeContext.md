# Active Context

## Текущая задача

Завершён архитектурный анализ хранения фотографий рецептов. Принято решение, созданы ADR и AR.

## Что было сделано в последней сессии

- Проведён итеративный сбор требований (блоки A–D) по теме фотографий рецептов
- Рассмотрены 4 варианта хранения: FS+nginx, PostgreSQL bytea, MinIO, FS+backend API
- Принято решение: **PostgreSQL bytea** (Вариант B)
- Создан `ADR-0034`: хранение фото в PostgreSQL (bytea), без Docker volume
- Создан `AR-0059`: таблица `recipe_photos`, CASCADE DELETE, изоляция
- Создан `AR-0060`: серверная валидация MIME/размера, thumbnail 400×300 через ImageSharp
- Создан `AR-0061`: публичный endpoint раздачи фото, `Cache-Control: public, max-age=86400`
- Создан `AR-0062`: seed-фото из `apps/seed/photos/` через `CookbookSeeder`, идемпотентно
- Обновлён `ARCHITECTURE.md`: добавлены ADR-0034 и AR-0059..0062 в индексы

## Ключевые решения

- Фото: PNG/JPEG, ≤ 10 МБ, 1 фото на рецепт для MVP
- Thumbnail: 400×300 px, crop center, ImageSharp
- API: `POST /api/v1/recipes/{recipeId}/photos` (JWT), `GET /api/v1/recipes/{recipeId}/photos/{photoId}` (публично)
- Seed-фото: `apps/seed/photos/`
