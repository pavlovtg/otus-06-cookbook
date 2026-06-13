"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { deleteRecipe } from "@/lib/bff/gateway";

interface Props {
  id: string;
}

export function DeleteRecipeButton({ id }: Props) {
  const router = useRouter();
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);

  async function handleDelete() {
    setLoading(true);
    try {
      await deleteRecipe(id);
      router.push("/");
      router.refresh();
    } catch {
      setLoading(false);
      setOpen(false);
    }
  }

  return (
    <>
      <button className="btn btn-danger btn-sm" data-testid="delete-recipe-trigger" onClick={() => setOpen(true)}>
        Удалить
      </button>

      <div className={`modal-backdrop${open ? " is-open" : ""}`} onClick={() => setOpen(false)}>
        <div className="modal" onClick={(e) => e.stopPropagation()}>
          <div className="modal-head">
            <h2>Удалить рецепт?</h2>
          </div>
          <div className="modal-body">
            <p className="t-body" style={{ color: "var(--fg-secondary)" }}>
              Это действие необратимо. Рецепт будет удалён навсегда.
            </p>
            <div className="form-actions">
              <button className="btn btn-ghost" onClick={() => setOpen(false)} disabled={loading}>
                Отмена
              </button>
              <button className="btn btn-danger" onClick={handleDelete} disabled={loading}>
                {loading ? "Удаление..." : "Удалить"}
              </button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
