import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { SearchInput } from '../components/SearchInput';
import { categories, ingredients } from '../mocks';

const meta: Meta<typeof SearchInput> = {
  title: 'Primitives/SearchInput',
  component: SearchInput,
  parameters: { layout: 'centered' },
  decorators: [
    (Story) => (
      <div style={{ minWidth: 480 }}>
        <Story />
      </div>
    ),
  ],
};
export default meta;
type S = StoryObj<typeof SearchInput>;

export const Default: S = {
  name: 'Пустой',
  args: {
    placeholder: 'Найти рецепты, ингредиенты, категории…',
  },
};

export const WithValue: S = {
  name: 'С введённым значением',
  args: {
    defaultValue: 'борщ',
    placeholder: 'Найти рецепты, ингредиенты, категории…',
  },
};

export const WithSuggestions: S = {
  name: 'С подсказками (статические)',
  args: {
    defaultValue: 'ит',
    placeholder: 'Найти рецепты, ингредиенты, категории…',
    suggestions: [
      { label: 'Итальянская', kind: 'категория' },
      { label: 'Картофель', kind: 'ингредиент' },
      { label: 'Куриная грудка', kind: 'ингредиент' },
    ],
  },
};

export const Playground: S = {
  name: 'Playground (живое автодополнение)',
  render: () => {
    const [value, setValue] = React.useState('');

    const suggestions = React.useMemo(() => {
      const trimmed = value.trim().toLowerCase();
      if (trimmed.length < 2) return [];
      const last = trimmed.split(/\s+/).pop() ?? '';
      if (last.length < 2) return [];

      const catSuggestions = categories
        .filter((c) => c.name.toLowerCase().includes(last))
        .slice(0, 6)
        .map((c) => ({ label: c.name, kind: 'категория' }));

      const ingSuggestions = ingredients
        .filter((i) => i.title.toLowerCase().includes(last))
        .slice(0, 6)
        .map((i) => ({ label: i.title, kind: 'ингредиент' }));

      return [...catSuggestions, ...ingSuggestions];
    }, [value]);

    function handlePickSuggestion(label: string) {
      const words = value.trimEnd().split(/\s+/);
      words.pop();
      words.push(label);
      setValue(words.join(' ') + ' ');
    }

    return (
      <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
        <SearchInput
          value={value}
          onValueChange={setValue}
          placeholder="Найти рецепты, ингредиенты, категории…"
          suggestions={suggestions}
          onPickSuggestion={handlePickSuggestion}
          maxLength={300}
        />
        <p style={{ fontSize: 12, color: 'var(--c-text-2, #888)', margin: 0 }}>
          Введите ≥ 2 символа для автодополнения. Текущий запрос:{' '}
          <code>{value || '—'}</code>
        </p>
      </div>
    );
  },
};
