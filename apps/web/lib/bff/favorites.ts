const CLIENT_BASE = `/api/recipes`;

export async function addFavorite(recipeId: string): Promise<void> {
  const response = await fetch(`${CLIENT_BASE}/${recipeId}/favorites`, {
    method: "POST",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to add favorite: ${response.status}`);
  }
}

export async function removeFavorite(recipeId: string): Promise<void> {
  const response = await fetch(`${CLIENT_BASE}/${recipeId}/favorites`, {
    method: "DELETE",
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`Failed to remove favorite: ${response.status}`);
  }
}
