namespace Recipes.Domain;

internal readonly record struct MealPlanId(Guid Value)
{
    public static MealPlanId New() => new(Guid.NewGuid());

    public static MealPlanId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("MealPlanId cannot be empty.", nameof(value));

        return new MealPlanId(value);
    }

    public override string ToString() => Value.ToString("D");
}
