using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Application;

internal sealed class MealPlanService
{
    private readonly IMealPlanRepository _repository;

    public MealPlanService(IMealPlanRepository repository)
    {
        _repository = repository;
    }

    public async Task<MealPlanView> GetAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var view = await _repository.GetPlanViewByUserIdAsync(userId, cancellationToken);
        if (view is not null)
            return view;

        var plan = MealPlan.Create(MealPlanId.New(), userId);
        await _repository.SavePlanAsync(plan, cancellationToken);
        await _repository.CommitAsync(cancellationToken);

        return new MealPlanView(plan.Id.Value, []);
    }

    public async Task<MealPlanView> ReplaceAsync(
        UserId userId,
        IReadOnlyList<MealPlanSlotInput> slots,
        CancellationToken cancellationToken = default)
    {
        var plan = await _repository.GetPlanByUserIdAsync(userId, cancellationToken)
                   ?? MealPlan.Create(MealPlanId.New(), userId);

        plan.SetSlots(slots);
        await _repository.SavePlanAsync(plan, cancellationToken);
        await _repository.CommitAsync(cancellationToken);

        var view = await _repository.GetPlanViewByUserIdAsync(userId, cancellationToken);
        return view ?? new MealPlanView(plan.Id.Value, []);
    }

    public async Task ClearAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var plan = await _repository.GetPlanByUserIdAsync(userId, cancellationToken);

        if (plan is null)
            return;

        plan.ClearSlots();
        await _repository.SavePlanAsync(plan, cancellationToken);
        await _repository.CommitAsync(cancellationToken);
    }
}
