import { describe, it, expect } from "vitest";
import { DashboardStatsDtoSchema } from "@/lib/schemas/dashboard";

const guestResponse = {
  totalRecipes: 42,
  top10ByRating: [
    { id: "11111111-0000-0000-0000-000000000001", title: "Борщ", averageRating: 4.8 },
  ],
  topFavoritesByRating: [],
  byMainIngredient: [{ categoryName: "Мясо", recipeCount: 10 }],
  byCuisine: [{ categoryName: "Русская", recipeCount: 15 }],
};

const authResponse = {
  ...guestResponse,
  myRecipes: 5,
  myComments: 12,
  topFavoritesByRating: [
    { id: "22222222-0000-0000-0000-000000000002", title: "Пельмени", averageRating: 4.5 },
  ],
  planFill: {
    "0_breakfast": true,
    "0_lunch": false,
    "1_dinner": true,
  },
};

describe("DashboardStatsDtoSchema — гость", () => {
  it("парсит корректный ответ для гостя", () => {
    const result = DashboardStatsDtoSchema.safeParse(guestResponse);
    expect(result.success).toBe(true);
  });

  it("totalRecipes — неотрицательное целое", () => {
    const result = DashboardStatsDtoSchema.safeParse({
      ...guestResponse,
      totalRecipes: -1,
    });
    expect(result.success).toBe(false);
  });

  it("top10ByRating — массив с корректными uuid", () => {
    const result = DashboardStatsDtoSchema.safeParse({
      ...guestResponse,
      top10ByRating: [{ id: "not-uuid", title: "X", averageRating: 4.0 }],
    });
    expect(result.success).toBe(false);
  });

  it("byMainIngredient — recipeCount неотрицательный", () => {
    const result = DashboardStatsDtoSchema.safeParse({
      ...guestResponse,
      byMainIngredient: [{ categoryName: "Мясо", recipeCount: -1 }],
    });
    expect(result.success).toBe(false);
  });

  it("myRecipes и myComments отсутствуют у гостя — допустимо", () => {
    const result = DashboardStatsDtoSchema.safeParse(guestResponse);
    if (result.success) {
      expect(result.data.myRecipes).toBeUndefined();
      expect(result.data.myComments).toBeUndefined();
    }
    expect(result.success).toBe(true);
  });
});

describe("DashboardStatsDtoSchema — авторизованный пользователь", () => {
  it("парсит корректный ответ для авторизованного пользователя", () => {
    const result = DashboardStatsDtoSchema.safeParse(authResponse);
    expect(result.success).toBe(true);
  });

  it("myRecipes присутствует и корректен", () => {
    const result = DashboardStatsDtoSchema.safeParse(authResponse);
    if (result.success) {
      expect(result.data.myRecipes).toBe(5);
    }
    expect(result.success).toBe(true);
  });

  it("myComments присутствует и корректен", () => {
    const result = DashboardStatsDtoSchema.safeParse(authResponse);
    if (result.success) {
      expect(result.data.myComments).toBe(12);
    }
    expect(result.success).toBe(true);
  });

  it("planFill — словарь string → boolean", () => {
    const result = DashboardStatsDtoSchema.safeParse(authResponse);
    if (result.success) {
      expect(result.data.planFill).toEqual({
        "0_breakfast": true,
        "0_lunch": false,
        "1_dinner": true,
      });
    }
    expect(result.success).toBe(true);
  });

  it("planFill с некорректным значением — отклоняется", () => {
    const result = DashboardStatsDtoSchema.safeParse({
      ...authResponse,
      planFill: { "0_breakfast": "yes" },
    });
    expect(result.success).toBe(false);
  });

  it("topFavoritesByRating — массив RecipeRankDto", () => {
    const result = DashboardStatsDtoSchema.safeParse(authResponse);
    if (result.success) {
      expect(result.data.topFavoritesByRating).toHaveLength(1);
      expect(result.data.topFavoritesByRating[0]?.title).toBe("Пельмени");
    }
    expect(result.success).toBe(true);
  });
});
