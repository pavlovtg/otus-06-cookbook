# Active Context

## Текущая задача

`categories-crud` полностью завершён (все 10 разделов). Все задачи выполнены.

## Что было сделано в последней сессии (categories-crud frontend)

- **5.1** `lib/schemas/category.ts` — Zod-схемы `CategorySchema`, `CategoryRequestSchema`, enum `CategoryType` (7 значений), `CategoryTypeLabels`
- **5.2** `lib/bff/categories.ts` — `getCategories`, `createCategory`, `updateCategory`, `deleteCategory`
- **5.2** API routes: `app/api/cookbook/v1/categories/route.ts` (GET, POST) и `[id]/route.ts` (PUT, DELETE) — proxy к gateway
- **6.1–6.3** `app/categories/page.tsx` — Server Component, 7 групп по типу, теги с кнопками; `CategoryModal.tsx` — модальная форма создания/редактирования; `DeleteCategoryButton.tsx` — подтверждение удаления с обработкой 409
- **6.4** Навигация: добавлен пункт «Категории» в `app/layout.tsx`
- **7.1** `docs/design/storybook/src/stories/Categories.stories.tsx` — 4 story: Page, Playground ★, EmptyGroup, SingleTag
- **9.1** `tests/unit/category.schema.test.ts` — 20 тестов Zod-схем
- **9.2** `tests/unit/category.bff.test.ts` — 10 тестов BFF с мокированным fetch

## Что было сделано ранее (bugfix 2)

- `/api/cookbook/photos/` → `/api/cookbook/v1/photos/` везде

## Ключевые решения

- Тип категории нельзя менять при редактировании (select disabled)
- Удаление: 409 → показываем сообщение «используется в рецептах» без кнопки «Удалить»
- Страница: `force-dynamic`, Server Component, refresh через `window.location.assign`
- Proxy routes: 204 → `new NextResponse(null, { status: 204 })`
