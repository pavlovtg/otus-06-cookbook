import "server-only";

import { serverFetch } from "@/lib/server-fetch";
import { CategorySchema, type Category } from "@/lib/schemas/category";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const SERVER_BASE = `${GATEWAY_URL}/api/cookbook/v1/categories`;

export async function getCategories(): Promise<Category[]> {
  const response = await serverFetch(SERVER_BASE, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch categories: ${response.status}`);
  }

  const data: unknown = await response.json();
  return CategorySchema.array().parse(data);
}
