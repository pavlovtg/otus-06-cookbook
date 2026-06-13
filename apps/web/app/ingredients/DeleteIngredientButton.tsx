"use client";

import { useState } from "react";
import { createPortal } from "react-dom";
import { useRouter } from "next/navigation";
import { deleteIngredient } from "@/lib/bff/ingredients";

interface Props {
  id: string;
  title: string;
}

export function DeleteIngredientButton({ id, title }: Props) {
  const router = useRouter();
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);

  async function handleDelete() {
    setLoading(true);
    try {
      await deleteIngredient(id);
      router.refresh();
    } catch {
      setLoading(false);
      setOpen(false);
    }
  }

  return (
    <>
      <button
        className="btn btn-danger btn-sm"
        data-testid="delete-ingredient-trigger"
        onClick={() => setOpen(true)}
      >
        Удалить
      </button>

      {open && createPortal(
        <div
          className="modal-backdrop is-open"
          onClick={() => setOpen(false)}
        >
          <div className="modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-head">
              <h2>Удалить ингредиент?</h2>
            </div>
            <div className="modal-body">
              <p className="t-body" style={{ color: "var(--fg-secondary)" }}>
                «{title}» будет удалён навсегда.
              </p>
              <div className="form-actions">
                <button
                  className="btn btn-ghost"
                  onClick={() => setOpen(false)}
                  disabled={loading}
                >
                  Отмена
                </button>
                <button
                  className="btn btn-danger"
                  onClick={handleDelete}
                  disabled={loading}
                >
                  {loading ? "Удаление..." : "Удалить"}
                </button>
              </div>
            </div>
          </div>
        </div>,
        document.body,
      )}
    </>
  );
}
