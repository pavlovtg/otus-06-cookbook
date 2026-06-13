namespace Recipes.Adapters.Postgresql;

internal static class IngredientSeeder
{
    public static async Task SeedAsync(RecipeRepository db, CancellationToken cancellationToken = default)
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
    }
}
