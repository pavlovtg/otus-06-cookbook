import {
  IngredientSchema,
  IngredientRequestSchema,
  PagedIngredientSchema,
  type Ingredient,
  type IngredientCategory,
  type IngredientRequest,
  type PagedIngredient,
} from "@/lib/schemas/ingredient";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const SERVER_BASE = `${GATEWAY_URL}/api/cookbook/v1/ingredients`;
const CLIENT_BASE = `/api/cookbook/v1/ingredients`;

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
  if (params?.pageSize != null) url.searchParams.set("pageSize", String(params.pageSize));

  const response = await fetch(url.toString(), { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch ingredients: ${response.status}`);
  }

  const data: unknown = await response.json();
  return PagedIngredientSchema.parse(data);
}

export async function getIngredient(id: string): Promise<Ingredient> {
  const response = await fetch(`${SERVER_BASE}/${id}`, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch ingredient ${id}: ${response.status}`);
  }

  const data: unknown = await response.json();
  return IngredientSchema.parse(data);
}

export async function createIngredient(
  data: IngredientRequest,
): Promise<Ingredient> {
  const body = IngredientRequestSchema.parse(data);

  const response = await fetch(CLIENT_BASE, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to create ingredient: ${response.status}`);
  }

  const result: unknown = await response.json();
  return IngredientSchema.parse(result);
}

export async function updateIngredient(
  id: string,
  data: IngredientRequest,
): Promise<void> {
  const body = IngredientRequestSchema.parse(data);

  const response = await fetch(`${CLIENT_BASE}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to update ingredient ${id}: ${response.status}`);
  }
}

export async function deleteIngredient(id: string): Promise<void> {
  const response = await fetch(`${CLIENT_BASE}/${id}`, {
    method: "DELETE",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to delete ingredient ${id}: ${response.status}`);
  }
}
