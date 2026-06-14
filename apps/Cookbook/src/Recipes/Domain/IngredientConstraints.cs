namespace Recipes.Domain;

internal static class IngredientConstraints
{
    public const int TitleMinLength = 2;
    public const int TitleMaxLength = 200;
    public const int UnitMaxLength = 20;

    public const float DefaultAmountMin = 0.001f;
    public const float DefaultAmountMax = 100_000f;
}
