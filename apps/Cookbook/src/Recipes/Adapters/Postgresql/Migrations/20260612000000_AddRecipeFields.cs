using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Recipes.Adapters.Postgresql.Migrations;

public partial class AddRecipeFields : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "title",
            schema: "cookbook",
            table: "recipes",
            type: "character varying(200)",
            maxLength: 200,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(2000)",
            oldMaxLength: 2000);

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "cookbook",
            table: "recipes",
            type: "character varying(2000)",
            maxLength: 2000,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");

        migrationBuilder.AddColumn<int>(
            name: "cooking_time",
            schema: "cookbook",
            table: "recipes",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "difficulty",
            schema: "cookbook",
            table: "recipes",
            type: "character varying(20)",
            maxLength: 20,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "instructions",
            schema: "cookbook",
            table: "recipes",
            type: "character varying(10000)",
            maxLength: 10000,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "servings",
            schema: "cookbook",
            table: "recipes",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.InsertData(
            schema: "cookbook",
            table: "recipes",
            columns: new[] { "id", "cooking_time", "description", "difficulty", "instructions", "servings", "title" },
            values: new object[,]
            {
                { new Guid("11111111-0000-0000-0000-000000000001"), 120, "Классический украинский борщ со свёклой, капустой и томатной пастой. Подаётся со сметаной и чесночными пампушками.", "everyday", "1. Сварить мясной бульон.\n2. Обжарить свёклу с томатной пастой.\n3. Добавить капусту и картофель.\n4. Соединить с бульоном, варить 20 минут.\n5. Подавать со сметаной.", 6, "Борщ" },
                { new Guid("11111111-0000-0000-0000-000000000002"), 40, "Традиционный новогодний салат с варёными овощами, колбасой, яйцами и майонезом.", "easy", "1. Отварить картофель, морковь, яйца.\n2. Нарезать кубиками все ингредиенты.\n3. Добавить горошек и колбасу.\n4. Заправить майонезом, посолить.", 8, "Оливье" },
                { new Guid("11111111-0000-0000-0000-000000000003"), 90, "Домашние пельмени из пресного теста с начинкой из смешанного фарша свинины и говядины.", "everyday", "1. Замесить крутое тесто из муки, яйца и воды.\n2. Смешать фарш свинины и говядины с луком.\n3. Раскатать тесто, вырезать кружки.\n4. Слепить пельмени.\n5. Варить в подсоленной воде 7–8 минут.", 4, "Пельмени" },
                { new Guid("11111111-0000-0000-0000-000000000004"), 30, "Тонкие русские блины на молоке. Подаются со сметаной, вареньем или икрой.", "easy", "1. Смешать яйца, молоко, муку, соль и сахар.\n2. Добавить растопленное масло.\n3. Жарить на раскалённой сковороде с двух сторон.\n4. Подавать горячими.", 4, "Блины" },
                { new Guid("11111111-0000-0000-0000-000000000005"), 50, "Куриное филе, фаршированное сливочным маслом с зеленью, в хрустящей панировке.", "festive", "1. Отбить куриное филе.\n2. Завернуть кусочек сливочного масла с зеленью.\n3. Обвалять в муке, яйце и панировочных сухарях.\n4. Жарить во фритюре 12–15 минут.", 2, "Котлеты по-киевски" },
                { new Guid("11111111-0000-0000-0000-000000000006"), 60, "Густой кисло-солёный суп с мясным ассорти, солёными огурцами, оливками и лимоном.", "everyday", "1. Обжарить мясное ассорти.\n2. Добавить лук, томатную пасту.\n3. Влить бульон, добавить солёные огурцы.\n4. Варить 20 минут.\n5. Подавать с оливками и лимоном.", 6, "Солянка" },
                { new Guid("11111111-0000-0000-0000-000000000007"), 180, "Маринованная свинина на углях. Маринад — лук, уксус, специи. Подаётся с лавашом и свежими овощами.", "everyday", "1. Нарезать свинину кусками 4–5 см.\n2. Замариновать с луком, уксусом и специями на 3–4 часа.\n3. Нанизать на шампуры.\n4. Жарить на углях 20–25 минут, переворачивая.", 6, "Шашлык" },
                { new Guid("11111111-0000-0000-0000-000000000008"), 30, "Холодный суп на квасе с варёными овощами, яйцами, колбасой и свежими огурцами.", "easy", "1. Нарезать кубиками картофель, яйца, колбасу, огурцы.\n2. Добавить зелёный лук и укроп.\n3. Залить холодным квасом.\n4. Посолить, добавить сметану по вкусу.", 4, "Окрошка" },
                { new Guid("11111111-0000-0000-0000-000000000009"), 150, "Узбекский плов из баранины с рисом, морковью и луком, приготовленный в казане на открытом огне.", "restaurant", "1. Раскалить масло в казане.\n2. Обжарить лук и баранину.\n3. Добавить морковь, специи (зира, барбарис).\n4. Залить водой, тушить 40 минут.\n5. Засыпать промытый рис, варить под крышкой 25 минут.", 8, "Плов" },
                { new Guid("11111111-0000-0000-0000-000000000010"), 120, "Многослойный торт из тонких медовых коржей со сметанным кремом. Настаивается не менее 8 часов.", "festive", "1. Растопить мёд с маслом и сахаром на водяной бане.\n2. Добавить яйца и соду, перемешать.\n3. Всыпать муку, замесить тесто.\n4. Раскатать и выпечь 8–10 коржей.\n5. Промазать сметанным кремом, дать настояться.", 10, "Медовик" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            schema: "cookbook",
            table: "recipes",
            keyColumn: "id",
            keyValues: new object[]
            {
                new Guid("11111111-0000-0000-0000-000000000001"),
                new Guid("11111111-0000-0000-0000-000000000002"),
                new Guid("11111111-0000-0000-0000-000000000003"),
                new Guid("11111111-0000-0000-0000-000000000004"),
                new Guid("11111111-0000-0000-0000-000000000005"),
                new Guid("11111111-0000-0000-0000-000000000006"),
                new Guid("11111111-0000-0000-0000-000000000007"),
                new Guid("11111111-0000-0000-0000-000000000008"),
                new Guid("11111111-0000-0000-0000-000000000009"),
                new Guid("11111111-0000-0000-0000-000000000010")
            });

        migrationBuilder.DropColumn(
            name: "cooking_time",
            schema: "cookbook",
            table: "recipes");

        migrationBuilder.DropColumn(
            name: "difficulty",
            schema: "cookbook",
            table: "recipes");

        migrationBuilder.DropColumn(
            name: "instructions",
            schema: "cookbook",
            table: "recipes");

        migrationBuilder.DropColumn(
            name: "servings",
            schema: "cookbook",
            table: "recipes");

        migrationBuilder.AlterColumn<string>(
            name: "title",
            schema: "cookbook",
            table: "recipes",
            type: "character varying(2000)",
            maxLength: 2000,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(200)",
            oldMaxLength: 200);

        migrationBuilder.AlterColumn<string>(
            name: "description",
            schema: "cookbook",
            table: "recipes",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(2000)",
            oldMaxLength: 2000);
    }
}
