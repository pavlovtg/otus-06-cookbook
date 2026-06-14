using Recipes.Domain;

namespace Recipes.Adapters.Postgresql;

internal static class SeedData
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

        // Vegetables (дополнительные)
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000058")), "Баклажан", "г", 200f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000059")), "Тыква", "г", 300f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000060")), "Брокколи", "г", 200f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000061")), "Цветная капуста", "г", 300f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000062")), "Шпинат", "г", 100f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000063")), "Сельдерей", "г", 100f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000064")), "Редис", "г", 100f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000065")), "Лук-порей", "г", 100f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000066")), "Спаржа", "г", 150f, IngredientCategory.Vegetables, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000067")), "Артишок", "шт.", 2f, IngredientCategory.Vegetables, isSystem: true),

        // Fruits and Berries (дополнительные)
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000068")), "Апельсин", "шт.", 2f, IngredientCategory.FruitsAndBerries, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000069")), "Банан", "шт.", 2f, IngredientCategory.FruitsAndBerries, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000070")), "Груша", "г", 150f, IngredientCategory.FruitsAndBerries, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000071")), "Вишня", "г", 150f, IngredientCategory.FruitsAndBerries, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000072")), "Малина", "г", 100f, IngredientCategory.FruitsAndBerries, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000073")), "Черника", "г", 100f, IngredientCategory.FruitsAndBerries, isSystem: true),

        // Meat and Poultry (дополнительные)
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000074")), "Баранина", "г", 400f, IngredientCategory.MeatAndPoultry, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000075")), "Утка", "г", 500f, IngredientCategory.MeatAndPoultry, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000076")), "Индейка", "г", 400f, IngredientCategory.MeatAndPoultry, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000077")), "Кролик", "г", 500f, IngredientCategory.MeatAndPoultry, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000078")), "Бекон", "г", 100f, IngredientCategory.MeatAndPoultry, isSystem: true),

        // Fish and Seafood (дополнительные)
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000079")), "Тунец", "г", 200f, IngredientCategory.FishAndSeafood, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000080")), "Сельдь", "г", 200f, IngredientCategory.FishAndSeafood, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000081")), "Мидии", "г", 200f, IngredientCategory.FishAndSeafood, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000082")), "Кальмар", "г", 200f, IngredientCategory.FishAndSeafood, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000083")), "Минтай", "г", 300f, IngredientCategory.FishAndSeafood, isSystem: true),

        // Dairy and Eggs (дополнительные)
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000084")), "Кефир", "мл", 200f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000085")), "Ряженка", "мл", 200f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000086")), "Сыр моцарелла", "г", 125f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000087")), "Сыр фета", "г", 100f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000088")), "Сливки 33%", "мл", 200f, IngredientCategory.DairyAndEggs, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000089")), "Йогурт натуральный", "г", 150f, IngredientCategory.DairyAndEggs, isSystem: true),

        // Grains and Cereals (дополнительные)
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000090")), "Перловка", "г", 150f, IngredientCategory.GrainsAndCereals, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000091")), "Пшено", "г", 150f, IngredientCategory.GrainsAndCereals, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000092")), "Кукурузная крупа", "г", 150f, IngredientCategory.GrainsAndCereals, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000093")), "Булгур", "г", 150f, IngredientCategory.GrainsAndCereals, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000094")), "Киноа", "г", 150f, IngredientCategory.GrainsAndCereals, isSystem: true),

        // Spices and Seasonings (дополнительные)
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000095")), "Зира", "г", 3f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000096")), "Корица", "г", 3f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000097")), "Имбирь молотый", "г", 3f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000098")), "Мускатный орех", "г", 2f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000099")), "Базилик сушёный", "г", 3f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000100")), "Орегано", "г", 3f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000101")), "Тимьян", "г", 3f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000102")), "Розмарин", "г", 3f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000103")), "Кинза свежая", "г", 20f, IngredientCategory.SpicesAndSeasonings, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000104")), "Мята свежая", "г", 10f, IngredientCategory.SpicesAndSeasonings, isSystem: true),

        // Sauces and Pastes (дополнительные)
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000105")), "Горчица", "г", 20f, IngredientCategory.SaucesAndPastes, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000106")), "Уксус яблочный", "мл", 30f, IngredientCategory.SaucesAndPastes, isSystem: true),
        Ingredient.Create(IngredientId.From(new Guid("22222222-0000-0000-0000-000000000107")), "Вустерский соус", "мл", 15f, IngredientCategory.SaucesAndPastes, isSystem: true),
    ];

    public static readonly (RecipeId RecipeId, RecipePhotoId PhotoId)[] RecipePhotoSeeds =
    [
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000001")), RecipePhotoId.From(new Guid("38f85723-5de0-4523-b9e9-48dbd8705f2b"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000002")), RecipePhotoId.From(new Guid("4fefadd8-d9e0-4ac7-a71e-6ea952905633"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000003")), RecipePhotoId.From(new Guid("261de524-1aa8-4851-b41c-7854ea58ae5a"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000004")), RecipePhotoId.From(new Guid("d578645b-952f-4d1b-b6b0-539a01a45ac6"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000005")), RecipePhotoId.From(new Guid("c9b6ec99-9df6-420b-a09e-d0650d72df8c"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000006")), RecipePhotoId.From(new Guid("15a9d215-b22b-48da-b748-595b49cff318"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000007")), RecipePhotoId.From(new Guid("a4ab5ae4-b32d-4b29-a489-a3725f135224"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000008")), RecipePhotoId.From(new Guid("bd79ca5f-b921-4cca-a814-b32b6fac4f0d"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000009")), RecipePhotoId.From(new Guid("491e10ea-80b5-46f7-9f8f-65b295359bb1"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000010")), RecipePhotoId.From(new Guid("1580683a-45da-4171-aa09-f7e979c1e0b9"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000011")), RecipePhotoId.From(new Guid("88d07117-74d0-44f2-8cf9-aec8d74a62b4"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000012")), RecipePhotoId.From(new Guid("198b6951-5e36-4fa7-a1c4-237f76656b96"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000013")), RecipePhotoId.From(new Guid("ca2e5d2a-b646-43d0-927f-8a3bda89c512"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000014")), RecipePhotoId.From(new Guid("071372d5-7cb8-4951-a384-260f572dc4e4"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000015")), RecipePhotoId.From(new Guid("d2b97ce6-de87-48ca-809c-48f1b089777c"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000016")), RecipePhotoId.From(new Guid("c50ce52c-2fda-42ac-ac0f-6374a168a181"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000017")), RecipePhotoId.From(new Guid("18cacde7-9256-4d22-b501-00cb2afb9970"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000018")), RecipePhotoId.From(new Guid("e0ef0d0d-bb62-4642-b145-6c9b65317e9f"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000019")), RecipePhotoId.From(new Guid("c26be25d-68fc-4371-abce-1cbe1f6ea03c"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000020")), RecipePhotoId.From(new Guid("34e892cc-7871-48f7-8d01-c8e0e2a843d5"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000021")), RecipePhotoId.From(new Guid("bcdc6a15-b26e-4116-8fab-f5631447e583"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000022")), RecipePhotoId.From(new Guid("0f1734b9-ef12-4b63-8eb7-636ca9315fb9"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000023")), RecipePhotoId.From(new Guid("901748a4-10ac-4d8e-b402-47e16bee73b7"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000024")), RecipePhotoId.From(new Guid("9b3cad97-3910-4cfd-8023-c8492aab02f5"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000025")), RecipePhotoId.From(new Guid("88f036c9-3fcd-4659-b502-2fd1772d88aa"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000026")), RecipePhotoId.From(new Guid("b57717ae-c83d-4d97-9055-acc98ec04d2d"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000027")), RecipePhotoId.From(new Guid("ecba09e0-b84c-4902-82b2-ef3d0040e01d"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000028")), RecipePhotoId.From(new Guid("d6d8a86c-db27-40b2-b894-22d5645c75c6"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000029")), RecipePhotoId.From(new Guid("2826cbdd-1372-4a41-b1cd-fb2896ead975"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000030")), RecipePhotoId.From(new Guid("7aef299a-de94-4dcc-ad9c-0ea9f5414a9e"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000031")), RecipePhotoId.From(new Guid("813f25f1-958b-47b0-ad0e-d157950491fe"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000032")), RecipePhotoId.From(new Guid("b3aa28c3-13dc-4571-b969-47a5f1341a34"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000033")), RecipePhotoId.From(new Guid("bc70ccaa-0a9a-4714-b53f-85bcf800045a"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000034")), RecipePhotoId.From(new Guid("4384def7-6e8f-45a8-97f3-db9acadeb427"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000035")), RecipePhotoId.From(new Guid("61f02301-6834-4031-a385-ae344fc96281"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000036")), RecipePhotoId.From(new Guid("87c14b90-ef37-4f22-9887-796da7880a78"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000037")), RecipePhotoId.From(new Guid("d02dbe22-a4fa-4a26-9e24-b484e7cf6b74"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000038")), RecipePhotoId.From(new Guid("c2f39d7e-adfd-4076-84cc-4480f96c9882"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000039")), RecipePhotoId.From(new Guid("704b5eae-059f-421a-a849-9d98aa7c4d3c"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000040")), RecipePhotoId.From(new Guid("96c6842a-e88f-4cb7-8122-56a28c00f89e"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000041")), RecipePhotoId.From(new Guid("774f3e0a-a704-4536-9bce-e373738f03c6"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000042")), RecipePhotoId.From(new Guid("c47ff729-b18c-4e60-b844-4bda19eb01ce"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000043")), RecipePhotoId.From(new Guid("2171fc71-b6b4-4990-b797-954a355ce9e7"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000044")), RecipePhotoId.From(new Guid("a98831dd-e84b-4c1d-a496-12365409cbb2"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000045")), RecipePhotoId.From(new Guid("3740e0a7-0bcf-48fa-bac9-fffd95e2e614"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000046")), RecipePhotoId.From(new Guid("dd31d8ae-4cad-45c4-abc2-c7e8f4d1a92c"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000047")), RecipePhotoId.From(new Guid("d918a084-bdaa-4345-99b1-6c2e64d0c46c"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000048")), RecipePhotoId.From(new Guid("f633ca29-45f5-4eee-a97f-d18524f332de"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000049")), RecipePhotoId.From(new Guid("dc1eaf14-5065-4a1b-86b7-3116356bfa4b"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000050")), RecipePhotoId.From(new Guid("80dad9cc-6181-47b1-87db-afc84e6fd3dc"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000051")), RecipePhotoId.From(new Guid("f69e18ab-9003-44a3-9d97-aded489bd835"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000052")), RecipePhotoId.From(new Guid("7230c353-54d1-401a-98ea-86646b6bbaa6"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000053")), RecipePhotoId.From(new Guid("0e2005ec-07ed-4b46-afe4-20920dc29698"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000054")), RecipePhotoId.From(new Guid("720e63eb-c844-4aa7-96a5-8d4bb4bdd216"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000055")), RecipePhotoId.From(new Guid("377f9275-d41d-4d3c-b3ef-8a8342c5272a"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000056")), RecipePhotoId.From(new Guid("d98dd06c-ab0d-46d4-90ca-31669eb31876"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000057")), RecipePhotoId.From(new Guid("02ecfe5d-77d4-45e6-828d-e7098698683b"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000058")), RecipePhotoId.From(new Guid("a46a51e9-5226-457e-8919-46d0610e0545"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000059")), RecipePhotoId.From(new Guid("a18f95ef-1107-4f8a-a57a-ed8fc6841631"))),
        (RecipeId.From(new Guid("11111111-0000-0000-0000-000000000060")), RecipePhotoId.From(new Guid("af4f69cf-d1fc-4287-9937-83a130bca3d0"))),
    ];

    // Вспомогательный метод для получения ID ингредиента по порядковому номеру
    private static IngredientId IngId(int n) =>
        IngredientId.From(new Guid($"22222222-0000-0000-0000-{n:D12}"));

    public static readonly Recipe[] Recipes =
    [
        // Борщ: свёкла(004), капуста(005), картофель(002), морковь(001), лук(003),
        //        томатная паста(052), говядина(014), сметана(025), соль(044), перец(045), лавровый лист(047)
        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000001")),
            "Борщ",
            "Классический украинский борщ со свёклой, капустой и томатной пастой. Подаётся со сметаной и чесночными пампушками.",
            120, Difficulty.Everyday, 6,
            "1. Сварить мясной бульон.\n2. Обжарить свёклу с томатной пастой.\n3. Добавить капусту и картофель.\n4. Соединить с бульоном, варить 20 минут.\n5. Подавать со сметаной.",
            [
                RecipeIngredient.Create(IngId(14), 500),
                RecipeIngredient.Create(IngId(4), 300),
                RecipeIngredient.Create(IngId(5), 300),
                RecipeIngredient.Create(IngId(2), 300),
                RecipeIngredient.Create(IngId(1), 150),
                RecipeIngredient.Create(IngId(3), 150),
                RecipeIngredient.Create(IngId(52), 50),
                RecipeIngredient.Create(IngId(25), 100),
                RecipeIngredient.Create(IngId(44), 10),
                RecipeIngredient.Create(IngId(45), 3),
                RecipeIngredient.Create(IngId(47), 2),
            ]),

        // Оливье: картофель(002), морковь(001), яйцо(027), колбаса варёная(019),
        //          майонез(053), соль(044), перец(045)
        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000002")),
            "Оливье",
            "Традиционный новогодний салат с варёными овощами, колбасой, яйцами и майонезом.",
            40, Difficulty.Easy, 8,
            "1. Отварить картофель, морковь, яйца.\n2. Нарезать кубиками все ингредиенты.\n3. Добавить горошек и колбасу.\n4. Заправить майонезом, посолить.",
            [
                RecipeIngredient.Create(IngId(2), 400),
                RecipeIngredient.Create(IngId(1), 200),
                RecipeIngredient.Create(IngId(27), 4),
                RecipeIngredient.Create(IngId(19), 300),
                RecipeIngredient.Create(IngId(53), 150),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(45), 2),
            ]),

        // Пельмени: мука(030), яйцо(027), фарш смешанный(018), лук(003), соль(044), перец(045)
        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000003")),
            "Пельмени",
            "Домашние пельмени из пресного теста с начинкой из смешанного фарша свинины и говядины.",
            90, Difficulty.Everyday, 4,
            "1. Замесить крутое тесто из муки, яйца и воды.\n2. Смешать фарш свинины и говядины с луком.\n3. Раскатать тесто, вырезать кружки.\n4. Слепить пельмени.\n5. Варить в подсоленной воде 7–8 минут.",
            [
                RecipeIngredient.Create(IngId(30), 400),
                RecipeIngredient.Create(IngId(27), 2),
                RecipeIngredient.Create(IngId(18), 500),
                RecipeIngredient.Create(IngId(3), 150),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(45), 3),
            ]),

        // Блины: молоко(023), яйцо(027), мука(030), масло сливочное(028), соль(044), сахар(046)
        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000004")),
            "Блины",
            "Тонкие русские блины на молоке. Подаются со сметаной, вареньем или икрой.",
            30, Difficulty.Easy, 4,
            "1. Смешать яйца, молоко, муку, соль и сахар.\n2. Добавить растопленное масло.\n3. Жарить на раскалённой сковороде с двух сторон.\n4. Подавать горячими.",
            [
                RecipeIngredient.Create(IngId(23), 500),
                RecipeIngredient.Create(IngId(27), 3),
                RecipeIngredient.Create(IngId(30), 200),
                RecipeIngredient.Create(IngId(28), 50),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(46), 20),
            ]),

        // Котлеты по-киевски: куриное филе(016), масло сливочное(028), петрушка(051),
        //                      мука(030), яйцо(027), панировочные сухари(056),
        //                      масло подсолнечное(042), соль(044), перец(045)
        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000005")),
            "Котлеты по-киевски",
            "Куриное филе, фаршированное сливочным маслом с зеленью, в хрустящей панировке.",
            50, Difficulty.Festive, 2,
            "1. Отбить куриное филе.\n2. Завернуть кусочек сливочного масла с зеленью.\n3. Обвалять в муке, яйце и панировочных сухарях.\n4. Жарить во фритюре 12–15 минут.",
            [
                RecipeIngredient.Create(IngId(16), 600),
                RecipeIngredient.Create(IngId(28), 100),
                RecipeIngredient.Create(IngId(51), 20),
                RecipeIngredient.Create(IngId(30), 50),
                RecipeIngredient.Create(IngId(27), 2),
                RecipeIngredient.Create(IngId(56), 100),
                RecipeIngredient.Create(IngId(42), 200),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(45), 2),
            ]),

        // Солянка: говядина(014), лук(003), томатная паста(052), огурец(007),
        //           лимон(012), соль(044), перец(045), лавровый лист(047)
        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000006")),
            "Солянка",
            "Густой кисло-солёный суп с мясным ассорти, солёными огурцами, оливками и лимоном.",
            60, Difficulty.Everyday, 6,
            "1. Обжарить мясное ассорти.\n2. Добавить лук, томатную пасту.\n3. Влить бульон, добавить солёные огурцы.\n4. Варить 20 минут.\n5. Подавать с оливками и лимоном.",
            [
                RecipeIngredient.Create(IngId(14), 400),
                RecipeIngredient.Create(IngId(3), 200),
                RecipeIngredient.Create(IngId(52), 60),
                RecipeIngredient.Create(IngId(7), 200),
                RecipeIngredient.Create(IngId(12), 1),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(45), 3),
                RecipeIngredient.Create(IngId(47), 2),
            ]),

        // Шашлык: свинина(015), лук(003), соль(044), перец(045), паприка(048)
        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000007")),
            "Шашлык",
            "Маринованная свинина на углях. Маринад — лук, уксус, специи. Подаётся с лавашом и свежими овощами.",
            180, Difficulty.Everyday, 6,
            "1. Нарезать свинину кусками 4–5 см.\n2. Замариновать с луком, уксусом и специями на 3–4 часа.\n3. Нанизать на шампуры.\n4. Жарить на углях 20–25 минут, переворачивая.",
            [
                RecipeIngredient.Create(IngId(15), 1500),
                RecipeIngredient.Create(IngId(3), 400),
                RecipeIngredient.Create(IngId(44), 15),
                RecipeIngredient.Create(IngId(45), 5),
                RecipeIngredient.Create(IngId(48), 10),
            ]),

        // Окрошка: картофель(002), яйцо(027), колбаса варёная(019), огурец(007),
        //           укроп(050), сметана(025), соль(044)
        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000008")),
            "Окрошка",
            "Холодный суп на квасе с варёными овощами, яйцами, колбасой и свежими огурцами.",
            30, Difficulty.Easy, 4,
            "1. Нарезать кубиками картофель, яйца, колбасу, огурцы.\n2. Добавить зелёный лук и укроп.\n3. Залить холодным квасом.\n4. Посолить, добавить сметану по вкусу.",
            [
                RecipeIngredient.Create(IngId(2), 300),
                RecipeIngredient.Create(IngId(27), 3),
                RecipeIngredient.Create(IngId(19), 200),
                RecipeIngredient.Create(IngId(7), 200),
                RecipeIngredient.Create(IngId(50), 30),
                RecipeIngredient.Create(IngId(25), 100),
                RecipeIngredient.Create(IngId(44), 5),
            ]),

        // Плов: рис(031), говядина(014), морковь(001), лук(003),
        //        масло подсолнечное(042), соль(044), куркума(049), лавровый лист(047)
        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000009")),
            "Плов",
            "Узбекский плов из баранины с рисом, морковью и луком, приготовленный в казане на открытом огне.",
            150, Difficulty.Restaurant, 8,
            "1. Раскалить масло в казане.\n2. Обжарить лук и баранину.\n3. Добавить морковь, специи (зира, барбарис).\n4. Залить водой, тушить 40 минут.\n5. Засыпать промытый рис, варить под крышкой 25 минут.",
            [
                RecipeIngredient.Create(IngId(31), 600),
                RecipeIngredient.Create(IngId(14), 700),
                RecipeIngredient.Create(IngId(1), 400),
                RecipeIngredient.Create(IngId(3), 300),
                RecipeIngredient.Create(IngId(42), 100),
                RecipeIngredient.Create(IngId(44), 15),
                RecipeIngredient.Create(IngId(49), 5),
                RecipeIngredient.Create(IngId(47), 2),
            ]),

        // Медовик: мука(030), яйцо(027), мёд(057), масло сливочное(028), сахар(046), сметана(025)
        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000010")),
            "Медовик",
            "Многослойный торт из тонких медовых коржей со сметанным кремом. Настаивается не менее 8 часов.",
            120, Difficulty.Festive, 10,
            "1. Растопить мёд с маслом и сахаром на водяной бане.\n2. Добавить яйца и соду, перемешать.\n3. Всыпать муку, замесить тесто.\n4. Раскатать и выпечь 8–10 коржей.\n5. Промазать сметанным кремом, дать настояться.",
            [
                RecipeIngredient.Create(IngId(30), 500),
                RecipeIngredient.Create(IngId(27), 3),
                RecipeIngredient.Create(IngId(57), 150),
                RecipeIngredient.Create(IngId(28), 100),
                RecipeIngredient.Create(IngId(46), 200),
                RecipeIngredient.Create(IngId(25), 500),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000011")),
            "Гречневая каша с грибами",
            "Рассыпчатая гречка, тушённая с лесными грибами и луком. Простое и сытное блюдо.",
            40, Difficulty.Easy, 4,
            "1. Промыть гречку, залить кипятком 1:2.\n2. Обжарить лук и грибы.\n3. Смешать с гречкой, тушить 15 минут.\n4. Посолить, добавить масло.",
            [
                RecipeIngredient.Create(IngId(32), 300),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(28), 30),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(45), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000012")),
            "Рассольник",
            "Суп с перловкой, солёными огурцами и говяжьими почками. Насыщенный кисловатый вкус.",
            90, Difficulty.Everyday, 6,
            "1. Сварить бульон из говядины.\n2. Добавить перловку, варить 30 минут.\n3. Добавить картофель и солёные огурцы.\n4. Заправить сметаной.",
            [
                RecipeIngredient.Create(IngId(14), 400),
                RecipeIngredient.Create(IngId(90), 100),
                RecipeIngredient.Create(IngId(2), 300),
                RecipeIngredient.Create(IngId(7), 200),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(25), 100),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(47), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000013")),
            "Щи из свежей капусты",
            "Лёгкий овощной суп из свежей капусты с картофелем и морковью.",
            60, Difficulty.Easy, 6,
            "1. Сварить бульон.\n2. Добавить нашинкованную капусту и картофель.\n3. Обжарить морковь и лук, добавить в суп.\n4. Варить 20 минут, посолить.",
            [
                RecipeIngredient.Create(IngId(5), 400),
                RecipeIngredient.Create(IngId(2), 300),
                RecipeIngredient.Create(IngId(1), 150),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(52), 30),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(47), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000014")),
            "Курица в сметанном соусе",
            "Нежные куриные бёдра, тушённые в густом сметанном соусе с чесноком и зеленью.",
            50, Difficulty.Easy, 4,
            "1. Обжарить куриные бёдра до золотистой корочки.\n2. Добавить лук и чеснок.\n3. Влить сметану, тушить 25 минут.\n4. Посыпать укропом.",
            [
                RecipeIngredient.Create(IngId(17), 800),
                RecipeIngredient.Create(IngId(25), 200),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(9), 3),
                RecipeIngredient.Create(IngId(50), 20),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(45), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000015")),
            "Вареники с картофелем",
            "Домашние вареники с картофельно-луковой начинкой. Подаются со сметаной и жареным луком.",
            80, Difficulty.Everyday, 4,
            "1. Замесить тесто из муки, яйца и воды.\n2. Отварить картофель, размять с жареным луком.\n3. Раскатать тесто, вырезать кружки.\n4. Слепить вареники, варить 5–7 минут.",
            [
                RecipeIngredient.Create(IngId(30), 400),
                RecipeIngredient.Create(IngId(27), 1),
                RecipeIngredient.Create(IngId(2), 500),
                RecipeIngredient.Create(IngId(3), 150),
                RecipeIngredient.Create(IngId(28), 50),
                RecipeIngredient.Create(IngId(25), 100),
                RecipeIngredient.Create(IngId(44), 8),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000016")),
            "Запечённый лосось",
            "Филе лосося, запечённое с лимоном, розмарином и оливковым маслом.",
            30, Difficulty.Easy, 2,
            "1. Смазать лосось оливковым маслом.\n2. Посыпать солью, перцем, розмарином.\n3. Выложить дольки лимона.\n4. Запекать при 200°C 20 минут.",
            [
                RecipeIngredient.Create(IngId(20), 600),
                RecipeIngredient.Create(IngId(43), 30),
                RecipeIngredient.Create(IngId(12), 1),
                RecipeIngredient.Create(IngId(102), 3),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(45), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000017")),
            "Греческий салат",
            "Классический салат с помидорами, огурцами, оливками, фетой и оливковым маслом.",
            15, Difficulty.Easy, 4,
            "1. Нарезать помидоры, огурцы, перец кубиками.\n2. Добавить оливки и фету.\n3. Заправить оливковым маслом, посолить.",
            [
                RecipeIngredient.Create(IngId(6), 300),
                RecipeIngredient.Create(IngId(7), 200),
                RecipeIngredient.Create(IngId(8), 150),
                RecipeIngredient.Create(IngId(87), 150),
                RecipeIngredient.Create(IngId(43), 40),
                RecipeIngredient.Create(IngId(44), 3),
                RecipeIngredient.Create(IngId(100), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000018")),
            "Тефтели в томатном соусе",
            "Сочные мясные шарики из смешанного фарша в густом томатном соусе.",
            60, Difficulty.Everyday, 4,
            "1. Смешать фарш с луком, яйцом и рисом.\n2. Сформировать тефтели.\n3. Обжарить, залить томатным соусом.\n4. Тушить 30 минут.",
            [
                RecipeIngredient.Create(IngId(18), 500),
                RecipeIngredient.Create(IngId(31), 50),
                RecipeIngredient.Create(IngId(27), 1),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(52), 80),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(45), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000019")),
            "Пицца Маргарита",
            "Классическая итальянская пицца с томатным соусом, моцареллой и базиликом.",
            60, Difficulty.Everyday, 4,
            "1. Замесить дрожжевое тесто.\n2. Раскатать, смазать томатным соусом.\n3. Выложить моцареллу.\n4. Запекать при 250°C 12–15 минут.\n5. Украсить базиликом.",
            [
                RecipeIngredient.Create(IngId(30), 300),
                RecipeIngredient.Create(IngId(86), 200),
                RecipeIngredient.Create(IngId(52), 80),
                RecipeIngredient.Create(IngId(43), 20),
                RecipeIngredient.Create(IngId(99), 3),
                RecipeIngredient.Create(IngId(44), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000020")),
            "Паста Карбонара",
            "Итальянская паста с беконом, яйцами, пармезаном и чёрным перцем.",
            25, Difficulty.Everyday, 2,
            "1. Отварить спагетти.\n2. Обжарить бекон.\n3. Смешать яйца с сыром.\n4. Соединить горячую пасту с беконом и яичной смесью.\n5. Посыпать перцем.",
            [
                RecipeIngredient.Create(IngId(34), 200),
                RecipeIngredient.Create(IngId(78), 150),
                RecipeIngredient.Create(IngId(27), 3),
                RecipeIngredient.Create(IngId(26), 80),
                RecipeIngredient.Create(IngId(45), 5),
                RecipeIngredient.Create(IngId(44), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000021")),
            "Манты",
            "Крупные паровые пельмени с бараниной и луком. Подаются со сметаной или томатным соусом.",
            120, Difficulty.Restaurant, 4,
            "1. Замесить крутое тесто.\n2. Смешать баранину с луком и специями.\n3. Раскатать тесто, нарезать квадратами.\n4. Слепить манты.\n5. Готовить на пару 40–45 минут.",
            [
                RecipeIngredient.Create(IngId(74), 600),
                RecipeIngredient.Create(IngId(3), 300),
                RecipeIngredient.Create(IngId(30), 400),
                RecipeIngredient.Create(IngId(44), 10),
                RecipeIngredient.Create(IngId(45), 5),
                RecipeIngredient.Create(IngId(95), 3),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000022")),
            "Рыбные котлеты",
            "Нежные котлеты из трески с луком и зеленью, обжаренные в панировке.",
            40, Difficulty.Easy, 4,
            "1. Пропустить треску через мясорубку.\n2. Добавить лук, яйцо, соль.\n3. Сформировать котлеты, обвалять в сухарях.\n4. Жарить на масле по 5 минут с каждой стороны.",
            [
                RecipeIngredient.Create(IngId(21), 600),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(27), 1),
                RecipeIngredient.Create(IngId(56), 80),
                RecipeIngredient.Create(IngId(42), 50),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(45), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000023")),
            "Запечённые овощи",
            "Ассорти из сезонных овощей, запечённых с оливковым маслом и прованскими травами.",
            45, Difficulty.Easy, 4,
            "1. Нарезать овощи крупными кусками.\n2. Смешать с маслом и специями.\n3. Выложить на противень.\n4. Запекать при 200°C 30–35 минут.",
            [
                RecipeIngredient.Create(IngId(58), 200),
                RecipeIngredient.Create(IngId(10), 200),
                RecipeIngredient.Create(IngId(8), 150),
                RecipeIngredient.Create(IngId(6), 200),
                RecipeIngredient.Create(IngId(43), 40),
                RecipeIngredient.Create(IngId(100), 3),
                RecipeIngredient.Create(IngId(101), 3),
                RecipeIngredient.Create(IngId(44), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000024")),
            "Творожная запеканка",
            "Нежная запеканка из творога с изюмом и ванилью. Подаётся со сметаной или вареньем.",
            60, Difficulty.Easy, 6,
            "1. Смешать творог с яйцами, сахаром и манкой.\n2. Добавить изюм.\n3. Выложить в форму, смазанную маслом.\n4. Запекать при 180°C 40 минут.",
            [
                RecipeIngredient.Create(IngId(29), 500),
                RecipeIngredient.Create(IngId(27), 3),
                RecipeIngredient.Create(IngId(46), 100),
                RecipeIngredient.Create(IngId(35), 50),
                RecipeIngredient.Create(IngId(28), 30),
                RecipeIngredient.Create(IngId(25), 100),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000025")),
            "Харчо",
            "Острый грузинский суп из говядины с рисом, грецкими орехами и кинзой.",
            90, Difficulty.Restaurant, 6,
            "1. Сварить говяжий бульон.\n2. Добавить рис и томатную пасту.\n3. Добавить грецкие орехи и чеснок.\n4. Заправить кинзой и хмели-сунели.",
            [
                RecipeIngredient.Create(IngId(14), 500),
                RecipeIngredient.Create(IngId(31), 100),
                RecipeIngredient.Create(IngId(52), 60),
                RecipeIngredient.Create(IngId(39), 80),
                RecipeIngredient.Create(IngId(9), 4),
                RecipeIngredient.Create(IngId(103), 30),
                RecipeIngredient.Create(IngId(44), 10),
                RecipeIngredient.Create(IngId(45), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000026")),
            "Сырники",
            "Пышные творожные оладьи, обжаренные до золотистой корочки. Подаются со сметаной.",
            30, Difficulty.Easy, 4,
            "1. Смешать творог с яйцом, сахаром и мукой.\n2. Сформировать сырники.\n3. Обвалять в муке.\n4. Жарить на масле по 3–4 минуты с каждой стороны.",
            [
                RecipeIngredient.Create(IngId(29), 400),
                RecipeIngredient.Create(IngId(27), 2),
                RecipeIngredient.Create(IngId(30), 80),
                RecipeIngredient.Create(IngId(46), 50),
                RecipeIngredient.Create(IngId(42), 40),
                RecipeIngredient.Create(IngId(25), 100),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000027")),
            "Куриный суп с лапшой",
            "Лёгкий куриный бульон с домашней лапшой, морковью и зеленью.",
            60, Difficulty.Easy, 6,
            "1. Сварить куриный бульон.\n2. Добавить морковь и лук.\n3. Засыпать лапшу, варить 10 минут.\n4. Посыпать укропом.",
            [
                RecipeIngredient.Create(IngId(16), 400),
                RecipeIngredient.Create(IngId(34), 150),
                RecipeIngredient.Create(IngId(1), 150),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(50), 20),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(47), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000028")),
            "Жаркое из свинины с картофелем",
            "Сытное жаркое из свинины с картофелем и луком, тушённое в горшочке.",
            90, Difficulty.Everyday, 4,
            "1. Нарезать свинину кусками, обжарить.\n2. Добавить лук и морковь.\n3. Выложить в горшочки с картофелем.\n4. Залить бульоном, запекать при 180°C 60 минут.",
            [
                RecipeIngredient.Create(IngId(15), 500),
                RecipeIngredient.Create(IngId(2), 600),
                RecipeIngredient.Create(IngId(3), 150),
                RecipeIngredient.Create(IngId(1), 100),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(45), 3),
                RecipeIngredient.Create(IngId(47), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000029")),
            "Фаршированный перец",
            "Болгарский перец, фаршированный смесью риса и мясного фарша в томатном соусе.",
            80, Difficulty.Everyday, 4,
            "1. Отварить рис до полуготовности.\n2. Смешать с фаршем и луком.\n3. Нафаршировать перцы.\n4. Тушить в томатном соусе 40 минут.",
            [
                RecipeIngredient.Create(IngId(8), 8),
                RecipeIngredient.Create(IngId(18), 400),
                RecipeIngredient.Create(IngId(31), 100),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(52), 80),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(45), 3),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000030")),
            "Уха",
            "Традиционная русская рыбная похлёбка из свежей рыбы с картофелем и зеленью.",
            50, Difficulty.Easy, 4,
            "1. Сварить рыбный бульон.\n2. Добавить картофель и морковь.\n3. Добавить рыбу, варить 15 минут.\n4. Посыпать укропом и петрушкой.",
            [
                RecipeIngredient.Create(IngId(20), 500),
                RecipeIngredient.Create(IngId(2), 300),
                RecipeIngredient.Create(IngId(1), 100),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(50), 20),
                RecipeIngredient.Create(IngId(51), 20),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(47), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000031")),
            "Чебуреки",
            "Хрустящие жареные пирожки из тонкого теста с сочной мясной начинкой.",
            90, Difficulty.Everyday, 4,
            "1. Замесить тесто из муки, воды и соли.\n2. Смешать фарш с луком и специями.\n3. Раскатать тесто, выложить начинку.\n4. Слепить полукруги, жарить во фритюре.",
            [
                RecipeIngredient.Create(IngId(30), 400),
                RecipeIngredient.Create(IngId(18), 400),
                RecipeIngredient.Create(IngId(3), 200),
                RecipeIngredient.Create(IngId(42), 200),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(45), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000032")),
            "Бефстроганов",
            "Нежная говядина в сметанном соусе с луком и горчицей. Подаётся с картофельным пюре.",
            60, Difficulty.Everyday, 4,
            "1. Нарезать говядину тонкими полосками.\n2. Обжарить с луком.\n3. Добавить сметану и горчицу.\n4. Тушить 20 минут.",
            [
                RecipeIngredient.Create(IngId(14), 600),
                RecipeIngredient.Create(IngId(3), 200),
                RecipeIngredient.Create(IngId(25), 200),
                RecipeIngredient.Create(IngId(105), 20),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(45), 3),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000033")),
            "Картофельное пюре",
            "Нежное картофельное пюре со сливочным маслом и горячим молоком.",
            30, Difficulty.Easy, 4,
            "1. Отварить картофель до мягкости.\n2. Слить воду, размять.\n3. Добавить горячее молоко и масло.\n4. Взбить до однородности, посолить.",
            [
                RecipeIngredient.Create(IngId(2), 800),
                RecipeIngredient.Create(IngId(23), 150),
                RecipeIngredient.Create(IngId(28), 80),
                RecipeIngredient.Create(IngId(44), 8),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000034")),
            "Голубцы",
            "Капустные листья, фаршированные рисом и мясом, тушённые в томатно-сметанном соусе.",
            120, Difficulty.Everyday, 6,
            "1. Отварить капустные листья.\n2. Смешать фарш с рисом и луком.\n3. Завернуть начинку в листья.\n4. Тушить в соусе из томатной пасты и сметаны 60 минут.",
            [
                RecipeIngredient.Create(IngId(5), 1),
                RecipeIngredient.Create(IngId(18), 400),
                RecipeIngredient.Create(IngId(31), 100),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(52), 60),
                RecipeIngredient.Create(IngId(25), 100),
                RecipeIngredient.Create(IngId(44), 8),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000035")),
            "Пирог с яблоками",
            "Воздушный яблочный пирог на кефире. Простой рецепт для домашней выпечки.",
            60, Difficulty.Easy, 8,
            "1. Смешать яйца с сахаром.\n2. Добавить кефир, муку и соду.\n3. Нарезать яблоки, добавить в тесто.\n4. Выпекать при 180°C 40 минут.",
            [
                RecipeIngredient.Create(IngId(30), 300),
                RecipeIngredient.Create(IngId(27), 3),
                RecipeIngredient.Create(IngId(46), 150),
                RecipeIngredient.Create(IngId(84), 200),
                RecipeIngredient.Create(IngId(11), 400),
                RecipeIngredient.Create(IngId(28), 50),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000036")),
            "Тыквенный суп-пюре",
            "Нежный крем-суп из тыквы со сливками и имбирём. Согревающее осеннее блюдо.",
            45, Difficulty.Easy, 4,
            "1. Запечь тыкву при 200°C 30 минут.\n2. Обжарить лук и имбирь.\n3. Смешать всё в блендере.\n4. Добавить сливки, прогреть.",
            [
                RecipeIngredient.Create(IngId(59), 800),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(97), 5),
                RecipeIngredient.Create(IngId(88), 150),
                RecipeIngredient.Create(IngId(43), 30),
                RecipeIngredient.Create(IngId(44), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000037")),
            "Котлеты из индейки",
            "Диетические котлеты из фарша индейки с луком и зеленью.",
            40, Difficulty.Easy, 4,
            "1. Пропустить индейку через мясорубку.\n2. Добавить лук, яйцо, соль.\n3. Сформировать котлеты.\n4. Жарить на масле по 5 минут с каждой стороны.",
            [
                RecipeIngredient.Create(IngId(76), 600),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(27), 1),
                RecipeIngredient.Create(IngId(50), 20),
                RecipeIngredient.Create(IngId(42), 40),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(45), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000038")),
            "Ризотто с грибами",
            "Кремовое итальянское ризотто с лесными грибами, пармезаном и белым вином.",
            45, Difficulty.Restaurant, 4,
            "1. Обжарить лук на оливковом масле.\n2. Добавить рис, обжарить 2 минуты.\n3. Постепенно добавлять бульон, помешивая.\n4. Добавить грибы и сыр.",
            [
                RecipeIngredient.Create(IngId(31), 300),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(26), 80),
                RecipeIngredient.Create(IngId(43), 40),
                RecipeIngredient.Create(IngId(28), 30),
                RecipeIngredient.Create(IngId(44), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000039")),
            "Чечевичный суп",
            "Сытный суп из красной чечевицы с морковью, луком и специями.",
            40, Difficulty.Easy, 4,
            "1. Обжарить лук и морковь.\n2. Добавить чечевицу и воду.\n3. Варить 25 минут.\n4. Добавить куркуму и паприку.",
            [
                RecipeIngredient.Create(IngId(38), 300),
                RecipeIngredient.Create(IngId(1), 150),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(49), 3),
                RecipeIngredient.Create(IngId(48), 3),
                RecipeIngredient.Create(IngId(43), 30),
                RecipeIngredient.Create(IngId(44), 8),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000040")),
            "Омлет с овощами",
            "Пышный омлет с болгарским перцем, помидорами и зеленью.",
            20, Difficulty.Easy, 2,
            "1. Взбить яйца с молоком.\n2. Обжарить перец и помидоры.\n3. Залить яичной смесью.\n4. Готовить под крышкой 5–7 минут.",
            [
                RecipeIngredient.Create(IngId(27), 4),
                RecipeIngredient.Create(IngId(23), 50),
                RecipeIngredient.Create(IngId(8), 100),
                RecipeIngredient.Create(IngId(6), 100),
                RecipeIngredient.Create(IngId(50), 10),
                RecipeIngredient.Create(IngId(28), 20),
                RecipeIngredient.Create(IngId(44), 3),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000041")),
            "Баклажаны по-грузински",
            "Жареные баклажаны с ореховой начинкой, чесноком и кинзой.",
            40, Difficulty.Everyday, 4,
            "1. Нарезать баклажаны пластинами, посолить.\n2. Обжарить на масле.\n3. Смешать орехи с чесноком и кинзой.\n4. Завернуть начинку в баклажаны.",
            [
                RecipeIngredient.Create(IngId(58), 600),
                RecipeIngredient.Create(IngId(39), 100),
                RecipeIngredient.Create(IngId(9), 3),
                RecipeIngredient.Create(IngId(103), 30),
                RecipeIngredient.Create(IngId(42), 60),
                RecipeIngredient.Create(IngId(44), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000042")),
            "Пирожки с капустой",
            "Мягкие дрожжевые пирожки с тушёной капустой и яйцом.",
            120, Difficulty.Everyday, 8,
            "1. Замесить дрожжевое тесто.\n2. Потушить капусту с луком.\n3. Добавить варёные яйца.\n4. Сформировать пирожки, выпекать при 180°C 25 минут.",
            [
                RecipeIngredient.Create(IngId(30), 500),
                RecipeIngredient.Create(IngId(5), 400),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(27), 3),
                RecipeIngredient.Create(IngId(28), 50),
                RecipeIngredient.Create(IngId(44), 8),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000043")),
            "Стейк из говядины",
            "Сочный стейк рибай средней прожарки с розмарином и чесночным маслом.",
            20, Difficulty.Restaurant, 2,
            "1. Довести стейк до комнатной температуры.\n2. Посолить и поперчить.\n3. Жарить на раскалённой сковороде по 3–4 минуты с каждой стороны.\n4. Дать отдохнуть 5 минут.",
            [
                RecipeIngredient.Create(IngId(14), 600),
                RecipeIngredient.Create(IngId(28), 50),
                RecipeIngredient.Create(IngId(9), 2),
                RecipeIngredient.Create(IngId(102), 3),
                RecipeIngredient.Create(IngId(44), 5),
                RecipeIngredient.Create(IngId(45), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000044")),
            "Морковный торт",
            "Влажный морковный торт со сливочным кремом из творожного сыра.",
            90, Difficulty.Festive, 10,
            "1. Натереть морковь, смешать с яйцами, сахаром и маслом.\n2. Добавить муку, корицу и орехи.\n3. Выпекать при 180°C 40 минут.\n4. Покрыть кремом из творожного сыра.",
            [
                RecipeIngredient.Create(IngId(1), 300),
                RecipeIngredient.Create(IngId(30), 250),
                RecipeIngredient.Create(IngId(27), 3),
                RecipeIngredient.Create(IngId(46), 200),
                RecipeIngredient.Create(IngId(96), 5),
                RecipeIngredient.Create(IngId(39), 80),
                RecipeIngredient.Create(IngId(29), 300),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000045")),
            "Суп из чечевицы с копчёностями",
            "Густой суп из зелёной чечевицы с копчёной колбасой и овощами.",
            50, Difficulty.Everyday, 6,
            "1. Обжарить лук, морковь и колбасу.\n2. Добавить чечевицу и воду.\n3. Варить 30 минут.\n4. Добавить томатную пасту, посолить.",
            [
                RecipeIngredient.Create(IngId(38), 300),
                RecipeIngredient.Create(IngId(19), 200),
                RecipeIngredient.Create(IngId(1), 150),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(52), 40),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(47), 2),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000046")),
            "Курица терияки",
            "Куриное филе в сладко-солёном соусе терияки с кунжутом и зелёным луком.",
            30, Difficulty.Easy, 2,
            "1. Нарезать куриное филе.\n2. Обжарить на масле.\n3. Добавить соевый соус, мёд и имбирь.\n4. Тушить 10 минут.\n5. Посыпать кунжутом.",
            [
                RecipeIngredient.Create(IngId(16), 400),
                RecipeIngredient.Create(IngId(54), 60),
                RecipeIngredient.Create(IngId(57), 30),
                RecipeIngredient.Create(IngId(97), 5),
                RecipeIngredient.Create(IngId(40), 20),
                RecipeIngredient.Create(IngId(42), 30),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000047")),
            "Пшённая каша с тыквой",
            "Сладкая пшённая каша с тыквой на молоке. Полезный завтрак.",
            40, Difficulty.Easy, 4,
            "1. Нарезать тыкву кубиками, потушить.\n2. Промыть пшено, залить молоком.\n3. Варить 20 минут, помешивая.\n4. Добавить тыкву, сахар и масло.",
            [
                RecipeIngredient.Create(IngId(91), 200),
                RecipeIngredient.Create(IngId(59), 300),
                RecipeIngredient.Create(IngId(23), 500),
                RecipeIngredient.Create(IngId(46), 40),
                RecipeIngredient.Create(IngId(28), 30),
                RecipeIngredient.Create(IngId(44), 3),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000048")),
            "Сельдь под шубой",
            "Слоёный салат с сельдью, свёклой, морковью, картофелем и майонезом.",
            60, Difficulty.Easy, 8,
            "1. Отварить свёклу, морковь, картофель.\n2. Натереть на тёрке.\n3. Выложить слоями: сельдь, картофель, морковь, свёкла.\n4. Каждый слой смазать майонезом.",
            [
                RecipeIngredient.Create(IngId(80), 300),
                RecipeIngredient.Create(IngId(4), 300),
                RecipeIngredient.Create(IngId(1), 200),
                RecipeIngredient.Create(IngId(2), 300),
                RecipeIngredient.Create(IngId(27), 3),
                RecipeIngredient.Create(IngId(53), 200),
                RecipeIngredient.Create(IngId(44), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000049")),
            "Брускетта с томатами",
            "Хрустящие тосты с помидорами, базиликом и оливковым маслом.",
            15, Difficulty.Easy, 4,
            "1. Поджарить хлеб на гриле.\n2. Натереть чесноком.\n3. Выложить нарезанные помидоры.\n4. Полить оливковым маслом, посыпать базиликом.",
            [
                RecipeIngredient.Create(IngId(55), 200),
                RecipeIngredient.Create(IngId(6), 300),
                RecipeIngredient.Create(IngId(9), 2),
                RecipeIngredient.Create(IngId(43), 40),
                RecipeIngredient.Create(IngId(99), 5),
                RecipeIngredient.Create(IngId(44), 3),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000050")),
            "Запечённая утка с яблоками",
            "Целая утка, запечённая с яблоками и апельсинами. Праздничное блюдо.",
            180, Difficulty.Festive, 6,
            "1. Натереть утку солью, перцем и специями.\n2. Нафаршировать яблоками и апельсинами.\n3. Запекать при 180°C 2,5 часа.\n4. Поливать выделившимся соком каждые 30 минут.",
            [
                RecipeIngredient.Create(IngId(75), 2000),
                RecipeIngredient.Create(IngId(11), 400),
                RecipeIngredient.Create(IngId(68), 2),
                RecipeIngredient.Create(IngId(57), 30),
                RecipeIngredient.Create(IngId(44), 15),
                RecipeIngredient.Create(IngId(45), 5),
                RecipeIngredient.Create(IngId(96), 3),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000051")),
            "Паэлья с морепродуктами",
            "Испанская паэлья с креветками, мидиями и кальмарами на шафрановом рисе.",
            60, Difficulty.Restaurant, 4,
            "1. Обжарить лук и чеснок на оливковом масле.\n2. Добавить рис, обжарить.\n3. Влить бульон с шафраном.\n4. Добавить морепродукты, готовить 20 минут.",
            [
                RecipeIngredient.Create(IngId(31), 300),
                RecipeIngredient.Create(IngId(22), 300),
                RecipeIngredient.Create(IngId(81), 200),
                RecipeIngredient.Create(IngId(82), 200),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(9), 3),
                RecipeIngredient.Create(IngId(43), 60),
                RecipeIngredient.Create(IngId(44), 8),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000052")),
            "Кролик в вине",
            "Нежный кролик, тушённый в белом вине с розмарином и чесноком.",
            120, Difficulty.Restaurant, 4,
            "1. Нарезать кролика порционными кусками.\n2. Обжарить до золотистой корочки.\n3. Добавить чеснок, розмарин и вино.\n4. Тушить 60 минут.",
            [
                RecipeIngredient.Create(IngId(77), 1500),
                RecipeIngredient.Create(IngId(9), 4),
                RecipeIngredient.Create(IngId(102), 5),
                RecipeIngredient.Create(IngId(43), 60),
                RecipeIngredient.Create(IngId(44), 10),
                RecipeIngredient.Create(IngId(45), 3),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000053")),
            "Овощное рагу",
            "Лёгкое летнее рагу из кабачков, баклажанов, помидоров и перца.",
            50, Difficulty.Easy, 4,
            "1. Нарезать все овощи кубиками.\n2. Обжарить лук и чеснок.\n3. Добавить остальные овощи.\n4. Тушить 30 минут, посолить.",
            [
                RecipeIngredient.Create(IngId(10), 300),
                RecipeIngredient.Create(IngId(58), 300),
                RecipeIngredient.Create(IngId(6), 300),
                RecipeIngredient.Create(IngId(8), 200),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(9), 2),
                RecipeIngredient.Create(IngId(43), 40),
                RecipeIngredient.Create(IngId(44), 8),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000054")),
            "Тирамису",
            "Классический итальянский десерт из савоярди, маскарпоне и кофе.",
            30, Difficulty.Festive, 8,
            "1. Взбить желтки с сахаром.\n2. Добавить маскарпоне.\n3. Обмакнуть савоярди в кофе.\n4. Выложить слоями, посыпать какао.\n5. Охладить 4 часа.",
            [
                RecipeIngredient.Create(IngId(27), 4),
                RecipeIngredient.Create(IngId(29), 500),
                RecipeIngredient.Create(IngId(46), 100),
                RecipeIngredient.Create(IngId(88), 200),
                RecipeIngredient.Create(IngId(55), 200),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000055")),
            "Минестроне",
            "Итальянский овощной суп с пастой, фасолью и сезонными овощами.",
            60, Difficulty.Easy, 6,
            "1. Обжарить лук, морковь, сельдерей.\n2. Добавить помидоры и фасоль.\n3. Влить бульон, добавить пасту.\n4. Варить 15 минут, посыпать пармезаном.",
            [
                RecipeIngredient.Create(IngId(36), 200),
                RecipeIngredient.Create(IngId(34), 100),
                RecipeIngredient.Create(IngId(6), 200),
                RecipeIngredient.Create(IngId(1), 150),
                RecipeIngredient.Create(IngId(3), 100),
                RecipeIngredient.Create(IngId(63), 100),
                RecipeIngredient.Create(IngId(26), 50),
                RecipeIngredient.Create(IngId(43), 40),
                RecipeIngredient.Create(IngId(44), 8),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000056")),
            "Шаурма",
            "Домашняя шаурма с куриным мясом, овощами и чесночным соусом в лаваше.",
            40, Difficulty.Easy, 4,
            "1. Замариновать курицу в специях.\n2. Обжарить до готовности.\n3. Нарезать тонкими полосками.\n4. Завернуть в лаваш с овощами и соусом.",
            [
                RecipeIngredient.Create(IngId(16), 500),
                RecipeIngredient.Create(IngId(6), 200),
                RecipeIngredient.Create(IngId(7), 150),
                RecipeIngredient.Create(IngId(25), 100),
                RecipeIngredient.Create(IngId(9), 3),
                RecipeIngredient.Create(IngId(48), 5),
                RecipeIngredient.Create(IngId(44), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000057")),
            "Форель на гриле",
            "Целая форель, приготовленная на гриле с лимоном и свежими травами.",
            30, Difficulty.Easy, 2,
            "1. Очистить форель, сделать надрезы.\n2. Натереть солью, перцем и травами.\n3. Вложить дольки лимона.\n4. Жарить на гриле по 8–10 минут с каждой стороны.",
            [
                RecipeIngredient.Create(IngId(20), 800),
                RecipeIngredient.Create(IngId(12), 1),
                RecipeIngredient.Create(IngId(50), 20),
                RecipeIngredient.Create(IngId(51), 20),
                RecipeIngredient.Create(IngId(43), 30),
                RecipeIngredient.Create(IngId(44), 8),
                RecipeIngredient.Create(IngId(45), 3),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000058")),
            "Пирог с вишней",
            "Открытый пирог из песочного теста с вишнёвой начинкой.",
            80, Difficulty.Everyday, 8,
            "1. Замесить песочное тесто.\n2. Выложить в форму, сделать бортики.\n3. Засыпать вишню с сахаром.\n4. Выпекать при 180°C 40 минут.",
            [
                RecipeIngredient.Create(IngId(30), 300),
                RecipeIngredient.Create(IngId(28), 150),
                RecipeIngredient.Create(IngId(27), 2),
                RecipeIngredient.Create(IngId(46), 150),
                RecipeIngredient.Create(IngId(71), 500),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000059")),
            "Суши роллы",
            "Домашние роллы с лососем, огурцом и авокадо в нори.",
            60, Difficulty.Restaurant, 4,
            "1. Сварить рис для суши.\n2. Выложить рис на нори.\n3. Добавить начинку из лосося и огурца.\n4. Свернуть в рулет, нарезать.",
            [
                RecipeIngredient.Create(IngId(31), 300),
                RecipeIngredient.Create(IngId(20), 200),
                RecipeIngredient.Create(IngId(7), 100),
                RecipeIngredient.Create(IngId(54), 40),
                RecipeIngredient.Create(IngId(106), 30),
                RecipeIngredient.Create(IngId(44), 5),
            ]),

        Recipe.Create(
            RecipeId.From(new Guid("11111111-0000-0000-0000-000000000060")),
            "Торт Наполеон",
            "Классический торт из слоёного теста с нежным заварным кремом.",
            180, Difficulty.Signature, 12,
            "1. Приготовить слоёное тесто, раскатать и выпечь 10–12 коржей.\n2. Сварить заварной крем из молока, яиц и сахара.\n3. Промазать коржи кремом.\n4. Посыпать крошкой, дать настояться 8 часов.",
            [
                RecipeIngredient.Create(IngId(30), 600),
                RecipeIngredient.Create(IngId(28), 300),
                RecipeIngredient.Create(IngId(27), 4),
                RecipeIngredient.Create(IngId(23), 700),
                RecipeIngredient.Create(IngId(46), 250),
                RecipeIngredient.Create(IngId(35), 50),
            ]),
    ];
}
