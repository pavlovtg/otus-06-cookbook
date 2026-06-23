import "server-only";

import { serverFetch } from "@/lib/server-fetch";
import {
  DashboardStatsDtoSchema,
  type DashboardStatsDto,
} from "@/lib/schemas/dashboard";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const UPSTREAM = `${GATEWAY_URL}/api/cookbook/v1/dashboard`;

export async function getDashboardStats(): Promise<DashboardStatsDto> {
  const response = await serverFetch(UPSTREAM, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to fetch dashboard stats: ${response.status}`);
  }

  const data: unknown = await response.json();
  return DashboardStatsDtoSchema.parse(data);
}
