using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IUserRepository : IUnitOfWorkRepository
{
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task CreateAsync(User user, CancellationToken cancellationToken = default);
}
