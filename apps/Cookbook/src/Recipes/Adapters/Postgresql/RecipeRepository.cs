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
        return await Recipes.FindAsync([id], cancellationToken);
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
