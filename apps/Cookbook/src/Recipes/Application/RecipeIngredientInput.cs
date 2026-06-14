using Recipes.Domain;

namespace Recipes.Application;

internal sealed record RecipeIngredientInput(
    IngredientId IngredientId,
    decimal Amount);
