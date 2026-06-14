export function getRecipePhotoUrl(photoId: string): string {
  return `/api/cookbook/photos/${photoId}`;
}

export function getRecipeThumbnailUrl(photoId: string): string {
  return `/api/cookbook/photos/${photoId}/thumbnail`;
}
