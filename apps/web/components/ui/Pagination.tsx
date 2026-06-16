"use client";

import * as React from "react";

export interface PaginationProps {
  page?: number;
  defaultPage?: number;
  total: number;
  onChange?: (page: number) => void;
}

function getPages(current: number, total: number): (number | "…")[] {
  if (total <= 7) {
    return Array.from({ length: total }, (_, i) => i + 1);
  }

  const delta = 2;
  const left = current - delta;
  const right = current + delta;

  const pages: (number | "…")[] = [];

  // Первая страница
  pages.push(1);

  // Левый разрыв
  if (left > 2) {
    pages.push("…");
  }

  // Окно вокруг текущей
  for (let i = Math.max(2, left); i <= Math.min(total - 1, right); i++) {
    pages.push(i);
  }

  // Правый разрыв
  if (right < total - 1) {
    pages.push("…");
  }

  // Последняя страница
  pages.push(total);

  return pages;
}

export function Pagination({
  page,
  defaultPage = 1,
  total,
  onChange,
}: PaginationProps) {
  const [inner, setInner] = React.useState(defaultPage);
  const v = page !== undefined ? page : inner;
  const set = (n: number) => {
    if (page === undefined) setInner(n);
    onChange?.(n);
  };

  if (total <= 1) return null;

  const pages = getPages(v, total);

  return (
    <div className="pagination">
      <button
        className="page-btn"
        disabled={v === 1}
        onClick={() => set(v - 1)}
        aria-label="Назад"
      >
        ←
      </button>
      {pages.map((p, i) =>
        p === "…" ? (
          <span key={`ellipsis-${i}`} className="page-btn" style={{ cursor: "default", opacity: 0.5 }}>
            …
          </span>
        ) : (
          <button
            key={p}
            className={["page-btn", p === v ? "is-active" : ""].filter(Boolean).join(" ")}
            onClick={() => set(p)}
          >
            {p}
          </button>
        )
      )}
      <button
        className="page-btn"
        disabled={v === total}
        onClick={() => set(v + 1)}
        aria-label="Вперёд"
      >
        →
      </button>
    </div>
  );
}
