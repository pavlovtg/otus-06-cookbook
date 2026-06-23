import "server-only";

import { serverFetch } from "@/lib/server-fetch";
import { ShoppingListGroupSchema, type ShoppingListGroup } from "@/lib/schemas/shopping-list";
import { z } from "zod";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const SERVER_BASE = `${GATEWAY_URL}/api/cookbook/v1/shopping-list`;

export async function getShoppingList(): Promise<ShoppingListGroup[]> {
  const response = await serverFetch(SERVER_BASE, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch shopping list: ${response.status}`);
  }

  const data: unknown = await response.json();
  return z.array(ShoppingListGroupSchema).parse(data);
}
