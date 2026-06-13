namespace Recipes.Domain;

internal static class RecipeConstraints
{
    public const int TitleMaxLength = 200;
    public const int DescriptionMaxLength = 2000;
    public const int InstructionsMaxLength = 10000;
    public const int DifficultyMaxLength = 20;

    public const int CookingTimeMin = 1;
    public const int CookingTimeMax = 9999;

    public const int ServingsMin = 1;
    public const int ServingsMax = 99;
}
