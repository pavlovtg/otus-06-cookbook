import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { Pagination } from "@/components/ui/Pagination";

describe("Pagination", () => {
  it("возвращает null если total <= 1", () => {
    const { container } = render(<Pagination total={1} />);
    expect(container.firstChild).toBeNull();
  });

  it("возвращает null если total = 0", () => {
    const { container } = render(<Pagination total={0} />);
    expect(container.firstChild).toBeNull();
  });

  it("рендерит все страницы если total <= 7", () => {
    render(<Pagination total={5} defaultPage={1} />);
    expect(screen.getByText("1")).toBeInTheDocument();
    expect(screen.getByText("5")).toBeInTheDocument();
  });

  it("рендерит ellipsis для большого total", () => {
    render(<Pagination total={20} defaultPage={10} />);
    const ellipses = screen.getAllByText("…");
    expect(ellipses.length).toBeGreaterThanOrEqual(1);
  });

  it("рендерит только правый ellipsis когда текущая страница близко к началу", () => {
    render(<Pagination total={20} defaultPage={2} />);
    // страница 2, delta=2 → left=0, right=4 → нет левого разрыва
    const ellipses = screen.getAllByText("…");
    expect(ellipses).toHaveLength(1);
  });

  it("рендерит только левый ellipsis когда текущая страница близко к концу", () => {
    render(<Pagination total={20} defaultPage={19} />);
    const ellipses = screen.getAllByText("…");
    expect(ellipses).toHaveLength(1);
  });

  it("кнопка Назад задизейблена на первой странице", () => {
    render(<Pagination total={5} defaultPage={1} />);
    expect(screen.getByRole("button", { name: "Назад" })).toBeDisabled();
  });

  it("кнопка Вперёд задизейблена на последней странице", () => {
    render(<Pagination total={5} defaultPage={5} />);
    expect(screen.getByRole("button", { name: "Вперёд" })).toBeDisabled();
  });

  it("uncontrolled: переходит на следующую страницу", () => {
    const onChange = vi.fn();
    render(<Pagination total={5} defaultPage={1} onChange={onChange} />);
    fireEvent.click(screen.getByRole("button", { name: "Вперёд" }));
    expect(onChange).toHaveBeenCalledWith(2);
  });

  it("uncontrolled: переходит на предыдущую страницу", () => {
    const onChange = vi.fn();
    render(<Pagination total={5} defaultPage={3} onChange={onChange} />);
    fireEvent.click(screen.getByRole("button", { name: "Назад" }));
    expect(onChange).toHaveBeenCalledWith(2);
  });

  it("uncontrolled: переходит на конкретную страницу", () => {
    const onChange = vi.fn();
    render(<Pagination total={5} defaultPage={1} onChange={onChange} />);
    fireEvent.click(screen.getByRole("button", { name: "3" }));
    expect(onChange).toHaveBeenCalledWith(3);
  });

  it("controlled: использует переданный page", () => {
    render(<Pagination total={5} page={3} />);
    const activeBtn = document.querySelector(".page-btn.is-active");
    expect(activeBtn?.textContent).toBe("3");
  });

  it("controlled: вызывает onChange но не меняет внутреннее состояние", () => {
    const onChange = vi.fn();
    render(<Pagination total={5} page={3} onChange={onChange} />);
    fireEvent.click(screen.getByRole("button", { name: "Вперёд" }));
    expect(onChange).toHaveBeenCalledWith(4);
    // page остаётся 3 (controlled)
    const activeBtn = document.querySelector(".page-btn.is-active");
    expect(activeBtn?.textContent).toBe("3");
  });

  it("активная страница имеет класс is-active", () => {
    render(<Pagination total={5} defaultPage={2} />);
    const btn2 = screen.getByRole("button", { name: "2" });
    expect(btn2).toHaveClass("is-active");
  });
});
