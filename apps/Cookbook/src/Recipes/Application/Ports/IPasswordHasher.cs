namespace Recipes.Application.Ports;

internal interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
