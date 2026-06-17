import "server-only";

import { serverFetch } from "@/lib/server-fetch";
import {
  IngredientSchema,
  PagedIngredientSchema,
  type Ingredient,
  type IngredientCategory,
  type PagedIngredient,
} from "@/lib/schemas/ingredient";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const SERVER_BASE = `${GATEWAY_URL}/api/cookbook/v1/ingredients`;

export async function getIngredients(params?: {
  title?: string;
  category?: IngredientCategory;
  page?: number;
  pageSize?: number;
}): Promise<PagedIngredient> {
  const url = new URL(SERVER_BASE);
  if (params?.title) url.searchParams.set("title", params.title);
  if (params?.category) url.searchParams.set("category", params.category);
  if (params?.page != null) url.searchParams.set("page", String(params.page));
  if (params?.pageSize != null)
    url.searchParams.set("pageSize", String(params.pageSize));

  const response = await serverFetch(url.toString(), { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch ingredients: ${response.status}`);
  }

  const data: unknown = await response.json();
  return PagedIngredientSchema.parse(data);
}

export async function getIngredient(id: string): Promise<Ingredient> {
  const response = await serverFetch(`${SERVER_BASE}/${id}`, {
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to fetch ingredient ${id}: ${response.status}`);
  }

  const data: unknown = await response.json();
  return IngredientSchema.parse(data);
}
