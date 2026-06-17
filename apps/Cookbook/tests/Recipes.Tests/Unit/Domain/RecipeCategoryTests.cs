using Recipes.Domain;
using Recipes.Domain.Exceptions;
using Xunit;

namespace Recipes.Tests.Domain;

public class RecipeCategoryTests
{
    private static readonly RecipeId ValidId = RecipeId.New();
    private const string ValidTitle = "Борщ";
    private const string ValidDescription = "Описание";
    private const int ValidCookingTime = 60;
    private const Difficulty ValidDifficulty = Difficulty.Everyday;
    private const int ValidServings = 4;
    private const string ValidInstructions = "Шаг 1. Готовить.";

    private static Recipe CreateRecipe(IReadOnlyDictionary<CategoryId, CategoryType>? categoryTypes = null) =>
        Recipe.Create(ValidId, ValidTitle, ValidDescription,
            ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            categoryTypes: categoryTypes);

    [Fact]
    public void Create_WithEmptyCategoryList_Succeeds()
    {
        var recipe = CreateRecipe(new Dictionary<CategoryId, CategoryType>());

        Assert.Empty(recipe.Categories);
    }

    [Fact]
    public void Create_WithNullCategoryTypes_HasEmptyCategories()
    {
        var recipe = CreateRecipe(categoryTypes: null);

        Assert.Empty(recipe.Categories);
    }

    [Fact]
    public void Create_WithDuplicateCategoryType_ThrowsRecipeDuplicateCategoryTypeException()
    {
        var id1 = CategoryId.New();
        var id2 = CategoryId.New();

        var categoryTypes = new Dictionary<CategoryId, CategoryType>
        {
            [id1] = CategoryType.MealRole,
            [id2] = CategoryType.MealRole,
        };

        var ex = Assert.Throws<RecipeDuplicateCategoryTypeException>(() => CreateRecipe(categoryTypes));
        Assert.Equal(CategoryType.MealRole, ex.DuplicateType);
    }

    [Fact]
    public void Create_WithMultipleDifferentTypes_Succeeds()
    {
        var id1 = CategoryId.New();
        var id2 = CategoryId.New();
        var id3 = CategoryId.New();

        var categoryTypes = new Dictionary<CategoryId, CategoryType>
        {
            [id1] = CategoryType.MealRole,
            [id2] = CategoryType.CookingMethod,
            [id3] = CategoryType.Cuisine,
        };

        var recipe = CreateRecipe(categoryTypes);
        var categoryIds = recipe.Categories.Select(c => c.CategoryId).ToList();

        Assert.Equal(3, recipe.Categories.Count);
        Assert.Contains(id1, categoryIds);
        Assert.Contains(id2, categoryIds);
        Assert.Contains(id3, categoryIds);
    }

    [Fact]
    public void Update_WithDuplicateCategoryType_ThrowsRecipeDuplicateCategoryTypeException()
    {
        var recipe = CreateRecipe();

        var id1 = CategoryId.New();
        var id2 = CategoryId.New();

        var categoryTypes = new Dictionary<CategoryId, CategoryType>
        {
            [id1] = CategoryType.Dietary,
            [id2] = CategoryType.Dietary,
        };

        Assert.Throws<RecipeDuplicateCategoryTypeException>(() =>
            recipe.Update(ValidTitle, ValidDescription, ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
                isPublic: true, categoryTypes: categoryTypes));
    }

    [Fact]
    public void Update_ReplacesCategoryIds()
    {
        var id1 = CategoryId.New();
        var recipe = CreateRecipe(new Dictionary<CategoryId, CategoryType>
        {
            [id1] = CategoryType.MealRole,
        });

        var id2 = CategoryId.New();
        recipe.Update(ValidTitle, ValidDescription, ValidCookingTime, ValidDifficulty, ValidServings, ValidInstructions,
            isPublic: true, categoryTypes: new Dictionary<CategoryId, CategoryType>
            {
                [id2] = CategoryType.MealRole,
            });

        var categoryIds = recipe.Categories.Select(c => c.CategoryId).ToList();
        Assert.Single(recipe.Categories);
        Assert.Contains(id2, categoryIds);
        Assert.DoesNotContain(id1, categoryIds);
    }
}
