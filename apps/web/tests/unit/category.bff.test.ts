import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import {
  getCategories,
  createCategory,
  updateCategory,
  deleteCategory,
} from "@/lib/bff/categories";

const mockCategory = {
  id: "550e8400-e29b-41d4-a716-446655440000",
  name: "Итальянская",
  description: "Блюда итальянской кухни",
  type: "cuisine",
};

const mockRequest = {
  name: "Итальянская",
  description: "Блюда итальянской кухни",
  type: "cuisine" as const,
};

beforeEach(() => {
  vi.stubGlobal("fetch", vi.fn());
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("getCategories", () => {
  it("возвращает список категорий", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify([mockCategory]), { status: 200 }),
    );

    const result = await getCategories();

    expect(result).toHaveLength(1);
    expect(result[0]?.name).toBe("Итальянская");
  });

  it("возвращает пустой список", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify([]), { status: 200 }),
    );

    const result = await getCategories();
    expect(result).toHaveLength(0);
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 500 }));

    await expect(getCategories()).rejects.toThrow("500");
  });
});

describe("createCategory", () => {
  it("отправляет POST и возвращает созданную категорию", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockCategory), { status: 201 }),
    );

    const result = await createCategory(mockRequest);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("/categories"),
      expect.objectContaining({ method: "POST" }),
    );
    expect(result.name).toBe("Итальянская");
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(createCategory(mockRequest)).rejects.toThrow("400");
  });
});

describe("updateCategory", () => {
  it("отправляет PUT-запрос", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 204 }));

    await updateCategory(mockCategory.id, mockRequest);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(mockCategory.id),
      expect.objectContaining({ method: "PUT" }),
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(updateCategory(mockCategory.id, mockRequest)).rejects.toThrow("400");
  });
});

describe("deleteCategory", () => {
  it("отправляет DELETE-запрос", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 204 }));

    await deleteCategory(mockCategory.id);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(mockCategory.id),
      expect.objectContaining({ method: "DELETE" }),
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 404 }));

    await expect(deleteCategory(mockCategory.id)).rejects.toThrow("404");
  });

  it("выбрасывает ошибку 409 при использовании в рецептах", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 409 }));

    await expect(deleteCategory(mockCategory.id)).rejects.toThrow("409");
  });
});
