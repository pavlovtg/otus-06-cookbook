using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
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

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.ToTable("recipes");

            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id)
                .HasColumnName("id")
                .HasConversion(
                    id => id.Value,
                    value => RecipeId.From(value));

            entity.Property(r => r.Title)
                .HasColumnName("title")
                .HasMaxLength(RecipeConstraints.TitleMaxLength)
                .IsRequired();

            entity.Property(r => r.Description)
                .HasColumnName("description")
                .HasMaxLength(RecipeConstraints.DescriptionMaxLength)
                .IsRequired();

            entity.Property(r => r.CookingTime)
                .HasColumnName("cooking_time")
                .IsRequired();

            entity.Property(r => r.Difficulty)
                .HasColumnName("difficulty")
                .HasMaxLength(RecipeConstraints.DifficultyMaxLength)
                .IsRequired()
                .HasConversion(
                    d => d.ToString().ToLowerInvariant(),
                    s => Enum.Parse<Difficulty>(s, ignoreCase: true));

            entity.Property(r => r.Servings)
                .HasColumnName("servings")
                .IsRequired();

            entity.Property(r => r.Instructions)
                .HasColumnName("instructions")
                .HasMaxLength(RecipeConstraints.InstructionsMaxLength)
                .IsRequired();

            entity.HasData(SeedData.Recipes);
        });
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
