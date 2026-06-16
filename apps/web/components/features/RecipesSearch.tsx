"use client";

import * as React from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { SearchInput } from "@/components/ui/SearchInput";
import type { Category } from "@/lib/schemas/category";
import type { Ingredient } from "@/lib/schemas/ingredient";

const MAX_QUERY_LENGTH = 300;

// ─── shared hook ────────────────────────────────────────────────────────────

function useRecipesNav() {
  const router = useRouter();
  const searchParams = useSearchParams();

  function buildUrl(newQ: string, newSort: string) {
    const params = new URLSearchParams(searchParams.toString());
    if (newQ) {
      params.set("q", newQ);
    } else {
      params.delete("q");
    }
    if (newSort) {
      params.set("sort", newSort);
    } else {
      params.delete("sort");
    }
    params.set("page", "1");
    return `/?${params.toString()}`;
  }

  const currentQ = searchParams.get("q") ?? "";
  const currentSort = searchParams.get("sort") ?? "";

  return { router, buildUrl, currentQ, currentSort };
}

// ─── SearchInput wrapper ─────────────────────────────────────────────────────

interface RecipesSearchInputProps {
  initialQ?: string;
  categories: Category[];
  ingredients: Ingredient[];
}

export function RecipesSearchInput({
  initialQ = "",
  categories,
  ingredients,
}: RecipesSearchInputProps) {
  const { router, buildUrl, currentSort } = useRecipesNav();
  const [q, setQ] = React.useState(initialQ);
  const timerRef = React.useRef<ReturnType<typeof setTimeout> | null>(null);

  function handleQueryChange(value: string) {
    const clamped = value.slice(0, MAX_QUERY_LENGTH);
    setQ(clamped);
    if (timerRef.current) clearTimeout(timerRef.current);
    timerRef.current = setTimeout(() => {
      router.push(buildUrl(clamped, currentSort));
    }, 300);
  }

  function handlePickSuggestion(label: string) {
    const words = q.trimEnd().split(/\s+/);
    words.pop();
    words.push(label);
    const newQ = (words.join(" ") + " ").slice(0, MAX_QUERY_LENGTH);
    setQ(newQ);
    if (timerRef.current) clearTimeout(timerRef.current);
    router.push(buildUrl(newQ.trim(), currentSort));
  }

  const suggestions = React.useMemo(() => {
    const trimmed = q.trim().toLowerCase();
    if (trimmed.length < 2) return [];
    const last = trimmed.split(/\s+/).pop() ?? "";
    if (last.length < 2) return [];

    const catSuggestions = categories
      .filter((c) => c.name.toLowerCase().includes(last))
      .slice(0, 6)
      .map((c) => ({ label: c.name, kind: "категория" }));

    const ingSuggestions = ingredients
      .filter((i) => i.title.toLowerCase().includes(last))
      .slice(0, 6)
      .map((i) => ({ label: i.title, kind: "ингредиент" }));

    return [...catSuggestions, ...ingSuggestions];
  }, [q, categories, ingredients]);

  return (
    <SearchInput
      value={q}
      onValueChange={handleQueryChange}
      placeholder="Найти рецепты, ингредиенты, категории…"
      suggestions={suggestions}
      onPickSuggestion={handlePickSuggestion}
      maxLength={MAX_QUERY_LENGTH}
    />
  );
}

// ─── Sort aside ──────────────────────────────────────────────────────────────

interface RecipesSortAsideProps {
  initialSort?: string;
  initialQ?: string;
}

const SORT_OPTIONS: Array<{ value: string; label: string }> = [
  { value: "title_asc", label: "А → Я" },
  { value: "title_desc", label: "Я → А" },
];

export function RecipesSortAside({
  initialSort = "",
  initialQ = "",
}: RecipesSortAsideProps) {
  const { router, buildUrl, currentSort, currentQ } = useRecipesNav();

  const activeSort = currentSort || initialSort;
  const activeQ = currentQ || initialQ;

  function handleSortChange(newSort: string) {
    router.push(buildUrl(activeQ, newSort));
  }

  return (
    <div className="aside-block">
      <span className="aside-label">Сортировка</span>
      {SORT_OPTIONS.map((opt) => (
        <div
          key={opt.value}
          className={[
            "aside-item",
            activeSort === opt.value ? "is-active" : "",
          ]
            .filter(Boolean)
            .join(" ")}
          role="button"
          tabIndex={0}
          onClick={() => handleSortChange(opt.value)}
          onKeyDown={(e) => e.key === "Enter" && handleSortChange(opt.value)}
        >
          <span>{opt.label}</span>
        </div>
      ))}
    </div>
  );
}
