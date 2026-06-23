import * as React from 'react';
import { XIcon } from '../icons';
import { RecipePhoto } from '../photo';
import { getRecipe } from '../mocks';
import type { PlanItem } from './PlannerGrid';

export interface PlannerSlotProps {
  slotKey: string;
  items: PlanItem[];
  dragId: string | null;
  onDrop: (slotKey: string, recipeId: string) => void;
  onServingsChange: (slotKey: string, idx: number, servings: number) => void;
  onRemove: (slotKey: string, idx: number) => void;
}

export function PlannerSlot({
  slotKey,
  items,
  dragId,
  onDrop,
  onServingsChange,
  onRemove,
}: PlannerSlotProps) {
  const [isDragOver, setIsDragOver] = React.useState(false);

  return (
    <div
      className={['planner-slot', !items.length ? 'is-empty' : '', isDragOver ? 'is-dragover' : '']
        .filter(Boolean)
        .join(' ')}
      onDragOver={(e) => {
        e.preventDefault();
        setIsDragOver(true);
      }}
      onDragLeave={() => setIsDragOver(false)}
      onDrop={(e) => {
        e.preventDefault();
        setIsDragOver(false);
        const id = dragId || e.dataTransfer.getData('text/plain');
        if (id) onDrop(slotKey, id);
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
                      onServingsChange(slotKey, idx, parseInt(e.target.value) || 1)
                    }
                  />
                </span>
              </div>
              <button
                className="remove"
                aria-label="Удалить"
                onClick={() => onRemove(slotKey, idx)}
              >
                <XIcon size={14} />
              </button>
            </div>
          );
        })
      )}
    </div>
  );
}
