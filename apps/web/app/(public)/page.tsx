export const dynamic = "force-dynamic";

import Link from "next/link";
import logger from "@/lib/logger";
import { getRecipes } from "@/lib/bff/recipes";
import { RecipeCard } from "@/components/features/RecipeCard";
import type { RecipeShortDto } from "@/lib/schemas/recipe";

export default async function HomePage() {
  let recipes: RecipeShortDto[] = [];
  try {
    recipes = await getRecipes();
  } catch (err) {
    logger.error({ err }, "Failed to load recipes");
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
