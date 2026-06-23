export const dynamic = "force-dynamic";

import { redirect } from "next/navigation";
import { getSession } from "@/lib/session";
import { getMealPlan } from "@/lib/bff/meal-plan.server";
import { getRecipes } from "@/lib/bff/recipes.server";
import { dtoToPlan, emptyPlan } from "@/lib/planner-utils";
import { PlannerPageClient } from "./PlannerPageClient";
import logger from "@/lib/logger";
import type { RecipeShortDto } from "@/lib/schemas/recipe";

export default async function PlannerPage() {
  const session = await getSession();
  if (!session.user) {
    redirect("/login");
  }

  let initialPlan = emptyPlan();
  try {
    const dto = await getMealPlan();
    initialPlan = dtoToPlan(dto);
  } catch (err) {
    logger.error({ err }, "Failed to load meal plan");
  }

  let recipes: RecipeShortDto[] = [];
  try {
    const result = await getRecipes(1, 200);
    recipes = result.items;
  } catch (err) {
    logger.error({ err }, "Failed to load recipes for planner");
  }

  return (
    <PlannerPageClient
      initialPlan={initialPlan}
      recipes={recipes}
      currentUserId={session.user.id}
    />
  );
}
