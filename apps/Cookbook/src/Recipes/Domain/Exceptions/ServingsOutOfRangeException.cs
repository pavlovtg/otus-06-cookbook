namespace Recipes.Domain.Exceptions;

internal sealed class ServingsOutOfRangeException : MealPlanDomainException
{
    public int MinValue { get; } = MealPlanConstraints.MinServings;
    public int MaxValue { get; } = MealPlanConstraints.MaxServings;
    public int ActualValue { get; }

    public ServingsOutOfRangeException(int actualValue)
    {
        ActualValue = actualValue;
    }
}
