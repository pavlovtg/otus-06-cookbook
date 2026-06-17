using Recipes.Domain;
using Recipes.Domain.Exceptions;
using Xunit;

namespace Recipes.Tests.Domain;

public class RecipeTests
{
    private const string ValidTitle = "Борщ";
    private const string ValidDescription = "Классический борщ";
    private const int ValidCookingTime = 90;
    private const Difficulty ValidDifficulty = Difficulty.Everyday;
    private const int ValidServings = 6;
    private const string ValidInstructions = "Сварить бульон. Добавить овощи.";

    [Fact]
    public void Create_WithValidData_ReturnsRecipe()
    {
        var id = RecipeId.New();

        var recipe = Recipe.Create(
            id, ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions);

        Assert.Equal(id, recipe.Id);
        Assert.Equal(ValidTitle, recipe.Title);
        Assert.Equal(ValidDescription, recipe.Description);
        Assert.Equal(ValidCookingTime, recipe.CookingTime);
        Assert.Equal(ValidDifficulty, recipe.Difficulty);
        Assert.Equal(ValidServings, recipe.Servings);
        Assert.Equal(ValidInstructions, recipe.Instructions);
    }

    [Fact]
    public void Create_WithNullDescription_SetsEmptyDescription()
    {
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, null,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions);

        Assert.Equal(string.Empty, recipe.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyTitle_ThrowsRecipeTitleEmptyException(string title)
    {
        Assert.Throws<RecipeTitleEmptyException>(() =>
            Recipe.Create(RecipeId.New(), title, ValidDescription,
                ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions));
    }

    [Fact]
    public void Create_WithTitleTooLong_ThrowsRecipeTitleTooLongException()
    {
        var longTitle = new string('A', 201);

        var ex = Assert.Throws<RecipeTitleTooLongException>(() =>
            Recipe.Create(RecipeId.New(), longTitle, ValidDescription,
                ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions));

        Assert.Equal(201, ex.ActualLength);
        Assert.Equal(RecipeConstraints.TitleMaxLength, ex.MaxLength);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ThrowsRecipeDescriptionTooLongException()
    {
        var longDescription = new string('A', 2001);

        var ex = Assert.Throws<RecipeDescriptionTooLongException>(() =>
            Recipe.Create(RecipeId.New(), ValidTitle, longDescription,
                ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions));

        Assert.Equal(2001, ex.ActualLength);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10000)]
    public void Create_WithInvalidCookingTime_ThrowsRecipeCookingTimeOutOfRangeException(int cookingTime)
    {
        var ex = Assert.Throws<RecipeCookingTimeOutOfRangeException>(() =>
            Recipe.Create(RecipeId.New(), ValidTitle, ValidDescription,
                cookingTime, ValidDifficulty, ValidServings, ValidInstructions));

        Assert.Equal(cookingTime, ex.ActualValue);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100)]
    public void Create_WithInvalidServings_ThrowsRecipeServingsOutOfRangeException(int servings)
    {
        var ex = Assert.Throws<RecipeServingsOutOfRangeException>(() =>
            Recipe.Create(RecipeId.New(), ValidTitle, ValidDescription,
                ValidCookingTime, ValidDifficulty, servings, ValidInstructions));

        Assert.Equal(servings, ex.ActualValue);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyInstructions_ThrowsRecipeInstructionsEmptyException(string instructions)
    {
        Assert.Throws<RecipeInstructionsEmptyException>(() =>
            Recipe.Create(RecipeId.New(), ValidTitle, ValidDescription,
                ValidCookingTime, ValidDifficulty, ValidServings, instructions));
    }

    [Fact]
    public void Create_WithInstructionsTooLong_ThrowsRecipeInstructionsTooLongException()
    {
        var longInstructions = new string('A', 10001);

        var ex = Assert.Throws<RecipeInstructionsTooLongException>(() =>
            Recipe.Create(RecipeId.New(), ValidTitle, ValidDescription,
                ValidCookingTime, ValidDifficulty, ValidServings, longInstructions));

        Assert.Equal(10001, ex.ActualLength);
    }

    [Fact]
    public void Update_WithValidData_UpdatesRecipe()
    {
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions);

        recipe.Update("Новый борщ", "Новое описание", 60, Difficulty.Festive, 4, "Новые инструкции", isPublic: true);

        Assert.Equal("Новый борщ", recipe.Title);
        Assert.Equal("Новое описание", recipe.Description);
        Assert.Equal(60, recipe.CookingTime);
        Assert.Equal(Difficulty.Festive, recipe.Difficulty);
        Assert.Equal(4, recipe.Servings);
        Assert.Equal("Новые инструкции", recipe.Instructions);
    }

    [Fact]
    public void Update_WithEmptyTitle_ThrowsRecipeTitleEmptyException()
    {
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions);

        Assert.Throws<RecipeTitleEmptyException>(() =>
            recipe.Update("", ValidDescription, ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions, isPublic: true));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(9999)]
    public void Create_WithBoundaryCookingTime_Succeeds(int cookingTime)
    {
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            cookingTime, ValidDifficulty, ValidServings, ValidInstructions);

        Assert.Equal(cookingTime, recipe.CookingTime);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(99)]
    public void Create_WithBoundaryServings_Succeeds(int servings)
    {
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, servings, ValidInstructions);

        Assert.Equal(servings, recipe.Servings);
    }
}
