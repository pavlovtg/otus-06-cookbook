using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Recipes.Adapters.Postgresql.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "cookbook");

        migrationBuilder.CreateTable(
            name: "recipes",
            schema: "cookbook",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                description = table.Column<string>(type: "text", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_recipes", x => x.id);
            });

        migrationBuilder.InsertData(
            schema: "cookbook",
            table: "recipes",
            columns: new[] { "id", "description", "title" },
            values: new object[,]
            {
                { new Guid("11111111-0000-0000-0000-000000000001"), "Классический украинский борщ со свёклой, капустой и томатной пастой. Подаётся со сметаной и чесночными пампушками.", "Борщ" },
                { new Guid("11111111-0000-0000-0000-000000000002"), "Традиционный новогодний салат с варёными овощами, колбасой, яйцами и майонезом.", "Оливье" },
                { new Guid("11111111-0000-0000-0000-000000000003"), "Домашние пельмени из пресного теста с начинкой из смешанного фарша свинины и говядины.", "Пельмени" },
                { new Guid("11111111-0000-0000-0000-000000000004"), "Тонкие русские блины на молоке. Подаются со сметаной, вареньем или икрой.", "Блины" },
                { new Guid("11111111-0000-0000-0000-000000000005"), "Куриное филе, фаршированное сливочным маслом с зеленью, в хрустящей панировке.", "Котлеты по-киевски" },
                { new Guid("11111111-0000-0000-0000-000000000006"), "Густой кисло-солёный суп с мясным ассорти, солёными огурцами, оливками и лимоном.", "Солянка" },
                { new Guid("11111111-0000-0000-0000-000000000007"), "Маринованная свинина на углях. Маринад — лук, уксус, специи. Подаётся с лавашом и свежими овощами.", "Шашлык" },
                { new Guid("11111111-0000-0000-0000-000000000008"), "Холодный суп на квасе с варёными овощами, яйцами, колбасой и свежими огурцами.", "Окрошка" },
                { new Guid("11111111-0000-0000-0000-000000000009"), "Узбекский плов из баранины с рисом, морковью и луком, приготовленный в казане на открытом огне.", "Плов" },
                { new Guid("11111111-0000-0000-0000-000000000010"), "Многослойный торт из тонких медовых коржей со сметанным кремом. Настаивается не менее 8 часов.", "Медовик" }
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "recipes",
            schema: "cookbook");
    }
}
