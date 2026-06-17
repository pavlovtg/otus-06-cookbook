"use client";

import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import {
  RecipeRequestSchema,
  type RecipeRequest,
  type Difficulty,
  type RecipeIngredientRequest,
} from "@/lib/schemas/recipe";
import { getIngredients } from "@/lib/bff/ingredients";
import { getCategories } from "@/lib/bff/categories";
import type { Ingredient } from "@/lib/schemas/ingredient";
import type { Category } from "@/lib/schemas/category";
import { CategoryTagInput } from "@/components/features/CategoryTagInput";

const DIFFICULTY_OPTIONS: { value: Difficulty; label: string }[] = [
  { value: "easy", label: "Просто" },
  { value: "everyday", label: "На каждый день" },
  { value: "festive", label: "Праздничное" },
  { value: "restaurant", label: "Ресторанное" },
  { value: "signature", label: "Авторское" },
];

interface RecipeFormProps {
  initialValues?: Partial<RecipeRequest>;
  onSubmit: (data: RecipeRequest) => Promise<void>;
  submitLabel?: string;
}

export function RecipeForm({
  initialValues,
  onSubmit,
  submitLabel = "Сохранить",
}: RecipeFormProps) {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [availableIngredients, setAvailableIngredients] = useState<Ingredient[]>([]);
  const [availableCategories, setAvailableCategories] = useState<Category[]>([]);

  const [form, setForm] = useState<RecipeRequest>({
    title: initialValues?.title ?? "",
    description: initialValues?.description ?? "",
    cookingTime: initialValues?.cookingTime ?? 30,
    difficulty: initialValues?.difficulty ?? "everyday",
    servings: initialValues?.servings ?? 2,
    instructions: initialValues?.instructions ?? "",
    ingredients: initialValues?.ingredients ?? [],
    categoryIds: initialValues?.categoryIds ?? [],
    isPublic: initialValues?.isPublic ?? true,
  });

  useEffect(() => {
    getIngredients({ pageSize: 1000 })
      .then((result) => setAvailableIngredients(result.items))
      .catch(() => {});
    getCategories()
      .then((cats) => setAvailableCategories(cats))
      .catch(() => {});
  }, []);

  function set<K extends keyof RecipeRequest>(key: K, value: RecipeRequest[K]) {
    setForm((prev) => ({ ...prev, [key]: value }));
    setErrors((prev) => ({ ...prev, [key]: "" }));
  }

  function addIngredient() {
    const first = availableIngredients[0];
    if (!first) return;
    set("ingredients", [
      ...form.ingredients,
      { ingredientId: first.id, amount: first.defaultAmount ?? 1 },
    ]);
  }

  function updateIngredient(
    index: number,
    patch: Partial<RecipeIngredientRequest>,
  ) {
    const updated = form.ingredients.map((ing, i) =>
      i === index ? { ...ing, ...patch } : ing,
    );
    set("ingredients", updated);
  }

  function removeIngredient(index: number) {
    set(
      "ingredients",
      form.ingredients.filter((_, i) => i !== index),
    );
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const result = RecipeRequestSchema.safeParse(form);
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
      await onSubmit(result.data);
    } catch {
      setLoading(false);
    }
  }

  return (
    <form
      onSubmit={handleSubmit}
      style={{ display: "flex", flexDirection: "column", gap: 16, maxWidth: 640 }}
    >
      <div className="field">
        <label htmlFor="title">Название</label>
        <input
          id="title"
          className="input"
          value={form.title}
          onChange={(e) => set("title", e.target.value)}
          placeholder="Название рецепта"
          maxLength={200}
        />
        {errors.title && <span className="error-text">{errors.title}</span>}
      </div>

      <div className="field">
        <label htmlFor="description">Описание</label>
        <textarea
          id="description"
          className="textarea"
          value={form.description}
          onChange={(e) => set("description", e.target.value)}
          placeholder="Краткое описание рецепта"
          maxLength={2000}
        />
        {errors.description && (
          <span className="error-text">{errors.description}</span>
        )}
      </div>

      <div className="field-row">
        <div className="field">
          <label htmlFor="cookingTime">Время (мин)</label>
          <input
            id="cookingTime"
            type="number"
            className="input"
            value={form.cookingTime}
            onChange={(e) => set("cookingTime", Number(e.target.value))}
            min={1}
          />
          {errors.cookingTime && (
            <span className="error-text">{errors.cookingTime}</span>
          )}
        </div>

        <div className="field">
          <label htmlFor="servings">Порций</label>
          <input
            id="servings"
            type="number"
            className="input"
            value={form.servings}
            onChange={(e) => set("servings", Number(e.target.value))}
            min={1}
          />
          {errors.servings && (
            <span className="error-text">{errors.servings}</span>
          )}
        </div>
      </div>

      <div className="field">
        <label>Категории</label>
        <CategoryTagInput
          categories={availableCategories}
          value={form.categoryIds}
          onChange={(ids) => set("categoryIds", ids)}
        />
      </div>

      <div className="field">
        <label htmlFor="difficulty">Сложность</label>
        <select
          id="difficulty"
          className="select"
          value={form.difficulty}
          onChange={(e) => set("difficulty", e.target.value as Difficulty)}
        >
          {DIFFICULTY_OPTIONS.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </select>
        {errors.difficulty && (
          <span className="error-text">{errors.difficulty}</span>
        )}
      </div>

      <div className="field">
        <label htmlFor="instructions">Инструкции</label>
        <textarea
          id="instructions"
          className="textarea"
          value={form.instructions}
          onChange={(e) => set("instructions", e.target.value)}
          placeholder="Пошаговые инструкции приготовления"
          style={{ minHeight: 160 }}
          maxLength={10000}
        />
        {errors.instructions && (
          <span className="error-text">{errors.instructions}</span>
        )}
      </div>

      {/* Ingredients */}
      <div className="field">
        <div
          style={{
            display: "flex",
            alignItems: "center",
            justifyContent: "space-between",
            marginBottom: 8,
          }}
        >
          <label>Ингредиенты</label>
          <button
            type="button"
            className="btn btn-ghost btn-sm"
            onClick={addIngredient}
            disabled={availableIngredients.length === 0}
          >
            + Добавить
          </button>
        </div>

        {form.ingredients.length === 0 && (
          <p
            className="t-small"
            style={{ color: "var(--fg-muted)", fontSize: 13 }}
          >
            Ингредиенты не добавлены
          </p>
        )}

        <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
          {form.ingredients.map((ing, index) => {
            const selected = availableIngredients.find(
              (a) => a.id === ing.ingredientId,
            );
            return (
              <div key={index} className="ing-row">
                <select
                  className="input"
                  value={ing.ingredientId}
                  onChange={(e) =>
                    updateIngredient(index, { ingredientId: e.target.value })
                  }
                >
                  {availableIngredients.map((a) => (
                    <option key={a.id} value={a.id}>
                      {a.title}
                    </option>
                  ))}
                </select>
                <input
                  type="number"
                  className="input"
                  value={ing.amount}
                  min={0.001}
                  step={0.001}
                  onChange={(e) =>
                    updateIngredient(index, { amount: Number(e.target.value) })
                  }
                />
                <span
                  className="t-small"
                  style={{ color: "var(--fg-muted)", textAlign: "center" }}
                >
                  {selected?.unit ?? ""}
                </span>
                <button
                  type="button"
                  className="remove"
                  onClick={() => removeIngredient(index)}
                  title="Удалить"
                >
                  ✕
                </button>
              </div>
            );
          })}
        </div>
      </div>

      <div className="field">
        <label className="checkbox">
          <input
            type="checkbox"
            name="is_public"
            checked={form.isPublic}
            onChange={(e) => set("isPublic", e.target.checked)}
          />
          Публичный (виден всем)
        </label>
      </div>

      <div className="form-actions">
        <button
          type="button"
          className="btn btn-ghost"
          onClick={() => router.back()}
          disabled={loading}
        >
          Отмена
        </button>
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? "Сохранение..." : submitLabel}
        </button>
      </div>
    </form>
  );
}
