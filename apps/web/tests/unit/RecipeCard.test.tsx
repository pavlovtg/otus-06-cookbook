import { render, screen } from "@testing-library/react";
import { RecipeCard } from "@/components/features/RecipeCard";
import { describe, it, expect } from "vitest";

describe("RecipeCard", () => {
  it("отображает название и описание рецепта", () => {
    const recipe = {
      id: "11111111-0000-0000-0000-000000000002",
      title: "Борщ",
      description: "Классический украинский борщ со свёклой",
      cookingTime: 120,
      difficulty: "everyday" as const,
      servings: 6,
      instructions: "1. Сварить бульон.",
    };

    render(<RecipeCard recipe={recipe} />);

    expect(screen.getByText("Борщ")).toBeInTheDocument();
    expect(
      screen.getByText("Классический украинский борщ со свёклой")
    ).toBeInTheDocument();
  });
});
