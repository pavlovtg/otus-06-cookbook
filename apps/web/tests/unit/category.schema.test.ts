import { describe, it, expect } from "vitest";
import {
  CategorySchema,
  CategoryRequestSchema,
  CategoryType,
  CategoryTypeLabels,
} from "@/lib/schemas/category";

const validCategory = {
  id: "550e8400-e29b-41d4-a716-446655440000",
  name: "Итальянская",
  description: "Блюда итальянской кухни",
  type: "cuisine",
};

describe("CategorySchema", () => {
  it("parses a valid category", () => {
    const result = CategorySchema.safeParse(validCategory);
    expect(result.success).toBe(true);
  });

  it("parses category without description", () => {
    const { description: _d, ...rest } = validCategory;
    const result = CategorySchema.safeParse(rest);
    expect(result.success).toBe(true);
  });

  it("parses category with null description", () => {
    const result = CategorySchema.safeParse({ ...validCategory, description: null });
    expect(result.success).toBe(true);
  });

  it("rejects unknown type", () => {
    const result = CategorySchema.safeParse({ ...validCategory, type: "unknown_type" });
    expect(result.success).toBe(false);
  });

  it("rejects missing id", () => {
    const { id: _id, ...rest } = validCategory;
    const result = CategorySchema.safeParse(rest);
    expect(result.success).toBe(false);
  });

  it("rejects non-uuid id", () => {
    const result = CategorySchema.safeParse({ ...validCategory, id: "not-a-uuid" });
    expect(result.success).toBe(false);
  });

  it("rejects missing name", () => {
    const { name: _n, ...rest } = validCategory;
    const result = CategorySchema.safeParse(rest);
    expect(result.success).toBe(false);
  });

  it("accepts all 7 valid types", () => {
    for (const type of CategoryType.options) {
      const result = CategorySchema.safeParse({ ...validCategory, type });
      expect(result.success).toBe(true);
    }
  });
});

describe("CategoryRequestSchema", () => {
  const validRequest = {
    name: "Итальянская",
    description: "Блюда итальянской кухни",
    type: "cuisine",
  };

  it("parses a valid request", () => {
    const result = CategoryRequestSchema.safeParse(validRequest);
    expect(result.success).toBe(true);
  });

  it("parses request without description", () => {
    const { description: _d, ...rest } = validRequest;
    const result = CategoryRequestSchema.safeParse(rest);
    expect(result.success).toBe(true);
  });

  it("parses request with null description", () => {
    const result = CategoryRequestSchema.safeParse({ ...validRequest, description: null });
    expect(result.success).toBe(true);
  });

  it("rejects empty name", () => {
    const result = CategoryRequestSchema.safeParse({ ...validRequest, name: "" });
    expect(result.success).toBe(false);
  });

  it("rejects name longer than 200 chars", () => {
    const result = CategoryRequestSchema.safeParse({
      ...validRequest,
      name: "А".repeat(201),
    });
    expect(result.success).toBe(false);
  });

  it("accepts name of exactly 200 chars", () => {
    const result = CategoryRequestSchema.safeParse({
      ...validRequest,
      name: "А".repeat(200),
    });
    expect(result.success).toBe(true);
  });

  it("rejects description longer than 2000 chars", () => {
    const result = CategoryRequestSchema.safeParse({
      ...validRequest,
      description: "А".repeat(2001),
    });
    expect(result.success).toBe(false);
  });

  it("accepts description of exactly 2000 chars", () => {
    const result = CategoryRequestSchema.safeParse({
      ...validRequest,
      description: "А".repeat(2000),
    });
    expect(result.success).toBe(true);
  });

  it("rejects unknown type", () => {
    const result = CategoryRequestSchema.safeParse({ ...validRequest, type: "bad_type" });
    expect(result.success).toBe(false);
  });

  it("rejects missing type", () => {
    const { type: _t, ...rest } = validRequest;
    const result = CategoryRequestSchema.safeParse(rest);
    expect(result.success).toBe(false);
  });
});

describe("CategoryType enum", () => {
  it("has exactly 7 values", () => {
    expect(CategoryType.options).toHaveLength(7);
  });

  it("contains all expected values", () => {
    const expected = [
      "meal_role",
      "cooking_method",
      "main_ingredient",
      "cuisine",
      "meal_time",
      "dietary",
      "serving_form",
    ];
    expect(CategoryType.options).toEqual(expected);
  });
});

describe("CategoryTypeLabels", () => {
  it("has a label for every CategoryType value", () => {
    for (const type of CategoryType.options) {
      expect(CategoryTypeLabels[type]).toBeTruthy();
    }
  });
});
