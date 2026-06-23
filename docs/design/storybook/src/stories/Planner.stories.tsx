import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { PlannerRecipeCard } from '../domain/PlannerRecipeCard';
import { PlannerSlot } from '../domain/PlannerSlot';
import { PlannerGrid, emptyPlan, type Plan } from '../domain/PlannerGrid';
import { PlannerPanel } from '../domain/PlannerPanel';
import { ShoppingList } from '../domain/ShoppingList';
import { recipes, getIngredient, mealPlan } from '../mocks';

const meta: Meta = { title: 'Domain/Planner', parameters: { layout: 'fullscreen' } };
export default meta;
type S = StoryObj;

// ---- 8.1 PlannerRecipeCard ----
export const RecipeCard: S = {
  name: 'PlannerRecipeCard',
  render: () => (
    <div style={{ padding: 24, display: 'flex', gap: 12, flexWrap: 'wrap' }}>
      {recipes.slice(0, 3).map((r) => (
        <div key={r.id} style={{ width: 200 }}>
          <PlannerRecipeCard recipe={r} />
        </div>
      ))}
    </div>
  ),
};

// ---- 8.2 PlannerSlot ----
export const SlotEmpty: S = {
  name: 'PlannerSlot / Empty',
  render: () => (
    <div style={{ padding: 24, width: 200 }}>
      <PlannerSlot
        slotKey="0_breakfast"
        items={[]}
        dragId={null}
        onDrop={() => {}}
        onServingsChange={() => {}}
        onRemove={() => {}}
      />
    </div>
  ),
};

export const SlotWithItems: S = {
  name: 'PlannerSlot / With items',
  render: () => {
    const [items, setItems] = React.useState([
      { recipe_id: 'r1', servings: 4 },
      { recipe_id: 'r2', servings: 2 },
    ]);
    return (
      <div style={{ padding: 24, width: 220 }}>
        <PlannerSlot
          slotKey="0_dinner"
          items={items}
          dragId={null}
          onDrop={(_, id) => setItems((xs) => [...xs, { recipe_id: id, servings: 2 }])}
          onServingsChange={(_, idx, s) =>
            setItems((xs) => xs.map((x, i) => (i === idx ? { ...x, servings: s } : x)))
          }
          onRemove={(_, idx) => setItems((xs) => xs.filter((_, i) => i !== idx))}
        />
      </div>
    );
  },
};

// ---- 8.3 PlannerGrid ----
export const GridEmpty: S = {
  name: 'PlannerGrid / Empty',
  render: () => (
    <div style={{ padding: 24 }}>
      <PlannerGrid plan={emptyPlan()} />
    </div>
  ),
};

export const GridWithPlan: S = {
  name: 'PlannerGrid / With plan',
  render: () => {
    const [plan, setPlan] = React.useState<Plan>(mealPlan);
    return (
      <div style={{ padding: 24 }}>
        <PlannerGrid plan={plan} onPlanChange={setPlan} />
      </div>
    );
  },
};

// ---- 8.4 PlannerPanel ----
export const Panel: S = {
  name: 'PlannerPanel',
  render: () => {
    const [q, setQ] = React.useState('');
    const list = recipes.filter((r) => r.title.toLowerCase().includes(q.toLowerCase()));
    return (
      <div style={{ padding: 24 }}>
        <PlannerPanel onSearch={setQ}>
          {list.map((r) => (
            <PlannerRecipeCard key={r.id} recipe={r} />
          ))}
        </PlannerPanel>
      </div>
    );
  },
};

// ---- 8.5 Playground ----
export const Playground: S = {
  name: 'Playground',
  render: () => {
    const [plan, setPlan] = React.useState<Plan>(mealPlan);
    const [q, setQ] = React.useState('');
    const list = recipes.filter((r) => r.title.toLowerCase().includes(q.toLowerCase()));

    const grouped = React.useMemo(() => {
      const acc = new Map<string, { ingredient: any; amount: number }>();
      for (const key of Object.keys(plan)) {
        for (const it of plan[key]) {
          const r = recipes.find((x) => x.id === it.recipe_id);
          if (!r) continue;
          const scale = it.servings / r.servings;
          for (const ri of r.ingredients) {
            const ing = getIngredient(ri.ingredient_id);
            if (!ing) continue;
            const cur = acc.get(ing.id) || { ingredient: ing, amount: 0 };
            cur.amount += ri.amount * scale;
            acc.set(ing.id, cur);
          }
        }
      }
      const groups: Record<string, any[]> = {};
      for (const v of acc.values()) {
        (groups[v.ingredient.category] ||= []).push(v);
      }
      for (const k of Object.keys(groups))
        groups[k].sort((a, b) => a.ingredient.title.localeCompare(b.ingredient.title, 'ru'));
      return groups;
    }, [plan]);

    return (
      <div style={{ padding: 24, display: 'flex', flexDirection: 'column', gap: 20 }}>
        <PlannerPanel onSearch={setQ}>
          {list.map((r) => (
            <PlannerRecipeCard key={r.id} recipe={r} />
          ))}
        </PlannerPanel>
        <PlannerGrid plan={plan} onPlanChange={setPlan} />
        <h3 className="t-subheading" style={{ marginTop: 8 }}>
          Список покупок
        </h3>
        <ShoppingList grouped={grouped} />
      </div>
    );
  },
};
