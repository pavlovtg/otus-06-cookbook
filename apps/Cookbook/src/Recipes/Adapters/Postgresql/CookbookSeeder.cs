using Microsoft.EntityFrameworkCore;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql;

internal static class CookbookSeeder
{
    public static async Task SeedAsync(RecipeRepository db, IPasswordHasher passwordHasher, string? photosPath = null, CancellationToken cancellationToken = default)
    {
        await SeedUsersAsync(db, passwordHasher, cancellationToken);
        await SeedCategoriesAsync(db, cancellationToken);
        await SeedIngredientsAsync(db, cancellationToken);
        await SeedRecipesAsync(db, cancellationToken);
        await SeedRecipeIngredientsAsync(db, cancellationToken);
        await SeedRecipeCategoriesAsync(db, cancellationToken);
        await SeedPhotosAsync(db, photosPath, cancellationToken);
    }

    private static async Task SeedUsersAsync(RecipeRepository db, IPasswordHasher passwordHasher, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var seedUsers = new[]
            {
                (Id: new Guid("00000000-0000-0000-0000-000000000001"), Email: "user@cookbook.local", DisplayName: "User", Role: UserRole.User),
                (Id: new Guid("00000000-0000-0000-0000-000000000002"), Email: "admin@cookbook.local", DisplayName: "Admin", Role: UserRole.Admin),
            };

            foreach (var (id, email, displayName, role) in seedUsers)
            {
                var userId = UserId.From(id);
                var exists = await db.Users.FindAsync([userId], cancellationToken);
                if (exists is null)
                {
                    var passwordHash = passwordHasher.Hash("Password1!");
                    var user = User.Create(userId, email, displayName, passwordHash, role);
                    await db.Users.AddAsync(user, cancellationToken);
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task SeedCategoriesAsync(RecipeRepository db, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var category in SeedData.Categories)
            {
                var exists = await db.Categories.FindAsync([category.Id], cancellationToken);
                if (exists is null)
                    await db.Categories.AddAsync(category, cancellationToken);
                else
                    exists.Update(category.Name, category.Description);
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task SeedIngredientsAsync(RecipeRepository db, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var ingredient in SeedData.Ingredients)
            {
                var exists = await db.Ingredients.FindAsync([ingredient.Id], cancellationToken);
                if (exists is null)
                    await db.Ingredients.AddAsync(ingredient, cancellationToken);
                else
                    exists.Update(ingredient.Title, ingredient.Unit, ingredient.DefaultAmount, ingredient.Category);
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task SeedRecipesAsync(RecipeRepository db, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var recipe in SeedData.Recipes)
            {
                var exists = await db.Recipes.FindAsync([recipe.Id], cancellationToken);
                if (exists is null)
                {
                    var recipeWithoutIngredients = Recipe.Create(
                        recipe.Id,
                        recipe.Title,
                        recipe.Description,
                        recipe.CookingTime,
                        recipe.Difficulty,
                        recipe.Servings,
                        recipe.Instructions);
                    await db.Recipes.AddAsync(recipeWithoutIngredients, cancellationToken);
                }
                else
                    exists.Update(
                        recipe.Title,
                        recipe.Description,
                        recipe.CookingTime,
                        recipe.Difficulty,
                        recipe.Servings,
                        recipe.Instructions);
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task SeedRecipeIngredientsAsync(RecipeRepository db, CancellationToken cancellationToken)
    {
        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var recipe in SeedData.Recipes)
            {
                var exists = await db.Recipes
                    .Include(r => r.Ingredients)
                    .FirstOrDefaultAsync(r => r.Id == recipe.Id, cancellationToken);

                if (exists is not null)
                    exists.Update(
                        exists.Title,
                        exists.Description,
                        exists.CookingTime,
                        exists.Difficulty,
                        exists.Servings,
                        exists.Instructions,
                        recipe.Ingredients);
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task SeedRecipeCategoriesAsync(RecipeRepository db, CancellationToken cancellationToken)
    {
        if (SeedData.RecipeCategorySeeds.Length == 0)
            return;

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var (recipeId, categoryTypes) in SeedData.RecipeCategorySeeds)
            {
                var recipe = await db.Recipes
                    .Include(r => r.Categories)
                    .FirstOrDefaultAsync(r => r.Id == recipeId, cancellationToken);

                if (recipe is null)
                    continue;

                // Идемпотентно: обновляем только если набор категорий изменился
                var existingIds = recipe.Categories.Select(c => c.CategoryId).ToHashSet();
                var newIds = categoryTypes.Keys.ToHashSet();
                if (existingIds.SetEquals(newIds))
                    continue;

                recipe.Update(
                    recipe.Title,
                    recipe.Description,
                    recipe.CookingTime,
                    recipe.Difficulty,
                    recipe.Servings,
                    recipe.Instructions,
                    recipe.Ingredients,
                    categoryTypes);
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static async Task SeedPhotosAsync(RecipeRepository db, string? photosPath, CancellationToken cancellationToken)
    {
        if (SeedData.RecipePhotoSeeds.Length == 0)
            return;

        if (string.IsNullOrWhiteSpace(photosPath))
            return;

        var photosDir = new DirectoryInfo(photosPath);
        if (!photosDir.Exists)
            return;

        var thumbnailGenerator = new ImageSharpThumbnailGenerator();

        await using var tx = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var (recipeId, photoId) in SeedData.RecipePhotoSeeds)
            {
                var recipe = await db.Recipes
                    .FirstOrDefaultAsync(r => r.Id == recipeId, cancellationToken);

                if (recipe is null)
                    continue;

                if (recipe.PhotoId == photoId)
                    continue;

                var photoFile = FindPhotoFile(photosDir, photoId);
                if (photoFile is null)
                    continue;

                var rawData = await File.ReadAllBytesAsync(photoFile.FullName, cancellationToken);
                var ext = photoFile.Extension.ToLowerInvariant();
                byte[] originalData;
                string mimeType;
                if (ext == ".png")
                {
                    originalData = rawData;
                    mimeType = "image/png";
                }
                else if (ext is ".jpg" or ".jpeg")
                {
                    originalData = rawData;
                    mimeType = "image/jpeg";
                }
                else
                {
                    // Конвертируем неподдерживаемые форматы (webp и др.) в JPEG
                    originalData = thumbnailGenerator.ConvertToJpeg(rawData);
                    mimeType = "image/jpeg";
                }
                var thumbnailData = thumbnailGenerator.Generate(originalData);

                if (recipe.PhotoId is not null)
                {
                    var oldPhoto = await db.RecipePhotos.FindAsync([recipe.PhotoId], cancellationToken);
                    if (oldPhoto is not null)
                        db.RecipePhotos.Remove(oldPhoto);
                }

                var photo = RecipePhoto.Create(photoId, recipeId, mimeType, originalData, thumbnailData);
                await db.RecipePhotos.AddAsync(photo, cancellationToken);
                recipe.SetPhoto(photoId);
            }

            await db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static FileInfo? FindPhotoFile(DirectoryInfo dir, RecipePhotoId photoId)
    {
        var id = photoId.Value.ToString();
        foreach (var ext in new[] { ".jpg", ".jpeg", ".png", ".webp" })
        {
            var file = new FileInfo(Path.Combine(dir.FullName, id + ext));
            if (file.Exists)
                return file;
        }
        return null;
    }
}
