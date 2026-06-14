using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IRecipePhotoService
{
    Task UploadAsync(RecipeId recipeId, string mimeType, Stream stream, CancellationToken cancellationToken = default);
    Task DeleteAsync(RecipeId recipeId, CancellationToken cancellationToken = default);
    Task<byte[]> GetOriginalAsync(RecipePhotoId id, CancellationToken cancellationToken = default);
    Task<byte[]> GetThumbnailAsync(RecipePhotoId id, CancellationToken cancellationToken = default);
}
