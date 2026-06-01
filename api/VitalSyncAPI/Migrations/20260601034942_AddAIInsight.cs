using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitalSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAIInsight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_user_profiles_user_id",
                table: "user_profiles");

            migrationBuilder.AddColumn<int>(
                name: "source",
                table: "health_records",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ai_insights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    health_record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    insights = table.Column<string>(type: "jsonb", nullable: false),
                    tips = table.Column<string>(type: "jsonb", nullable: false),
                    overall_assessment = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    disclaimer = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_insights", x => x.id);
                    table.ForeignKey(
                        name: "FK_ai_insights_health_records_health_record_id",
                        column: x => x.health_record_id,
                        principalTable: "health_records",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ai_insights_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_user_id",
                table: "user_profiles",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ai_insights_health_record_id",
                table: "ai_insights",
                column: "health_record_id");

            migrationBuilder.CreateIndex(
                name: "IX_ai_insights_user_id",
                table: "ai_insights",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_insights");

            migrationBuilder.DropIndex(
                name: "IX_user_profiles_user_id",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "source",
                table: "health_records");

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_user_id",
                table: "user_profiles",
                column: "user_id");
        }
    }
}
