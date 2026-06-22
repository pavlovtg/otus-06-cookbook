using Recipes.Domain.Exceptions;

namespace Recipes.Domain;

internal sealed class RecipeComment
{
    public RecipeCommentId Id { get; private set; }
    public RecipeId RecipeId { get; private set; }
    public UserId AuthorId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private RecipeComment() { }

    public static RecipeComment Create(
        RecipeCommentId id,
        RecipeId recipeId,
        UserId authorId,
        string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length < RecipeCommentConstraints.MinTextLength)
            throw new CommentTextEmptyException();

        if (text.Length > RecipeCommentConstraints.MaxTextLength)
            throw new CommentTextTooLongException(text.Length);

        return new RecipeComment
        {
            Id = id,
            RecipeId = recipeId,
            AuthorId = authorId,
            Text = text,
            CreatedAt = DateTime.UtcNow,
        };
    }
}
