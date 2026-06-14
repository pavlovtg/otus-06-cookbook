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
    ];
}
