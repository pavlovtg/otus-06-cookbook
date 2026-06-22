using Recipes.Domain;
using Recipes.Domain.Exceptions;
using Xunit;

namespace Recipes.Tests.Domain;

public class RecipeCommentTests
{
    private static readonly RecipeCommentId SomeId = RecipeCommentId.New();
    private static readonly RecipeId SomeRecipeId = RecipeId.New();
    private static readonly UserId SomeAuthorId = UserId.New();

    // ── Пустой текст ─────────────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyText_ThrowsCommentTextEmptyException(string text)
    {
        Assert.Throws<CommentTextEmptyException>(() =>
            RecipeComment.Create(SomeId, SomeRecipeId, SomeAuthorId, text));
    }

    // ── Текст длиннее 2000 символов ──────────────────────────────────────────

    [Fact]
    public void Create_WithTextLongerThan2000_ThrowsCommentTextTooLongException()
    {
        var text = new string('a', RecipeCommentConstraints.MaxTextLength + 1);

        var ex = Assert.Throws<CommentTextTooLongException>(() =>
            RecipeComment.Create(SomeId, SomeRecipeId, SomeAuthorId, text));

        Assert.Equal(text.Length, ex.Length);
    }

    // ── Граничные значения ───────────────────────────────────────────────────

    [Fact]
    public void Create_WithTextOf1Char_DoesNotThrow()
    {
        var text = new string('a', RecipeCommentConstraints.MinTextLength);

        var comment = RecipeComment.Create(SomeId, SomeRecipeId, SomeAuthorId, text);

        Assert.Equal(text, comment.Text);
    }

    [Fact]
    public void Create_WithTextOf2000Chars_DoesNotThrow()
    {
        var text = new string('a', RecipeCommentConstraints.MaxTextLength);

        var comment = RecipeComment.Create(SomeId, SomeRecipeId, SomeAuthorId, text);

        Assert.Equal(text, comment.Text);
    }
}
