"use client";

import * as React from "react";
import { useRouter } from "next/navigation";
import { StarsRating } from "@/components/StarsRating";
import { setRating, deleteRating } from "@/lib/bff/ratings";

interface RatingWidgetProps {
  recipeId: string;
  initialMyRating?: number | null;
  initialAverageRating?: number | null;
}

export function RatingWidget({
  recipeId,
  initialMyRating,
  initialAverageRating,
}: RatingWidgetProps) {
  const router = useRouter();
  const [myRating, setMyRating] = React.useState<number>(initialMyRating ?? 0);
  const [averageRating, setAverageRating] = React.useState<number | null>(
    initialAverageRating ?? null,
  );
  const [pending, setPending] = React.useState(false);

  async function handleChange(v: number) {
    if (pending) return;
    setPending(true);
    try {
      const summary = await setRating(recipeId, v);
      setMyRating(summary.myRating ?? v);
      setAverageRating(summary.averageRating);
      router.refresh();
    } catch {
      // ignore
    } finally {
      setPending(false);
    }
  }

  async function handleDelete() {
    if (pending || myRating === 0) return;
    setPending(true);
    try {
      await deleteRating(recipeId);
      setMyRating(0);
      // Среднее пересчитывается на сервере с учётом оценок других пользователей —
      // полагаемся на router.refresh, который перечитает Server Component.
      router.refresh();
    } catch {
      // ignore
    } finally {
      setPending(false);
    }
  }

  return (
    <div className="card card-pad-lg">
      <h3 className="t-subheading" style={{ marginBottom: 12 }}>
        Оценить рецепт
      </h3>
      <div style={{ display: "flex", flexDirection: "column", gap: 8 }}>
        <StarsRating
          value={myRating || undefined}
          defaultValue={myRating}
          onChange={handleChange}
          size={24}
        />
        {myRating > 0 && (
          <button
            type="button"
            className="btn btn-ghost btn-sm"
            onClick={handleDelete}
            disabled={pending}
            style={{ alignSelf: "flex-start" }}
          >
            Удалить оценку
          </button>
        )}
        {averageRating != null && (
          <span className="t-small" style={{ color: "var(--fg-secondary)" }}>
            Средняя оценка: {averageRating.toFixed(1)}
          </span>
        )}
      </div>
    </div>
  );
}
