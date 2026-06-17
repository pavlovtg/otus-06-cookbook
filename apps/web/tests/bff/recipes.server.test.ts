import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("@/lib/server-fetch", () => ({
  serverFetch: vi.fn(),
}));

import { getRecipe, getRecipes } from "@/lib/bff/recipes.server";
import { serverFetch } from "@/lib/server-fetch";

const mockRecipeShort = {
  id: "11111111-0000-0000-0000-000000000001",
  title: "Борщ",
  description: "Классический борщ",
  cookingTime: 120,
  difficulty: "everyday",
  photoId: null,
  categoryIds: [],
  isPublic: true,
  authorName: "Иван",
};

const mockRecipe = {
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
  authorName: "Иван",
};

const pagedResponse = (items: unknown[]) =>
  new Response(
    JSON.stringify({ items, total: items.length, page: 1, pageSize: 18 }),
    { status: 200 },
  );

beforeEach(() => {
  vi.mocked(serverFetch).mockReset();
});

afterEach(() => {
  vi.clearAllMocks();
});

describe("getRecipes (server)", () => {
  it("вызывает serverFetch с абсолютным SERVER_BASE и cache=no-store", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(pagedResponse([mockRecipeShort]));

    await getRecipes();

    const [url, init] = vi.mocked(serverFetch).mock.calls[0]!;
    expect(url).toMatch(/^http:\/\//);
    expect(url).toContain("/api/cookbook/v1/recipes");
    expect(init).toEqual({ cache: "no-store" });
  });

  it("прокидывает page/pageSize/q/sort в query", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(pagedResponse([]));

    await getRecipes(2, 24, "борщ", "title_asc");

    const url = decodeURIComponent(
      vi.mocked(serverFetch).mock.calls[0]?.[0] as string,
    );
    expect(url).toContain("page=2");
    expect(url).toContain("pageSize=24");
    expect(url).toContain("q=борщ");
    expect(url).toContain("sort=title_asc");
  });

  it("не добавляет q/sort если не переданы", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(pagedResponse([]));

    await getRecipes();

    const url = vi.mocked(serverFetch).mock.calls[0]?.[0] as string;
    expect(url).not.toContain("q=");
    expect(url).not.toContain("sort=");
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 500 }),
    );

    await expect(getRecipes()).rejects.toThrow("500");
  });
});

describe("getRecipe (server)", () => {
  it("вызывает serverFetch по id", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockRecipe), { status: 200 }),
    );

    const result = await getRecipe(mockRecipe.id);

    const url = vi.mocked(serverFetch).mock.calls[0]?.[0] as string;
    expect(url).toContain(`/recipes/${mockRecipe.id}`);
    expect(result.title).toBe("Борщ");
  });

  it("выбрасывает ошибку при 404", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 404 }),
    );

    await expect(getRecipe("missing")).rejects.toThrow("404");
  });
});
