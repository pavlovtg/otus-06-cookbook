namespace Recipes.Application.Ports;

internal interface IUnitOfWorkRepository
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}
