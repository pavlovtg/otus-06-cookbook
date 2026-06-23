import * as React from 'react';

export interface ShoppingListItem {
  ingredientId: string;
  title: string;
  amount: number;
  unit: string;
}

export interface ShoppingListGroup {
  category: string;
  items: ShoppingListItem[];
}

export function ShoppingList({ groups }: { groups: ShoppingListGroup[] }) {
  if (!groups.length) return null;
  return (
    <div className="shopping-table">
      {groups.map((group) => (
        <React.Fragment key={group.category}>
          <div className="shopping-group-head">{group.category}</div>
          {group.items.map((item) => (
            <div className="shopping-row" key={item.ingredientId}>
              <span>{item.title}</span>
              <span className="qty">{Math.round(item.amount * 100) / 100}</span>
              <span className="unit">{item.unit}</span>
            </div>
          ))}
        </React.Fragment>
      ))}
    </div>
  );
}
