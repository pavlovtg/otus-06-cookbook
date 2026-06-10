import type { Meta, StoryObj } from '@storybook/react';
import { useState } from 'react';
import { Pager } from './Pager';

const meta: Meta<typeof Pager> = { title: 'Primitives/Pager', component: Pager, tags: ['autodocs'] };
export default meta;
type S = StoryObj<typeof Pager>;

export const FewPages: S = { args: { page: 2, pages: 4 } };
export const ManyPages: S = { args: { page: 7, pages: 20 } };
export const Interactive: S = {
  render: () => {
    const [p, setP] = useState(1);
    return <Pager page={p} pages={20} onGo={setP} />;
  },
};
