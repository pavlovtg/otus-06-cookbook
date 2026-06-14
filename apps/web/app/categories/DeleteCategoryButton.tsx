"use client";

import { useState } from "react";
import { createPortal } from "react-dom";
import { deleteCategory } from "@/lib/bff/categories";

interface Props {
  id: string;
  name: string;
}

export function DeleteCategoryButton({ id, name }: Props) {
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [errorDetail, setErrorDetail] = useState<string | null>(null);

  async function handleDelete() {
    setLoading(true);
    setErrorDetail(null);
    try {
      await deleteCategory(id);
      setOpen(false);
      window.location.assign(window.location.pathname);
    } catch (err: unknown) {
      setLoading(false);
      const detail =
        err instanceof Error ? err.message : "Не удалось удалить категорию";
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
        className="btn-icon"
        style={{ width: 20, height: 20, background: "transparent", boxShadow: "none" }}
        data-testid="delete-category-trigger"
        onClick={handleOpen}
        title="Удалить"
      >
        <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round">
          <path d="m6 6 12 12M6 18 18 6" />
        </svg>
      </button>

      {open &&
        createPortal(
          <div
            className="modal-backdrop is-open"
            onClick={() => setOpen(false)}
          >
            <div className="modal" onClick={(e) => e.stopPropagation()}>
              <div className="modal-head">
                <h2>Удалить «{name}»?</h2>
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
                      Категория используется в рецептах
                    </strong>
                    {errorDetail}
                  </div>
                ) : (
                  <p
                    className="t-body"
                    style={{ color: "var(--fg-secondary)" }}
                  >
                    Удалить можно только если категория не используется в рецептах.
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
                      data-testid="delete-category-confirm"
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
