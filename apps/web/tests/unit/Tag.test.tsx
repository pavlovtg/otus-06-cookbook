import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { Tag, Chip } from "@/components/ui/Tag";

describe("Tag", () => {
  it("рендерит children", () => {
    render(<Tag>Завтрак</Tag>);
    expect(screen.getByText("Завтрак")).toBeInTheDocument();
  });

  it("применяет класс tag по умолчанию", () => {
    const { container } = render(<Tag>Завтрак</Tag>);
    expect(container.querySelector(".tag")).toBeInTheDocument();
  });

  it("применяет tag-accent для tone=accent", () => {
    const { container } = render(<Tag tone="accent">Завтрак</Tag>);
    expect(container.querySelector(".tag-accent")).toBeInTheDocument();
  });

  it("применяет tag-private для tone=private", () => {
    const { container } = render(<Tag tone="private">Приватное</Tag>);
    expect(container.querySelector(".tag-private")).toBeInTheDocument();
  });

  it("рендерит LockIcon для tone=private без icon", () => {
    const { container } = render(<Tag tone="private">Приватное</Tag>);
    expect(container.querySelector("svg")).toBeInTheDocument();
  });

  it("рендерит custom icon вместо LockIcon для tone=private", () => {
    render(<Tag tone="private" icon={<span data-testid="custom-icon" />}>Приватное</Tag>);
    expect(screen.getByTestId("custom-icon")).toBeInTheDocument();
  });

  it("не рендерит иконку для tone=default без icon", () => {
    const { container } = render(<Tag tone="default">Завтрак</Tag>);
    expect(container.querySelector("svg")).not.toBeInTheDocument();
  });
});

describe("Chip", () => {
  it("рендерит children", () => {
    render(<Chip>Тег</Chip>);
    expect(screen.getByText("Тег")).toBeInTheDocument();
  });

  it("рендерит кнопку удаления если передан onRemove", () => {
    render(<Chip onRemove={() => {}}>Тег</Chip>);
    expect(screen.getByRole("button", { name: "Удалить" })).toBeInTheDocument();
  });

  it("не рендерит кнопку удаления без onRemove", () => {
    render(<Chip>Тег</Chip>);
    expect(screen.queryByRole("button")).not.toBeInTheDocument();
  });

  it("вызывает onRemove при клике", () => {
    const onRemove = vi.fn();
    render(<Chip onRemove={onRemove}>Тег</Chip>);
    fireEvent.click(screen.getByRole("button", { name: "Удалить" }));
    expect(onRemove).toHaveBeenCalledOnce();
  });
});
