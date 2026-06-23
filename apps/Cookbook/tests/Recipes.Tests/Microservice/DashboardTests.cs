using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class DashboardTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;

    public async Task InitializeAsync() => await fixture.TruncateAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    // ── 5.7 Anonymous → 200 ──────────────────────────────────────────────────

    [Fact]
    public async Task GetDashboard_Anonymous_Returns200_WithTotalRecipes()
    {
        var response = await _client.GetAsync("/api/v1/dashboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
        Assert.NotNull(dto);
        Assert.True(dto.TotalRecipes >= 0);
        Assert.Null(dto.MyRecipes);
        Assert.Null(dto.MyComments);
        Assert.Null(dto.PlanFill);
        Assert.Null(dto.TotalUsers);
        Assert.Null(dto.TotalComments);
        Assert.Null(dto.TopUsersByRating);
        Assert.Null(dto.TopUsersByComments);
    }

    // ── 5.8 Authorized user → 200 with user fields ───────────────────────────

    [Fact]
    public async Task GetDashboard_AuthorizedUser_Returns200_WithUserFields()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/dashboard");
        request.Headers.Authorization = authHeader;
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
        Assert.NotNull(dto);
        Assert.True(dto.TotalRecipes >= 0);
        Assert.NotNull(dto.MyRecipes);
        Assert.NotNull(dto.MyComments);
        Assert.NotNull(dto.PlanFill);
        // admin-поля должны быть null для обычного пользователя
        Assert.Null(dto.TotalUsers);
        Assert.Null(dto.TotalComments);
        Assert.Null(dto.TopUsersByRating);
        Assert.Null(dto.TopUsersByComments);
    }

    // ── 5.9 Admin → 200 with admin fields ────────────────────────────────────

    [Fact]
    public async Task GetDashboard_Admin_Returns200_WithAdminFields()
    {
        var adminHeader = await fixture.GetAdminAuthHeaderAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/dashboard");
        request.Headers.Authorization = adminHeader;
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var dto = await response.Content.ReadFromJsonAsync<DashboardStatsDto>();
        Assert.NotNull(dto);
        Assert.True(dto.TotalRecipes >= 0);
        // user-поля присутствуют (admin тоже авторизован)
        Assert.NotNull(dto.MyRecipes);
        Assert.NotNull(dto.MyComments);
        // admin-поля присутствуют
        Assert.NotNull(dto.TotalUsers);
        Assert.NotNull(dto.TotalComments);
        Assert.NotNull(dto.TopUsersByRating);
        Assert.NotNull(dto.TopUsersByComments);
    }
}
