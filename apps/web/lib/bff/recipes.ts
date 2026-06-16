import {
  RecipeDtoSchema,
  RecipePagedResultSchema,
  RecipeRequestSchema,
  type RecipeDto,
  type RecipePagedResult,
  type RecipeRequest,
} from "@/lib/schemas/recipe";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const REVERSE_PROXY_URL =
  process.env["REVERSE_PROXY_URL"] ?? "http://reverse-proxy";
const SERVER_BASE = `${GATEWAY_URL}/api/cookbook/v1/recipes`;
const CLIENT_BASE = `/api/cookbook/v1/recipes`;

async function purgePhotoCache(photoId: string): Promise<void> {
  const urls = [
    `${REVERSE_PROXY_URL}/purge/api/cookbook/v1/photos/${photoId}`,
    `${REVERSE_PROXY_URL}/purge/api/cookbook/v1/photos/${photoId}/thumbnail`,
  ];
  await Promise.allSettled(
    urls.map((url) => fetch(url, { method: "PURGE", cache: "no-store" })),
  );
}

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
  const response = await fetch(url, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch recipes: ${response.status}`);
  }

  const data: unknown = await response.json();
  return RecipePagedResultSchema.parse(data);
}

export async function getRecipe(id: string): Promise<RecipeDto> {
  const response = await fetch(`${SERVER_BASE}/${id}`, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch recipe ${id}: ${response.status}`);
  }

  const data: unknown = await response.json();
  return RecipeDtoSchema.parse(data);
}

export async function createRecipe(data: RecipeRequest): Promise<RecipeDto> {
  const body = RecipeRequestSchema.parse(data);

  const response = await fetch(CLIENT_BASE, {
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

  const response = await fetch(`${CLIENT_BASE}/${id}`, {
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
  const response = await fetch(`${CLIENT_BASE}/${id}`, {
    method: "DELETE",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to delete recipe ${id}: ${response.status}`);
  }
}

export async function uploadRecipePhoto(
  recipeId: string,
  file: File,
): Promise<void> {
  const formData = new FormData();
  formData.append("file", file);

  const response = await fetch(`${CLIENT_BASE}/${recipeId}/photo`, {
    method: "POST",
    body: formData,
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to upload photo: ${response.status}`);
  }

  const data: unknown = await response.json();
  const photoId =
    data != null &&
    typeof data === "object" &&
    "photoId" in data &&
    typeof (data as Record<string, unknown>)["photoId"] === "string"
      ? ((data as Record<string, unknown>)["photoId"] as string)
      : null;

  if (photoId) {
    await purgePhotoCache(photoId);
  }
}

export async function deleteRecipePhoto(
  recipeId: string,
  photoId: string,
): Promise<void> {
  const response = await fetch(`${CLIENT_BASE}/${recipeId}/photo`, {
    method: "DELETE",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to delete photo: ${response.status}`);
  }

  await purgePhotoCache(photoId);
}
