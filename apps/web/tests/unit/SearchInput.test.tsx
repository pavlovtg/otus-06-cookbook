import { render, screen, fireEvent } from "@testing-library/react";
import { describe, it, expect, vi } from "vitest";
import { SearchInput } from "@/components/ui/SearchInput";

describe("SearchInput", () => {
  it("рендерит input с placeholder по умолчанию", () => {
    render(<SearchInput />);
    expect(screen.getByPlaceholderText("Поиск…")).toBeInTheDocument();
  });

  it("рендерит input с кастомным placeholder", () => {
    render(<SearchInput placeholder="Найти рецепт" />);
    expect(screen.getByPlaceholderText("Найти рецепт")).toBeInTheDocument();
  });

  it("uncontrolled: обновляет значение при вводе", () => {
    render(<SearchInput />);
    const input = screen.getByPlaceholderText("Поиск…") as HTMLInputElement;
    fireEvent.change(input, { target: { value: "борщ" } });
    expect(input.value).toBe("борщ");
  });

  it("uncontrolled: вызывает onValueChange при вводе", () => {
    const onValueChange = vi.fn();
    render(<SearchInput onValueChange={onValueChange} />);
    fireEvent.change(screen.getByPlaceholderText("Поиск…"), { target: { value: "борщ" } });
    expect(onValueChange).toHaveBeenCalledWith("борщ");
  });

  it("controlled: использует переданное value", () => {
    render(<SearchInput value="суп" onValueChange={() => {}} />);
    const input = screen.getByPlaceholderText("Поиск…") as HTMLInputElement;
    expect(input.value).toBe("суп");
  });

  it("controlled: не меняет внутреннее состояние", () => {
    const onValueChange = vi.fn();
    render(<SearchInput value="суп" onValueChange={onValueChange} />);
    const input = screen.getByPlaceholderText("Поиск…") as HTMLInputElement;
    fireEvent.change(input, { target: { value: "борщ" } });
    expect(onValueChange).toHaveBeenCalledWith("борщ");
    // value остаётся controlled
    expect(input.value).toBe("суп");
  });

  it("не рендерит autocomplete без suggestions", () => {
    const { container } = render(<SearchInput />);
    expect(container.querySelector(".autocomplete")).not.toBeInTheDocument();
  });

  it("рендерит autocomplete с suggestions", () => {
    render(<SearchInput suggestions={[{ label: "Борщ" }, { label: "Суп" }]} />);
    expect(screen.getByText("Борщ")).toBeInTheDocument();
    expect(screen.getByText("Суп")).toBeInTheDocument();
  });

  it("рендерит kind для suggestion", () => {
    render(<SearchInput suggestions={[{ label: "Борщ", kind: "рецепт" }]} />);
    expect(screen.getByText("рецепт")).toBeInTheDocument();
  });

  it("не рендерит kind если не передан", () => {
    render(<SearchInput suggestions={[{ label: "Борщ" }]} />);
    expect(screen.queryByText("рецепт")).not.toBeInTheDocument();
  });

  it("открывает autocomplete при фокусе", () => {
    const { container } = render(
      <SearchInput suggestions={[{ label: "Борщ" }]} />
    );
    fireEvent.focus(screen.getByPlaceholderText("Поиск…"));
    expect(container.querySelector(".autocomplete.is-open")).toBeInTheDocument();
  });

  it("вызывает onPickSuggestion при клике на элемент", () => {
    const onPickSuggestion = vi.fn();
    render(
      <SearchInput
        suggestions={[{ label: "Борщ" }]}
        onPickSuggestion={onPickSuggestion}
      />
    );
    fireEvent.mouseDown(screen.getByText("Борщ").closest(".autocomplete-item")!);
    expect(onPickSuggestion).toHaveBeenCalledWith("Борщ");
  });

  it("устанавливает значение при выборе suggestion (uncontrolled)", () => {
    render(<SearchInput suggestions={[{ label: "Борщ" }]} />);
    const input = screen.getByPlaceholderText("Поиск…") as HTMLInputElement;
    fireEvent.mouseDown(screen.getByText("Борщ").closest(".autocomplete-item")!);
    expect(input.value).toBe("Борщ");
  });
});
