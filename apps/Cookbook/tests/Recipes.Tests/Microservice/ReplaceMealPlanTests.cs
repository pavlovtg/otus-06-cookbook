using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class ReplaceMealPlanTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;

    public Task InitializeAsync() => fixture.TruncateAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<HttpResponseMessage> PutAsync(MealPlanRequest request, System.Net.Http.Headers.AuthenticationHeaderValue authHeader)
    {
        using var req = new HttpRequestMessage(HttpMethod.Put, "/api/v1/meal-plan");
        req.Headers.Authorization = authHeader;
        req.Content = JsonContent.Create(request);
        return await _client.SendAsync(req);
    }

    [Fact]
    public async Task Put_WithoutAuth_Returns401()
    {
        var request = new MealPlanRequest([]);
        var response = await _client.PutAsJsonAsync("/api/v1/meal-plan", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Put_WithEmptySlots_Returns200WithEmptyPlan()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        var request = new MealPlanRequest([]);

        var response = await PutAsync(request, authHeader);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<MealPlanDto>();
        Assert.NotNull(dto);
        Assert.Empty(dto.Slots);
    }

    [Fact]
    public async Task Put_WithValidSlots_Returns200WithPlan()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        var recipeId = Guid.NewGuid();
        var request = new MealPlanRequest([
            new MealPlanSlotRequest(1, 1, [new MealPlanItemRequest(recipeId, 3)])
        ]);

        var response = await PutAsync(request, authHeader);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<MealPlanDto>();
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Single(dto.Slots);
        Assert.Equal(1, dto.Slots[0].WeekDay);
        Assert.Equal(1, dto.Slots[0].MealType);
        Assert.Single(dto.Slots[0].Items);
        Assert.Equal(3, dto.Slots[0].Items[0].Servings);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    [InlineData(-1)]
    public async Task Put_WithInvalidServings_Returns400(int servings)
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        var request = new MealPlanRequest([
            new MealPlanSlotRequest(1, 1, [new MealPlanItemRequest(Guid.NewGuid(), servings)])
        ]);

        var response = await PutAsync(request, authHeader);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_WithMultipleSlots_Returns200WithAllSlots()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        var request = new MealPlanRequest([
            new MealPlanSlotRequest(1, 1, [new MealPlanItemRequest(Guid.NewGuid(), 2)]),
            new MealPlanSlotRequest(1, 2, [new MealPlanItemRequest(Guid.NewGuid(), 1)]),
            new MealPlanSlotRequest(2, 3, [new MealPlanItemRequest(Guid.NewGuid(), 4)])
        ]);

        var response = await PutAsync(request, authHeader);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<MealPlanDto>();
        Assert.NotNull(dto);
        Assert.Equal(3, dto.Slots.Count);
    }

    [Fact]
    public async Task Put_TwiceSameUser_ReplacesSlots()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();

        var firstRequest = new MealPlanRequest([
            new MealPlanSlotRequest(1, 1, [new MealPlanItemRequest(Guid.NewGuid(), 2)]),
            new MealPlanSlotRequest(2, 2, [new MealPlanItemRequest(Guid.NewGuid(), 1)])
        ]);
        await PutAsync(firstRequest, authHeader);

        var secondRequest = new MealPlanRequest([
            new MealPlanSlotRequest(7, 3, [new MealPlanItemRequest(Guid.NewGuid(), 5)])
        ]);
        var response = await PutAsync(secondRequest, authHeader);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<MealPlanDto>();
        Assert.NotNull(dto);
        Assert.Single(dto.Slots);
        Assert.Equal(7, dto.Slots[0].WeekDay);
        Assert.Equal(3, dto.Slots[0].MealType);
    }

    [Fact]
    public async Task Put_WithMultipleItemsInSlot_AllItemsReturned()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        var request = new MealPlanRequest([
            new MealPlanSlotRequest(3, 2,
            [
                new MealPlanItemRequest(Guid.NewGuid(), 1),
                new MealPlanItemRequest(Guid.NewGuid(), 2),
            ])
        ]);

        var response = await PutAsync(request, authHeader);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<MealPlanDto>();
        Assert.NotNull(dto);
        Assert.Single(dto.Slots);
        Assert.Equal(2, dto.Slots[0].Items.Count);
    }
}
