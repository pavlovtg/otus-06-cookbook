import { useEffect, useState } from 'react';
import { fakeApi } from '../mock/fakeApi';
import type { ShoppingItem } from '../mock/types';

export function ShoppingList() {
  const [items, setItems] = useState<ShoppingItem[] | null>(null);
  const [checked, setChecked] = useState<Record<string, boolean>>({});

  useEffect(() => { fakeApi.getShoppingList().then(setItems); }, []);

  if (!items) return <div className="muted">загрузка…</div>;

  return (
    <>
      <h1>Список покупок</h1>
      <p className="muted">Автоматически собран из плана меню. Ингредиенты агрегированы по единицам измерения.</p>
      <div className="card">
        {/* style: shopping-list */}
        <table className="sl-table">
          <thead><tr><th></th><th>Продукт</th><th>Количество</th><th>Ед.</th></tr></thead>
          <tbody>
            {items.map((it) => {
              const k = it.name + '|' + it.unit;
              return (
                <tr key={k} style={{ opacity: checked[k] ? 0.5 : 1 }}>
                  <td><input type="checkbox" checked={!!checked[k]} onChange={(e) => setChecked({ ...checked, [k]: e.target.checked })} /></td>
                  <td style={{ textDecoration: checked[k] ? 'line-through' : 'none' }}>{it.name}</td>
                  <td>{Math.round(it.qty * 10) / 10}</td>
                  <td>{it.unit}</td>
                </tr>
              );
            })}
            {items.length === 0 && <tr><td colSpan={4} className="muted">План пуст — нечего покупать.</td></tr>}
          </tbody>
        </table>
      </div>
    </>
  );
}
