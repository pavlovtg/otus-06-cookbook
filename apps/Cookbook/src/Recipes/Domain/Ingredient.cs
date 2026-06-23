using Recipes.Domain.Exceptions;

namespace Recipes.Domain;

internal sealed class Ingredient
{
    public IngredientId Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public float DefaultAmount { get; private set; }
    public IngredientCategory Category { get; private set; }
    public bool IsSystem { get; private set; }

    private Ingredient() { }

    public static Ingredient Create(
        IngredientId id,
        string title,
        string unit,
        float defaultAmount,
        IngredientCategory category,
        bool isSystem = false)
    {
        ValidateTitle(title);
        ValidateUnit(unit);
        ValidateDefaultAmount(defaultAmount);

        return new Ingredient
        {
            Id = id,
            Title = title,
            Unit = unit,
            DefaultAmount = defaultAmount,
            Category = category,
            IsSystem = isSystem,
        };
    }

    public void Update(
        string title,
        string unit,
        float defaultAmount,
        IngredientCategory category,
        bool? isSystem = null)
    {
        ValidateTitle(title);
        ValidateUnit(unit);
        ValidateDefaultAmount(defaultAmount);

        Title = title;
        Unit = unit;
        DefaultAmount = defaultAmount;
        Category = category;

        if (isSystem.HasValue)
            IsSystem = isSystem.Value;
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Length < IngredientConstraints.TitleMinLength)
            throw new IngredientTitleTooShortException(title?.Length ?? 0);

        if (title.Length > IngredientConstraints.TitleMaxLength)
            throw new IngredientTitleTooLongException(title.Length);
    }

    private static void ValidateUnit(string unit)
    {
        if (unit is not null && unit.Length > IngredientConstraints.UnitMaxLength)
            throw new IngredientUnitTooLongException(unit.Length);
    }

    private static void ValidateDefaultAmount(float defaultAmount)
    {
        if (defaultAmount < IngredientConstraints.DefaultAmountMin || defaultAmount > IngredientConstraints.DefaultAmountMax)
            throw new IngredientDefaultAmountOutOfRangeException(defaultAmount);
    }
}
