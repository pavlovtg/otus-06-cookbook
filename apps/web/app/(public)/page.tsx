export const dynamic = "force-dynamic";

import Link from "next/link";
import logger from "@/lib/logger";
import { getRecipes } from "@/lib/bff/recipes";
import { getCategories } from "@/lib/bff/categories";
import { RecipeCard } from "@/components/features/RecipeCard";
import { PaginationNav } from "@/components/ui/PaginationNav";
import type { RecipeShortDto } from "@/lib/schemas/recipe";
import type { Category } from "@/lib/schemas/category";

export default async function HomePage({
  searchParams,
}: {
  searchParams: Promise<{ page?: string }>;
}) {
  const params = await searchParams;
  const page = Math.max(1, parseInt(params.page ?? "1", 10) || 1);
  const pageSize = 18;

  let recipes: RecipeShortDto[] = [];
  let categories: Category[] = [];
  let total = 0;

  try {
    const [pagedResult, cats] = await Promise.all([
      getRecipes(page, pageSize),
      getCategories(),
    ]);
    recipes = pagedResult.items;
    total = pagedResult.total;
    categories = cats;
  } catch (err) {
    logger.error({ err }, "Failed to load recipes or categories");
  }

  const totalPages = Math.ceil(total / pageSize);

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
        <>
          <div className="recipes-grid">
            {recipes.map((recipe) => (
              <Link
                key={recipe.id}
                href={`/recipes/${recipe.id}?page=${page}`}
                style={{ textDecoration: "none", color: "inherit" }}
              >
                <RecipeCard recipe={recipe} categories={categories} />
              </Link>
            ))}
          </div>
          <PaginationNav page={page} totalPages={totalPages} />
        </>
      )}
    </>
  );
}
