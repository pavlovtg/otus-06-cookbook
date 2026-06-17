namespace Recipes.Domain.Exceptions;

internal sealed class UserNotFoundException : UserDomainException
{
    public string Email { get; }

    public UserNotFoundException(string email)
    {
        Email = email;
    }
}
