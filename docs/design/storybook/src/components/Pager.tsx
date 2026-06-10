export type PagerProps = { page: number; pages: number; onGo?: (p: number) => void };

export function Pager({ page, pages, onGo }: PagerProps) {
  const items: (number | '…')[] = [];
  for (let p = 1; p <= pages; p++) {
    if (p === 1 || p === pages || Math.abs(p - page) <= 2) items.push(p);
    else if (Math.abs(p - page) === 3) items.push('…');
  }
  return (
    <div className="pager">
      <button className="btn" disabled={page === 1} onClick={() => onGo?.(Math.max(1, page - 1))}>‹</button>
      {items.map((it, i) => it === '…'
        ? <span key={'e' + i} className="muted">…</span>
        : <button key={it} className={'btn' + (it === page ? ' cur' : '')} onClick={() => onGo?.(it)}>{it}</button>
      )}
      <button className="btn" disabled={page === pages} onClick={() => onGo?.(Math.min(pages, page + 1))}>›</button>
    </div>
  );
}
