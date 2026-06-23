using Recipes.Domain;

namespace Recipes.Application.Ports;

internal interface IMealPlanRepository : IUnitOfWorkRepository
{
    Task<MealPlan?> GetPlanByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<MealPlanView?> GetPlanViewByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
    Task SavePlanAsync(MealPlan mealPlan, CancellationToken cancellationToken = default);
}
