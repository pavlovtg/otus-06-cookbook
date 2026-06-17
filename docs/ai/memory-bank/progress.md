# Прогресс

## Статус проекта

В разработке.

## Завершённые фичи

- Список рецептов (главная страница)
- Детальная страница рецепта
- Создание рецепта
- Редактирование рецепта
- Удаление рецепта
- CRUD ингредиентов (backend + тесты + frontend схемы/BFF + E2E тесты)
- `recipe-ingredients` — все 56 задач (секции 1–8) полностью реализованы
- Авторизация (`user-auth`) — JWT, BCrypt, iron-session, BFF, middleware, UI (login/register/user-chip)
- `recipe-author` (секции 1–4, 7.1–7.3) — backend: домен, миграция, фильтрация, 403, контракт, контроллер
- `recipe-author` (секции 5, 6, 7.4) — frontend: Zod-схемы, BFF, форма (чекбокс isPublic), RecipeCard (author-inline, photo-private), детальная страница (authorName, tag-private, 403), Storybook, E2E тесты

## В работе

`user-favorites` — все секции 1–9 выполнены (59/59 задач).

## Заархивировано

- `recipe-categories` (31/31 задач) → `openspec/changes/archive/2026-06-16-recipe-categories/`
- `user-auth` (76/76 задач) → `openspec/changes/archive/2026-06-17-user-auth/`
- `recipe-author` (29/29 задач) → `openspec/changes/archive/2026-06-17-recipe-author/`

## Выполнено (последнее — recipe-author frontend)

- Zod: `isPublic: z.boolean()` и `authorName: z.string().nullable()` в `RecipeShortDtoSchema`, `RecipeDtoSchema`; `isPublic` в `RecipeRequestSchema`
- BFF: `isPublic` передаётся автоматически через `RecipeRequestSchema.parse(data)`
- `RecipeForm.tsx`: чекбокс `<label class="checkbox">` с `name="is_public"`, предзаполнение в edit-форме
- `RecipeCard.tsx`: `.photo-private` + `LockIcon` при `!isPublic`; `.author-inline` + `UserIcon` + `authorName` в `.footer`
- `recipes/[id]/page.tsx`: `authorName` + `UserIcon` и `tag-private` + `LockIcon` в `.detail-bar .meta`; обработка 403 с UI-сообщением
- Storybook: stories `Private` (переименована), `WithAuthor`, `PrivateWithAuthor`
- E2E: `tests/e2e/test_recipe_visibility_api.py` — 10 сценариев (публичный/приватный, анонимный/автор/другой пользователь, поля isPublic/authorName)
- Unit-тесты: `validDto`, `validShortDto`, `validRequest` обновлены; добавлены тесты `isPublic = false` и `без isPublic`

## Выполнено (ранее)

- Багфикс `RecipesController`: `CategoryDomainException` не перехватывалась → добавлен catch
- Багфикс unit-тестов фронтенда: фикстуры без `categoryIds` → добавлен `categoryIds: []`
- Багфикс e2e-тестов: `VALID_RECIPE` без `categoryIds` → добавлен `"categoryIds": []`
- Багфикс fallback-тегов: убран fallback из `RecipeCard.tsx` и `page.tsx`
- Фикс UI-тестов ингредиентов: BFF-роуты, `router.push`, `force-dynamic`, SSR/CSR base URL
- Рефакторинг `CookbookSeeder`, UI-тесты фото
