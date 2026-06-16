import { render, screen } from "@testing-library/react";
import { describe, it, expect } from "vitest";
import { Skeleton, EmptyState } from "@/components/ui/Skeleton";

describe("Skeleton", () => {
  it("рендерит элемент с классом skeleton", () => {
    const { container } = render(<Skeleton />);
    expect(container.querySelector(".skeleton")).toBeInTheDocument();
  });

  it("применяет skeleton-card для card=true", () => {
    const { container } = render(<Skeleton card />);
    expect(container.querySelector(".skeleton-card")).toBeInTheDocument();
  });

  it("не применяет skeleton-card без card", () => {
    const { container } = render(<Skeleton />);
    expect(container.querySelector(".skeleton-card")).not.toBeInTheDocument();
  });

  it("устанавливает height через style", () => {
    const { container } = render(<Skeleton height={40} />);
    const el = container.querySelector(".skeleton") as HTMLElement;
    expect(el.style.height).toBe("40px");
  });

  it("использует height=16 по умолчанию", () => {
    const { container } = render(<Skeleton />);
    const el = container.querySelector(".skeleton") as HTMLElement;
    expect(el.style.height).toBe("16px");
  });

  it("не устанавливает height для card=true", () => {
    const { container } = render(<Skeleton card />);
    const el = container.querySelector(".skeleton") as HTMLElement;
    expect(el.style.height).toBe("");
  });

  it("устанавливает width через style", () => {
    const { container } = render(<Skeleton width={200} />);
    const el = container.querySelector(".skeleton") as HTMLElement;
    expect(el.style.width).toBe("200px");
  });

  it("использует width=100% по умолчанию", () => {
    const { container } = render(<Skeleton />);
    const el = container.querySelector(".skeleton") as HTMLElement;
    expect(el.style.width).toBe("100%");
  });
});

describe("EmptyState", () => {
  it("рендерит title", () => {
    render(<EmptyState title="Ничего не найдено" />);
    expect(screen.getByText("Ничего не найдено")).toBeInTheDocument();
  });

  it("рендерит description", () => {
    render(<EmptyState description="Попробуйте другой запрос" />);
    expect(screen.getByText("Попробуйте другой запрос")).toBeInTheDocument();
  });

  it("рендерит eyebrow", () => {
    render(<EmptyState eyebrow="404" />);
    expect(screen.getByText("404")).toBeInTheDocument();
  });

  it("рендерит action", () => {
    render(<EmptyState action={<button>Создать</button>} />);
    expect(screen.getByRole("button", { name: "Создать" })).toBeInTheDocument();
  });

  it("не рендерит опциональные элементы если не переданы", () => {
    const { container } = render(<EmptyState />);
    expect(container.querySelector(".state-eyebrow")).not.toBeInTheDocument();
    expect(container.querySelector(".t-display")).not.toBeInTheDocument();
    expect(container.querySelector(".t-small")).not.toBeInTheDocument();
  });

  it("рендерит все элементы вместе", () => {
    render(
      <EmptyState
        eyebrow="Пусто"
        title="Нет рецептов"
        description="Добавьте первый рецепт"
        action={<button>Добавить</button>}
      />
    );
    expect(screen.getByText("Пусто")).toBeInTheDocument();
    expect(screen.getByText("Нет рецептов")).toBeInTheDocument();
    expect(screen.getByText("Добавьте первый рецепт")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Добавить" })).toBeInTheDocument();
  });
});
