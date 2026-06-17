import { z } from "zod";

const CLIENT_BASE = `/api/cookbook/v1/recipes`;

export const RatingSummaryDtoSchema = z.object({
  averageRating: z.number().nullable(),
  myRating: z.number().int().min(1).max(5).nullable(),
});

export type RatingSummaryDto = z.infer<typeof RatingSummaryDtoSchema>;

export async function setRating(
  recipeId: string,
  value: number,
): Promise<RatingSummaryDto> {
  const response = await fetch(`${CLIENT_BASE}/${recipeId}/rating`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ value }),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to set rating: ${response.status}`);
  }

  const data: unknown = await response.json();
  return RatingSummaryDtoSchema.parse(data);
}

export async function deleteRating(recipeId: string): Promise<void> {
  const response = await fetch(`${CLIENT_BASE}/${recipeId}/rating`, {
    method: "DELETE",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to delete rating: ${response.status}`);
  }
}
