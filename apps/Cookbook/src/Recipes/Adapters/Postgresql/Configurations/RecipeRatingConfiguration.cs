using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class RecipeRatingConfiguration : IEntityTypeConfiguration<RecipeRating>
{
    public void Configure(EntityTypeBuilder<RecipeRating> entity)
    {
        entity.ToTable("recipe_ratings");

        entity.HasKey(r => new { r.UserId, r.RecipeId });

        entity.Property(r => r.UserId)
            .HasColumnName("user_id")
            .HasConversion(
                id => id.Value,
                value => UserId.From(value));

        entity.Property(r => r.RecipeId)
            .HasColumnName("recipe_id")
            .HasConversion(
                id => id.Value,
                value => RecipeId.From(value));

        entity.Property(r => r.Value)
            .HasColumnName("value")
            .IsRequired();

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne<Recipe>()
            .WithMany(r => r.Ratings)
            .HasForeignKey(r => r.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
