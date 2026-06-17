using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Recipes.Adapters.Postgresql.Configurations;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql;

internal sealed class RecipeRepository : DbContext, IRecipeRepository, IIngredientRepository, IRecipePhotoRepository, ICategoryRepository, IUserRepository
{
    public const string DefaultSchema = "cookbook";

    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipePhoto> RecipePhotos => Set<RecipePhoto>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<User> Users => Set<User>();

    public RecipeRepository(DbContextOptions<RecipeRepository> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DefaultSchema);
        modelBuilder.ApplyConfiguration(new RecipeConfiguration());
        modelBuilder.ApplyConfiguration(new IngredientConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeIngredientConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeCategoryConfiguration());
        modelBuilder.ApplyConfiguration(new RecipePhotoConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }

    public async Task<PagedResult<Recipe>> GetRecipesPagedAsync(
        int page,
        int pageSize,
        string? q = null,
        RecipeSortOrder sort = RecipeSortOrder.TitleAsc,
        UserId? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Recipes
            .Include(r => r.Categories)
            .AsNoTracking();

        // Скрываем приватные рецепты чужих авторов
        query = query.Where(r => r.IsPublic || r.AuthorId == currentUserId);

        if (!string.IsNullOrWhiteSpace(q))
        {
            var words = q.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (var word in words)
            {
                var w = word.ToLower();
                query = query.Where(r =>
                    r.Title.ToLower().Contains(w) ||
                    r.Description.ToLower().Contains(w) ||
                    r.Categories.Any(rc => Categories.Any(c => c.Id == rc.CategoryId && c.Name.ToLower().Contains(w))) ||
                    r.Ingredients.Any(ri => Ingredients.Any(i => i.Id == ri.IngredientId && i.Title.ToLower().Contains(w))));
            }
        }

        var total = await query.CountAsync(cancellationToken);

        var ordered = sort == RecipeSortOrder.TitleDesc
            ? query.OrderByDescending(r => r.Title)
            : query.OrderBy(r => r.Title);

        var items = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Recipe>(items, total, page, pageSize);
    }

