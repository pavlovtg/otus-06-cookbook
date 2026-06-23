namespace Recipes.Application;

internal sealed record CategoryCountView(
    string CategoryName,
    int RecipeCount);
