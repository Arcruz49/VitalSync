using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitalSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSOP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "health_conditions",
                columns: new[] { "id", "name" },
                values: new object[] { 17, "Síndrome do Ovário Policístico (SOP)" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "health_conditions",
                keyColumn: "id",
                keyValue: 17);
        }
    }
}
