import "server-only";

import { serverFetch } from "@/lib/server-fetch";
import { MealPlanDtoSchema, type MealPlanDto } from "@/lib/schemas/meal-plan";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const SERVER_BASE = `${GATEWAY_URL}/api/cookbook/v1/meal-plan`;

export async function getMealPlan(): Promise<MealPlanDto> {
  const response = await serverFetch(SERVER_BASE, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch meal plan: ${response.status}`);
  }

  const data: unknown = await response.json();
  return MealPlanDtoSchema.parse(data);
}
