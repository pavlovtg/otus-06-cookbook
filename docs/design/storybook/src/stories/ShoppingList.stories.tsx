import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { ShoppingList } from '../domain/ShoppingList';
import type { ShoppingListGroup } from '../domain/ShoppingList';

const meta: Meta<typeof ShoppingList> = {
  title: 'Domain/ShoppingList',
  component: ShoppingList,
  parameters: { layout: 'centered' },
};
export default meta;
type S = StoryObj<typeof ShoppingList>;

const filledGroups: ShoppingListGroup[] = [
  {
    category: 'Крупы и зерновые',
    items: [
      { ingredientId: 'i1', title: 'Спагетти', amount: 400, unit: 'г' },
      { ingredientId: 'i13', title: 'Мука пшеничная', amount: 290, unit: 'г' },
    ],
  },
  {
    category: 'Мясо и птица',
    items: [
      { ingredientId: 'i2', title: 'Бекон', amount: 150, unit: 'г' },
      { ingredientId: 'i11', title: 'Куриная грудка', amount: 450, unit: 'г' },
    ],
  },
  {
    category: 'Молочные продукты и яйца',
    items: [
      { ingredientId: 'i3', title: 'Яйцо куриное', amount: 7, unit: 'шт' },
      { ingredientId: 'i4', title: 'Сыр пармезан', amount: 110, unit: 'г' },
      { ingredientId: 'i14', title: 'Сыр моцарелла', amount: 150, unit: 'г' },
    ],
  },
  {
    category: 'Овощи',
    items: [
      { ingredientId: 'i7', title: 'Помидор', amount: 4, unit: 'шт' },
      { ingredientId: 'i8', title: 'Лук репчатый', amount: 2, unit: 'шт' },
      { ingredientId: 'i15', title: 'Базилик', amount: 20, unit: 'г' },
    ],
  },
  {
    category: 'Специи и приправы',
    items: [
      { ingredientId: 'i5', title: 'Чёрный перец', amount: 4, unit: 'г' },
      { ingredientId: 'i6', title: 'Соль', amount: 15, unit: 'г' },
    ],
  },
];

export const Filled: S = {
  name: 'Заполненный список',
  render: () => (
    <div style={{ minWidth: 360 }}>
      <ShoppingList groups={filledGroups} />
    </div>
  ),
};

export const Empty: S = {
  name: 'Пустой список',
  render: () => (
    <div style={{ minWidth: 360, padding: 24 }}>
      <ShoppingList groups={[]} />
      <p className="t-small" style={{ color: 'var(--text-secondary)', marginTop: 8 }}>
        Компонент ничего не рендерит при пустом массиве групп
      </p>
    </div>
  ),
};
