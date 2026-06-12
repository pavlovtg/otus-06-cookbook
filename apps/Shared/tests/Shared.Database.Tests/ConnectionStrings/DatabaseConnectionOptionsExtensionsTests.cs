using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Shared.Database.ConnectionStrings;
using Xunit;

namespace Shared.Database.Tests.ConnectionStrings;

public class DatabaseConnectionOptionsExtensionsTests
{
    [Fact]
    public void ToConnectionString_CreateTypicalString()
    {
        // Arrange
        var options = new DatabaseConnectionOptions<DbContext>
        {
            Host = "postgre.sql",
            Database = "my_database",
            Port = 666,
            Username = "account",
            Password = "P@ssw0rd"
        };

        // Act
        var connectionString = options.ToConnectionString();

        // Assert
        Assert.Contains($"Host={options.Host}", connectionString);
        Assert.Contains($"Database={options.Database}", connectionString);
        Assert.Contains($"Port={options.Port}", connectionString);
        Assert.Contains($"Username={options.Username}", connectionString);
        Assert.Contains($"Password={options.Password}", connectionString);
    }

    [Fact]
    public void ToConnectionString_CheckDefaults()
    {
        // Arrange
        var options = new DatabaseConnectionOptions<DbContext> { Host = "postgre.sql" };

        // Act
        var connectionString = options.ToConnectionString().ToLowerInvariant();

        // Assert
        const string Name = "_defaultValues";
        var type = typeof(DatabaseConnectionOptionsExtensions);
        var info = type.GetField(Name, BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(info);

        var value = info!.GetValue(null);
        Assert.NotNull(value);

        var defaults = value as Dictionary<string, object?>;
        Assert.NotNull(defaults);

        foreach (var pair in defaults!)
        {
            Assert.Contains($"{pair.Key}={pair.Value}".ToLowerInvariant(), connectionString);
        }
    }

    [Fact]
    public void ToConnectionString_Extensions()
    {
        // Arrange
        var options = new DatabaseConnectionOptions<DbContext> { Host = "postgre.sql", Extensions = { { "Pooling", true } } };

        // Act
        var connectionString = options.ToConnectionString();

        // Assert
        Assert.Contains("Pooling=True", connectionString);
    }

    [Fact]
    public void ToConnectionString_Escaping()
    {
        // Arrange
        var options = new DatabaseConnectionOptions<DbContext> { Host = "postgre.sql", Password = "Password with spaces" };

        // Act
        var connectionString = options.ToConnectionString();

        // Assert
        Assert.Contains($"Password=\"{options.Password}\"", connectionString);
    }

    [Fact]
    public void ToConnectionString_ValidateHost()
    {
        // Arrange
        var options = new DatabaseConnectionOptions<DbContext>();

        // Act
        var ex = Assert.Throws<ArgumentException>(() => options.ToConnectionString());

        // Assert
        Assert.Equal(nameof(options.Host), ex.ParamName);
        Assert.Contains("Invalid database Host.", ex.Message);
    }

    [Fact]
    public void ToConnectionString_ValidatePort()
    {
        // Arrange
        var options = new DatabaseConnectionOptions<DbContext> { Host = "postgre.sql", Port = 1234567890 };

        // Act
        var ex = Assert.Throws<ArgumentException>(() => options.ToConnectionString());

        // Assert
        Assert.Equal(nameof(options.Port), ex.ParamName);
        Assert.Contains("Invalid database Port.", ex.Message);
    }
}
