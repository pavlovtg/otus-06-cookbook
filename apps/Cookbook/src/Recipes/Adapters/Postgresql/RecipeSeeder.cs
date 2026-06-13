namespace Recipes.Adapters.Postgresql;

internal static class RecipeSeeder
{
    public static async Task SeedAsync(RecipeRepository db, CancellationToken cancellationToken = default)
    {
        foreach (var recipe in SeedData.Recipes)
        {
            var exists = await db.Recipes.FindAsync([recipe.Id], cancellationToken);
            if (exists is null)
                await db.Recipes.AddAsync(recipe, cancellationToken);
            else
                exists.Update(recipe.Title, recipe.Description, recipe.CookingTime, recipe.Difficulty, recipe.Servings, recipe.Instructions);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
