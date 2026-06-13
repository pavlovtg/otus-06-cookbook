import * as React from 'react';
import type { Recipe } from '../mocks';
import { getIngredient } from '../mocks';

export function IngredientList({ recipe, servings }: { recipe: Recipe; servings?: number }) {
  const scale = (servings ?? recipe.servings) / recipe.servings;
  return (
    <div className="ingredients-list">
      {recipe.ingredients.map((ri, i) => {
        const ing = getIngredient(ri.ingredient_id);
        if (!ing) return null;
        const amount = Math.round(ri.amount * scale * 100) / 100;
        return (
          <div className="ingredient-row" key={i}>
            <span className="name">{ing.title}</span>
            <span className="amount">
              {amount} {ing.unit}
            </span>
          </div>
        );
      })}
    </div>
  );
}
