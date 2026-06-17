import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("@/lib/server-fetch", () => ({
  serverFetch: vi.fn(),
}));

import { getIngredient, getIngredients } from "@/lib/bff/ingredients.server";
import { serverFetch } from "@/lib/server-fetch";

const mockIngredient = {
  id: "aad1f839-df31-49f2-84d7-4dd01e04be77",
  title: "Морковь",
  unit: "г",
  defaultAmount: 100,
  category: "vegetables",
  isSystem: false,
};

const pagedResponse = (items: unknown[]) =>
  new Response(
    JSON.stringify({ items, total: items.length, page: 1, pageSize: 20 }),
    { status: 200 },
  );

beforeEach(() => {
  vi.mocked(serverFetch).mockReset();
});

afterEach(() => {
  vi.clearAllMocks();
});

describe("getIngredients (server)", () => {
  it("вызывает serverFetch с абсолютным SERVER_BASE", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(pagedResponse([mockIngredient]));

    await getIngredients();

    const [url, init] = vi.mocked(serverFetch).mock.calls[0]!;
    expect(url).toMatch(/^http:\/\//);
    expect(url).toContain("/api/cookbook/v1/ingredients");
    expect(init).toEqual({ cache: "no-store" });
  });

  it("прокидывает title/category/page/pageSize в query", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(pagedResponse([]));

    await getIngredients({
      title: "морк",
      category: "vegetables",
      page: 2,
      pageSize: 50,
    });

    const url = decodeURIComponent(
      vi.mocked(serverFetch).mock.calls[0]?.[0] as string,
    );
    expect(url).toContain("title=морк");
    expect(url).toContain("category=vegetables");
    expect(url).toContain("page=2");
    expect(url).toContain("pageSize=50");
  });

  it("возвращает распарсенный PagedIngredient", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(pagedResponse([mockIngredient]));

    const result = await getIngredients();

    expect(result.items).toHaveLength(1);
    expect(result.items[0]?.title).toBe("Морковь");
    expect(result.total).toBe(1);
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 500 }),
    );

    await expect(getIngredients()).rejects.toThrow("500");
  });
});

describe("getIngredient (server)", () => {
  it("вызывает serverFetch по id", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockIngredient), { status: 200 }),
    );

    const result = await getIngredient(mockIngredient.id);

    const url = vi.mocked(serverFetch).mock.calls[0]?.[0] as string;
    expect(url).toContain(`/ingredients/${mockIngredient.id}`);
    expect(result.title).toBe("Морковь");
  });

  it("выбрасывает ошибку при 404", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 404 }),
    );

    await expect(getIngredient("missing")).rejects.toThrow("404");
  });
});
