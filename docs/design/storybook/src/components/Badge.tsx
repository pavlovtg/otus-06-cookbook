import type { ReactNode } from 'react';

export type BadgeProps = { variant?: 'default' | 'cat' | 'priv'; children: ReactNode };

export function Badge({ variant = 'default', children }: BadgeProps) {
  const cls = ['badge', variant !== 'default' ? variant : ''].filter(Boolean).join(' ');
  return <span className={cls}>{children}</span>;
}
