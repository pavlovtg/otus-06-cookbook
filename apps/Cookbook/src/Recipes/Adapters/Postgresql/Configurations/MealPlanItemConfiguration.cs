using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class MealPlanItemConfiguration : IEntityTypeConfiguration<MealPlanItem>
{
    public void Configure(EntityTypeBuilder<MealPlanItem> entity)
    {
        entity.ToTable("meal_plan_items");

        entity.HasKey(i => i.Id);

        entity.Property(i => i.Id)
            .HasColumnName("id");

        entity.Property(i => i.RecipeId)
            .HasColumnName("recipe_id")
            .HasConversion(
                id => id.Value,
                value => RecipeId.From(value))
            .IsRequired();

        entity.Property(i => i.Servings)
            .HasColumnName("servings")
            .HasConversion(
                s => s.Value,
                value => Servings.From(value))
            .IsRequired();

        entity.Property<Guid?>("MealPlanSlotId")
            .HasColumnName("meal_plan_slot_id");
    }
}
