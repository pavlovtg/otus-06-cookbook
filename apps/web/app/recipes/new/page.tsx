"use client";

import { useRouter } from "next/navigation";
import { createRecipe } from "@/lib/bff/recipes";
import { RecipeForm } from "@/components/features/RecipeForm";
import type { RecipeRequest } from "@/lib/schemas/recipe";

export default function NewRecipePage() {
  const router = useRouter();

  async function handleSubmit(data: RecipeRequest) {
    const recipe = await createRecipe(data);
    router.push(`/recipes/${recipe.id}`);
    router.refresh();
  }

  return (
    <>
      <div className="page-head">
        <div className="left">
          <h1 className="t-heading">Новый рецепт</h1>
        </div>
      </div>
      <RecipeForm onSubmit={handleSubmit} submitLabel="Создать рецепт" />
    </>
  );
}
