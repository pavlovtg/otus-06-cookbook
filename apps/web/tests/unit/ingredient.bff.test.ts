import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import {
  createIngredient,
  deleteIngredient,
  getIngredient,
  getIngredients,
  updateIngredient,
} from "@/lib/bff/ingredients";

const mockIngredient = {
  id: "aad1f839-df31-49f2-84d7-4dd01e04be77",
  title: "Морковь",
  unit: "г",
  defaultAmount: 100,
  category: "vegetables",
  isSystem: false,
};

const mockRequest = {
  title: "Морковь",
  unit: "г",
  defaultAmount: 100,
  category: "vegetables" as const,
};

beforeEach(() => {
  vi.stubGlobal("fetch", vi.fn());
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("getIngredients", () => {
  it("возвращает список ингредиентов", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(
        JSON.stringify({ items: [mockIngredient], total: 1, page: 1, pageSize: 20 }),
        { status: 200 }
      )
    );

    const result = await getIngredients();

    expect(result.items).toHaveLength(1);
    expect(result.items[0]?.title).toBe("Морковь");
  });

  it("передаёт фильтр по названию в query-параметр", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(
        JSON.stringify({ items: [], total: 0, page: 1, pageSize: 20 }),
        { status: 200 }
      )
    );

    await getIngredients({ title: "морк" });

    const url = decodeURIComponent(vi.mocked(fetch).mock.calls[0]?.[0] as string);
    expect(url).toContain("title=морк");
  });

  it("передаёт фильтр по категории в query-параметр", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(
        JSON.stringify({ items: [], total: 0, page: 1, pageSize: 20 }),
        { status: 200 }
      )
    );

    await getIngredients({ category: "vegetables" });

    const url = vi.mocked(fetch).mock.calls[0]?.[0] as string;
    expect(url).toContain("category=vegetables");
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 500 }));

    await expect(getIngredients()).rejects.toThrow("500");
  });
});

describe("getIngredient", () => {
  it("возвращает ингредиент по id", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockIngredient), { status: 200 })
    );

    const result = await getIngredient(mockIngredient.id);

    expect(result.id).toBe(mockIngredient.id);
    expect(result.title).toBe("Морковь");
  });

  it("выбрасывает ошибку при 404", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 404 }));

    await expect(getIngredient("non-existent")).rejects.toThrow("404");
  });
});

describe("createIngredient", () => {
  it("отправляет POST и возвращает созданный ингредиент", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockIngredient), { status: 201 })
    );

    const result = await createIngredient(mockRequest);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("/ingredients"),
      expect.objectContaining({ method: "POST" })
    );
    expect(result.title).toBe("Морковь");
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(createIngredient(mockRequest)).rejects.toThrow("400");
  });
});

describe("updateIngredient", () => {
  it("отправляет PUT-запрос", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 204 }));

    await updateIngredient(mockIngredient.id, mockRequest);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(mockIngredient.id),
      expect.objectContaining({ method: "PUT" })
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(updateIngredient(mockIngredient.id, mockRequest)).rejects.toThrow("400");
  });
});

describe("deleteIngredient", () => {
  it("отправляет DELETE-запрос", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 204 }));

    await deleteIngredient(mockIngredient.id);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(mockIngredient.id),
      expect.objectContaining({ method: "DELETE" })
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 404 }));

    await expect(deleteIngredient(mockIngredient.id)).rejects.toThrow("404");
  });
});
