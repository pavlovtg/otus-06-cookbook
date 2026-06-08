using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Recipes.Adapters.Postgresql;

internal sealed class RecipesDbContextFactory : IDesignTimeDbContextFactory<RecipesDbContext>
{
    public RecipesDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<RecipesDbContext>()
            .UseNpgsql("Host=localhost;Database=recipes;Username=postgres;Password=postgres")
            .Options;

        return new RecipesDbContext(options);
    }
}
