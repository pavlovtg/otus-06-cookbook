"use client";

import { useState } from "react";
import { MinusIcon, PlusIcon } from "@/components/icons";
import type { RecipeIngredientDto } from "@/lib/schemas/recipe";

interface Props {
  ingredients: RecipeIngredientDto[];
  baseServings: number;
}

export function IngredientsCard({ ingredients, baseServings }: Props) {
  const [currentServings, setCurrentServings] = useState<number>(baseServings);

  const decrement = () => setCurrentServings((s) => Math.max(1, s - 1));
  const increment = () => setCurrentServings((s) => Math.min(99, s + 1));

  const scale = (baseAmount: number): number =>
    Math.round(baseAmount * (currentServings / baseServings) * 100) / 100;

  return (
    <div className="card card-pad-lg">
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 14,
        }}
      >
        <h3 className="t-subheading">Ингредиенты</h3>
        <div className="servings-control">
          <button
            type="button"
            onClick={decrement}
            disabled={currentServings === 1}
            aria-label="Уменьшить порции"
          >
            <MinusIcon size={14} />
          </button>
          <span className="value">{currentServings} порц.</span>
          <button
            type="button"
            onClick={increment}
            disabled={currentServings === 99}
            aria-label="Увеличить порции"
          >
            <PlusIcon size={14} />
          </button>
        </div>
      </div>

      {ingredients.length === 0 ? (
        <p className="t-small">Ингредиенты не указаны</p>
      ) : (
        <>
          <div className="ingredients-list">
            {ingredients.map((ing) => (
              <div key={ing.ingredientId} className="ingredient-row">
                <span className="name">{ing.title}</span>
                <span className="amount">
                  {scale(ing.amount)} {ing.unit}
                </span>
              </div>
            ))}
          </div>
          <p className="t-micro" style={{ marginTop: 8 }}>
            на {currentServings} порц.
          </p>
        </>
      )}
    </div>
  );
}
