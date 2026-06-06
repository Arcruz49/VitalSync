using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitalSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddNutritionStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "nutrition_records",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "nutrition_records");
        }
    }
}
