using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Adapters.Postgresql.Migrations;

/// <inheritdoc />
public partial class AddRecipeComments : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "recipe_comments",
            schema: "cookbook",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                author_id = table.Column<Guid>(type: "uuid", nullable: false),
                text = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_recipe_comments", x => x.id);
                table.ForeignKey(
                    name: "FK_recipe_comments_recipes_recipe_id",
                    column: x => x.recipe_id,
                    principalSchema: "cookbook",
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_recipe_comments_users_author_id",
                    column: x => x.author_id,
                    principalSchema: "cookbook",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_recipe_comments_author_id",
            schema: "cookbook",
            table: "recipe_comments",
            column: "author_id");

        migrationBuilder.CreateIndex(
            name: "IX_recipe_comments_recipe_id_author_id",
            schema: "cookbook",
            table: "recipe_comments",
            columns: new[] { "recipe_id", "author_id" },
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "recipe_comments",
            schema: "cookbook");
    }
}
