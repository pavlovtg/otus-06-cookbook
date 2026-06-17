"use client";

import * as React from "react";
import { HeartIcon, HeartFillIcon } from "@/components/icons";
import { addFavorite, removeFavorite } from "@/lib/bff/favorites";

interface FavoriteButtonProps {
  recipeId: string;
  isFavorite: boolean;
}

export function FavoriteButton({ recipeId, isFavorite }: FavoriteButtonProps) {
  const [active, setActive] = React.useState(isFavorite);
  const [loading, setLoading] = React.useState(false);

  async function handleClick(e: React.MouseEvent) {
    e.preventDefault();
    e.stopPropagation();
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
      // silent — не меняем состояние при ошибке
    } finally {
      setLoading(false);
    }
  }

  return (
    <button
      className={["btn-icon", "photo-fav", active ? "is-on" : ""].filter(Boolean).join(" ")}
      onClick={handleClick}
      aria-label={active ? "Убрать из избранного" : "Добавить в избранное"}
      disabled={loading}
    >
      {active ? <HeartFillIcon size={16} /> : <HeartIcon size={16} />}
    </button>
  );
}
