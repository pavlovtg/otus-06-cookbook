using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Adapters.Postgresql.Migrations;

/// <inheritdoc />
public partial class AddRecipeIngredients : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "recipe_ingredients",
            schema: "cookbook",
            columns: table => new
            {
                ingredient_id = table.Column<Guid>(type: "uuid", nullable: false),
                RecipeId = table.Column<Guid>(type: "uuid", nullable: false),
                amount = table.Column<decimal>(type: "numeric(12,3)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_recipe_ingredients", x => new { x.RecipeId, x.ingredient_id });
                table.ForeignKey(
                    name: "FK_recipe_ingredients_recipes_RecipeId",
                    column: x => x.RecipeId,
                    principalSchema: "cookbook",
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "recipe_ingredients",
            schema: "cookbook");
    }
}
