import {
  RecipeDtoSchema,
  RecipeListSchema,
  RecipeRequestSchema,
  type RecipeDto,
  type RecipeRequest,
} from "@/lib/schemas/recipe";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const BASE = `${GATEWAY_URL}/api/cookbook/recipes/v1`;

export async function getRecipes(): Promise<RecipeDto[]> {
  const response = await fetch(BASE, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch recipes: ${response.status}`);
  }

  const data: unknown = await response.json();
  return RecipeListSchema.parse(data);
}

export async function getRecipe(id: string): Promise<RecipeDto> {
  const response = await fetch(`${BASE}/${id}`, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch recipe ${id}: ${response.status}`);
  }

  const data: unknown = await response.json();
  return RecipeDtoSchema.parse(data);
}

export async function createRecipe(data: RecipeRequest): Promise<RecipeDto> {
  const body = RecipeRequestSchema.parse(data);

  const response = await fetch(BASE, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to create recipe: ${response.status}`);
  }

  const result: unknown = await response.json();
  return RecipeDtoSchema.parse(result);
}

export async function updateRecipe(
  id: string,
  data: RecipeRequest,
): Promise<void> {
  const body = RecipeRequestSchema.parse(data);

  const response = await fetch(`${BASE}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to update recipe ${id}: ${response.status}`);
  }
}

export async function deleteRecipe(id: string): Promise<void> {
  const response = await fetch(`${BASE}/${id}`, {
    method: "DELETE",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to delete recipe ${id}: ${response.status}`);
  }
}
