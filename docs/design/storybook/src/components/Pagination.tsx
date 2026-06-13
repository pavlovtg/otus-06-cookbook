import * as React from 'react';

export interface PaginationProps {
  page?: number;
  defaultPage?: number;
  total: number;
  onChange?: (page: number) => void;
}
export function Pagination({ page, defaultPage = 1, total, onChange }: PaginationProps) {
  const [inner, setInner] = React.useState(defaultPage);
  const v = page !== undefined ? page : inner;
  const set = (n: number) => {
    if (page === undefined) setInner(n);
    onChange?.(n);
  };
  if (total <= 1) return null;
  return (
    <div className="pagination">
      <button className="page-btn" disabled={v === 1} onClick={() => set(v - 1)} aria-label="Назад">
        ←
      </button>
      {Array.from({ length: total }).map((_, i) => (
        <button
          key={i}
          className={['page-btn', i + 1 === v ? 'is-active' : ''].filter(Boolean).join(' ')}
          onClick={() => set(i + 1)}
        >
          {i + 1}
        </button>
      ))}
      <button className="page-btn" disabled={v === total} onClick={() => set(v + 1)} aria-label="Вперёд">
        →
      </button>
    </div>
  );
}
