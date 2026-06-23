import "server-only";

import { serverFetch } from "@/lib/server-fetch";
import {
  PagedResultCommentDtoSchema,
  type PagedResultCommentDto,
} from "./comments";

const GATEWAY_URL = process.env["GATEWAY_URL"] ?? "http://api-gateway";
const SERVER_BASE = `${GATEWAY_URL}/api/cookbook/v1/recipes`;

export async function getComments(
  recipeId: string,
  page = 1,
  pageSize = 10,
): Promise<PagedResultCommentDto> {
  const url = `${SERVER_BASE}/${recipeId}/comments?page=${page}&pageSize=${pageSize}`;
  const response = await serverFetch(url, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to get comments: ${response.status}`);
  }

  const data: unknown = await response.json();
  return PagedResultCommentDtoSchema.parse(data);
}
