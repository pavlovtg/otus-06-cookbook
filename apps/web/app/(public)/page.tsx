import { getRecipes } from "@/lib/bff/gateway";
import { RecipeCard } from "@/components/features/RecipeCard";
import type { RecipeDto } from "@/lib/schemas/recipe";

export default async function HomePage() {
  let recipes: RecipeDto[] = [];
  try {
    recipes = await getRecipes();
  } catch {
    recipes = [];
  }

  return (
    <main>
      <h1>Рецепты</h1>
      <ul>
        {recipes.map((recipe) => (
          <li key={recipe.id}>
            <RecipeCard recipe={recipe} />
          </li>
        ))}
      </ul>
    </main>
  );
}
