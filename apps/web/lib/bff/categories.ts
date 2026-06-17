import {
  CategorySchema,
  CategoryRequestSchema,
  type Category,
  type CategoryRequest,
} from "@/lib/schemas/category";

const CLIENT_BASE = `/api/cookbook/v1/categories`;

export async function getCategories(): Promise<Category[]> {
  const response = await fetch(CLIENT_BASE, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch categories: ${response.status}`);
  }

  const data: unknown = await response.json();
  return CategorySchema.array().parse(data);
}

export async function createCategory(data: CategoryRequest): Promise<Category> {
  const body = CategoryRequestSchema.parse(data);

  const response = await fetch(CLIENT_BASE, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to create category: ${response.status}`);
  }

  const result: unknown = await response.json();
  return CategorySchema.parse(result);
}

export async function updateCategory(
  id: string,
  data: CategoryRequest,
): Promise<void> {
  const body = CategoryRequestSchema.parse(data);

  const response = await fetch(`${CLIENT_BASE}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to update category ${id}: ${response.status}`);
  }
}

export async function deleteCategory(id: string): Promise<void> {
  const response = await fetch(`${CLIENT_BASE}/${id}`, {
    method: "DELETE",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to delete category ${id}: ${response.status}`);
  }
}
