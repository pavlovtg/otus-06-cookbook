import { describe, it, expect } from "vitest";
import {
  RecipeDtoSchema,
  RecipeIngredientDtoSchema,
  RecipeRequestSchema,
  RecipeListSchema,
  RecipeShortDtoSchema,
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
  photoId: null,
  categoryIds: [],
  isPublic: true,
  authorName: "Анна Воронова",
};

const validRequest = {
  title: "Борщ",
  description: "Классический борщ",
  cookingTime: 120,
  difficulty: "everyday",
  servings: 6,
  instructions: "1. Сварить бульон.",
  ingredients: [],
  categoryIds: [],
  isPublic: true,
};

const validShortDto = {
  id: "11111111-0000-0000-0000-000000000001",
  title: "Борщ",
  description: "Классический борщ",
  cookingTime: 120,
  difficulty: "everyday",
  photoId: null,
  categoryIds: [],
  isPublic: true,
  authorName: null,
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

  it("принимает isPublic = false", () => {
    const result = RecipeRequestSchema.safeParse({
      ...validRequest,
      isPublic: false,
    });
    expect(result.success).toBe(true);
  });

  it("отклоняет запрос без isPublic", () => {
    const { isPublic: _, ...withoutIsPublic } = validRequest;
    const result = RecipeRequestSchema.safeParse(withoutIsPublic);
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

// ── averageRating / myRating (10.3) ───────────────────────────────────────────

describe("RecipeShortDtoSchema — поля рейтинга", () => {
  it("принимает объект с averageRating и myRating", () => {
    const result = RecipeShortDtoSchema.safeParse({
      ...validShortDto,
      averageRating: 4.2,
      myRating: 4,
    });
    expect(result.success).toBe(true);
  });

  it("принимает объект с averageRating = null и myRating = null", () => {
    const result = RecipeShortDtoSchema.safeParse({
      ...validShortDto,
      averageRating: null,
      myRating: null,
    });
    expect(result.success).toBe(true);
  });

  it("принимает объект без полей averageRating и myRating", () => {
    const result = RecipeShortDtoSchema.safeParse(validShortDto);
    expect(result.success).toBe(true);
  });
});

describe("RecipeDtoSchema — поля рейтинга", () => {
  it("принимает объект с averageRating и myRating", () => {
    const result = RecipeDtoSchema.safeParse({
      ...validDto,
      averageRating: 3.7,
      myRating: 3,
    });
    expect(result.success).toBe(true);
  });

  it("принимает объект с averageRating = null и myRating = null", () => {
    const result = RecipeDtoSchema.safeParse({
      ...validDto,
      averageRating: null,
      myRating: null,
    });
    expect(result.success).toBe(true);
  });

  it("принимает объект без полей averageRating и myRating", () => {
    const result = RecipeDtoSchema.safeParse(validDto);
    expect(result.success).toBe(true);
  });
});
