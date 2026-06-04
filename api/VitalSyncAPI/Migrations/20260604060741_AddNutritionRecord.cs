using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitalSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNutritionRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "nutrition_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    meal_type = table.Column<int>(type: "integer", nullable: false),
                    food_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    calories_kcal = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    protein_g = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    carbs_g = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    fat_g = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    confidence = table.Column<decimal>(type: "numeric(3,2)", nullable: false),
                    notes = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    measured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nutrition_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_nutrition_records_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nutrition_records_user_id",
                table: "nutrition_records",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nutrition_records");
        }
    }
}
