import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { Segmented, Tabs } from '../components/Segmented';
import { Pagination } from '../components/Pagination';

const meta: Meta = { title: 'Primitives/Navigation', parameters: { layout: 'centered' } };
export default meta;
type S = StoryObj;

export const SegmentedBasic: S = {
  render: () => (
    <Segmented
      defaultValue="all"
      options={[
        { value: 'all', label: 'Все' },
        { value: 'favorites', label: 'Избранное' },
        { value: 'mine', label: 'Мои' },
      ]}
    />
  ),
};

export const TabsBasic: S = {
  render: () => (
    <Tabs
      defaultValue="recipes"
      options={[
        { value: 'recipes', label: 'Рецепты' },
        { value: 'planner', label: 'Планировщик' },
        { value: 'shopping', label: 'Покупки' },
      ]}
    />
  ),
};

export const PaginationBasic: S = { render: () => <Pagination total={7} defaultPage={3} /> };

export const PaginationLong: S = { render: () => <Pagination total={20} defaultPage={6} /> };

export const Playground: S = {
  render: () => (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 20 }}>
      <Segmented options={[{ value: 'all', label: 'Все' }, { value: 'fav', label: 'Избранное' }]} />
      <Tabs options={[{ value: 'a', label: 'Tab A' }, { value: 'b', label: 'Tab B' }, { value: 'c', label: 'Tab C' }]} />
      <Pagination total={9} />
    </div>
  ),
};
