"use client";

import * as React from "react";
import { HeartIcon, HeartFillIcon } from "@/components/icons";
import { addFavorite, removeFavorite } from "@/lib/bff/favorites";

interface FavoriteDetailButtonProps {
  recipeId: string;
  isFavorite: boolean;
}

export function FavoriteDetailButton({
  recipeId,
  isFavorite,
}: FavoriteDetailButtonProps) {
  const [active, setActive] = React.useState(isFavorite);
  const [loading, setLoading] = React.useState(false);

  async function handleClick() {
    if (loading) return;
    setLoading(true);
    try {
      if (active) {
        await removeFavorite(recipeId);
      } else {
        await addFavorite(recipeId);
      }
      setActive((prev) => !prev);
    } catch {
      // silent
    } finally {
      setLoading(false);
    }
  }

  return (
    <button
      className={active ? "btn btn-ghost btn-sm" : "btn btn-primary btn-sm"}
      onClick={handleClick}
      disabled={loading}
      aria-label={active ? "Убрать из избранного" : "Добавить в избранное"}
    >
      {active ? (
        <>
          <HeartFillIcon size={14} /> В избранном
        </>
      ) : (
        <>
          <HeartIcon size={14} /> В избранное
        </>
      )}
    </button>
  );
}
