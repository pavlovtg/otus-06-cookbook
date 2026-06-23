namespace Recipes.Domain;

internal sealed class MealPlanSlot
{
    public Guid Id { get; private set; }
    public WeekDay WeekDay { get; private set; }
    public MealType MealType { get; private set; }

    private List<MealPlanItem> _items = [];
    public IReadOnlyList<MealPlanItem> Items => _items.AsReadOnly();

    private MealPlanSlot() { }

    public static MealPlanSlot Create(WeekDay weekDay, MealType mealType)
    {
        return new MealPlanSlot
        {
            Id = Guid.NewGuid(),
            WeekDay = weekDay,
            MealType = mealType,
        };
    }

    internal void AddItem(MealPlanItem item)
    {
        _items.Add(item);
    }

    internal bool RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
            return false;

        _items.Remove(item);
        return true;
    }

    internal bool UpdateServings(Guid itemId, Servings servings)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
            return false;

        item.UpdateServings(servings);
        return true;
    }

    internal void Clear()
    {
        _items.Clear();
    }

    internal void ReplaceItems(IEnumerable<MealPlanItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
    }
}
