using Recipes.Domain;
using Recipes.Domain.Exceptions;
using Xunit;

namespace Recipes.Tests.Domain;

public class CategoryTests
{
    private const string ValidName = "Первое блюдо";
    private const string ValidDescription = "Супы и похлёбки.";
    private const CategoryType ValidType = CategoryType.MealRole;

    [Fact]
    public void Create_WithValidData_ReturnsCategory()
    {
        var id = CategoryId.New();

        var category = Category.Create(id, ValidName, ValidDescription, ValidType);

        Assert.Equal(id, category.Id);
        Assert.Equal(ValidName, category.Name);
        Assert.Equal(ValidDescription, category.Description);
        Assert.Equal(ValidType, category.Type);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithNameTooShort_ThrowsCategoryNameTooShortException(string name)
    {
        Assert.Throws<CategoryNameTooShortException>(() =>
            Category.Create(CategoryId.New(), name, ValidDescription, ValidType));
    }

    [Fact]
    public void Create_WithNameTooLong_ThrowsCategoryNameTooLongException()
    {
        var longName = new string('А', CategoryConstraints.NameMaxLength + 1);

        Assert.Throws<CategoryNameTooLongException>(() =>
            Category.Create(CategoryId.New(), longName, ValidDescription, ValidType));
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ThrowsCategoryDescriptionTooLongException()
    {
        var longDesc = new string('А', CategoryConstraints.DescriptionMaxLength + 1);

        Assert.Throws<CategoryDescriptionTooLongException>(() =>
            Category.Create(CategoryId.New(), ValidName, longDesc, ValidType));
    }

    [Fact]
    public void Create_WithEmptyDescription_Succeeds()
    {
        var category = Category.Create(CategoryId.New(), ValidName, string.Empty, ValidType);

        Assert.Equal(string.Empty, category.Description);
    }

    [Fact]
    public void Create_WithNameAtMaxLength_Succeeds()
    {
        var maxName = new string('А', CategoryConstraints.NameMaxLength);

        var category = Category.Create(CategoryId.New(), maxName, ValidDescription, ValidType);

        Assert.Equal(maxName, category.Name);
    }

    [Fact]
    public void Create_WithDescriptionAtMaxLength_Succeeds()
    {
        var maxDesc = new string('А', CategoryConstraints.DescriptionMaxLength);

        var category = Category.Create(CategoryId.New(), ValidName, maxDesc, ValidType);

        Assert.Equal(maxDesc, category.Description);
    }

    [Fact]
    public void Create_WithAllCategoryTypes_Succeeds()
    {
        foreach (var type in Enum.GetValues<CategoryType>())
        {
            var category = Category.Create(CategoryId.New(), ValidName, ValidDescription, type);
            Assert.Equal(type, category.Type);
        }
    }

    [Fact]
    public void Update_WithValidData_UpdatesNameAndDescription()
    {
        var category = Category.Create(CategoryId.New(), ValidName, ValidDescription, ValidType);

        category.Update("Второе блюдо", "Основные горячие блюда.");

        Assert.Equal("Второе блюдо", category.Name);
        Assert.Equal("Основные горячие блюда.", category.Description);
    }

    [Fact]
    public void Update_DoesNotChangeType()
    {
        var category = Category.Create(CategoryId.New(), ValidName, ValidDescription, ValidType);

        category.Update("Новое имя", "Новое описание.");

        Assert.Equal(ValidType, category.Type);
    }

    [Fact]
    public void Update_WithNameTooShort_ThrowsCategoryNameTooShortException()
    {
        var category = Category.Create(CategoryId.New(), ValidName, ValidDescription, ValidType);

        Assert.Throws<CategoryNameTooShortException>(() =>
            category.Update("", ValidDescription));
    }

    [Fact]
    public void Update_WithDescriptionTooLong_ThrowsCategoryDescriptionTooLongException()
    {
        var category = Category.Create(CategoryId.New(), ValidName, ValidDescription, ValidType);
        var longDesc = new string('А', CategoryConstraints.DescriptionMaxLength + 1);

        Assert.Throws<CategoryDescriptionTooLongException>(() =>
            category.Update(ValidName, longDesc));
    }
}
