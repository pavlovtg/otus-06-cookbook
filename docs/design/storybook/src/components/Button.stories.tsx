import type { Meta, StoryObj } from '@storybook/react';
import { Button } from './Button';

const meta: Meta<typeof Button> = {
  title: 'Primitives/Button',
  component: Button,
  tags: ['autodocs'],
  argTypes: {
    variant: { control: 'inline-radio', options: ['default', 'primary', 'danger', 'ghost'] },
    size: { control: 'inline-radio', options: ['md', 'sm'] },
    disabled: { control: 'boolean' },
    onClick: { action: 'click' },
  },
  args: { children: 'Кнопка', variant: 'default', size: 'md' },
};
export default meta;
type S = StoryObj<typeof Button>;

export const Default: S = {};
export const Primary: S = { args: { variant: 'primary' } };
export const Danger: S = { args: { variant: 'danger' } };
export const Ghost: S = { args: { variant: 'ghost' } };
export const Small: S = { args: { size: 'sm', variant: 'primary' } };
export const Disabled: S = { args: { disabled: true, variant: 'primary' } };
export const Gallery: S = {
  render: () => (
    <div className="row">
      <Button>Default</Button>
      <Button variant="primary">Primary</Button>
      <Button variant="danger">Danger</Button>
      <Button variant="ghost">Ghost</Button>
      <Button size="sm" variant="primary">Small primary</Button>
      <Button disabled variant="primary">Disabled</Button>
    </div>
  ),
};
