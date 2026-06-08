using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitalSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddWeeklyReport2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "weekly_reports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    week_start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    week_end = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    summary = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    metrics_analysis = table.Column<string>(type: "jsonb", nullable: false),
                    patterns = table.Column<string>(type: "jsonb", nullable: false),
                    recommendations = table.Column<string>(type: "jsonb", nullable: false),
                    nutrition_summary = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    disclaimer = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_weekly_reports", x => x.id);
                    table.ForeignKey(
                        name: "FK_weekly_reports_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_weekly_reports_user_id",
                table: "weekly_reports",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "weekly_reports");
        }
    }
}
