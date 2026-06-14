import { describe, it, expect } from "vitest";
import {
  RecipeDtoSchema,
  RecipeIngredientDtoSchema,
  RecipeRequestSchema,
  RecipeListSchema,
} from "@/lib/schemas/recipe";

const validDto = {
  id: "11111111-0000-0000-0000-000000000001",
  title: "Борщ",
  description: "Классический борщ",
  cookingTime: 120,
  difficulty: "everyday",
  servings: 6,
  instructions: "1. Сварить бульон.",
  ingredients: [],
};

const validRequest = {
  title: "Борщ",
  description: "Классический борщ",
  cookingTime: 120,
  difficulty: "everyday",
  servings: 6,
  instructions: "1. Сварить бульон.",
  ingredients: [],
};

const validShortDto = {
  id: "11111111-0000-0000-0000-000000000001",
  title: "Борщ",
  description: "Классический борщ",
  cookingTime: 120,
  difficulty: "everyday",
};

describe("RecipeDtoSchema", () => {
  it("парсит корректный объект", () => {
    const result = RecipeDtoSchema.safeParse(validDto);
    expect(result.success).toBe(true);
  });

  it("отклоняет некорректный uuid", () => {
    const result = RecipeDtoSchema.safeParse({ ...validDto, id: "not-a-uuid" });
    expect(result.success).toBe(false);
  });

  it("отклоняет отрицательный cookingTime", () => {
    const result = RecipeDtoSchema.safeParse({ ...validDto, cookingTime: -1 });
    expect(result.success).toBe(false);
  });

  it("отклоняет неизвестный difficulty", () => {
    const result = RecipeDtoSchema.safeParse({
      ...validDto,
      difficulty: "unknown",
    });
    expect(result.success).toBe(false);
  });

  it("принимает все допустимые значения difficulty", () => {
    const difficulties = [
      "easy",
      "everyday",
      "festive",
      "restaurant",
      "signature",
    ];
    for (const difficulty of difficulties) {
      const result = RecipeDtoSchema.safeParse({ ...validDto, difficulty });
      expect(result.success).toBe(true);
    }
  });
});

describe("RecipeRequestSchema", () => {
  it("парсит корректный запрос", () => {
    const result = RecipeRequestSchema.safeParse(validRequest);
    expect(result.success).toBe(true);
  });

  it("отклоняет пустой title", () => {
    const result = RecipeRequestSchema.safeParse({
      ...validRequest,
      title: "",
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет title длиннее 200 символов", () => {
    const result = RecipeRequestSchema.safeParse({
      ...validRequest,
      title: "a".repeat(201),
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет пустое description", () => {
    const result = RecipeRequestSchema.safeParse({
      ...validRequest,
      description: "",
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет нулевой servings", () => {
    const result = RecipeRequestSchema.safeParse({
      ...validRequest,
      servings: 0,
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет нулевой cookingTime", () => {
    const result = RecipeRequestSchema.safeParse({
      ...validRequest,
      cookingTime: 0,
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет пустые instructions", () => {
    const result = RecipeRequestSchema.safeParse({
      ...validRequest,
      instructions: "",
    });
    expect(result.success).toBe(false);
  });
});

describe("RecipeListSchema", () => {
  it("парсит массив рецептов", () => {
    const result = RecipeListSchema.safeParse([validShortDto, validShortDto]);
    expect(result.success).toBe(true);
    if (result.success) {
      expect(result.data).toHaveLength(2);
    }
  });

  it("парсит пустой массив", () => {
    const result = RecipeListSchema.safeParse([]);
    expect(result.success).toBe(true);
  });

  it("отклоняет массив с некорректным элементом", () => {
    const result = RecipeListSchema.safeParse([{ ...validShortDto, id: "bad" }]);
    expect(result.success).toBe(false);
  });
});

// ── RecipeIngredientDtoSchema (8.9) ───────────────────────────────────────────

const validIngredientDto = {
  ingredientId: "22222222-0000-0000-0000-000000000002",
  title: "Морковь",
  amount: 150,
  unit: "г",
};

describe("RecipeIngredientDtoSchema", () => {
  it("парсит корректный объект", () => {
    const result = RecipeIngredientDtoSchema.safeParse(validIngredientDto);
    expect(result.success).toBe(true);
  });

  it("отклоняет некорректный ingredientId (не uuid)", () => {
    const result = RecipeIngredientDtoSchema.safeParse({
      ...validIngredientDto,
      ingredientId: "not-a-uuid",
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет нулевой amount", () => {
    const result = RecipeIngredientDtoSchema.safeParse({
      ...validIngredientDto,
      amount: 0,
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет отрицательный amount", () => {
    const result = RecipeIngredientDtoSchema.safeParse({
      ...validIngredientDto,
      amount: -1,
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет объект без поля title", () => {
    const { title: _, ...withoutTitle } = validIngredientDto;
    const result = RecipeIngredientDtoSchema.safeParse(withoutTitle);
    expect(result.success).toBe(false);
  });

  it("отклоняет объект без поля unit", () => {
    const { unit: _, ...withoutUnit } = validIngredientDto;
    const result = RecipeIngredientDtoSchema.safeParse(withoutUnit);
    expect(result.success).toBe(false);
  });

  it("принимает дробный amount", () => {
    const result = RecipeIngredientDtoSchema.safeParse({
      ...validIngredientDto,
      amount: 0.001,
    });
    expect(result.success).toBe(true);
  });
});
