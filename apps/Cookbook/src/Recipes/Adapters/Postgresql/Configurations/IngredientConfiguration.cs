using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> entity)
    {
        entity.ToTable("ingredients");

        entity.HasKey(i => i.Id);

        entity.Property(i => i.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => IngredientId.From(value));

        entity.Property(i => i.Title)
            .HasColumnName("title")
            .HasMaxLength(IngredientConstraints.TitleMaxLength)
            .IsRequired();

        entity.Property(i => i.Unit)
            .HasColumnName("unit")
            .HasMaxLength(IngredientConstraints.UnitMaxLength)
            .IsRequired();

        entity.Property(i => i.DefaultAmount)
            .HasColumnName("default_amount")
            .IsRequired();

        entity.Property(i => i.Category)
            .HasColumnName("category")
            .HasMaxLength(50)
            .IsRequired()
            .HasConversion(
                c => c.ToString().ToLowerInvariant(),
                s => Enum.Parse<IngredientCategory>(s, ignoreCase: true));

        entity.Property(i => i.IsSystem)
            .HasColumnName("is_system")
            .IsRequired();
    }
}
