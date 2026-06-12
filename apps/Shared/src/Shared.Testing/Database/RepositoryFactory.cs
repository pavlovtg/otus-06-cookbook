using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Shared.Testing.Database;

public sealed class RepositoryFactory<TRepository> : IAsyncDisposable
    where TRepository : DbContext
{
    private readonly string _connectionString;
    private readonly Action<NpgsqlDbContextOptionsBuilder>? _npgsqlOptionsAction;
    private readonly Func<DbContextOptionsBuilder<TRepository>, TRepository> _repositoryFactory;
    private readonly Stack<TRepository> _repositories = new();

    public RepositoryFactory(
        string connectionString,
        Func<DbContextOptionsBuilder<TRepository>, TRepository> repositoryFactory,
        Action<NpgsqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
    {
        _connectionString = connectionString;
        _repositoryFactory = repositoryFactory;
        _npgsqlOptionsAction = npgsqlOptionsAction;
    }

    public TRepository Create()
    {
        var repository = CreateRepository();
        _repositories.Push(repository);
        return repository;
    }

    public async Task MigrateAsync()
    {
        await using var context = CreateRepository();
        await context.Database.MigrateAsync();
    }

    public async ValueTask DisposeAsync()
    {
        while (_repositories.TryPop(out var repository))
            await repository.DisposeAsync();
    }

    private TRepository CreateRepository()
    {
        var optionsBuilder = new DbContextOptionsBuilder<TRepository>();
        optionsBuilder.UseNpgsql(_connectionString, _npgsqlOptionsAction);
        return _repositoryFactory(optionsBuilder);
    }
}
