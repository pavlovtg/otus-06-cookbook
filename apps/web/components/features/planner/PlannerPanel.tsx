"use client";

import * as React from "react";
import type { RecipeShortDto } from "@/lib/schemas/recipe";
import { PlannerRecipeCard } from "./PlannerRecipeCard";

export type PlannerMode = "all" | "fav" | "mine";

const MODE_OPTIONS: { value: PlannerMode; label: string }[] = [
  { value: "all", label: "Все" },
  { value: "fav", label: "Избранное" },
  { value: "mine", label: "Мои" },
];

export interface PlannerPanelProps {
  recipes: RecipeShortDto[];
  currentUserId?: string;
}

export function PlannerPanel({ recipes, currentUserId }: PlannerPanelProps) {
  const [mode, setMode] = React.useState<PlannerMode>("all");
  const [search, setSearch] = React.useState("");

  const filtered = recipes.filter((r) => {
    if (mode === "fav" && !r.isFavorite) return false;
    if (mode === "mine" && r.authorId !== currentUserId) return false;
    if (search && !r.title.toLowerCase().includes(search.toLowerCase()))
      return false;
    return true;
  });

  return (
    <div className="planner-panel">
      <div style={{ display: "flex", gap: 12, alignItems: "center", flexWrap: "wrap" }}>
        <div className="segmented" role="tablist">
          {MODE_OPTIONS.map((o) => (
            <button
              key={o.value}
              role="tab"
              aria-selected={mode === o.value}
              className={mode === o.value ? "is-active" : ""}
              onClick={() => setMode(o.value)}
            >
              {o.label}
            </button>
          ))}
        </div>
        <div style={{ flex: 1, minWidth: 200 }}>
          <div className="search-wrap">
            <input
              className="search-input"
              placeholder="Поиск рецептов…"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
        </div>
      </div>
      <div className="scroll" style={{ display: "flex", gap: 8, overflowX: "auto", paddingTop: 8 }}>
        {filtered.map((r) => (
          <PlannerRecipeCard key={r.id} recipe={r} />
        ))}
        {filtered.length === 0 && (
          <span style={{ color: "var(--text-3, #999)", fontSize: 14 }}>
            Рецептов не найдено
          </span>
        )}
      </div>
    </div>
  );
}
