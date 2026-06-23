"use client";

import { useDroppable } from "@dnd-kit/core";
import type { PlanItem } from "@/lib/planner-utils";

export interface PlannerSlotProps {
  slotKey: string;
  items: PlanItem[];
  recipeNames: Record<string, string>;
  recipePhotos: Record<string, string | undefined>;
  onServingsChange: (slotKey: string, idx: number, servings: number) => void;
  onRemove: (slotKey: string, idx: number) => void;
}

export function PlannerSlot({
  slotKey,
  items,
  recipeNames,
  recipePhotos,
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
        items.map((it, idx) => {
          const photoId = recipePhotos[it.recipeId];
          return (
            <div className="planner-slot-item" key={idx}>
              <div className="thumb">
                {photoId ? (
                  // eslint-disable-next-line @next/next/no-img-element
                  <img
                    src={`/api/cookbook/v1/photos/${photoId}/thumbnail`}
                    alt={recipeNames[it.recipeId] ?? it.recipeId}
                  />
                ) : null}
              </div>
              <div className="info">
                <span className="name">
                  {recipeNames[it.recipeId] ?? it.recipeId}
                </span>
              </div>
              <button
                className="remove"
                aria-label="Удалить"
                onClick={() => onRemove(slotKey, idx)}
              >
                ×
              </button>
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
          );
        })
      )}
    </div>
  );
}
