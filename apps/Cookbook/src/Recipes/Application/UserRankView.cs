using Recipes.Domain;

namespace Recipes.Application;

internal sealed record UserRankView(
    UserId Id,
    string DisplayName,
    float? AverageRating,
    int CommentCount);
