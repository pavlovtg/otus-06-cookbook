using Microsoft.EntityFrameworkCore;

namespace Recipes.Adapters.Postgresql;

internal static class CookbookSeeder
{
    public static async Task SeedAsync(RecipeRepository db, CancellationToken cancellationToken = default)
    {
        await SeedIngredientsAsync(db, cancellationToken);
        await SeedRecipesAsync(db, cancellationToken);
        await SeedRecipeIngredientsAsync(db, cancellationToken);
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
                    await db.Recipes.AddAsync(recipe, cancellationToken);
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
}
