using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class RecipePhotoConfiguration : IEntityTypeConfiguration<RecipePhoto>
{
    public void Configure(EntityTypeBuilder<RecipePhoto> entity)
    {
        entity.ToTable("recipe_photos");

        entity.HasKey(p => p.Id);

        entity.Property(p => p.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => RecipePhotoId.From(value));

        entity.Property(p => p.RecipeId)
            .HasColumnName("recipe_id")
            .HasConversion(
                id => id.Value,
                value => RecipeId.From(value))
            .IsRequired();

        entity.Property(p => p.OriginalData)
            .HasColumnName("original_data")
            .IsRequired();

        entity.Property(p => p.ThumbnailData)
            .HasColumnName("thumbnail_data")
            .IsRequired();

        entity.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(p => p.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
