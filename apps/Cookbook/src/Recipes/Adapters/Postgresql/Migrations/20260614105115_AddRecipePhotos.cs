using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Adapters.Postgresql.Migrations;

/// <inheritdoc />
public partial class AddRecipePhotos : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>(
            name: "photo_id",
            schema: "cookbook",
            table: "recipes",
            type: "uuid",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "recipe_photos",
            schema: "cookbook",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                original_data = table.Column<byte[]>(type: "bytea", nullable: false),
                thumbnail_data = table.Column<byte[]>(type: "bytea", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_recipe_photos", x => x.id);
                table.ForeignKey(
                    name: "FK_recipe_photos_recipes_recipe_id",
                    column: x => x.recipe_id,
                    principalSchema: "cookbook",
                    principalTable: "recipes",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_recipe_photos_recipe_id",
            schema: "cookbook",
            table: "recipe_photos",
            column: "recipe_id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "recipe_photos",
            schema: "cookbook");

        migrationBuilder.DropColumn(
            name: "photo_id",
            schema: "cookbook",
            table: "recipes");
    }
}
