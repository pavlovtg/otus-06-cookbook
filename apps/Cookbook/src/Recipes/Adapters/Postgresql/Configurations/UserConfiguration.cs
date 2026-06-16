using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("users");

        entity.HasKey(u => u.Id);

        entity.Property(u => u.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => UserId.From(value));

        entity.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(UserConstraints.EmailMaxLength)
            .IsRequired();

        entity.HasIndex(u => u.Email)
            .IsUnique();

        entity.Property(u => u.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(UserConstraints.DisplayNameMaxLength)
            .IsRequired();

        entity.Property(u => u.PasswordHash)
            .HasColumnName("password_hash")
            .HasMaxLength(UserConstraints.PasswordHashMaxLength)
            .IsRequired();

        entity.Property(u => u.Role)
            .HasColumnName("role")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                s => Enum.Parse<UserRole>(s, ignoreCase: true));
    }
}
