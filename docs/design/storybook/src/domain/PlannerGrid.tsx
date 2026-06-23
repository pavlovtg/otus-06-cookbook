import * as React from 'react';
import { DAY_LABELS, MEAL_KEYS, MEAL_LABELS, getRecipe, type MealKey } from '../mocks';
import { PlannerSlot } from './PlannerSlot';

export type PlanItem = { recipe_id: string; servings: number };
export type Plan = Record<string, PlanItem[]>; // key = `${dayIdx}_${meal}`

export function emptyPlan(): Plan {
  const p: Plan = {};
  for (let d = 0; d < 7; d++) for (const m of MEAL_KEYS) p[`${d}_${m}`] = [];
  return p;
}

export interface PlannerGridProps {
  plan: Plan;
  onPlanChange?: (plan: Plan) => void;
}

export function PlannerGrid({ plan, onPlanChange }: PlannerGridProps) {
  const [dragId, setDragId] = React.useState<string | null>(null);

  React.useEffect(() => {
    const onDragStart = (e: DragEvent) => {
      const t = e.target as HTMLElement;
      const id = t.closest('[data-recipe-id]')?.getAttribute('data-recipe-id');
      if (id) setDragId(id);
    };
    document.addEventListener('dragstart', onDragStart);
    return () => document.removeEventListener('dragstart', onDragStart);
  }, []);

  const handleDrop = (key: string, recipeId: string) => {
    const r = getRecipe(recipeId);
    onPlanChange?.({
      ...plan,
      [key]: [...(plan[key] || []), { recipe_id: recipeId, servings: r?.servings ?? 2 }],
    });
    setDragId(null);
  };

  const handleServingsChange = (key: string, idx: number, servings: number) => {
    onPlanChange?.({
      ...plan,
      [key]: (plan[key] || []).map((x, i) => (i === idx ? { ...x, servings } : x)),
    });
  };

  const handleRemove = (key: string, idx: number) => {
    onPlanChange?.({
      ...plan,
      [key]: (plan[key] || []).filter((_, i) => i !== idx),
    });
  };

  return (
    <div className="planner-grid">
      <div />
      {DAY_LABELS.map((d) => (
        <div key={d} className="planner-head-cell">
          {d}
        </div>
      ))}
      {(MEAL_KEYS as readonly MealKey[]).map((m) => (
        <React.Fragment key={m}>
          <div className="planner-row-label">{MEAL_LABELS[m]}</div>
          {DAY_LABELS.map((_, d) => {
            const key = `${d}_${m}`;
            return (
              <PlannerSlot
                key={key}
                slotKey={key}
                items={plan[key] || []}
                dragId={dragId}
                onDrop={handleDrop}
                onServingsChange={handleServingsChange}
                onRemove={handleRemove}
              />
            );
          })}
        </React.Fragment>
      ))}
    </div>
  );
}
