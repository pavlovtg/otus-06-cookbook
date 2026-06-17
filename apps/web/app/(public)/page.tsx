export const dynamic = "force-dynamic";

import Link from "next/link";
import { Suspense } from "react";
import logger from "@/lib/logger";
import { getRecipes } from "@/lib/bff/recipes.server";
import { getCategories } from "@/lib/bff/categories.server";
import { getIngredients } from "@/lib/bff/ingredients.server";
import { getSession } from "@/lib/session";
import { RecipeCard } from "@/components/features/RecipeCard";
import { PaginationNav } from "@/components/ui/PaginationNav";
import {
  RecipesSearchInput,
  RecipesSortAside,
  RecipesModeAside,
} from "@/components/features/RecipesSearch";
import type { RecipeShortDto } from "@/lib/schemas/recipe";
import type { Category } from "@/lib/schemas/category";
import type { Ingredient } from "@/lib/schemas/ingredient";

export default async function HomePage({
  searchParams,
}: {
  searchParams: Promise<{ page?: string; q?: string; sort?: string; mode?: string }>;
}) {
  const params = await searchParams;
  const page = Math.max(1, parseInt(params.page ?? "1", 10) || 1);
  const pageSize = 18;
  const q = params.q ?? "";
  const sort = params.sort ?? "";
  const mode = params.mode ?? "";
  const favoritesMode = mode === "favorites";

  let recipes: RecipeShortDto[] = [];
  let categories: Category[] = [];
  let ingredients: Ingredient[] = [];
  let total = 0;

  const session = await getSession();
  const isAuthenticated = !!session.user;

  try {
    const [pagedResult, cats] = await Promise.all([
      getRecipes(
        page,
        pageSize,
        q || undefined,
        sort || undefined,
        favoritesMode && isAuthenticated ? true : undefined,
      ),
      getCategories(),
    ]);
    recipes = pagedResult.items;
    total = pagedResult.total;
    categories = cats;
  } catch (err) {
    logger.error({ err }, "Failed to load recipes or categories");
  }

  try {
    const ings = await getIngredients({ pageSize: 200 });
    ingredients = ings.items;
  } catch (err) {
    logger.error({ err }, "Failed to load ingredients for autocomplete");
  }

  const totalPages = Math.ceil(total / pageSize);

  return (
    <div className="layout-with-aside">
      <aside className="aside">
        <h1 className="aside-title">
          Готовим <span className="t-gradient">сегодня</span>
        </h1>
        {isAuthenticated && (
          <Suspense>
            <RecipesModeAside initialMode={mode} />
          </Suspense>
        )}
        <Suspense>
          <RecipesSortAside initialSort={sort} initialQ={q} />
        </Suspense>
      </aside>

      <div>
        <div className="toolbar">
          <Suspense>
            <RecipesSearchInput
              initialQ={q}
              categories={categories}
              ingredients={ingredients}
            />
          </Suspense>
          {isAuthenticated && (
            <Link href="/recipes/new" className="btn btn-primary">
              + Новый рецепт
            </Link>
          )}
        </div>

        {recipes.length === 0 ? (
          <div className="state">
            {favoritesMode && isAuthenticated ? (
              <>
                <div className="state-eyebrow">В избранном пусто</div>
                <p className="t-display">Сохраните любимое</p>
                <p className="t-small">
                  Нажмите на сердечко на карточке рецепта, чтобы добавить в избранное
                </p>
              </>
            ) : (
              <>
                <div className="state-eyebrow">Пусто</div>
                <p className="t-display">
                  {q ? "Ничего не нашли" : "Рецептов пока нет"}
                </p>
                {q ? (
                  <p className="t-small">Попробуйте другой запрос</p>
                ) : (
                  isAuthenticated && (
                    <Link href="/recipes/new" className="btn btn-ghost">
                      Добавить первый рецепт
                    </Link>
                  )
                )}
              </>
            )}
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
                  <RecipeCard
                    recipe={recipe}
                    categories={categories}
                    showFavorite={isAuthenticated}
                  />
                </Link>
              ))}
            </div>
            <PaginationNav page={page} totalPages={totalPages} />
          </>
        )}
      </div>
    </div>
  );
}
