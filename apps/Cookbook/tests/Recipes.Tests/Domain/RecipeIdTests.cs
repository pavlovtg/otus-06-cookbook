using Recipes.Domain;
using Xunit;

namespace Recipes.Tests.Domain;

public class RecipeIdTests
{
    [Fact]
    public void New_ReturnsDifferentIds()
    {
        var id1 = RecipeId.New();
        var id2 = RecipeId.New();

        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void New_ReturnsNonEmptyGuid()
    {
        var id = RecipeId.New();

        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void From_WithValidGuid_ReturnsRecipeId()
    {
        var guid = Guid.NewGuid();

        var id = RecipeId.From(guid);

        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_WithEmptyGuid_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => RecipeId.From(Guid.Empty));
    }

    [Fact]
    public void ToString_ReturnsGuidInDFormat()
    {
        var guid = Guid.NewGuid();
        var id = RecipeId.From(guid);

        Assert.Equal(guid.ToString("D"), id.ToString());
    }

    [Fact]
    public void Equality_SameGuid_AreEqual()
    {
        var guid = Guid.NewGuid();

        var id1 = RecipeId.From(guid);
        var id2 = RecipeId.From(guid);

        Assert.Equal(id1, id2);
    }
}
