import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("@/lib/server-fetch", () => ({
  serverFetch: vi.fn(),
}));

import { getCategories } from "@/lib/bff/categories.server";
import { serverFetch } from "@/lib/server-fetch";

const mockCategory = {
  id: "550e8400-e29b-41d4-a716-446655440000",
  name: "Итальянская",
  description: "Блюда итальянской кухни",
  type: "cuisine",
};

beforeEach(() => {
  vi.mocked(serverFetch).mockReset();
});

afterEach(() => {
  vi.clearAllMocks();
});

describe("getCategories (server)", () => {
  it("вызывает serverFetch с SERVER_BASE и cache=no-store", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(JSON.stringify([mockCategory]), { status: 200 }),
    );

    await getCategories();

    expect(serverFetch).toHaveBeenCalledTimes(1);
    const [url, init] = vi.mocked(serverFetch).mock.calls[0]!;
    expect(url).toContain("/api/cookbook/v1/categories");
    expect(url).toMatch(/^http:\/\//); // абсолютный URL (через gateway)
    expect(init).toEqual({ cache: "no-store" });
  });

  it("возвращает распарсенный список категорий", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(JSON.stringify([mockCategory]), { status: 200 }),
    );

    const result = await getCategories();

    expect(result).toHaveLength(1);
    expect(result[0]?.name).toBe("Итальянская");
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 500 }),
    );

    await expect(getCategories()).rejects.toThrow("500");
  });
});
