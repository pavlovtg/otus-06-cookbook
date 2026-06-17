import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { addFavorite, removeFavorite } from "@/lib/bff/favorites";

const RECIPE_ID = "11111111-0000-0000-0000-000000000001";

beforeEach(() => {
  vi.stubGlobal("fetch", vi.fn());
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("addFavorite", () => {
  it("отправляет POST на /api/recipes/:id/favorites", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 200 }));

    await addFavorite(RECIPE_ID);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(`/api/recipes/${RECIPE_ID}/favorites`),
      expect.objectContaining({ method: "POST" }),
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 401 }));

    await expect(addFavorite(RECIPE_ID)).rejects.toThrow("401");
  });

  it("выбрасывает ошибку при 500", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 500 }));

    await expect(addFavorite(RECIPE_ID)).rejects.toThrow("500");
  });
});

describe("removeFavorite", () => {
  it("отправляет DELETE на /api/recipes/:id/favorites", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 204 }));

    await removeFavorite(RECIPE_ID);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(`/api/recipes/${RECIPE_ID}/favorites`),
      expect.objectContaining({ method: "DELETE" }),
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 401 }));

    await expect(removeFavorite(RECIPE_ID)).rejects.toThrow("401");
  });

  it("выбрасывает ошибку при 404", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 404 }));

    await expect(removeFavorite(RECIPE_ID)).rejects.toThrow("404");
  });
});
