using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace Shared.Testing.Database;

/// <summary>
/// Разделяемый PostgreSQL-контейнер для xUnit ICollectionFixture.
/// Создаётся один раз на коллекцию тестов — устраняет накладные расходы
/// на docker run/stop при каждом тестовом классе.
/// </summary>
public sealed class PostgresContainerFixture
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithOutputConsumer(Consume.DoNotConsumeStdoutAndStderr())
        .Build();

    public string ConnectionString => _postgres.GetConnectionString();

    public Task InitializeAsync() => _postgres.StartAsync();

    public Task DisposeAsync() => _postgres.DisposeAsync().AsTask();
}
