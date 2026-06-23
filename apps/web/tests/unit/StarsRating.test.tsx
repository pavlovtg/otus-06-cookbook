import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { StarsRating } from "@/components/StarsRating";

describe("StarsRating", () => {
  it("рендерит 5 кнопок-звёзд", () => {
    render(<StarsRating />);
    const buttons = screen.getAllByRole("button");
    expect(buttons).toHaveLength(5);
  });

  it("вызывает onChange при клике на звезду", () => {
    const onChange = vi.fn();
    render(<StarsRating onChange={onChange} />);
    fireEvent.click(screen.getByLabelText("3 из 5"));
    expect(onChange).toHaveBeenCalledWith(3);
  });

  it("не вызывает onChange в readOnly-режиме", () => {
    const onChange = vi.fn();
    render(<StarsRating readOnly onChange={onChange} />);
    fireEvent.click(screen.getByLabelText("2 из 5"));
    expect(onChange).not.toHaveBeenCalled();
  });

  it("отображает переданное value (controlled)", () => {
    render(<StarsRating value={4} />);
    const buttons = screen.getAllByRole("button");
    const activeButtons = buttons.filter((b) => b.classList.contains("is-on"));
    expect(activeButtons).toHaveLength(4);
  });

  it("обновляет внутреннее состояние при клике (uncontrolled)", () => {
    render(<StarsRating defaultValue={0} />);
    fireEvent.click(screen.getByLabelText("5 из 5"));
    const buttons = screen.getAllByRole("button");
    const activeButtons = buttons.filter((b) => b.classList.contains("is-on"));
    expect(activeButtons).toHaveLength(5);
  });

  it("добавляет класс is-readonly в readOnly-режиме", () => {
    const { container } = render(<StarsRating readOnly />);
    expect(container.querySelector(".is-readonly")).toBeInTheDocument();
  });
});
