using Recipes.Domain;

namespace Recipes.Application;

internal sealed record CommentDetail(
    RecipeCommentId Id,
    RecipeId RecipeId,
    UserId AuthorId,
    string AuthorName,
    string Text,
    DateTime CreatedAt);
