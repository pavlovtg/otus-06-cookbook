"use client";

import { useRouter } from "next/navigation";
import { updateRecipe } from "@/lib/bff/recipes";
import { RecipeForm } from "@/components/features/RecipeForm";
import type { RecipeDto, RecipeRequest } from "@/lib/schemas/recipe";

interface Props {
  recipe: RecipeDto;
}

export function EditRecipeForm({ recipe }: Props) {
  const router = useRouter();

  async function handleSubmit(data: RecipeRequest) {
    await updateRecipe(recipe.id, data);
    router.push(`/recipes/${recipe.id}`);
    router.refresh();
  }

  return (
    <RecipeForm
      initialValues={{
        title: recipe.title,
        description: recipe.description,
        cookingTime: recipe.cookingTime,
        difficulty: recipe.difficulty,
        servings: recipe.servings,
        instructions: recipe.instructions,
        ingredients: recipe.ingredients.map((ing) => ({
          ingredientId: ing.ingredientId,
          amount: ing.amount,
        })),
      }}
      onSubmit={handleSubmit}
      submitLabel="Сохранить изменения"
    />
  );
}
