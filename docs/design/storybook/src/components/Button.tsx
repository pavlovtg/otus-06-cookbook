import type { ButtonHTMLAttributes, ReactNode } from 'react';

export type ButtonProps = {
  variant?: 'default' | 'primary' | 'danger' | 'ghost';
  size?: 'md' | 'sm';
  children: ReactNode;
} & ButtonHTMLAttributes<HTMLButtonElement>;

export function Button({ variant = 'default', size = 'md', className = '', children, ...rest }: ButtonProps) {
  const cls = ['btn', variant !== 'default' ? variant : '', size === 'sm' ? 'sm' : '', className].filter(Boolean).join(' ');
  return <button className={cls} {...rest}>{children}</button>;
}
