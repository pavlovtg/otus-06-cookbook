import type { Meta, StoryObj } from '@storybook/react';
import { Badge } from './Badge';

const meta: Meta<typeof Badge> = {
  title: 'Primitives/Badge',
  component: Badge,
  tags: ['autodocs'],
  args: { children: 'badge', variant: 'default' },
  argTypes: { variant: { control: 'inline-radio', options: ['default', 'cat', 'priv'] } },
};
export default meta;
type S = StoryObj<typeof Badge>;

export const Default: S = {};
export const Category: S = { args: { variant: 'cat', children: 'Завтрак' } };
export const Private: S = { args: { variant: 'priv', children: 'приватный' } };
export const Tag: S = { args: { children: '#итальянское' } };
export const Gallery: S = {
  render: () => (
    <div className="row">
      <Badge>#тег</Badge>
      <Badge variant="cat">Завтрак</Badge>
      <Badge variant="cat">Десерт</Badge>
      <Badge variant="priv">приватный</Badge>
    </div>
  ),
};
