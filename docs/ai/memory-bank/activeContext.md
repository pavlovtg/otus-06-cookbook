# Active Context

## Текущая задача

Завершена реализация change `recipe-photos` — все 27 задач выполнены.

## Что было сделано в последней сессии

- **INFRA-1**: nginx собран с `ngx_cache_purge`; настроен `proxy_cache` для `/api/cookbook/photos/`; разрешён `PURGE` от `172.0.0.0/8`
- **FE-1**: Zod-схемы `RecipeDto` и `RecipeShortDto` — добавлен `photoId: z.string().uuid().nullable()`
- **FE-2**: BFF-функции `uploadRecipePhoto(recipeId, file)` и `deleteRecipePhoto(recipeId, photoId)` в `lib/bff/recipes.ts`
- **FE-3**: PURGE-запросы в nginx после upload/delete через `lib/bff/photos.ts`
- **FE-4**: Утилиты `getRecipePhotoUrl` и `getRecipeThumbnailUrl` в `lib/bff/photos.ts`
- **UI-1**: `RecipeCard` — `<img>` если `photoId != null`, SVG-заглушка иначе
- **UI-2**: Детальная страница рецепта — фото или заглушка
- **UI-3**: `RecipePhotoActions` — кнопки «Загрузить/Заменить/Удалить фото»
- **UI-4**: Storybook-истории `RecipeCard.stories.tsx` и `RecipePhotoActions.stories.tsx`; добавлен `photo_url?` в тип `Recipe` в `mocks.ts`
- **TEST-3**: Unit-тесты frontend — `photos.test.ts`, обновлены `recipe.schema.test.ts`, `recipe.bff.test.ts`, `RecipeCard.test.tsx`
- **TEST-4**: Integration DB тесты — добавлены в `RecipeRepositoryTests.cs` (SaveAsync, GetOriginalAsync, GetThumbnailAsync, CASCADE DELETE)
- **TEST-6**: E2E API тесты — добавлены в `test_recipes_api.py`
- **TEST-7**: UI E2E тесты — добавлены в `test_recipes.py`

## Ключевые решения

- Фото: PNG/JPEG, ≤ 10 МБ, 1 фото на рецепт для MVP
- Thumbnail: 400×300 px, crop center, ImageSharp
- PURGE: BFF отправляет `PURGE` в nginx напрямую через Docker-сеть (`http://reverse-proxy/...`)
- Seed-фото: `apps/seed/photos/` — 60 файлов `.jpeg`/`.png`
