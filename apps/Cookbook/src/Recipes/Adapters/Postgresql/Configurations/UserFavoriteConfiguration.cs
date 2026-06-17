using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
{
    public void Configure(EntityTypeBuilder<UserFavorite> entity)
    {
        entity.ToTable("user_favorites");

        entity.HasKey(uf => new { uf.UserId, uf.RecipeId });

        entity.Property(uf => uf.UserId)
            .HasColumnName("user_id")
            .HasConversion(
                id => id.Value,
                value => UserId.From(value));

        entity.Property(uf => uf.RecipeId)
            .HasColumnName("recipe_id")
            .HasConversion(
                id => id.Value,
                value => RecipeId.From(value));

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(uf => uf.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
