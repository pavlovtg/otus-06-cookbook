using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Recipes.Adapters.Postgresql.Configurations;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql;

internal sealed class RecipeRepository : DbContext, IRecipeRepository, IIngredientRepository
{
    public const string DefaultSchema = "cookbook";

    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();

    public RecipeRepository(DbContextOptions<RecipeRepository> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        modelBuilder.ApplyConfiguration(new RecipeConfiguration());
        modelBuilder.ApplyConfiguration(new IngredientConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeIngredientConfiguration());
    }

    public async IAsyncEnumerable<Recipe> GetRecipesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var recipe in Recipes
            .AsNoTracking()
            .AsAsyncEnumerable()
            .WithCancellation(cancellationToken))
        {
            yield return recipe;
        }
    }

    public async Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        return await Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<RecipeWithIngredientDetails?> GetByIdWithDetailsAsync(
        RecipeId id,
        CancellationToken cancellationToken = default)
    {
        var recipe = await Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (recipe is null)
            return null;

        var ingredientIds = recipe.Ingredients.Select(i => i.IngredientId).ToList();

        var ingredientMap = await Ingredients
            .AsNoTracking()
            .Where(i => ingredientIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, cancellationToken);

        var details = recipe.Ingredients
            .Select(ri => ingredientMap.TryGetValue(ri.IngredientId, out var ing)
                ? new RecipeIngredientDetail(ri.IngredientId, ing.Title, ri.Amount, ing.Unit)
                : new RecipeIngredientDetail(ri.IngredientId, string.Empty, ri.Amount, string.Empty))
            .ToList();

        return new RecipeWithIngredientDetails(recipe, details);
    }

    public async Task CreateAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        await Recipes.AddAsync(recipe, cancellationToken);
    }

    public Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        Recipes.Update(recipe);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        var recipe = await Recipes.FindAsync([id], cancellationToken);
        if (recipe is not null)
            Recipes.Remove(recipe);
    }

    public async Task<RecipeUsageResult> GetRecipesUsingIngredientAsync(
        IngredientId ingredientId,
        CancellationToken cancellationToken = default)
    {
        var query = Recipes
            .AsNoTracking()
            .Where(r => r.Ingredients.Any(i => i.IngredientId == ingredientId));

        var totalCount = await query.CountAsync(cancellationToken);

        var topTitles = await query
            .OrderBy(r => r.Title)
            .Take(10)
            .Select(r => r.Title)
            .ToListAsync(cancellationToken);

        return new RecipeUsageResult(topTitles, totalCount);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
    }

    // ── IIngredientRepository ────────────────────────────────────────────────

    public async Task<PagedResult<Ingredient>> GetIngredientsAsync(
        int page,
        int pageSize,
        string? titleFilter = null,
        IngredientCategory? categoryFilter = null,
        CancellationToken cancellationToken = default)
    {
        var query = Ingredients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(titleFilter))
            query = query.Where(i => i.Title.ToLower().Contains(titleFilter.ToLower()));

        if (categoryFilter.HasValue)
            query = query.Where(i => i.Category == categoryFilter.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Ingredient>(items, total, page, pageSize);
    }

    public async Task<Ingredient?> GetByIdAsync(IngredientId id, CancellationToken cancellationToken = default)
    {
        return await Ingredients.FindAsync([id], cancellationToken);
    }

    public async Task CreateAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
    {
        await Ingredients.AddAsync(ingredient, cancellationToken);
    }

    public Task UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
    {
        Ingredients.Update(ingredient);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(IngredientId id, CancellationToken cancellationToken = default)
    {
        var ingredient = await Ingredients.FindAsync([id], cancellationToken);
        if (ingredient is not null)
            Ingredients.Remove(ingredient);
    }
}
