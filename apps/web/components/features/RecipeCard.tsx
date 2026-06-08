import type { RecipeDto } from "@/lib/schemas/recipe";

interface RecipeCardProps {
  recipe: RecipeDto;
}

export function RecipeCard({ recipe }: RecipeCardProps) {
  return (
    <article>
      <h2>{recipe.title}</h2>
      <p>{recipe.description}</p>
    </article>
  );
}
