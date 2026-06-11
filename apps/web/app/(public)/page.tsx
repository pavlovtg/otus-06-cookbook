import Link from "next/link";
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
    <>
      <div className="page-head">
        <div className="left">
          <h1 className="t-heading">Рецепты</h1>
        </div>
        <Link href="/recipes/new" className="btn btn-primary">
          + Новый рецепт
        </Link>
      </div>

      {recipes.length === 0 ? (
        <div className="state">
          <div className="state-eyebrow">Пусто</div>
          <p className="t-display">Рецептов пока нет</p>
          <Link href="/recipes/new" className="btn btn-ghost">
            Добавить первый рецепт
          </Link>
        </div>
      ) : (
        <div className="recipes-grid">
          {recipes.map((recipe) => (
            <Link key={recipe.id} href={`/recipes/${recipe.id}`} style={{ textDecoration: "none", color: "inherit" }}>
              <RecipeCard recipe={recipe} />
            </Link>
          ))}
        </div>
      )}
    </>
  );
}
