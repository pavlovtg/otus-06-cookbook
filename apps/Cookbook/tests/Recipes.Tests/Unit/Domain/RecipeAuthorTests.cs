using Recipes.Domain;
using Xunit;

namespace Recipes.Tests.Domain;

public class RecipeAuthorTests
{
    private static readonly RecipeId ValidId = RecipeId.New();
    private const string ValidTitle = "Борщ";
    private const string ValidDescription = "Классический борщ";
    private const int ValidCookingTime = 90;
    private const Difficulty ValidDifficulty = Difficulty.Everyday;
    private const int ValidServings = 6;
    private const string ValidInstructions = "Сварить бульон. Добавить овощи.";

    [Fact]
    public void Create_WithIsPublicTrue_SetsIsPublicTrue()
    {
        var authorId = UserId.From(Guid.NewGuid());

        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            isPublic: true, authorId: authorId);

        Assert.True(recipe.IsPublic);
        Assert.Equal(authorId, recipe.AuthorId);
    }

    [Fact]
    public void Create_WithIsPublicFalse_SetsIsPublicFalse()
    {
        var authorId = UserId.From(Guid.NewGuid());

        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            isPublic: false, authorId: authorId);

        Assert.False(recipe.IsPublic);
    }

    [Fact]
    public void Create_WithNullAuthorId_SetsAuthorIdNull()
    {
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            isPublic: true, authorId: null);

        Assert.Null(recipe.AuthorId);
    }

    [Fact]
    public void Update_WithIsPublicFalse_SetsIsPublicFalse()
    {
        var authorId = UserId.From(Guid.NewGuid());
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            isPublic: true, authorId: authorId);

        recipe.Update(ValidTitle, ValidDescription, ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            isPublic: false);

        Assert.False(recipe.IsPublic);
    }
}
