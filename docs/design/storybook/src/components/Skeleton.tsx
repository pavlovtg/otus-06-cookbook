import * as React from 'react';

export function Skeleton({ height, width = '100%', card }: { height?: number | string; width?: number | string; card?: boolean }) {
  return (
    <div
      className={['skeleton', card ? 'skeleton-card' : ''].filter(Boolean).join(' ')}
      style={{ width, height: card ? undefined : (height ?? 16) }}
    />
  );
}

export function EmptyState({
  eyebrow,
  title,
  description,
  action,
}: {
  eyebrow?: React.ReactNode;
  title?: React.ReactNode;
  description?: React.ReactNode;
  action?: React.ReactNode;
}) {
  return (
    <div className="state">
      {eyebrow && <div className="state-eyebrow">{eyebrow}</div>}
      {title && <div className="t-display">{title}</div>}
      {description && <p className="t-small">{description}</p>}
      {action}
    </div>
  );
}
