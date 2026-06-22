import { describe, it, expect } from "vitest";
import {
  CommentDtoSchema,
  CommentRequestSchema,
  PagedResultCommentDtoSchema,
} from "@/lib/bff/comments";

const validComment = {
  id: "11111111-0000-0000-0000-000000000001",
  recipeId: "22222222-0000-0000-0000-000000000002",
  authorId: "33333333-0000-0000-0000-000000000003",
  authorName: "Анна Воронова",
  text: "Очень вкусно!",
  createdAt: "2026-06-09T14:20:00Z",
};

describe("CommentDtoSchema", () => {
  it("принимает корректный объект", () => {
    expect(CommentDtoSchema.safeParse(validComment).success).toBe(true);
  });

  it("отклоняет невалидный uuid в id", () => {
    expect(
      CommentDtoSchema.safeParse({ ...validComment, id: "not-a-uuid" }).success,
    ).toBe(false);
  });

  it("отклоняет невалидный uuid в recipeId", () => {
    expect(
      CommentDtoSchema.safeParse({ ...validComment, recipeId: "bad" }).success,
    ).toBe(false);
  });

  it("отклоняет невалидный uuid в authorId", () => {
    expect(
      CommentDtoSchema.safeParse({ ...validComment, authorId: "bad" }).success,
    ).toBe(false);
  });

  it("отклоняет объект без поля text", () => {
    const { text: _, ...rest } = validComment;
    expect(CommentDtoSchema.safeParse(rest).success).toBe(false);
  });

  it("отклоняет объект без поля createdAt", () => {
    const { createdAt: _, ...rest } = validComment;
    expect(CommentDtoSchema.safeParse(rest).success).toBe(false);
  });
});

describe("CommentRequestSchema", () => {
  it("принимает текст длиной 1 символ", () => {
    expect(CommentRequestSchema.safeParse({ text: "A" }).success).toBe(true);
  });

  it("принимает текст длиной 2000 символов", () => {
    expect(
      CommentRequestSchema.safeParse({ text: "A".repeat(2000) }).success,
    ).toBe(true);
  });

  it("отклоняет пустую строку", () => {
    expect(CommentRequestSchema.safeParse({ text: "" }).success).toBe(false);
  });

  it("отклоняет текст длиной 2001 символ", () => {
    expect(
      CommentRequestSchema.safeParse({ text: "A".repeat(2001) }).success,
    ).toBe(false);
  });

  it("отклоняет объект без поля text", () => {
    expect(CommentRequestSchema.safeParse({}).success).toBe(false);
  });
});

describe("PagedResultCommentDtoSchema", () => {
  it("принимает корректный объект с items", () => {
    const result = PagedResultCommentDtoSchema.safeParse({
      items: [validComment],
      total: 1,
      page: 1,
      pageSize: 10,
    });
    expect(result.success).toBe(true);
  });

  it("принимает пустой список items", () => {
    const result = PagedResultCommentDtoSchema.safeParse({
      items: [],
      total: 0,
      page: 1,
      pageSize: 10,
    });
    expect(result.success).toBe(true);
  });

  it("отклоняет отрицательный total", () => {
    const result = PagedResultCommentDtoSchema.safeParse({
      items: [],
      total: -1,
      page: 1,
      pageSize: 10,
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет page = 0", () => {
    const result = PagedResultCommentDtoSchema.safeParse({
      items: [],
      total: 0,
      page: 0,
      pageSize: 10,
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет объект без поля items", () => {
    const result = PagedResultCommentDtoSchema.safeParse({
      total: 0,
      page: 1,
      pageSize: 10,
    });
    expect(result.success).toBe(false);
  });
});
