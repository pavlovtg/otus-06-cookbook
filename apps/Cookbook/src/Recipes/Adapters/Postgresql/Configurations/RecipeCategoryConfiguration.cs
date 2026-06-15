using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class RecipeCategoryConfiguration : IEntityTypeConfiguration<RecipeCategory>
{
    public void Configure(EntityTypeBuilder<RecipeCategory> entity)
    {
        entity.ToTable("recipe_categories");

        entity.HasKey("RecipeId", nameof(RecipeCategory.CategoryId));

        entity.Property(rc => rc.RecipeId)
            .HasColumnName("recipe_id")
            .HasConversion(
                id => id.Value,
                value => RecipeId.From(value));

        entity.Property(rc => rc.CategoryId)
            .HasColumnName("category_id")
            .HasConversion(
                id => id.Value,
                value => CategoryId.From(value));
    }
}
