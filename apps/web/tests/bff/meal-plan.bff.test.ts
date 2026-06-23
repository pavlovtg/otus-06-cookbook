import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("@/lib/server-fetch", () => ({
  serverFetch: vi.fn(),
}));

import { getMealPlan } from "@/lib/bff/meal-plan.server";
import { updateMealPlan, clearMealPlan } from "@/lib/bff/meal-plan";
import { serverFetch } from "@/lib/server-fetch";

const mockDto = {
  id: "11111111-0000-0000-0000-000000000001",
  slots: [
    {
      weekDay: 1,
      mealType: 1,
      items: [
        {
          recipeId: "22222222-0000-0000-0000-000000000002",
          servings: 2,
        },
      ],
    },
  ],
};

const mockRequest = {
  slots: [
    {
      weekDay: 1,
      mealType: 1,
      items: [
        {
          recipeId: "22222222-0000-0000-0000-000000000002",
          servings: 2,
        },
      ],
    },
  ],
};

beforeEach(() => {
  vi.mocked(serverFetch).mockReset();
});

afterEach(() => {
  vi.clearAllMocks();
});

// ── getMealPlan ───────────────────────────────────────────────────────────────

describe("getMealPlan", () => {
  it("вызывает serverFetch с абсолютным URL и cache=no-store", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockDto), { status: 200 }),
    );

    await getMealPlan();

    const [url, init] = vi.mocked(serverFetch).mock.calls[0]!;
    expect(url).toMatch(/^http:\/\//);
    expect(url).toContain("/api/cookbook/v1/meal-plan");
    expect(init).toEqual({ cache: "no-store" });
  });

  it("возвращает распарсенный MealPlanDto", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockDto), { status: 200 }),
    );

    const result = await getMealPlan();

    expect(result.slots).toHaveLength(1);
    expect(result.slots[0]!.weekDay).toBe(1);
    expect(result.slots[0]!.items[0]!.servings).toBe(2);
  });

  it("выбрасывает ошибку при 401", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 401 }),
    );

    await expect(getMealPlan()).rejects.toThrow("401");
  });

  it("выбрасывает ошибку при 500", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 500 }),
    );

    await expect(getMealPlan()).rejects.toThrow("500");
  });
});

// ── updateMealPlan ────────────────────────────────────────────────────────────

describe("updateMealPlan", () => {
  it("вызывает serverFetch с методом PUT", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 200 }),
    );

    await updateMealPlan(mockRequest);

    const [url, init] = vi.mocked(serverFetch).mock.calls[0]!;
    expect(url).toContain("/api/cookbook/v1/meal-plan");
    expect(init?.method).toBe("PUT");
  });

  it("передаёт тело запроса как JSON", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 200 }),
    );

    await updateMealPlan(mockRequest);

    const init = vi.mocked(serverFetch).mock.calls[0]![1];
    expect(init?.headers).toMatchObject({ "Content-Type": "application/json" });
    const body = JSON.parse(init?.body as string) as unknown;
    expect(body).toMatchObject(mockRequest);
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 400 }),
    );

    await expect(updateMealPlan(mockRequest)).rejects.toThrow("400");
  });
});

// ── clearMealPlan ─────────────────────────────────────────────────────────────

describe("clearMealPlan", () => {
  it("вызывает serverFetch с методом DELETE", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 204 }),
    );

    await clearMealPlan();

    const [url, init] = vi.mocked(serverFetch).mock.calls[0]!;
    expect(url).toContain("/api/cookbook/v1/meal-plan");
    expect(init?.method).toBe("DELETE");
  });

  it("не выбрасывает ошибку при 204", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 204 }),
    );

    await expect(clearMealPlan()).resolves.toBeUndefined();
  });

  it("выбрасывает ошибку при 401", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 401 }),
    );

    await expect(clearMealPlan()).rejects.toThrow("401");
  });
});
