namespace Recipes.Domain;

internal sealed class MealPlan
{
    public MealPlanId Id { get; private set; }
    public UserId UserId { get; private set; }

    private List<MealPlanSlot> _slots = [];
    public IReadOnlyList<MealPlanSlot> Slots => _slots.AsReadOnly();

    private MealPlan() { }

    public static MealPlan Create(MealPlanId id, UserId userId)
    {
        return new MealPlan
        {
            Id = id,
            UserId = userId,
        };
    }

    public void ClearSlots()
    {
        _slots.Clear();
    }

    public void SetSlots(IReadOnlyList<MealPlanSlotInput> slotInputs)
    {
        _slots.Clear();

        foreach (var slotInput in slotInputs)
        {
            var slot = MealPlanSlot.Create(slotInput.WeekDay, slotInput.MealType);
            foreach (var itemInput in slotInput.Items)
                slot.AddItem(MealPlanItem.Create(itemInput.RecipeId, itemInput.Servings));

            _slots.Add(slot);
        }
    }
}
