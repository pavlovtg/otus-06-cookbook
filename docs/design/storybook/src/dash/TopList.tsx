import * as React from 'react';
import { StarIcon } from '../icons';

export interface TopListItem {
  name: string;
  value: number | string;
  withStar?: boolean;
}
export function TopList({ items }: { items: TopListItem[] }) {
  if (!items.length) return <p className="t-small">Пока нет данных</p>;
  return (
    <div className="top-list">
      {items.map((r, i) => (
        <div key={i} className="top-list-row">
          <span className="rank">#{i + 1}</span>
          <span className="name">{r.name}</span>
          <span className="val">
            {r.withStar !== false && <StarIcon size={12} />}
            {r.value}
          </span>
        </div>
      ))}
    </div>
  );
}
