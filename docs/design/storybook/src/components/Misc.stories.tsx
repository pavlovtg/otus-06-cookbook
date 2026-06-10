import type { Meta, StoryObj } from '@storybook/react';
import { useState } from 'react';
import { Spinner } from './Spinner';
import { Notice } from './Notice';
import { Modal } from './Modal';
import { Button } from './Button';

const meta: Meta = { title: 'Primitives/Misc', tags: ['autodocs'] };
export default meta;

export const SpinnerStory: StoryObj = { name: 'Spinner', render: () => <Spinner /> };
export const NoticeStory: StoryObj = { name: 'Notice', render: () => <Notice>Это уведомление-предупреждение.</Notice> };

export const ModalStory: StoryObj = {
  name: 'Modal',
  render: () => {
    const [open, setOpen] = useState(true);
    return (
      <>
        <Button variant="primary" onClick={() => setOpen(true)}>Открыть модалку</Button>
        <Modal
          open={open}
          title="Подтверждение"
          onClose={() => setOpen(false)}
          footer={
            <>
              <Button onClick={() => setOpen(false)}>Отмена</Button>
              <Button variant="danger" onClick={() => setOpen(false)}>Удалить</Button>
            </>
          }
        >
          <p>Вы уверены, что хотите удалить этот рецепт?</p>
        </Modal>
      </>
    );
  },
};
