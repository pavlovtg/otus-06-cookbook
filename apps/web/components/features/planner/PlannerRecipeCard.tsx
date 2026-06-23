"use client";

import { useDraggable } from "@dnd-kit/core";
import { CSS } from "@dnd-kit/utilities";
import type { RecipeShortDto } from "@/lib/schemas/recipe";

export interface PlannerRecipeCardProps {
  recipe: RecipeShortDto;
}

export function PlannerRecipeCard({ recipe }: PlannerRecipeCardProps) {
  const { attributes, listeners, setNodeRef, transform, isDragging } =
    useDraggable({ id: recipe.id, data: { recipe } });

  const style = {
    transform: CSS.Translate.toString(transform),
    opacity: isDragging ? 0.4 : 1,
  };

  return (
    <div
      ref={setNodeRef}
      className="planner-recipe"
      data-recipe-id={recipe.id}
      style={style}
      {...listeners}
      {...attributes}
    >
      <div className="photo">
        {recipe.photoId ? (
          // eslint-disable-next-line @next/next/no-img-element
          <img
            src={`/api/cookbook/v1/photos/${recipe.photoId}/thumbnail`}
            alt={recipe.title}
          />
        ) : null}
      </div>
      <div className="name">{recipe.title}</div>
    </div>
  );
}
