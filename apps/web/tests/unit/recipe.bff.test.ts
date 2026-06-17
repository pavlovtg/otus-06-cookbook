import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import {
  createRecipe,
  deleteRecipe,
  deleteRecipePhoto,
  updateRecipe,
  uploadRecipePhoto,
} from "@/lib/bff/recipes";
import { getRecipe, getRecipes } from "@/lib/bff/recipes.server";

const PHOTO_ID = "aaaaaaaa-0000-0000-0000-000000000001";

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

const mockRequest = {
  title: "Борщ",
  description: "Классический борщ",
  cookingTime: 120,
  difficulty: "everyday" as const,
  servings: 6,
  instructions: "1. Сварить бульон.",
  ingredients: [],
  categoryIds: [],
  isPublic: true,
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
      new Response(
        JSON.stringify({ items: [mockRecipeShort], total: 1, page: 1, pageSize: 18 }),
        { status: 200 }
      )
    );

    const result = await getRecipes();

    expect(result.items).toHaveLength(1);
    expect(result.items[0]?.title).toBe("Борщ");
    expect(result.total).toBe(1);
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

describe("uploadRecipePhoto", () => {
  it("отправляет POST multipart и вызывает PURGE", async () => {
    vi.mocked(fetch)
      .mockResolvedValueOnce(
        new Response(JSON.stringify({ photoId: PHOTO_ID }), { status: 200 })
      )
      .mockResolvedValue(new Response(null, { status: 200 }));

    const file = new File(["data"], "photo.jpg", { type: "image/jpeg" });
    await uploadRecipePhoto(mockRecipe.id, file);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(`/recipes/${mockRecipe.id}/photo`),
      expect.objectContaining({ method: "POST" })
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    const file = new File(["data"], "photo.jpg", { type: "image/jpeg" });
    await expect(uploadRecipePhoto(mockRecipe.id, file)).rejects.toThrow("400");
  });
});

describe("deleteRecipePhoto", () => {
  it("отправляет DELETE и вызывает PURGE", async () => {
    vi.mocked(fetch)
      .mockResolvedValueOnce(new Response(null, { status: 204 }))
      .mockResolvedValue(new Response(null, { status: 200 }));

    await deleteRecipePhoto(mockRecipe.id, PHOTO_ID);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(`/recipes/${mockRecipe.id}/photo`),
      expect.objectContaining({ method: "DELETE" })
    );
  });

  it("выбрасывает ошибку при неуспешном ответе", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(deleteRecipePhoto(mockRecipe.id, PHOTO_ID)).rejects.toThrow("400");
  });
});
