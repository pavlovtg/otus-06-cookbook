using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipes.Adapters.Postgresql.Migrations;

/// <inheritdoc />
public partial class AddMealPlan : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "meal_plans",
            schema: "cookbook",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_meal_plans", x => x.id);
                table.ForeignKey(
                    name: "FK_meal_plans_users_user_id",
                    column: x => x.user_id,
                    principalSchema: "cookbook",
                    principalTable: "users",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "meal_plan_slots",
            schema: "cookbook",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                week_day = table.Column<int>(type: "integer", nullable: false),
                meal_type = table.Column<int>(type: "integer", nullable: false),
                meal_plan_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_meal_plan_slots", x => x.id);
                table.ForeignKey(
                    name: "FK_meal_plan_slots_meal_plans_meal_plan_id",
                    column: x => x.meal_plan_id,
                    principalSchema: "cookbook",
                    principalTable: "meal_plans",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "meal_plan_items",
            schema: "cookbook",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                recipe_id = table.Column<Guid>(type: "uuid", nullable: false),
                servings = table.Column<int>(type: "integer", nullable: false),
                meal_plan_slot_id = table.Column<Guid>(type: "uuid", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_meal_plan_items", x => x.id);
                table.ForeignKey(
                    name: "FK_meal_plan_items_meal_plan_slots_meal_plan_slot_id",
                    column: x => x.meal_plan_slot_id,
                    principalSchema: "cookbook",
                    principalTable: "meal_plan_slots",
                    principalColumn: "id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_meal_plan_items_meal_plan_slot_id",
            schema: "cookbook",
            table: "meal_plan_items",
            column: "meal_plan_slot_id");

        migrationBuilder.CreateIndex(
            name: "IX_meal_plan_slots_meal_plan_id_week_day_meal_type",
            schema: "cookbook",
            table: "meal_plan_slots",
            columns: new[] { "meal_plan_id", "week_day", "meal_type" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_meal_plans_user_id",
            schema: "cookbook",
            table: "meal_plans",
            column: "user_id",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "meal_plan_items",
            schema: "cookbook");

        migrationBuilder.DropTable(
            name: "meal_plan_slots",
            schema: "cookbook");

        migrationBuilder.DropTable(
            name: "meal_plans",
            schema: "cookbook");
    }
}
