import { describe, it, expect } from "vitest";
import { RatingSummaryDtoSchema } from "@/lib/bff/ratings";

describe("RatingSummaryDtoSchema", () => {
  it("принимает корректные числовые значения", () => {
    const result = RatingSummaryDtoSchema.safeParse({
      averageRating: 4.5,
      myRating: 3,
    });
    expect(result.success).toBe(true);
  });

  it("принимает null-значения", () => {
    const result = RatingSummaryDtoSchema.safeParse({
      averageRating: null,
      myRating: null,
    });
    expect(result.success).toBe(true);
  });

  it("принимает averageRating = 1.0 и myRating = 1", () => {
    const result = RatingSummaryDtoSchema.safeParse({
      averageRating: 1.0,
      myRating: 1,
    });
    expect(result.success).toBe(true);
  });

  it("принимает averageRating = 5.0 и myRating = 5", () => {
    const result = RatingSummaryDtoSchema.safeParse({
      averageRating: 5.0,
      myRating: 5,
    });
    expect(result.success).toBe(true);
  });

  it("отклоняет myRating = 0", () => {
    const result = RatingSummaryDtoSchema.safeParse({
      averageRating: 3.0,
      myRating: 0,
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет myRating = 6", () => {
    const result = RatingSummaryDtoSchema.safeParse({
      averageRating: 3.0,
      myRating: 6,
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет дробный myRating", () => {
    const result = RatingSummaryDtoSchema.safeParse({
      averageRating: 3.0,
      myRating: 2.5,
    });
    expect(result.success).toBe(false);
  });

  it("отклоняет объект без поля averageRating", () => {
    const result = RatingSummaryDtoSchema.safeParse({ myRating: 3 });
    expect(result.success).toBe(false);
  });

  it("отклоняет объект без поля myRating", () => {
    const result = RatingSummaryDtoSchema.safeParse({ averageRating: 4.0 });
    expect(result.success).toBe(false);
  });
});
