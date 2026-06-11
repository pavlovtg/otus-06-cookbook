import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { Button, IconButton, AsyncButton } from '../components/Button';
import { PlusIcon, TrashIcon, HeartIcon, HeartFillIcon, EditIcon, SearchIcon } from '../icons';

const meta: Meta<typeof Button> = {
  title: 'Primitives/Button',
  component: Button,
  args: { children: 'Кнопка', variant: 'primary', size: 'md' },
};
export default meta;
type S = StoryObj<typeof Button>;

export const Primary: S = { args: { variant: 'primary' } };
export const Ghost: S = { args: { variant: 'ghost' } };
export const Danger: S = { args: { variant: 'danger', children: 'Удалить' } };
export const WithIcon: S = { args: { variant: 'primary', icon: <PlusIcon size={14} />, children: 'Новый рецепт' } };
export const Small: S = { args: { variant: 'ghost', size: 'sm', children: 'Маленькая' } };

export const Playground: S = {
  render: () => (
    <div style={{ display: 'flex', flexDirection: 'column', gap: 16, maxWidth: 720 }}>
      <div style={{ display: 'flex', gap: 10, flexWrap: 'wrap' }}>
        <Button variant="primary">Primary</Button>
        <Button variant="primary" size="sm">Primary sm</Button>
        <Button variant="ghost">Ghost</Button>
        <Button variant="ghost" size="sm">Ghost sm</Button>
        <Button variant="danger">Danger</Button>
        <Button variant="primary" icon={<PlusIcon size={14} />}>С иконкой</Button>
        <Button variant="ghost" icon={<EditIcon size={14} />} size="sm">Редактировать</Button>
        <Button variant="primary" disabled>Disabled</Button>
      </div>
      <div style={{ display: 'flex', gap: 10, alignItems: 'center' }}>
        <IconButton label="В избранное"><HeartIcon size={16} /></IconButton>
        <IconButton label="В избранном" active><HeartFillIcon size={16} /></IconButton>
        <IconButton label="Поиск" size="sm"><SearchIcon size={14} /></IconButton>
        <IconButton label="Удалить" size="sm" tone="danger"><TrashIcon size={14} /></IconButton>
      </div>
      <div>
        <AsyncButton variant="primary" onAction={() => new Promise((r) => setTimeout(r, 900))}>
          Сохранить
        </AsyncButton>
      </div>
    </div>
  ),
};
