using Recipes.Domain;

namespace Recipes.Application;

internal sealed record RecipeWithIngredientDetails(
    Recipe Recipe,
    IReadOnlyList<RecipeIngredientDetail> Ingredients,
    string? AuthorName,
    int? MyRating = null);
