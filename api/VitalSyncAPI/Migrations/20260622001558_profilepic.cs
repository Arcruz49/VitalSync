using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitalSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class profilepic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "profile_pic",
                table: "user_profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profile_pic",
                table: "user_profiles");
        }
    }
}
