using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Adapters.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeAuthorFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "author_id",
                schema: "cookbook",
                table: "recipes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_public",
                schema: "cookbook",
                table: "recipes",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "author_id",
                schema: "cookbook",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "is_public",
                schema: "cookbook",
                table: "recipes");
        }
    }
}
