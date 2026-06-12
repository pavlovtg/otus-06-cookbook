using System.Text.Json.Serialization;

namespace Shared.Database.ConnectionStrings;

public class DatabaseConnectionOptions
{
    private const string HostKey = "Host";
    private const string PortKey = "Port";
    private const string DatabaseKey = "Database";
    private const string UsernameKey = "Username";
    private const string PasswordKey = "Password";

    [JsonIgnore]
    public string? Host
    {
        get => Extensions.TryGetValue(HostKey, out var value) ? (string?)value : null;
        set => Extensions[HostKey] = value;
    }

    [JsonIgnore]
    public int? Port
    {
        //todo: Тестовый хост серелизует порт как string
        get => Extensions.TryGetValue(PortKey, out var value) ? (value is string integerString ? int.Parse(integerString) : (int?)value) : null;
        set => Extensions[PortKey] = value;
    }

    [JsonIgnore]
    public string? Database
    {
        get => Extensions.TryGetValue(DatabaseKey, out var value) ? (string?)value : null;
        set => Extensions[DatabaseKey] = value;
    }

    [JsonIgnore]
    public string? Username
    {
        get => Extensions.TryGetValue(UsernameKey, out var value) ? (string?)value : null;
        set => Extensions[UsernameKey] = value;
    }

    [JsonIgnore]
    public string? Password
    {
        get => Extensions.TryGetValue(PasswordKey, out var value) ? (string?)value : null;
        set => Extensions[PasswordKey] = value;
    }

    [JsonExtensionData]
    public Dictionary<string, object?> Extensions { get; } = new();
}
