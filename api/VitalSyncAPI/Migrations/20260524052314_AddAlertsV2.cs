using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitalSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAlertsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    health_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metric_type_id = table.Column<int>(type: "integer", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    triggered_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    acknowledged_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.id);
                    table.ForeignKey(
                        name: "FK_alerts_health_records_health_record_id",
                        column: x => x.health_record_id,
                        principalTable: "health_records",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_alerts_metric_types_metric_type_id",
                        column: x => x.metric_type_id,
                        principalTable: "metric_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_alerts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerts_health_record_id",
                table: "alerts",
                column: "health_record_id");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_metric_type_id",
                table: "alerts",
                column: "metric_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_user_id",
                table: "alerts",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerts");
        }
    }
}
