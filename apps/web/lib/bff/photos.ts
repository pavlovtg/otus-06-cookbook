export function getRecipePhotoUrl(photoId: string): string {
  return `/api/cookbook/v1/photos/${photoId}`;
}

export function getRecipeThumbnailUrl(photoId: string): string {
  return `/api/cookbook/v1/photos/${photoId}/thumbnail`;
}
