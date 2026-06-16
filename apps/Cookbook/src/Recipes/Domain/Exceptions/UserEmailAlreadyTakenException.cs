namespace Recipes.Domain.Exceptions;

internal sealed class UserEmailAlreadyTakenException : UserDomainException
{
    public string Email { get; }

    public UserEmailAlreadyTakenException(string email)
    {
        Email = email;
    }
}
