using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Recipes.Adapters.Postgresql;

internal sealed class RecipesDbContextFactory : IDesignTimeDbContextFactory<RecipeRepository>
{
    public RecipeRepository CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<RecipeRepository>()
            .UseNpgsql(
                "Host=localhost;Database=recipes;Username=postgres;Password=postgres",
                o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, RecipeRepository.DefaultSchema))
            .Options;

        return new RecipeRepository(options);
    }
}
