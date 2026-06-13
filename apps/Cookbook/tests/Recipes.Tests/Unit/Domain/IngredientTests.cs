using Recipes.Domain;
using Recipes.Domain.Exceptions;
using Xunit;

namespace Recipes.Tests.Domain;

public class IngredientTests
{
    private const string ValidTitle = "Морковь";
    private const string ValidUnit = "г";
    private const float ValidDefaultAmount = 100f;
    private const IngredientCategory ValidCategory = IngredientCategory.Vegetables;

    [Fact]
    public void Create_WithValidData_ReturnsIngredient()
    {
        var id = IngredientId.New();

        var ingredient = Ingredient.Create(id, ValidTitle, ValidUnit, ValidDefaultAmount, ValidCategory);

        Assert.Equal(id, ingredient.Id);
        Assert.Equal(ValidTitle, ingredient.Title);
        Assert.Equal(ValidUnit, ingredient.Unit);
        Assert.Equal(ValidDefaultAmount, ingredient.DefaultAmount);
        Assert.Equal(ValidCategory, ingredient.Category);
        Assert.False(ingredient.IsSystem);
    }

    [Fact]
    public void Create_WithIsSystem_SetsIsSystem()
    {
        var ingredient = Ingredient.Create(IngredientId.New(), ValidTitle, ValidUnit, ValidDefaultAmount, ValidCategory, isSystem: true);

        Assert.True(ingredient.IsSystem);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData(" ")]
    public void Create_WithTitleTooShort_ThrowsIngredientTitleTooShortException(string title)
    {
        Assert.Throws<IngredientTitleTooShortException>(() =>
            Ingredient.Create(IngredientId.New(), title, ValidUnit, ValidDefaultAmount, ValidCategory));
    }

    [Fact]
    public void Create_WithTitleTooLong_ThrowsIngredientTitleTooLongException()
    {
        var longTitle = new string('A', 201);

        var ex = Assert.Throws<IngredientTitleTooLongException>(() =>
            Ingredient.Create(IngredientId.New(), longTitle, ValidUnit, ValidDefaultAmount, ValidCategory));

        Assert.Equal(201, ex.ActualLength);
        Assert.Equal(IngredientConstraints.TitleMaxLength, ex.MaxLength);
    }

    [Fact]
    public void Create_WithUnitTooLong_ThrowsIngredientUnitTooLongException()
    {
        var longUnit = new string('г', 21);

        var ex = Assert.Throws<IngredientUnitTooLongException>(() =>
            Ingredient.Create(IngredientId.New(), ValidTitle, longUnit, ValidDefaultAmount, ValidCategory));

        Assert.Equal(21, ex.ActualLength);
    }

    [Theory]
    [InlineData(0f)]
    [InlineData(-1f)]
    [InlineData(100001f)]
    public void Create_WithInvalidDefaultAmount_ThrowsIngredientDefaultAmountOutOfRangeException(float amount)
    {
        var ex = Assert.Throws<IngredientDefaultAmountOutOfRangeException>(() =>
            Ingredient.Create(IngredientId.New(), ValidTitle, ValidUnit, amount, ValidCategory));

        Assert.Equal(amount, ex.ActualValue);
    }

    [Theory]
    [InlineData(0.001f)]
    [InlineData(100000f)]
    public void Create_WithBoundaryDefaultAmount_Succeeds(float amount)
    {
        var ingredient = Ingredient.Create(IngredientId.New(), ValidTitle, ValidUnit, amount, ValidCategory);

        Assert.Equal(amount, ingredient.DefaultAmount);
    }

    [Fact]
    public void Create_WithTitleMinLength_Succeeds()
    {
        var ingredient = Ingredient.Create(IngredientId.New(), "Аб", ValidUnit, ValidDefaultAmount, ValidCategory);

        Assert.Equal("Аб", ingredient.Title);
    }

    [Fact]
    public void Update_WithValidData_UpdatesIngredient()
    {
        var ingredient = Ingredient.Create(IngredientId.New(), ValidTitle, ValidUnit, ValidDefaultAmount, ValidCategory);

        ingredient.Update("Свёкла", "шт.", 2f, IngredientCategory.FruitsAndBerries);

        Assert.Equal("Свёкла", ingredient.Title);
        Assert.Equal("шт.", ingredient.Unit);
        Assert.Equal(2f, ingredient.DefaultAmount);
        Assert.Equal(IngredientCategory.FruitsAndBerries, ingredient.Category);
    }

    [Fact]
    public void Update_WithTitleTooShort_ThrowsIngredientTitleTooShortException()
    {
        var ingredient = Ingredient.Create(IngredientId.New(), ValidTitle, ValidUnit, ValidDefaultAmount, ValidCategory);

        Assert.Throws<IngredientTitleTooShortException>(() =>
            ingredient.Update("А", ValidUnit, ValidDefaultAmount, ValidCategory));
    }
}
