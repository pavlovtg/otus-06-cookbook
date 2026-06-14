using Recipes.Domain;
using Xunit;

namespace Recipes.Tests.Domain;

public class IngredientIdTests
{
    [Fact]
    public void New_ReturnsDifferentIds()
    {
        var id1 = IngredientId.New();
        var id2 = IngredientId.New();

        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void New_ReturnsNonEmptyGuid()
    {
        var id = IngredientId.New();

        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void From_WithValidGuid_ReturnsIngredientId()
    {
        var guid = Guid.NewGuid();

        var id = IngredientId.From(guid);

        Assert.Equal(guid, id.Value);
    }

    [Fact]
    public void From_WithEmptyGuid_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => IngredientId.From(Guid.Empty));
    }

    [Fact]
    public void ToString_ReturnsGuidInDFormat()
    {
        var guid = Guid.NewGuid();
        var id = IngredientId.From(guid);

        Assert.Equal(guid.ToString("D"), id.ToString());
    }

    [Fact]
    public void Equality_SameGuid_AreEqual()
    {
        var guid = Guid.NewGuid();

        var id1 = IngredientId.From(guid);
        var id2 = IngredientId.From(guid);

        Assert.Equal(id1, id2);
    }
}
