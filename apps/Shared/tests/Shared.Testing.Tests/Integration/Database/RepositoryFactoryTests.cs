using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Shared.Testing.Database;
using Testcontainers.PostgreSql;
using Xunit;

namespace Shared.Testing.Tests.Integration.Database;

public sealed class RepositoryFactoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr())
        .Build();

    public Task InitializeAsync() => _postgres.StartAsync();

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();

    private RepositoryFactory<TestDbContext> CreateFactory() =>
        new(_postgres.GetConnectionString(), builder => new TestDbContext(builder.Options));

    // ── Create ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_ReturnsNewInstance()
    {
        await using var factory = CreateFactory();

        var ctx = factory.Create();

        Assert.NotNull(ctx);
    }

    [Fact]
    public async Task Create_CalledTwice_ReturnsDifferentInstances()
    {
        await using var factory = CreateFactory();

        var ctx1 = factory.Create();
        var ctx2 = factory.Create();

        Assert.NotSame(ctx1, ctx2);
    }

    // ── EnsureCreated / Write-Read ────────────────────────────────────────────

    [Fact]
    public async Task EnsureCreated_WriteInOneContext_ReadInAnother_DataIsPersisted()
    {
        await using var factory = CreateFactory();

        await using (var ctx = factory.Create())
        {
            await ctx.Database.EnsureCreatedAsync();
        }

        var entity = new TestEntity { Name = "Тест" };

        await using (var writeCtx = factory.Create())
        {
            writeCtx.Entities.Add(entity);
            await writeCtx.SaveChangesAsync();
        }

        await using var readCtx = factory.Create();
        var result = await readCtx.Entities.FirstOrDefaultAsync(e => e.Id == entity.Id);

        Assert.NotNull(result);
        Assert.Equal("Тест", result.Name);
    }

    // ── DisposeAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task DisposeAsync_DisposesAllCreatedInstances()
    {
        var factory = CreateFactory();

        var ctx1 = factory.Create();
        var ctx2 = factory.Create();

        await factory.DisposeAsync();

        // После dispose попытка использовать контекст должна бросить исключение
        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
            ctx1.Database.CanConnectAsync());

        await Assert.ThrowsAsync<ObjectDisposedException>(() =>
            ctx2.Database.CanConnectAsync());
    }
}
