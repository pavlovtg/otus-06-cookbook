using System.Net;
using System.Net.Http.Json;
using Recipes.Adapters.Web.Dto;
using Xunit;

namespace Recipes.Tests.Microservice;

[Collection("RecipeMicroservice")]
public sealed class RecipePhotoTests(RecipeMicroserviceFixture fixture) : IAsyncLifetime
{
    private readonly HttpClient _client = fixture.Client;
    private System.Net.Http.Headers.AuthenticationHeaderValue _authHeader = null!;

    public async Task InitializeAsync()
    {
        await fixture.TruncateAsync();
        _authHeader = await fixture.GetAuthHeaderAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task UploadPhoto_Returns200_AndRecipeHasPhotoId()
    {
        var recipe = await CreateTestRecipeAsync();
        Assert.Null(recipe.PhotoId);

        var response = await UploadPhotoAsync(recipe.Id);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var updated = await _client.GetFromJsonAsync<RecipeDto>($"/api/v1/recipes/{recipe.Id}");
        Assert.NotNull(updated);
        Assert.NotNull(updated.PhotoId);
    }

    [Fact]
    public async Task GetPhoto_Returns200_AfterUpload()
    {
        var recipe = await CreateTestRecipeAsync();
        await UploadPhotoAsync(recipe.Id);

        var updated = await _client.GetFromJsonAsync<RecipeDto>($"/api/v1/recipes/{recipe.Id}");
        Assert.NotNull(updated?.PhotoId);

        var photoResponse = await _client.GetAsync($"/api/v1/photos/{updated.PhotoId}");
        Assert.Equal(HttpStatusCode.OK, photoResponse.StatusCode);
        Assert.Equal("image/jpeg", photoResponse.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetThumbnail_Returns200_AfterUpload()
    {
        var recipe = await CreateTestRecipeAsync();
        await UploadPhotoAsync(recipe.Id);

        var updated = await _client.GetFromJsonAsync<RecipeDto>($"/api/v1/recipes/{recipe.Id}");
        Assert.NotNull(updated?.PhotoId);

        var thumbResponse = await _client.GetAsync($"/api/v1/photos/{updated.PhotoId}/thumbnail");
        Assert.Equal(HttpStatusCode.OK, thumbResponse.StatusCode);
    }

    [Fact]
    public async Task DeletePhoto_Returns204_AndRecipePhotoIdIsNull()
    {
        var recipe = await CreateTestRecipeAsync();
        await UploadPhotoAsync(recipe.Id);

        using var deleteMsg = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/recipes/{recipe.Id}/photo");
        deleteMsg.Headers.Authorization = _authHeader;
        var deleteResponse = await _client.SendAsync(deleteMsg);
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var updated = await _client.GetFromJsonAsync<RecipeDto>($"/api/v1/recipes/{recipe.Id}");
        Assert.NotNull(updated);
        Assert.Null(updated.PhotoId);
    }

    [Fact]
    public async Task UploadPhoto_Returns400_WhenInvalidMimeType()
    {
        var recipe = await CreateTestRecipeAsync();

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[1024]);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/gif");
        content.Add(fileContent, "file", "test.gif");

        using var msg = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/recipes/{recipe.Id}/photo");
        msg.Headers.Authorization = _authHeader;
        msg.Content = content;
        var response = await _client.SendAsync(msg);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPhoto_Returns400_WhenPhotoNotFound()
    {
        var response = await _client.GetAsync($"/api/v1/photos/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<RecipeDto> CreateTestRecipeAsync()
    {
        var request = new RecipeRequest(
            Title: "Рецепт для теста фото",
            Description: "Описание",
            CookingTime: 30,
            Difficulty: "easy",
            Servings: 2,
            Instructions: "Шаг 1.",
            Ingredients: [],
            CategoryIds: []
        );

        using var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/recipes");
        msg.Headers.Authorization = _authHeader;
        msg.Content = JsonContent.Create(request);
        var response = await _client.SendAsync(msg);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RecipeDto>())!;
    }

    private async Task<HttpResponseMessage> UploadPhotoAsync(Guid recipeId)
    {
        // Минимальный валидный JPEG (1x1 px)
        var jpegBytes = new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01,
            0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0xFF, 0xDB, 0x00, 0x43,
            0x00, 0x08, 0x06, 0x06, 0x07, 0x06, 0x05, 0x08, 0x07, 0x07, 0x07, 0x09,
            0x09, 0x08, 0x0A, 0x0C, 0x14, 0x0D, 0x0C, 0x0B, 0x0B, 0x0C, 0x19, 0x12,
            0x13, 0x0F, 0x14, 0x1D, 0x1A, 0x1F, 0x1E, 0x1D, 0x1A, 0x1C, 0x1C, 0x20,
            0x24, 0x2E, 0x27, 0x20, 0x22, 0x2C, 0x23, 0x1C, 0x1C, 0x28, 0x37, 0x29,
            0x2C, 0x30, 0x31, 0x34, 0x34, 0x34, 0x1F, 0x27, 0x39, 0x3D, 0x38, 0x32,
            0x3C, 0x2E, 0x33, 0x34, 0x32, 0xFF, 0xC0, 0x00, 0x0B, 0x08, 0x00, 0x01,
            0x00, 0x01, 0x01, 0x01, 0x11, 0x00, 0xFF, 0xC4, 0x00, 0x1F, 0x00, 0x00,
            0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
            0x09, 0x0A, 0x0B, 0xFF, 0xC4, 0x00, 0xB5, 0x10, 0x00, 0x02, 0x01, 0x03,
            0x03, 0x02, 0x04, 0x03, 0x05, 0x05, 0x04, 0x04, 0x00, 0x00, 0x01, 0x7D,
            0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12, 0x21, 0x31, 0x41, 0x06,
            0x13, 0x51, 0x61, 0x07, 0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xA1, 0x08,
            0x23, 0x42, 0xB1, 0xC1, 0x15, 0x52, 0xD1, 0xF0, 0x24, 0x33, 0x62, 0x72,
            0x82, 0x09, 0x0A, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x25, 0x26, 0x27, 0x28,
            0x29, 0x2A, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x43, 0x44, 0x45,
            0x46, 0x47, 0x48, 0x49, 0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59,
            0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x73, 0x74, 0x75,
            0x76, 0x77, 0x78, 0x79, 0x7A, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89,
            0x8A, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0xA2, 0xA3,
            0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6,
            0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9,
            0xCA, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xE1, 0xE2,
            0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xF1, 0xF2, 0xF3, 0xF4,
            0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA, 0xFF, 0xDA, 0x00, 0x08, 0x01, 0x01,
            0x00, 0x00, 0x3F, 0x00, 0xFB, 0xD2, 0x8A, 0x28, 0x03, 0xFF, 0xD9
        };

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(jpegBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        content.Add(fileContent, "file", "test.jpg");

        using var msg = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/recipes/{recipeId}/photo");
        msg.Headers.Authorization = _authHeader;
        msg.Content = content;
        return await _client.SendAsync(msg);
    }
}
