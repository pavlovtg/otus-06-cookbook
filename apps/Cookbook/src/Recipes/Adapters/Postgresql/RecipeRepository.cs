using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Recipes.Adapters.Postgresql.Configurations;
using Recipes.Application;
using Recipes.Application.Ports;
using Recipes.Domain;

namespace Recipes.Adapters.Postgresql;

internal sealed class RecipeRepository : DbContext, IRecipeRepository, IIngredientRepository, IRecipePhotoRepository, ICategoryRepository, IUserRepository, IMealPlanRepository, IShoppingListRepository
{
    public const string DefaultSchema = "cookbook";

    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<RecipePhoto> RecipePhotos => Set<RecipePhoto>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserFavorite> UserFavorites => Set<UserFavorite>();
    public DbSet<RecipeRating> RecipeRatings => Set<RecipeRating>();
    public DbSet<RecipeComment> RecipeComments => Set<RecipeComment>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<MealPlanSlot> MealPlanSlots => Set<MealPlanSlot>();
    public DbSet<MealPlanItem> MealPlanItems => Set<MealPlanItem>();

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
        modelBuilder.ApplyConfiguration(new UserFavoriteConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeRatingConfiguration());
        modelBuilder.ApplyConfiguration(new RecipeCommentConfiguration());
        modelBuilder.ApplyConfiguration(new MealPlanConfiguration());
        modelBuilder.ApplyConfiguration(new MealPlanSlotConfiguration());
        modelBuilder.ApplyConfiguration(new MealPlanItemConfiguration());
    }

    public async Task<PagedResult<RecipeListItem>> GetRecipesPagedAsync(
        int page,
        int pageSize,
        string? q = null,
        RecipeSortOrder sort = RecipeSortOrder.TitleAsc,
        UserId? currentUserId = null,
        bool? favorites = null,
        CancellationToken cancellationToken = default)
    {
        var query = Recipes
            .Include(r => r.Categories)
            .AsNoTracking();

        // Скрываем приватные рецепты чужих авторов
        if (currentUserId.HasValue)
        {
            var uid = currentUserId.Value;
            query = query.Where(r => r.IsPublic || r.AuthorId == uid);
        }
        else
        {
            query = query.Where(r => r.IsPublic);
        }

        // Фильтрация по избранному
        if (favorites == true && currentUserId.HasValue)
        {
            var uid = currentUserId.Value;
            query = query.Where(r => UserFavorites.Any(uf => uf.UserId == uid && uf.RecipeId == r.Id));
        }

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

        IOrderedQueryable<Recipe> ordered = sort switch
        {
            RecipeSortOrder.TitleDesc => query.OrderByDescending(r => r.Title),
            RecipeSortOrder.RatingDesc => query.OrderByDescending(r => r.AverageRating == null ? 0 : 1)
                                               .ThenByDescending(r => r.AverageRating),
            _ => query.OrderBy(r => r.Title),
        };

        var recipes = await ordered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        List<RecipeListItem> items;
        if (currentUserId.HasValue)
        {
            var uid = currentUserId.Value;
            var recipeIds = recipes.Select(r => r.Id).ToList();
            var myRatingsMap = await RecipeRatings
                .AsNoTracking()
                .Where(rr => rr.UserId == uid && recipeIds.Contains(rr.RecipeId))
                .Select(rr => new { rr.RecipeId, rr.Value })
                .ToListAsync(cancellationToken);
            var myRatingsDict = myRatingsMap.ToDictionary(x => x.RecipeId, x => (int?)x.Value);
            items = recipes
                .Select(r => new RecipeListItem(r, myRatingsDict.GetValueOrDefault(r.Id)))
                .ToList();
        }
        else
        {
            items = recipes.Select(r => new RecipeListItem(r, null)).ToList();
        }

        return new PagedResult<RecipeListItem>(items, total, page, pageSize);
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
            .OrderBy(i => i.Category)
            .ThenBy(i => i.Title)
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

    // ── IRecipeRepository: ratings ───────────────────────────────────────────

    public async Task UpsertRatingAsync(UserId userId, RecipeId recipeId, int value, CancellationToken cancellationToken = default)
    {
        var existing = await RecipeRatings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.RecipeId == recipeId, cancellationToken);

        if (existing is not null)
        {
            existing.UpdateValue(value);
            RecipeRatings.Update(existing);
        }
        else
        {
            await RecipeRatings.AddAsync(RecipeRating.Create(userId, recipeId, value), cancellationToken);
        }
    }

    public async Task<bool> DeleteRatingAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default)
    {
        var existing = await RecipeRatings
            .FirstOrDefaultAsync(r => r.UserId == userId && r.RecipeId == recipeId, cancellationToken);

        if (existing is null)
            return false;

        RecipeRatings.Remove(existing);
        return true;
    }

    public async Task<float?> GetAverageRatingAsync(RecipeId recipeId, CancellationToken cancellationToken = default)
    {
        var hasRatings = await RecipeRatings
            .AsNoTracking()
            .AnyAsync(r => r.RecipeId == recipeId, cancellationToken);

        if (!hasRatings)
            return null;

        return (float?)await RecipeRatings
            .AsNoTracking()
            .Where(r => r.RecipeId == recipeId)
            .AverageAsync(r => (double)r.Value, cancellationToken);
    }

    public async Task<int?> GetMyRatingAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default)
    {
        return await RecipeRatings
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.RecipeId == recipeId)
            .Select(r => (int?)r.Value)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // ── IRecipeRepository: favorites ─────────────────────────────────────────

    public async Task AddFavoriteAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default)
    {
        var exists = await UserFavorites
            .AnyAsync(uf => uf.UserId == userId && uf.RecipeId == recipeId, cancellationToken);

        if (!exists)
            await UserFavorites.AddAsync(UserFavorite.Create(userId, recipeId), cancellationToken);
    }

    public async Task RemoveFavoriteAsync(UserId userId, RecipeId recipeId, CancellationToken cancellationToken = default)
    {
        var favorite = await UserFavorites
            .FirstOrDefaultAsync(uf => uf.UserId == userId && uf.RecipeId == recipeId, cancellationToken);

        if (favorite is not null)
            UserFavorites.Remove(favorite);
    }

    public async Task<IReadOnlySet<RecipeId>> GetFavoriteIdsAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var ids = await UserFavorites
            .AsNoTracking()
            .Where(uf => uf.UserId == userId)
            .Select(uf => uf.RecipeId)
            .ToListAsync(cancellationToken);

        return ids.ToHashSet();
    }

    // ── IRecipeRepository: comments ──────────────────────────────────────────

    public async Task<PagedResult<CommentDetail>> GetCommentsPagedAsync(
        RecipeId recipeId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = RecipeComments
            .AsNoTracking()
            .Where(c => c.RecipeId == recipeId);

        var total = await query.CountAsync(cancellationToken);

        var rows = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Join(
                Users.AsNoTracking(),
                c => c.AuthorId,
                u => u.Id,
                (c, u) => new { Comment = c, AuthorName = u.DisplayName })
            .ToListAsync(cancellationToken);

        var items = rows
            .Select(r => new CommentDetail(
                r.Comment.Id,
                r.Comment.RecipeId,
                r.Comment.AuthorId,
                r.AuthorName,
                r.Comment.Text,
                r.Comment.CreatedAt))
            .ToList();

        return new PagedResult<CommentDetail>(items, total, page, pageSize);
    }

    public async Task AddCommentAsync(RecipeComment comment, CancellationToken cancellationToken = default)
    {
        await RecipeComments.AddAsync(comment, cancellationToken);
    }

    public async Task DeleteCommentAsync(RecipeCommentId commentId, CancellationToken cancellationToken = default)
    {
        var comment = await RecipeComments.FindAsync([commentId], cancellationToken);
        if (comment is not null)
            RecipeComments.Remove(comment);
    }

    public async Task<CommentDetail?> GetCommentAsync(RecipeCommentId commentId, CancellationToken cancellationToken = default)
    {
        var row = await RecipeComments
            .AsNoTracking()
            .Where(c => c.Id == commentId)
            .Join(
                Users.AsNoTracking(),
                c => c.AuthorId,
                u => u.Id,
                (c, u) => new { Comment = c, AuthorName = u.DisplayName })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
            return null;

        return new CommentDetail(
            row.Comment.Id,
            row.Comment.RecipeId,
            row.Comment.AuthorId,
            row.AuthorName,
            row.Comment.Text,
            row.Comment.CreatedAt);
    }

    // ── IMealPlanRepository ──────────────────────────────────────────────────

    public async Task<MealPlan?> GetPlanByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await MealPlans
            .Include(p => p.Slots)
                .ThenInclude(s => s.Items)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<MealPlanView?> GetPlanViewByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        var plan = await MealPlans
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new { p.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (plan is null)
            return null;

        var rows = await (
            from slot in MealPlanSlots.AsNoTracking()
            where EF.Property<MealPlanId>(slot, "meal_plan_id") == plan.Id
            from item in MealPlanItems.AsNoTracking()
                .Where(i => EF.Property<Guid?>(i, "MealPlanSlotId") == slot.Id)
                .DefaultIfEmpty()
            join recipe in Recipes.AsNoTracking()
                on item!.RecipeId equals recipe.Id into recipeJoin
            from recipe in recipeJoin.DefaultIfEmpty()
            select new
            {
                SlotId = slot.Id,
                slot.WeekDay,
                slot.MealType,
                Item = item,
                RecipeTitle = recipe != null ? recipe.Title : null,
            }).ToListAsync(cancellationToken);

        var slots = rows
            .GroupBy(r => new { r.SlotId, r.WeekDay, r.MealType })
            .OrderBy(g => g.Key.WeekDay)
            .ThenBy(g => g.Key.MealType)
            .Select(g => new MealPlanSlotView(
                g.Key.WeekDay,
                g.Key.MealType,
                g.Where(r => r.Item != null)
                    .Select(r => new MealPlanItemView(
                        r.Item!.Id,
                        r.Item.RecipeId.Value,
                        r.RecipeTitle ?? string.Empty,
                        r.Item.Servings.Value))
                    .ToList()))
            .ToList();

        return new MealPlanView(plan.Id.Value, slots);
    }

    // ── IShoppingListRepository ──────────────────────────────────────────────

    public async Task<IReadOnlyList<ShoppingListGroupView>> GetShoppingListAsync(
        UserId userId,
        CancellationToken cancellationToken = default)
    {
        var plan = await MealPlans
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new { p.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (plan is null)
            return [];

        // Загружаем слоты → items
        var items = await (
            from slot in MealPlanSlots.AsNoTracking()
            where EF.Property<MealPlanId>(slot, "meal_plan_id") == plan.Id
            from item in MealPlanItems.AsNoTracking()
                .Where(i => EF.Property<Guid?>(i, "MealPlanSlotId") == slot.Id)
            select new { item.RecipeId, item.Servings }
        ).ToListAsync(cancellationToken);

        if (items.Count == 0)
            return [];

        var recipeIds = items.Select(i => i.RecipeId).Distinct().ToList();

        // Загружаем рецепты с ингредиентами
        var recipes = await Recipes
            .AsNoTracking()
            .Include(r => r.Ingredients)
            .Where(r => recipeIds.Contains(r.Id))
            .ToListAsync(cancellationToken);

        var recipeMap = recipes.ToDictionary(r => r.Id);

        // Собираем все IngredientId
        var ingredientIds = recipes
            .SelectMany(r => r.Ingredients)
            .Select(ri => ri.IngredientId)
            .Distinct()
            .ToList();

        // Загружаем ингредиенты
        var ingredientMap = await Ingredients
            .AsNoTracking()
            .Where(i => ingredientIds.Contains(i.Id))
            .ToDictionaryAsync(i => i.Id, cancellationToken);

        // Агрегация: группировка по IngredientId, суммирование amount * (servings / recipe.Servings)
        var totals = new Dictionary<IngredientId, decimal>();

        foreach (var item in items)
        {
            if (!recipeMap.TryGetValue(item.RecipeId, out var recipe))
                continue;

            if (recipe.Servings <= 0)
                continue;

            var factor = (decimal)item.Servings.Value / recipe.Servings;

            foreach (var ri in recipe.Ingredients)
            {
                var amount = ri.Amount * factor;
                totals[ri.IngredientId] = totals.GetValueOrDefault(ri.IngredientId) + amount;
            }
        }

        // Группировка по Category, сортировка
        var groups = totals
            .Where(kv => ingredientMap.ContainsKey(kv.Key))
            .GroupBy(kv => ingredientMap[kv.Key].Category)
            .OrderBy(g => (int)g.Key)
            .Select(g => new ShoppingListGroupView(
                g.Key,
                g.OrderBy(kv => ingredientMap[kv.Key].Title)
                    .Select(kv => new ShoppingListItemView(
                        kv.Key,
                        ingredientMap[kv.Key].Title,
                        kv.Value,
                        ingredientMap[kv.Key].Unit))
                    .ToList()))
            .ToList();

        return groups;
    }

    public async Task SavePlanAsync(MealPlan mealPlan, CancellationToken cancellationToken = default)
    {
        var exists = await MealPlans
            .AnyAsync(p => p.Id == mealPlan.Id, cancellationToken);

        if (!exists)
        {
            await MealPlans.AddAsync(mealPlan, cancellationToken);
        }
        else
        {
            // Удаляем старые слоты (Items удаляются каскадно через OnDelete(Cascade))
            var oldSlots = await MealPlanSlots
                .Where(s => EF.Property<MealPlanId>(s, "meal_plan_id") == mealPlan.Id)
                .ToListAsync(cancellationToken);
            MealPlanSlots.RemoveRange(oldSlots);

            // Обновляем план и добавляем новые слоты
            var planEntry = Entry(mealPlan);
            if (planEntry.State == EntityState.Detached)
                MealPlans.Attach(mealPlan);
            planEntry.State = EntityState.Modified;

            foreach (var slot in mealPlan.Slots)
            {
                Entry(slot).State = EntityState.Added;
                foreach (var item in slot.Items)
                    Entry(item).State = EntityState.Added;
            }
        }
    }
}
