using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class AuthControllerTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;

    public Task InitializeAsync() => fixture.TruncateAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Register_NewUser_Returns200WithToken()
    {
        var request = new RegisterRequest("new@test.local", "New User", "Password1!");

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.NotEmpty(auth.Token);
        Assert.Equal("new@test.local", auth.User.Email);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns409()
    {
        var request = new RegisterRequest("dup@test.local", "User", "Password1!");
        await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithToken()
    {
        var email = "login@test.local";
        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest(email, "User", "Password1!"));

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, "Password1!"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(auth);
        Assert.NotEmpty(auth.Token);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var email = "wrongpwd@test.local";
        await _client.PostAsJsonAsync("/api/v1/auth/register", new RegisterRequest(email, "User", "Password1!"));

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest(email, "WrongPassword!"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Me_WithToken_Returns200WithUserInfo()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        request.Headers.Authorization = authHeader;

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>();
        Assert.NotNull(user);
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Fact]
    public async Task Me_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithToken_Returns204()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/logout");
        request.Headers.Authorization = authHeader;

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithoutToken_Returns401()
    {
        var response = await _client.PostAsync("/api/v1/auth/logout", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
