import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

vi.mock("@/lib/server-fetch", () => ({
  serverFetch: vi.fn(),
}));

import { getShoppingList } from "@/lib/bff/shopping-list.server";
import { serverFetch } from "@/lib/server-fetch";

const mockGroups = [
  {
    category: "Овощи",
    items: [
      {
        ingredientId: "11111111-0000-0000-0000-000000000001",
        title: "Морковь",
        amount: 2,
        unit: "кг",
      },
    ],
  },
  {
    category: "Молочные продукты",
    items: [
      {
        ingredientId: "22222222-0000-0000-0000-000000000002",
        title: "Молоко",
        amount: 1,
        unit: "л",
      },
    ],
  },
];

beforeEach(() => {
  vi.mocked(serverFetch).mockReset();
});

afterEach(() => {
  vi.clearAllMocks();
});

// ── getShoppingList ───────────────────────────────────────────────────────────

describe("getShoppingList", () => {
  it("вызывает serverFetch с абсолютным URL и cache=no-store", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockGroups), { status: 200 }),
    );

    await getShoppingList();

    const [url, init] = vi.mocked(serverFetch).mock.calls[0]!;
    expect(url).toMatch(/^http:\/\//);
    expect(url).toContain("/api/cookbook/v1/shopping-list");
    expect(init).toEqual({ cache: "no-store" });
  });

  it("возвращает распарсенный массив групп", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockGroups), { status: 200 }),
    );

    const result = await getShoppingList();

    expect(result).toHaveLength(2);
    expect(result[0]!.category).toBe("Овощи");
    expect(result[0]!.items[0]!.title).toBe("Морковь");
    expect(result[0]!.items[0]!.amount).toBe(2);
    expect(result[1]!.category).toBe("Молочные продукты");
  });

  it("возвращает пустой массив при пустом ответе", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(JSON.stringify([]), { status: 200 }),
    );

    const result = await getShoppingList();
    expect(result).toHaveLength(0);
  });

  it("выбрасывает ошибку при 401", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 401 }),
    );

    await expect(getShoppingList()).rejects.toThrow("401");
  });

  it("выбрасывает ошибку при 500", async () => {
    vi.mocked(serverFetch).mockResolvedValueOnce(
      new Response(null, { status: 500 }),
    );

    await expect(getShoppingList()).rejects.toThrow("500");
  });
});
