using Recipes.Domain.Exceptions;

namespace Recipes.Domain;

internal sealed class RecipePhoto
{
    public RecipePhotoId Id { get; private set; }
    public RecipeId RecipeId { get; private set; }
    public byte[] OriginalData { get; private set; } = [];
    public byte[] ThumbnailData { get; private set; } = [];

    private RecipePhoto() { }

    public static RecipePhoto Create(
        RecipePhotoId id,
        RecipeId recipeId,
        string mimeType,
        byte[] originalData,
        byte[] thumbnailData)
    {
        RecipePhotoValidator.ValidateMime(mimeType);
        RecipePhotoValidator.ValidateSize(originalData.LongLength);

        return new RecipePhoto
        {
            Id = id,
            RecipeId = recipeId,
            OriginalData = originalData,
            ThumbnailData = thumbnailData,
        };
    }
}
