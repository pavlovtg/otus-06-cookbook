import type { Meta, StoryObj } from '@storybook/react';
import { RatingStars } from './RatingStars';

const meta: Meta<typeof RatingStars> = {
  title: 'Primitives/RatingStars',
  component: RatingStars,
  tags: ['autodocs'],
  args: { value: 0, readOnly: false },
  argTypes: { value: { control: { type: 'range', min: 0, max: 5, step: 1 } } },
};
export default meta;
type S = StoryObj<typeof RatingStars>;

export const Empty: S = {};
export const ThreeStars: S = { args: { value: 3 } };
export const FiveStars: S = { args: { value: 5 } };
export const ReadOnly: S = { args: { value: 4, readOnly: true } };
