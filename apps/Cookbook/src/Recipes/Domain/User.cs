namespace Recipes.Domain;

internal sealed class User
{
    public UserId Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }

    private User() { }

    public static User Create(
        UserId id,
        string email,
        string displayName,
        string passwordHash,
        UserRole role = UserRole.User)
    {
        return new User
        {
            Id = id,
            Email = email,
            DisplayName = displayName,
            PasswordHash = passwordHash,
            Role = role,
        };
    }
}
