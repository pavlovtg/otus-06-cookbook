using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class GetShoppingListTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;

    public Task InitializeAsync() => fixture.TruncateAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    // ── 5.8 Без авторизации → 401 ────────────────────────────────────────────

    [Fact]
    public async Task Get_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync("/api/v1/shopping-list");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ── 5.7 Пустой план → 200 с пустым массивом ──────────────────────────────

    [Fact]
    public async Task Get_WithEmptyPlan_Returns200WithEmptyArray()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/shopping-list");
        request.Headers.Authorization = authHeader;

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<ShoppingListGroupDto[]>();
        Assert.NotNull(dto);
        Assert.Empty(dto);
    }

    // ── 5.6 Заполненный план → 200 с данными ─────────────────────────────────

    [Fact]
    public async Task Get_WithFilledPlan_Returns200WithItems()
    {
        var authHeader = await fixture.GetAuthHeaderAsync();

        // Создаём ингредиент
        var ingredientRequest = new IngredientRequest("Potato", "g", 200f, "vegetables");
        using var ingReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/ingredients")
        {
            Content = JsonContent.Create(ingredientRequest),
            Headers = { Authorization = authHeader },
        };
        var ingResp = await _client.SendAsync(ingReq);
        ingResp.EnsureSuccessStatusCode();
        var ingredient = await ingResp.Content.ReadFromJsonAsync<IngredientDto>();
        Assert.NotNull(ingredient);

        // Создаём рецепт с ингредиентом
        var recipeRequest = new RecipeRequest(
            Title: "Test Recipe",
            Description: "Description",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Step 1.",
            IsPublic: true,
            Ingredients: [new RecipeIngredientRequest(ingredient.Id, 400m)],
            CategoryIds: []);

        using var recipeReq = new HttpRequestMessage(HttpMethod.Post, "/api/v1/recipes")
        {
            Content = JsonContent.Create(recipeRequest),
            Headers = { Authorization = authHeader },
        };
        var recipeResp = await _client.SendAsync(recipeReq);
        recipeResp.EnsureSuccessStatusCode();
        var recipe = await recipeResp.Content.ReadFromJsonAsync<RecipeDto>();
        Assert.NotNull(recipe);

        // Заполняем план: рецепт на 2 порции (= стандартные порции → factor=1 → 400г)
        var planRequest = new MealPlanRequest([
            new MealPlanSlotRequest(1, 1, [new MealPlanItemRequest(recipe.Id, 2)])
        ]);

        using var planReq = new HttpRequestMessage(HttpMethod.Put, "/api/v1/meal-plan")
        {
            Content = JsonContent.Create(planRequest),
            Headers = { Authorization = authHeader },
        };
        var planResp = await _client.SendAsync(planReq);
        planResp.EnsureSuccessStatusCode();

        // Запрашиваем список покупок
        using var shoppingReq = new HttpRequestMessage(HttpMethod.Get, "/api/v1/shopping-list");
        shoppingReq.Headers.Authorization = authHeader;

        var response = await _client.SendAsync(shoppingReq);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var dto = await response.Content.ReadFromJsonAsync<ShoppingListGroupDto[]>();
        Assert.NotNull(dto);
        Assert.NotEmpty(dto);

        var allItems = dto.SelectMany(g => g.Items).ToList();
        Assert.Single(allItems);
        Assert.Equal("Potato", allItems[0].Title);
        Assert.Equal(400m, allItems[0].Amount);
        Assert.Equal("g", allItems[0].Unit);
    }
}
