namespace Recipes.Domain;

internal static class RecipePhotoConstraints
{
    public const long MaxSizeBytes = 10 * 1024 * 1024; // 10 MB

    public static readonly string[] AllowedMimeTypes = ["image/jpeg", "image/png"];
}
