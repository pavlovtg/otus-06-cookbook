import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { setRating, deleteRating } from "@/lib/bff/ratings";

const RECIPE_ID = "11111111-0000-0000-0000-000000000001";

beforeEach(() => {
  vi.stubGlobal("fetch", vi.fn());
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("setRating", () => {
  it("отправляет PUT на /api/cookbook/v1/recipes/:id/rating", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 200 }));

    await setRating(RECIPE_ID, 4);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(`/api/cookbook/v1/recipes/${RECIPE_ID}/rating`),
      expect.objectContaining({ method: "PUT" }),
    );
  });

  it("передаёт value в теле запроса", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 200 }));

    await setRating(RECIPE_ID, 3);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.any(String),
      expect.objectContaining({ body: JSON.stringify({ value: 3 }) }),
    );
  });

  it("выбрасывает ошибку при 400", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(setRating(RECIPE_ID, 0)).rejects.toThrow("400");
  });

  it("выбрасывает ошибку при 401", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 401 }));

    await expect(setRating(RECIPE_ID, 3)).rejects.toThrow("401");
  });
});

describe("deleteRating", () => {
  it("отправляет DELETE на /api/cookbook/v1/recipes/:id/rating", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 204 }));

    await deleteRating(RECIPE_ID);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(`/api/cookbook/v1/recipes/${RECIPE_ID}/rating`),
      expect.objectContaining({ method: "DELETE" }),
    );
  });

  it("выбрасывает ошибку при 400", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(deleteRating(RECIPE_ID)).rejects.toThrow("400");
  });

  it("выбрасывает ошибку при 401", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 401 }));

    await expect(deleteRating(RECIPE_ID)).rejects.toThrow("401");
  });
});
