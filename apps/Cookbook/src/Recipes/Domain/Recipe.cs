using Recipes.Domain.Exceptions;

namespace Recipes.Domain;

internal sealed class Recipe
{
    public RecipeId Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int CookingTime { get; private set; }
    public Difficulty Difficulty { get; private set; }
    public int Servings { get; private set; }
    public string Instructions { get; private set; } = string.Empty;

    private List<RecipeIngredient> _ingredients = [];
    public IReadOnlyList<RecipeIngredient> Ingredients => _ingredients.AsReadOnly();

    private Recipe() { }

    public static Recipe Create(
        RecipeId id,
        string title,
        string? description,
        int cookingTime,
        Difficulty difficulty,
        int servings,
        string instructions,
        IEnumerable<RecipeIngredient>? ingredients = null)
    {
        ValidateTitle(title);
        ValidateDescription(description);
        ValidateCookingTime(cookingTime);
        ValidateServings(servings);
        ValidateInstructions(instructions);

        var ingredientList = ingredients?.ToList() ?? [];
        ValidateIngredientsCount(ingredientList.Count);

        return new Recipe
        {
            Id = id,
            Title = title,
            Description = description ?? string.Empty,
            CookingTime = cookingTime,
            Difficulty = difficulty,
            Servings = servings,
            Instructions = instructions,
            _ingredients = ingredientList,
        };
    }

    public void Update(
        string title,
        string? description,
        int cookingTime,
        Difficulty difficulty,
        int servings,
        string instructions,
        IEnumerable<RecipeIngredient>? ingredients = null)
    {
        ValidateTitle(title);
        ValidateDescription(description);
        ValidateCookingTime(cookingTime);
        ValidateServings(servings);
        ValidateInstructions(instructions);

        var ingredientList = ingredients?.ToList() ?? [];
        ValidateIngredientsCount(ingredientList.Count);

        Title = title;
        Description = description ?? string.Empty;
        CookingTime = cookingTime;
        Difficulty = difficulty;
        Servings = servings;
        Instructions = instructions;

        _ingredients.Clear();
        _ingredients.AddRange(ingredientList);
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new RecipeTitleEmptyException();

        if (title.Length > RecipeConstraints.TitleMaxLength)
            throw new RecipeTitleTooLongException(title.Length);
    }

    private static void ValidateDescription(string? description)
    {
        if (description is not null && description.Length > RecipeConstraints.DescriptionMaxLength)
            throw new RecipeDescriptionTooLongException(description.Length);
    }

    private static void ValidateCookingTime(int cookingTime)
    {
        if (cookingTime < RecipeConstraints.CookingTimeMin || cookingTime > RecipeConstraints.CookingTimeMax)
            throw new RecipeCookingTimeOutOfRangeException(cookingTime);
    }

    private static void ValidateServings(int servings)
    {
        if (servings < RecipeConstraints.ServingsMin || servings > RecipeConstraints.ServingsMax)
            throw new RecipeServingsOutOfRangeException(servings);
    }

    private static void ValidateInstructions(string instructions)
    {
        if (string.IsNullOrWhiteSpace(instructions))
            throw new RecipeInstructionsEmptyException();

        if (instructions.Length > RecipeConstraints.InstructionsMaxLength)
            throw new RecipeInstructionsTooLongException(instructions.Length);
    }

    private static void ValidateIngredientsCount(int count)
    {
        if (count > RecipeConstraints.IngredientsMaxCount)
            throw new RecipeIngredientsTooManyException(count);
    }
}
