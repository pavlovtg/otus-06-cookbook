using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Recipes.Adapters.Postgresql.Configurations;
using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql;

internal sealed class RecipeRepository : DbContext, IRecipeRepository
{
    public const string DefaultSchema = "cookbook";

    public DbSet<Recipe> Recipes => Set<Recipe>();

    public RecipeRepository(DbContextOptions<RecipeRepository> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        modelBuilder.ApplyConfiguration(new RecipeConfiguration());
    }

    public async IAsyncEnumerable<Recipe> GetAllAsync(
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
}
