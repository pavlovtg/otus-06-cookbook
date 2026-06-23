using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class GetMealPlanTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;

    public Task InitializeAsync() => fixture.TruncateAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Get_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/meal-plan");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_WithAuth_Returns200WithEmptyPlan()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/meal-plan");
        request.Headers.Authorization = authHeader;

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<MealPlanDto>();
        Assert.NotNull(dto);
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Empty(dto.Slots);
    }

    [Fact]
    public async Task Get_CalledTwice_ReturnsSamePlanId()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();

        using var req1 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/meal-plan");
        req1.Headers.Authorization = authHeader;
        var resp1 = await _client.SendAsync(req1);
        var dto1 = await resp1.Content.ReadFromJsonAsync<MealPlanDto>();

        using var req2 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/meal-plan");
        req2.Headers.Authorization = authHeader;
        var resp2 = await _client.SendAsync(req2);
        var dto2 = await resp2.Content.ReadFromJsonAsync<MealPlanDto>();

        Assert.NotNull(dto1);
        Assert.NotNull(dto2);
        Assert.Equal(dto1.Id, dto2.Id);
    }

    [Fact]
    public async Task Get_AfterReplace_ReturnsSavedSlots()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();

        var putRequest = new MealPlanRequest([
            new MealPlanSlotRequest(1, 1, [new MealPlanItemRequest(Guid.NewGuid(), 2)])
        ]);

        using var putReq = new HttpRequestMessage(HttpMethod.Put, "/api/v1/meal-plan");
        putReq.Headers.Authorization = authHeader;
        putReq.Content = JsonContent.Create(putRequest);
        await _client.SendAsync(putReq);

        using var getReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/meal-plan");
        getReq.Headers.Authorization = authHeader;
        var getResp = await _client.SendAsync(getReq);

        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
        var dto = await getResp.Content.ReadFromJsonAsync<MealPlanDto>();
        Assert.NotNull(dto);
        Assert.Single(dto.Slots);
        Assert.Equal(1, dto.Slots[0].WeekDay);
        Assert.Equal(1, dto.Slots[0].MealType);
    }

    [Fact]
    public async Task Get_TwoDifferentUsers_ReturnsDifferentPlans()
    {
        var auth1 = await fixture.GetAuthHeaderAsync();
        var auth2 = await fixture.GetAuthHeaderAsync();

        using var req1 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/meal-plan");
        req1.Headers.Authorization = auth1;
        var resp1 = await _client.SendAsync(req1);
        var dto1 = await resp1.Content.ReadFromJsonAsync<MealPlanDto>();

        using var req2 = new HttpRequestMessage(HttpMethod.Get, "/api/v1/meal-plan");
        req2.Headers.Authorization = auth2;
        var resp2 = await _client.SendAsync(req2);
        var dto2 = await resp2.Content.ReadFromJsonAsync<MealPlanDto>();

        Assert.NotNull(dto1);
        Assert.NotNull(dto2);
        Assert.NotEqual(dto1.Id, dto2.Id);
    }
}
