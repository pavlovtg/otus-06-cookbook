import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { getDashboardStats } from "@/lib/bff/dashboard.server";

const mockGuestResponse = {
  totalRecipes: 42,
  top10ByRating: [
    { id: "11111111-0000-0000-0000-000000000001", title: "Борщ", averageRating: 4.8 },
  ],
  topFavoritesByRating: [],
  byMainIngredient: [{ categoryName: "Мясо", recipeCount: 10 }],
  byCuisine: [{ categoryName: "Русская", recipeCount: 15 }],
};

beforeEach(() => {
  vi.stubGlobal("fetch", vi.fn());
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("getDashboardStats", () => {
  it("возвращает корректно распарсенный DashboardStatsDto", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockGuestResponse), { status: 200 }),
    );

    const result = await getDashboardStats();

    expect(result.totalRecipes).toBe(42);
    expect(result.top10ByRating).toHaveLength(1);
    expect(result.top10ByRating[0]?.title).toBe("Борщ");
    expect(result.byMainIngredient[0]?.categoryName).toBe("Мясо");
  });

  it("бросает ошибку при статусе не 200", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify({ error: "Unauthorized" }), { status: 401 }),
    );

    await expect(getDashboardStats()).rejects.toThrow(
      "Failed to fetch dashboard stats: 401",
    );
  });

  it("бросает ошибку при невалидном JSON-ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify({ totalRecipes: -1, top10ByRating: [] }), {
        status: 200,
      }),
    );

    await expect(getDashboardStats()).rejects.toThrow();
  });

  it("корректно парсит ответ с myRecipes и planFill", async () => {
    const authResponse = {
      ...mockGuestResponse,
      myRecipes: 5,
      myComments: 12,
      planFill: { "0_breakfast": true },
    };

    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(authResponse), { status: 200 }),
    );

    const result = await getDashboardStats();

    expect(result.myRecipes).toBe(5);
    expect(result.myComments).toBe(12);
    expect(result.planFill).toEqual({ "0_breakfast": true });
  });
});