    public async Task<Recipe?> GetByIdAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        return await Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Categories)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<RecipeWithIngredientDetails?> GetByIdWithDetailsAsync(
        RecipeId id,
        CancellationToken cancellationToken = default)
    {
        var recipe = await Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Categories)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (recipe is null)
            return null;

        var ingredientIds = recipe.Ingredients.Select(i => i.IngredientId).ToList();

        var ingredientMap = await Ingredients
            .AsNoTracking()
            .Where(i => ingredientIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, cancellationToken);

        var details = recipe.Ingredients
            .Select(ri => ingredientMap.TryGetValue(ri.IngredientId, out var ing)
                ? new RecipeIngredientDetail(ri.IngredientId, ing.Title, ri.Amount, ing.Unit)
                : new RecipeIngredientDetail(ri.IngredientId, string.Empty, ri.Amount, string.Empty))
            .ToList();

        string? authorName = null;
        if (recipe.AuthorId is not null)
        {
            authorName = await Users
                .AsNoTracking()
                .Where(u => u.Id == recipe.AuthorId)
                .Select(u => u.DisplayName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new RecipeWithIngredientDetails(recipe, details, authorName);
    }

    public async Task CreateAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        await Recipes.AddAsync(recipe, cancellationToken);
    }

    public Task UpdateAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        Recipes.Update(recipe);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(RecipeId id, CancellationToken cancellationToken = default)
    {
        var recipe = await Recipes.FindAsync([id], cancellationToken);
        if (recipe is not null)
            Recipes.Remove(recipe);
    }

    public async Task<RecipeUsageResult> GetRecipesUsingIngredientAsync(
        IngredientId ingredientId,
        CancellationToken cancellationToken = default)
    {
        var query = Recipes
            .AsNoTracking()
            .Where(r => r.Ingredients.Any(i => i.IngredientId == ingredientId));

        var totalCount = await query.CountAsync(cancellationToken);

        var topTitles = await query
            .OrderBy(r => r.Title)
            .Take(10)
            .Select(r => r.Title)
            .ToListAsync(cancellationToken);

        return new RecipeUsageResult(topTitles, totalCount);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
    }

    // ── IIngredientRepository ────────────────────────────────────────────────

    public async Task<PagedResult<Ingredient>> GetIngredientsAsync(
        int page,
        int pageSize,
        string? titleFilter = null,
        IngredientCategory? categoryFilter = null,
        CancellationToken cancellationToken = default)
    {
        var query = Ingredients.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(titleFilter))
            query = query.Where(i => i.Title.ToLower().Contains(titleFilter.ToLower()));

        if (categoryFilter.HasValue)
            query = query.Where(i => i.Category == categoryFilter.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Ingredient>(items, total, page, pageSize);
    }

    public async Task<Ingredient?> GetByIdAsync(IngredientId id, CancellationToken cancellationToken = default)
    {
        return await Ingredients.FindAsync([id], cancellationToken);
    }

    public async Task CreateAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
    {
        await Ingredients.AddAsync(ingredient, cancellationToken);
    }

    public Task UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
    {
        Ingredients.Update(ingredient);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(IngredientId id, CancellationToken cancellationToken = default)
    {
        var ingredient = await Ingredients.FindAsync([id], cancellationToken);
        if (ingredient is not null)
            Ingredients.Remove(ingredient);
    }

    // ── IRecipePhotoRepository ───────────────────────────────────────────────

    async Task IRecipePhotoRepository.SaveAsync(RecipePhoto photo, CancellationToken cancellationToken)
    {
        var existing = await RecipePhotos
            .FirstOrDefaultAsync(p => p.Id == photo.Id, cancellationToken);

        if (existing is null)
            await RecipePhotos.AddAsync(photo, cancellationToken);
        else
            RecipePhotos.Update(photo);
    }

    async Task<byte[]?> IRecipePhotoRepository.GetOriginalAsync(RecipePhotoId id, CancellationToken cancellationToken)
    {
        return await RecipePhotos
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => p.OriginalData)
            .FirstOrDefaultAsync(cancellationToken);
    }

    async Task<byte[]?> IRecipePhotoRepository.GetThumbnailAsync(RecipePhotoId id, CancellationToken cancellationToken)
    {
        return await RecipePhotos
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => p.ThumbnailData)
            .FirstOrDefaultAsync(cancellationToken);
    }

    async Task IRecipePhotoRepository.DeleteAsync(RecipePhotoId id, CancellationToken cancellationToken)
    {
        var photo = await RecipePhotos.FindAsync([id], cancellationToken);
        if (photo is not null)
            RecipePhotos.Remove(photo);
    }

    // ── ICategoryRepository ──────────────────────────────────────────────────

    public async Task<IReadOnlyList<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        return await Categories.FindAsync([id], cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetByIdsAsync(IEnumerable<CategoryId> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return await Categories
            .AsNoTracking()
            .Where(c => idList.Contains(c.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await Categories.CountAsync(cancellationToken);
    }

    public Task<bool> IsUsedInRecipesAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        return Recipes
            .AsNoTracking()
            .AnyAsync(r => r.Categories.Any(c => c.CategoryId == id), cancellationToken);
    }

    public async Task CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        await Categories.AddAsync(category, cancellationToken);
    }

    public Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        Categories.Update(category);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        var category = await Categories.FindAsync([id], cancellationToken);
        if (category is not null)
            Categories.Remove(category);
    }

    // ── IUserRepository ──────────────────────────────────────────────────────

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<UserId, string>> GetDisplayNamesByIdsAsync(
        IEnumerable<UserId> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        var result = await Users
            .AsNoTracking()
            .Where(u => idList.Contains(u.Id))
            .Select(u => new { u.Id, u.DisplayName })
            .ToListAsync(cancellationToken);

        return result.ToDictionary(u => u.Id, u => u.DisplayName);
    }

    async Task IUserRepository.CreateAsync(User user, CancellationToken cancellationToken)
    {
        await Users.AddAsync(user, cancellationToken);
    }
}
