import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { Modal } from '../components/Modal';
import { Button } from '../components/Button';
import { Field, Input } from '../components/Input';
import { Toast, ToastStack, useToasts } from '../components/Toast';
import { Skeleton, EmptyState } from '../components/Skeleton';

const meta: Meta = { title: 'Primitives/Feedback' };
export default meta;
type S = StoryObj;

export const ModalBasic: S = {
  render: () => {
    const [open, setOpen] = React.useState(true);
    return (
      <>
        <Button onClick={() => setOpen(true)}>Открыть</Button>
        <Modal
          open={open}
          onClose={() => setOpen(false)}
          title="Требуется вход"
          footer={
            <>
              <Button variant="ghost" onClick={() => setOpen(false)}>Отмена</Button>
              <Button variant="primary" onClick={() => setOpen(false)}>Войти</Button>
            </>
          }
        >
          <Field label="Email"><Input defaultValue="user1@cookbook.test" /></Field>
          <Field label="Пароль"><Input type="password" defaultValue="demo1234" /></Field>
        </Modal>
      </>
    );
  },
};

export const ToastBasic: S = {
  render: () => (
    <div style={{ position: 'relative', minHeight: 180 }}>
      <Toast item={{ id: '1', message: 'Сохранено', kind: 'success' }} />
      <div style={{ height: 10 }} />
      <Toast item={{ id: '2', message: 'Не удалось войти', kind: 'error' }} />
      <div style={{ height: 10 }} />
      <Toast item={{ id: '3', message: 'Просто уведомление' }} />
    </div>
  ),
};

export const ToastInteractive: S = {
  render: () => {
    const { toasts, push } = useToasts();
    return (
      <>
        <div style={{ display: 'flex', gap: 8 }}>
          <Button onClick={() => push('Сохранено', 'success')}>Success</Button>
          <Button variant="ghost" onClick={() => push('Ошибка', 'error')}>Error</Button>
          <Button variant="ghost" onClick={() => push('Просто текст')}>Default</Button>
        </div>
        <ToastStack items={toasts} />
      </>
    );
  },
};

export const Skeletons: S = {
  render: () => (
    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 16, maxWidth: 720 }}>
      <Skeleton card />
      <Skeleton card />
      <Skeleton card />
    </div>
  ),
};

export const Empty: S = {
  render: () => (
    <EmptyState
      eyebrow="В избранном пусто"
      title="Сохраните любимое"
      description="Откройте карточку рецепта и нажмите сердечко."
    />
  ),
};
