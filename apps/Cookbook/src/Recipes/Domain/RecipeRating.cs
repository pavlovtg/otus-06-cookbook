using Recipes.Domain.Exceptions;

namespace Recipes.Domain;

internal sealed class RecipeRating
{
    public UserId UserId { get; private set; }
    public RecipeId RecipeId { get; private set; }
    public int Value { get; private set; }

    private RecipeRating() { }

    public static RecipeRating Create(UserId userId, RecipeId recipeId, int value)
    {
        if (value < RecipeConstraints.RatingMin || value > RecipeConstraints.RatingMax)
            throw new RatingValueOutOfRangeException(value);

        return new() { UserId = userId, RecipeId = recipeId, Value = value };
    }

    public void UpdateValue(int value)
    {
        if (value < RecipeConstraints.RatingMin || value > RecipeConstraints.RatingMax)
            throw new RatingValueOutOfRangeException(value);

        Value = value;
    }
}
