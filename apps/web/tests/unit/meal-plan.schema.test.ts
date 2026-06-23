import { describe, it, expect } from "vitest";
import {
  MealPlanItemDtoSchema,
  MealPlanSlotDtoSchema,
  MealPlanDtoSchema,
  MealPlanItemRequestSchema,
  MealPlanRequestSchema,
} from "@/lib/schemas/meal-plan";

const validItem = {
  recipeId: "11111111-0000-0000-0000-000000000001",
  servings: 2,
};

const validSlot = {
  weekDay: 1,
  mealType: 1,
  items: [validItem],
};

const validDto = {
  slots: [validSlot],
};

// ── MealPlanItemDtoSchema ─────────────────────────────────────────────────────

describe("MealPlanItemDtoSchema", () => {
  it("парсит корректный объект", () => {
    expect(MealPlanItemDtoSchema.safeParse(validItem).success).toBe(true);
  });

  it("принимает servings = 1 (минимум)", () => {
    expect(
      MealPlanItemDtoSchema.safeParse({ ...validItem, servings: 1 }).success,
    ).toBe(true);
  });

  it("принимает servings = 99 (максимум)", () => {
    expect(
      MealPlanItemDtoSchema.safeParse({ ...validItem, servings: 99 }).success,
    ).toBe(true);
  });

  it("отклоняет servings = 0", () => {
    expect(
      MealPlanItemDtoSchema.safeParse({ ...validItem, servings: 0 }).success,
    ).toBe(false);
  });

  it("отклоняет servings = 100", () => {
    expect(
      MealPlanItemDtoSchema.safeParse({ ...validItem, servings: 100 }).success,
    ).toBe(false);
  });

  it("отклоняет некорректный recipeId (не uuid)", () => {
    expect(
      MealPlanItemDtoSchema.safeParse({ ...validItem, recipeId: "not-uuid" })
        .success,
    ).toBe(false);
  });

  it("отклоняет дробный servings", () => {
    expect(
      MealPlanItemDtoSchema.safeParse({ ...validItem, servings: 1.5 }).success,
    ).toBe(false);
  });
});

// ── MealPlanSlotDtoSchema ─────────────────────────────────────────────────────

describe("MealPlanSlotDtoSchema", () => {
  it("парсит корректный слот", () => {
    expect(MealPlanSlotDtoSchema.safeParse(validSlot).success).toBe(true);
  });

  it("принимает weekDay = 1 (минимум)", () => {
    expect(
      MealPlanSlotDtoSchema.safeParse({ ...validSlot, weekDay: 1 }).success,
    ).toBe(true);
  });

  it("принимает weekDay = 7 (максимум)", () => {
    expect(
      MealPlanSlotDtoSchema.safeParse({ ...validSlot, weekDay: 7 }).success,
    ).toBe(true);
  });

  it("отклоняет weekDay = 0", () => {
    expect(
      MealPlanSlotDtoSchema.safeParse({ ...validSlot, weekDay: 0 }).success,
    ).toBe(false);
  });

  it("отклоняет weekDay = 8", () => {
    expect(
      MealPlanSlotDtoSchema.safeParse({ ...validSlot, weekDay: 8 }).success,
    ).toBe(false);
  });

  it("принимает mealType = 1 (минимум)", () => {
    expect(
      MealPlanSlotDtoSchema.safeParse({ ...validSlot, mealType: 1 }).success,
    ).toBe(true);
  });

  it("принимает mealType = 3 (максимум)", () => {
    expect(
      MealPlanSlotDtoSchema.safeParse({ ...validSlot, mealType: 3 }).success,
    ).toBe(true);
  });

  it("отклоняет mealType = 0", () => {
    expect(
      MealPlanSlotDtoSchema.safeParse({ ...validSlot, mealType: 0 }).success,
    ).toBe(false);
  });

  it("отклоняет mealType = 4", () => {
    expect(
      MealPlanSlotDtoSchema.safeParse({ ...validSlot, mealType: 4 }).success,
    ).toBe(false);
  });

  it("принимает пустой массив items", () => {
    expect(
      MealPlanSlotDtoSchema.safeParse({ ...validSlot, items: [] }).success,
    ).toBe(true);
  });
});

// ── MealPlanDtoSchema ─────────────────────────────────────────────────────────

describe("MealPlanDtoSchema", () => {
  it("парсит корректный DTO", () => {
    expect(MealPlanDtoSchema.safeParse(validDto).success).toBe(true);
  });

  it("принимает пустой массив slots", () => {
    expect(MealPlanDtoSchema.safeParse({ slots: [] }).success).toBe(true);
  });

  it("принимает опциональный id", () => {
    const result = MealPlanDtoSchema.safeParse({
      ...validDto,
      id: "22222222-0000-0000-0000-000000000002",
    });
    expect(result.success).toBe(true);
  });

  it("отклоняет некорректный id", () => {
    expect(
      MealPlanDtoSchema.safeParse({ ...validDto, id: "bad-id" }).success,
    ).toBe(false);
  });
});

// ── MealPlanRequestSchema ─────────────────────────────────────────────────────

describe("MealPlanRequestSchema", () => {
  it("парсит корректный запрос", () => {
    const req = {
      slots: [
        {
          weekDay: 2,
          mealType: 2,
          items: [{ recipeId: "33333333-0000-0000-0000-000000000003", servings: 4 }],
        },
      ],
    };
    expect(MealPlanRequestSchema.safeParse(req).success).toBe(true);
  });

  it("принимает пустой массив slots", () => {
    expect(MealPlanRequestSchema.safeParse({ slots: [] }).success).toBe(true);
  });

  it("отклоняет запрос без поля slots", () => {
    expect(MealPlanRequestSchema.safeParse({}).success).toBe(false);
  });

  it("отклоняет item с некорректным recipeId", () => {
    const req = {
      slots: [
        {
          weekDay: 1,
          mealType: 1,
          items: [{ recipeId: "not-uuid", servings: 2 }],
        },
      ],
    };
    expect(MealPlanItemRequestSchema.safeParse(req.slots[0]!.items[0]).success).toBe(false);
  });
});
