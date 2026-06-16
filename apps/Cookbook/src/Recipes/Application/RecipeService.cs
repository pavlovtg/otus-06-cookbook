using System.Runtime.CompilerServices;
using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Application;

internal sealed class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository;
    private readonly ICategoryRepository _categoryRepository;

    public RecipeService(IRecipeRepository repository, ICategoryRepository categoryRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
    }

    public async IAsyncEnumerable<Recipe> GetRecipesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var recipe in _repository.GetRecipesAsync(cancellationToken))
            yield return recipe;
    }

    public async Task<Recipe> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);
    }

    public async Task<RecipeWithIngredientDetails> GetByIdWithDetailsAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);
    }

    public async Task<Recipe> CreateAsync(
        string title,
        string? description,
        int cookingTime,
        Difficulty difficulty,
        int servings,
        string instructions,
        IEnumerable<RecipeIngredientInput> ingredients,
        IEnumerable<CategoryId> categoryIds,
        CancellationToken cancellationToken = default)
    {
        var recipeIngredients = ingredients
            .Select(i => RecipeIngredient.Create(i.IngredientId, i.Amount))
            .ToList();

        var categoryTypes = await BuildCategoryTypesAsync(categoryIds, cancellationToken);

        var recipe = Recipe.Create(RecipeId.New(), title, description, cookingTime, difficulty, servings, instructions,
            recipeIngredients, categoryTypes);
        await _repository.CreateAsync(recipe, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
        return recipe;
    }

    public async Task UpdateAsync(
        RecipeId id,
        string title,
        string? description,
        int cookingTime,
        Difficulty difficulty,
        int servings,
        string instructions,
        IEnumerable<RecipeIngredientInput> ingredients,
        IEnumerable<CategoryId> categoryIds,
        CancellationToken cancellationToken = default)
    {
        var recipe = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);

        var recipeIngredients = ingredients
            .Select(i => RecipeIngredient.Create(i.IngredientId, i.Amount))
            .ToList();

        var categoryTypes = await BuildCategoryTypesAsync(categoryIds, cancellationToken);

        recipe.Update(title, description, cookingTime, difficulty, servings, instructions,
            recipeIngredients, categoryTypes);
        await _repository.UpdateAsync(recipe, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        var recipe = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);

        await _repository.DeleteAsync(recipe.Id, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }

    private async Task<IReadOnlyDictionary<CategoryId, CategoryType>> BuildCategoryTypesAsync(
        IEnumerable<CategoryId> categoryIds,
        CancellationToken cancellationToken)
    {
        var ids = categoryIds.ToList();
        if (ids.Count == 0)
            return new Dictionary<CategoryId, CategoryType>();

        var categories = await _categoryRepository.GetByIdsAsync(ids, cancellationToken);

        if (categories.Count != ids.Count)
        {
            var foundIds = categories.Select(c => c.Id).ToHashSet();
            var missingId = ids.First(id => !foundIds.Contains(id));
            throw new CategoryNotFoundException(missingId);
        }

        return categories.ToDictionary(c => c.Id, c => c.Type);
    }
}
