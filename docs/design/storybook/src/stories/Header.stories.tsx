import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { Header } from '../domain/Header';

const meta: Meta<typeof Header> = {
  title: 'Domain/Header',
  component: Header,
  parameters: { layout: 'fullscreen' },
};
export default meta;
type S = StoryObj<typeof Header>;

export const Guest: S = { args: { role: 'guest', active: '#/recipes' } };
export const User: S = { args: { role: 'user', userName: 'Анна Воронова', active: '#/recipes' } };
export const Admin: S = { args: { role: 'admin', userName: 'Админ', active: '#/categories' } };
