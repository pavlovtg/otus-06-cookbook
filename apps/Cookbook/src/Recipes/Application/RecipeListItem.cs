using Recipes.Domain;

namespace Recipes.Application;

internal sealed record RecipeListItem(Recipe Recipe, int? MyRating);
