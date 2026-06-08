using Microsoft.EntityFrameworkCore;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql;

internal sealed class RecipesDbContext : DbContext
{
    public DbSet<Recipe> Recipes => Set<Recipe>();

    public RecipesDbContext(DbContextOptions<RecipesDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("cookbook");

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
                .HasMaxLength(2000)
                .IsRequired();

            entity.Property(r => r.Description)
                .HasColumnName("description")
                .IsRequired();

            entity.HasData(SeedData.Recipes);
        });
    }
}
