import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import {
  createRecipe,
  deleteRecipe,
  getRecipe,
  getRecipes,
  updateRecipe,
} from "@/lib/bff/recipes";

const mockRecipe = {
  id: "11111111-0000-0000-0000-000000000001",
  title: "Борщ",
  description: "Классический борщ",
  cookingTime: 120,
  difficulty: "everyday",
  servings: 6,
  instructions: "1. Сварить бульон.",
};

const mockRequest = {
  title: "Борщ",
  description: "Классический борщ",
  cookingTime: 120,
  difficulty: "everyday" as const,
  servings: 6,
  instructions: "1. Сварить бульон.",
};

beforeEach(() => {
  vi.stubGlobal("fetch", vi.fn());
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("getRecipes", () => {
  it("возвращает список рецептов", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify([mockRecipe]), { status: 200 })
    );

    const result = await getRecipes();

    expect(result).toHaveLength(1);
    expect(result[0]?.title).toBe("Борщ");
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 500 }));

    await expect(getRecipes()).rejects.toThrow("500");
  });
});

describe("getRecipe", () => {
  it("возвращает рецепт по id", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockRecipe), { status: 200 })
    );

    const result = await getRecipe(mockRecipe.id);

    expect(result.id).toBe(mockRecipe.id);
    expect(result.title).toBe("Борщ");
  });

  it("выбрасывает ошибку при 404", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 404 }));

    await expect(getRecipe("non-existent")).rejects.toThrow("404");
  });
});

describe("createRecipe", () => {
  it("отправляет POST и возвращает созданный рецепт", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockRecipe), { status: 201 })
    );

    const result = await createRecipe(mockRequest);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("/recipes"),
      expect.objectContaining({ method: "POST" })
    );
    expect(result.title).toBe("Борщ");
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(createRecipe(mockRequest)).rejects.toThrow("400");
  });
});

describe("updateRecipe", () => {
  it("отправляет PUT-запрос", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 204 }));

    await updateRecipe(mockRecipe.id, mockRequest);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(mockRecipe.id),
      expect.objectContaining({ method: "PUT" })
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(updateRecipe(mockRecipe.id, mockRequest)).rejects.toThrow("400");
  });
});

describe("deleteRecipe", () => {
  it("отправляет DELETE-запрос", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 204 }));

    await deleteRecipe(mockRecipe.id);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(mockRecipe.id),
      expect.objectContaining({ method: "DELETE" })
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 404 }));

    await expect(deleteRecipe(mockRecipe.id)).rejects.toThrow("404");
  });
});
