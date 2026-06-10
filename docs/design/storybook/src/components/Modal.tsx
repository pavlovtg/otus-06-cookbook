import type { ReactNode } from 'react';

export type ModalProps = { open: boolean; title?: string; onClose?: () => void; children?: ReactNode; footer?: ReactNode };

export function Modal({ open, title, onClose, children, footer }: ModalProps) {
  if (!open) return null;
  return (
    <div className="modal-back" role="dialog" aria-modal onClick={onClose}>
      <div className="modal" onClick={(e) => e.stopPropagation()}>
        {title && <h3>{title}</h3>}
        <div>{children}</div>
        {footer && <div className="row" style={{ justifyContent: 'flex-end', marginTop: 16 }}>{footer}</div>}
      </div>
    </div>
  );
}
