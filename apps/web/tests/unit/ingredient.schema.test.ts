import { describe, it, expect } from "vitest";
import {
  IngredientSchema,
  IngredientRequestSchema,
  PagedIngredientSchema,
  pagedIngredientSchema,
} from "@/lib/schemas/ingredient";

const validIngredient = {
  id: "550e8400-e29b-41d4-a716-446655440000",
  title: "Морковь",
  unit: "г",
  defaultAmount: 100,
  category: "vegetables",
  isSystem: false,
};

const validPaged = {
  items: [validIngredient],
  total: 1,
  page: 1,
  pageSize: 100,
};

describe("IngredientSchema", () => {
  it("parses a valid ingredient", () => {
    const result = IngredientSchema.safeParse(validIngredient);
    expect(result.success).toBe(true);
  });

  it("rejects unknown category", () => {
    const result = IngredientSchema.safeParse({
      ...validIngredient,
      category: "unknown_category",
    });
    expect(result.success).toBe(false);
  });

  it("rejects missing id", () => {
    const { id: _id, ...rest } = validIngredient;
    const result = IngredientSchema.safeParse(rest);
    expect(result.success).toBe(false);
  });

  it("rejects non-uuid id", () => {
    const result = IngredientSchema.safeParse({
      ...validIngredient,
      id: "not-a-uuid",
    });
    expect(result.success).toBe(false);
  });
});

describe("IngredientRequestSchema", () => {
  const validRequest = {
    title: "Морковь",
    unit: "г",
    defaultAmount: 100,
    category: "vegetables",
  };

  it("parses a valid request", () => {
    const result = IngredientRequestSchema.safeParse(validRequest);
    expect(result.success).toBe(true);
  });

  it("rejects title shorter than 2 chars", () => {
    const result = IngredientRequestSchema.safeParse({
      ...validRequest,
      title: "А",
    });
    expect(result.success).toBe(false);
  });

  it("rejects title longer than 200 chars", () => {
    const result = IngredientRequestSchema.safeParse({
      ...validRequest,
      title: "А".repeat(201),
    });
    expect(result.success).toBe(false);
  });

  it("accepts title of exactly 200 chars", () => {
    const result = IngredientRequestSchema.safeParse({
      ...validRequest,
      title: "А".repeat(200),
    });
    expect(result.success).toBe(true);
  });

  it("rejects defaultAmount = 0", () => {
    const result = IngredientRequestSchema.safeParse({
      ...validRequest,
      defaultAmount: 0,
    });
    expect(result.success).toBe(false);
  });

  it("rejects defaultAmount > 100000", () => {
    const result = IngredientRequestSchema.safeParse({
      ...validRequest,
      defaultAmount: 100001,
    });
    expect(result.success).toBe(false);
  });
});

describe("PagedIngredientSchema", () => {
  it("parses a valid paged result", () => {
    const result = PagedIngredientSchema.safeParse(validPaged);
    expect(result.success).toBe(true);
  });

  it("parses empty items list", () => {
    const result = PagedIngredientSchema.safeParse({
      ...validPaged,
      items: [],
      total: 0,
    });
    expect(result.success).toBe(true);
  });

  it("rejects negative total", () => {
    const result = PagedIngredientSchema.safeParse({
      ...validPaged,
      total: -1,
    });
    expect(result.success).toBe(false);
  });

  it("rejects page = 0", () => {
    const result = PagedIngredientSchema.safeParse({
      ...validPaged,
      page: 0,
    });
    expect(result.success).toBe(false);
  });

  it("rejects pageSize = 0", () => {
    const result = PagedIngredientSchema.safeParse({
      ...validPaged,
      pageSize: 0,
    });
    expect(result.success).toBe(false);
  });

  it("rejects missing items field", () => {
    const { items: _items, ...rest } = validPaged;
    const result = PagedIngredientSchema.safeParse(rest);
    expect(result.success).toBe(false);
  });

  it("rejects missing total field", () => {
    const { total: _total, ...rest } = validPaged;
    const result = PagedIngredientSchema.safeParse(rest);
    expect(result.success).toBe(false);
  });

  it("rejects invalid ingredient inside items", () => {
    const result = PagedIngredientSchema.safeParse({
      ...validPaged,
      items: [{ ...validIngredient, category: "bad_category" }],
    });
    expect(result.success).toBe(false);
  });

  it("pagedIngredientSchema() factory returns same schema", () => {
    const schema = pagedIngredientSchema();
    const result = schema.safeParse(validPaged);
    expect(result.success).toBe(true);
  });
});
