using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Adapters.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "average_rating",
                schema: "cookbook",
                table: "recipes",
                type: "real",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "recipe_ratings",
                schema: "cookbook",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recipe_ratings", x => new { x.user_id, x.recipe_id });
                    table.ForeignKey(
                        name: "FK_recipe_ratings_recipes_recipe_id",
                        column: x => x.recipe_id,
                        principalSchema: "cookbook",
                        principalTable: "recipes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recipe_ratings_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "cookbook",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_recipe_ratings_recipe_id",
                schema: "cookbook",
                table: "recipe_ratings",
                column: "recipe_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "recipe_ratings",
                schema: "cookbook");

            migrationBuilder.DropColumn(
                name: "average_rating",
                schema: "cookbook",
                table: "recipes");
        }
    }
}
