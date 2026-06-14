"use client";

import { useState } from "react";
import { createPortal } from "react-dom";
import {
  IngredientRequestSchema,
  IngredientCategory,
  IngredientCategoryLabels,
  type Ingredient,
  type IngredientRequest,
} from "@/lib/schemas/ingredient";
import { createIngredient, updateIngredient } from "@/lib/bff/ingredients";

interface Props {
  ingredient?: Ingredient;
  trigger: React.ReactNode;
}

const CATEGORY_OPTIONS = IngredientCategory.options;

export function IngredientModal({ ingredient, trigger }: Props) {
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const [form, setForm] = useState<IngredientRequest>({
    title: ingredient?.title ?? "",
    unit: ingredient?.unit ?? "г",
    defaultAmount: ingredient?.defaultAmount ?? 100,
    category: ingredient?.category ?? "vegetables",
  });

  function set<K extends keyof IngredientRequest>(
    key: K,
    value: IngredientRequest[K],
  ) {
    setForm((prev) => ({ ...prev, [key]: value }));
    setErrors((prev) => ({ ...prev, [key]: "" }));
  }

  function handleOpen() {
    setForm({
      title: ingredient?.title ?? "",
      unit: ingredient?.unit ?? "г",
      defaultAmount: ingredient?.defaultAmount ?? 100,
      category: ingredient?.category ?? "vegetables",
    });
    setErrors({});
    setOpen(true);
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const result = IngredientRequestSchema.safeParse(form);
    if (!result.success) {
      const fieldErrors: Record<string, string> = {};
      for (const issue of result.error.issues) {
        const key = issue.path[0] as string;
        fieldErrors[key] = issue.message;
      }
      setErrors(fieldErrors);
      return;
    }
    setLoading(true);
    try {
      if (ingredient) {
        await updateIngredient(ingredient.id, result.data);
      } else {
        await createIngredient(result.data);
      }
      setOpen(false);
      setLoading(false);
      window.location.assign(window.location.pathname + window.location.search);
    } catch {
      setLoading(false);
    }
  }

  return (
    <>
      <span onClick={handleOpen}>{trigger}</span>

      {open && createPortal(
        <div
          className="modal-backdrop is-open"
          onClick={() => setOpen(false)}
        >
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-head">
              <h2>{ingredient ? "Редактировать ингредиент" : "Новый ингредиент"}</h2>
            </div>
            <form className="modal-body" onSubmit={handleSubmit}>
              <div className="field">
                <label htmlFor="ingredient-title">Название</label>
                <input
                  id="ingredient-title"
                  className="input"
                  value={form.title}
                  onChange={(e) => set("title", e.target.value)}
                  placeholder="Название ингредиента"
                  maxLength={200}
                  disabled={loading}
                />
                {errors.title && (
                  <span className="error-text">{errors.title}</span>
                )}
              </div>

              <div className="field-row">
                <div className="field">
                  <label htmlFor="ingredient-unit">Единица измерения</label>
                  <input
                    id="ingredient-unit"
                    className="input"
                    value={form.unit}
                    onChange={(e) => set("unit", e.target.value)}
                    placeholder="г, мл, шт"
                    maxLength={20}
                    disabled={loading}
                  />
                  {errors.unit && (
                    <span className="error-text">{errors.unit}</span>
                  )}
                </div>

                <div className="field">
                  <label htmlFor="ingredient-defaultAmount">
                    Количество по умолчанию
                  </label>
                  <input
                    id="ingredient-defaultAmount"
                    type="number"
                    className="input"
                    value={form.defaultAmount}
                    onChange={(e) =>
                      set("defaultAmount", Number(e.target.value))
                    }
                    min={0.001}
                    max={100000}
                    step={0.001}
                    disabled={loading}
                  />
                  {errors.defaultAmount && (
                    <span className="error-text">{errors.defaultAmount}</span>
                  )}
                </div>
              </div>

              <div className="field">
                <label htmlFor="ingredient-category">Категория</label>
                <select
                  id="ingredient-category"
                  className="select"
                  value={form.category}
                  onChange={(e) =>
                    set(
                      "category",
                      e.target.value as IngredientRequest["category"],
                    )
                  }
                  disabled={loading}
                >
                  {CATEGORY_OPTIONS.map((cat) => (
                    <option key={cat} value={cat}>
                      {IngredientCategoryLabels[cat]}
                    </option>
                  ))}
                </select>
                {errors.category && (
                  <span className="error-text">{errors.category}</span>
                )}
              </div>

              <div className="form-actions">
                <button
                  type="button"
                  className="btn btn-ghost"
                  onClick={() => setOpen(false)}
                  disabled={loading}
                >
                  Отмена
                </button>
                <button
                  type="submit"
                  className="btn btn-primary"
                  disabled={loading}
                >
                  {loading
                    ? "Сохранение..."
                    : ingredient
                      ? "Сохранить"
                      : "Создать"}
                </button>
              </div>
            </form>
          </div>
        </div>,
        document.body,
      )}
    </>
  );
}
