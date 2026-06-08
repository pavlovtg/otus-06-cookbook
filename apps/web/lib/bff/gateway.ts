import { RecipeListSchema, type RecipeDto } from "@/lib/schemas/recipe";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";

export async function getRecipes(): Promise<RecipeDto[]> {
  const response = await fetch(`${GATEWAY_URL}/api/cookbook/recipes/v1`, {
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to fetch recipes: ${response.status}`);
  }

  const data: unknown = await response.json();
  return RecipeListSchema.parse(data);
}
