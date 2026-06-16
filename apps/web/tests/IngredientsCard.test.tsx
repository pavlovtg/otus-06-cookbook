import { render, screen, fireEvent } from "@testing-library/react";
import { IngredientsCard } from "@/components/features/IngredientsCard";
import { describe, it, expect } from "vitest";
import type { RecipeIngredientDto } from "@/lib/schemas/recipe";

const INGREDIENTS: RecipeIngredientDto[] = [
  {
    ingredientId: "aaaaaaaa-0000-0000-0000-000000000001",
    title: "Мука",
    amount: 200,
    unit: "г",
  },
  {
    ingredientId: "aaaaaaaa-0000-0000-0000-000000000002",
    title: "Молоко",
    amount: 0.5,
    unit: "л",
  },
];

describe("IngredientsCard", () => {
  it("3.1 отображает начальные порции и количества ингредиентов", () => {
    const { container } = render(
      <IngredientsCard ingredients={INGREDIENTS} baseServings={4} />
    );

    // span.value — контрол порций
    const valueEl = container.querySelector(".servings-control .value");
    expect(valueEl?.textContent?.replace(/\s+/g, " ").trim()).toBe("4 порц.");

    // подпись под списком
    const microEl = container.querySelector(".t-micro");
    expect(microEl?.textContent?.replace(/\s+/g, " ").trim()).toBe("на 4 порц.");

    expect(screen.getByText(/200/)).toBeInTheDocument();
    expect(screen.getByText(/0\.5/)).toBeInTheDocument();
  });

  it("3.2 клик «+» увеличивает порции и пересчитывает количества", () => {
    const { container } = render(
      <IngredientsCard ingredients={INGREDIENTS} baseServings={4} />
    );

    fireEvent.click(screen.getByRole("button", { name: "Увеличить порции" }));

    const valueEl = container.querySelector(".servings-control .value");
    expect(valueEl?.textContent?.replace(/\s+/g, " ").trim()).toBe("5 порц.");

    const microEl = container.querySelector(".t-micro");
    expect(microEl?.textContent?.replace(/\s+/g, " ").trim()).toBe("на 5 порц.");

    // 200 * (5/4) = 250
    expect(screen.getByText(/250/)).toBeInTheDocument();
    // 0.5 * (5/4) = 0.625 → round(0.625 * 100) / 100 = 0.63
    expect(screen.getByText(/0\.63/)).toBeInTheDocument();
  });

  it("3.3 клик «−» уменьшает порции и пересчитывает количества", () => {
    const { container } = render(
      <IngredientsCard ingredients={INGREDIENTS} baseServings={4} />
    );

    fireEvent.click(screen.getByRole("button", { name: "Уменьшить порции" }));

    const valueEl = container.querySelector(".servings-control .value");
    expect(valueEl?.textContent?.replace(/\s+/g, " ").trim()).toBe("3 порц.");

    const microEl = container.querySelector(".t-micro");
    expect(microEl?.textContent?.replace(/\s+/g, " ").trim()).toBe("на 3 порц.");

    // 200 * (3/4) = 150
    expect(screen.getByText(/150/)).toBeInTheDocument();
    // 0.5 * (3/4) = 0.375 → round(0.375 * 100) / 100 = 0.38
    expect(screen.getByText(/0\.38/)).toBeInTheDocument();
  });

  it("3.4 кнопка «−» disabled при currentServings === 1", () => {
    render(<IngredientsCard ingredients={INGREDIENTS} baseServings={1} />);

    const minusBtn = screen.getByRole("button", { name: "Уменьшить порции" });
    expect(minusBtn).toBeDisabled();

    // После нажатия «+» кнопка «−» должна стать активной
    fireEvent.click(screen.getByRole("button", { name: "Увеличить порции" }));
    expect(minusBtn).not.toBeDisabled();
  });

  it("3.5 кнопка «+» disabled при currentServings === 99", () => {
    render(<IngredientsCard ingredients={INGREDIENTS} baseServings={99} />);

    const plusBtn = screen.getByRole("button", { name: "Увеличить порции" });
    expect(plusBtn).toBeDisabled();

    // После нажатия «−» кнопка «+» должна стать активной
    fireEvent.click(screen.getByRole("button", { name: "Уменьшить порции" }));
    expect(plusBtn).not.toBeDisabled();
  });
});
