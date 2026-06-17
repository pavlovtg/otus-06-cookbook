import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { PaginationNav } from "@/components/ui/PaginationNav";

const mockPush = vi.fn();

vi.mock("next/navigation", () => ({
  useRouter: () => ({ push: mockPush }),
}));

beforeEach(() => {
  vi.clearAllMocks();
});

describe("PaginationNav", () => {
  it("рендерит пагинацию", () => {
    render(<PaginationNav page={1} totalPages={5} />);
    expect(screen.getByRole("button", { name: "1" })).toBeInTheDocument();
  });

  it("вызывает router.push с нужной страницей при клике", () => {
    render(<PaginationNav page={1} totalPages={5} />);
    fireEvent.click(screen.getByRole("button", { name: "3" }));
    expect(mockPush).toHaveBeenCalledWith("?page=3");
  });

  it("вызывает router.push при клике Вперёд", () => {
    render(<PaginationNav page={2} totalPages={5} />);
    fireEvent.click(screen.getByRole("button", { name: "Вперёд" }));
    expect(mockPush).toHaveBeenCalledWith("?page=3");
  });
});
