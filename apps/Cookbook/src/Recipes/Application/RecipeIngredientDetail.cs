using Recipes.Domain;

namespace Recipes.Application;

internal sealed record RecipeIngredientDetail(
    IngredientId IngredientId,
    string Title,
    decimal Amount,
    string Unit);
