"use client";

import { PlannerSlot } from "./PlannerSlot";
import {
  DAY_LABELS,
  MEAL_KEYS,
  MEAL_LABELS,
  type Plan,
  type MealKey,
} from "@/lib/planner-utils";

export interface PlannerGridProps {
  plan: Plan;
  recipeNames: Record<string, string>;
  onServingsChange: (slotKey: string, idx: number, servings: number) => void;
  onRemove: (slotKey: string, idx: number) => void;
}

export function PlannerGrid({
  plan,
  recipeNames,
  onServingsChange,
  onRemove,
}: PlannerGridProps) {
  return (
    <div className="planner-grid">
      <div />
      {DAY_LABELS.map((d) => (
        <div key={d} className="planner-head-cell">
          {d}
        </div>
      ))}
      {(MEAL_KEYS as readonly MealKey[]).map((m) => (
        <div key={m} style={{ display: "contents" }}>
          <div className="planner-row-label">{MEAL_LABELS[m]}</div>
          {DAY_LABELS.map((_, d) => {
            const key = `${d}_${m}`;
            return (
              <PlannerSlot
                key={key}
                slotKey={key}
                items={plan[key] ?? []}
                recipeNames={recipeNames}
                onServingsChange={onServingsChange}
                onRemove={onRemove}
              />
            );
          })}
        </div>
      ))}
    </div>
  );
}
