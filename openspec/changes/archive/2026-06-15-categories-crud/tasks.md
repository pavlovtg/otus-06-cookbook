# categories-crud

## 1. OpenAPI-контракт

- [x] 1.1 Добавить эндпоинты категорий (`GET /api/v1/categories`, `POST /api/v1/categories`, `PUT /api/v1/categories/{id}`, `DELETE /api/v1/categories/{id}`) в контракт-файл сервиса `recipes` (`docs/contracts/cookbook/`)

## 2. Backend: домен и приложение

- [x] 2.1 Создать агрегат `Category` с полями `id`, `name`, `description`, `type` в `apps/Cookbook/src/Recipes/Domain/`
- [x] 2.2 Создать enum `CategoryType` (7 значений) и класс `CategoryConstraints` (лимит 1000) в `Domain/`
- [x] 2.3 Создать доменные исключения `CategoryInUseException`, `CategoryLimitExceededException`
- [x] 2.4 Создать порты `ICategoryRepository`, `ICategoryService` в `Application/Ports/`
- [x] 2.5 Реализовать `CategoryService` с методами `GetCategoriesAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync` (включая проверки лимита и использования)

## 3. Backend: инфраструктура

- [x] 3.1 Создать EF Core-конфигурацию и миграцию для таблицы `categories`
- [x] 3.2 Реализовать `CategoryRepository` в `Adapters/Postgresql/`
- [x] 3.3 Добавить `CategoriesController` в `Adapters/Web/` по паттерну `IngredientsController`
- [x] 3.4 Зарегистрировать зависимости в DI-контейнере

## 4. Seed-данные

- [x] 4.1 Добавить ~60 категорий по 7 типам в `CookbookSeeder` с upsert по `name + type`

## 5. Frontend: схема и BFF

- [x] 5.1 Создать Zod-схему `lib/schemas/category.ts` (поля `id`, `name`, `description`, `type`)
- [x] 5.2 Создать BFF-файл `lib/bff/categories.ts` с функциями `getCategories`, `createCategory`, `updateCategory`, `deleteCategory`

## 6. Frontend: страница

- [x] 6.1 Создать страницу `app/categories/page.tsx` — Server Component, загружает список категорий
- [x] 6.2 Реализовать отображение: 7 карточек по типу, теги с кнопками редактирования/удаления (по макету `views.categories`)
- [x] 6.3 Реализовать модальную форму создания/редактирования категории
- [x] 6.4 Добавить пункт «Категории» в навигацию

## 7. Storybook

- [x] 7.1 Добавить story для страницы/компонента категорий в `docs/design/storybook/`

## 8. Тесты backend

- [x] 8.1 Unit-тесты доменной логики `Category`: валидация полей, `CategoryConstraints`, исключения
- [x] 8.2 Integration DB-тесты `CategoryRepository`: CRUD, upsert, ограничения
- [x] 8.3 Microservice-тесты `CategoriesController`: все эндпоинты, лимит 1000, удаление используемой категории

## 9. Тесты frontend

- [x] 9.1 Unit-тесты Zod-схемы: `category.schema.test.ts`
- [x] 9.2 Unit-тесты BFF: `category.bff.test.ts` с мокированным fetch

## 10. E2E-тесты

- [x] 10.1 API E2E (`tests/e2e/`): list, create, edit, delete
- [x] 10.2 UI E2E (`tests/ui/`): отображение страницы, создание, редактирование, удаление категории
