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
  const [errorDetail, setErrorDetail] = useState<string | null>(null);

  async function handleDelete() {
    setLoading(true);
    setErrorDetail(null);
    try {
      await deleteIngredient(id);
      router.refresh();
      setOpen(false);
    } catch (err: unknown) {
      setLoading(false);
      const detail =
        err instanceof Error ? err.message : "Не удалось удалить ингредиент";
      setErrorDetail(detail);
    }
  }

  function handleOpen() {
    setErrorDetail(null);
    setOpen(true);
  }

  return (
    <>
      <button
        className="btn btn-danger btn-sm"
        data-testid="delete-ingredient-trigger"
        onClick={handleOpen}
      >
        Удалить
      </button>

      {open &&
        createPortal(
          <div
            className="modal-backdrop is-open"
            onClick={() => setOpen(false)}
          >
            <div className="modal" onClick={(e) => e.stopPropagation()}>
              <div className="modal-head">
                <h2>Удалить ингредиент?</h2>
              </div>
              <div className="modal-body">
                {errorDetail ? (
                  <div
                    style={{
                      padding: "12px 16px",
                      background: "rgba(244,114,114,0.08)",
                      borderRadius: "var(--radius-card)",
                      boxShadow: "0 0 0 1px rgba(244,114,114,0.18) inset",
                      color: "var(--danger)",
                      fontSize: 14,
                      lineHeight: 1.5,
                    }}
                  >
                    <strong style={{ display: "block", marginBottom: 4 }}>
                      Ингредиент используется в рецептах
                    </strong>
                    {errorDetail}
                  </div>
                ) : (
                  <p
                    className="t-body"
                    style={{ color: "var(--fg-secondary)" }}
                  >
                    «{title}» будет удалён навсегда.
                  </p>
                )}

                <div className="form-actions">
                  <button
                    className="btn btn-ghost"
                    onClick={() => setOpen(false)}
                    disabled={loading}
                  >
                    {errorDetail ? "Закрыть" : "Отмена"}
                  </button>
                  {!errorDetail && (
                    <button
                      className="btn btn-danger"
                      onClick={handleDelete}
                      disabled={loading}
                    >
                      {loading ? "Удаление..." : "Удалить"}
                    </button>
                  )}
                </div>
              </div>
            </div>
          </div>,
          document.body,
        )}
    </>
  );
}
