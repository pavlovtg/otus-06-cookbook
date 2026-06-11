import * as React from 'react';
import { XIcon, LockIcon } from '../icons';

export type TagTone = 'default' | 'accent' | 'private';
export function Tag({ children, tone = 'default', icon }: { children: React.ReactNode; tone?: TagTone; icon?: React.ReactNode }) {
  const cls = ['tag', tone === 'accent' ? 'tag-accent' : '', tone === 'private' ? 'tag-private' : '']
    .filter(Boolean)
    .join(' ');
  return (
    <span className={cls}>
      {tone === 'private' && !icon ? <LockIcon size={12} /> : icon}
      <span>{children}</span>
    </span>
  );
}

export function Chip({ children, onRemove }: { children: React.ReactNode; onRemove?: () => void }) {
  return (
    <span className="chip">
      <span>{children}</span>
      {onRemove && (
        <button type="button" onClick={onRemove} aria-label="Удалить">
          <XIcon size={12} />
        </button>
      )}
    </span>
  );
}
