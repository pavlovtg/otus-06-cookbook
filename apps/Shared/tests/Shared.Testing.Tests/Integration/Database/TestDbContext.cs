using Microsoft.EntityFrameworkCore;

namespace Shared.Testing.Tests.Integration.Database;

internal sealed class TestDbContext : DbContext
{
    public DbSet<TestEntity> Entities => Set<TestEntity>();

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestEntity>(entity =>
        {
            entity.ToTable("test_entities");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
        });
    }
}

internal sealed class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
