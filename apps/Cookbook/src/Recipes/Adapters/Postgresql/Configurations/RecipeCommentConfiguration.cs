using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class RecipeCommentConfiguration : IEntityTypeConfiguration<RecipeComment>
{
    public void Configure(EntityTypeBuilder<RecipeComment> entity)
    {
        entity.ToTable("recipe_comments");

        entity.HasKey(c => c.Id);

        entity.Property(c => c.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => RecipeCommentId.From(value));

        entity.Property(c => c.RecipeId)
            .HasColumnName("recipe_id")
            .HasConversion(
                id => id.Value,
                value => RecipeId.From(value));

        entity.Property(c => c.AuthorId)
            .HasColumnName("author_id")
            .HasConversion(
                id => id.Value,
                value => UserId.From(value));

        entity.Property(c => c.Text)
            .HasColumnName("text")
            .HasMaxLength(RecipeCommentConstraints.MaxTextLength)
            .IsRequired();

        entity.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        entity.HasIndex(c => new { c.RecipeId, c.AuthorId })
            .IsUnique();

        entity.HasOne<Recipe>()
            .WithMany()
            .HasForeignKey(c => c.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
