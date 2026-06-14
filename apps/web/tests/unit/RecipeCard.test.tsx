import { render, screen } from "@testing-library/react";
import { RecipeCard } from "@/components/features/RecipeCard";
import { describe, it, expect } from "vitest";

const PHOTO_ID = "aaaaaaaa-0000-0000-0000-000000000001";

describe("RecipeCard", () => {
  it("отображает название и описание рецепта", () => {
    const recipe = {
      id: "11111111-0000-0000-0000-000000000002",
      title: "Борщ",
      description: "Классический украинский борщ со свёклой",
      cookingTime: 120,
      difficulty: "everyday" as const,
      photoId: null,
    };

    render(<RecipeCard recipe={recipe} />);

    expect(screen.getByText("Борщ")).toBeInTheDocument();
    expect(
      screen.getByText("Классический украинский борщ со свёклой")
    ).toBeInTheDocument();
  });

  it("рендерит <img> если photoId != null", () => {
    const recipe = {
      id: "11111111-0000-0000-0000-000000000002",
      title: "Борщ",
      description: "Классический украинский борщ со свёклой",
      cookingTime: 120,
      difficulty: "everyday" as const,
      photoId: PHOTO_ID,
    };

    render(<RecipeCard recipe={recipe} />);

    const img = screen.getByRole("img", { name: "Борщ" });
    expect(img).toBeInTheDocument();
    const src = img.getAttribute("src") ?? "";
    const decodedSrc = src.startsWith("/_next/image")
      ? decodeURIComponent(new URL(src, "http://localhost").searchParams.get("url") ?? "")
      : src;
    expect(decodedSrc).toBe(`/api/cookbook/v1/photos/${PHOTO_ID}/thumbnail`);
  });

  it("рендерит SVG-заглушку если photoId == null", () => {
    const recipe = {
      id: "11111111-0000-0000-0000-000000000002",
      title: "Борщ",
      description: "Классический украинский борщ со свёклой",
      cookingTime: 120,
      difficulty: "everyday" as const,
      photoId: null,
    };

    const { container } = render(<RecipeCard recipe={recipe} />);

    expect(container.querySelector("svg")).toBeInTheDocument();
    expect(screen.queryByRole("img")).not.toBeInTheDocument();
  });
});
