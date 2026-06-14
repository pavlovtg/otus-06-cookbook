using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> entity)
    {
        entity.ToTable("categories");

        entity.HasKey(c => c.Id);

        entity.Property(c => c.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => CategoryId.From(value));

        entity.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(CategoryConstraints.NameMaxLength)
            .IsRequired();

        entity.Property(c => c.Description)
            .HasColumnName("description")
            .HasMaxLength(CategoryConstraints.DescriptionMaxLength)
            .IsRequired();

        entity.Property(c => c.Type)
            .HasColumnName("type")
            .HasMaxLength(50)
            .IsRequired()
            .HasConversion(
                t => t.ToString().ToLowerInvariant(),
                s => Enum.Parse<CategoryType>(s, ignoreCase: true));
    }
}
