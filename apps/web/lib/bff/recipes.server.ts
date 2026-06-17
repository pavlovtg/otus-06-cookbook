import "server-only";

import { serverFetch } from "@/lib/server-fetch";
import {
  RecipeDtoSchema,
  RecipePagedResultSchema,
  type RecipeDto,
  type RecipePagedResult,
} from "@/lib/schemas/recipe";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const SERVER_BASE = `${GATEWAY_URL}/api/cookbook/v1/recipes`;

export async function getRecipes(
  page = 1,
  pageSize = 18,
  q?: string,
  sort?: string,
): Promise<RecipePagedResult> {
  const params = new URLSearchParams();
  params.set("page", String(page));
  params.set("pageSize", String(pageSize));
  if (q) params.set("q", q);
  if (sort) params.set("sort", sort);
  const url = `${SERVER_BASE}?${params.toString()}`;
  const response = await serverFetch(url, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch recipes: ${response.status}`);
  }

  const data: unknown = await response.json();
  return RecipePagedResultSchema.parse(data);
}

export async function getRecipe(id: string): Promise<RecipeDto> {
  const response = await serverFetch(`${SERVER_BASE}/${id}`, {
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to fetch recipe ${id}: ${response.status}`);
  }

  const data: unknown = await response.json();
  return RecipeDtoSchema.parse(data);
}
