import { z } from "zod";

const CLIENT_BASE = `/api/cookbook/v1/recipes`;

export const CommentDtoSchema = z.object({
  id: z.string().uuid(),
  recipeId: z.string().uuid(),
  authorId: z.string().uuid(),
  authorName: z.string(),
  text: z.string(),
  createdAt: z.string(),
});

export type CommentDto = z.infer<typeof CommentDtoSchema>;

export const CommentRequestSchema = z.object({
  text: z.string().min(1).max(2000),
});

export type CommentRequest = z.infer<typeof CommentRequestSchema>;

export const PagedResultCommentDtoSchema = z.object({
  items: z.array(CommentDtoSchema),
  total: z.number().int().nonnegative(),
  page: z.number().int().positive(),
  pageSize: z.number().int().positive(),
});

export type PagedResultCommentDto = z.infer<typeof PagedResultCommentDtoSchema>;

export async function getComments(
  recipeId: string,
  page = 1,
  pageSize = 10,
): Promise<PagedResultCommentDto> {
  const url = `${CLIENT_BASE}/${recipeId}/comments?page=${page}&pageSize=${pageSize}`;
  const response = await fetch(url, { cache: "no-store" });

  if (!response.ok) {
    throw new Error(`Failed to get comments: ${response.status}`);
  }

  const data: unknown = await response.json();
  return PagedResultCommentDtoSchema.parse(data);
}

export async function addComment(
  recipeId: string,
  text: string,
): Promise<CommentDto> {
  const body = CommentRequestSchema.parse({ text });

  const response = await fetch(`${CLIENT_BASE}/${recipeId}/comments`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to add comment: ${response.status}`);
  }

  const data: unknown = await response.json();
  return CommentDtoSchema.parse(data);
}

export async function deleteComment(
  recipeId: string,
  commentId: string,
): Promise<void> {
  const response = await fetch(
    `${CLIENT_BASE}/${recipeId}/comments/${commentId}`,
    {
      method: "DELETE",
      cache: "no-store",
    },
  );

  if (!response.ok) {
    throw new Error(`Failed to delete comment: ${response.status}`);
  }
}
