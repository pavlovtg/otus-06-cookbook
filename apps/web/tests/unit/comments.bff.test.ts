import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { getComments, addComment, deleteComment } from "@/lib/bff/comments";

const RECIPE_ID = "11111111-0000-0000-0000-000000000001";
const COMMENT_ID = "aaaaaaaa-0000-0000-0000-000000000001";

const mockComment = {
  id: COMMENT_ID,
  recipeId: RECIPE_ID,
  authorId: "33333333-0000-0000-0000-000000000003",
  authorName: "Анна Воронова",
  text: "Очень вкусно!",
  createdAt: "2026-06-09T14:20:00Z",
};

const mockPagedResult = {
  items: [mockComment],
  total: 1,
  page: 1,
  pageSize: 10,
};

beforeEach(() => {
  vi.stubGlobal("fetch", vi.fn());
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("getComments", () => {
  it("отправляет GET на /api/cookbook/v1/recipes/:id/comments", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockPagedResult), {
        status: 200,
        headers: { "Content-Type": "application/json" },
      }),
    );

    await getComments(RECIPE_ID);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(`/api/cookbook/v1/recipes/${RECIPE_ID}/comments`),
      expect.objectContaining({ cache: "no-store" }),
    );
  });

  it("передаёт параметры page и pageSize в URL", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(
        JSON.stringify({ ...mockPagedResult, page: 2, pageSize: 5 }),
        { status: 200 },
      ),
    );

    await getComments(RECIPE_ID, 2, 5);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining("page=2&pageSize=5"),
      expect.any(Object),
    );
  });

  it("возвращает распарсенный PagedResult", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockPagedResult), { status: 200 }),
    );

    const result = await getComments(RECIPE_ID);

    expect(result.items).toHaveLength(1);
    expect(result.items[0]?.text).toBe("Очень вкусно!");
    expect(result.total).toBe(1);
  });

  it("выбрасывает ошибку при 404", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 404 }));

    await expect(getComments(RECIPE_ID)).rejects.toThrow("404");
  });

  it("выбрасывает ошибку при 500", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 500 }));

    await expect(getComments(RECIPE_ID)).rejects.toThrow("500");
  });
});

describe("addComment", () => {
  it("отправляет POST на /api/cookbook/v1/recipes/:id/comments", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockComment), {
        status: 201,
        headers: { "Content-Type": "application/json" },
      }),
    );

    await addComment(RECIPE_ID, "Очень вкусно!");

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(`/api/cookbook/v1/recipes/${RECIPE_ID}/comments`),
      expect.objectContaining({ method: "POST" }),
    );
  });

  it("передаёт text в теле запроса", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockComment), { status: 201 }),
    );

    await addComment(RECIPE_ID, "Очень вкусно!");

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.any(String),
      expect.objectContaining({
        body: JSON.stringify({ text: "Очень вкусно!" }),
      }),
    );
  });

  it("возвращает созданный CommentDto", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(
      new Response(JSON.stringify(mockComment), { status: 201 }),
    );

    const result = await addComment(RECIPE_ID, "Очень вкусно!");

    expect(result.id).toBe(COMMENT_ID);
    expect(result.text).toBe("Очень вкусно!");
  });

  it("выбрасывает ошибку при 400", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 400 }));

    await expect(addComment(RECIPE_ID, "x")).rejects.toThrow("400");
  });

  it("выбрасывает ошибку при 401", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 401 }));

    await expect(addComment(RECIPE_ID, "x")).rejects.toThrow("401");
  });
});

describe("deleteComment", () => {
  it("отправляет DELETE на /api/cookbook/v1/recipes/:id/comments/:commentId", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 204 }));

    await deleteComment(RECIPE_ID, COMMENT_ID);

    expect(vi.mocked(fetch)).toHaveBeenCalledWith(
      expect.stringContaining(
        `/api/cookbook/v1/recipes/${RECIPE_ID}/comments/${COMMENT_ID}`,
      ),
      expect.objectContaining({ method: "DELETE" }),
    );
  });

  it("выбрасывает ошибку при 403", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 403 }));

    await expect(deleteComment(RECIPE_ID, COMMENT_ID)).rejects.toThrow("403");
  });

  it("выбрасывает ошибку при 401", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 401 }));

    await expect(deleteComment(RECIPE_ID, COMMENT_ID)).rejects.toThrow("401");
  });

  it("выбрасывает ошибку при 404", async () => {
    vi.mocked(fetch).mockResolvedValueOnce(new Response(null, { status: 404 }));

    await expect(deleteComment(RECIPE_ID, COMMENT_ID)).rejects.toThrow("404");
  });
});
