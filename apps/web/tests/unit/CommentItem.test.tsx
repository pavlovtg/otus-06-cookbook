import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { CommentItem } from "@/components/features/CommentItem";
import type { CommentDto } from "@/lib/bff/comments";

const mockComment: CommentDto = {
  id: "aaaaaaaa-0000-0000-0000-000000000001",
  recipeId: "11111111-0000-0000-0000-000000000001",
  authorId: "33333333-0000-0000-0000-000000000003",
  authorName: "Анна Воронова",
  text: "Очень вкусно!",
  createdAt: "2026-06-09T14:20:00Z",
};

describe("CommentItem", () => {
  it("отображает имя автора", () => {
    render(<CommentItem comment={mockComment} />);
    expect(screen.getByText("Анна Воронова")).toBeInTheDocument();
  });

  it("отображает текст комментария", () => {
    render(<CommentItem comment={mockComment} />);
    expect(screen.getByText("Очень вкусно!")).toBeInTheDocument();
  });

  it("отображает аватар с первой буквой имени", () => {
    render(<CommentItem comment={mockComment} />);
    expect(screen.getByText("А")).toBeInTheDocument();
  });

  it("отображает дату", () => {
    render(<CommentItem comment={mockComment} />);
    // Дата должна присутствовать в DOM (точный формат зависит от локали)
    const timeEl = document.querySelector(".time");
    expect(timeEl).not.toBeNull();
    expect(timeEl?.textContent).toBeTruthy();
  });

  it("не отображает кнопку удаления по умолчанию", () => {
    render(<CommentItem comment={mockComment} />);
    expect(screen.queryByRole("button", { name: /удалить/i })).toBeNull();
  });

  it("не отображает кнопку удаления при canDelete=false", () => {
    render(<CommentItem comment={mockComment} canDelete={false} />);
    expect(screen.queryByRole("button", { name: /удалить/i })).toBeNull();
  });

  it("отображает кнопку удаления при canDelete=true", () => {
    render(<CommentItem comment={mockComment} canDelete onDelete={vi.fn()} />);
    expect(screen.getByRole("button", { name: /удалить/i })).toBeInTheDocument();
  });

  it("вызывает onDelete при клике на кнопку удаления", () => {
    const onDelete = vi.fn();
    render(<CommentItem comment={mockComment} canDelete onDelete={onDelete} />);
    fireEvent.click(screen.getByRole("button", { name: /удалить/i }));
    expect(onDelete).toHaveBeenCalledTimes(1);
  });

  it("использует '?' как инициал если authorName пустой", () => {
    render(<CommentItem comment={{ ...mockComment, authorName: "" }} />);
    expect(screen.getByText("?")).toBeInTheDocument();
  });
});
