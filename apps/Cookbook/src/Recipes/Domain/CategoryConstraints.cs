namespace Recipes.Domain;

internal static class CategoryConstraints
{
    public const int NameMinLength = 1;
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 2000;
    public const int MaxCount = 1000;
}
