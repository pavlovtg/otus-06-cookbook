using Recipes.Domain;

namespace Recipes.Domain.Exceptions;

internal sealed class RecipeInstructionsTooLongException : RecipeDomainException
{
    public int MaxLength { get; } = RecipeConstraints.InstructionsMaxLength;
    public int ActualLength { get; }

    public RecipeInstructionsTooLongException(int actualLength)
    {
        ActualLength = actualLength;
    }
}
