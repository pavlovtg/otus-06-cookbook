using Recipes.Domain;

namespace Recipes.Application;

internal sealed record RecipeShortWithAuthor(Recipe Recipe, string? AuthorName, bool? IsFavorite = null, float? AverageRating = null, int? MyRating = null);
