using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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

    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
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
