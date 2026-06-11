# recipes-crud

## 1. Архитектурные документы

- [x] 1.1 Создать новый ADR, superseding ADR-0018: «Чистый CSS из Storybook как система стилей фронтенда»
- [x] 1.2 Создать AR: «Доменная валидация через типизированные исключения; контроллер явно перехватывает и формирует Problem+JSON»
- [x] 1.3 Создать AR: «Один глобальный fallback IExceptionHandler для непредвиденных ошибок»
- [x] 1.4 Обновить ARCHITECTURE.md: добавить ссылки на новые ADR и AR

## 2. OpenAPI-контракт

- [x] 2.1 Расширить схему `RecipeDto` в `docs/contracts/cookbook/recipes.yaml`: добавить поля `cookingTime` (int, мин), `difficulty` (string enum), `servings` (int), `instructions` (string)
- [x] 2.2 Добавить схему `RecipeRequest` (переиспользуется для POST и PUT): поля `title`, `description`, `cookingTime`, `difficulty`, `servings`, `instructions`
- [x] 2.3 Добавить эндпоинт `POST /api/recipes/v1` → `201 Created` с `RecipeDto` в теле; `400` при ошибке валидации
- [x] 2.4 Добавить эндпоинт `GET /api/recipes/v1/{id}` → `200 OK` с `RecipeDto`; `400` при некорректном id
- [x] 2.5 Добавить эндпоинт `PUT /api/recipes/v1/{id}` → `204 No Content`; `400` при ошибке валидации
- [x] 2.6 Добавить эндпоинт `DELETE /api/recipes/v1/{id}` → `204 No Content`; `400` при некорректном id

## 3. Backend: доменная модель

- [x] 3.1 Расширить агрегат `Recipe`: добавить поля `CookingTime`, `Difficulty` (enum), `Servings`, `Instructions`
- [x] 3.2 Добавить enum `Difficulty` (`Easy`, `Everyday`, `Festive`, `Restaurant`, `Signature`)
- [x] 3.3 Настроить EF Core value converter для `Difficulty`: хранить как lowercase string, парсить без учёта регистра
- [x] 3.4 Реализовать фабричный метод `Recipe.Create(...)` с полной доменной валидацией
- [x] 3.5 Реализовать метод `Recipe.Update(...)` с полной доменной валидацией
- [x] 3.6 Создать типизированное исключение `RecipeDomainException` и специализированные подклассы
- [x] 3.7 Написать unit-тесты доменной валидации (позитивные и негативные сценарии)

## 4. Backend: Application и порты

- [x] 4.1 Добавить методы `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync` в `IRecipeRepository`
- [x] 4.2 Добавить методы `GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync` в `IRecipeService`
- [x] 4.3 Реализовать методы сервиса

## 5. Backend: инфраструктура и данные

- [x] 5.1 Создать EF Core миграцию для расширения таблицы `cookbook.recipes` новыми полями
- [x] 5.2 Реализовать методы репозитория (`GetByIdAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`)
- [x] 5.3 Заменить seed-данные: удалить старые записи, добавить новые с заполненными полями (10+ рецептов)

## 6. Backend: Web-адаптер

- [x] 6.1 Обновить `RecipeDto`: добавить новые поля (`cookingTime`, `difficulty`, `servings`, `instructions`)
- [x] 6.2 Добавить `RecipeRequest` DTO (используется для POST и PUT)
- [x] 6.3 Добавить эндпоинт `POST /api/recipes/v1` в контроллер
- [x] 6.4 Добавить эндпоинт `GET /api/recipes/v1/{id}` в контроллер
- [x] 6.5 Добавить эндпоинт `PUT /api/recipes/v1/{id}` в контроллер
- [x] 6.6 Добавить эндпоинт `DELETE /api/recipes/v1/{id}` в контроллер
- [x] 6.7 Реализовать явный перехват `RecipeDomainException` в контроллере → `400 Bad Request` с `application/problem+json`
- [x] 6.8 Зарегистрировать один глобальный fallback `IExceptionHandler` → `500 Internal Server Error`
- [x] 6.9 Написать интеграционные тесты CRUD-эндпоинтов (`WebApplicationFactory` + Testcontainers)

## 7. Frontend: схемы и BFF

- [x] 7.1 Расширить Zod-схему `RecipeDto` в `apps/web/lib/schemas/recipe.ts`: добавить `cookingTime`, `difficulty`, `servings`, `instructions`
- [x] 7.2 Добавить Zod-схему `RecipeRequestSchema` (переиспользуется для создания и редактирования)
- [x] 7.3 Добавить BFF-функции `getRecipe(id)`, `createRecipe(data)`, `updateRecipe(id, data)`, `deleteRecipe(id)` в `apps/web/lib/bff/gateway.ts`
- [x] 7.4 Написать unit-тесты Zod-схем (валидация корректных и некорректных данных)

## 8. Frontend: стили и компоненты

- [x] 8.1 Скопировать `docs/design/storybook/src/styles.css` в `apps/web/app/globals.css`
- [x] 8.2 Скопировать `docs/design/storybook/src/motion.css` в `apps/web/app/`; импортировать в `globals.css`
- [x] 8.3 Перенести вспомогательные модули: `icons.tsx`, `photo.tsx` из Storybook в `apps/web/components/`
- [x] 8.4 Перенести примитивы: `Button`, `Tag`, `SearchInput`, `Skeleton`, `Modal`, `Toast` из Storybook в `apps/web/components/ui/`
- [x] 8.5 Перенести `RecipeCard` из Storybook в `apps/web/components/features/`; адаптировать типы на Zod-схемы
- [x] 8.6 Обновить `apps/web/app/layout.tsx`: подключить `globals.css`, добавить шрифт Inter, структуру `.app`/`.header`/`.main`

## 9. Frontend: страницы

- [x] 9.1 Обновить главную страницу `/`: список рецептов с расширенными карточками по макету `docs/design/mockup/index.html`
- [x] 9.2 Реализовать страницу `/recipes/[id]`: детальная страница рецепта (RSC) по макету
- [x] 9.3 Реализовать страницу `/recipes/new`: форма создания рецепта с валидацией (Client Component)
- [x] 9.4 Реализовать страницу `/recipes/[id]/edit`: форма редактирования рецепта с предзаполнением (Client Component)
- [x] 9.5 Реализовать диалог подтверждения удаления рецепта

## 10. Тесты: e2e и UI

- [x] 10.1 Написать e2e/ui-тест: отображение списка рецептов с расширенными полями (сценарий из `recipe-list`)
- [x] 10.2 Написать e2e/ui-тест: переход на детальную страницу по клику на карточку (сценарий из `recipe-list`)
- [x] 10.3 Написать e2e/ui-тест: открытие детальной страницы рецепта (сценарий из `recipe-detail`)
- [x] 10.4 Написать e2e/ui-тест: возврат к списку с детальной страницы (сценарий из `recipe-detail`)
- [x] 10.5 Написать e2e/ui-тест: успешное создание рецепта (сценарий из `recipe-create`)
- [x] 10.6 Написать e2e/ui-тест: ошибка валидации при создании (сценарий из `recipe-create`)
- [x] 10.7 Написать e2e/ui-тест: успешное редактирование рецепта с предзаполнением (сценарии из `recipe-edit`)
- [x] 10.8 Написать e2e/ui-тест: успешное удаление рецепта после подтверждения (сценарий из `recipe-delete`)
- [x] 10.9 Написать e2e/ui-тест: отмена удаления рецепта (сценарий из `recipe-delete`)
