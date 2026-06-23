import { describe, it, expect } from "vitest";
import {
  ShoppingListItemSchema,
  ShoppingListGroupSchema,
} from "@/lib/schemas/shopping-list";

const validItem = {
  ingredientId: "11111111-0000-0000-0000-000000000001",
  title: "Морковь",
  amount: 2.5,
  unit: "кг",
};

const validGroup = {
  category: "Овощи",
  items: [validItem],
};

// ── ShoppingListItemSchema ────────────────────────────────────────────────────

describe("ShoppingListItemSchema", () => {
  it("парсит корректный объект", () => {
    expect(ShoppingListItemSchema.safeParse(validItem).success).toBe(true);
  });

  it("отклоняет некорректный ingredientId (не uuid)", () => {
    expect(
      ShoppingListItemSchema.safeParse({ ...validItem, ingredientId: "not-uuid" }).success,
    ).toBe(false);
  });

  it("отклоняет пустой title", () => {
    expect(
      ShoppingListItemSchema.safeParse({ ...validItem, title: "" }).success,
    ).toBe(false);
  });

  it("принимает дробный amount", () => {
    expect(
      ShoppingListItemSchema.safeParse({ ...validItem, amount: 0.5 }).success,
    ).toBe(true);
  });

  it("отклоняет отсутствие поля unit", () => {
    const { unit: _u, ...rest } = validItem;
    expect(ShoppingListItemSchema.safeParse(rest).success).toBe(false);
  });
});

// ── ShoppingListGroupSchema ───────────────────────────────────────────────────

describe("ShoppingListGroupSchema", () => {
  it("парсит корректный объект", () => {
    expect(ShoppingListGroupSchema.safeParse(validGroup).success).toBe(true);
  });

  it("принимает пустой массив items", () => {
    expect(
      ShoppingListGroupSchema.safeParse({ ...validGroup, items: [] }).success,
    ).toBe(true);
  });

  it("отклоняет пустую category", () => {
    expect(
      ShoppingListGroupSchema.safeParse({ ...validGroup, category: "" }).success,
    ).toBe(false);
  });

  it("отклоняет отсутствие поля category", () => {
    const { category: _c, ...rest } = validGroup;
    expect(ShoppingListGroupSchema.safeParse(rest).success).toBe(false);
  });

  it("отклоняет невалидный item внутри items", () => {
    expect(
      ShoppingListGroupSchema.safeParse({
        ...validGroup,
        items: [{ ...validItem, ingredientId: "bad" }],
      }).success,
    ).toBe(false);
  });
});
