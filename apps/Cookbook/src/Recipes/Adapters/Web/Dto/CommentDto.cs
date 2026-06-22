namespace Recipes.Adapters.Web.Dto;

internal sealed record CommentDto(
    Guid Id,
    Guid RecipeId,
    Guid AuthorId,
    string AuthorName,
    string Text,
    DateTime CreatedAt);
