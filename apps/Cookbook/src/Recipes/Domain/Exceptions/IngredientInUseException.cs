namespace Recipes.Domain.Exceptions;

internal sealed class IngredientInUseException : IngredientDomainException
{
    public IReadOnlyList<string> TopRecipeTitles { get; }
    public int TotalCount { get; }

    public IngredientInUseException(IReadOnlyList<string> topRecipeTitles, int totalCount)
    {
        TopRecipeTitles = topRecipeTitles;
        TotalCount = totalCount;
    }
}
