using Recipes.Domain.Exceptions;

namespace Recipes.Domain;

internal static class RecipePhotoValidator
{
    public static void ValidateMime(string mimeType)
    {
        if (!RecipePhotoConstraints.AllowedMimeTypes.Contains(mimeType))
            throw new RecipePhotoInvalidMimeException();
    }

    public static void ValidateSize(long sizeBytes)
    {
        if (sizeBytes > RecipePhotoConstraints.MaxSizeBytes)
            throw new RecipePhotoTooLargeException();
    }
}
