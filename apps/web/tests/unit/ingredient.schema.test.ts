import { describe, expect, it } from "vitest";
import {
  IngredientCategory,
  IngredientRequestSchema,
  IngredientSchema,
} from "@/lib/schemas/ingredient";

const validDto = {
  id: "aad1f839-df31-49f2-84d7-4dd01e04be77",
  title: "Морковь",
  unit: "г",
  defaultAmount: 100,
  category: "vegetables",
  isSystem: true,
};

const validRequest = {
  title: "Морковь",
  unit: "г",
  defaultAmount: 100,
  category: "vegetables",
};

describe("IngredientCategory", () => {
  it("принимает все допустимые значения категории", () => {
    const categories = [
      "vegetables",
      "fruits_and_berries",
      "meat_and_poultry",
      "fish_and_seafood",
      "dairy_and_eggs",
      "grains_and_cereals",
      "legumes",
      "nuts_and_seeds",
      "oils_and_fats",
      "spices_and_seasonings",
      "sauces_and_pastes",
      "bakery_and_sweets",
      "other",
    ];
    for (const category of categories) {
      const result = IngredientCategory.safeParse(category);
      expect(result.success).toBe(true);
    }
  });

  it("отклоняет неизвестную категорию", () => {
    const result = IngredientCategory.safeParse("unknown_category");
    expect(result.success).toBe(false);
  });
});

describe("IngredientSchema", () => {
  it("парсит корректный объект", () => {
    const result = IngredientSchema.safeParse(validDto);
    expect(result.success).toBe(true);
  });

  it("отклоняет некорректный uuid", () => {
    const result = IngredientSchema.safeParse({ ...validDto, id: "not-a-uuid" });
    expect(result.success).toBe(false);
  });

  it("отклоняет неизвестную категорию", () => {
    const result = IngredientSchema.safeParse({ ...validDto, category: "invalid" });
    expect(result.success).toBe(false);
  });

  it("отклоняет отсутствующий title", () => {
    const { title: _title, ...withoutTitle } = validDto;
    const result = IngredientSchema.safeParse(withoutTitle);
    expect(result.success).toBe(false);
  });

  it("отклоняет нечисловой defaultAmount", () => {
    const result = IngredientSchema.safeParse({ ...validDto, defaultAmount: "сто" });
    expect(result.success).toBe(false);
  });
});

describe("IngredientRequestSchema", () => {
  it("парсит корректный запрос", () => {
    const result = IngredientRequestSchema.safeParse(validRequest);
    expect(result.success).toBe(true);
  });

  it("отклоняет title короче 2 символов", () => {
    const result = IngredientRequestSchema.safeParse({ ...validRequest, title: "А" });
    expect(result.success).toBe(false);
  });

  it("отклоняет title длиннее 200 символов", () => {
    const result = IngredientRequestSchema.safeParse({
      ...validRequest,
      title: "А".repeat(201),
    });
    expect(result.success).toBe(false);
  });

  it("принимает title ровно 2 символа", () => {
    const result = IngredientRequestSchema.safeParse({ ...validRequest, title: "Аб" });
    expect(result.success).toBe(true);
  });

  it("отклоняет unit длиннее 20 символов", () => {
    const result = IngredientRequestSchema.safeParse({
      ...validRequest,
      unit: "г".repeat(21),
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет defaultAmount равный нулю", () => {
    const result = IngredientRequestSchema.safeParse({ ...validRequest, defaultAmount: 0 });
    expect(result.success).toBe(false);
  });

  it("отклоняет defaultAmount больше 100000", () => {
    const result = IngredientRequestSchema.safeParse({
      ...validRequest,
      defaultAmount: 100001,
    });
    expect(result.success).toBe(false);
  });

  it("принимает граничные значения defaultAmount", () => {
    expect(
      IngredientRequestSchema.safeParse({ ...validRequest, defaultAmount: 0.001 }).success
    ).toBe(true);
    expect(
      IngredientRequestSchema.safeParse({ ...validRequest, defaultAmount: 100000 }).success
    ).toBe(true);
  });

  it("отклоняет неизвестную категорию", () => {
    const result = IngredientRequestSchema.safeParse({
      ...validRequest,
      category: "unknown",
    });
    expect(result.success).toBe(false);
  });
});
