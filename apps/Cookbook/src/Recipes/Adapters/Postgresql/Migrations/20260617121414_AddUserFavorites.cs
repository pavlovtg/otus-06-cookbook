using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Adapters.Postgresql.Migrations;

/// <inheritdoc />
public partial class AddUserFavorites : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "user_favorites",
            schema: "cookbook",
            columns: table => new
            {
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_user_favorites", x => new { x.user_id, x.recipe_id });
                table.ForeignKey(
                    name: "FK_user_favorites_recipes_recipe_id",
                    column: x => x.recipe_id,
                    principalSchema: "cookbook",
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_user_favorites_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "cookbook",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_user_favorites_recipe_id",
            schema: "cookbook",
            table: "user_favorites",
            column: "recipe_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "user_favorites",
            schema: "cookbook");
    }
}
