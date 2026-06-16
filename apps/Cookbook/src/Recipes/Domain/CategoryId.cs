namespace Recipes.Domain;

internal readonly record struct CategoryId(Guid Value)
{
    public static CategoryId New() => new(Guid.NewGuid());

    public static CategoryId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("CategoryId cannot be empty.", nameof(value));

        return new CategoryId(value);
    }

    public override string ToString() => Value.ToString("D");
}
