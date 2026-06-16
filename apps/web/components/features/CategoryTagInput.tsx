"use client";

import { useState, useRef } from "react";
import type { Category } from "@/lib/schemas/category";
import { CategoryTypeLabels } from "@/lib/schemas/category";

interface CategoryTagInputProps {
  categories: Category[];
  value: string[];
  onChange: (ids: string[]) => void;
}

export function CategoryTagInput({
  categories,
  value,
  onChange,
}: CategoryTagInputProps) {
  const [query, setQuery] = useState("");
  const [open, setOpen] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);

  const selectedCats = value
    .map((id) => categories.find((c) => c.id === id))
    .filter((c): c is Category => c !== undefined);

  const suggestions = query.trim().length > 0
    ? categories
        .filter((c) => c.name.toLowerCase().includes(query.toLowerCase().trim()))
        .slice(0, 8)
    : [];

  function addCategory(cat: Category) {
    // Замена при совпадении типа: один тип — одна категория
    const filtered = value.filter((id) => {
      const existing = categories.find((c) => c.id === id);
      return existing?.type !== cat.type;
    });
    onChange([...filtered, cat.id]);
    setQuery("");
    setOpen(false);
    inputRef.current?.focus();
  }

  function removeCategory(id: string) {
    onChange(value.filter((v) => v !== id));
  }

  return (
    <div className="tag-input" data-testid="category-tag-input">
      {selectedCats.map((c) => (
        <span key={c.id} className="chip" data-testid="category-chip">
          <span>{c.name}</span>
          <button
            type="button"
            onClick={() => removeCategory(c.id)}
            aria-label={`Удалить ${c.name}`}
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" width={12} height={12}>
              <path d="m6 6 12 12M6 18 18 6" />
            </svg>
          </button>
        </span>
      ))}

      <div style={{ position: "relative", flex: 1, minWidth: 120 }}>
        <input
          ref={inputRef}
          type="text"
          value={query}
          onChange={(e) => {
            setQuery(e.target.value);
            setOpen(true);
          }}
          onFocus={() => setOpen(true)}
          onBlur={() => setTimeout(() => setOpen(false), 150)}
          placeholder={selectedCats.length > 0 ? "Добавьте ещё…" : "Начните вводить категорию…"}
          data-testid="category-search-input"
          style={{ background: "transparent", border: 0, outline: "none", color: "var(--fg-primary)", fontSize: 14, padding: "4px 6px", width: "100%" }}
        />

        {open && suggestions.length > 0 && (
          <div className="autocomplete is-open" data-testid="category-autocomplete">
            {suggestions.map((c) => (
              <div
                key={c.id}
                className="autocomplete-item"
                onMouseDown={(e) => {
                  e.preventDefault();
                  addCategory(c);
                }}
                data-testid="category-suggestion"
              >
                <span>{c.name}</span>
                <span className="kind">{CategoryTypeLabels[c.type]}</span>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
