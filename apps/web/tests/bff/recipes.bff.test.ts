import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { getRecipes } from "@/lib/bff/recipes";
import { RecipePagedResultSchema } from "@/lib/schemas/recipe";

const mockRecipeShort = {
  id: "11111111-0000-0000-0000-000000000001",
  title: "Борщ",
  description: "Классический борщ",
  cookingTime: 120,
  difficulty: "everyday",
  photoId: null,
  categoryIds: [],
  isPublic: true,
  authorName: "Анна Воронова",
};

const mockPagedResult = {
  items: [mockRecipeShort],
  total: 42,
  page: 2,
  pageSize: 18,
};

beforeEach(() => {
  vi.stubGlobal("fetch", vi.fn());
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("getRecipes с пагинацией", () => {
  it("передаёт page и pageSize в URL", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockPagedResult), { status: 200 }),
    );

    await getRecipes(2, 18);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("page=2"),
      expect.any(Object),
    );
    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("pageSize=18"),
      expect.any(Object),
    );
  });

  it("возвращает PagedResult с items, total, page, pageSize", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockPagedResult), { status: 200 }),
    );

    const result = await getRecipes(2, 18);

    expect(result.items).toHaveLength(1);
    expect(result.items[0]?.title).toBe("Борщ");
    expect(result.total).toBe(42);
    expect(result.page).toBe(2);
    expect(result.pageSize).toBe(18);
  });

  it("использует значения по умолчанию page=1, pageSize=18", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(
        JSON.stringify({ ...mockPagedResult, page: 1 }),
        { status: 200 },
      ),
    );

    await getRecipes();

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("page=1"),
      expect.any(Object),
    );
    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("pageSize=18"),
      expect.any(Object),
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 500 }));

    await expect(getRecipes()).rejects.toThrow("500");
  });
});

describe("getRecipes с поиском и сортировкой", () => {
  it("передаёт q в URL", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockPagedResult), { status: 200 }),
    );

    await getRecipes(1, 18, "борщ");

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("q=%D0%B1%D0%BE%D1%80%D1%89"),
      expect.any(Object),
    );
  });

  it("передаёт sort в URL", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockPagedResult), { status: 200 }),
    );

    await getRecipes(1, 18, undefined, "title_asc");

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("sort=title_asc"),
      expect.any(Object),
    );
  });

  it("передаёт q и sort одновременно", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockPagedResult), { status: 200 }),
    );

    await getRecipes(1, 18, "суп", "title_desc");

    const calledUrl = vi.mocked(fetch).mock.calls[0]?.[0] as string;
    expect(calledUrl).toContain("sort=title_desc");
    expect(calledUrl).toContain("q=");
  });

  it("не добавляет q в URL если не передан", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockPagedResult), { status: 200 }),
    );

    await getRecipes(1, 18);

    expect(vi.mocked(fetch)).not.toHaveBeenCalledWith(
      expect.stringContaining("q="),
      expect.any(Object),
    );
  });

  it("не добавляет sort в URL если не передан", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockPagedResult), { status: 200 }),
    );

    await getRecipes(1, 18);

    expect(vi.mocked(fetch)).not.toHaveBeenCalledWith(
      expect.stringContaining("sort="),
      expect.any(Object),
    );
  });
});

describe("RecipePagedResultSchema", () => {
  it("парсит корректный PagedResult", () => {
    const result = RecipePagedResultSchema.parse(mockPagedResult);

    expect(result.total).toBe(42);
    expect(result.items).toHaveLength(1);
  });

  it("отклоняет отрицательный total", () => {
    expect(() =>
      RecipePagedResultSchema.parse({ ...mockPagedResult, total: -1 }),
    ).toThrow();
  });

  it("отклоняет page = 0", () => {
    expect(() =>
      RecipePagedResultSchema.parse({ ...mockPagedResult, page: 0 }),
    ).toThrow();
  });
});
