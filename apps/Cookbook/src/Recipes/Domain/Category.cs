using Recipes.Domain.Exceptions;

namespace Recipes.Domain;

internal sealed class Category
{
    public CategoryId Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public CategoryType Type { get; private set; }

    private Category() { }

    public static Category Create(
        CategoryId id,
        string name,
        string description,
        CategoryType type)
    {
        ValidateName(name);
        ValidateDescription(description);

        return new Category
        {
            Id = id,
            Name = name,
            Description = description,
            Type = type,
        };
    }

    public void Update(string name, string description)
    {
        ValidateName(name);
        ValidateDescription(description);

        Name = name;
        Description = description;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < CategoryConstraints.NameMinLength)
            throw new CategoryNameTooShortException(name?.Length ?? 0);

        if (name.Length > CategoryConstraints.NameMaxLength)
            throw new CategoryNameTooLongException(name.Length);
    }

    private static void ValidateDescription(string description)
    {
        if (description is not null && description.Length > CategoryConstraints.DescriptionMaxLength)
            throw new CategoryDescriptionTooLongException(description.Length);
    }
}
