using Recipes.Application.Ports;
using Recipes.Domain;
using Recipes.Domain.Exceptions;

namespace Recipes.Application;

internal sealed class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _repository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;

    public RecipeService(IRecipeRepository repository, ICategoryRepository categoryRepository, IUserRepository userRepository)
    {
        _repository = repository;
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
    }

    public async Task<PagedResult<RecipeShortWithAuthor>> GetRecipesPagedAsync(
        int page,
        int pageSize,
        string? q = null,
        RecipeSortOrder sort = RecipeSortOrder.TitleAsc,
        UserId? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetRecipesPagedAsync(page, pageSize, q, sort, currentUserId, cancellationToken);

        var authorIds = result.Items
            .Where(r => r.AuthorId.HasValue)
            .Select(r => r.AuthorId!.Value)
            .DistinctBy(id => id.Value)
            .ToList();

        var authorNames = authorIds.Count > 0
            ? await _userRepository.GetDisplayNamesByIdsAsync(authorIds, cancellationToken)
            : new Dictionary<UserId, string>();

        var items = result.Items
            .Select(r =>
            {
                string? authorName = r.AuthorId.HasValue && authorNames.TryGetValue(r.AuthorId.Value, out var name)
                    ? name
                    : null;
                return new RecipeShortWithAuthor(r, authorName);
            })
            .ToList();

        return new PagedResult<RecipeShortWithAuthor>(items, result.Total, result.Page, result.PageSize);
    }

    public async Task<Recipe> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);
    }

    public async Task<RecipeWithIngredientDetails> GetByIdWithDetailsAsync(
        RecipeId id,
        UserId? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var details = await _repository.GetByIdWithDetailsAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);

        if (!details.Recipe.IsPublic && details.Recipe.AuthorId != currentUserId)
            throw new RecipeForbiddenException();

        return details;
    }

    public async Task<Recipe> CreateAsync(
        string title,
        string? description,
        int cookingTime,
        Difficulty difficulty,
        int servings,
        string instructions,
        bool isPublic,
        UserId? authorId,
        IEnumerable<RecipeIngredientInput> ingredients,
        IEnumerable<CategoryId> categoryIds,
        CancellationToken cancellationToken = default)
    {
        if (authorId is null)
            throw new RecipeUnauthorizedException();

        var recipeIngredients = ingredients
            .Select(i => RecipeIngredient.Create(i.IngredientId, i.Amount))
            .ToList();

        var categoryTypes = await BuildCategoryTypesAsync(categoryIds, cancellationToken);

        var recipe = Recipe.Create(RecipeId.New(), title, description, cookingTime, difficulty, servings, instructions,
            recipeIngredients, categoryTypes, isPublic, authorId);
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
        bool isPublic,
        UserId? currentUserId,
        UserRole? currentUserRole,
        IEnumerable<RecipeIngredientInput> ingredients,
        IEnumerable<CategoryId> categoryIds,
        CancellationToken cancellationToken = default)
    {
        var recipe = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);

        if (!CanModifyRecipe(recipe, currentUserId, currentUserRole))
            throw new RecipeForbiddenException();

        var recipeIngredients = ingredients
            .Select(i => RecipeIngredient.Create(i.IngredientId, i.Amount))
            .ToList();

        var categoryTypes = await BuildCategoryTypesAsync(categoryIds, cancellationToken);

        recipe.Update(title, description, cookingTime, difficulty, servings, instructions,
            isPublic, recipeIngredients, categoryTypes);
        await _repository.UpdateAsync(recipe, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }

    public async Task DeleteAsync(RecipeId id, UserId? currentUserId, UserRole? currentUserRole, CancellationToken cancellationToken = default)
    {
        var recipe = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new RecipeNotFoundException(id);

        if (!CanModifyRecipe(recipe, currentUserId, currentUserRole))
            throw new RecipeForbiddenException();

        await _repository.DeleteAsync(recipe.Id, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }

    private static bool CanModifyRecipe(Recipe recipe, UserId? currentUserId, UserRole? currentUserRole)
    {
        if (currentUserRole == UserRole.Admin)
            return true;

        return recipe.AuthorId is not null && recipe.AuthorId == currentUserId;
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
