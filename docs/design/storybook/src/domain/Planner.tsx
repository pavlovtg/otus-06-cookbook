import * as React from 'react';
import { XIcon } from '../icons';
import { RecipePhoto } from '../photo';
import { DAY_LABELS, MEAL_KEYS, MEAL_LABELS, getRecipe, type MealKey, type Recipe } from '../mocks';

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

  const updateSlot = (key: string, fn: (items: PlanItem[]) => PlanItem[]) => {
    onPlanChange?.({ ...plan, [key]: fn(plan[key] || []) });
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
            const items = plan[key] || [];
            return (
              <div
                key={key}
                className={['planner-slot', !items.length ? 'is-empty' : ''].filter(Boolean).join(' ')}
                onDragOver={(e) => {
                  e.preventDefault();
                  (e.currentTarget as HTMLElement).classList.add('is-dragover');
                }}
                onDragLeave={(e) => (e.currentTarget as HTMLElement).classList.remove('is-dragover')}
                onDrop={(e) => {
                  e.preventDefault();
                  (e.currentTarget as HTMLElement).classList.remove('is-dragover');
                  const id = dragId || e.dataTransfer.getData('text/plain');
                  if (!id) return;
                  const r = getRecipe(id);
                  updateSlot(key, (xs) => [...xs, { recipe_id: id, servings: r?.servings ?? 2 }]);
                  setDragId(null);
                }}
              >
                {!items.length ? (
                  <span>—</span>
                ) : (
                  items.map((it, idx) => {
                    const r = getRecipe(it.recipe_id);
                    if (!r) return null;
                    return (
                      <div className="planner-slot-item" key={idx}>
                        <div className="thumb">
                          <RecipePhoto seed={r.id} title={r.title} />
                        </div>
                        <div className="info">
                          <span className="name">{r.title}</span>
                          <span className="servings-mini">
                            порц.:{' '}
                            <input
                              type="number"
                              min={1}
                              max={99}
                              value={it.servings}
                              onChange={(e) =>
                                updateSlot(key, (xs) =>
                                  xs.map((x, i) => (i === idx ? { ...x, servings: parseInt(e.target.value) || 1 } : x)),
                                )
                              }
                            />
                          </span>
                        </div>
                        <button
                          className="remove"
                          aria-label="Удалить"
                          onClick={() => updateSlot(key, (xs) => xs.filter((_, i) => i !== idx))}
                        >
                          <XIcon size={14} />
                        </button>
                      </div>
                    );
                  })
                )}
              </div>
            );
          })}
        </React.Fragment>
      ))}
    </div>
  );
}

export function PlannerRecipe({ recipe }: { recipe: Recipe }) {
  return (
    <div
      className="planner-recipe"
      data-recipe-id={recipe.id}
      draggable
      onDragStart={(e) => e.dataTransfer.setData('text/plain', recipe.id)}
    >
      <div className="photo">
        <RecipePhoto seed={recipe.id} title={recipe.title} />
      </div>
      <div className="name">{recipe.title}</div>
    </div>
  );
}
