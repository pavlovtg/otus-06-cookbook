using Recipes.Domain;
using Recipes.Domain.Exceptions;
using Xunit;

namespace Recipes.Tests.Domain;

public class RecipeWithIngredientsTests
{
    private const string ValidTitle = "Борщ";
    private const string ValidDescription = "Классический борщ";
    private const int ValidCookingTime = 90;
    private const Difficulty ValidDifficulty = Difficulty.Everyday;
    private const int ValidServings = 6;
    private const string ValidInstructions = "Сварить бульон. Добавить овощи.";

    private static RecipeIngredient MakeIngredient() =>
        RecipeIngredient.Create(IngredientId.New(), 100m);

    // ── Recipe.Create с ингредиентами ─────────────────────────────────────────

    [Fact]
    public void Create_WithIngredients_SetsIngredients()
    {
        var ingredients = new[] { MakeIngredient(), MakeIngredient() };

        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            ingredients);

        Assert.Equal(2, recipe.Ingredients.Count);
    }

    [Fact]
    public void Create_WithoutIngredients_HasEmptyIngredients()
    {
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions);

        Assert.Empty(recipe.Ingredients);
    }

    [Fact]
    public void Create_WithExactlyMaxIngredients_Succeeds()
    {
        var ingredients = Enumerable.Range(0, RecipeConstraints.IngredientsMaxCount)
            .Select(_ => MakeIngredient())
            .ToList();

        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            ingredients);

        Assert.Equal(RecipeConstraints.IngredientsMaxCount, recipe.Ingredients.Count);
    }

    [Fact]
    public void Create_WithTooManyIngredients_ThrowsRecipeIngredientsTooManyException()
    {
        var ingredients = Enumerable.Range(0, RecipeConstraints.IngredientsMaxCount + 1)
            .Select(_ => MakeIngredient())
            .ToList();

        var ex = Assert.Throws<RecipeIngredientsTooManyException>(() =>
            Recipe.Create(
                RecipeId.New(), ValidTitle, ValidDescription,
                ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
                ingredients));

        Assert.Equal(RecipeConstraints.IngredientsMaxCount + 1, ex.ActualCount);
        Assert.Equal(RecipeConstraints.IngredientsMaxCount, ex.MaxCount);
    }

    // ── Recipe.Update с ингредиентами ─────────────────────────────────────────

    [Fact]
    public void Update_WithIngredients_ReplacesIngredients()
    {
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            new[] { MakeIngredient() });

        var newIngredients = new[] { MakeIngredient(), MakeIngredient(), MakeIngredient() };
        recipe.Update(ValidTitle, ValidDescription, ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions, isPublic: true, newIngredients);

        Assert.Equal(3, recipe.Ingredients.Count);
    }

    [Fact]
    public void Update_WithEmptyIngredients_ClearsIngredients()
    {
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            new[] { MakeIngredient() });

        recipe.Update(ValidTitle, ValidDescription, ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions, isPublic: true, []);

        Assert.Empty(recipe.Ingredients);
    }

    [Fact]
    public void Update_WithTooManyIngredients_ThrowsRecipeIngredientsTooManyException()
    {
        var recipe = Recipe.Create(
            RecipeId.New(), ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions);

        var tooMany = Enumerable.Range(0, RecipeConstraints.IngredientsMaxCount + 1)
            .Select(_ => MakeIngredient())
            .ToList();

        Assert.Throws<RecipeIngredientsTooManyException>(() =>
            recipe.Update(ValidTitle, ValidDescription, ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions, isPublic: true, tooMany));
    }
}
