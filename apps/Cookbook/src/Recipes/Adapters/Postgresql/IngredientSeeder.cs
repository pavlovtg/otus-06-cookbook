using Recipes.Domain;

namespace Recipes.Adapters.Postgresql;

internal static class IngredientSeeder
{
    public static async Task SeedAsync(RecipeRepository db, CancellationToken cancellationToken = default)
    {
        foreach (var ingredient in IngredientSeedData.Ingredients)
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

internal static class IngredientSeedData
{
    public static readonly Ingredient[] Ingredients =
    [
        // Vegetables
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000001")), "Морковь", "г", 100f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000002")), "Картофель", "г", 200f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000003")), "Лук репчатый", "г", 100f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000004")), "Свёкла", "г", 150f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000005")), "Капуста белокочанная", "г", 200f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000006")), "Помидор", "г", 150f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000007")), "Огурец", "г", 100f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000008")), "Перец болгарский", "г", 100f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000009")), "Чеснок", "зуб.", 3f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000010")), "Кабачок", "г", 200f, IngredientCategory.Vegetables, isSystem: true),

        // Fruits and Berries
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000011")), "Яблоко", "г", 150f, IngredientCategory.FruitsAndBerries, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000012")), "Лимон", "шт.", 1f, IngredientCategory.FruitsAndBerries, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000013")), "Клубника", "г", 100f, IngredientCategory.FruitsAndBerries, isSystem: true),

        // Meat and Poultry
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000014")), "Говядина", "г", 300f, IngredientCategory.MeatAndPoultry, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000015")), "Свинина", "г", 300f, IngredientCategory.MeatAndPoultry, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000016")), "Куриное филе", "г", 300f, IngredientCategory.MeatAndPoultry, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000017")), "Куриные бёдра", "г", 400f, IngredientCategory.MeatAndPoultry, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000018")), "Фарш смешанный", "г", 400f, IngredientCategory.MeatAndPoultry, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000019")), "Колбаса варёная", "г", 150f, IngredientCategory.MeatAndPoultry, isSystem: true),

        // Fish and Seafood
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000020")), "Лосось", "г", 200f, IngredientCategory.FishAndSeafood, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000021")), "Треска", "г", 200f, IngredientCategory.FishAndSeafood, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000022")), "Креветки", "г", 150f, IngredientCategory.FishAndSeafood, isSystem: true),

        // Dairy and Eggs
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000023")), "Молоко", "мл", 200f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000024")), "Сливки 20%", "мл", 100f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000025")), "Сметана", "г", 100f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000026")), "Сыр твёрдый", "г", 100f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000027")), "Яйцо куриное", "шт.", 2f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000028")), "Масло сливочное", "г", 50f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000029")), "Творог", "г", 200f, IngredientCategory.DairyAndEggs, isSystem: true),

        // Grains and Cereals
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000030")), "Мука пшеничная", "г", 200f, IngredientCategory.GrainsAndCereals, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000031")), "Рис", "г", 150f, IngredientCategory.GrainsAndCereals, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000032")), "Гречка", "г", 150f, IngredientCategory.GrainsAndCereals, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000033")), "Овсяные хлопья", "г", 100f, IngredientCategory.GrainsAndCereals, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000034")), "Макароны", "г", 200f, IngredientCategory.GrainsAndCereals, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000035")), "Манная крупа", "г", 100f, IngredientCategory.GrainsAndCereals, isSystem: true),

        // Legumes
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000036")), "Фасоль красная", "г", 150f, IngredientCategory.Legumes, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000037")), "Горох", "г", 150f, IngredientCategory.Legumes, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000038")), "Чечевица", "г", 150f, IngredientCategory.Legumes, isSystem: true),

        // Nuts and Seeds
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000039")), "Грецкий орех", "г", 50f, IngredientCategory.NutsAndSeeds, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000040")), "Семена кунжута", "г", 20f, IngredientCategory.NutsAndSeeds, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000041")), "Миндаль", "г", 50f, IngredientCategory.NutsAndSeeds, isSystem: true),

        // Oils and Fats
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000042")), "Масло подсолнечное", "мл", 30f, IngredientCategory.OilsAndFats, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000043")), "Масло оливковое", "мл", 30f, IngredientCategory.OilsAndFats, isSystem: true),

        // Spices and Seasonings
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000044")), "Соль", "г", 5f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000045")), "Перец чёрный молотый", "г", 2f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000046")), "Сахар", "г", 50f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000047")), "Лавровый лист", "шт.", 2f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000048")), "Паприка", "г", 5f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000049")), "Куркума", "г", 3f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000050")), "Укроп свежий", "г", 20f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000051")), "Петрушка свежая", "г", 20f, IngredientCategory.SpicesAndSeasonings, isSystem: true),

        // Sauces and Pastes
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000052")), "Томатная паста", "г", 50f, IngredientCategory.SaucesAndPastes, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000053")), "Майонез", "г", 50f, IngredientCategory.SaucesAndPastes, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000054")), "Соевый соус", "мл", 30f, IngredientCategory.SaucesAndPastes, isSystem: true),

        // Bakery and Sweets
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000055")), "Хлеб белый", "г", 100f, IngredientCategory.BakeryAndSweets, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000056")), "Панировочные сухари", "г", 50f, IngredientCategory.BakeryAndSweets, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000057")), "Мёд", "г", 30f, IngredientCategory.BakeryAndSweets, isSystem: true),
    ];
}
