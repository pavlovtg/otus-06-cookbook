import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { Input, Textarea, Select, Checkbox, Field } from '../components/Input';
import { SearchInput } from '../components/SearchInput';
import { Tag, Chip } from '../components/Tag';

const meta: Meta = { title: 'Primitives/Forms' };
export default meta;
type S = StoryObj;

export const InputBasic: S = {
  render: () => (
    <Field label="Название">
      <Input placeholder="Карбонара" />
    </Field>
  ),
};

export const Search: S = {
  render: () => (
    <div style={{ maxWidth: 480 }}>
      <SearchInput
        placeholder="Найти рецепты, ингредиенты, категории…"
        suggestions={[
          { label: 'Карбонара', kind: 'рецепт' },
          { label: 'Помидор', kind: 'ингредиент' },
          { label: 'Итальянская', kind: 'категория' },
        ]}
      />
    </div>
  ),
};

export const TagsAndChips: S = {
  render: () => (
    <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
      <Tag>Итальянская</Tag>
      <Tag tone="accent">пользовательский</Tag>
      <Tag tone="private">Приватный</Tag>
      <Chip onRemove={() => {}}>Завтраки</Chip>
      <Chip onRemove={() => {}}>Вегетарианские</Chip>
    </div>
  ),
};

export const Playground: S = {
  render: () => (
    <form style={{ maxWidth: 480, display: 'flex', flexDirection: 'column', gap: 14 }} onSubmit={(e) => e.preventDefault()}>
      <Field label="Название"><Input defaultValue="Карбонара" /></Field>
      <Field label="Описание" hint="Краткое описание блюда"><Textarea defaultValue="Классическая римская паста" /></Field>
      <div className="field-row">
        <Field label="Время (мин)"><Input type="number" defaultValue={20} /></Field>
        <Field label="Порции"><Input type="number" defaultValue={4} /></Field>
      </div>
      <Field label="Сложность">
        <Select defaultValue="everyday">
          <option value="easy">Простой</option>
          <option value="everyday">Повседневный</option>
          <option value="festive">Праздничный</option>
        </Select>
      </Field>
      <Checkbox defaultChecked label="Публичный (виден всем)" />
      <Field label="С ошибкой" error="От 1 до 2000 символов"><Input defaultValue="" /></Field>
    </form>
  ),
};
