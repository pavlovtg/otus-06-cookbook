import * as React from 'react';
import { RecipePhoto } from '../photo';
import type { Recipe } from '../mocks';

export interface PlannerRecipeCardProps {
  recipe: Recipe;
}

export function PlannerRecipeCard({ recipe }: PlannerRecipeCardProps) {
  return (
    <div
      className="planner-recipe"
      data-recipe-id={recipe.id}
      draggable
      onDragStart={(e) => e.dataTransfer.setData('text/plain', recipe.id)}
    >
      <div className="photo">
        <RecipePhoto seed={recipe.id} title={recipe.title} />
      </div>
      <div className="name">{recipe.title}</div>
    </div>
  );
}
