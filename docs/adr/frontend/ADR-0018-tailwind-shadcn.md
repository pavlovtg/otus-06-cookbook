# ADR-0018: Tailwind CSS + shadcn/ui как система стилей и компонентов

- **Статус**: принят
- **Домен**: frontend
- **Дата**: 2026-06-07

## Контекст

Frontend требует:

- Адаптивную вёрстку (desktop + mobile).
- Готовый набор доступных компонентов (формы, диалоги, таблицы, кнопки, dropdown'ы) для ускорения MVP за 1 неделю.
- Минимальный bundle (NFR: initial JS ≤ 200 KB gzip).
- AI-friendliness (главный критерий выбора).

## Рассмотренные варианты

- **Tailwind CSS + shadcn/ui** — utility-first CSS + копируемые в проект React-компоненты на Radix UI; огромный AI-датасет, минимальный runtime overhead, полный контроль над кодом компонентов.
- **CSS Modules + собственная библиотека компонентов** — больше ручной работы, не вписывается в срок.
- **CSS-in-JS (Emotion, styled-components)** — runtime cost, плохо стыкуется с React Server Components.
- **Готовая UI-библиотека (MUI, Chakra)** — больший bundle, меньше гибкости, runtime cost.

## Решение

Стилизация — **Tailwind CSS**. Базовая библиотека компонентов — **shadcn/ui** (компоненты копируются в проект и редактируются под нужды).

## Последствия

- Компоненты shadcn/ui хранятся в репозитории как часть кода (а не как зависимость), их можно менять.
- Tailwind конфигурируется в `tailwind.config.ts`; кастомные токены дизайн-системы — в этом же файле.
- Совместимость с React Server Components — нативная (Tailwind работает на этапе сборки).
- CSS-in-JS-библиотеки запрещены без отдельного нового ADR.

## Связанные документы

- [ADR-0015: Next.js как frontend meta-framework](ADR-0015-nextjs-frontend-meta-framework.md)
- [ADR-0016: React + TypeScript как UI-стек frontend](ADR-0016-react-typescript-frontend.md)
- [Стандарт структуры frontend-проекта](../../standards/frontend-project-structure.md)
