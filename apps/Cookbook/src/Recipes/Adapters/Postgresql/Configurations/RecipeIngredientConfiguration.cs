using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> entity)
    {
        entity.ToTable("recipe_ingredients");

        entity.HasKey("RecipeId", nameof(RecipeIngredient.IngredientId));

        entity.Property(ri => ri.IngredientId)
            .HasColumnName("ingredient_id")
            .HasConversion(
                id => id.Value,
                value => IngredientId.From(value));

        entity.Property(ri => ri.Amount)
            .HasColumnName("amount")
            .HasColumnType("numeric(12,3)")
            .IsRequired();
    }
}
