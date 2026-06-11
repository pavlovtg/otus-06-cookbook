import * as React from 'react';

type Variant = 'primary' | 'ghost' | 'danger';
type Size = 'md' | 'sm';

export interface ButtonProps extends Omit<React.ButtonHTMLAttributes<HTMLButtonElement>, 'type'> {
  variant?: Variant;
  size?: Size;
  type?: 'button' | 'submit' | 'reset';
  icon?: React.ReactNode;
  trailingIcon?: React.ReactNode;
}

export function Button({
  variant = 'primary',
  size = 'md',
  type = 'button',
  icon,
  trailingIcon,
  children,
  className,
  ...rest
}: ButtonProps) {
  const cls = ['btn', `btn-${variant}`, size === 'sm' ? 'btn-sm' : '', className].filter(Boolean).join(' ');
  return (
    <button type={type} className={cls} {...rest}>
      {icon}
      {children != null && <span>{children}</span>}
      {trailingIcon}
    </button>
  );
}

export interface IconButtonProps extends Omit<React.ButtonHTMLAttributes<HTMLButtonElement>, 'type'> {
  size?: 'md' | 'sm';
  tone?: 'default' | 'danger';
  active?: boolean;
  label: string;
}
export function IconButton({ size = 'md', tone = 'default', active, label, children, className, ...rest }: IconButtonProps) {
  const cls = [
    size === 'sm' ? 'btn-icon-sm' : 'btn-icon',
    active ? 'is-on' : '',
    tone === 'danger' ? 'is-danger' : '',
    className,
  ].filter(Boolean).join(' ');
  return (
    <button type="button" className={cls} aria-label={label} title={label} {...rest}>
      {children}
    </button>
  );
}

/** AsyncButton: press → spinner → ✓ */
export function AsyncButton({
  onAction,
  children,
  ...rest
}: { onAction: () => Promise<void> | void } & Omit<ButtonProps, 'onClick'>) {
  const [state, setState] = React.useState<'idle' | 'loading' | 'done'>('idle');
  const handle = async () => {
    setState('loading');
    try {
      await onAction();
      setState('done');
      setTimeout(() => setState('idle'), 1200);
    } catch {
      setState('idle');
    }
  };
  return (
    <Button {...rest} disabled={state === 'loading' || rest.disabled} onClick={handle}>
      {state === 'loading' ? <Spinner /> : state === 'done' ? '✓ Готово' : children}
    </Button>
  );
}

function Spinner() {
  return (
    <span style={{ display: 'inline-flex', alignItems: 'center', gap: 8 }}>
      <svg width={14} height={14} viewBox="0 0 24 24" style={{ animation: 'spin 700ms linear infinite' }}>
        <circle cx="12" cy="12" r="9" stroke="currentColor" strokeWidth="2" fill="none" opacity={0.25} />
        <path d="M21 12a9 9 0 0 0-9-9" stroke="currentColor" strokeWidth="2" fill="none" strokeLinecap="round" />
      </svg>
      <style>{`@keyframes spin{to{transform:rotate(360deg)}}`}</style>
      Загрузка
    </span>
  );
}
