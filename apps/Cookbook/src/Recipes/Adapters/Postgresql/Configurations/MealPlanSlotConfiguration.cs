using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class MealPlanSlotConfiguration : IEntityTypeConfiguration<MealPlanSlot>
{
    public void Configure(EntityTypeBuilder<MealPlanSlot> entity)
    {
        entity.ToTable("meal_plan_slots");

        entity.HasKey(s => s.Id);

        entity.Property(s => s.Id)
            .HasColumnName("id");

        entity.Property(s => s.WeekDay)
            .HasColumnName("week_day")
            .IsRequired();

        entity.Property(s => s.MealType)
            .HasColumnName("meal_type")
            .IsRequired();

        entity.HasOne<MealPlan>()
            .WithMany(p => p.Slots)
            .HasForeignKey("meal_plan_id")
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasIndex("meal_plan_id", nameof(MealPlanSlot.WeekDay), nameof(MealPlanSlot.MealType))
            .IsUnique();

        entity.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey("MealPlanSlotId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
