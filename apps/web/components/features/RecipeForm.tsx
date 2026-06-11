"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { RecipeRequestSchema, type RecipeRequest, type Difficulty } from "@/lib/schemas/recipe";

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

export function RecipeForm({ initialValues, onSubmit, submitLabel = "Сохранить" }: RecipeFormProps) {
  const router = useRouter();
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const [form, setForm] = useState<RecipeRequest>({
    title: initialValues?.title ?? "",
    description: initialValues?.description ?? "",
    cookingTime: initialValues?.cookingTime ?? 30,
    difficulty: initialValues?.difficulty ?? "everyday",
    servings: initialValues?.servings ?? 2,
    instructions: initialValues?.instructions ?? "",
  });

  function set<K extends keyof RecipeRequest>(key: K, value: RecipeRequest[K]) {
    setForm((prev) => ({ ...prev, [key]: value }));
    setErrors((prev) => ({ ...prev, [key]: "" }));
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
    <form onSubmit={handleSubmit} style={{ display: "flex", flexDirection: "column", gap: 16, maxWidth: 640 }}>
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
        {errors.description && <span className="error-text">{errors.description}</span>}
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
          {errors.cookingTime && <span className="error-text">{errors.cookingTime}</span>}
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
          {errors.servings && <span className="error-text">{errors.servings}</span>}
        </div>
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
            <option key={opt.value} value={opt.value}>{opt.label}</option>
          ))}
        </select>
        {errors.difficulty && <span className="error-text">{errors.difficulty}</span>}
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
        {errors.instructions && <span className="error-text">{errors.instructions}</span>}
      </div>

      <div className="form-actions">
        <button type="button" className="btn btn-ghost" onClick={() => router.back()} disabled={loading}>
          Отмена
        </button>
        <button type="submit" className="btn btn-primary" disabled={loading}>
          {loading ? "Сохранение..." : submitLabel}
        </button>
      </div>
    </form>
  );
}
