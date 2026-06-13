using Npgsql;

namespace Shared.Database.ConnectionStrings;

public static class DatabaseConnectionOptionsExtensions
{
    private static readonly HashSet<string> _stopList = new() { "ENLIST" };
    private static readonly Dictionary<string, object?> _defaultValues = new()
    {
        { "Enlist", true },
        { "KeepAlive", 30 }    // Some net-services between service and DB close the connection by their own timeout, ignoring the tcp keep-alive packet.
    };

    public static string ToConnectionString(this DatabaseConnectionOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(options.Host))
        {
            throw new ArgumentException("Invalid database Host.", nameof(options.Host));
        }

        if (options.Port is < 0 or > 65535)
        {
            throw new ArgumentException("Invalid database Port.", nameof(options.Port));
        }

        var builder = new NpgsqlConnectionStringBuilder();

        foreach (var pair in _defaultValues)
        {
            builder.Add(pair);
        }

        foreach (var pair in options.Extensions)
        {
            if (_stopList.Contains(pair.Key.ToUpperInvariant()))
            {
                continue;
            }

            builder.Add(pair);
        }

        return builder.ConnectionString;
    }

    public static DatabaseConnectionOptions FromConnectionString(this string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Invalid connection string", nameof(connectionString));
        }

        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        var options = new DatabaseConnectionOptions();

        foreach (var property in builder)
        {
            options.Extensions[property.Key] = property.Value;
        }

        return options;
    }
}
