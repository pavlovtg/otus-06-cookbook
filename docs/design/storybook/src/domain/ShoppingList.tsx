import * as React from 'react';

export interface ShoppingRow {
  ingredient: { title: string; unit: string; category: string };
  amount: number;
}
export function ShoppingList({ grouped }: { grouped: Record<string, ShoppingRow[]> }) {
  const cats = Object.keys(grouped);
  if (!cats.length) return null;
  return (
    <div className="shopping-table">
      {cats.map((cat) => (
        <React.Fragment key={cat}>
          <div className="shopping-group-head">{cat}</div>
          {grouped[cat].map((row, i) => (
            <div className="shopping-row" key={i}>
              <span>{row.ingredient.title}</span>
              <span className="qty">{Math.round(row.amount * 100) / 100}</span>
              <span className="unit">{row.ingredient.unit}</span>
            </div>
          ))}
        </React.Fragment>
      ))}
    </div>
  );
}
