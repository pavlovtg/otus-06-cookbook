import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { FavoriteButton } from "@/components/features/FavoriteButton";

vi.mock("@/lib/bff/favorites", () => ({
  addFavorite: vi.fn().mockResolvedValue(undefined),
  removeFavorite: vi.fn().mockResolvedValue(undefined),
}));

import { addFavorite, removeFavorite } from "@/lib/bff/favorites";

beforeEach(() => {
  vi.clearAllMocks();
});

afterEach(() => {
  vi.clearAllMocks();
});

const RECIPE_ID = "11111111-0000-0000-0000-000000000001";

describe("FavoriteButton", () => {
  it("рендерит кнопку 'Добавить в избранное' когда isFavorite=false", () => {
    render(<FavoriteButton recipeId={RECIPE_ID} isFavorite={false} />);
    expect(screen.getByRole("button", { name: "Добавить в избранное" })).toBeInTheDocument();
  });

  it("рендерит кнопку 'Убрать из избранного' когда isFavorite=true", () => {
    render(<FavoriteButton recipeId={RECIPE_ID} isFavorite={true} />);
    expect(screen.getByRole("button", { name: "Убрать из избранного" })).toBeInTheDocument();
  });

  it("вызывает addFavorite при клике когда isFavorite=false", async () => {
    render(<FavoriteButton recipeId={RECIPE_ID} isFavorite={false} />);
    fireEvent.click(screen.getByRole("button"));
    await waitFor(() => {
      expect(addFavorite).toHaveBeenCalledWith(RECIPE_ID);
    });
  });

  it("вызывает removeFavorite при клике когда isFavorite=true", async () => {
    render(<FavoriteButton recipeId={RECIPE_ID} isFavorite={true} />);
    fireEvent.click(screen.getByRole("button"));
    await waitFor(() => {
      expect(removeFavorite).toHaveBeenCalledWith(RECIPE_ID);
    });
  });

  it("переключает aria-label после успешного клика", async () => {
    render(<FavoriteButton recipeId={RECIPE_ID} isFavorite={false} />);
    fireEvent.click(screen.getByRole("button"));
    await waitFor(() => {
      expect(screen.getByRole("button", { name: "Убрать из избранного" })).toBeInTheDocument();
    });
  });

  it("не меняет состояние при ошибке", async () => {
    vi.mocked(addFavorite).mockRejectedValueOnce(new Error("network error"));
    render(<FavoriteButton recipeId={RECIPE_ID} isFavorite={false} />);
    fireEvent.click(screen.getByRole("button"));
    await waitFor(() => {
      expect(screen.getByRole("button", { name: "Добавить в избранное" })).toBeInTheDocument();
    });
  });
});
