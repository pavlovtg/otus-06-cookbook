using Recipes.Domain;

namespace Recipes.Application;

internal sealed record RecipeRankView(
    RecipeId Id,
    string Title,
    float AverageRating);
