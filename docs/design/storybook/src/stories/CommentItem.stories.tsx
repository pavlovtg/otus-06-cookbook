import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { CommentItem } from '../domain/CommentItem';
import { comments } from '../mocks';

const meta: Meta<typeof CommentItem> = {
  title: 'Domain/CommentItem',
  component: CommentItem,
  parameters: { layout: 'centered' },
};

export default meta;
type S = StoryObj<typeof CommentItem>;

export const Default: S = {
  render: () => (
    <div style={{ maxWidth: 520 }}>
      <CommentItem comment={comments[0]!} />
    </div>
  ),
};

export const WithDelete: S = {
  render: () => {
    const [visible, setVisible] = React.useState(true);
    if (!visible) {
      return (
        <p style={{ color: 'var(--fg-muted)', fontSize: 14 }}>
          Комментарий удалён
        </p>
      );
    }
    return (
      <div style={{ maxWidth: 520 }}>
        <CommentItem
          comment={comments[1]!}
          canDelete
          onDelete={() => setVisible(false)}
        />
      </div>
    );
  },
};

export const WithoutDelete: S = {
  render: () => (
    <div style={{ maxWidth: 520 }}>
      <CommentItem comment={comments[2]!} canDelete={false} />
    </div>
  ),
};
