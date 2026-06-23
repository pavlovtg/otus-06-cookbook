"use client";

import { useDroppable } from "@dnd-kit/core";
import type { PlanItem } from "@/lib/planner-utils";

export interface PlannerSlotProps {
  slotKey: string;
  items: PlanItem[];
  recipeNames: Record<string, string>;
  onServingsChange: (slotKey: string, idx: number, servings: number) => void;
  onRemove: (slotKey: string, idx: number) => void;
}

export function PlannerSlot({
  slotKey,
  items,
  recipeNames,
  onServingsChange,
  onRemove,
}: PlannerSlotProps) {
  const { setNodeRef, isOver } = useDroppable({ id: slotKey });

  const classes = [
    "planner-slot",
    items.length === 0 ? "is-empty" : "",
    isOver ? "is-dragover" : "",
  ]
    .filter(Boolean)
    .join(" ");

  return (
    <div ref={setNodeRef} className={classes}>
      {items.length === 0 ? (
        <span>—</span>
      ) : (
        items.map((it, idx) => (
          <div className="planner-slot-item" key={idx}>
            <div className="info">
              <span className="name">
                {recipeNames[it.recipeId] ?? it.recipeId}
              </span>
              <span className="servings-mini">
                порц.:{" "}
                <input
                  type="number"
                  min={1}
                  max={99}
                  value={it.servings}
                  onChange={(e) =>
                    onServingsChange(
                      slotKey,
                      idx,
                      parseInt(e.target.value) || 1,
                    )
                  }
                />
              </span>
            </div>
            <button
              className="remove"
              aria-label="Удалить"
              onClick={() => onRemove(slotKey, idx)}
            >
              ×
            </button>
          </div>
        ))
      )}
    </div>
  );
}
