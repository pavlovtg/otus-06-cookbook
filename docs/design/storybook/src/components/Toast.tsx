import * as React from 'react';

export type ToastKind = 'default' | 'success' | 'error';
export interface ToastItem {
  id: string;
  message: string;
  kind?: ToastKind;
}

export function Toast({ item }: { item: ToastItem }) {
  const cls = ['toast', 'is-show', item.kind ? `toast-${item.kind}` : ''].filter(Boolean).join(' ');
  return (
    <div className={cls}>
      <span className="dotty" />
      <span>{item.message}</span>
    </div>
  );
}

export function ToastStack({ items }: { items: ToastItem[] }) {
  return (
    <div className="toast-stack">
      {items.map((t) => (
        <Toast key={t.id} item={t} />
      ))}
    </div>
  );
}

/** Uncontrolled hook: const { toasts, push } = useToasts() */
export function useToasts() {
  const [toasts, setToasts] = React.useState<ToastItem[]>([]);
  const push = React.useCallback((message: string, kind: ToastKind = 'default') => {
    const id = Math.random().toString(36).slice(2);
    setToasts((prev) => [...prev, { id, message, kind }]);
    setTimeout(() => setToasts((prev) => prev.filter((t) => t.id !== id)), 3000);
  }, []);
  return { toasts, push };
}
