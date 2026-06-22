import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { CommentsSection } from "@/components/features/CommentsSection";
import type { PagedResultCommentDto } from "@/lib/bff/comments";

vi.mock("@/lib/bff/comments", () => ({
  getComments: vi.fn(),
  addComment: vi.fn(),
  deleteComment: vi.fn(),
}));

import { getComments, addComment, deleteComment } from "@/lib/bff/comments";

const RECIPE_ID = "11111111-0000-0000-0000-000000000001";
const USER_ID = "33333333-0000-0000-0000-000000000003";
const AUTHOR_ID = "44444444-0000-0000-0000-000000000004";

const mockComment = {
  id: "aaaaaaaa-0000-0000-0000-000000000001",
  recipeId: RECIPE_ID,
  authorId: AUTHOR_ID,
  authorName: "Анна Воронова",
  text: "Очень вкусно!",
  createdAt: "2026-06-09T14:20:00Z",
};

const emptyData: PagedResultCommentDto = {
  items: [],
  total: 0,
  page: 1,
  pageSize: 10,
};

const dataWithComment: PagedResultCommentDto = {
  items: [mockComment],
  total: 1,
  page: 1,
  pageSize: 10,
};

const myComment = { ...mockComment, authorId: USER_ID };
const dataWithMyComment: PagedResultCommentDto = {
  items: [myComment],
  total: 1,
  page: 1,
  pageSize: 10,
};

beforeEach(() => {
  vi.clearAllMocks();
  vi.mocked(getComments).mockResolvedValue(emptyData);
  vi.mocked(addComment).mockResolvedValue({
    ...mockComment,
    id: "bbbbbbbb-0000-0000-0000-000000000002",
  });
  vi.mocked(deleteComment).mockResolvedValue(undefined);
});

describe("CommentsSection", () => {
  it("отображает заголовок 'Комментарии'", () => {
    render(
      <CommentsSection recipeId={RECIPE_ID} initialData={emptyData} />,
    );
    expect(screen.getByText(/комментарии/i)).toBeInTheDocument();
  });

  it("отображает сообщение если комментариев нет", () => {
    render(
      <CommentsSection recipeId={RECIPE_ID} initialData={emptyData} />,
    );
    expect(screen.getByText(/комментариев пока нет/i)).toBeInTheDocument();
  });

  it("отображает список комментариев", () => {
    render(
      <CommentsSection recipeId={RECIPE_ID} initialData={dataWithComment} />,
    );
    expect(screen.getByText("Очень вкусно!")).toBeInTheDocument();
    expect(screen.getByText("Анна Воронова")).toBeInTheDocument();
  });

  it("не отображает форму если пользователь не авторизован", () => {
    render(
      <CommentsSection recipeId={RECIPE_ID} initialData={emptyData} />,
    );
    expect(screen.queryByPlaceholderText(/напишите комментарий/i)).toBeNull();
  });

  it("отображает форму для авторизованного пользователя без своего комментария", () => {
    render(
      <CommentsSection
        recipeId={RECIPE_ID}
        initialData={emptyData}
        currentUserId={USER_ID}
      />,
    );
    expect(screen.getByPlaceholderText(/напишите комментарий/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /отправить/i })).toBeInTheDocument();
  });

  it("скрывает форму если пользователь уже оставил комментарий", () => {
    render(
      <CommentsSection
        recipeId={RECIPE_ID}
        initialData={dataWithMyComment}
        currentUserId={USER_ID}
      />,
    );
    expect(screen.queryByPlaceholderText(/напишите комментарий/i)).toBeNull();
  });

  it("кнопка 'Отправить' задизейблена при пустом тексте", () => {
    render(
      <CommentsSection
        recipeId={RECIPE_ID}
        initialData={emptyData}
        currentUserId={USER_ID}
      />,
    );
    expect(screen.getByRole("button", { name: /отправить/i })).toBeDisabled();
  });

  it("кнопка 'Отправить' активна при введённом тексте", () => {
    render(
      <CommentsSection
        recipeId={RECIPE_ID}
        initialData={emptyData}
        currentUserId={USER_ID}
      />,
    );
    fireEvent.change(screen.getByPlaceholderText(/напишите комментарий/i), {
      target: { value: "Отличный рецепт!" },
    });
    expect(screen.getByRole("button", { name: /отправить/i })).not.toBeDisabled();
  });

  it("вызывает addComment при сабмите формы", async () => {
    vi.mocked(getComments).mockResolvedValue(emptyData);
    render(
      <CommentsSection
        recipeId={RECIPE_ID}
        initialData={emptyData}
        currentUserId={USER_ID}
      />,
    );
    fireEvent.change(screen.getByPlaceholderText(/напишите комментарий/i), {
      target: { value: "Отличный рецепт!" },
    });
    fireEvent.click(screen.getByRole("button", { name: /отправить/i }));
    await waitFor(() => {
      expect(addComment).toHaveBeenCalledWith(RECIPE_ID, "Отличный рецепт!");
    });
  });

  it("отображает кнопку удаления для автора комментария", () => {
    render(
      <CommentsSection
        recipeId={RECIPE_ID}
        initialData={dataWithMyComment}
        currentUserId={USER_ID}
      />,
    );
    expect(screen.getByRole("button", { name: /удалить/i })).toBeInTheDocument();
  });

  it("не отображает кнопку удаления для чужого комментария", () => {
    render(
      <CommentsSection
        recipeId={RECIPE_ID}
        initialData={dataWithComment}
        currentUserId={USER_ID}
      />,
    );
    expect(screen.queryByRole("button", { name: /удалить/i })).toBeNull();
  });

  it("отображает кнопку удаления для автора рецепта", () => {
    render(
      <CommentsSection
        recipeId={RECIPE_ID}
        initialData={dataWithComment}
        currentUserId={USER_ID}
        recipeAuthorId={USER_ID}
      />,
    );
    expect(screen.getByRole("button", { name: /удалить/i })).toBeInTheDocument();
  });

  it("отображает кнопку удаления для admin", () => {
    render(
      <CommentsSection
        recipeId={RECIPE_ID}
        initialData={dataWithComment}
        currentUserId={USER_ID}
        isAdmin
      />,
    );
    expect(screen.getByRole("button", { name: /удалить/i })).toBeInTheDocument();
  });

  it("вызывает deleteComment при клике на удаление", async () => {
    vi.mocked(getComments).mockResolvedValue(dataWithMyComment);
    render(
      <CommentsSection
        recipeId={RECIPE_ID}
        initialData={dataWithMyComment}
        currentUserId={USER_ID}
      />,
    );
    fireEvent.click(screen.getByRole("button", { name: /удалить/i }));
    await waitFor(() => {
      expect(deleteComment).toHaveBeenCalledWith(RECIPE_ID, myComment.id);
    });
  });

  it("отображает счётчик комментариев в заголовке", () => {
    render(
      <CommentsSection recipeId={RECIPE_ID} initialData={dataWithComment} />,
    );
    expect(screen.getByText(/комментарии \(1\)/i)).toBeInTheDocument();
  });
});
