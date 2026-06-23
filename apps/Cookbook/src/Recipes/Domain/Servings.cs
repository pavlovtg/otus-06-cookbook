using Recipes.Domain.Exceptions;

namespace Recipes.Domain;

internal readonly record struct Servings(int Value)
{
    public static Servings From(int value)
    {
        if (value < MealPlanConstraints.MinServings || value > MealPlanConstraints.MaxServings)
            throw new ServingsOutOfRangeException(value);

        return new Servings(value);
    }

    public override string ToString() => Value.ToString();
}
