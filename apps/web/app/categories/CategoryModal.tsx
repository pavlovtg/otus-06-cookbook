"use client";

import { useState } from "react";
import { createPortal } from "react-dom";
import {
  CategoryRequestSchema,
  CategoryType,
  CategoryTypeLabels,
  type Category,
  type CategoryRequest,
} from "@/lib/schemas/category";
import { createCategory, updateCategory } from "@/lib/bff/categories";

interface Props {
  category?: Category;
  trigger: React.ReactNode;
}

const TYPE_OPTIONS = CategoryType.options;

export function CategoryModal({ category, trigger }: Props) {
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const [form, setForm] = useState<CategoryRequest>({
    name: category?.name ?? "",
    description: category?.description ?? "",
    type: category?.type ?? "meal_role",
  });

  function set<K extends keyof CategoryRequest>(
    key: K,
    value: CategoryRequest[K],
  ) {
    setForm((prev) => ({ ...prev, [key]: value }));
    setErrors((prev) => ({ ...prev, [key]: "" }));
  }

  function handleOpen() {
    setForm({
      name: category?.name ?? "",
      description: category?.description ?? "",
      type: category?.type ?? "meal_role",
    });
    setErrors({});
    setOpen(true);
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    const result = CategoryRequestSchema.safeParse(form);
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
      if (category) {
        await updateCategory(category.id, result.data);
      } else {
        await createCategory(result.data);
      }
      setOpen(false);
      setLoading(false);
      window.location.assign(window.location.pathname);
    } catch {
      setLoading(false);
    }
  }

  return (
    <>
      <span onClick={handleOpen}>{trigger}</span>

      {open &&
        createPortal(
          <div
            className="modal-backdrop is-open"
            onClick={() => setOpen(false)}
          >
            <div className="modal" onClick={(e) => e.stopPropagation()}>
              <div className="modal-head">
                <h2>
                  {category ? "Редактировать категорию" : "Новая категория"}
                </h2>
              </div>
              <form className="modal-body" onSubmit={handleSubmit}>
                <div className="field">
                  <label htmlFor="category-name">Название</label>
                  <input
                    id="category-name"
                    className="input"
                    value={form.name}
                    onChange={(e) => set("name", e.target.value)}
                    placeholder="Название категории"
                    maxLength={200}
                    disabled={loading}
                    data-testid="category-name-input"
                  />
                  {errors.name && (
                    <span className="error-text">{errors.name}</span>
                  )}
                </div>

                <div className="field">
                  <label htmlFor="category-type">Тип</label>
                  <select
                    id="category-type"
                    className="select"
                    value={form.type}
                    onChange={(e) =>
                      set("type", e.target.value as CategoryRequest["type"])
                    }
                    disabled={loading || !!category}
                    data-testid="category-type-select"
                  >
                    {TYPE_OPTIONS.map((t) => (
                      <option key={t} value={t}>
                        {CategoryTypeLabels[t]}
                      </option>
                    ))}
                  </select>
                  {errors.type && (
                    <span className="error-text">{errors.type}</span>
                  )}
                </div>

                <div className="field">
                  <label htmlFor="category-description">Описание</label>
                  <textarea
                    id="category-description"
                    className="textarea"
                    value={form.description ?? ""}
                    onChange={(e) => set("description", e.target.value)}
                    placeholder="Необязательное описание"
                    maxLength={2000}
                    disabled={loading}
                    data-testid="category-description-input"
                  />
                  {errors.description && (
                    <span className="error-text">{errors.description}</span>
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
                    data-testid="category-submit"
                  >
                    {loading
                      ? "Сохранение..."
                      : category
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
