import type { Meta, StoryObj } from '@storybook/react';
import * as React from 'react';
import { PlannerGrid, PlannerRecipe, emptyPlan, type Plan } from '../domain/Planner';
import { ShoppingList } from '../domain/ShoppingList';
import { recipes, getIngredient, MEAL_KEYS } from '../mocks';
import { SearchInput } from '../components/SearchInput';
import { Segmented } from '../components/Segmented';

const meta: Meta = { title: 'Domain/Planner', parameters: { layout: 'fullscreen' } };
export default meta;
type S = StoryObj;

export const PanelRecipe: S = {
  render: () => (
    <div style={{ width: 220 }}>
      <PlannerRecipe recipe={recipes[0]} />
    </div>
  ),
};

export const GridEmpty: S = {
  render: () => (
    <div style={{ padding: 24 }}>
      <PlannerGrid plan={emptyPlan()} />
    </div>
  ),
};

export const Playground: S = {
  render: () => {
    const initial: Plan = emptyPlan();
    initial['0_breakfast'] = [{ recipe_id: 'r2', servings: 2 }];
    initial['0_dinner'] = [{ recipe_id: 'r1', servings: 4 }];
    initial['1_lunch'] = [{ recipe_id: 'r5', servings: 6 }];
    initial['2_dinner'] = [{ recipe_id: 'r3', servings: 2 }];
    initial['4_dinner'] = [{ recipe_id: 'r4', servings: 2 }];
    const [plan, setPlan] = React.useState<Plan>(initial);
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
        <div className="planner-panel">
          <div style={{ display: 'flex', gap: 12, alignItems: 'center', flexWrap: 'wrap' }}>
            <Segmented options={[{ value: 'all', label: 'Все' }, { value: 'fav', label: 'Избранное' }, { value: 'mine', label: 'Мои' }]} />
            <div style={{ flex: 1, minWidth: 200 }}>
              <SearchInput placeholder="Поиск рецептов…" onValueChange={setQ} />
            </div>
          </div>
          <div className="scroll">
            {list.map((r) => (
              <PlannerRecipe key={r.id} recipe={r} />
            ))}
          </div>
        </div>
        <PlannerGrid plan={plan} onPlanChange={setPlan} />
        <h3 className="t-subheading" style={{ marginTop: 8 }}>Список покупок</h3>
        <ShoppingList grouped={grouped} />
      </div>
    );
  },
};
