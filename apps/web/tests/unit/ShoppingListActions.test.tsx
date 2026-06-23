import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach } from "vitest";
import { ShoppingListActions } from "@/app/shopping-list/ShoppingListActions";
import type { ShoppingListGroup } from "@/lib/schemas/shopping-list";

const mockGroups: ShoppingListGroup[] = [
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
  Object.defineProperty(navigator, "clipboard", {
    value: { writeText: vi.fn().mockResolvedValue(undefined) },
    writable: true,
    configurable: true,
  });
  vi.spyOn(window, "print").mockImplementation(() => undefined);
});

describe("ShoppingListActions", () => {
  it("рендерит кнопки «Скопировать» и «Распечатать»", () => {
    render(<ShoppingListActions groups={mockGroups} />);
    expect(screen.getByText("Скопировать")).toBeInTheDocument();
    expect(screen.getByText("Распечатать")).toBeInTheDocument();
  });

  it("вызывает clipboard.writeText при клике «Скопировать»", async () => {
    render(<ShoppingListActions groups={mockGroups} />);
    fireEvent.click(screen.getByText("Скопировать"));
    await waitFor(() => {
      expect(navigator.clipboard.writeText).toHaveBeenCalledOnce();
    });
    const text = vi.mocked(navigator.clipboard.writeText).mock.calls[0]![0];
    expect(text).toContain("Овощи");
    expect(text).toContain("Морковь");
    expect(text).toContain("2 кг");
    expect(text).toContain("Молочные продукты");
  });

  it("показывает «Скопировано» после успешного копирования", async () => {
    render(<ShoppingListActions groups={mockGroups} />);
    fireEvent.click(screen.getByText("Скопировать"));
    await waitFor(() => {
      expect(screen.getByText("Скопировано")).toBeInTheDocument();
    });
  });

  it("вызывает window.print при клике «Распечатать»", () => {
    render(<ShoppingListActions groups={mockGroups} />);
    fireEvent.click(screen.getByText("Распечатать"));
    expect(window.print).toHaveBeenCalledOnce();
  });

  it("корректно формирует текст для пустого списка групп", async () => {
    render(<ShoppingListActions groups={[]} />);
    fireEvent.click(screen.getByText("Скопировать"));
    await waitFor(() => {
      expect(navigator.clipboard.writeText).toHaveBeenCalledWith("");
    });
  });
});
