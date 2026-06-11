import * as React from 'react';
import { XIcon } from '../icons';

export interface ModalProps {
  open: boolean;
  onClose: () => void;
  title?: React.ReactNode;
  children: React.ReactNode;
  footer?: React.ReactNode;
  size?: 'md' | 'lg';
}
export function Modal({ open, onClose, title, children, footer, size = 'md' }: ModalProps) {
  React.useEffect(() => {
    if (!open) return;
    const onKey = (e: KeyboardEvent) => e.key === 'Escape' && onClose();
    document.addEventListener('keydown', onKey);
    return () => document.removeEventListener('keydown', onKey);
  }, [open, onClose]);

  return (
    <div className={['modal-backdrop', open ? 'is-open' : ''].filter(Boolean).join(' ')} onClick={(e) => e.target === e.currentTarget && onClose()}>
      <div className={['modal', size === 'lg' ? 'modal-lg' : ''].filter(Boolean).join(' ')}>
        {(title || true) && (
          <div className="modal-head">
            {title && <h2>{title}</h2>}
            <button className="modal-close" onClick={onClose} aria-label="Закрыть">
              <XIcon size={16} />
            </button>
          </div>
        )}
        <div className="modal-body">{children}</div>
        {footer && <div className="form-actions">{footer}</div>}
      </div>
    </div>
  );
}
