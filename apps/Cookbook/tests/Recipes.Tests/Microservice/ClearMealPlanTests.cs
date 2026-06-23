using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class ClearMealPlanTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;

    public Task InitializeAsync() => fixture.TruncateAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private async Task<HttpResponseMessage> DeleteAsync(System.Net.Http.Headers.AuthenticationHeaderValue authHeader)
    {
        using var req = new HttpRequestMessage(HttpMethod.Delete, "/api/v1/meal-plan");
        req.Headers.Authorization = authHeader;
        return await _client.SendAsync(req);
    }

    private async Task PutSlotsAsync(System.Net.Http.Headers.AuthenticationHeaderValue authHeader)
    {
        var request = new MealPlanRequest([
            new MealPlanSlotRequest(1, 1, [new MealPlanItemRequest(Guid.NewGuid(), 2)])
        ]);
        using var req = new HttpRequestMessage(HttpMethod.Put, "/api/v1/meal-plan");
        req.Headers.Authorization = authHeader;
        req.Content = JsonContent.Create(request);
        await _client.SendAsync(req);
    }

    private async Task<MealPlanDto?> GetPlanAsync(System.Net.Http.Headers.AuthenticationHeaderValue authHeader)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/api/v1/meal-plan");
        req.Headers.Authorization = authHeader;
        var resp = await _client.SendAsync(req);
        return await resp.Content.ReadFromJsonAsync<MealPlanDto>();
    }

    [Fact]
    public async Task Delete_WithoutAuth_Returns401()
    {
        var response = await _client.DeleteAsync("/api/v1/meal-plan");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WithAuth_Returns204()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();

        var response = await DeleteAsync(authHeader);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenPlanHasNoSlots_Returns204()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        // Создаём пустой план через GET
        using var getReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/meal-plan");
        getReq.Headers.Authorization = authHeader;
        await _client.SendAsync(getReq);

        var response = await DeleteAsync(authHeader);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AfterReplace_PlanBecomesEmpty()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        await PutSlotsAsync(authHeader);

        await DeleteAsync(authHeader);

        var dto = await GetPlanAsync(authHeader);
        Assert.NotNull(dto);
        Assert.Empty(dto.Slots);
    }

    [Fact]
    public async Task Delete_CalledTwice_BothReturn204()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        await PutSlotsAsync(authHeader);

        var resp1 = await DeleteAsync(authHeader);
        var resp2 = await DeleteAsync(authHeader);

        Assert.Equal(HttpStatusCode.NoContent, resp1.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, resp2.StatusCode);
    }

    [Fact]
    public async Task Delete_DoesNotAffectOtherUser()
    {
        var auth1 = await fixture.GetAuthHeaderAsync();
        var auth2 = await fixture.GetAuthHeaderAsync();

        await PutSlotsAsync(auth1);
        await PutSlotsAsync(auth2);

        await DeleteAsync(auth1);

        var dto1 = await GetPlanAsync(auth1);
        var dto2 = await GetPlanAsync(auth2);

        Assert.NotNull(dto1);
        Assert.Empty(dto1.Slots);

        Assert.NotNull(dto2);
        Assert.Single(dto2.Slots);
    }
}
