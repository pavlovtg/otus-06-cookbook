using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> entity)
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

        entity.Property(r => r.PhotoId)
            .HasColumnName("photo_id")
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? RecipePhotoId.From(value.Value) : null)
            .IsRequired(false);

        entity.Property(r => r.IsPublic)
            .HasColumnName("is_public")
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(r => r.AuthorId)
            .HasColumnName("author_id")
            .HasConversion(
                id => id.HasValue ? id.Value.Value : (Guid?)null,
                value => value.HasValue ? UserId.From(value.Value) : null)
            .IsRequired(false);
    }
}
