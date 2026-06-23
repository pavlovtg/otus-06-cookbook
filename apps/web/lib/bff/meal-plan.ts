"use server";

import { serverFetch } from "@/lib/server-fetch";
import {
  MealPlanRequestSchema,
  type MealPlanRequest,
} from "@/lib/schemas/meal-plan";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const SERVER_BASE = `${GATEWAY_URL}/api/cookbook/v1/meal-plan`;

export async function updateMealPlan(data: MealPlanRequest): Promise<void> {
  const body = MealPlanRequestSchema.parse(data);

  const response = await serverFetch(SERVER_BASE, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to update meal plan: ${response.status}`);
  }
}

export async function clearMealPlan(): Promise<void> {
  const response = await serverFetch(SERVER_BASE, {
    method: "DELETE",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to clear meal plan: ${response.status}`);
  }
}
