namespace Recipes.Domain.Exceptions;

internal sealed class WeekDayOutOfRangeException : MealPlanDomainException
{
    public int ActualValue { get; }

    public WeekDayOutOfRangeException(int actualValue)
    {
        ActualValue = actualValue;
    }
}
