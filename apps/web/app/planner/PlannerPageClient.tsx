"use client";

import * as React from "react";
import Link from "next/link";
import {
  DndContext,
  DragOverlay,
  type DragEndEvent,
  type DragStartEvent,
} from "@dnd-kit/core";
import { PlannerPanel } from "@/components/features/planner/PlannerPanel";
import { PlannerGrid } from "@/components/features/planner/PlannerGrid";
import { PlannerRecipeCard } from "@/components/features/planner/PlannerRecipeCard";
import { updateMealPlan, clearMealPlan } from "@/lib/bff/meal-plan";
import { planToRequest, type Plan } from "@/lib/planner-utils";
import { CartIcon } from "@/lib/icons";
import type { RecipeShortDto } from "@/lib/schemas/recipe";

export interface PlannerPageClientProps {
  initialPlan: Plan;
  recipes: RecipeShortDto[];
  currentUserId?: string;
}

function useDebounce<T>(value: T, delay: number): T {
  const [debounced, setDebounced] = React.useState(value);
  React.useEffect(() => {
    const t = setTimeout(() => setDebounced(value), delay);
    return () => clearTimeout(t);
  }, [value, delay]);
  return debounced;
}

export function PlannerPageClient({
  initialPlan,
  recipes,
  currentUserId,
}: PlannerPageClientProps) {
  const [plan, setPlan] = React.useState<Plan>(initialPlan);
  const [activeRecipe, setActiveRecipe] = React.useState<RecipeShortDto | null>(null);
  const [showClearDialog, setShowClearDialog] = React.useState(false);
  const [saving, setSaving] = React.useState(false);
  const [saveError, setSaveError] = React.useState<string | null>(null);

  // Карта id → название для слотов
  const recipeNames = React.useMemo(
    () => Object.fromEntries(recipes.map((r) => [r.id, r.title])),
    [recipes],
  );

  // Карта id → photoId для слотов
  const recipePhotos = React.useMemo(
    () => Object.fromEntries(recipes.map((r) => [r.id, r.photoId ?? undefined])),
    [recipes],
  );

  // Автосохранение с debounce 300 мс
  const debouncedPlan = useDebounce(plan, 300);
  const isFirstRender = React.useRef(true);

  React.useEffect(() => {
    if (isFirstRender.current) {
      isFirstRender.current = false;
      return;
    }
    setSaving(true);
    setSaveError(null);
    updateMealPlan(planToRequest(debouncedPlan))
      .catch((e: unknown) => {
        setSaveError(e instanceof Error ? e.message : "Ошибка сохранения");
      })
      .finally(() => setSaving(false));
  }, [debouncedPlan]);

  // DnD handlers
  const handleDragStart = (event: DragStartEvent) => {
    const recipe = recipes.find((r) => r.id === event.active.id);
    setActiveRecipe(recipe ?? null);
  };

  const handleDragEnd = (event: DragEndEvent) => {
    setActiveRecipe(null);
    const { active, over } = event;
    if (!over) return;
    const recipeId = String(active.id);
    const slotKey = String(over.id);
    const recipe = recipes.find((r) => r.id === recipeId);
    if (!recipe) return;
    setPlan((prev) => ({
      ...prev,
      [slotKey]: [
        ...(prev[slotKey] ?? []),
        { recipeId, servings: 2 },
      ],
    }));
  };

  const handleServingsChange = (
    slotKey: string,
    idx: number,
    servings: number,
  ) => {
    setPlan((prev) => ({
      ...prev,
      [slotKey]: (prev[slotKey] ?? []).map((x, i) =>
        i === idx ? { ...x, servings } : x,
      ),
    }));
  };

  const handleRemove = (slotKey: string, idx: number) => {
    setPlan((prev) => ({
      ...prev,
      [slotKey]: (prev[slotKey] ?? []).filter((_, i) => i !== idx),
    }));
  };

  const handleClearConfirm = async () => {
    setShowClearDialog(false);
    setSaving(true);
    setSaveError(null);
    try {
      await clearMealPlan();
      // Сбрасываем план локально
      const empty: Plan = {};
      for (const key of Object.keys(plan)) empty[key] = [];
      isFirstRender.current = true; // не триггерим автосохранение
      setPlan(empty);
    } catch (e: unknown) {
      setSaveError(e instanceof Error ? e.message : "Ошибка очистки");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div style={{ display: "flex", flexDirection: "column", gap: 16 }}>
      <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", flexWrap: "wrap", gap: 8 }}>
        <h1 style={{ margin: 0 }}>Планировщик меню</h1>
        <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
          {saving && (
            <span style={{ fontSize: 13, color: "var(--text-3, #999)" }}>
              Сохранение…
            </span>
          )}
          {saveError && (
            <span style={{ fontSize: 13, color: "var(--danger, #e53)" }}>
              {saveError}
            </span>
          )}
          <Link href="/shopping-list" className="btn btn-ghost btn-sm">
            <CartIcon size={14} />
            Список покупок
          </Link>
          <button
            className="btn btn-ghost btn-sm"
            onClick={() => setShowClearDialog(true)}
          >
            Очистить всё
          </button>
        </div>
      </div>

      <DndContext onDragStart={handleDragStart} onDragEnd={handleDragEnd}>
        <PlannerPanel recipes={recipes} currentUserId={currentUserId} />
        <PlannerGrid
          plan={plan}
          recipeNames={recipeNames}
          recipePhotos={recipePhotos}
          onServingsChange={handleServingsChange}
          onRemove={handleRemove}
        />
        <DragOverlay>
          {activeRecipe ? <PlannerRecipeCard recipe={activeRecipe} /> : null}
        </DragOverlay>
      </DndContext>

      {/* Диалог подтверждения очистки */}
      <div
        className={`modal-backdrop${showClearDialog ? " is-open" : ""}`}
        onClick={() => setShowClearDialog(false)}
      >
        <div
          className="modal"
          onClick={(e) => e.stopPropagation()}
          role="dialog"
          aria-modal="true"
          aria-labelledby="clear-dialog-title"
        >
          <div className="modal-head">
            <h2 id="clear-dialog-title">Очистить план?</h2>
          </div>
          <div className="modal-body">
            <p>Все блюда будут удалены из плана меню. Это действие нельзя отменить.</p>
          </div>
          <div className="form-actions" style={{ marginTop: 20 }}>
            <button
              className="btn btn-ghost"
              onClick={() => setShowClearDialog(false)}
            >
              Отмена
            </button>
            <button className="btn btn-danger" onClick={handleClearConfirm}>
              Очистить
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
