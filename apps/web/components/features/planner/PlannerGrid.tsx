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
  recipePhotos: Record<string, string | undefined>;
  onServingsChange: (slotKey: string, idx: number, servings: number) => void;
  onRemove: (slotKey: string, idx: number) => void;
}

export function PlannerGrid({
  plan,
  recipeNames,
  recipePhotos,
  onServingsChange,
  onRemove,
}: PlannerGridProps) {
  return (
    <div className="planner-grid-wrap">
      <div className="planner-grid">
        {/* Заголовки дней */}
        {DAY_LABELS.map((d) => (
          <div key={d} className="planner-head-cell">
            {d}
          </div>
        ))}

        {/* Строки по приёмам пищи */}
        {(MEAL_KEYS as readonly MealKey[]).map((m) => (
          <div key={m} style={{ display: "contents" }}>
            {/* Метка приёма пищи на всю ширину */}
            <div className="planner-meal-label">{MEAL_LABELS[m]}</div>
            {DAY_LABELS.map((_, d) => {
              const key = `${d}_${m}`;
              return (
                <PlannerSlot
                  key={key}
                  slotKey={key}
                  items={plan[key] ?? []}
                  recipeNames={recipeNames}
                  recipePhotos={recipePhotos}
                  onServingsChange={onServingsChange}
                  onRemove={onRemove}
                />
              );
            })}
          </div>
        ))}
      </div>
    </div>
  );
}
