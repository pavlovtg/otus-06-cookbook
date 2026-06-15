import { describe, it, expect } from "vitest";
import { getRecipePhotoUrl, getRecipeThumbnailUrl } from "@/lib/bff/photos";
import { RecipeDtoSchema, RecipeShortDtoSchema } from "@/lib/schemas/recipe";

const PHOTO_ID = "aaaaaaaa-0000-0000-0000-000000000001";

describe("getRecipePhotoUrl", () => {
  it("формирует корректный URL оригинала", () => {
    expect(getRecipePhotoUrl(PHOTO_ID)).toBe(
      `/api/cookbook/v1/photos/${PHOTO_ID}`,
    );
  });
});

describe("getRecipeThumbnailUrl", () => {
  it("формирует корректный URL thumbnail", () => {
    expect(getRecipeThumbnailUrl(PHOTO_ID)).toBe(
      `/api/cookbook/v1/photos/${PHOTO_ID}/thumbnail`,
    );
  });
});

describe("RecipeDtoSchema — photoId", () => {
  const base = {
    id: "11111111-0000-0000-0000-000000000001",
    title: "Борщ",
    description: "Классический борщ",
    cookingTime: 120,
    difficulty: "everyday",
    servings: 6,
    instructions: "1. Сварить бульон.",
    ingredients: [],
    categoryIds: [],
  };

  it("принимает photoId: null", () => {
    const result = RecipeDtoSchema.safeParse({ ...base, photoId: null });
    expect(result.success).toBe(true);
  });

  it("принимает photoId: uuid", () => {
    const result = RecipeDtoSchema.safeParse({ ...base, photoId: PHOTO_ID });
    expect(result.success).toBe(true);
    if (result.success) {
      expect(result.data.photoId).toBe(PHOTO_ID);
    }
  });

  it("отклоняет photoId: не-uuid строку", () => {
    const result = RecipeDtoSchema.safeParse({ ...base, photoId: "not-a-uuid" });
    expect(result.success).toBe(false);
  });

  it("отклоняет отсутствующий photoId", () => {
    const result = RecipeDtoSchema.safeParse(base);
    expect(result.success).toBe(false);
  });
});

describe("RecipeShortDtoSchema — photoId", () => {
  const base = {
    id: "11111111-0000-0000-0000-000000000001",
    title: "Борщ",
    description: "Классический борщ",
    cookingTime: 120,
    difficulty: "everyday",
    categoryIds: [],
  };

  it("принимает photoId: null", () => {
    const result = RecipeShortDtoSchema.safeParse({ ...base, photoId: null });
    expect(result.success).toBe(true);
  });

  it("принимает photoId: uuid", () => {
    const result = RecipeShortDtoSchema.safeParse({ ...base, photoId: PHOTO_ID });
    expect(result.success).toBe(true);
    if (result.success) {
      expect(result.data.photoId).toBe(PHOTO_ID);
    }
  });

  it("отклоняет photoId: не-uuid строку", () => {
    const result = RecipeShortDtoSchema.safeParse({ ...base, photoId: "bad" });
    expect(result.success).toBe(false);
  });
});
