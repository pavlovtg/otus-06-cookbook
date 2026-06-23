using Recipes.Domain;
using Recipes.Domain.Exceptions;
using Xunit;

namespace Recipes.Tests.Domain;

public class MealPlanTests
{
    private static UserId NewUserId() => UserId.From(Guid.NewGuid());
    private static RecipeId NewRecipeId() => RecipeId.From(Guid.NewGuid());

    // ── Create ───────────────────────────────────────────────────────────────

    [Fact]
    public void Create_ReturnsEmptySlots()
    {
        var plan = MealPlan.Create(MealPlanId.New(), NewUserId());

        Assert.Empty(plan.Slots);
    }

    [Fact]
    public void Create_SetsIdAndUserId()
    {
        var id = MealPlanId.New();
        var userId = NewUserId();

        var plan = MealPlan.Create(id, userId);

        Assert.Equal(id, plan.Id);
        Assert.Equal(userId, plan.UserId);
    }

    // ── SetSlots ─────────────────────────────────────────────────────────────

    [Fact]
    public void SetSlots_WithValidInput_SlotsAreSet()
    {
        var plan = MealPlan.Create(MealPlanId.New(), NewUserId());
        var slots = new List<MealPlanSlotInput>
        {
            new(WeekDay.Monday, MealType.Breakfast,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(2))])
        };

        plan.SetSlots(slots);

        Assert.Single(plan.Slots);
        Assert.Equal(WeekDay.Monday, plan.Slots[0].WeekDay);
        Assert.Equal(MealType.Breakfast, plan.Slots[0].MealType);
        Assert.Single(plan.Slots[0].Items);
    }

    [Fact]
    public void SetSlots_ReplacesExistingSlots()
    {
        var plan = MealPlan.Create(MealPlanId.New(), NewUserId());
        plan.SetSlots([new(WeekDay.Monday, MealType.Breakfast,
            [new MealPlanItemInput(NewRecipeId(), Servings.From(1))])]);

        plan.SetSlots([
            new(WeekDay.Tuesday, MealType.Lunch,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(3))]),
            new(WeekDay.Wednesday, MealType.Dinner,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(2))])
        ]);

        Assert.Equal(2, plan.Slots.Count);
        Assert.Equal(WeekDay.Tuesday, plan.Slots[0].WeekDay);
        Assert.Equal(WeekDay.Wednesday, plan.Slots[1].WeekDay);
    }

    [Fact]
    public void SetSlots_WithMultipleItems_AllItemsPresent()
    {
        var plan = MealPlan.Create(MealPlanId.New(), NewUserId());
        var slots = new List<MealPlanSlotInput>
        {
            new(WeekDay.Friday, MealType.Dinner,
            [
                new MealPlanItemInput(NewRecipeId(), Servings.From(1)),
                new MealPlanItemInput(NewRecipeId(), Servings.From(4)),
                new MealPlanItemInput(NewRecipeId(), Servings.From(2)),
            ])
        };

        plan.SetSlots(slots);

        Assert.Equal(3, plan.Slots[0].Items.Count);
    }

    [Fact]
    public void SetSlots_WithEmptyList_SlotsAreEmpty()
    {
        var plan = MealPlan.Create(MealPlanId.New(), NewUserId());
        plan.SetSlots([new(WeekDay.Monday, MealType.Breakfast,
            [new MealPlanItemInput(NewRecipeId(), Servings.From(1))])]);

        plan.SetSlots([]);

        Assert.Empty(plan.Slots);
    }

    // ── ClearSlots ───────────────────────────────────────────────────────────

    [Fact]
    public void ClearSlots_RemovesAllSlots()
    {
        var plan = MealPlan.Create(MealPlanId.New(), NewUserId());
        plan.SetSlots([
            new(WeekDay.Monday, MealType.Breakfast,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(2))]),
            new(WeekDay.Tuesday, MealType.Lunch,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(3))])
        ]);

        plan.ClearSlots();

        Assert.Empty(plan.Slots);
    }

    [Fact]
    public void ClearSlots_OnEmptyPlan_DoesNotThrow()
    {
        var plan = MealPlan.Create(MealPlanId.New(), NewUserId());

        var ex = Record.Exception(() => plan.ClearSlots());

        Assert.Null(ex);
    }

    // ── Servings ─────────────────────────────────────────────────────────────

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(99)]
    public void Servings_From_ValidValues_Succeeds(int value)
    {
        var servings = Servings.From(value);

        Assert.Equal(value, servings.Value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100)]
    [InlineData(200)]
    public void Servings_From_OutOfRange_ThrowsServingsOutOfRangeException(int value)
    {
        var ex = Assert.Throws<ServingsOutOfRangeException>(() => Servings.From(value));

        Assert.Equal(value, ex.ActualValue);
        Assert.Equal(MealPlanConstraints.MinServings, ex.MinValue);
        Assert.Equal(MealPlanConstraints.MaxServings, ex.MaxValue);
    }

    [Fact]
    public void SetSlots_ItemServings_ArePreserved()
    {
        var plan = MealPlan.Create(MealPlanId.New(), NewUserId());
        var slots = new List<MealPlanSlotInput>
        {
            new(WeekDay.Sunday, MealType.Dinner,
                [new MealPlanItemInput(NewRecipeId(), Servings.From(7))])
        };

        plan.SetSlots(slots);

        Assert.Equal(7, plan.Slots[0].Items[0].Servings.Value);
    }
}
